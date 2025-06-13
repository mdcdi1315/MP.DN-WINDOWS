
using System.Runtime.Versioning;

namespace MP
{
    /// <summary>
    /// Returns various information about the current operating system that the app runs upon. <br />
    /// Because that information is differently retrieved between operating systems, one must use the
    /// <see cref="RegisterPlatformLayer(AbstractPlatformLayer)"/> method with a valid instance to define the platform conventions.
    /// </summary>
    [Annotations.NativeLayer]
    public static class SystemInfo
    {
        private static AbstractPlatformLayer apl;

        private static void UnloadPlatformLayer(System.Object obj, System.EventArgs e)
        {
            apl?.UnloadLayer(); 
            apl = null;
        }

        /// <summary>
        /// Registers the platform layer to the current <see cref="SystemInfo"/> class. <br />
        /// Note that only ONE instance can be registered for the app lifetime!!!
        /// </summary>
        /// <param name="layer">The platform layer to be used.</param>
        /// <exception cref="System.ArgumentNullException"><paramref name="layer"/> was <see langword="null"/>.</exception>
        public static void RegisterPlatformLayer(AbstractPlatformLayer layer)
        {
            if (apl is not null) { return; }
            if (layer is null) { throw new System.ArgumentNullException(nameof(layer)); }
            DebugProvider.WriteLine($"Registering platform layer with hash code {layer.GetHashCode():x8}");
            apl = layer;
            DebugProvider.WriteLine($"Registering platform layer unload event!");
            System.AppDomain.CurrentDomain.DomainUnload += new(UnloadPlatformLayer);
        }

        /// <summary>
        /// Gets the platform layer that is used by the <see cref="SystemInfo"/> class. <br />
        /// From this property you may be able to retrieve more system information if you know the registered type.
        /// </summary>
        public static AbstractPlatformLayer LayerUsed => apl;

        /// <summary>Gets the user that opened the app.</summary>
        public static System.String UserName => apl.UserName;

        /// <summary>Gets the process directory where the app is opened from.</summary>
        public static System.String CurrentProcessDirectory => apl.CurrentProcessDirectory;

        /// <summary>Gets or sets the current directory that the files and common services are using to resolve paths.</summary>
        public static System.String CurrentDirectory
        {
            get => apl.CurrentDirectory;
            set => apl.CurrentDirectory = value;
        }

        /// <summary>
        /// Gets a value whether the app has been opened from a VM. <br />
        /// This property may or may not be implemented. If not, it always returns <see langword="false"/>.
        /// </summary>
        public static System.Boolean IsVirtualMachine => apl.IsVirtualMachine;

        /// <summary>Gets the current date and time at the moment this property was called. For a UTC timestamp, use <see cref="UtcNow"/>.</summary>
        public static System.DateTime Now => apl.Now;

        /// <summary>Gets the current date and time at the moment this property was called. For a local timestamp, use <see cref="Now"/>.</summary>
        public static System.DateTime UtcNow => apl.UtcNow;

        /// <summary>
        /// OS-specific property. <br />
        /// Returns the base directory where all the OS executable code is residing. <br />
        /// This property may or may not be implemented. If not, it always returns <see langword="null"/>.
        /// </summary>
        public static System.String OSDirectory => apl.OSDirectory;

        /// <summary>Gets the directory where the system's directory is considered to be.</summary>
        public static System.String SystemDirectory => apl.SystemDirectory;

        /// <summary>
        /// OS-specific property. <br />
        /// Gets the computer name where this app is running from. <br />
        /// This value varies from OS to OS and from user to user. <br />
        /// This property may or may not be implemented. If not, it always returns <see langword="null"/>.
        /// </summary>
        public static System.String ComputerName => apl.ComputerName;

        /// <summary>
        /// Gets a value of the type of the firmware used to launch the current OS. <br />
        /// This property may or may not be implemented. 
        /// If not, it presumes that the app runs on a newer environment and thus returns the 'UEFI' string.
        /// </summary>
        public static System.String SystemFirmwareType => apl.SystemFirmwareType;

        /// <summary>Gets the current OS instance version.</summary>
        public static System.Version OperatingSystemVersion => apl.OperatingSystemVersion;

