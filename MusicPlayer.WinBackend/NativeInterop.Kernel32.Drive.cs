
using MP;
using MP.Utilities;
using MP.Collections;
using MP.Annotations;
using MP.NativeInterop.Windows;
using System.Runtime.InteropServices;

partial class Interop
{
    public static unsafe partial class Kernel32
    {
        [AssignsLastError]
        [DllImport(Libraries.Kernel32, EntryPoint = "GetLogicalDriveStringsW", ExactSpelling = true)]
        private static extern System.UInt32 GetLogicalDriveStrings_Native(
            System.UInt32 nbufferlength,
            System.Char* p_buffer
        );

        [RequiresNativeLayer]
        public static System.String[] GetLogicalDriveStrings()
        {
            System.UInt32 length = 1000U, u_length;
            System.Char* p = (System.Char*)SystemInfo.GetDefaultMemoryManager().Allocate(length);
        g_retry:
            u_length = GetLogicalDriveStrings_Native(length, p);
            if (u_length == 0U) {
                return null;
            } else if (u_length > length) {
                p = (System.Char*)SystemInfo.GetDefaultMemoryManager().ReAllocate(p, length = u_length);
                goto g_retry;
            }
            try {
                ArrayBuilder<System.String> b = new();
                System.Text.StringBuilder sb = new();
                System.Char* pc = p;
                while (true)
                {
                    if (*pc == '\0') {
                        b.Add(sb.ToStringAndClear());
                        if (pc[1] == '\0') { break; }
                    } else {
                        sb.Append(*pc);
                    }
                    pc++;
                }
                return b.Build();
            } finally {
                SystemInfo.GetDefaultMemoryManager().Free(p);
            }
        }

        [AssignsLastError]
        [DllImport(Libraries.Kernel32, EntryPoint = "SetVolumeLabelW", ExactSpelling = true)]
        private static extern BOOL SetVolumeLabel_Native(System.Char* driveLetter, System.Char* volumeName);

        public static BOOL SetVolumeLabel(System.String driveLetter, System.String volumeName)
        {
            fixed (System.Char* p_drive = driveLetter)
            fixed (System.Char* p_volume = volumeName) {
                return SetVolumeLabel_Native(p_drive, p_volume);
            }
        }
    }
}
