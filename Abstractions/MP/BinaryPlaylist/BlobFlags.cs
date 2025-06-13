
using System;

namespace MP.BinaryPlaylist
{
    /// <summary>
    /// Specifies additional guidance for the reader when this blob will be read. <br />
    /// The flags contained here are standard for all blobs; blob implementations may also  <br />
    /// specify custom reading flags here , as long as they do not overlap any of these values.
    /// </summary>
    [Flags]
    public enum BlobFlags : System.Byte
    {
        /// <summary>
        /// The blob is written with no additional special flags.
        /// </summary>
        Normal = 0,
        /// <summary>
        /// The blob can be read only , written once and should not be modified in the future.
        /// </summary>
        ReadOnly = 1,
        /// <summary>
        /// The blob is a special blob , which means that is a blob that is reserved, or that it requires special handling.
        /// </summary>
        Special = 2,
        /// <summary>
        /// The blob is a custom blob , and can be only read by the creator of this blob. <br />
        /// Should be specified when the blob's type is not one of the known ones (see the <see cref="BlobTypes"/> enumeration). 
        /// </summary>
        Custom = 4,
        /// <summary>
        /// The blob may contain location values for tracking down data offsets inside it's data. <br />
        /// This case is depended on the blob and it's own header. <br />
        /// How the blob will interpret these values is implementation-specific for each blob.
        /// </summary>
        RelocationBlob = 8
    }
}