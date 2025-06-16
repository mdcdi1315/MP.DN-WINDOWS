using MP;
using System;
using System.Runtime.Versioning;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;

// Type mappings:
// DWORD corresponds to System.UInt32
// WORD corresponds to System.UInt16
// SHORT corresponds to System.Int16
// USHORT corresponds to System.UInt16
// LONG corresponds to System.Int32
// ULONG corresponds to System.UInt32
// UINT corresponds to System.UInt32
// BYTE corresponds to System.Byte
// ULARGE_INTEGER corresponds to System.UInt64
// LARGE_INTEGER corresponds to System.Int64
// UCHAR corresponds to System.Byte
// ULONGLONG corresponds to System.UInt64
// SIZE_T is a ULONG_PTR , which does mean that: The largest unsigned pointer size. In 64-bit this is effectively a System.UInt64.
// QWORD corresponds to System.UInt64 (I have never found this in Win32 API but in IMFByteStream interface, thought to doc it tho)

[MP.Annotations.NativeLayer]
[System.Security.SuppressUnmanagedCodeSecurity] // Although that it has no effect in .NET 8 it might gain effect again in the future , so mark it.
internal static partial class Interop
{
    /// <summary>
    /// Blittable version of Windows BOOL type. It is convenient in situations where
    /// manual marshalling is required, or to avoid overhead of regular bool marshalling.
    /// </summary>
    /// <remarks>
    /// Some Windows APIs return arbitrary integer values although the return type is defined
    /// as BOOL. It is best to never compare BOOL to TRUE. Always use bResult != BOOL.FALSE
    /// or bResult == BOOL.FALSE .
    /// </remarks>
    public enum BOOL : System.Int32
    {
        FALSE = 0,
        TRUE = 1,
    }

    /// <summary>
    /// Blittable version of Windows BOOLEAN type. It is convenient in situations where
    /// manual marshalling is required, or to avoid overhead of regular bool marshalling.
    /// </summary>
    /// <remarks>
    /// Some Windows APIs return arbitrary integer values although the return type is defined
    /// as BOOLEAN. It is best to never compare BOOLEAN to TRUE. Always use bResult != BOOLEAN.FALSE
    /// or bResult == BOOLEAN.FALSE .
    /// </remarks>
    public enum BOOLEAN : System.Byte
    {
        FALSE = 0,
        TRUE = 1,
    }

    /// <summary>Describes NT status codes.</summary>
    public enum NTSTATUS : System.UInt32
    {
        STATUS_SUCCESS = 0x0,
        STATUS_NOT_FOUND = 0xC0000225,
        STATUS_INVALID_PARAMETER = 0xc000000d,
        STATUS_INVALID_HANDLE = 0xC0000008,
        STATUS_NO_MEMORY = 0xc0000017,
        STATUS_AUTH_TAG_MISMATCH = 0xc000a002,
        STATUS_SOME_NOT_MAPPED = 0x00000107,
        STATUS_BUFFER_OVERFLOW = 0x80000005,
        STATUS_NO_MORE_FILES = 0x80000006,
        STATUS_OBJECT_NAME_NOT_FOUND = 0xC0000034,
        STATUS_NONE_MAPPED = 0xC0000073,
        STATUS_INSUFFICIENT_RESOURCES = 0xC000009A,
        STATUS_ACCESS_DENIED = 0xC0000022,
        STATUS_ACCOUNT_RESTRICTION = 0xc000006e,
        STATUS_FILE_NOT_FOUND = 0xC000000F
    }

    [Flags]
    public enum ObjectAttributes : System.UInt32
    {
        // https://msdn.microsoft.com/en-us/library/windows/hardware/ff564586.aspx
        // https://msdn.microsoft.com/en-us/library/windows/hardware/ff547804.aspx

        /// <summary>
        /// This handle can be inherited by child processes of the current process.
        /// </summary>
        OBJ_INHERIT = 0x00000002,

