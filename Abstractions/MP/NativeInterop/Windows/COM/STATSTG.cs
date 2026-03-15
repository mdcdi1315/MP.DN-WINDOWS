
using System.Runtime.Versioning;
using System.Runtime.InteropServices;

namespace MP.NativeInterop.Windows.COM
{
    /// <summary>
    /// The STATSTG structure contains statistical data about an open storage, stream, or byte-array object. <br />
    /// This structure is used in the IEnumSTATSTG, ILockBytes, IStorage, and <see cref="IStream"/> interfaces.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    [SupportedOSPlatform(WindowsVersions._WIN32_WINNT_WIN2K)]
    public unsafe struct STATSTG
    {
        /// <summary>
        /// A pointer to a NULL-terminated Unicode string that contains the name. <br />
        /// Space for this string is allocated by the method called and freed by the caller (for more information, see CoTaskMemFree).  <br />
        /// To not return this member, specify the STATFLAG_NONAME value when you call a method that returns a <see cref="STATSTG"/> structure, except for calls to IEnumSTATSTG::Next, which provides no way to specify this value.
        /// </summary>
        public System.Char* pwcsName;

        /// <summary>
        /// Indicates the type of storage object. <br />
        /// This is one of the values from the <see cref="STGTY"/> enumeration.
        /// </summary>
        public STGTY type;

        /// <summary>Specifies the size, in bytes, of the stream or byte array.</summary>
        public System.UInt64 cbSize;

        /// <summary>Indicates the last modification time for this storage, stream, or byte array.</summary>
        public FILETIME mtime;
        /// <summary>Indicates the creation time for this storage, stream, or byte array.</summary>
        public FILETIME ctime;
        /// <summary>Indicates the last access time for this storage, stream, or byte array.</summary>
        public FILETIME atime;

        /// <summary>
        /// Indicates the access mode specified when the object was opened. <br />
        /// This member is only valid in calls to <see cref="IStream.Stat"/> methods.
        /// </summary>
        public System.UInt32 grfMode;

        /// <summary>
        /// Indicates the types of region locking supported by the stream or byte array. <br />
        /// For more information about the values available, see the <see cref="LOCKTYPE"/> enumeration. <br />
        /// This member is not used for storage objects.
        /// </summary>
        public LOCKTYPE grfLocksSupported;

        /// <summary>
        /// Indicates the class identifier for the storage object; set to <see cref="GUID.Empty"/> for new storage objects. <br />
        /// This member is not used for streams or byte arrays.
        /// </summary>
        public GUID clsid;

        /// <summary>
        /// Indicates the current state bits of the storage object; that is, the value most recently set by the IStorage::SetStateBits method. <br />
        /// This member is not valid for streams or byte arrays.
        /// </summary>
        public System.UInt32 grfStateBits;

        /// <summary>Reserved for future use.</summary>
        public System.UInt32 reserved;
    }
}