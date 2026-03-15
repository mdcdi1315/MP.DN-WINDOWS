
using MP.Utilities;
using System;
using System.Collections.Generic;

namespace MP.Serialization
{
    /// <summary>
    /// Defines information over how a field is serialized, as well as the value it contains.
    /// </summary>
    public class SerializedField
    {
        private System.String name;
        private System.Object value;
        private SerializedFieldType type;

        /// <summary>
        /// Directly instantiates a new <see cref="SerializedField"/> class. <br />
        /// This constructor is only meant to be used by derived classes.
        /// </summary>
        protected SerializedField() { }

        /// <summary>
        /// Creates a new <see cref="SerializedField"/> class, defining the name of the field as well as it's actual value.
        /// </summary>
        /// <param name="name">The name of the field to be decoded.</param>
        /// <param name="value">The value of the field to be decoded.</param>
        public SerializedField(System.String name, System.Object value) : this(name, value, FindType(value)) { }

        /// <summary>
        /// Creates a new <see cref="SerializedField"/> class, defining the name of the field as well as it's actual value and it's desired type intended to be de/encoded into.
        /// </summary>
        /// <param name="name">The name of the field to be decoded.</param>
        /// <param name="value">The value of the field to be decoded.</param>
        /// <param name="type">The type of the field to be de/encoded as.</param>
        public SerializedField(System.String name, System.Object value, SerializedFieldType type)
        {
            ArgumentException.ThrowIfNullOrEmpty(name);
            this.type = type;
            this.name = name;
            this.value = value;
        }

        private static SerializedFieldType FindType(System.Object value)
        {
            if (value is null) {
                return SerializedFieldType.Empty;
            }
            Type tp = value.GetType();
            if (tp.IsArray) {
                return FindSimpleType(tp.GetElementType() , true) | SerializedFieldType.Array;
            } else {
                return FindSimpleType(tp);
            }
        }

        private static SerializedFieldType FindSimpleType(Type simple , System.Boolean array = false)
        {
            if (simple == typeof(System.Boolean)) {
                return SerializedFieldType.Boolean;
            } else if (simple == typeof(System.Byte)) {
                return SerializedFieldType.Byte;
            } else if (simple == typeof(System.SByte)) {
                return SerializedFieldType.SByte;
            } else if (simple == typeof(System.Int16)) {
                return SerializedFieldType.Int16;
            } else if (simple == typeof(System.UInt16)) {
                return SerializedFieldType.UInt16;
            } else if (simple == typeof(System.Int32)) {
                return SerializedFieldType.Int32;
            } else if (simple == typeof(System.UInt32)) {
                return SerializedFieldType.UInt32;
            } else if (simple == typeof(System.Int64)) {
                return SerializedFieldType.Int64;
            } else if (simple == typeof(System.UInt64)) {
                return SerializedFieldType.UInt64;
            } else if (simple == typeof(System.String)) {
                return SerializedFieldType.String;
            } else if (simple == typeof(Record) || simple == typeof(UninitializedRecord)) {
                return SerializedFieldType.Object;
            } else if (simple == typeof(System.Single)) {
                return SerializedFieldType.Single;
            } else if (simple == typeof(System.Double)) {
                return SerializedFieldType.Double;
            } else if (array && simple == typeof(System.Object)) { 
                return SerializedFieldType.MixedPrimitives;
            } else if (simple.ImplementsInterface(typeof(IList<>))) {
                return FindSimpleType(simple.GenericTypeArguments[0]) | SerializedFieldType.List;
            } else if (simple.ImplementsInterface(typeof(IDictionary<,>))) {
                return FindSimpleType(simple.GenericTypeArguments[1]) | SerializedFieldType.StrictStringDictionary;
            } else {
                throw new SerializationException($"Cannot determine the serialized type {simple.FullName}!!!");
            }
        }

        /// <summary>
        /// Retrieves the name of the serialzed field.
        /// </summary>
        public System.String Name => name;

        /// <summary>
        /// Retrieves the value of the serialized field.
        /// </summary>
        public System.Object Value => value;

        /// <summary>
        /// Retrieves the type of the serialized field.
        /// </summary>
        public SerializedFieldType Type => type;

        /// <summary>
        /// Retrieves the value of the serialized field as a <see cref="Record"/> instance, if applicable. <br />
        /// If not applicable, it returns <see langword="null"/>.
        /// </summary>
        public Record RecordValue
        {
            get {
                if (value is Record rc) {
                    return rc;
                } else if (value is UninitializedRecord urc) {
                    return urc.Value;
                } else {
                   return null; 
                }
            }
        }

        private static Record RecordConverter(UninitializedRecord u) => u.Value;

        /// <summary>
        /// Retrieves the value of the serialized field as a <see cref="Record"/> array, if applicable. <br />
        /// If not applicable, it returns <see langword="null"/>.
        /// </summary>
        public Record[] RecordArrayValue
        {
            get {
                if (value is Record[] records) {
                    return records;
                } else if (value is UninitializedRecord[] urecords) {
                    return Array.ConvertAll(urecords, new Converter<UninitializedRecord, Record>(RecordConverter));
                } else {
                    return null;
                }
            }
        }
    }
}