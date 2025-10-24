

namespace MP.Serialization.MPSBF
{
    /// <summary>
    /// A special type for encoding BOOL entry values in MPSBF records.
    /// </summary>
    public enum BOOL : System.Int32
    {
        /// <summary>
        /// The value is <see langword="false"/>.
        /// </summary>
        FALSE = 0,
        /// <summary>
        /// The value is <see langword="true"/>.
        /// </summary>
        TRUE = 1,
    }
}