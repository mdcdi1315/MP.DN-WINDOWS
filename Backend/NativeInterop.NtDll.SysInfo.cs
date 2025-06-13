using MP;
using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;


partial class Interop
{
    public unsafe static partial class NtDll
    {
        [StructLayout(LayoutKind.Explicit , Pack = 1 , Size = 283)]
        public struct RTL_OSVERSIONINFOEXW
        {
            [FieldOffset(0)]
            public System.UInt32 OSVersionInfoSize;

            [FieldOffset(4)]
            public System.UInt32 Major;

            [FieldOffset(8)]
            public System.UInt32 Minor;

            [FieldOffset(12)]
            public System.UInt32 BuildNumber;

            [FieldOffset(16)]
            public System.UInt32 PlatformID;

            [FieldOffset(20)]
            private fixed System.Char _CSDVersion[128];

            [FieldOffset(276)]
            public System.UInt16 ServicePackMajor;

            [FieldOffset(278)]
            public System.UInt16 ServicePackMinor;

            [FieldOffset(280)]
            public System.UInt16 SuiteMask;

            [FieldOffset(281)]
            public System.Byte ProductType;

            [FieldOffset(282)]
            public System.Byte RSVD;

            public readonly System.String CSDVersion
            {
                get {
                    fixed (System.Char* verpack = _CSDVersion) {
                        return new System.String(verpack);
                    }
                }
            }

            public readonly System.Version Version => new(Major.ToInt32(), Minor.ToInt32(), BuildNumber.ToInt32() , 0);

            public static RTL_OSVERSIONINFOEXW CreateStruct()
            {
                RTL_OSVERSIONINFOEXW cr = new();
                cr.OSVersionInfoSize = sizeof(RTL_OSVERSIONINFOEXW).ToUInt32();
                Unsafe.InitBlockUnaligned(cr._CSDVersion, 0, 128 * sizeof(System.Char));
                return cr;
            }
        }

        [DllImport(Libraries.NtDll , EntryPoint = "RtlGetVersion" , SetLastError = false)]
        private static extern NTSTATUS RtlGetVersion_Native(RTL_OSVERSIONINFOEXW* ptr);

        public static RTL_OSVERSIONINFOEXW RtlGetVersion()
        {
            RTL_OSVERSIONINFOEXW data = RTL_OSVERSIONINFOEXW.CreateStruct();
            RtlGetVersion_Native(&data);
            return data;
        }
    }
}


