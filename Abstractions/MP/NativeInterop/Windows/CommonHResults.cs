

using System.Runtime.Versioning;

namespace MP.NativeInterop.Windows
{
    /// <summary>
    /// Provides common <see cref="HRESULT"/> codes that may occur on COM calls.
    /// </summary>
    public static class CommonHResults
    {
        /// <summary>Success.</summary>
        public const System.UInt32 S_OK = 0x00000000;

        /// <summary>The call succeeded, but returned <see langword="false"/>.</summary>
        public const System.UInt32 S_FALSE = 0x00000001;

        /// <summary>Catastrophic failure</summary>
        public const System.UInt32 E_UNEXPECTED = 0x8000FFFF;

        /// <summary>Not implemented</summary>
        public const System.UInt32 E_NOTIMPL = 0x80004001;

        /// <summary>Ran out of memory</summary>
        public const System.UInt32 E_OUTOFMEMORY = 0x8007000E;

        /// <summary>One or more arguments are invalid</summary>
        public const System.UInt32 E_INVALIDARG = 0x80070057;

        /// <summary>No such interface supported</summary>
        public const System.UInt32 E_NOINTERFACE = 0x80004002;

        /// <summary>Invalid pointer</summary>
        public const System.UInt32 E_POINTER = 0x80004003;

        /// <summary>Invalid handle</summary>
        public const System.UInt32 E_HANDLE = 0x80070006;

        /// <summary>Operation aborted</summary>
        public const System.UInt32 E_ABORT = 0x80004004;

        /// <summary>Unspecified error</summary>
        public const System.UInt32 E_FAIL = 0x80004005;

        /// <summary>General access denied error</summary>
        public const System.UInt32 E_ACCESSDENIED = 0x80070005;

        /// <summary>The data necessary to complete this operation is not yet available.</summary>
        public const System.UInt32 E_PENDING = 0x8000000A;

        /// <summary>The operation attempted to access data outside the valid range</summary>
        public const System.UInt32 E_BOUNDS = 0x8000000B;

        /// <summary>A concurrent or interleaved operation changed the state of the object, invalidating this operation.</summary>
        public const System.UInt32 E_CHANGEDSTATE = 0x8000000C;

        /// <summary>An illegal state change was requested.</summary>
        public const System.UInt32 E_ILLEGAL_STATE_CHANGE = 0x8000000D;

        /// <summary>A method was called at an unexpected time.</summary>
        public const System.UInt32 E_ILLEGAL_METHOD_CALL = 0x8000000E;

        /// <summary>String not null terminated.</summary>
        public const System.UInt32 E_STRING_NOT_NULL_TERMINATED = unchecked((System.UInt32)0x80000017E);

        /// <summary>A delegate was assigned when not allowed.</summary>
        public const System.UInt32 E_ILLEGAL_DELEGATE_ASSIGNMENT = 0x80000018;

        /// <summary>The application is exiting and cannot service this request</summary>
        public const System.UInt32 E_APPLICATION_EXITING = 0x8000001A;

        /// <summary>The application view is exiting and cannot service this request</summary>
        public const System.UInt32 E_APPLICATION_VIEW_EXITING = 0x8000001B;

        /// <summary>The object must support the <see cref="COM.IAgileObject"/> interface</summary>
        [SupportedOSPlatform(WindowsVersions.NTDDI_WIN8)]
        public const System.UInt32 RO_E_MUST_BE_AGILE = 0x8000001C;

        /// <summary>Activating a single-threaded class from <see cref="System.Threading.ApartmentState.MTA"/> is not supported</summary>
        public const System.UInt32 RO_E_UNSUPPORTED_FROM_MTA = 0x8000001D;

        /// <summary>Unable to perform requested operation.</summary>
        public const System.UInt32 STG_E_INVALIDFUNCTION = 0x80030001;

        /// <summary>%1 could not be found.</summary>
        public const System.UInt32 STG_E_FILENOTFOUND = 0x80030002;

        /// <summary>The path %1 could not be found.</summary>
        public const System.UInt32 STG_E_PATHNOTFOUND = 0x80030003;

        /// <summary>There are insufficient resources to open another file.</summary>
        public const System.UInt32 STG_E_TOOMANYOPENFILES = 0x80030004;

        /// <summary>Access Denied.</summary>
        public const System.UInt32 STG_E_ACCESSDENIED = 0x80030005;

        /// <summary>Attempted an operation on an invalid object.</summary>
        public const System.UInt32 STG_E_INVALIDHANDLE = 0x80030006;

        /// <summary>There is insufficient memory available to complete operation.</summary>
        public const System.UInt32 STG_E_INSUFFICIENTMEMORY = 0x80030008;

        /// <summary>Invalid pointer error.</summary>
        public const System.UInt32 STG_E_INVALIDPOINTER = 0x80030009;

        /// <summary>There are no more entries to return.</summary>
        public const System.UInt32 STG_E_NOMOREFILES = 0x80030012;

        /// <summary>Disk is write-protected.</summary>
        public const System.UInt32 STG_E_DISKISWRITEPROTECTED = 0x80030013;

        /// <summary>An error occurred during a seek operation.</summary>
        public const System.UInt32 STG_E_SEEKERROR = 0x80030019;

        /// <summary>A disk error occurred during a write operation.</summary>
        public const System.UInt32 STG_E_WRITEFAULT = 0x8003001D;

        /// <summary>A disk error occurred during a read operation.</summary>
        public const System.UInt32 STG_E_READFAULT = 0x8003001E;

        /// <summary>A share violation has occurred.</summary>
        public const System.UInt32 STG_E_SHAREVIOLATION = 0x80030020;

        /// <summary>Invalid parameter error.</summary>
        public const System.UInt32 STG_E_INVALIDPARAMETER = 0x80030057;

        /// <summary>There is insufficient disk space to complete operation.</summary>
        public const System.UInt32 STG_E_MEDIUMFULL = 0x80030070;

        /// <summary>An unexpected error occurred.</summary>
        public const System.UInt32 STG_E_UNKNOWN = 0x800300FD;

        /// <summary>That function is not implemented.</summary>
        public const System.UInt32 STG_E_UNIMPLEMENTEDFUNCTION = 0x800300FE;

        /// <summary>Invalid flag error.</summary>
        public const System.UInt32 STG_E_INVALIDFLAG = 0x800300FF;

        /// <summary>Out of present range.</summary>
        public const System.UInt32 DISP_E_OVERFLOW = 0x8002000A;

        /// <summary>Type mismatch.</summary>
        public const System.UInt32 DISP_E_TYPEMISMATCH = 0x80020005;

        /// <summary>Bad variable type.</summary>
        public const System.UInt32 DISP_E_BADVARTYPE = 0x80020008;
    }
}