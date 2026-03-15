
using MP.IO;
using System;
using MP.Annotations;
using MP.NativeInterop.Windows;
using System.Runtime.Versioning;
using System.Runtime.InteropServices;

using unsafe LP_PROGRESS_ROUTINE = delegate* unmanaged[Stdcall]<System.Int64, System.Int64, System.Int64, System.Int64, System.UInt32, System.UInt32, System.IntPtr, System.IntPtr, void*, System.UInt32>;

partial class Interop
{
    unsafe partial class Kernel32
    {
        [Flags]
        public enum CopyFileExFlags : System.UInt32
        {
            None = 0,
            COPY_FILE_ALLOW_DECRYPTED_DESTINATION = 0x00000008,
            COPY_FILE_COPY_SYMLINK = 0x00000800,
            COPY_FILE_FAIL_IF_EXISTS = 0x00000001,
            COPY_FILE_NO_BUFFERING = 0x00001000,
            COPY_FILE_OPEN_SOURCE_FOR_WRITE = 0x00000004,
            COPY_FILE_RESTARTABLE = 0x00000002,
            [SupportedOSPlatform(WindowsVersions.NTDDI_WIN10_MN)]
            COPY_FILE_REQUEST_COMPRESSED_TRAFFIC = 0x10000000
        }

        [AssignsLastError]
        [DllImport(Libraries.Kernel32, EntryPoint = "CopyFileExW", ExactSpelling = true)]
        private static extern BOOL CopyFileExW_Native(
            System.Char* source,
            System.Char* dest,
            LP_PROGRESS_ROUTINE progress = null,
            void* lpData = null,
            BOOL* pCancel = null,
            CopyFileExFlags flags = CopyFileExFlags.None
        );

        public static BOOL CopyFileEx(System.String source, System.String dest, CopyFileExFlags flags, LP_PROGRESS_ROUTINE progress = null, void* data = null, BOOL* cancel = null)
        {
            fixed (System.Char* p_src = NativePathManipulations.EnsureExtendedPrefixIfNeeded(source))
            fixed (System.Char* p_dst = NativePathManipulations.EnsureExtendedPrefixIfNeeded(dest))
            {
                return CopyFileExW_Native(p_src, p_dst, progress, data, cancel, flags);
            }
        }
    }
}