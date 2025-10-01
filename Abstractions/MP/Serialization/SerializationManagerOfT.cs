
using System;
using MP.Utilities;
using System.Threading;
using System.Collections.Generic;
using MP.Annotations.CodeAnalysis;
using System.Diagnostics.CodeAnalysis;

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
        private IRecordWriter writer;
        private IRecordReader reader;
        private List<ITypeTranscoder> typetranscoders;
        private Dictionary<Type, IList<SerializationManagerUtilities.SerializationManagerFieldInformation>> typecache;

        /// <summary>
        /// Creates a new empty instance of the <see cref="SerializationManager{T}"/> class. <br />
        /// The constructor does also initialize the serialization cache so be prepared for premature <see cref="SerializationException"/>s.
        /// </summary>
        /// <exception cref="SerializationException">The serialization cache could not be initialized.</exception>
        [Throws(typeof(SerializationException))]
        public SerializationManager()
        {
            reader = null;
            writer = null;
            typetranscoders = null;
            typecache = new(15); // Assume that many different class fields are used - specify a capacity of 15 so.
            // Generate the record information cache
            SerializationManagerUtilities.GenerateRecordInformation(typeof(T), typecache);
            // Optimize cache size after the cache has been successfully determined.
            // It will not be changed again throughout the lifetime of the manager.
            typecache.TrimExcess(); 
        }

        /// <summary>
        /// Gets or sets the record reader to use throughout the entire life of this <see cref="SerializationManager{T}"/> class instance.
        /// </summary>
        /// <exception cref="InvalidOperationException">Attempted to set a new record reader but a record reader was already provided to the instance.</exception>
        public IRecordReader Reader
        {
            [return: MaybeNull]
            get => reader;
            set {
                ArgumentNullException.ThrowIfNull(value);
                if (reader is not null)
                {
                    throw new InvalidOperationException("A serialized class reader is already provided.");
                }
                reader = value;
            }
        }

        /// <summary>
        /// Gets or sets the record writer to use throughout the entire life of this <see cref="SerializationManager{T}"/> class instance.
        /// </summary>
        /// <exception cref="InvalidOperationException">Attempted to set a new record writer but a record writer was already provided to the instance.</exception>
        public IRecordWriter Writer
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
        /// Binds a type transcoder to this <see cref="SerializationManager{T}"/> instance.
        /// </summary>
        /// <param name="transcoder">The <see cref="ITypeTranscoder"/> instance to associate with this serialization manager.</param>
        /// <exception cref="ArgumentNullException"><paramref name="transcoder"/> was <see langword="null"/>.</exception>
        public void AddTranscoder(ITypeTranscoder transcoder)
        {
            ArgumentNullException.ThrowIfNull(transcoder);
            (typetranscoders ??= new(10)).Add(transcoder);
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
            writer.WriteNew(destination, EncodeRecord(obj));
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
            reader.InitializeForNewPayload(source);
            try {
                DecodeRecord(obj, reader.Payload);
            } finally {
                reader.EndPayloadDecoding();
            }
        }

        #region Private implementation details

        private Record EncodeRecord(System.Object objectref)
        {
            var fielddata = LookupCache(objectref.GetType());
            Record.Builder builder = new(fielddata.Count);

            System.Object value;
            System.Boolean isarray;
            Type inspecting , temp;
            SerializedField sf;

            foreach (var fieldi in fielddata)
            {
                if (SerializationManagerUtilities.IsPrimitiveOrPrimitiveArray(fieldi.FieldType))
                {
                    ITypeTranscoder tt = SerializationManagerUtilities.GetTranscoder(fieldi.FieldType, typetranscoders);
                    if (tt is not null) {
                        if (tt.SerializedType == SerializedFieldType.Object ||
                            (tt.SerializedType & SerializedFieldType.Array) == SerializedFieldType.Object) {
                            throw new InvalidTypeTranscoderConfigurationException("The type transcoder must only accept primitive types, or arrays of them.", tt);
                        }
                        value = tt.Encode(fieldi.GetValue(objectref));
                    } else {
                        value = fieldi.GetValue(objectref);
                    }
                } else {
                    value = fieldi.GetValue(objectref);
                }

                if (value is null)
                {
                    builder.Add(new SerializedField(fieldi.Name, null));
                    continue;
                }

                temp = value.GetType();

                if (isarray = temp.IsArray) {
                    inspecting = temp.GetElementType();
                } else {
                    inspecting = temp;
                }

                if (inspecting.ImplementsInterface(typeof(ISerializableClass)))
                {
                    if (isarray) {
                        Array arr = fieldi.GetValue(objectref) as Array;
                        Record[] records = new Record[arr.LongLength];
                        for (long I = 0; I < arr.LongLength; I++)
                        {
                            records[I] = EncodeRecord(arr.GetValue(I));
                        }
                        sf = new(fieldi.Name, records);
                        fieldi.ApplyConstraints(arr, sf.Type, ConstraintApplicationTime.Writing);
                        builder.Add(sf);
                    } else {
                        System.Object v = fieldi.GetValue(objectref);
                        sf = new SerializedField(fieldi.Name, EncodeRecord(v));
                        fieldi.ApplyConstraints(v, sf.Type, ConstraintApplicationTime.Writing);
                        builder.Add(sf);
                    }
                } else {
                    sf = new SerializedField(fieldi.Name, value);
                    fieldi.ApplyConstraints(sf.Value, sf.Type, ConstraintApplicationTime.Writing);
                    builder.Add(sf);
                }
            }

            return builder.Build();
        }

        private void DecodeRecord(System.Object objectref , Record retrieved)
        {
            foreach (var fieldi in LookupCache(objectref.GetType()))
            {
                var fieldname = fieldi.Name;
                foreach (var r in retrieved)
                {
                    if (fieldname == r.Name) 
                    {  
                        SerializedFieldType sft = r.Type;
                        System.Boolean isarray = sft.HasFlag(SerializedFieldType.Array);
                        if (SerializationManagerUtilities.IsPrimitiveOrPrimitiveArray(sft))
                        {
                            System.Object value = r.Value;

                            if (isarray && (sft & ~SerializedFieldType.Array) == SerializedFieldType.MixedPrimitives)
                            {
                                // An array of primitive numbers of unknown types. We need to translate each one as being of the underlying type.
                                Array original = value as Array;
                                var ft = fieldi.FieldType.GetElementType();
                                Array finalarray = Array.CreateInstance(ft, original.LongLength);
                                for (long I = 0; I < original.LongLength; I++)
                                {
                                    finalarray.SetValue(SerializationManagerUtilities.SetAndGet(ft, original.GetValue(I)), I);
                                }
                                fieldi.SetValue(objectref, finalarray);
                            } else {
                                // A primitive or array of known primitives. Thus, we can directly set to the underlying field.
                                fieldi.SetValue(objectref, value);
                            }
                            value = fieldi.GetValue(objectref); // HACK: I am doing this so that widening conversions can appropriately work.

                            // Type transcoding works only on top of the primitive types and their arrays
                            ITypeTranscoder tt = SerializationManagerUtilities.GetTranscoder(sft, fieldi.FieldType, typetranscoders);
                            if (tt is not null) {
                                // A type transcoder was found for this field, use it
                                fieldi.SetValue(objectref , value = tt.Decode(value, fieldi.FieldType));
                                sft = tt.SerializedType;
                                // Update the value, see below why
                                isarray = sft.HasFlag(SerializedFieldType.Array);
                            }

                            // Apply the constraints now.
                            fieldi.ApplyConstraints(value, sft, ConstraintApplicationTime.Reading);
                        } else if (sft == SerializedFieldType.Object || (sft & ~SerializedFieldType.Array) == SerializedFieldType.Object) 
                        {
                            // We have another Record to decode.
                            // We have two seperate cases, one being an array of records, or it is just another record.
                            if (isarray) {
                                Record[] records = r.RecordArrayValue;
                                Array arr = Array.CreateInstance(fieldi.FieldType, records.LongLength);
                                for (long I = 0; I < records.LongLength; I++)
                                {
                                    // We must apply DecodeInnerRecord to each one record.
                                    arr.SetValue(DecodeInnerRecord(records[I], fieldi , true) , I);
                                }
                                fieldi.ApplyConstraints(arr , sft, ConstraintApplicationTime.Reading);
                                fieldi.SetValue(objectref, arr);
                            } else {
                                System.Object dc = DecodeInnerRecord(r.RecordValue, fieldi);
                                fieldi.ApplyConstraints(dc , sft, ConstraintApplicationTime.Reading);
                                fieldi.SetValue(objectref, dc);
                            }
                        } else {
                            // If reached this, it means that the serialization reader has somehow returned invalid
                            // data and does not implement correctly the serialization model. We should throw.

                            throw new InvalidSerializationReaderLayoutException($"Cannot determine what to do for field {r.Name} (Actual: {fieldi.ActualName}) with value {r.Value}. Do you have coded correctly your reader implementation?");
                        }

                        fieldname = null; // Indicating that field set was succeeded. See below why.
                        break;
                    }
                }
                if (fieldname is not null) {
                    // If the field was not set successfully and we have an optional value for it, set it.
                    // Otherwise fail.
                    if (fieldi.DefaultValue is not null) {
                        fieldi.SetValue(objectref, fieldi.DefaultValue); // Notice that on optional setting , the constraints do not apply for various reasons.
                    } else {
                        throw new SerializationException($"The field with name {fieldname} is required and cannot be found in the serialized record.");
                    }
                }
            }
        }

        private Object DecodeInnerRecord(Record value , SerializationManagerUtilities.SerializationManagerFieldInformation originalfieldinfo , System.Boolean arraymode = false)
        {
            System.Object o;
            Type fieldtype = arraymode ? originalfieldinfo.FieldType.GetElementType() : originalfieldinfo.FieldType;
            Type[] bindings = originalfieldinfo.FieldDerivedTypeBindings;
            if (bindings.Length == 0) {
                o = SerializationManagerUtilities.CreateObjectOfType(fieldtype);
                DecodeRecord(o, value); // No bindings found or detected thus directly decode.
                return o;
            } else { // We must do type binding checks
                // For this to work we must select a candidate that has the most fields as 'defined'.
                // That's why we will select that class type that defines the most fields.

                // A 'max' variable indicating the best candidate
                // By default, it is set to the count of the base type (so that the derived entries can actually make a match).
                int bestcandidatefieldcount = LookupCache(fieldtype).Count; 
                int bestcandidatepos = -1; // The exact position of the candidate in the 'bindings' array. -1 indicates that no such candidate was found.
                
                for (int I = 0; I < bindings.Length; I++)
                {
                    int fc = 0; // Field count
                    var fi = LookupCache(bindings[I]);
                    foreach (var recfield in value)
                    {
                        foreach (var fieldinf in fi)
                        {
                            if (recfield.Name == fieldinf.Name) {
                                fc++;
                                break; // Found the field in the record, continue to see other fields as well.
                            }
                        }
                    }
                    // Check, is this a better candidate than the previous one defined?
                    if (fc > bestcandidatefieldcount) {
                        bestcandidatefieldcount = fc;
                        bestcandidatepos = I;
                    }
                }
                
                if (bestcandidatepos > -1) {
                    // We have a best candidate, use that instead to decode the record.
                    o = SerializationManagerUtilities.CreateObjectOfType(bindings[bestcandidatepos]);
                } else {
                    // We do not have such , it is possibly the pre-defined object type of the field.
                    o = SerializationManagerUtilities.CreateObjectOfType(fieldtype);
                }

                // Now, with all these information , we can decode the record.
                DecodeRecord(o, value);
                
                return o;
            }
        }

        private IList<SerializationManagerUtilities.SerializationManagerFieldInformation> LookupCache(Type type)
        {
            if (typecache.TryGetValue(type, out var list)) {
                return list;
            }
            throw new SerializationException($"Cache NOT COMPLETE!!!! The type {type.FullName} was not found in the serialization cache.");
        }

        #endregion

        /// <summary>
        /// Disposes this <see cref="SerializationManager{T}"/>. <br />
        /// Thread-safe.
        /// </summary>
        public void Dispose()
        {
            try {
                Monitor.Enter(this);
                writer?.Dispose();
                writer = null;
                reader?.Dispose();
                reader = null;
                typecache?.Clear();
                typecache = null;
                typetranscoders?.Clear();
                typetranscoders = null;
            } finally {
                Monitor.Exit(this);
            }
        }
    }
}