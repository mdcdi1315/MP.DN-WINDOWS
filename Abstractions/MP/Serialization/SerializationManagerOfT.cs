
using System;
using MP.Utilities;
using MP.Collections;
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
    public sealed class SerializationManager<T> : IDisposable, ISyncronized
        where T : notnull, ISerializableClass
    {
        private IRecordWriter writer;
        private IRecordReader reader;
        private SingleLinkedList<ITypeTranscoder> typetranscoders;
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
            (typetranscoders ??= new()).Add(transcoder);
        }

        /// <summary>
        /// Serializes an object with the current serialized class writer to the specified destination stream.
        /// </summary>
        /// <param name="obj">The object to be written in serialized form in <paramref name="destination"/>.</param>
        /// <param name="destination">The serialized object data written with the specified <see cref="Writer"/> instance.</param>
        /// <exception cref="InvalidOperationException">A valid <see cref="Writer"/> instance is not provided yet.</exception>
        /// <exception cref="SerializationException">A serialization exception occured.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="obj"/> is null or/and <paramref name="destination"/> is null.</exception>
        public void Serialize(T obj, IO.DataStream destination)
        {
            ArgumentNullException.ThrowIfNull(obj);
            ArgumentNullException.ThrowIfNull(destination);
            if (writer is null) {
                throw new InvalidOperationException("A serialized class writer is required, but such a writer is not provided yet.");
            } else {
                Monitor.Enter(writer);
                try {
                    writer.WriteNew(destination, EncodeRecord(obj));
                } finally {
                    Monitor.Exit(writer);
                }
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
        public void Deserialize(T obj, IO.DataStream source)
        {
            ArgumentNullException.ThrowIfNull(obj);
            ArgumentNullException.ThrowIfNull(source);
            if (reader is null) {
                throw new InvalidOperationException("A serialized class reader is required, but such a reader is not provided yet.");
            } else {
                Monitor.Enter(reader);
                try {
                    reader.InitializeForNewPayload(source);
                    try {
                        DecodeRecord(obj, reader.Payload);
                    } finally {
                        reader.EndPayloadDecoding();
                    }
                } finally {
                    Monitor.Exit(reader);
                }
            }
        }

        #region Private implementation details

        private SerializedField EncodeField(System.Object objectref, SerializationManagerUtilities.SerializationManagerFieldInformation field_info)
        {
            SerializedField sf;
            System.Object value;
            if (SerializationManagerUtilities.IsPrimitiveOrPrimitiveArray(field_info.FieldType)) {
                ITypeTranscoder tt = SerializationManagerUtilities.GetTranscoder(field_info.FieldType, typetranscoders);
                if (tt is not null) {
                    if (SerializationManagerUtilities.IsObjectOrArrayOfObjects(tt.SerializedType)) {
                        throw new InvalidTypeTranscoderConfigurationException("The type transcoder must only accept primitive types, or arrays of them.", tt);
                    } else {
                        value = tt.Encode(field_info.GetValue(objectref));
                    }
                } else {
                    value = field_info.GetValue(objectref);
                }
            } else {
                value = field_info.GetValue(objectref);
            }

            if (value is null) {
                return new SerializedField(field_info.Name, null);
            }

            System.Boolean isarray;
            System.Type temp = value.GetType(), inspecting = (isarray = temp.IsArray) ? temp.GetElementType() : temp;

            if (inspecting.ImplementsInterface(typeof(ISerializableClass)))
            {
                if (isarray) {
                    Array arr = field_info.GetValue(objectref) as Array;
                    Record[] records = new Record[arr.LongLength];
                    for (long I = 0; I < arr.LongLength; I++)
                    {
                        records[I] = EncodeRecord(arr.GetValue(I));
                    }
                    sf = new(field_info.Name, records);
                    field_info.ApplyConstraints(arr, sf.Type, ConstraintApplicationTime.Writing);
                    return sf;
                } else {
                    sf = new SerializedField(field_info.Name, EncodeRecord(field_info.GetValue(objectref)));
                }
            } else if (inspecting.IsTypeNonGenericMatch(typeof(IList<>))) {
                sf = new SerializedField(field_info.Name, field_info.GetValue(objectref));
            } else if (inspecting.IsTypeNonGenericMatch(typeof(IDictionary<,>))) {
                System.Object v = field_info.GetValue(objectref);
                sf = new SerializedField(field_info.Name, SerializationManagerUtilities.CreateRecordsFromStringStrictDictionary(v));
                field_info.ApplyConstraints(v, sf.Type, ConstraintApplicationTime.Writing);
                return sf;
            } else {
                sf = new SerializedField(field_info.Name, value);
            }
            field_info.ApplyConstraints(sf.Value, sf.Type, ConstraintApplicationTime.Writing);
            return sf;
        }

        private Record EncodeRecord(System.Object objectref)
        {
            Record.Builder builder = new();

            foreach (var fieldi in LookupCache(objectref.GetType())) { builder.Add(EncodeField(objectref, fieldi)); }

            return builder.Build();
        }

        private void DecodeField(System.Object objectref, SerializedField field, SerializationManagerUtilities.SerializationManagerFieldInformation field_info)
        {
            if (field is null) {
                // If the field was not retrieved successfully from the record and we have an optional value for it, set it.
                // Otherwise fail.
                if (field_info.DefaultValue is null) {
                    throw new SerializationException($"The field with name {field_info.Name} is required and cannot be found in the serialized record.");
                } else {
                    field_info.SetValue(objectref, field_info.DefaultValue); // Notice that on optional setting , the constraints do not apply for various reasons.
                    return;
                }
            } else {
                System.Object final_value;
                SerializedFieldType sft = field.Type;

                if (SerializationManagerUtilities.IsPrimitiveOrPrimitiveArray(sft))
                {
                    if (SerializationManagerUtilities.IsArrayAndExtractType(sft, out var t)) 
                    {
                        bool list;
                        Array finalarray;
                        Type fti = field_info.FieldType, ft;
                        // Some readers may not support lists and might have decoded it as an array. Handle this case.
                        if (fti.IsTypeNonGenericMatch(typeof(IList<>))) {
                            ft = fti.GenericTypeArguments[0];
                            list = true;
                        } else {
                            ft = fti.GetElementType();
                            list = false;
                        }

                        if (t == SerializedFieldType.MixedPrimitives) {
                            // An array of primitive numbers of unknown types. We need to translate each one as being of the underlying type.
                            Array original = field.Value as Array;
                            finalarray = Array.CreateInstance(ft, original.LongLength);
                            for (long I = 0; I < original.LongLength; I++)
                            {
                                finalarray.SetValue(SerializationManagerUtilities.SetAndGet(ft, original.GetValue(I)), I);
                            }
                        } else {
                            // We do have a stable array, so we can safely decode.
                            finalarray = field.Value as Array;
                        }

                        if (list) {
                            final_value = SerializationManagerUtilities.CreateListObject(finalarray);
                        } else {
                            final_value = finalarray;
                        }
                    } else {
                        // HACK: I am doing this so that widening conversions can appropriately work.
                        // For same object types and objects of those this will not have any effect.
                        final_value = SerializationManagerUtilities.SetAndGet(field_info.FieldType, field.Value);
                    }

                    // Type transcoding works only on top of the primitive types and their arrays
                    ITypeTranscoder tt = SerializationManagerUtilities.GetTranscoder(sft, field_info.FieldType, typetranscoders);
                    if (tt is not null)
                    {
                        // A type transcoder was found for this field, use it
                        final_value = tt.Decode(final_value, field_info.FieldType);
                    }
                } else if (SerializationManagerUtilities.IsObjectOrArrayOfObjects(sft)) {
                    // We have another Record to decode.
                    // We have two seperate cases, one being an array of records, or it is just another record.
                    if (sft.HasFlag(SerializedFieldType.Array)) {
                        Record[] records = field.RecordArrayValue;
                        Type fti = field_info.FieldType;
                        // Just like with the lists, string-strict dictionaries fall into the same category.
                        // As such, if we do have one, it will be transformed now.
                        if (fti.IsTypeNonGenericMatch(typeof(IDictionary<,>))) {
                            final_value = SerializationManagerUtilities.CreateStringStrictDictionary(records, fti.GenericTypeArguments[1]);
                        } else {
                            Array arr = Array.CreateInstance(fti, records.LongLength);
                            for (long I = 0; I < records.LongLength; I++)
                            {
                                // We must apply DecodeInnerRecord to each one record.
                                arr.SetValue(DecodeInnerRecord(records[I], field_info, true), I);
                            }
                            final_value = arr;
                        }
                    } else {
                        final_value = DecodeInnerRecord(field.RecordValue, field_info);
                    }
                } else if (SerializationManagerUtilities.IsListAndExtractType(sft, out var t)) {
                    if (t == SerializedFieldType.MixedPrimitives) {
                        // When MixedPrimitives, lists are encoded as Arrays as well, with the sole difference that they are assigned a IList<T> object. 
                        // As such, we will create such an object through reflection.
                        Array original = field.Value as Array;
                        var ft = field_info.FieldType.GenericTypeArguments[0];
                        Array finalarray = Array.CreateInstance(ft, original.LongLength);
                        for (long I = 0; I < original.LongLength; I++)
                        {
                            finalarray.SetValue(SerializationManagerUtilities.SetAndGet(ft, original.GetValue(I)), I);
                        }
                        final_value = SerializationManagerUtilities.CreateListObject(finalarray);
                    } else {
                        final_value = field.Value;
                    }
                } else if (SerializationManagerUtilities.IsStringDictAndExtractType(sft, out _)) {
                    final_value = SerializationManagerUtilities.CreateStringStrictDictionary(field.Value as Record[], field_info.FieldType.GetGenericArguments()[1]);
                } else {
                    // If reached this, it means that the serialization reader has somehow returned invalid
                    // data and does not implement correctly the serialization model. We should throw.

                    throw new InvalidSerializationReaderLayoutException($"Cannot determine what to do for field {field.Name} (Actual: {field_info.ActualName}) with value {field.Value}. Do you have coded correctly your reader implementation?");
                }

                // Now, apply the constraints on our field and finally set it.
                field_info.ApplyConstraints(final_value, sft, ConstraintApplicationTime.Reading);
                field_info.SetValue(objectref, final_value);
            }
        }

        private void DecodeRecord(System.Object objectref , Record retrieved)
        {
            foreach (var fieldi in LookupCache(objectref.GetType())) {
                DecodeField(objectref, retrieved.LookupField(fieldi.Name), fieldi);
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

                for (int I = 0, fc = 0; I < bindings.Length; I++, fc = 0)
                {
                    foreach (var field_info in LookupCache(bindings[I]))
                    {
                        if (value.LookupField(field_info.Name) is not null) {
                            // Found the field in the record, continue to see other fields as well.
                            fc++; 
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
            } else {
                throw new SerializationException($"Cache NOT COMPLETE!!!! The type {type.FullName} was not found in the serialization cache.");
            }
        }

        #endregion

        /// <summary>
        /// Disposes this <see cref="SerializationManager{T}"/>. <br />
        /// Thread-safe.
        /// </summary>
        public void Dispose()
        {
            Monitor.Enter(this);
            try {
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