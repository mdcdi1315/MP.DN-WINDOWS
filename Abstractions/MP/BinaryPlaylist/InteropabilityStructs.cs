using System.Runtime.InteropServices;

namespace MP.BinaryPlaylist
{
    /// <summary>
    /// Specifies a Music Player .MPBPL binary playlist file header. <br />
    /// The first blob starts after this header.
    /// </summary>
    [StructLayout(LayoutKind.Explicit, Size = 8, Pack = 1)]
    public unsafe struct PLAYLISTHDR
    {
        /// <summary>
        /// First header identifier byte.
        /// </summary>
        [FieldOffset(0)]
        public System.Byte ID0;

        /// <summary>
        /// Second header identifier byte.
        /// </summary>
        [FieldOffset(1)]
        public System.Byte ID1;

        /// <summary>
        /// Third header identifier byte.
        /// </summary>
        [FieldOffset(2)]
        public System.Byte ID2;

        /// <summary>
        /// Specifies the generic format version of this .MPBPL file. <br />
        /// Note that this is an internal value and is only relevant with the whole file only; <br />
        /// It does not suggest any blob versions etc etc...
        /// </summary>
        [FieldOffset(3)]
        public System.Int16 Version;

        /// <summary>
        /// Contains the number of blobs contained in the current playlist.
        /// </summary>
        [FieldOffset(5)]
        public System.UInt16 Blobs;

        /// <summary>
        /// Contains the mask of the blob which holds an enumeration value which as a number is the largest value. <br />
        /// For security protection only, and this field logic is not required to be implemented by the readers. <br />
        /// Note that the current MPBPL implementation does fully support this field.
        /// </summary>
        [FieldOffset(7)]
        public BlobTypes MaxType;

        /// <summary>
        /// Gets a value whether this structure instance validly represents a MPBPL Format Header.
        /// </summary>
        public readonly System.Boolean IsValidHeader => ID0 == 66 && ID1 == 80 && ID2 == 76;

        /// <summary>
        /// Default header constructor. It initializes only the header identifier bytes to the correct values.
        /// </summary>
        public PLAYLISTHDR()
        {
            ID0 = 66;
            ID1 = 80;
            ID2 = 76;
        }
    }

    /// <summary>
    /// A common header for all the shared blobs currently being used by the Music Player Playlist. <br />
    /// Blobs may contain their own header that you may need to read to learn specific information about it.
    /// </summary>
    [StructLayout(LayoutKind.Explicit , Size = 16 , Pack = 1)]
    public unsafe struct BLOBHEADER
    {
        /// <summary>
        /// The first byte of the playlist blob identifier. <br />
        /// This byte specifies flags for this blob.
        /// </summary>
        [FieldOffset(0)]
        public BlobFlags Identifier1;

        /// <summary>
        /// The second byte of the playlist blob identifier. <br />
        /// This byte specifies this blob type.
        /// </summary>
        [FieldOffset(1)]
        public BlobTypes Identifier2;

        /// <summary>
        /// The version of this blob. <br />
        /// This can be an arbitrary version set by the playlist's editing tools.
        /// </summary>
        [FieldOffset(2)]
        public System.UInt16 Version;

        /// <summary>
        /// The number of child structures contained in this blob. <br />
        /// Implementation note: It does not mean that a blob will contain only structures ,  <br />
        /// it can also contain data of arbitrary length. <br /> The field can have the value zero , 
        /// and it is used to assist the programmer in random memory access cases. <br />
        /// Use the Length field to learn the exact size of this blob. <br />
        /// </summary>
        [FieldOffset(4)]
        public System.UInt32 Count;

        /// <summary>
        /// The blob length in bytes.  <br />
        /// A number of 0 suggests an empty blob. <br />
        /// Adding this value to the stream position should get you to the next blob.
        /// </summary>
        [FieldOffset(8)]
        public System.Int64 Length;
    }
}
