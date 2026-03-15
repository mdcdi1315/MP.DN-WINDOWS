using System;
using MP.Annotations;
using MP.NativeInterop.Windows;
using System.Runtime.InteropServices;

partial class Interop
{
    public static unsafe partial class Kernel32
    {
        [AssignsLastError]
        [DllImport(Libraries.Kernel32, ExactSpelling = true)]
        public static extern BOOL CloseHandle(IntPtr handle);

        [DllImport(Libraries.Kernel32, ExactSpelling = true)]
        public static extern System.IntPtr GetCurrentProcess();

        public enum DuplicateHandleOptions : System.UInt32
        {
            None = 0,
            DUPLICATE_CLOSE_SOURCE = 0x00000001,
            DUPLICATE_SAME_ACCESS = 0x00000002,
        }

        [AssignsLastError]
        [DllImport(Libraries.Kernel32, EntryPoint = "DuplicateHandle")]
        private static extern BOOL DuplicateHandle_Native(
            IntPtr processhandle, 
            IntPtr srchandle,
            IntPtr targetprocesshandle,
            IntPtr* targethandle,
            System.UInt32 accessflags,
            BOOL inherithandle,
            DuplicateHandleOptions options);

        public static BOOL DuplicateHandleInCurrentProcess(System.IntPtr src , out System.IntPtr duplicated)
        {
            System.IntPtr currentproc = GetCurrentProcess();
            System.IntPtr dup;
            BOOL ret = DuplicateHandle_Native(currentproc , src , currentproc , &dup , 0 , BOOL.TRUE , DuplicateHandleOptions.DUPLICATE_SAME_ACCESS);
            CloseHandle(currentproc);
            duplicated = dup;
            return ret;
        }

        public static BOOL DuplicateHandleInCurrentForOtherProcess(System.IntPtr targetproc , System.IntPtr src , out System.IntPtr duplicated)
        {
            System.IntPtr currentproc = GetCurrentProcess();
            System.IntPtr dup;
            BOOL ret = DuplicateHandle_Native(currentproc, src, targetproc, &dup, 0, BOOL.TRUE, DuplicateHandleOptions.DUPLICATE_SAME_ACCESS);
            CloseHandle(currentproc);
            duplicated = dup;
            return ret;
        }

        public static System.IntPtr GetRealCurrentProcess()
        {
            System.IntPtr src = GetCurrentProcess();
            System.IntPtr ret;
            if (DuplicateHandleInCurrentProcess(src, out ret) == BOOL.FALSE) { 
                CloseHandle(src);
                throw new AggregateException("Failed to create a duplicate handle for the current process."); 
            }
            CloseHandle(src);
            return ret;
        }
    }
}
