
using MP.Annotations;
using System.Runtime.InteropServices;

partial class Interop
{
    partial class Kernel32
    {
        public enum WaitObjectReturnValue : System.UInt32
        {
            /// <summary>
            /// The object was signaled.
            /// </summary>
            WAIT_OBJECT_0 = 0x00000000,
            WAIT_ABANDONED = 0x00000080,
            WAIT_TIMEOUT = 0x00000102,
            /// <summary>
            /// The call failed, call <see cref="GetLastError"/>.
            /// </summary>
            WAIT_FAILED = 0xFFFFFFFF
        }

        [AssignsLastError]
        [DllImport(Libraries.Kernel32 , ExactSpelling = true)]
        public static extern WaitObjectReturnValue WaitForSingleObject(System.IntPtr targetobj , System.UInt32 millisecondstowait);
    }
}