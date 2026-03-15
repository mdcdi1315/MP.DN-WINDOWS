
using System;

namespace MP.BinaryPlaylist
{
    /// <summary>
    /// Specifies additional guidance for the reader when this blob will be read. <br />
    /// The flags contained here are standard for all blobs; blob implementations may also  <br />
    /// specify custom reading flags here , as long as they do not overlap any of these values.
    /// </summary>
    [Flags]
    public enum BlobFlags : System.UInt16
    {
        /// <summary>
        /// The blob is written with no additional special flags.
        /// </summary>
        Normal = 0,
        /// <summary>
        /// The blob can be read only , written once and should not be modified in the future.
        /// </summary>
        ReadOnly = 1 << 0,
        /// <summary>
        /// The blob is a special blob , which means that is a blob that is reserved, or that it requires special handling.
        /// </summary>
        Special = 1 << 1,
        /// <summary>
        /// The blob is a custom blob , and can be only read by the creator of this blob.
        /// </summary>
        Custom = 1 << 2,
        /// <summary>
        /// The blob is incomplete, that is, it may contain corrupt data. <br />
        /// Typically flagged during writing to indicate that the blob is not yet finalized. <br />
        /// Readers encountering this flag should abruptly stop reading as even the length indicated might not be correct.
        /// </summary>
        Incomplete = 1 << 3,
    }
}