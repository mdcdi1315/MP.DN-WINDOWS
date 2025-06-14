using MP;
using System;
using System.IO;
using System.Collections.Generic;
using System.Runtime.InteropServices;

partial class Interop
{
    unsafe partial class Kernel32
    {
        [DllImport(Libraries.Kernel32 , EntryPoint = "GetCurrentDirectoryW" , SetLastError = true)]
        private static extern System.UInt32 GetCurrentDirectory_Native(System.UInt32 bufferlength , System.Char* buffer);

        public static System.String GetCurrentDirectory() 
        {
            System.UInt32 size = GetCurrentDirectory_Native(0, null);
            if (size == 0) {
                throw new MP.ExceptionSystem.NativeWindowsException();
            }
            size += 2; // Add +2 to ensure that the buffer will fit the entire string contents.
            System.Char[] nativech = new System.Char[size];
            System.UInt32 fs;
            fixed (System.Char* buffer = nativech)
            {
                if ((fs = GetCurrentDirectory_Native(size, buffer)) == 0) {
                    throw new MP.ExceptionSystem.NativeWindowsException();
                }
            }
            System.Int32 idx = 0;
            // Avoid the 32767 path prefix to be shown to the user.
            if (fs > 4 && new System.String(nativech , 0 ,4) == PathInternal.ExtendedPathPrefix) { idx = 4; }
            return new System.String(nativech , idx , (fs - idx).ToInt32());
        }

        [DllImport(Libraries.Kernel32, EntryPoint = "SetCurrentDirectoryW" , SetLastError = true)]
        private static extern BOOL SetCurrentDirectory_Native(System.Char* buffer);

        public static void SetCurrentDirectory(System.String path) 
        {
            // Ensure that the string will be terminated with C's \0.
            path = PathInternal.EnsureExtendedPrefix(path) + "\0";
            fixed (System.Char* buffer = path)
            {
                if (SetCurrentDirectory_Native(buffer) == BOOL.FALSE) {
                    throw new MP.ExceptionSystem.NativeWindowsException();
                }
            }
        }

        [DllImport(Libraries.Kernel32 , EntryPoint = "IsNativeVhdBoot" , SetLastError = true)]
        private static extern BOOL IsNativeVhdBoot_Native(BOOL* vhdboot);

        public static System.Boolean IsNativeVhdBoot()
        {
            BOOL ret;
            if (IsNativeVhdBoot_Native(&ret) == BOOL.FALSE) {
                throw new MP.ExceptionSystem.NativeWindowsException();
            }
            return ret == BOOL.TRUE;
        }

        [DllImport(Libraries.Kernel32, EntryPoint = "GetSystemTimeAsFileTime" , SetLastError = false)]
        private static extern void GetSystemTimeAsFileTime_Native(FILETIME* ft);

        public static FILETIME GetSystemTimeAsFileTime()
        {
            FILETIME current;
            GetSystemTimeAsFileTime_Native(&current);
            return current;
        }

        public enum PROCESSOR_ARCHITECTURE : System.UInt16
        {
            /// <summary>
            /// x64 (AMD or Intel)
            /// </summary>
            PROCESSOR_ARCHITECTURE_AMD64 = 9,
            /// <summary>
            /// ARM
            /// </summary>
            PROCESSOR_ARCHITECTURE_ARM = 5,
            /// <summary>
            /// ARM64
            /// </summary>
            PROCESSOR_ARCHITECTURE_ARM64 = 12,
            /// <summary>
            /// Intel Itanium-based
            /// </summary>
            PROCESSOR_ARCHITECTURE_IA64 = 6,
            /// <summary>
            /// x86
            /// </summary>
            PROCESSOR_ARCHITECTURE_INTEL = 0,
            PROCESSOR_ARCHITECTURE_UNKNOWN = 0xffff
        }

        [StructLayout(LayoutKind.Explicit , Pack = 2 , Size = 36)]
        public struct SYSTEM_INFO
        {
            [FieldOffset(0)]
            public PROCESSOR_ARCHITECTURE ProcessorArchitecture;

            [FieldOffset(2)]
            public System.UInt16 RSVD;

            [FieldOffset(4)]
            public System.UInt32 PageSize;

