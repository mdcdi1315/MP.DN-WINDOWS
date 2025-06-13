

namespace MP.ComInterop
{
    /// <summary>
    /// Provides common <see cref="HRESULT"/> codes that may occur on COM calls.
    /// </summary>
    public static class CommonHResults
    {
        public const System.Int32 S_OK = 0x00000000;

        public const System.Int32 S_FALSE = 0x00000001;

        public const System.Int32 E_UNEXPECTED = unchecked((System.Int32)0x8000FFFF);

        public const System.Int32 E_NOTIMPL = unchecked((System.Int32)0x80004001);

        public const System.Int32 E_OUTOFMEMORY = unchecked((System.Int32)0x8007000E);

        public const System.Int32 E_INVALIDARG = unchecked((System.Int32)0x80070057);

        public const System.Int32 E_NOINTERFACE = unchecked((System.Int32)0x80004002);

        public const System.Int32 E_POINTER = unchecked((System.Int32)0x80004003);

        public const System.Int32 E_HANDLE = unchecked((System.Int32)0x80070006);

        public const System.Int32 E_ABORT = unchecked((System.Int32)0x80004004);

        public const System.Int32 E_FAIL = unchecked((System.Int32)0x80004005);

        public const System.Int32 E_ACCESSDENIED = unchecked((System.Int32)0x80070005);

        public const System.Int32 E_PENDING = unchecked((System.Int32)0x8000000A);

        public const System.Int32 E_BOUNDS = unchecked((System.Int32)0x8000000B);

        public const System.Int32 E_CHANGEDSTATE = unchecked((System.Int32)0x8000000C);

        public const System.Int32 E_ILLEGAL_STATE_CHANGE = unchecked((System.Int32)0x8000000D);

        public const System.Int32 E_ILLEGAL_METHOD_CALL = unchecked((System.Int32)0x8000000E);

        public const System.Int32 E_STRING_NOT_NULL_TERMINATED = unchecked((System.Int32)0x80000017E);

        public const System.Int32 STG_E_INVALIDFUNCTION = unchecked((System.Int32)0x80030001);

        public const System.Int32 STG_E_FILENOTFOUND = unchecked((System.Int32)0x80030002);

        public const System.Int32 STG_E_PATHNOTFOUND = unchecked((System.Int32)0x80030003);

        public const System.Int32 STG_E_TOOMANYOPENFILES = unchecked((System.Int32)0x80030004);

        public const System.Int32 STG_E_ACCESSDENIED = unchecked((System.Int32)0x80030005);

        public const System.Int32 STG_E_INVALIDHANDLE = unchecked((System.Int32)0x80030006);

        public const System.Int32 STG_E_INSUFFICIENTMEMORY = unchecked((System.Int32)0x80030008);

        public const System.Int32 STG_E_INVALIDPOINTER = unchecked((System.Int32)0x80030009);

        public const System.Int32 STG_E_NOMOREFILES = unchecked((System.Int32)0x80030012);

        public const System.Int32 STG_E_DISKISWRITEPROTECTED = unchecked((System.Int32)0x80030013);

        public const System.Int32 STG_E_SEEKERROR = unchecked((System.Int32)0x80030019);

        public const System.Int32 STG_E_WRITEFAULT = unchecked((System.Int32)0x8003001D);

        public const System.Int32 STG_E_READFAULT = unchecked((System.Int32)0x8003001E);

        public const System.Int32 STG_E_SHAREVIOLATION = unchecked((System.Int32)0x80030020);

        public const System.Int32 STG_E_UNKNOWN = unchecked((System.Int32)0x800300FD);

        public const System.Int32 STG_E_UNIMPLEMENTEDFUNCTION = unchecked((System.Int32)0x800300FE);

        public const System.Int32 STG_E_INVALIDFLAG = unchecked((System.Int32)0x800300FF);

        public const System.Int32 DISP_E_OVERFLOW = unchecked((System.Int32)0x8002000A);

        public const System.Int32 DISP_E_TYPEMISMATCH = unchecked((System.Int32)0x80020005);

        public const System.Int32 DISP_E_BADVARTYPE = unchecked((System.Int32)0x80020008);


    }
}