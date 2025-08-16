


namespace MP.Serialization
{
    /// <summary>
    /// Defines the type of the contained serialzed field.
    /// </summary>
    public enum SerializedFieldType : System.UInt16
    {
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
        /// Flag indicating that the field is an array of the specified type.
        /// </summary>
        Array = 1 << 8,
        /// <summary>
        /// Flag indicating that the field is a dictionary whose keys are strings and their values are exclusively the one defined in the first 255 values.
        /// </summary>
        StrictStringDictionary = 1 << 9,
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