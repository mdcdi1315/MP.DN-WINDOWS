
using System;
using MP.Utilities;
using MP.Collections;
using System.Reflection;
using System.Diagnostics;
using System.Collections.Generic;
using MP.Annotations.CodeAnalysis;
using System.Diagnostics.CodeAnalysis;

namespace MP.Serialization
{
    internal static class SerializationManagerUtilities
    {
        internal sealed class SerializationManagerFieldInformation
        {
            private readonly FieldInfo originalinfo; // The original information of this field.
            private readonly System.String recordname; // The name of the field as it is expected on the record. Will be null if the fieldNameAttribute is not defined on the field.
            private readonly System.Type[] fieldbindings; // record bindings provided through the definition of the DerivedTypeBindingAttribute
            private readonly System.Object optionalvalue; // will be null if the attribute is not defined
            private readonly SerializationConstraintAttribute[] constraints; // The constraints to be applied to the field. Can be an empty array

            public SerializationManagerFieldInformation(FieldInfo fieldinfo)
            {
                originalinfo = fieldinfo;
                recordname = null;

                System.Boolean optionaldefined = false, bindingdefined = false;

                List<SerializationConstraintAttribute> constraints = new();

                foreach (var attribute in fieldinfo.GetCustomAttributes(true))
                {
                    if (attribute is FieldNameAttribute fna) {
                        recordname = fna.FieldName;
                    } else if (attribute is IOptionalFieldAttributeDataAccessor opt) {
                        if (bindingdefined) { continue; }
                        optionaldefined = true;
                        optionalvalue = opt.GetValue();
                        if (optionalvalue is null)
                        {
                            throw new SerializationException("Optional field default value cannot be null.");
                        }
                        else if (optionalvalue.GetType() != originalinfo.FieldType)
                        {
                            throw new SerializationException($"The optional field default value type does not agree with the type specified in the field type.\nFound {optionalvalue.GetType().FullName} while expected {originalinfo.FieldType.FullName}.");
                        }
                    } else if (attribute is DerivedTypeBindingAttribute binding) {
                        if (optionaldefined) { continue; }
                        bindingdefined = true;
                        if (!binding.AreAllDerivingFrom(originalinfo.FieldType))
                        {
                            throw new SerializationException($"Some types specified in the DerivedTypeBindingAttribute for the field {originalinfo.Name} are not deriving from {originalinfo.FieldType.FullName}.");
                        }
                        fieldbindings = binding.DerivedTypes;
                    } else if (attribute is SerializationConstraintAttribute sca) {
                        constraints.Add(sca);
                    } else {
                        DebugProvider.WriteLine($"SERMANAGER: [WARN] Unknown/unrecognized attribute applied to field: {attribute.GetType().FullName}");
                    }
                }

                if (originalinfo.FieldType.IsAbstract && originalinfo.FieldType.ImplementsInterface(typeof(ISerializableClass)) && !bindingdefined) {
                    throw new SerializationException("Field types being abstract classes implementing ISerializableClass not being marked with the DerivedTypeBindingAttribute are invalid.");
                }

                this.constraints = constraints.ToArray();
            }

            [NotNull]
            public Type FieldType => originalinfo.FieldType;

            [NotNull]
            public String ActualName => originalinfo.Name;

            [MaybeNull]
            public String RecordName => recordname;

            /// <summary>
            /// Computes the name of the field as it is expected to occur on the record declaration.
            /// </summary>
            public String Name => recordname ?? originalinfo.Name;

            [NotNull]
            public Type[] FieldDerivedTypeBindings => fieldbindings ?? Type.EmptyTypes;

            [MaybeNull]
            public Object DefaultValue => optionalvalue;

            [NotNull]
            public SerializationConstraintAttribute[] Constraints => constraints;

