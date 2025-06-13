

namespace MP.Networking
{
    /// <summary>
    /// Defines the HTTP version to run the specified request under.
    /// </summary>
    public enum HttpVersion : System.Byte
    {
        /// <summary>
        /// Uses HTTP version 1.0.
        /// </summary>
        Version1 = 0,
        /// <summary>
        /// Uses HTTP version 1.1.
        /// </summary>
        Version1_1 = 1,
    }
}