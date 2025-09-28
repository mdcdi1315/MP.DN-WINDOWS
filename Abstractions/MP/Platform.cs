


namespace MP
{
    /// <summary>
    /// Defines common platform names. <br />
    /// This is to be used by the <see cref="SystemInfo"/> class so that users of it can identify the OS 
    /// </summary>
    public enum Platform : System.Byte
    {
        /// <summary>
        /// Defines the Microsoft Windows OS. 
        /// </summary>
        Windows = 0,
        /// <summary>
        /// Defines an OS based on Unix.
        /// </summary>
        Unix = 1,
        /// <summary>
        /// Defines the Apple Mac OS X OS.
        /// </summary>
        OSX = 2,
        /// <summary>
        /// Defines an OS based on BSD (FreeBSD is an example).
        /// </summary>
        BSD = 3,
    }
}