        /// <summary>
        /// This flag only applies to objects that are named within the object manager.
        /// By default, such objects are deleted when all open handles to them are closed.
        /// If this flag is specified, the object is not deleted when all open handles are closed.
        /// </summary>
        OBJ_PERMANENT = 0x00000010,

        /// <summary>
        /// Only a single handle can be open for this object.
        /// </summary>
        OBJ_EXCLUSIVE = 0x00000020,

        /// <summary>
        /// Lookups for this object should be case insensitive.
        /// </summary>
        OBJ_CASE_INSENSITIVE = 0x00000040,

        /// <summary>
        /// Create on existing object should open, not fail with STATUS_OBJECT_NAME_COLLISION.
        /// </summary>
        OBJ_OPENIF = 0x00000080,

        /// <summary>
        /// Open the symbolic link, not its target.
        /// </summary>
        OBJ_OPENLINK = 0x00000100,

        // Only accessible from kernel mode
        // OBJ_KERNEL_HANDLE

        // Access checks enforced, even in kernel mode
        // OBJ_FORCE_ACCESS_CHECK
        // OBJ_VALID_ATTRIBUTES = 0x000001F2
    }

    /// <summary>
    /// Defines Windows IOCTL codes. Used by the DeviceIoControl function.
    /// </summary>
    /// <remarks>
    /// <c><![CDATA[public static System.UInt32 CTL_CODE(System.UInt32 Devtype , System.UInt32 func , System.UInt32 method , System.UInt32 access) => (Devtype << 16) | (access << 14) | (func << 2) | method;]]></c>
    /// </remarks>
    public enum IOCTL : System.UInt32
    {
        IOCTL_STORAGE_READ_CAPACITY = 2969920
    }

