
using System;
using MP.NativeInterop;
using System.Collections;
using System.Runtime.InteropServices;
using System.Diagnostics.CodeAnalysis;

namespace MP
{
    /// <summary>
    /// Provides a default platform layer to use with the Abstractions library. <br />
    /// This platform layer actually forwards to .NET common calls that are known to be supported on all platforms. <br />
    /// This is primarily intended for developers using several library components outside of the Music Player platform
    /// and they use only clean and transparent .NET API's.
    /// </summary>
    // I will not mark this with NativeLayer attribute, since it uses API's defined by .NET itself.
    // However, we do need to suppress the platform layer marking.
    [SuppressMessage("MDCDI1315.APIUSAGE", "MPAL00003", Justification = "Forwards to transparent .NET calls")]
    public sealed class DefaultPlatformLayer : AbstractPlatformLayer
    {
        private Platform currentplatform;
        private ProcessorArchitecture architecture;

        private unsafe sealed class InteropServicesMemoryFactory : INativeMemoryManager
        {
            [return: NotNull]
            public void* Allocate(ulong size) => NativeMemory.Alloc(new(size));

            public bool Free([AllowNull] void* pointer)
            {
                if (pointer is null) {
                    return false;
                } else {
                    NativeMemory.Free(pointer);
                    return true;
                }
            }

            // We don't need this when using NativeMemory.
            public ulong GetSize([AllowNull] void* pointer) => throw new NotSupportedException();

            public void* ReAllocate([AllowNull] void* old, ulong size) => NativeMemory.Realloc(old, new(size));

            public void* AllocateAligned(System.UInt64 size, System.UInt32 align) => NativeMemory.AlignedAlloc(new(size), new(align));

            public void* ReAllocateAligned([AllowNull] void* old, System.UInt64 size, System.UInt64 align) => NativeMemory.AlignedRealloc(old, new(size), new(align));

            public System.Boolean FreeAligned([AllowNull] void* pointer)
            {
                if (pointer is null) {
                    return false;
                } else {
                    NativeMemory.AlignedFree(pointer);
                    return true;
                }
            }
        }

        /// <summary>
        /// Destroys internal state used by the platform layer.
        /// </summary>
        protected override void UnloadLayer() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultPlatformLayer"/> class.
        /// </summary>
        public DefaultPlatformLayer() {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows)) {
                currentplatform = Platform.Windows;
            } else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux)) {
                currentplatform = Platform.Unix;
            } else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX)) {
                currentplatform = Platform.OSX;
            } else if (RuntimeInformation.IsOSPlatform(OSPlatform.FreeBSD)) {
                currentplatform = Platform.BSD;
            } else {
                // Possibly it will be a unix system
                currentplatform = Platform.Unix;
            }
            architecture = RuntimeInformation.OSArchitecture switch {
                Architecture.X86 => ProcessorArchitecture.X86,
                Architecture.X64 => ProcessorArchitecture.AMD64,
                Architecture.Arm => ProcessorArchitecture.ARM32,
                Architecture.Arm64 => ProcessorArchitecture.ARM64,
                Architecture.Armv6 => ProcessorArchitecture.ARM32,
                _ => throw new NotImplementedException($"NOT IMPLEMENTED FOR THIS CASE: {RuntimeInformation.OSArchitecture}")
            };
            RegisterMemoryManager(DefaultMemoryManager, new InteropServicesMemoryFactory());
        }

        /// <inheritdoc />
        public override string UserName => Environment.UserName;

        /// <inheritdoc />
        public override string CurrentProcessDirectory => AppDomain.CurrentDomain.BaseDirectory;

        /// <inheritdoc />
        public override string CurrentDirectory
        {
            get => Environment.CurrentDirectory;
            set => Environment.CurrentDirectory = value;
        }

        /// <inheritdoc />
        public override DateTime Now => DateTime.Now;

        /// <inheritdoc />
        public override DateTime UtcNow => DateTime.UtcNow;

        /// <inheritdoc />
        public override string SystemDirectory => Environment.SystemDirectory;

        /// <inheritdoc />
        public override Version OperatingSystemVersion => Environment.OSVersion.Version;

        /// <inheritdoc />
        public override uint PageSize => Environment.SystemPageSize.ToUInt32();

        /// <inheritdoc />
        public override Platform Platform => currentplatform;

        /// <summary>This call is not supported and will always throw <see cref="NotSupportedException"/>.</summary>
        public override ProcessorArchitecture ProcessorArchitecture => architecture;

        /// <summary>This call is not supported and will always return <see langword="false"/>.</summary>
        public override bool HasAdminPriviledges => false;

        /// <inheritdoc />
        public override string ExpandEnvironmentVariables(string format) => Environment.ExpandEnvironmentVariables(format);

        /// <summary>This call is not supported and will always throw <see cref="NotSupportedException"/>.</summary>
        public override string GetComputerName(ComputerNameTypes type) => throw new NotSupportedException("Computer Name Types cannot be retrieved via the .NET API's.");

        /// <inheritdoc />
        public override string GetEnvironmentVariable(string name) => Environment.GetEnvironmentVariable(name);

        /// <inheritdoc />
        public override EnvironmentVariable[] GetEnvironmentVariables()
        {
            var p = Environment.GetEnvironmentVariables(EnvironmentVariableTarget.Process);
            EnvironmentVariable[] vret = new EnvironmentVariable[p.Count];
            int I = 0;
            var e = p.GetEnumerator();
            DictionaryEntry de;
            while (e.MoveNext())
            {
                de = e.Entry;
                vret[I] = new(de.Key.ToString(), de.Value.ToString());
                I++;
            }
            return vret;
        }

        /// <inheritdoc />
        public override string GetKnownFolder(ShellKnownFolder folder) => Environment.GetFolderPath(
            folder switch
            {
                ShellKnownFolder.Desktop => Environment.SpecialFolder.Desktop,
                ShellKnownFolder.SendTo => Environment.SpecialFolder.SendTo,
                ShellKnownFolder.Documents => Environment.SpecialFolder.MyDocuments,
                ShellKnownFolder.System => Environment.SpecialFolder.System,
                ShellKnownFolder.SystemX86 => Environment.SpecialFolder.SystemX86,
                ShellKnownFolder.Windows => Environment.SpecialFolder.Windows,
                ShellKnownFolder.Music => Environment.SpecialFolder.MyMusic,
                ShellKnownFolder.Videos => Environment.SpecialFolder.MyVideos,
                ShellKnownFolder.Ringtones => throw new NotSupportedException("Not supported by .NET API's"),
                ShellKnownFolder.CDBurning => Environment.SpecialFolder.CDBurning,
                ShellKnownFolder.Downloads => throw new NotSupportedException("Not supported by .NET API's"),
                ShellKnownFolder.Screenshots => throw new NotSupportedException("Not supported by .NET API's"),
                ShellKnownFolder.Profile => Environment.SpecialFolder.UserProfile,
                ShellKnownFolder.Pictures => Environment.SpecialFolder.MyPictures,
                _ => throw new NotSupportedException($"This enumeration case is not supported: {folder}")
            }
        );

        /// <inheritdoc />
        public override void SetEnvironmentVariable(string name, string value) => Environment.SetEnvironmentVariable(name, value);
    }
}