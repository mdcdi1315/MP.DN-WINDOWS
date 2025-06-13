namespace MP.ComInterop
{
    /// <summary>
    /// The <see cref="STGTY"/> enumeration values are used in the type member of the <see cref="STATSTG"/> structure to indicate the type of the storage element.  <br />
    /// A storage element is a storage object, a stream object, or a byte-array object (LOCKBYTES).
    /// </summary>
    public enum STGTY : System.UInt32
    {
        /// <summary>Indicates that the storage element is a storage object.</summary>
        STGTY_STORAGE = 1,
        /// <summary>Indicates that the storage element is a stream object.</summary>
        STGTY_STREAM = 2,
        /// <summary>Indicates that the storage element is a byte-array object.</summary>
        STGTY_LOCKBYTES = 3,
        /// <summary>Indicates that the storage element is a property storage object.</summary>
        STGTY_PROPERTY = 4
    }
}