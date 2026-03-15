
using MP.NativeInterop.Windows;
using System.Runtime.InteropServices;

partial class Interop
{
    unsafe partial class NtDll
    {
        [StructLayout(LayoutKind.Sequential)]
        public struct IO_STATUS_BLOCK
        {
            /// <summary>Status data</summary>
            public IO_STATUS Status;

            /// <summary>Request dependent value.</summary>
            public System.UIntPtr Information;
        }

        // This isn't an actual Windows type, it is a union within IO_STATUS_BLOCK. We *have* to separate it out as
        // the size of IntPtr varies by architecture and we can't specify the size at compile time to offset the
        // Information pointer in the status block.
        [StructLayout(LayoutKind.Explicit)]
        public struct IO_STATUS
        {
            /// <summary>
            /// The completion status, either STATUS_SUCCESS if the operation was completed successfully or
            /// some other informational, warning, or error status.
            /// </summary>
            [FieldOffset(0)]
            public NTSTATUS Status;

            /// <summary>
            /// Reserved for internal use.
            /// </summary>
            [FieldOffset(0)]
            public System.IntPtr Pointer;
        }
    }
}