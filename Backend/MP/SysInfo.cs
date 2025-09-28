using System;
using Microsoft.Win32;
using System.Diagnostics.CodeAnalysis;

namespace MP
{
    public sealed class SystemInfo_Windows : AbstractPlatformLayer
    {
        private sealed class WindowsMemHandleFactory : MemoryHandleFactory, IDisposable
        {
            private MemoryHeap hp;

            public IMemoryHandle CreateMemoryHandle(ulong size) => (hp ??= MemoryHeap.Create()).Allocate(size.ToInt32());

            [return: MaybeNull]
            public IAttributeable GetStatistics()
            {
                throw new System.NotImplementedException();
            }

            public void Dispose()
            {
                hp?.Dispose();
            }
        }

        private WindowsMemHandleFactory fac;

        protected override void UnloadLayer()
        {
            fac?.Dispose();
            fac = null;
            base.UnloadLayer();
        }

        public override Platform Platform => Platform.Windows;

        public override MemoryHandleFactory GetMemoryHandleFactory() => fac ??= new WindowsMemHandleFactory();

        public override System.String UserName => Interop.Advapi32.GetUserName();

        public override System.String CurrentProcessDirectory => Interop.Kernel32.GetProcessDirectory();

        public override System.String CurrentDirectory
        {
            get => Interop.Kernel32.GetCurrentDirectory();
            set => Interop.Kernel32.SetCurrentDirectory(value);
        }

        public override System.Boolean IsVirtualMachine => Interop.Kernel32.IsNativeVhdBoot();

        public override System.DateTime Now => Interop.Kernel32.GetSystemTimeAsFileTime().ToDateTimeOffset().LocalDateTime;

        public override System.DateTime UtcNow => Interop.Kernel32.GetSystemTimeAsFileTime().ToDateTimeUtc();

        public override System.String OSDirectory => Interop.Kernel32.GetSystemWindowsDirectory();

        public override System.String SystemDirectory => Interop.Kernel32.GetSystemDirectory();

        public override System.String ComputerName => Interop.Kernel32.GetComputerNameEx(Interop.Kernel32.COMPUTER_NAME_FORMAT.ComputerNameNetBIOS);

        public override System.String SystemFirmwareType => Interop.Kernel32.GetFirmwareType() switch {
            Interop.Kernel32.FIRMWARE_TYPE.FirmwareTypeBios => "BIOS",
            Interop.Kernel32.FIRMWARE_TYPE.FirmwareTypeUefi => "UEFI",
            _ => "UNKNOWN"
        };

        public override System.Version OperatingSystemVersion => Interop.NtDll.RtlGetVersion().Version;

        public override System.UInt32 PageSize => Interop.Kernel32.GetNativeSystemInfo().PageSize;

        public override ProcessorArchitecture ProcessorArchitecture => Interop.Kernel32.GetNativeSystemInfo().ProcessorArchitecture switch {
            Interop.Kernel32.PROCESSOR_ARCHITECTURE.PROCESSOR_ARCHITECTURE_INTEL => ProcessorArchitecture.X86,
            Interop.Kernel32.PROCESSOR_ARCHITECTURE.PROCESSOR_ARCHITECTURE_ARM => ProcessorArchitecture.ARM32,
            Interop.Kernel32.PROCESSOR_ARCHITECTURE.PROCESSOR_ARCHITECTURE_AMD64 => ProcessorArchitecture.AMD64,
            Interop.Kernel32.PROCESSOR_ARCHITECTURE.PROCESSOR_ARCHITECTURE_ARM64 => ProcessorArchitecture.ARM64,
            Interop.Kernel32.PROCESSOR_ARCHITECTURE.PROCESSOR_ARCHITECTURE_IA64 => ProcessorArchitecture.IA64,
            _ => ProcessorArchitecture.Unknown
        };

        public override System.Guid HardwareProfileID => new(Interop.Advapi32.GetCurrentHwProfile().ProfileGuid);

        public override System.String HardwareProfile => Interop.Advapi32.GetCurrentHwProfile().ProfileName;

        public override System.String GetComputerName(ComputerNameTypes type) => Interop.Kernel32.GetComputerNameEx((Interop.Kernel32.COMPUTER_NAME_FORMAT)type);

        public override System.UInt16 ActiveProcessorGroupCount => Interop.Kernel32.GetActiveProcessorGroupCount();

        public override System.UInt16 MaximumProcessorGroupCount => Interop.Kernel32.GetMaximumProcessorGroupCount();

        public override System.Int32 GetProcessorCount(System.UInt16 group) => Interop.Kernel32.GetActiveProcessorCount(group);

        public System.Int32 ProcessorMask => Interop.Kernel32.GetNativeSystemInfo().ProcessorMask;

        public System.UInt16 ProcessorLevel => Interop.Kernel32.GetNativeSystemInfo().ProcessorLevel;