    /// <summary>Defines the Windows file attributes.</summary>
    [Flags]
    public enum FileAttributes : System.UInt32
    {
        /// <summary>
        /// A file that is read-only. 
        /// Applications can read the file, but cannot write to it or delete it. 
        /// This attribute is not honored on directories.
        /// </summary>
        FILE_ATTRIBUTE_READONLY = 1,
        /// <summary>
        /// The file or directory is hidden. 
        /// It is not included in an ordinary directory listing.
        /// </summary>
        FILE_ATTRIBUTE_HIDDEN = 2,
        /// <summary>
        /// A file or directory that the operating system uses a part of, or uses exclusively.
        /// </summary>
        FILE_ATTRIBUTE_SYSTEM = 4,
        /// <summary>The handle that identifies a directory.</summary>
        FILE_ATTRIBUTE_DIRECTORY = 16,
        /// <summary>
        /// A file or directory that is an archive file or directory. 
        /// Applications typically use this attribute to mark files for backup or removal.
        /// </summary>
        FILE_ATTRIBUTE_ARCHIVE = 32,
        /// <summary>
        /// A file that does not have other attributes set. 
        /// This attribute is valid only when used alone.
        /// </summary>
        FILE_ATTRIBUTE_NORMAL = 128,
        /// <summary>
        /// A file that is being used for temporary storage. 
        /// File systems avoid writing data back to mass storage if sufficient cache memory is available, because typically, an application deletes a temporary file after the handle is closed. In that scenario, the system can entirely avoid writing the data. 
        /// Otherwise, the data is written after the handle is closed.
        /// </summary>
        FILE_ATTRIBUTE_TEMPORARY = 256,
        /// <summary>A file that is a sparse file.</summary>
        FILE_ATTRIBUTE_SPARSE_FILE = 512,
        /// <summary>
        /// A file or directory that has an associated reparse point, or a file that is a symbolic link.
        /// </summary>
        FILE_ATTRIBUTE_REPARSE_POINT = 1024,
        /// <summary>
        /// A file or directory that is compressed. 
        /// For a file, all of the data in the file is compressed. 
        /// For a directory, compression is the default for newly created files and subdirectories.
        /// </summary>
        FILE_ATTRIBUTE_COMPRESSED = 2048,
        /// <summary>
        /// The data of a file is not available immediately. 
        /// This attribute indicates that the file data is physically moved to offline storage. 
        /// This attribute is used by Remote Storage, which is the hierarchical storage management software. 
        /// Applications should not arbitrarily change this attribute.
        /// </summary>
        FILE_ATTRIBUTE_OFFLINE = 4096,
        /// <summary>
        /// The file or directory is not to be indexed by the content indexing service.
        /// </summary>
        FILE_ATTRIBUTE_NOT_CONTENT_INDEXED = 8192,
        /// <summary>
        /// A file or directory that is encrypted. 
        /// For a file, all data streams in the file are encrypted.
        /// For a directory, encryption is the default for newly created files and subdirectories.
        /// </summary>
        FILE_ATTRIBUTE_ENCRYPTED = 16384,
        /// <summary>
        /// The directory or user data stream is configured with integrity (only supported on ReFS volumes). 
        /// It is not included in an ordinary directory listing. The integrity setting persists with the file if it's renamed.
        /// If a file is copied the destination file will have integrity set if either the source file or destination directory have integrity set.
        /// </summary>
        [SupportedOSPlatform(MP.WindowsInterop.WindowsVersions.NTDDI_WIN8)]
        FILE_ATTRIBUTE_INTEGRITY_STREAM = 32768,
        /// <summary>This value is reserved for system use.</summary>
        FILE_ATTRIBUTE_VIRTUAL = 65536,
        /// <summary>
        /// The user data stream not to be read by the background data integrity scanner (AKA scrubber).
        /// When set on a directory it only provides inheritance. This flag is only supported on Storage Spaces and ReFS volumes. 
        /// It is not included in an ordinary directory listing.
        /// </summary>
        [SupportedOSPlatform(MP.WindowsInterop.WindowsVersions.NTDDI_WIN8)]
        FILE_ATTRIBUTE_NO_SCRUB_DATA = 131072,
        /// <summary>
        /// A file or directory with extended attributes. <br />
        /// <strong>IMPORTANT</strong>: This constant is for internal use only.
        /// </summary>
        FILE_ATTRIBUTE_EA = 262144,
        /// <summary>
        /// This attribute only appears in directory enumeration classes (FILE_DIRECTORY_INFORMATION, FILE_BOTH_DIR_INFORMATION, etc.). 
        /// When this attribute is set, it means that the file or directory has no physical representation on the local system; the item is virtual.
        /// Opening the item will be more expensive than normal, e.g. it will cause at least some of it to be fetched from a remote store.
        /// </summary>
        FILE_ATTRIBUTE_RECALL_ON_OPEN = FILE_ATTRIBUTE_EA,
        /// <summary>
        /// This attribute indicates user intent that the file or directory should be kept fully present locally even when not being actively accessed. 
        /// This attribute is for use with hierarchical storage management software.
        /// </summary>
        FILE_ATTRIBUTE_PINNED = 524288,
        /// <summary>
        /// This attribute indicates that the file or directory should not be kept fully present locally except when being actively accessed. 
        /// This attribute is for use with hierarchical storage management software.
        /// </summary>
        FILE_ATTRIBUTE_UNPINNED = 1048576
    }

    // https://msdn.microsoft.com/en-us/library/windows/desktop/aa380518.aspx
    // https://msdn.microsoft.com/en-us/library/windows/hardware/ff564879.aspx
    [StructLayout(LayoutKind.Sequential)]
    public unsafe struct UNICODE_STRING
    {
        /// <summary>Length in bytes, not including the null terminator, if any.</summary>
        public System.UInt16 Length;

        /// <summary>Max size of the buffer in bytes</summary>
        public System.UInt16 MaximumLength;

        /// <summary>The actual pointer to the string buffer</summary>
        public System.Char* Buffer;

        public unsafe void FreeCreatedString()
        {
            if (Buffer is null) { return; }
            Ole32.CoTaskMemFree(Buffer);
            Buffer = null;
        }

