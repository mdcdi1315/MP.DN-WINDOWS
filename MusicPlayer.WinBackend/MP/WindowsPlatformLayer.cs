
using System;
using MP.Utilities;
using MP.Annotations;
using MP.NativeInterop.Windows;
using MP.NativeInterop.Windows.COM;
using System.Diagnostics.CodeAnalysis;

namespace MP
{
    [NativeLayer]
    public sealed class WindowsPlatformLayer : AbstractPlatformLayer
    {
        private sealed class WinMemoryManager : NativeInterop.INativeMemoryManager, IDisposable
        {
            private System.IntPtr handle;

            public WinMemoryManager()
            {
                handle = Interop.Kernel32.HeapCreate(Interop.Kernel32.HeapCreateFlags.HEAP_GENERATE_EXCEPTIONS, 0UL, 0UL);
                if (handle == IntPtr.Zero) {
                    throw new ExceptionSystem.NativeWindowsException();
                }
            }

            [return: NotNull]
            public unsafe void* Allocate(ulong size)
            {
                void* p = Interop.Kernel32.HeapAlloc(handle, Interop.Kernel32.HeapAllocFlags.None, size);
                if (p is null) {
                    throw new InsufficientMemoryException("Not enough memory to complete the allocation!");
                } else {
                    return p;
                }
            }

            public void Dispose()
            {
                if (handle == IntPtr.Zero) { return; }
                if (Interop.Kernel32.HeapDestroy(handle) == NativeInterop.Windows.BOOL.FALSE) {
                    throw new InvalidOperationException("Cannot destroy the native MP heap!!!");
                }
                handle = IntPtr.Zero;
            }

            public unsafe bool Free([AllowNull] void* pointer)
            {
                if (pointer is null) {
                    return false;
                } else {
                    return Interop.Kernel32.HeapFree(handle, Interop.Kernel32.HeapFreeFlags.None, pointer) != NativeInterop.Windows.BOOL.FALSE;
                }
            }

            public unsafe ulong GetSize([AllowNull] void* pointer)
            {
                if (pointer is null) {
                    return 0UL;
                } else {
                    ulong s = Interop.Kernel32.HeapSize(handle, Interop.Kernel32.HeapFreeFlags.None, pointer);
                    if (s == System.UInt64.MaxValue) {
                        return 0UL;
                    } else {
                        return s;
                    }
                }
            }

            [return: NotNull]
            public unsafe void* ReAllocate([AllowNull] void* old, ulong size)
            {
                void* p;
                if (old is null) {
                    p = Interop.Kernel32.HeapAlloc(handle, Interop.Kernel32.HeapAllocFlags.None, size);
                } else {
                    p = Interop.Kernel32.HeapReAlloc(handle, Interop.Kernel32.HeapReAllocFlags.None, old, size);
                }
                if (p is null) {
                    throw new InsufficientMemoryException("Not enough memory to complete the allocation!");
                } else {
                    return p;
                }
            }

            [SuppressMessage("Interoperability", "CA1416", Justification = "Already validated that will be called only on Windows 10 20348+")]
            public Optional<IAttributeable> GetStatistics()
            {
                var vi = Interop.NtDll.RtlGetVersion();
                if (vi.Major < 10 || vi.BuildNumber < 20348) {
                    return Optional<IAttributeable>.Empty();
                } else {
                    BOOL g = Interop.Kernel32.HeapSummary(handle, out var summary);
                    if (g == BOOL.FALSE) {
                        return Optional<IAttributeable>.Empty();
                    } else {
                        return Optional<IAttributeable>.Of(
                            new Collections.Record([
                                    new Collections.AttributeKeyValuePair("Allocated", summary.cbAllocated),
                                    new Collections.AttributeKeyValuePair("Commited", summary.cbCommitted),
                                    new Collections.AttributeKeyValuePair("Reserved", summary.cbReserved),
                                    new Collections.AttributeKeyValuePair("MaxReservedMemory", summary.cbMaxReserve),
                            ])
                        );
                    }
                }
            }
        }

        public WindowsPlatformLayer()
        {
            RegisterMemoryManager(DefaultMemoryManager, new WinMemoryManager());
            RegisterMemoryManager(COMMemoryManager.NAME, WindowsCOMLibrary.COMMemoryManager);
        }