            [Throws(typeof(SerializationException))]
            public void ApplyConstraints(System.Object value, SerializedFieldType type, ConstraintApplicationTime time)
            {
                SerializationException except;
                foreach (var constraint in constraints)
                {
                    if (constraint.AppliesDuring == time || constraint.AppliesDuring == ConstraintApplicationTime.Both)
                    {
                        foreach (var app in constraint.AppliesTo)
                        {
                            if (app == type)
                            {
                                if (!constraint.IsSatisfied(value, out except)) {
                                    throw except ?? new SerializationException($"Constraint of type {constraint.GetType().FullName} failed to be satisfied to {value}.");
                                }
                                break;
                            }
                        }
                    }
                }
            }

            [DebuggerHidden]
            [DebuggerStepThrough]
            public void SetValue(System.Object objref, System.Object value)
            {
                try {
                    originalinfo.SetValue(objref, value);
                } catch (FieldAccessException fae) {
                    throw new SerializationException($"Unable to access the field named as {originalinfo.Name}.", fae);
                } catch (TargetException t) {
                    throw new SerializationException($"Unable to set the field named as {originalinfo.Name}.", t);
                }
            }

            [DebuggerHidden]
            [DebuggerStepThrough]
            public System.Object GetValue(System.Object objref)
            {
                try {
                    return originalinfo.GetValue(objref);
                } catch (FieldAccessException fae) {
                    throw new SerializationException($"Unable to access the field named as {originalinfo.Name}.", fae);
                } catch (TargetException t) {
                    throw new SerializationException($"Unable to set the field named as {originalinfo.Name}.", t);
                } catch (ArgumentException arg) {
                    throw new SerializationException($"Unable to set the field named as {originalinfo.Name}.", arg);
                }
            }

            [DebuggerHidden]
            [DebuggerStepThrough]
            public System.Object CreateObjectOfFieldType() => CreateObjectOfType(originalinfo.FieldType);
        }

        // A special placeholder class allowing to set and get the primitive numeric types as we wish to.
        // See GetAndSet method about how this class is used.
        private sealed class ExactTypeSetterAndGetter<T> 
        {
            public T Value;

            public ExactTypeSetterAndGetter() => Value = default;
        }

        public static void GenerateRecordInformation(Type typetoserialize, IDictionary<Type , IList<SerializationManagerFieldInformation>> fieldinformationcache)
        {
            if (fieldinformationcache.ContainsKey(typetoserialize)) { return; } // Cache already generated for this class type, return.

            Type ft;
            FieldInfo[] fieldstoinspect = typetoserialize.GetFields(BindingFlags.Public | BindingFlags.Instance);
            ArrayBasedList<SerializationManagerFieldInformation> infotemplist = new(fieldstoinspect.Length);

            SerializationManagerFieldInformation inf;
            
            foreach (var field in fieldstoinspect) 
            {
                inf = new(field);
                ft = field.FieldType;
                infotemplist.Add(inf);
                foreach (var binding in inf.FieldDerivedTypeBindings) { 
                    // Apply the binding types to the cache.
                    GenerateRecordInformation(binding, fieldinformationcache);
                }
                if (ft.ImplementsInterface(typeof(ISerializableClass))) {
                    // Generate additional record information if this field holds a serializable class.
                    GenerateRecordInformation(ft, fieldinformationcache);
                } else if (ft.IsTypeNonGenericMatch(typeof(IList<>))) {
                    Type t = ft.GenericTypeArguments[0];
                    if (t.ImplementsInterface(typeof(ISerializableClass))) {
                        GenerateRecordInformation(t, fieldinformationcache);
                    }
                } else if (ft.IsTypeNonGenericMatch(typeof(IDictionary<,>))) {
                    Type t = ft.GenericTypeArguments[1];
                    if (t.ImplementsInterface(typeof(ISerializableClass))) {
                        GenerateRecordInformation(t, fieldinformationcache);
                    }
                }
            }

            fieldinformationcache.Add(typetoserialize, infotemplist);
        }

