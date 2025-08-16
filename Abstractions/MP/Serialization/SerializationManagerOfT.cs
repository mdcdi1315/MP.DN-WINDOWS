

using System;
using MP.Utilities;
using System.Reflection;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace MP.Serialization
{
    /// <summary>
    /// Provides (manages) serialization sessions and services around the specified class type. <br />
    /// The class type must not be a <see cref="Nullable{T}"/>.
    /// </summary>
    /// <typeparam name="T">
    /// The type implementing the <see cref="ISerializableClass"/> 
    /// interface, indicating that the specified type can be handled 
    /// by the serialization services.</typeparam>
    public sealed class SerializationManager<T> : IDisposable
        where T : notnull, ISerializableClass
    {
        private System.Type typeinfo;
        private ISerializedClassReader reader;
        private ISerializedClassWriter writer;
        private Dictionary<System.String, FieldInfo> fieldinfocache;

        /// <summary>
        /// Creates a new empty instance of the <see cref="SerializationManager{T}"/> class.
        /// </summary>
        public SerializationManager()
        {
            typeinfo = typeof(T);
            reader = null;
            writer = null;
            fieldinfocache = null;
        }

        /// <summary>
        /// Gets or sets the serialized class reader to use throughout the entire life of this <see cref="SerializationManager{T}"/> class instance.
        /// </summary>
        public ISerializedClassReader Reader
        {
            [return: MaybeNull]
            get => reader;
            set
            {
                ArgumentNullException.ThrowIfNull(value);
                if (reader is not null)
                {
                    throw new InvalidOperationException("A serialized class reader is already provided.");
                }
                reader = value;
            }
        }

        /// <summary>
        /// Gets or sets the serialized class writer to use throughout the entire life of this <see cref="SerializationManager{T}"/> class instance.
        /// </summary>
        public ISerializedClassWriter Writer
        {
            [return: MaybeNull]
            get => writer;
            set
            {
                ArgumentNullException.ThrowIfNull(value);
                if (writer is not null)
                {
                    throw new InvalidOperationException("A serialized class writer is already provided.");
                }
                writer = value;
            }
        }

        /// <summary>
        /// Serializes an object with the current serialized class writer to the specified destination stream.
        /// </summary>
        /// <param name="obj">The object to be written in serialized form in <paramref name="destination"/>.</param>
        /// <param name="destination">The serialized object data written with the specified <see cref="Writer"/> instance.</param>
        /// <exception cref="InvalidOperationException">A valid <see cref="Writer"/> instance is not provided yet.</exception>
        /// <exception cref="SerializationException">A serialization exception occured.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="obj"/> is null or/and <paramref name="destination"/> is null.</exception>
        public void Serialize(T obj, System.IO.Stream destination)
        {
            ArgumentNullException.ThrowIfNull(obj);
            ArgumentNullException.ThrowIfNull(destination);
            if (writer is null)
            {
                throw new InvalidOperationException("A serialized class writer is required, but such a writer is not provided yet.");
            }
            writer.Initialize(destination);
            try {
                EncodeClass(obj, writer, typeinfo);
            } finally {
                writer.FinalizeWriteOp();
            }
        }

        /// <summary>
        /// Deserializes an object with the current serialized class reader from the specified source stream.
        /// </summary>
        /// <param name="obj">The object reference where the deserialized data will be written to.</param>
        /// <param name="source">The source stream to decode the object from.</param>
        /// <exception cref="SerializationException">A serialization exception occured.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="obj"/> is null or/and <paramref name="source"/> is null.</exception>
        /// <exception cref="InvalidOperationException">A valid <see cref="Reader"/> instance is not provided yet.</exception>
        public void Deserialize(T obj, System.IO.Stream source)
        {
            ArgumentNullException.ThrowIfNull(obj);
            ArgumentNullException.ThrowIfNull(source);
            if (reader is null)
            {
                throw new InvalidOperationException("A serialized class reader is required, but such a reader is not provided yet.");
            }
            BuildFieldInfoCache();
            reader.Initialize(source);
            DecodeClass(obj, reader, fieldinfocache);
        }

        #region Private implementation details

        private static void EncodeClass(System.Object obj, ISerializedClassWriter writer, Type typeinfo)
        {
            SerializedFieldInformation sfi;
            System.Object value;
            SerializationException serexcept;
            foreach (var f in typeinfo.GetFields(BindingFlags.Public | BindingFlags.Instance))
            {
                sfi = EncodeInformation(f);
                value = f.GetValue(obj);
                if (IsPrimitiveOrPrimitiveArray(sfi.Type))
                {
                    if (GetAndApplyConstraints(f, sfi, value, ConstraintApplicationTime.Writing, out serexcept))
                    {
                        writer.AddField(sfi, value);
                    }
                    else
                    {
                        throw new SerializationException("Specified class instance cannot be serialized because one of the constraints were failed.", serexcept);
                    }
                }
                else
                {
                    var wr = writer.GetEmptyWriter();
                    EncodeClass(value, wr, f.FieldType);
                    writer.AddField(sfi, wr);
                }
            }
        }

        private static void DecodeClass(System.Object obj, ISerializedClassReader reader, Dictionary<System.String, FieldInfo> fieldinfocache)
        {
            SerializationException except;
            SerializedFieldInformation sfi;
            while (reader.MoveNext())
            {
                sfi = reader.GetFieldInformation();
                if (fieldinfocache.TryGetValue(sfi.Name, out var fi) == false)
                {
                    throw new SerializationException($"Cannot find an associated field in the field table of the class.\nField Name: {sfi.Name}");
                }
                var ft = fi.FieldType;
                System.Object val = reader.DecodeField();
                if (sfi.Type.HasFlag(SerializedFieldType.StrictStringDictionary)) {
                    throw new NotSupportedException("This bit flag is not currently supported.");
                } else if (sfi.Type.HasFlag(SerializedFieldType.Array)) {
                    if (val is not Array a) {
                        throw new InvalidSerializationReaderLayoutException("Expected an ISerializedClassReader array, but no such array was provided.");
                    }
                    int len = a.GetLength(0);
                    Array a2 = Array.CreateInstance(ft.GetElementType(), len);
                    if ((sfi.Type & ~SerializedFieldType.Array) == SerializedFieldType.Object) {
                        System.Object el;
                        for (int I = 0; I < len; I++)
                        {
                            el = a.GetValue(I);
                            if (el is ISerializedClassReader anotherreader) {
                                try {
                                    System.Object reference = Activator.CreateInstance(ft, true);
                                    // Apply recursive references
                                    DecodeClass(reference, anotherreader, GetFieldInfoCacheFor(ft));
                                    a2.SetValue(reference, I);
                                } catch (TargetInvocationException tie) {
                                    throw new SerializationException($"Cannot apply an object for the field named as {sfi.Name} because the object of type {fi.FieldType.FullName} cannot be created due to an error in it's internal constructor.", tie.InnerException);
                                } catch (MemberAccessException mae) {
                                    throw new SerializationException($"Cannot apply an object for the field named as {sfi.Name} because the object of type {fi.FieldType.FullName} cannot be created because of a constructor access exception.", mae);
                                }
                            } else {
                                throw new InvalidSerializationReaderLayoutException("Expected to return a new ISerializedClassReader instance, but no such instance was found.");
                            }
                        }
                    } else {
                        for (int I = 0; I < len; I++)
                        {
                            a2.SetValue(a.GetValue(I), I);
                        }
                    }
                    fi.SetValue(obj, a2);
                } else if (sfi.Type == SerializedFieldType.Object) {
                    if (val is ISerializedClassReader anotherreader) {
                        if (ft.ImplementsInterface(typeof(ISerializableClass)) == false)
                        {
                            throw new SerializationException($"The type named as {ft.FullName} does not implement the ISerializableClass interface.");
                        }
                        try {
                            System.Object reference = Activator.CreateInstance(ft, true);
                            // Apply recursive references
                            DecodeClass(reference, anotherreader, GetFieldInfoCacheFor(ft));
                            fi.SetValue(obj, reference);
                        } catch (TargetInvocationException tie) {
                            throw new SerializationException($"Cannot apply an object for the field named as {sfi.Name} because the object of type {fi.FieldType.FullName} cannot be created due to an error in it's internal constructor.", tie.InnerException);
                        } catch (MemberAccessException mae) {
                            throw new SerializationException($"Cannot apply an object for the field named as {sfi.Name} because the object of type {fi.FieldType.FullName} cannot be created because of a constructor access exception.", mae);
                        }
                    } else {
                        throw new InvalidSerializationReaderLayoutException("Expected to return a new ISerializedClassReader instance, but no such instance was found.");
                    }
                } else {
                    if (sfi.Type.WillMostLikelyMatchWith(ft) == false)
                    {
                        throw new SerializationException($"Cannot serialize type of {ft.FullName} because it cannot correspond losslessly to {sfi.Type}.");
                    }
                    if (GetAndApplyConstraints(fi, sfi, val, ConstraintApplicationTime.Reading, out except) == false)
                    {
                        throw new SerializationException("Cannot apply an constraint for the current class field.", except);
                    }
                    fi.SetValue(obj, val);
                }
            }
        }

        private void BuildFieldInfoCache()
        {
            if (fieldinfocache is not null) { return; }
            fieldinfocache = GetFieldInfoCacheFor(typeinfo);
        }

        private static Dictionary<System.String, FieldInfo> GetFieldInfoCacheFor(Type type)
        {
            FieldNameAttribute fnifexist;
            Dictionary<System.String, FieldInfo> dict = new(4);
            foreach (var field in type.GetFields(BindingFlags.Public | BindingFlags.Instance))
            {
                fnifexist = field.GetCustomAttribute<FieldNameAttribute>();
                dict.Add((fnifexist is not null) ? fnifexist.FieldName : field.Name, field);
            }
            return dict;
        }

        private static System.Boolean IsPrimitiveOrPrimitiveArray(SerializedFieldType sft)
        {
            if (sft.HasFlag(SerializedFieldType.Array))
            {
                sft &= ~SerializedFieldType.Array;
            }
            return sft >= SerializedFieldType.PRIMITIVE_TYPES_START && sft <= SerializedFieldType.PRIMITIVE_TYPES_END;
        }

        private static SerializedFieldType EncodeFieldType(Type fieldtype)
        {
            static SerializedFieldType GetSimpleType(System.Type ft) => ft.FullName switch
            {
                "System.String" => SerializedFieldType.String,
                "System.Boolean" => SerializedFieldType.Boolean,
                "System.Byte" => SerializedFieldType.Byte,
                "System.SByte" => SerializedFieldType.SByte,
                "System.Int16" => SerializedFieldType.Int16,
                "System.UInt16" => SerializedFieldType.UInt16,
                "System.Int32" => SerializedFieldType.Int32,
                "System.UInt32" => SerializedFieldType.UInt32,
                "System.Int64" => SerializedFieldType.Int64,
                "System.UInt64" => SerializedFieldType.UInt64,
                "System.Single" => SerializedFieldType.Single,
                "System.Double" => SerializedFieldType.Double,
                _ => 0
            };
            if (fieldtype.IsArray)
            {
                return GetSimpleType(fieldtype.GetElementType()) | SerializedFieldType.Array;
            }
            else if (fieldtype.ImplementsInterface(typeof(ISerializableClass)))
            {
                return SerializedFieldType.Object;
            }
            else
            {
                return GetSimpleType(fieldtype);
            }
        }

        private static SerializedFieldInformation EncodeInformation(FieldInfo f)
        {
            FieldNameAttribute fnifexist = f.GetCustomAttribute<FieldNameAttribute>();
            return new(
                    (fnifexist is not null) ? fnifexist.FieldName : f.Name,
                    EncodeFieldType(f.FieldType)
                );
        }

        private static IEnumerable<SerializationConstraintAttribute> GetConstraints(FieldInfo f)
        {
            foreach (var attr in f.GetCustomAttributes())
            {
                if (attr is SerializationConstraintAttribute sa) { yield return sa; }
            }
        }

        private delegate System.Boolean IsSatisfiedConstraintDelegate(SerializationConstraintAttribute attr, System.Object val, out SerializationException except);

        private static System.Boolean EnumerateConstraints(
            FieldInfo f,
            System.Object value,
            SerializedFieldType sft,
            ConstraintApplicationTime apptime,
            IsSatisfiedConstraintDelegate @delegate,
            out SerializationException except)
        {
            except = null;
            System.Boolean cannotbeappliedstop;
            foreach (var constraint in GetConstraints(f))
            {
                if (constraint.AppliesDuring == apptime || constraint.AppliesDuring == ConstraintApplicationTime.Both) { continue; }
                cannotbeappliedstop = true;
                foreach (var applied in constraint.AppliesTo)
                {
                    if (applied == sft)
                    {
                        cannotbeappliedstop = false;
                        break;
                    }
                }
                if (cannotbeappliedstop) { continue; }
                if (@delegate(constraint, value, out except) == false)
                {
                    except ??= new SerializationException($"Constraint of type {constraint.GetType().Name} failed for object with value {value}.");
                    return false;
                }
            }
            return true;
        }

        private static System.Boolean ArrayConstraintTest(SerializationConstraintAttribute attr, System.Object value, out SerializationException except)
        {
            except = null;
            SerializationException e;
            Array arr = value as Array;
            int len = arr.GetLength(0);
            for (int I = 0; I < len; I++)
            {
                if (attr.IsSatisfied(arr.GetValue(I), out e) == false)
                {
                    except = new($"Serialization constraint failed for the array element at zero-based index {I}.", e);
                    return false;
                }
            }
            return true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static System.Boolean ElementConstraintTest(SerializationConstraintAttribute attr, System.Object value, out SerializationException except) => attr.IsSatisfied(value, out except);

        private static System.Boolean GetAndApplyConstraints(FieldInfo f,
            SerializedFieldInformation sfi, System.Object value,
            ConstraintApplicationTime apptime,
            out SerializationException except)
        {
            System.Boolean isarray;
            SerializedFieldType sft;
            except = null;
            if (isarray = sfi.Type.HasFlag(SerializedFieldType.Array))
            {
                sft = sfi.Type & ~SerializedFieldType.Array;
            }
            else
            {
                sft = sfi.Type;
            }
            return EnumerateConstraints(f, value, sft, apptime, isarray ? ArrayConstraintTest : ElementConstraintTest, out except);
        }

        #endregion

        /// <summary>
        /// Disposes this <see cref="SerializationManager{T}"/> instance, destroying any reader and writer instances used. <br />
        /// Must be called after all threads involving serialization operations have been completed.
        /// </summary>
        public void Dispose()
        {
            typeinfo = null;
            reader?.Dispose();
            reader = null;
            writer?.Dispose();
            writer = null;
        }
    }
}