        public static UNICODE_STRING CreateFromString(System.String str)
        {
            if (str.Length > 32767) { throw new ArgumentOutOfRangeException(nameof(str), "The provided string is too large to fit into a UNICODE_STRING structure"); }
            UNICODE_STRING strnew = new();
            strnew.MaximumLength = strnew.Length = (str.Length * sizeof(System.Char)).ToUInt16();
            System.Char* pbuf = (System.Char*)Ole32.CoTaskMemAlloc(strnew.Length);
            if (pbuf is null) {
                throw new OutOfMemoryException("Not enough memory to create the UNICODE_STRING instance!");
            }
            fixed (System.Char* ps = str)
            {
                Unsafe.CopyBlockUnaligned(pbuf, ps, strnew.Length);
            }
            strnew.Buffer = pbuf;
            return strnew;
        }
    
        public static UNICODE_STRING CreateFromSpan(ReadOnlySpan<System.Char> span)
        {
            if (span.Length > 32767) { throw new ArgumentOutOfRangeException(nameof(span), "The provided string is too large to fit into a UNICODE_STRING structure"); }
            UNICODE_STRING strnew = new();
            strnew.MaximumLength = strnew.Length = (span.Length * sizeof(System.Char)).ToUInt16();
            System.Char* pbuf = (System.Char*)Ole32.CoTaskMemAlloc(strnew.Length);
            if (pbuf is null) {
                throw new OutOfMemoryException("Not enough memory to create the UNICODE_STRING instance!");
            }
            Unsafe.CopyBlockUnaligned(
                ref *(System.Byte*)pbuf , 
                ref Unsafe.As<System.Char , System.Byte>(ref Unsafe.AsRef(in span[0])) , 
                strnew.Length
            );
            strnew.Buffer = pbuf;
            return strnew;
        }
    }

    /// <summary>
    /// <a href="https://msdn.microsoft.com/en-us/library/windows/hardware/ff557749.aspx">OBJECT_ATTRIBUTES</a> structure.
    /// The OBJECT_ATTRIBUTES structure specifies attributes that can be applied to objects or object handles by routines 
    /// that create objects and/or return handles to objects.
    /// </summary>
    // Keep it sequential for various reasons.
    public unsafe struct OBJECT_ATTRIBUTES
    {
        public uint Length;

        /// <summary>
        /// Optional handle to root object directory for the given ObjectName.
        /// Can be a file system directory or object manager directory.
        /// </summary>
        public IntPtr RootDirectory;

        /// <summary>
        /// Name of the object. Must be fully qualified if RootDirectory isn't set.
        /// Otherwise is relative to RootDirectory.
        /// </summary>
        public UNICODE_STRING* ObjectName;

        public ObjectAttributes Attributes;

        /// <summary>
        /// If null, object will receive default security settings.
        /// </summary>
        public void* SecurityDescriptor;

        /// <summary>
        /// Optional quality of service to be applied to the object. Used to indicate
        /// security impersonation level and context tracking mode (dynamic or static).
        /// </summary>
        public void* SecurityQualityOfService;

        /// <summary>
        /// Equivalent of InitializeObjectAttributes macro with the exception that you can directly set SQOS.
        /// </summary>
        public unsafe OBJECT_ATTRIBUTES(UNICODE_STRING* objectName, ObjectAttributes attributes, IntPtr rootDirectory)
        {
            Length = (uint)sizeof(OBJECT_ATTRIBUTES);
            RootDirectory = rootDirectory;
            ObjectName = objectName;
            Attributes = attributes;
            SecurityDescriptor = null;
            SecurityQualityOfService = null;
        }
    }

    [StructLayout(LayoutKind.Explicit , Size = 8 , Pack = 4)]
    public struct FILETIME
    {
        [FieldOffset(0)]
        public System.UInt32 dwLowDateTime;
        [FieldOffset(4)]
        public System.UInt32 dwHighDateTime;