        public static System.Boolean IsPrimitiveOrPrimitiveArray(Type t)
        {
            if (t.IsArray) {
                var te = t.GetElementType();
                return te.IsPrimitive || te == typeof(System.Boolean) || te == typeof(System.String);
            } else {
                return t.IsPrimitive || t == typeof(System.Boolean) || t == typeof(System.String);
            }
        }

        public static ITypeTranscoder GetTranscoder(SerializedFieldType serfieldtype , Type actualtype , IList<ITypeTranscoder> typetranscoders)
        {
            if (typetranscoders is null) { return null; }
            foreach (var transcoder in typetranscoders)
            {
                if (transcoder.SerializedType == serfieldtype)
                {
                    var t = transcoder.ActualType;
                    if (transcoder.StrictTypeMatch) {
                        if (t == actualtype) { return transcoder; }
                    } else {
                        if (t.IsInterface) {
                            if (actualtype.IsClass && !actualtype.IsAbstract && actualtype.ImplementsInterface(t)) {
                                return transcoder;
                            }
                        } else if (actualtype.IsTypeOrDerivesFrom(t)) {
                            return transcoder;
                        }
                    }
                }
            }
            return null;
        }

        public static ITypeTranscoder GetTranscoder(Type actualtype , IList<ITypeTranscoder> typetranscoders)
        {
            if (typetranscoders is null) { return null; }
            foreach (var transcoder in typetranscoders)
            {
                var t = transcoder.ActualType;
                if (transcoder.StrictTypeMatch) {
                    if (t == actualtype) { return transcoder; }
                } else {
                    if (t.IsInterface) {
                        if (actualtype.IsClass && !actualtype.IsAbstract && actualtype.ImplementsInterface(t)) {
                            return transcoder;
                        }
                    } else if (actualtype.IsTypeOrDerivesFrom(t)) {
                        return transcoder;
                    }
                }
            }
            return null;
        }

        public static System.Boolean IsPrimitiveOrPrimitiveArray(SerializedFieldType sft)
        {
            if (sft.HasFlag(SerializedFieldType.Array)) { sft &= ~SerializedFieldType.Array; }
            return sft == SerializedFieldType.MixedPrimitives || (sft >= SerializedFieldType.PRIMITIVE_TYPES_START && sft <= SerializedFieldType.PRIMITIVE_TYPES_END);
        }

        public static bool IsObjectOrArrayOfObjects(SerializedFieldType ft) => ft == SerializedFieldType.Object || (ft & ~SerializedFieldType.Array) == SerializedFieldType.Object;

        public static bool IsArrayAndExtractType(SerializedFieldType ft, out SerializedFieldType type)
        {
            if (ft.HasFlag(SerializedFieldType.Array)) {
                type = ft & ~SerializedFieldType.Array;
                return true;
            } else {
                type = SerializedFieldType.Empty;
                return false;
            }
        }

        public static bool IsListAndExtractType(SerializedFieldType ft, out SerializedFieldType type)
        {
            if (ft.HasFlag(SerializedFieldType.List)) {
                type = ft & ~SerializedFieldType.List;
                return true;
            } else {
                type = SerializedFieldType.Empty;
                return false;
            }
        }

        public static bool IsStringDictAndExtractType(SerializedFieldType ft, out SerializedFieldType type)
        {
            if (ft.HasFlag(SerializedFieldType.StrictStringDictionary)) {
                type = ft & ~SerializedFieldType.StrictStringDictionary;
                return true;
            } else {
                type = SerializedFieldType.Empty;
                return false;
            }
        }

        public static object CreateListObject(Array data_array) => Activator.CreateInstance(
            typeof(ArrayBasedList<>).MakeGenericType(data_array.GetType().GetElementType()),
            data_array
        );

