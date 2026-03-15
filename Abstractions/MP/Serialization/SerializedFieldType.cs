


namespace MP.Serialization
{
    /// <summary>
    /// Defines the type of the contained serialzed field.
    /// </summary>
    public enum SerializedFieldType : System.UInt16
    {
        /// <summary>
        /// The field has an empty value (or <see langword="null"/>).
        /// </summary>
        Empty,
        /// <summary>
        /// The type of the field is another class implementing the <see cref="ISerializableClass"/> interface.
        /// </summary>
        Object,
        /// <summary>
        /// The type of the field is a <see cref="System.Boolean">boolean</see>, a type that takes only two distinct values - <see langword="true"/> and <see langword="false"/>.
        /// </summary>
        Boolean,
        /// <summary>
        /// The type of the field is a <see cref="System.String">string</see>, that is a sequence of characters.
        /// </summary>
        String,
        /// <summary>
        /// The type of the field is of type <see cref="System.Byte"/>.
        /// </summary>
        Byte,
        /// <summary>
        /// The type of the field is of type <see cref="System.SByte"/>.
        /// </summary>
        SByte,
        /// <summary>
        /// The type of the field is of type <see cref="System.Int16"/>.
        /// </summary>
        Int16,
        /// <summary>
        /// The type of the field is of type <see cref="System.UInt16"/>.
        /// </summary>
        UInt16,
        /// <summary>
        /// The type of the field is of type <see cref="System.Int32"/>.
        /// </summary>
        Int32,
        /// <summary>
        /// The type of the field is of type <see cref="System.UInt32"/>.
        /// </summary>
        UInt32,
        /// <summary>
        /// The type of the field is of type <see cref="System.Int64"/>.
        /// </summary>
        Int64,
        /// <summary>
        /// The type of the field is of type <see cref="System.UInt64"/>.
        /// </summary>
        UInt64,
        /// <summary>
        /// The type of the field is of type <see cref="System.Single"/>.
        /// </summary>
        Single,
        /// <summary>
        /// The type of the field is of type <see cref="System.Double"/>.
        /// </summary>
        Double,
        /// <summary>
        /// Only used when the <see cref="Array"/> or <see cref="List"/> flag is defined. <br />
        /// There are cases that readers cannot find the original numeric type so they return an arbitrary primitive type. Thus, a simple upcast is done by the serialization manager inherently to adapt to the field type. <br />
        /// However, there are also even rarer cases that it happens to define an array of primitives. <br />
        /// Thus, if the array does contain for example some shorts and some doubles, this will allow the serialization manager to work as it is expected. <br />
        /// Note that, for such cases, you have to pass an <see cref="System.Array"/> that is <see cref="System.Object"/> so that to accomondate all the defined primitives. <br />
        /// The serialization manager will elsewise take care of it and will appropriately transform the array as it is required.
        /// </summary>
        MixedPrimitives = 255,
        /// <summary>
        /// Flag indicating that the field is an array of the specified type.
        /// </summary>
        Array = 1 << 9,
        /// <summary>
        /// Flag indicating that the field is a dictionary whose keys are strings and their values are exclusively the one defined in the first 255 values. <br />
        /// Values are encoded as <see cref="Record"/>s containing two fields, the key and value of each entry.
        /// </summary>
        StrictStringDictionary = 1 << 10,
        /// <summary>
        /// Flag indicating that the field is a list of the specified type. <br />
        /// Fields in the actual metadata must be represented with the <see cref="System.Collections.Generic.IList{T}"/> interface. <br />
        /// The serialization manager treats this as the <see cref="Array"/> flag.
        /// </summary>
        List = 1 << 11,
        /// <summary>
        /// Defines a constant for the lower bound of the primitive types range. Only used for the API internals.
        /// </summary>
        PRIMITIVE_TYPES_START = Boolean,
        /// <summary>
        ///  Defines a constant for the upper bound of the primitive types range. Only used for the API internals.
        /// </summary>
        PRIMITIVE_TYPES_END = Double
    }
}