        public FILETIME(System.Int64 fileTime)
        {
            dwLowDateTime = (System.UInt32)fileTime;
            dwHighDateTime = (System.UInt32)(fileTime >> 32);
        }

        public readonly System.Int64 ToTicks() => (dwHighDateTime.ToInt64() << 32) + dwLowDateTime;
        public readonly DateTime ToDateTimeUtc() => DateTime.FromFileTimeUtc(ToTicks());
        public readonly DateTimeOffset ToDateTimeOffset() => DateTimeOffset.FromFileTime(ToTicks());
        public readonly SYSTEMTIME ToSystemTime()
        {
            BOOL br = Kernel32.FileTimeToSystemTime(this, out var st);
            if (br == BOOL.FALSE) { throw new MP.ExceptionSystem.NativeWindowsException(); }
            return st;
        }

        /// <summary>Forwards the <see cref="DateTime.ToString()"/> method.</summary>
        public override readonly string ToString() => ToDateTimeUtc().ToString();

        public static FILETIME FromDateTime(System.DateTime dt) => new(dt.ToFileTimeUtc());

        public static FILETIME Now => Kernel32.GetSystemTimeAsFileTime();

        public static explicit operator LongFileTime(FILETIME fileTime) => new() { TicksSince1601 = fileTime.ToTicks() };
    }

    /// <summary>
    /// 100-nanosecond intervals (ticks) since January 1, 1601 (UTC).
    /// </summary>
    /// <remarks>
    /// For NT times that are defined as longs (LARGE_INTEGER, etc.).
    /// Do NOT use for <see cref="FILETIME"/> unless you are POSITIVE it will fall on an
    /// 8 byte boundary.
    /// </remarks>
    [StructLayout(LayoutKind.Explicit, Size = 8 , Pack = 4)]
    public struct LongFileTime
    {
        /// <summary>100-nanosecond intervals (ticks) since January 1, 1601 (UTC).</summary>
        [FieldOffset(0)]
        public System.Int64 TicksSince1601;

        public LongFileTime(DateTime dt) => TicksSince1601 = dt.ToFileTimeUtc();

        public LongFileTime(DateTimeOffset ofs) => TicksSince1601 = ofs.ToFileTime();

        public DateTimeOffset ToDateTimeOffset() => new(DateTime.FromFileTimeUtc(TicksSince1601));

        // Special value to not update a date value during date information update.
        public static LongFileTime MinusOne => new() { TicksSince1601 = -1 };

        public static explicit operator FILETIME(LongFileTime lft) => new(lft.TicksSince1601);
    }

    /// <summary>
    /// System Time structure for .NET. <br />
    /// Note that the ability to convert to a more concrete format (like <see cref="FILETIME"/>) ,
    /// is not straightforward and requires interop calls.
    /// </summary>
    [StructLayout(LayoutKind.Explicit, Pack = 2 , Size = 16)]
    public struct SYSTEMTIME
    {
        [FieldOffset(0)]
        public System.UInt16 Year;

        [FieldOffset(2)]
        public System.UInt16 Month;

        [FieldOffset(4)]
        public System.UInt16 DayOfWeek;

        [FieldOffset(6)]
        public System.UInt16 Day;

        [FieldOffset(8)]
        public System.UInt16 Hour;

        [FieldOffset(10)]
        public System.UInt16 Minute;

        [FieldOffset(12)]
        public System.UInt16 Second;

        [FieldOffset(14)]
        public System.UInt16 Millisecond;

        public readonly FILETIME ToFileTime()
        {
            BOOL bs = Kernel32.SystemTimeToFileTime(this, out var ft);
            if (bs == BOOL.FALSE) { throw new MP.ExceptionSystem.NativeWindowsException(); }
            return ft;
        }
    }

