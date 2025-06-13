
using System.Runtime.Versioning;

namespace MP
{
    /// <summary>
    /// Defines the abstract platform layer that is used by the <see cref="SystemInfo"/> class
    /// so that the native platform connections can be established. <br />
    /// The implementer of this class must at least implement a handful of methods and properties,
    /// however other ones are fully optional.
    /// </summary>
    [Annotations.NativeLayer]
    public abstract class AbstractPlatformLayer
    {
        /// <summary>
        /// Called just before the app's domain unloads. <br />
        /// Useful for destroying state that the layer itself uses to work.
        /// </summary>
        protected internal virtual void UnloadLayer() { }

        /// <summary>Gets the user that opened the app.</summary>
        public abstract System.String UserName { get; }

        /// <summary>Gets the process directory where the app is opened from.</summary>
        public abstract System.String CurrentProcessDirectory { get; }

        /// <summary>Gets or sets the current directory that the files and common services are using to resolve paths.</summary>
        public abstract System.String CurrentDirectory { get; set; }

        /// <summary>
        /// Gets a value whether the app has been opened from a VM. <br />
        /// This can be overriden , but when not it always returns <see langword="false"/>.
        /// </summary>
        public virtual System.Boolean IsVirtualMachine { get => false; }

        /// <summary>Gets the current date and time at the moment this property was called. For a UTC timestamp, use <see cref="UtcNow"/>.</summary>
        public abstract System.DateTime Now { get; }

        /// <summary>Gets the current date and time at the moment this property was called. For a local timestamp, use <see cref="Now"/>.</summary>
        public abstract System.DateTime UtcNow { get; }

        /// <summary>
        /// OS-specific property. <br />
        /// Returns the base directory where all the OS executable code is residing. <br />
        /// This property may or may not be implemented. If not, it always returns <see langword="null"/>.
        /// </summary>
        public virtual System.String OSDirectory => null;

        /// <summary>Gets the directory where the system's directory is considered to be.</summary>
        public abstract System.String SystemDirectory { get; }

        /// <summary>
        /// OS-specific property. <br />
        /// Gets the computer name where this app is running from. <br />
        /// This value varies from OS to OS and from user to user. <br />
        /// This property may or may not be implemented. If not, it always returns <see langword="null"/>.
        /// </summary>
        public virtual System.String ComputerName => null;

        /// <summary>
        /// Gets a value of the type of the firmware used to launch the current OS. <br />
        /// This property may or may not be implemented. 
        /// If not, it presumes that the app runs on a newer environment and thus returns the 'UEFI' string.
        /// </summary>
        public virtual System.String SystemFirmwareType => "UEFI";

        /// <summary>Gets the current OS instance version.</summary>
        public abstract System.Version OperatingSystemVersion { get; }

        /// <summary>
        /// Gets the app's memory allocation page. <br />
        /// All the mem allocations are happening based on a multiple of this value.
        /// </summary>
        public abstract System.UInt32 PageSize { get; }

        /// <summary>
        /// Gets the current CPU instruction set that is used by all the apps of this OS instance.
        /// </summary>
        public abstract ProcessorArchitecture ProcessorArchitecture { get; }

        /// <summary>Gets the Windows hardware profile ID. This value is only defined on the Windows platform.</summary>
        [SupportedOSPlatformGuard("windows")]
        public virtual System.Guid HardwareProfileID => System.Guid.Empty;

        /// <summary>Gets the Windows hardware profile name. This value is only defined on the Windows platform.</summary>
        [SupportedOSPlatformGuard("windows")]
        public virtual System.String HardwareProfile => null;

        /// <summary>
        /// Gets the number of active processor groups. <br />
        /// This value seems to be supported only in Windows, 
        /// however please if you find a platform that implements this, report an issue for it.
        /// </summary>
        [SupportedOSPlatform("windows")]
        public virtual System.UInt16 ActiveProcessorGroupCount => 0;

        /// <summary>
        /// Gets the number of maximum processor groups. <br />
        /// This value seems to be supported only in Windows, 
        /// however please if you find a platform that implements this, report an issue for it.
        /// </summary>
        [SupportedOSPlatform("windows")]
        public virtual System.UInt16 MaximumProcessorGroupCount => 0;

        /// <summary>
        /// Gets a value whether the app is running with administrative priviledges. <br />
        /// On Windows, this equals with the NT security authority, while on Unix it is an app opened with the 'sudo' command.
        /// </summary>
        public abstract System.Boolean HasAdminPriviledges { get; }

        /// <summary>
        /// Gets an environment variable from the current app instance and returns it's value.
        /// </summary>
        /// <param name="name">The name of the environment variable to retrieve.</param>
        /// <returns>The value of the environment variable specified in <paramref name="name"/>.</returns>
        public abstract System.String GetEnvironmentVariable(System.String name);

        /// <summary>
        /// Sets an environment variable to the current app instance. <br />
        /// If the variable in question does not exist, it must be created on the fly.
        /// </summary>
        /// <param name="name">The name of the environment variable to be set.</param>
        /// <param name="value">The value of the environment variable to be set.</param>
        public abstract void SetEnvironmentVariable(System.String name, System.String value);

        /// <summary>
        /// Given an formatted string that contains environment variables, <br />
        /// this method expands all the valid variables found in <paramref name="format"/>.
        /// </summary>
        /// <param name="format">The formatted string that contains the variables to be expanded. May also not contain any variables.</param>
        /// <returns>A string with all the defined variables in <paramref name="format"/> expanded to their values.</returns>
        public abstract System.String ExpandEnvironmentVariables(System.String format);

        /// <summary>
        /// Gets all the currently defined environment variables defined in this app instance. 
        /// </summary>
        /// <returns>An array of <see cref="EnvironmentVariable"/> structures.</returns>
        public abstract EnvironmentVariable[] GetEnvironmentVariables();

        /// <summary>
        /// Gets a common user or known folder path from a number of pre-defined values.
        /// </summary>
        /// <param name="folder">The user of known folder path to look up.</param>
        /// <returns>The requested known folder.</returns>
        public abstract System.String GetKnownFolder(ShellKnownFolder folder);

        /// <summary>
        /// Gets the number of processors defined in a group.
        /// </summary>
        /// <param name="group">The group of processors to get their count.</param>
        /// <returns>The number of active processors.</returns>
        [SupportedOSPlatform("windows")]
        public virtual System.Int32 GetProcessorCount(System.UInt16 group) => 0;

        /// <summary>
        /// Gets the current computer name as it is defined by different installed components.
        /// </summary>
        /// <param name="type">The computer name type to look up.</param>
        /// <returns>The computer name requested by <paramref name="type"/>.</returns>
        public abstract System.String GetComputerName(ComputerNameTypes type);
    }
}