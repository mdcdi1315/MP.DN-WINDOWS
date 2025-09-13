using MP;
using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

partial class Interop
{
    public unsafe static partial class Advapi32 
    {
        public const System.UInt32 SECURITY_BUILTIN_DOMAIN_RID = 0x00000020;
        public const System.UInt32 DOMAIN_ALIAS_RID_ADMINS = 0x00000220;

        public const System.UInt32 STANDARD_RIGHTS_REQUIRED = 0x000F0000;
        public const System.UInt32 STANDARD_RIGHTS_ALL = 0x001F0000;

        [DllImport(Libraries.Advapi32 , EntryPoint = "GetUserNameW" , SetLastError = true)]
        private static extern BOOL GetUserName_Native(System.Char* buffer, System.UInt32* buffersize);

        public static System.String GetUserName()
        {
            System.Char[] ret = new System.Char[512];
            System.UInt32* sizeinout = stackalloc System.UInt32[1];
            *sizeinout = ret.Length.ToUInt32();
            fixed (System.Char* outbuf = ret)
            {
                if (GetUserName_Native(outbuf , sizeinout) == BOOL.FALSE) {
                    throw new MP.ExceptionSystem.NativeWindowsException();
                }
            }
            return new System.String(ret , 0 , (*sizeinout - 1).ToInt32());
        }

        [StructLayout(LayoutKind.Explicit, Pack = 4, Size = 244)]
        public struct HW_PROFILE_INFO
        {
            [FieldOffset(0)]
            public System.UInt32 DockInfo;

            [FieldOffset(4)]
            private fixed System.Char HWProfileGuid[39];

            public readonly System.String ProfileGuid
            {
                get {
                    fixed (System.Char* dt = HWProfileGuid) { return new(dt); }
                }
            }

            [FieldOffset(82)]
            private fixed System.Char HWProfileName[80];

            public readonly System.String ProfileName
            {
                get {
                    fixed (System.Char* dt = HWProfileName) { return new(dt); }
                }
            }
        }

        [DllImport(Libraries.Advapi32 , EntryPoint = "GetCurrentHwProfileW" , SetLastError = true)]
        private static extern BOOL GetCurrentHwProfile_Native(HW_PROFILE_INFO* profileptr);

        public static HW_PROFILE_INFO GetCurrentHwProfile()
        {
            HW_PROFILE_INFO prf = default;
            if (GetCurrentHwProfile_Native(&prf) == BOOL.FALSE) 
            {
                throw new MP.ExceptionSystem.NativeWindowsException();
            }
            return prf;
        }

        [StructLayout(LayoutKind.Explicit , Size = 6 , Pack = 1)]
        public struct SID_IDENTIFIER_AUTHORITY
        {
            [FieldOffset(0)]
            public System.Byte ID1;

            [FieldOffset(1)]
            public System.Byte ID2;

            [FieldOffset(2)]
            public System.Byte ID3;

            [FieldOffset(3)]
            public System.Byte ID4;

            [FieldOffset(4)]
            public System.Byte ID5;

            [FieldOffset(5)]
            public System.Byte ID6;

            // Based on winnt.h header , just setting ID6 field with 5 means the NT authority. Weird.
            public static SID_IDENTIFIER_AUTHORITY SECURITY_NT_AUTHORITY => new() { ID6 = 5 };
        }

        [DllImport(Libraries.Advapi32 , EntryPoint = "AllocateAndInitializeSid" , SetLastError = true)]
        private static extern BOOL AllocateAndInitializeSid_Native(
            SID_IDENTIFIER_AUTHORITY* pauthority,
            System.Byte SubAuthorityCount,
            System.UInt32 A0,
            System.UInt32 A1,
            System.UInt32 A2,
            System.UInt32 A3,
            System.UInt32 A4,
            System.UInt32 A5,
            System.UInt32 A6,
            System.UInt32 A7,
            void** ptrsid);

        // BEWARE ON THE sidorcreated parameter! it returns a pointer to a pointer.
        // The best practice here is to init the sid structure by not assigning to the variable any value.
        public static BOOL AllocateAndInitializeSid(SID_IDENTIFIER_AUTHORITY authority , System.UInt32[] subauthorities , void** sidcreated)
        {
            if (subauthorities is null) { return BOOL.FALSE; }
            if (subauthorities.Length > 8) { return BOOL.FALSE; }
            System.Byte sucount = subauthorities.Length.ToByte();
            System.UInt32[] sunative = new System.UInt32[8];
            fixed (System.UInt32* dst = sunative)
            fixed (System.UInt32* src = subauthorities)
            {
                Unsafe.CopyBlockUnaligned(dst, src, (subauthorities.Length * sizeof(System.UInt32)).ToUInt32());
            }
            return AllocateAndInitializeSid_Native(
                &authority, 
                sucount,
                sunative[0],
                sunative[1],
                sunative[2],
                sunative[3],
                sunative[4],
                sunative[5],
                sunative[6],
                sunative[7],
                sidcreated);
        }

        [DllImport(Libraries.Advapi32 , EntryPoint = "FreeSid")]
        private static extern void* FreeSid_Native(void* psid);

        public static System.Boolean FreeSid(void* sid) => FreeSid_Native(sid) is null;

        [DllImport(Libraries.Advapi32 , EntryPoint = "CheckTokenMembership" , SetLastError = true)]
        private static extern BOOL CheckTokenMembership_Native(System.IntPtr tokhand, void* sid, BOOL* ismember);

        public static BOOL CheckTokenMembership(System.IntPtr token, void* sid , out BOOL member)
        {
            BOOL memret = BOOL.FALSE , ret;
            ret = CheckTokenMembership_Native(token , sid , &memret);
            member = memret;
            return ret;
        }
    
        public enum ACCESS_TOKEN_RIGHTS : System.UInt32
        {
            TOKEN_ASSIGN_PRIMARY = 0x0001,
            TOKEN_DUPLICATE = 0x0002,
            TOKEN_IMPERSONATE = 0x0004,
            TOKEN_QUERY = 0x0008,
            TOKEN_QUERY_SOURCE = 0x0010,
            TOKEN_ADJUST_PRIVILEGES = 0x0020,
            TOKEN_ADJUST_GROUPS = 0x0040,
            TOKEN_ADJUST_DEFAULT = 0x0080,
            TOKEN_ADJUST_SESSIONID = 0x0100,
            TOKEN_ALL_ACCESS_P = STANDARD_RIGHTS_REQUIRED |
                          TOKEN_ASSIGN_PRIMARY      |
                          TOKEN_DUPLICATE           |
                          TOKEN_IMPERSONATE         |
                          TOKEN_QUERY               |
                          TOKEN_QUERY_SOURCE        |
                          TOKEN_ADJUST_PRIVILEGES   |
                          TOKEN_ADJUST_GROUPS       |
                          TOKEN_ADJUST_DEFAULT,
            TOKEN_ALL_ACCESS = TOKEN_ALL_ACCESS_P | TOKEN_ADJUST_SESSIONID,
        }

        [DllImport(Libraries.Advapi32 , EntryPoint = "OpenProcessToken" , SetLastError = true)]
        private static extern BOOL OpenProcessToken_Native(System.IntPtr processhand , ACCESS_TOKEN_RIGHTS rights , System.IntPtr* outtokhandle);

        public static BOOL OpenProcessToken(System.IntPtr processhandle , ACCESS_TOKEN_RIGHTS rights , out System.IntPtr token)
        {
            System.IntPtr tok;
            BOOL ret = OpenProcessToken_Native(processhandle, rights, &tok);
            token = tok;
            return ret;
        }
    }
}