        protected override void UnloadLayer()
        {
            ((IDisposable)GetDefaultMemoryManager()).Dispose();
        }

        public override string UserName => throw new NotImplementedException();

        public override string CurrentProcessDirectory => Interop.Kernel32.GetProcessDirectory();

        public override string CurrentDirectory 
        { 
            get => Interop.Kernel32.GetCurrentDirectory(); 
            set {
                ArgumentNullException.ThrowIfNull(value);
                Interop.Kernel32.SetCurrentDirectory(value);
            }
        }

        public override DateTime Now => DateTime.FromFileTime(Interop.Kernel32.GetSystemTimeAsFileTime().ToTicks());

        public override DateTime UtcNow => Interop.Kernel32.GetSystemTimeAsFileTime().ToDateTimeUtc();

        public override string SystemDirectory => Interop.Kernel32.GetSystemDirectory();

        public override System.Boolean IsVirtualMachine => Interop.Kernel32.IsNativeVhdBoot();

        public override Version OperatingSystemVersion => Interop.NtDll.RtlGetVersion().Version;

        public System.Int32 ProcessorMask => Interop.Kernel32.GetNativeSystemInfo().ProcessorMask;

        public System.UInt16 ProcessorLevel => Interop.Kernel32.GetNativeSystemInfo().ProcessorLevel;

        public System.UInt16 ProcessorRevision => Interop.Kernel32.GetNativeSystemInfo().ProcessorRevision;

        public override System.UInt16 ActiveProcessorGroupCount => Interop.Kernel32.GetActiveProcessorGroupCount();

        public override System.UInt16 MaximumProcessorGroupCount => Interop.Kernel32.GetMaximumProcessorGroupCount();

        public override System.String SystemFirmwareType => Interop.Kernel32.GetFirmwareType() switch
        {
            Interop.Kernel32.FIRMWARE_TYPE.FirmwareTypeBios => "BIOS",
            Interop.Kernel32.FIRMWARE_TYPE.FirmwareTypeUefi => "UEFI",
            _ => "UNKNOWN"
        };

        public override uint PageSize => Interop.Kernel32.GetNativeSystemInfo().PageSize;

        public override ProcessorArchitecture ProcessorArchitecture => Interop.Kernel32.GetNativeSystemInfo().ProcessorArchitecture switch {
            Interop.Kernel32.PROCESSOR_ARCHITECTURE.PROCESSOR_ARCHITECTURE_INTEL => ProcessorArchitecture.X86,
            Interop.Kernel32.PROCESSOR_ARCHITECTURE.PROCESSOR_ARCHITECTURE_ARM => ProcessorArchitecture.ARM32,
            Interop.Kernel32.PROCESSOR_ARCHITECTURE.PROCESSOR_ARCHITECTURE_AMD64 => ProcessorArchitecture.AMD64,
            Interop.Kernel32.PROCESSOR_ARCHITECTURE.PROCESSOR_ARCHITECTURE_ARM64 => ProcessorArchitecture.ARM64,
            Interop.Kernel32.PROCESSOR_ARCHITECTURE.PROCESSOR_ARCHITECTURE_IA64 => ProcessorArchitecture.IA64,
            _ => ProcessorArchitecture.Unknown
        };

        public override bool HasAdminPriviledges => throw new NotImplementedException();

        public override Platform Platform => Platform.Windows;

        public override string ExpandEnvironmentVariables(string format)
        {
            ArgumentNullException.ThrowIfNull(format);
            return Interop.Kernel32.ExpandEnvironmentStrings(format);   
        }

        public override System.String GetComputerName(ComputerNameTypes type) => Interop.Kernel32.GetComputerNameEx((Interop.Kernel32.COMPUTER_NAME_FORMAT)type);

        public override string GetEnvironmentVariable(string name)
        {
            ArgumentNullException.ThrowIfNull(name);
            return Interop.Kernel32.GetEnvironmentVariable(name);
        }

        public override EnvironmentVariable[] GetEnvironmentVariables()
        {
            throw new NotImplementedException();
        }

        public override string GetKnownFolder(ShellKnownFolder folder)
        {
            throw new NotImplementedException();
        }

        public override void SetEnvironmentVariable(string name, string value)
        {
            ArgumentNullException.ThrowIfNull(name);
            Interop.Kernel32.SetEnvironmentVariable(name, value);
        }
    }
}