        /// <summary>
        /// Gets the app's memory allocation page. <br />
        /// All the mem allocations are happening based on a multiple of this value.
        /// </summary>
        public static System.UInt32 PageSize => apl.PageSize;

        /// <summary>
        /// Gets the current CPU instruction set that is used by all the apps of this OS instance.
        /// </summary>
        public static ProcessorArchitecture ProcessorArchitecture => apl.ProcessorArchitecture;

        /// <summary>Gets the Windows hardware profile ID. This value is only defined on the Windows platform.</summary>
        [SupportedOSPlatformGuard("windows")]
        public static System.Guid HardwareProfileID => apl.HardwareProfileID;

        /// <summary>Gets the Windows hardware profile name. This value is only defined on the Windows platform.</summary>
        [SupportedOSPlatformGuard("windows")]
        public static System.String HardwareProfile => apl.HardwareProfile;

        /// <summary>
        /// Gets the number of active processor groups.
        /// </summary>
        [SupportedOSPlatform("windows")]
        public static System.UInt16 ActiveProcessorGroupCount => apl.ActiveProcessorGroupCount;

        /// <summary>
        /// Gets the number of maximum processor groups.
        /// </summary>
        [SupportedOSPlatform("windows")]
        public static System.UInt16 MaximumProcessorGroupCount => apl.MaximumProcessorGroupCount;

        /// <summary>
        /// Gets a value whether the app is running with administrative priviledges. <br />
        /// On Windows, this equals with the NT security authority, while on Unix it is an app opened with the 'sudo' command.
        /// </summary>
        public static System.Boolean HasAdminPriviledges => apl.HasAdminPriviledges;

        /// <summary>
        /// Gets an environment variable from the current app instance and returns it's value.
        /// </summary>
        /// <param name="name">The name of the environment variable to retrieve.</param>
        /// <returns>The value of the environment variable specified in <paramref name="name"/>.</returns>
        public static System.String GetEnvironmentVariable(System.String name) => apl.GetEnvironmentVariable(name);

        /// <summary>
        /// Sets an environment variable to the current app instance. <br />
        /// If the variable in question does not exist, it must be created on the fly.
        /// </summary>
        /// <param name="name">The name of the environment variable to be set.</param>
        /// <param name="value">The value of the environment variable to be set.</param>
        public static void SetEnvironmentVariable(System.String name, System.String value) => apl.SetEnvironmentVariable(name, value);

        /// <summary>
        /// Given an formatted string that contains environment variables, <br />
        /// this method expands all the valid variables found in <paramref name="format"/>.
        /// </summary>
        /// <param name="format">The formatted string that contains the variables to be expanded. May also not contain any variables.</param>
        /// <returns>A string with all the defined variables in <paramref name="format"/> expanded to their values.</returns>
        public static System.String ExpandEnvironmentVariables(System.String format) => apl.ExpandEnvironmentVariables(format);

        /// <summary>
        /// Gets all the currently defined environment variables defined in this app instance. 
        /// </summary>
        /// <returns>An array of <see cref="EnvironmentVariable"/> structures.</returns>
        public static EnvironmentVariable[] GetEnvironmentVariables() => apl.GetEnvironmentVariables();

        /// <summary>
        /// Gets a common user or known folder path from a number of pre-defined values.
        /// </summary>
        /// <param name="folder">The user of known folder path to look up.</param>
        /// <returns>The requested known folder.</returns>
        public static System.String GetKnownFolder(ShellKnownFolder folder) => apl.GetKnownFolder(folder);

        /// <summary>
        /// Gets the number of processors defined in a group.
        /// </summary>
        /// <param name="group">The group of processors to get their count.</param>
        /// <returns>The number of active processors.</returns>
        [SupportedOSPlatform("windows")]
        public static System.Int32 GetProcessorCount(System.UInt16 group) => apl.GetProcessorCount(group);

        /// <summary>
        /// Gets the current computer name as it is defined by different installed components.
        /// </summary>
        /// <param name="type">The computer name type to look up.</param>
        /// <returns>The computer name requested by <paramref name="type"/>.</returns>
        public static System.String GetComputerName(ComputerNameTypes type) => apl.GetComputerName(type);
    }
}