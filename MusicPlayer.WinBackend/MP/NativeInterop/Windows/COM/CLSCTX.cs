

namespace MP.NativeInterop.Windows.COM
{
    [System.Flags]
    public enum CLSCTX : System.UInt32
    {
        /// <summary>
        /// The code that creates and manages objects of this class is a DLL that runs in the same process as the caller of the function specifying the class context.
        /// </summary>
        CLSCTX_INPROC_SERVER = 0x1,
        /// <summary>
        /// The code that manages objects of this class is an in-process handler. This is a DLL that runs in the client process and implements client-side structures of this class when instances of the class are accessed remotely.
        /// </summary>
        CLSCTX_INPROC_HANDLER = 0x2,
        /// <summary>
        /// The EXE code that creates and manages objects of this class runs on same machine but is loaded in a separate process space.
        /// </summary>
        CLSCTX_LOCAL_SERVER = 0x4,
        /// <summary>
        /// Obsolete.
        /// </summary>
        CLSCTX_INPROC_SERVER16 = 0x8,
        /// <summary>
        /// A remote context. The LocalServer32 or LocalService code that creates and manages objects of this class is run on a different computer.
        /// </summary>
        CLSCTX_REMOTE_SERVER = 0x10,
        /// <summary>
        /// Obsolete.
        /// </summary>
        CLSCTX_INPROC_HANDLER16 = 0x20,
        CLSCTX_RESERVED1 = 0x40,
        CLSCTX_RESERVED2 = 0x80,
        CLSCTX_RESERVED3 = 0x100,
        CLSCTX_RESERVED4 = 0x200,
        /// <summary>
        /// Disables the downloading of code from the directory service or the Internet. This flag cannot be set at the same time as <see cref="CLSCTX_ENABLE_CODE_DOWNLOAD"/>.
        /// </summary>
        CLSCTX_NO_CODE_DOWNLOAD = 0x400,
        CLSCTX_RESERVED5 = 0x800,
        /// <summary>
        /// Specify if you want the activation to fail if it uses custom marshalling.
        /// </summary>
        CLSCTX_NO_CUSTOM_MARSHAL = 0x1000,
        /// <summary>
        /// Enables the downloading of code from the directory service or the Internet. This flag cannot be set at the same time as <see cref="CLSCTX_NO_CODE_DOWNLOAD"/>.
        /// </summary>
        CLSCTX_ENABLE_CODE_DOWNLOAD = 0x2000,
        CLSCTX_NO_FAILURE_LOG = 0x4000,
        CLSCTX_DISABLE_AAA = 0x8000,
        CLSCTX_ENABLE_AAA = 0x10000,
        CLSCTX_FROM_DEFAULT_CONTEXT = 0x20000,
        CLSCTX_ACTIVATE_X86_SERVER = 0x40000,
        CLSCTX_ACTIVATE_32_BIT_SERVER,
        CLSCTX_ACTIVATE_64_BIT_SERVER = 0x80000,
        CLSCTX_ENABLE_CLOAKING = 0x100000,
        CLSCTX_APPCONTAINER = 0x400000,
        CLSCTX_ACTIVATE_AAA_AS_IU = 0x800000,
        CLSCTX_RESERVED6 = 0x1000000,
        CLSCTX_ACTIVATE_ARM32_SERVER = 0x2000000,
        CLSCTX_ALLOW_LOWER_TRUST_REGISTRATION,
        CLSCTX_PS_DLL = 0x80000000,
        // Custom values down here
        CLSCTX_INPROC = CLSCTX_INPROC_SERVER | CLSCTX_INPROC_HANDLER,
        CLSCTX_SERVER = CLSCTX_INPROC_SERVER | CLSCTX_LOCAL_SERVER | CLSCTX_REMOTE_SERVER,
        CLSCTX_ALL = CLSCTX_SERVER | CLSCTX_INPROC_HANDLER
    }
}