        public static Array CreateArrayFromListObject(object list_object)
        {
            var t = list_object.GetType();
            int count = (int)t.InvokeMember("Count", BindingFlags.Instance | BindingFlags.Public | BindingFlags.GetProperty, null, list_object, Array.Empty<Object>());
            Array a = Array.CreateInstance(t.GetGenericArguments()[0], count);
            System.Collections.IEnumerator e = ((System.Collections.IEnumerable)list_object).GetEnumerator();
            try {
                long I = 0L;
                while (e.MoveNext()) { a.SetValue(e.Current, I); }
                return a;
            } finally {
                if (e is IDisposable d) { d.Dispose(); }
            }
        }

        public static Record[] CreateRecordsFromStringStrictDictionary(object dict_object)
        {
            var t = dict_object.GetType();

            MethodInfo mi = typeof(SerializationManagerUtilities).GetMethod("CreateRecordForSSD").MakeGenericMethod(t.GenericTypeArguments[1]);

            Record[] rcs = new Record[(int)t.InvokeMember("Count", BindingFlags.Instance | BindingFlags.Public | BindingFlags.GetProperty, null, dict_object, Array.Empty<Object>())];

            System.Collections.IEnumerator e = ((System.Collections.IEnumerable)dict_object).GetEnumerator();

            try {
                long I = 0L;
                while (e.MoveNext()) { rcs[I++] = (Record)mi.Invoke(null, new object[] { e.Current }); }
                return rcs;
            } finally {
                if (e is IDisposable d) { d.Dispose(); }
            }
        }

        // Strict-string dictionaries are stored as a Record array 
        // containing two fields: The 'key' and 'value' members.
        public static object CreateStringStrictDictionary(Record[] keyvaluepairs, Type typeinfo)
        {
            Type dict_type = typeof(Dictionary<,>).MakeGenericType(typeof(String), typeinfo);
            var mi = dict_type.GetMethod("Add", new[] { typeof(string), typeinfo });
            if (mi is null) {
                throw new SerializationException("Cannot locate method add in Dictionary class. Lookup failed.");
            } else {
                object ret = Activator.CreateInstance(dict_type, keyvaluepairs.Length);

                SerializedField k, v;
                foreach (Record value in keyvaluepairs)
                {
                    if ((k = value.LookupField("key")) is null) {
                        throw new SerializationException("No field named as key found in the encoded form of the dictionary!");
                    } else if ((v = value.LookupField("value")) is null) {
                        throw new SerializationException("No field named as value found in the encoded form of the dictionary!");
                    } else {
                        mi.Invoke(ret, new object[] { k.Value, SetAndGet(typeinfo, v.Value) });
                    }
                }
                return ret;
            }
        }

        [DebuggerHidden]
        [DebuggerStepThrough]
        public static Object CreateObjectOfType(Type type)
        {
            try {
                return Activator.CreateInstance(type, true);
            } catch (MethodAccessException mae) {
                throw new SerializationException($"Cannot create an object of type {type.FullName} because the constructor cannot be accessed.", mae);
            } catch (MissingMethodException mme) {
                throw new SerializationException($"Cannot create an object of type {type.FullName} because the constructor cannot be found.", mme);
            } catch (TargetInvocationException tie) {
                throw new SerializationException($"Cannot create an object of type {type.FullName} because the constructor threw an exception.", tie.InnerException);
            } catch (TypeLoadException tle) {
                throw new SerializationException($"Cannot create an object of type {type.FullName} because the type is not loaded in the environment.", tle);
            }
        }

        public static Object SetAndGet(Type wanted , Object actualvalue)
        {
            var t = typeof(ExactTypeSetterAndGetter<>).MakeGenericType(wanted);
            var obj = t.GetConstructor(Type.EmptyTypes).Invoke(Array.Empty<Object>());
            var f = t.GetField("Value"); // Will always succeed.
            f.SetValue(obj, actualvalue);
            return f.GetValue(obj);
        }

        private static Record CreateRecordForSSD<T>(KeyValuePair<string, T> kvp) => new(new[] { new SerializedField("key", kvp.Key), new SerializedField("value", kvp.Value) });
    }
}