    /// <summary>
    /// GUID native marshalling type. <br />
    /// Provides also methods to convert from , and to , a <see cref="System.Guid"/> structure.
    /// </summary>
    [StructLayout(LayoutKind.Explicit, Size = 16, Pack = 4)]
    public unsafe struct GUID
    {
        [FieldOffset(0)]
        public System.UInt32 Data1;
        [FieldOffset(4)]
        public System.UInt16 Data2;
        [FieldOffset(6)]
        public System.UInt16 Data3;
        [FieldOffset(8)]
        public System.Byte Data4_0;
        [FieldOffset(9)]
        public System.Byte Data4_1;
        [FieldOffset(10)]
        public System.Byte Data4_2;
        [FieldOffset(11)]
        public System.Byte Data4_3;
        [FieldOffset(12)]
        public System.Byte Data4_4;
        [FieldOffset(13)]
        public System.Byte Data4_5;
        [FieldOffset(14)]
        public System.Byte Data4_6;
        [FieldOffset(15)]
        public System.Byte Data4_7;

        public GUID() { }

        public readonly Guid GetGuid()
        {
            Guid ret = Guid.Empty;
            Unsafe.CopyBlockUnaligned(
                ref Unsafe.As<Guid, System.Byte>(ref ret),
                ref Unsafe.As<GUID, System.Byte>(ref Unsafe.AsRef(in this)),
                16U);
            return ret;
        }

        public static GUID FromGUID(Guid guid)
        {
            GUID result = new();
            Unsafe.CopyBlockUnaligned(
                ref Unsafe.As<GUID, System.Byte>(ref result),
                ref Unsafe.As<Guid, System.Byte>(ref guid),
                16U);
            return result;
        }
        
        public static GUID Empty
        {
            get {
                GUID ret = new();
                Unsafe.InitBlockUnaligned(ref Unsafe.As<GUID, System.Byte>(ref ret), 0, 16U);
                return ret;
            }
        }

        public static GUID From16BytePointer(System.Byte* p)
        {
            GUID result = new();
            if (p is null) { return result; }
            // Using managed pointer translation is faster than pinning
            // We could eventually use Unsafe.AsRef for p but
            // C# seems to translate 'ref p[0]' with just an ldarg instruction (does it work , so simply?)
            Unsafe.CopyBlockUnaligned(
                ref Unsafe.As<GUID , System.Byte>(ref result), 
                ref p[0], 16U);
            return result;
        }

        public static GUID FromString(System.String str) => FromGUID(new(str));

        /// <summary>
        /// Returns the fully constructed GUID.
        /// </summary>
        public override readonly System.String ToString() => GetGuid().ToString();
    }

    [StructLayout(LayoutKind.Explicit , Pack = 8)]
    public unsafe struct OVERLAPPED
    {
        [StructLayout(LayoutKind.Explicit, Size = 8 , Pack = 4)]
        public unsafe struct OVERLAPPEDUNION
        {
            [FieldOffset(0)]
            public void* Pointer;

            [FieldOffset(0)]
            public System.UInt32 Offset;

            [FieldOffset(4)]
            public System.UInt32 OffsetHigh;
        }

        [FieldOffset(0)]
        public System.UInt32* Internal;

        [FieldOffset(8)]
        public System.UInt32* InternalSize;

        [FieldOffset(16)]
        public OVERLAPPEDUNION Union;

        [FieldOffset(24)]
        public System.IntPtr HEvent;

        // Doc says that when we initialize and use such a structure all the members must be properly initialized and all other ones
        // should have the value zero. So, we initialize it properly here with zeroes so we can avoid thinking about that in call sites.
        public OVERLAPPED()
        {
            Internal = null;
            InternalSize = null;
            Union = new();
            Union.Pointer = null;
            HEvent = System.IntPtr.Zero;
        }
    }

    private static System.String GetAndTrimString(Span<System.Char> buffer)
    {
        int length = buffer.Length;
        while (length > 0 && buffer[length - 1] <= 32)
        {
            length--; // trim off spaces and non-printable ASCII chars at the end of the resource
        }
        return buffer.Slice(0, length).ToString();
    }

}