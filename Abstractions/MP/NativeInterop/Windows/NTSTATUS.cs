

namespace MP.NativeInterop.Windows
{
    /// <summary>
    /// Describes NT status codes. <br />
    /// These are typically used in the NT kernel API's.
    /// </summary>
    public enum NTSTATUS : System.UInt32
    {
        /// <summary>Success.</summary>
        STATUS_SUCCESS = 0x0,
        /// <summary>The requested object was not found.</summary>
        STATUS_NOT_FOUND = 0xC0000225,
        /// <summary>An invalid value was passed for a function parameter.</summary>
        STATUS_INVALID_PARAMETER = 0xc000000d,
        /// <summary>An invalid handle value was passed for a function parameter.</summary>
        STATUS_INVALID_HANDLE = 0xC0000008,
        /// <summary>Out of memory.</summary>
        STATUS_NO_MEMORY = 0xc0000017,
        /// <summary>Authorization tag is not matching.</summary>
        STATUS_AUTH_TAG_MISMATCH = 0xc000a002,
        /// <summary>Memory mapped files: Some files are not mapped.</summary>
        STATUS_SOME_NOT_MAPPED = 0x00000107,
        /// <summary>The operation would cause an arithmetic overflow.</summary>
        STATUS_BUFFER_OVERFLOW = 0x80000005,
        /// <summary></summary>
        STATUS_NO_MORE_FILES = 0x80000006,
        /// <summary>The specified object was not found.</summary>
        STATUS_OBJECT_NAME_NOT_FOUND = 0xC0000034,
        /// <summary>Memory mapped files: No files are mapped.</summary>
        STATUS_NONE_MAPPED = 0xC0000073,
        /// <summary>Not enough resources to complete the operation.</summary>
        STATUS_INSUFFICIENT_RESOURCES = 0xC000009A,
        /// <summary>Access to the resource wrapped by the function is denied.</summary>
        STATUS_ACCESS_DENIED = 0xC0000022,
        /// <summary>There is an active account resctriction and as such, the specified function cannot be used.</summary>
        STATUS_ACCOUNT_RESTRICTION = 0xc000006e,
        /// <summary>I/O: The specified file cannot be found.</summary>
        STATUS_FILE_NOT_FOUND = 0xC000000F,
        /// <summary>I/O: The file %hs does not exist.</summary>
        STATUS_NO_SUCH_FILE = 0xC000000F,
        /// <summary>I/O: The end-of-file marker has been reached. There is no valid data in the file beyond this marker.</summary>
        STATUS_END_OF_FILE = 0xC0000011,
    }
}