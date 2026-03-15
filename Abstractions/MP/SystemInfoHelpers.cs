

namespace MP
{
    /// <summary>
    /// Defines an environment variable.
    /// </summary>
    public struct EnvironmentVariable
    {
        /// <summary>Gets or sets the name of the current environment variable.</summary>
        public System.String Name;
        /// <summary>Gets or sets the value of the current environment variable.</summary>
        public System.String Value;

        /// <summary>
        /// Constructs an empty <see cref="EnvironmentVariable"/> instance.
        /// </summary>
        public EnvironmentVariable() { }

        /// <summary>
        /// Constructs a new environment variable from the specified name and value.
        /// </summary>
        /// <param name="name">The name of the environment variable.</param>
        /// <param name="value">The value of the environment variable.</param>
        public EnvironmentVariable(System.String name, System.String value)
        {
            Name = name;
            Value = value;
        }

        /// <summary>
        /// Returns a string that visualizes the contents of the current environment variable.
        /// </summary>
        /// <returns>A string containing the environment variable name and value.</returns>
        public readonly override System.String ToString() => $"{Name}={Value}";
    }

    /// <summary>
    /// Defines common computer name types.
    /// </summary>
    public enum ComputerNameTypes
    {
        /// <summary>
        /// Gets the computer's NetBIOS name.
        /// </summary>
        NetBIOS,
        /// <summary>
        /// Gets the computer's DNS host name.
        /// </summary>
        DNSHostName,
        /// <summary>
        /// Gets the computer's DNS domain name.
        /// </summary>
        DNSDomain,
        /// <summary>
        /// Gets the computer's DNS full domain name.
        /// </summary>
        FullDNS,
        /// <summary>
        /// Gets the computer's physical NetBIOS name.
        /// </summary>
        PhysicalNetBIOS,
        /// <summary>
        /// Gets the computer's physical DNS host name.
        /// </summary>
        PhysicalDNSHostName,
        /// <summary>
        /// Gets the computer's physical DNS domain name.
        /// </summary>
        PhysicalDNSDomain,
        /// <summary>
        /// Gets the computer's DNS full domain name.
        /// </summary>
        PhysicalFullDNS,
    }

    /// <summary>
    /// Defines common known folders for the Windows Shell.
    /// </summary>
    public enum ShellKnownFolder : System.Byte
    {
        /// <summary>Gets the Desktop folder.</summary>
        Desktop,
        /// <summary>Gets the Windows SendTo folder.</summary>
        SendTo,
        /// <summary>Gets the Documents folder.</summary>
        Documents,
        /// <summary>Gets the system folder.</summary>
        System,
        /// <summary>Gets the Windows System32 folder.</summary>
        SystemX86,
        /// <summary>Gets the Windows OS 'Windows' folder.</summary>
        Windows,
        /// <summary>Gets the Music folder.</summary>
        Music,
        /// <summary>Gets the Videos folder.</summary>
        Videos,
        /// <summary>Gets the Ringtones folder.</summary>
        Ringtones,
        /// <summary>Gets the CD Burning folder.</summary>
        CDBurning,
        /// <summary>Gets the Downloads folder.</summary>
        Downloads,
        /// <summary>Gets the Screenshots folder.</summary>
        Screenshots,
        /// <summary>Gets the user's profile folder.</summary>
        Profile,
        /// <summary>Gets the Pictures folder.</summary>
        Pictures,
        /// <summary>A typed constant indicating that the constant before this one was the last valid one.</summary>
        ShellKnownFolderMax
    }

    /// <summary>
    /// Defines common CPU architectures where this app may be running into.
    /// </summary>
    public enum ProcessorArchitecture : System.Byte
    {
        /// <summary>Returned when a valid value cannot be determined.</summary>
        Unknown,
        /// <summary>Intel x64 platform</summary>
        AMD64,
        /// <summary>ARM 64-bit platform</summary>
        ARM64,
        /// <summary>ARM 32-bit platform</summary>
        ARM32,
        /// <summary>Intel x86 platform</summary>
        X86,
        /// <summary>Intel Itanium 64-bit platform</summary>
        IA64
    }
}