            [FieldOffset(8)]
            public System.Int32 MinimumAppAddressPtr;

            [FieldOffset(12)]
            public System.Int32 MaximumAppAddressPtr;

            [FieldOffset(16)]
            public System.Int32 ProcessorMaskPtr;

            [FieldOffset(20)]
            public System.UInt32 ActiveProccessors;

            [FieldOffset(24)]
            public System.UInt32 ProcessorType;

            [FieldOffset(28)]
            public System.UInt32 AllocationGranularity;

            [FieldOffset(32)]
            public System.UInt16 ProcessorLevel;

            [FieldOffset(34)]
            public System.UInt16 ProcessorRevision;

            public readonly System.Int32 ProcessorMask => new IntPtr(ProcessorMaskPtr).ToInt32();
        }

        [DllImport(Libraries.Kernel32 , EntryPoint = "GetNativeSystemInfo" , SetLastError = false)]
        private static extern void GetNativeSystemInfo_Native(SYSTEM_INFO* sysinfoptr);

        public static SYSTEM_INFO GetNativeSystemInfo()
        {
            SYSTEM_INFO inf = default;
            GetNativeSystemInfo_Native(&inf);
            return inf;
        }

        [DllImport(Libraries.Kernel32 , EntryPoint = "GetSystemWindowsDirectoryW" , SetLastError = true)]
        private static extern System.UInt32 GetSystemWindowsDirectory_Native(System.Char* buffer, System.UInt32 size);

        public static System.String GetSystemWindowsDirectory()
        {
            System.String result = new('\0', 512);
            fixed (System.Char* buffer = result) 
            {
                if (GetSystemWindowsDirectory_Native(buffer , result.Length.ToUInt32()) == 0) {
                    throw new MP.ExceptionSystem.NativeWindowsException();
                }
            }
            return FixedBufferExtensions.GetStringFromFixedBuffer(result);
        }

        [DllImport(Libraries.Kernel32 , EntryPoint = "GetSystemDirectoryW" , SetLastError = true)]
        private static extern System.UInt32 GetSystemDirectory_Native(System.Char* buffer, System.UInt32 size);

        public static System.String GetSystemDirectory() 
        {
            System.String result = new('\0', 512);
            fixed (System.Char* buffer = result)
            {
                if (GetSystemDirectory_Native(buffer , result.Length.ToUInt32()) == 0) {
                    throw new MP.ExceptionSystem.NativeWindowsException();
                }
            }
            return FixedBufferExtensions.GetStringFromFixedBuffer(result);
        }

        public enum COMPUTER_NAME_FORMAT : System.Int32
        {
            ComputerNameNetBIOS,
            ComputerNameDnsHostname,
            ComputerNameDnsDomain,
            ComputerNameDnsFullyQualified,
            ComputerNamePhysicalNetBIOS,
            ComputerNamePhysicalDnsHostname,
            ComputerNamePhysicalDnsDomain,
            ComputerNamePhysicalDnsFullyQualified,
            ComputerNameMax
        }

        [DllImport(Libraries.Kernel32 , EntryPoint = "GetComputerNameExW" , SetLastError = true)]
        private static extern BOOL GetComputerNameEx_Native(COMPUTER_NAME_FORMAT format , System.Char* buffer, System.UInt32* bufsize);
    
        public static System.String GetComputerNameEx(COMPUTER_NAME_FORMAT format)
        {
            System.Char[] ret = new System.Char[512];
            System.UInt32* sizeinout = stackalloc System.UInt32[1];
            *sizeinout = ret.Length.ToUInt32();
            fixed (System.Char* buffer = ret)
            {
                if (GetComputerNameEx_Native(format , buffer , sizeinout) == BOOL.FALSE) {
                    throw new MP.ExceptionSystem.NativeWindowsException();
                }
            }
            return new(ret, 0, (*sizeinout).ToInt32());
        }

        public enum FIRMWARE_TYPE : System.Int32
        {
            FirmwareTypeUnknown,
            FirmwareTypeBios,
            FirmwareTypeUefi,
            FirmwareTypeMax
        }

        [DllImport(Libraries.Kernel32 , EntryPoint = "GetFirmwareType" , SetLastError = true)]
        private static extern BOOL GetFirmwareType_Native(FIRMWARE_TYPE* type);

