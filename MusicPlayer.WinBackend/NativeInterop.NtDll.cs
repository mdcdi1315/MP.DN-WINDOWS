
using MP;
using System;
using MP.NativeInterop.Windows;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;

partial class Interop
{
    public unsafe static partial class NtDll
    {
        [Flags]
        public enum Suite : System.UInt16
        {
            VER_SUITE_SMALLBUSINESS = 0x00000001,
            VER_SUITE_ENTERPRISE = 0x00000002,
            VER_SUITE_BACKOFFICE = 0x00000004,
            VER_SUITE_COMMUNICATIONS = 0x00000008,
            VER_SUITE_TERMINAL = 0x00000010,
            VER_SUITE_SMALLBUSINESS_RESTRICTED = 0x00000020,
            VER_SUITE_EMBEDDEDNT = 0x00000040,
            VER_SUITE_DATACENTER = 0x00000080,
            VER_SUITE_SINGLEUSERTS = 0x00000100,
            VER_SUITE_PERSONAL = 0x00000200,
            VER_SUITE_BLADE = 0x00000400,
            VER_SUITE_EMBEDDED_RESTRICTED = 0x00000800,
            VER_SUITE_SECURITY_APPLIANCE = 0x00001000,
            VER_SUITE_STORAGE_SERVER = 0x00002000,
            VER_SUITE_COMPUTE_SERVER = 0x00004000,
            VER_SUITE_WH_SERVER = 0x00008000,
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct RTL_OSVERSIONINFOEXW
        {
            public System.UInt32 OSVersionInfoSize;

            public System.UInt32 Major;

            public System.UInt32 Minor;

            public System.UInt32 BuildNumber;

            public System.UInt32 PlatformID;

            private fixed System.Char _CSDVersion[128];

            public System.UInt16 ServicePackMajor;

            public System.UInt16 ServicePackMinor;

            public Suite SuiteMask;

            public System.Byte ProductType;

            public System.Byte RSVD;

            public readonly System.String CSDVersion
            {
                get {
                    fixed (System.Char* verpack = _CSDVersion) {
                        return new System.String(verpack);
                    }
                }
            }

            public readonly Version Version => new(Major.ToInt32(), Minor.ToInt32(), BuildNumber.ToInt32() , 0);

            public static RTL_OSVERSIONINFOEXW CreateStruct()
            {
                RTL_OSVERSIONINFOEXW cr = new();
                cr.OSVersionInfoSize = sizeof(RTL_OSVERSIONINFOEXW).ToUInt32();
                Unsafe.InitBlockUnaligned(cr._CSDVersion, 0, 128 * sizeof(System.Char));
                return cr;
            }
        }

        [DllImport(Libraries.NtDll, ExactSpelling = true)]
        public static extern System.UInt32 RtlNtStatusToDosError(NTSTATUS Status);

        [DllImport(Libraries.NtDll, EntryPoint = "RtlGetVersion", ExactSpelling = true)]
        private static extern NTSTATUS RtlGetVersion_Native(RTL_OSVERSIONINFOEXW* ptr);

        public static RTL_OSVERSIONINFOEXW RtlGetVersion()
        {
            RTL_OSVERSIONINFOEXW data = RTL_OSVERSIONINFOEXW.CreateStruct();
            NTSTATUS s = RtlGetVersion_Native(&data);
            MP.ExceptionSystem.NativeWindowsException.ThrowIfError(s);
            return data;
        }
    }
}


