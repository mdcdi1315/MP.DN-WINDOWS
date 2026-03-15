

using MP.IO.DataStructuring;
using MP.IO.DataStructuring.Generation;

namespace MP.BinaryPlaylist
{
    [DataStructureGenerator]
    internal partial struct BPL_BLOB_HEADER : IDataStructure
    {
        /// <summary>The indentifier of the blob for the current playlist stream.</summary>
        [FixedString(4, StringFieldEncoding.ASCII)]
        public System.String ID;

        /// <summary>
        /// This field specifies flags for this blob.
        /// </summary>
        public BlobFlags Flags;

        /// <summary>
        /// The version of this blob. <br />
        /// This can be an arbitrary version set by the playlist's editing tools.
        /// </summary>
        public System.UInt16 Version;

        /// <summary>
        /// The number of child structures contained in this blob. <br />
        /// Implementation note: It does not mean that a blob will contain only structures ,  <br />
        /// it can also contain data of arbitrary length. <br /> The field can have the value zero , 
        /// and it is used to assist the programmer in random memory access cases. <br />
        /// Use the Length field to learn the exact size of this blob. <br />
        /// </summary>
        public System.UInt32 Count;

        /// <summary>Reserved.</summary>
        public System.UInt32 RSVD_0;

        /// <summary>
        /// The blob length in bytes.  <br />
        /// A number of 0 suggests an empty blob. <br />
        /// Adding this value to the stream position should get you to the next blob.
        /// </summary>
        public System.Int64 Length;
    }
}