        public System.UInt16 ProcessorRevision => Interop.Kernel32.GetNativeSystemInfo().ProcessorRevision;

        public override unsafe System.Boolean HasAdminPriviledges
        {
            get
            {
                void* adminsid;
                if (Interop.Advapi32.AllocateAndInitializeSid(Interop.Advapi32.SID_IDENTIFIER_AUTHORITY.SECURITY_NT_AUTHORITY,
                    [Interop.Advapi32.SECURITY_BUILTIN_DOMAIN_RID , Interop.Advapi32.DOMAIN_ALIAS_RID_ADMINS] , &adminsid) == Interop.BOOL.FALSE) 
                {
                    throw new MP.ExceptionSystem.NativeWindowsException();
                }
                Interop.BOOL member , ret;
                System.Int32 err;
                System.IntPtr tokencurrentproc = Interop.Kernel32.GetCurrentProcessToken();
                ret = Interop.Advapi32.CheckTokenMembership(tokencurrentproc, adminsid , out member);
                err = Interop.Kernel32.GetLastError();
                Interop.Kernel32.CloseHandle(tokencurrentproc);
                Interop.Advapi32.FreeSid(adminsid); // Free the allocated SID pointer even if the native call was failed.
                if (ret == Interop.BOOL.FALSE) { throw new MP.ExceptionSystem.NativeWindowsException(err); }
                return member == Interop.BOOL.TRUE;
            }
        }

        public override System.String GetKnownFolder(ShellKnownFolder folder)
        {
            if (folder >= ShellKnownFolder.ShellKnownFolderMax) { throw new System.ArgumentOutOfRangeException(nameof(folder) , "A valid known folder must be selected!"); }
            Interop.GUID guid = Interop.GUID.FromString(folder switch { 
                ShellKnownFolder.Desktop => InternalResources.Shell32_KF_Desktop,
                ShellKnownFolder.SendTo => InternalResources.Shell32_KF_SendTo,
                ShellKnownFolder.Pictures => InternalResources.Shell32_KF_Pictures,
                ShellKnownFolder.Music => InternalResources.Shell32_KF_Music,
                ShellKnownFolder.Windows => InternalResources.Shell32_KF_Windows,
                ShellKnownFolder.CDBurning => InternalResources.Shell32_KF_CDBurning,
                ShellKnownFolder.Ringtones => InternalResources.Shell32_KF_Ringtones,
                ShellKnownFolder.Profile => InternalResources.Shell32_KF_Profile,
                ShellKnownFolder.Screenshots => InternalResources.Shell32_KF_Screenshots,
                ShellKnownFolder.System => InternalResources.Shell32_KF_System,
                ShellKnownFolder.SystemX86 => InternalResources.Shell32_KF_SystemX86,
                ShellKnownFolder.Videos => InternalResources.Shell32_KF_Videos,
                ShellKnownFolder.Documents => InternalResources.Shell32_KF_Documents,
                ShellKnownFolder.Downloads => InternalResources.Shell32_KF_Downloads,
                _ => null
            });
            System.String ret;
            Interop.Shell32.SHGetKnownFolderPath(guid, out ret).ThrowOnFailure();
            return ret;
        }

        /// <summary>
        /// Gets a known folder by using a Shell's GUID identifier. <br />
        /// The identifiers can be found at <see href="https://learn.microsoft.com/en-us/windows/win32/shell/knownfolderid"/>.
        /// </summary>
        /// <param name="guid">The shell's GUID to the corresponding known folder to get.</param>
        /// <returns>The known folder path.</returns>
        public System.String GetKnownFolderFromGuid(System.Guid guid)
        {
            Interop.Shell32.SHGetKnownFolderPath(Interop.GUID.FromGUID(guid), out System.String path).ThrowOnFailure();
            return path;
        }

        public override System.String GetEnvironmentVariable(System.String name) => Interop.Kernel32.GetEnvironmentVariable(name);

        public override void SetEnvironmentVariable(System.String name , System.String value) => Interop.Kernel32.SetEnvironmentVariable(name, value);

        public override System.String ExpandEnvironmentVariables(System.String format) => Interop.Kernel32.ExpandEnvironmentStrings(format);

        public override EnvironmentVariable[] GetEnvironmentVariables()
        {
            System.String[] vars = Interop.Kernel32.GetEnvironmentStrings();
            if (vars.Length == 0) { return System.Array.Empty<EnvironmentVariable>(); }
            EnvironmentVariable[] variables = new EnvironmentVariable[vars.Length];
            System.String name , value; 
            for (System.Int32 I = 0; I < vars.Length; I++) 
            {
                System.Int32 idx = vars[I].IndexOf('=');
                if (idx == -1) { throw new System.FormatException("The environment variable layout had an invalid format."); }
                value = vars[I].Substring(idx + 1);
                name = vars[I].Remove(idx);
                variables[I] = new(name , value);
            }
            vars = null;
            return variables;
        }
    }
}