        public static FIRMWARE_TYPE GetFirmwareType()
        {
            FIRMWARE_TYPE tp = FIRMWARE_TYPE.FirmwareTypeUnknown;
            if (GetFirmwareType_Native(&tp) == BOOL.FALSE) {
                throw new MP.ExceptionSystem.NativeWindowsException();
            }
            return tp;
        }

        [StructLayout(LayoutKind.Explicit , Pack = 1 , Size = 12)]
        public struct SYSTEM_POWER_STATUS
        {
            [FieldOffset(0)]
            public System.Byte ACLineStatus;

            [FieldOffset(1)]
            public System.Byte BatteryFlag;

            [FieldOffset(2)]
            public System.Byte BatteryLifePercent;

            [FieldOffset(3)]
            public System.Byte SystemStatusFlag;

            [FieldOffset(4)]
            public System.UInt32 BatteryLifeTime;

            [FieldOffset(8)]
            public System.UInt32 BatteryFullLifeTime;
        }

        [DllImport(Libraries.Kernel32 , EntryPoint = "GetSystemPowerStatus" , SetLastError = true)]
        private static extern BOOL GetSystemPowerStatus_Native(SYSTEM_POWER_STATUS* state);

        public static SYSTEM_POWER_STATUS GetSystemPowerStatus()
        {
            SYSTEM_POWER_STATUS status = default;
            if (GetSystemPowerStatus_Native(&status) == BOOL.FALSE) 
            {
                throw new MP.ExceptionSystem.NativeWindowsException();
            }
            return status;
        }

        [DllImport(Libraries.Kernel32 , SetLastError = false)]
        public static extern System.UInt16 GetActiveProcessorGroupCount();

        [DllImport(Libraries.Kernel32, SetLastError = false)]
        public static extern System.UInt16 GetMaximumProcessorGroupCount();

        [DllImport(Libraries.Kernel32, SetLastError = false , EntryPoint = "GetActiveProcessorCount")]
        private static extern System.UInt32 GetActiveProcessorCount_Native(System.UInt16 groupidx);

        public static System.Int32 GetActiveProcessorCount(System.UInt16 group)
        {
            System.UInt32 GCNT;
            if ((GCNT = GetActiveProcessorCount_Native(group)) == 0) {
                throw new MP.ExceptionSystem.NativeWindowsException();
            }
            return GCNT.ToInt32();
        }

        public static System.String GetProcessDirectory()
        {
            System.Char[] ret = new System.Char[32767];
            System.UInt32 fs;
            fixed (System.Char* p = ret) 
            {
                fs = GetModuleFileName_Native(IntPtr.Zero , p , ret.Length.ToUInt32());
                if (fs == 0) { throw new MP.ExceptionSystem.NativeWindowsException(); }
            }
            System.Int32 idx = fs.ToInt32();
            for (System.Int32 I = idx - 1; I >= 0; I--) 
            {
                if (ret[I] == '\\') { idx = I; break; }
            }
            return new(ret, 0, idx);
        }

        public static System.IntPtr GetCurrentProcessToken() => new(-4);

        [DllImport(Libraries.Kernel32 , EntryPoint = "ExpandEnvironmentStringsW" , SetLastError = true)]
        private static extern System.UInt32 ExpandEnvironmentStrings_Native(System.Char* input , System.Char* output , System.UInt32 outsize);

        public static System.String ExpandEnvironmentStrings(System.String input)
        {
            input += "\0";
            System.String output = new('\0', 512);
            System.UInt32 fs;
            fixed (System.Char* pout = output)
            fixed (System.Char* pin = input)
            {
                fs = ExpandEnvironmentStrings_Native(pin, pout, output.Length.ToUInt32());
            }
            if (fs == 0) { throw new MP.ExceptionSystem.NativeWindowsException(); }
            return output.Remove((fs - 1).ToInt32());
        }

        [DllImport(Libraries.Kernel32 , EntryPoint = "GetEnvironmentVariableW" , SetLastError = true)]
        private static extern System.UInt32 GetEnvironmentVariable_Native(System.Char* name, System.Char* bufferout, System.UInt32 outsize);

