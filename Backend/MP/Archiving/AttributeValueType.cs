

namespace MP.Archiving
{
    /// <summary>
    /// Defines default attribute value types, which all the archivers and the archive readers must recognize these. <br />
    /// If they cannot recognize the given value for any possible reason , they must skip it by using the <see cref="MPARCHATTRIB.AttributeLength"/> value.
    /// </summary>
    public enum AttributeValueType : System.UInt16
    {
        None = 0,
        /// <summary>
        /// Special value to indicate that the value is nothing (This does not use any space in the actual file)
        /// </summary>
        Null,
        /// <summary>
        /// For cases that the default attribute types do not cover the use case,
        /// archivers may use a custom value of predefined size to save the data in.
        /// </summary>
        Custom,
        /// <summary>
        /// The attribute's value is a Unicode UTF-16 string saved in little-endian order.
        /// </summary>
        String,
        /// <summary>
        /// The attribute's value is a <see cref="MPARCHUINT"/> value.
        /// </summary>
        ArchiveUnsignedInteger,
        /// <summary>
        /// The attribute's value is a <see cref="MPARCHDATETIME"/> value.
        /// </summary>
        ArchiveDateTime,
        /// <summary>
        /// The attribute's value is a <see cref="System.Byte"/> that, when is only zero , it represents the <see langword="false"/> value. <br />
        /// Otherwise it does always represent the <see langword="true"/> value.
        /// </summary>
        Boolean,
        /// <summary>
        /// The attribute's value is a byte.
        /// </summary>
        Byte,
        /// <summary>
        /// The attribute's value is a signed byte. In .NET this is represented with the <see cref="System.SByte"/> type.
        /// </summary>
        SignedByte,
        /// <summary>
        /// The attribute's value is a signed number ranging from -32768 to 32767.
        /// </summary>
        Short,
        /// <summary>
        /// The attribute's value is an unsigned number ranging from 0 to 65535.
        /// </summary>
        UnsignedShort,
        /// <summary>
        /// The attribute's value is a signed number ranging from -2147483648 to 2147483647.
        /// </summary>
        Integer,
        /// <summary>
        /// The attribute's value is an unsigned number ranging from 0 to 4294967295.
        /// </summary>
        UnsignedInteger,
        /// <summary>
        /// The attribute's value is a signed number ranging from -9223372036854775808 to 9223372036854775807.
        /// </summary>
        Long,
        /// <summary>
        /// The attribute's value is an unsigned number ranging from 0 to 18446744073709551615.
        /// </summary>
        UnsignedLong
    }
}