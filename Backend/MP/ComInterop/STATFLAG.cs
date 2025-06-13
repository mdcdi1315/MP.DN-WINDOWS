

namespace MP.ComInterop
{
    /// <summary>
    /// The <strong>STATFLAG</strong> enumeration values indicate whether the method should try to return a name in the pwcsName member of the <see cref="STATSTG"/> structure. 
    /// The values are used in the ILockBytes::Stat, IStorage::Stat, and <see cref="IStream.Stat"/> methods to save memory when the pwcsName member is not required.
    /// </summary>
    public enum STATFLAG : System.UInt32
    {
        /// <summary>
        /// Requests that the statistics include the pwcsName member of the <see cref="STATSTG"/> structure.
        /// </summary>
        STATFLAG_DEFAULT = 0,
        /// <summary>
        /// Requests that the statistics not include the pwcsName member of the <see cref="STATSTG"/> structure.  <br />
        /// If the name is omitted, there is no need for the ILockBytes::Stat, IStorage::Stat, and <see cref="IStream.Stat"/> methods to allocate and free memory for the string value of the name,
        /// therefore the method reduces time and resources used in an allocation and free operation.
        /// </summary>
        STATFLAG_NONAME = 1,
        /// <summary>Not implemented.</summary>
        STATFLAG_NOOPEN = 2
    }
}