        public static System.String GetEnvironmentVariable(System.String name)
        {
            System.String ret = new('\0', 32767);
            System.UInt32 fs;
            fixed (System.Char* pout = ret)
            fixed (System.Char* pin = name)
            {
                fs = GetEnvironmentVariable_Native(pin, pout, ret.Length.ToUInt32());
            }
            if (fs == 0) {
                System.Int32 err = GetLastError();
                if (err == Errors.ERROR_ENVVAR_NOT_FOUND) 
                {
                    throw new ArgumentException("The environment variable was not found." , nameof(name));
                }
                throw new MP.ExceptionSystem.NativeWindowsException(err);
            }
            return ret.Remove(fs.ToInt32());
        }

        [DllImport(Libraries.Kernel32 , EntryPoint = "SetEnvironmentVariableW" , SetLastError = true)]
        private static extern BOOL SetEnvironmentVariable_Native(System.Char* name, System.Char* value);

        public static void SetEnvironmentVariable(System.String name , System.String value)
        {
            BOOL ret;
            fixed (System.Char* pnam = name)
            fixed (System.Char* pval = value)
            {
                ret = SetEnvironmentVariable_Native(pnam, pval);
            }
            if (ret == BOOL.FALSE) { throw new MP.ExceptionSystem.NativeWindowsException(); }
        }

        [DllImport(Libraries.Kernel32 , EntryPoint = "FileTimeToSystemTime" , ExactSpelling = true , SetLastError = true)]
        private static extern BOOL FileTimeToSystemTime_Native(FILETIME* filetime, SYSTEMTIME* systime);

        public static BOOL FileTimeToSystemTime(FILETIME filetime, out SYSTEMTIME systime)
        {
            SYSTEMTIME outret;
            BOOL ret = FileTimeToSystemTime_Native(&filetime, &outret);
            systime = outret;
            return ret;
        }

        [DllImport(Libraries.Kernel32 , EntryPoint = "SystemTimeToFileTime" , ExactSpelling = true , SetLastError = true)]
        private static extern BOOL SystemTimeToFileTime_Native(SYSTEMTIME* systemtime , FILETIME* filetime);

        public static BOOL SystemTimeToFileTime(SYSTEMTIME systemtime, out FILETIME filetime)
        {
            FILETIME outret;
            BOOL ret = SystemTimeToFileTime_Native(&systemtime, &outret);
            filetime = outret;
            return ret;
        }

        [DllImport(Libraries.Kernel32 , EntryPoint = "GetEnvironmentStringsW" , ExactSpelling = true)]
        private static extern System.Char* GetEnvironmentStrings_Native();

        [DllImport(Libraries.Kernel32 , EntryPoint = "FreeEnvironmentStringsW" , ExactSpelling = true , SetLastError = true)]
        private static extern BOOL FreeEnvironmentStrings_Native(System.Char* penv);

        public static System.String[] GetEnvironmentStrings()
        {
            System.Char* pptr = GetEnvironmentStrings_Native();
            if (pptr is null) { return System.Array.Empty<System.String>(); }
            System.Char* penvpointer = pptr;
            List<System.String> strings = new();
            System.Text.StringBuilder sb = new(100);
            System.Char p0;
            // Initially save all the found variables
            while (true)
            {
                p0 = *penvpointer;
                if (p0 == '\0') {
                    strings.Add(sb.ToString());
                    sb.Clear();
                    if (penvpointer[1] == '\0') { break; } // If this is occured the last string will have been saved too.
                } else {
                    sb.Append(p0);
                }
                penvpointer++;
            }
            // Free the used environment block.
            if (FreeEnvironmentStrings_Native(pptr) == BOOL.FALSE) { throw new MP.ExceptionSystem.NativeWindowsException(); }
            sb = null; // Free the string builder we do not need it anymore
            // We may have hidden environment variables at the first strings , strip them out from the result
            for (System.Int32 I = 0; I < strings.Count; I++)
            {
                if (strings[I][0] == '=') {
                    // Hidden variable, remove it and re-run the loop from the start
                    strings.RemoveAt(I);
                    I = -1;
                } else {
                    // We do not have any other such variables , so break the loop
                    break;
                }
            }
            return strings.ToArray();
        }

    }
}