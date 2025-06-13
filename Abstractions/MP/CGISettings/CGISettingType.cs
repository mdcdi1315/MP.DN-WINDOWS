namespace MP.CGISettings
{
    /// <summary>
    /// Defines different setting types that can be saved into a CGI Settings file.
    /// </summary>
    public enum CGISettingType : System.Byte
    {
        /// <summary>Represents a setting that can store a string value.</summary>
        String,
        /// <summary>Represents a setting that can store a boolean value.</summary>
        Boolean,
        /// <summary>Represents a setting that can store a byte value.</summary>
        Byte,
        /// <summary>Represents a setting that can store a signed byte value.</summary>
        SByte,
        /// <summary>Represents a setting that can store a signed short value.</summary>
        Int16,
        /// <summary>Represents a setting that can store an unsigned short value.</summary>
        UInt16,
        /// <summary>Represents a setting that can store a signed integer value.</summary>
        Int32,
        /// <summary>Represents a setting that can store an unsigned integer value.</summary>
        UInt32,
        /// <summary>Represents a setting that can store a signed long integer value.</summary>
        Int64,
        /// <summary>Represents a setting that can store an unsigned long integer value.</summary>
        UInt64,
        /// <summary>Represents a setting that can store an entire byte array.</summary>
        ByteArray,
        /// <summary>
        /// Represents a setting that can store a FileInfo implementation class. <br />
        /// Can be overriden by the users to provide their own implementation.
        /// </summary>
        FileInfo,
        /// <summary>
        /// Represents a setting that can store a DirectoryInfo implementation class. <br />
        /// Can be overriden by the users to provide their own implementation.
        /// </summary>
        DirectoryInfo,
        /// <summary>Represents a setting that can store a single-precision floating number.</summary>
        Single,
        /// <summary>Represents a setting that can store a double-precision floating number.</summary>
        Double,
        /// <summary>
        /// Represents a setting that can store a color. <br />
        /// Can be overriden by the users to provide their own implementation.
        /// </summary>
        Color
    }
}