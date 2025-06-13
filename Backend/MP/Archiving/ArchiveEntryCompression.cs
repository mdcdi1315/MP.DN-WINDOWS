

namespace MP.Archiving
{
    /// <summary>
    /// Determines the compression type of this entry.
    /// </summary>
    public enum ArchiveEntryCompression : System.Byte
    {
        /// <summary>
        /// Just store the entry. Does not perform any compression algorithm.
        /// </summary>
        Store = 0,
        /// <summary>
        /// Compress using the GZip algorithm.
        /// </summary>
        GZip = 1,
        /// <summary>
        /// Compress using the BZip2 algorithm.
        /// </summary>
        BZip2 = 2
    }
}