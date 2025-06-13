
using System;
using System.Runtime.InteropServices;

namespace MP.TagReading.ID3
{
    [Flags]
    public enum ID3V2FrameStatusFlags : System.Byte
    {
        None = 0,
        DiscardTag = 2,
        DiscardFrame = 4,
        ReadOnly = 8
    }

    [Flags]
    public enum ID3V2FrameFormatFlags : System.Byte
    {
        None = 0,
        /// <summary>
        /// From <see href="https://id3.org/id3v2.4.0-structure"/>: <br />
        /// This flag indicates whether or not this frame belongs in a group
        /// with other frames. If set, a group identifier byte is added to the
        /// frame. Every frame with the same group identifier belongs to the
        /// same group.
        /// </summary>
        ContainsGroupInformation = 2,
        /// <summary>
        /// From <see href="https://id3.org/id3v2.4.0-structure"/>: <br />
        /// Frame is compressed using zlib [zlib] deflate method. <br />
        /// If set, this requires the 'Data Length Indicator' bit to be set as well.
        /// </summary>
        UsesZlibCompression = 16,
        /// <summary>
        /// From <see href="https://id3.org/id3v2.4.0-structure"/>: <br />
        /// This flag indicates whether or not the frame is encrypted. If set,
        /// one byte indicating with which method it was encrypted will be
        /// added to the frame. See description of the ENCR frame for more
        /// information about encryption method registration. Encryption
        /// should be done after compression. Whether or not setting this flag
        /// requires the presence of a 'Data Length Indicator' depends on the
        /// specific algorithm used.
        /// </summary>
        EncryptedFrame = 32,
        /// <summary>
        /// From <see href="https://id3.org/id3v2.4.0-structure"/>: <br />
        /// This flag indicates whether or not unsynchronisation was applied
        /// to this frame. See section 6 for details on unsynchronisation.
        /// If this flag is set all data from the end of this header to the
        /// end of this frame has been unsynchronised. Although desirable, the
        /// presence of a 'Data Length Indicator' is not made mandatory by
        /// unsynchronisation.
        /// </summary>
        Unsynchronised = 64,
        /// <summary>
        /// From <see href="https://id3.org/id3v2.4.0-structure"/>: <br />
        /// This flag indicates that a data length indicator has been added to
        /// the frame. The data length indicator is the value one would write
        /// as the 'Frame length' if all of the frame format flags were
        /// zeroed, represented as a 32 bit synchsafe integer.
        /// </summary>
        DataLengthPresent = 128
    }

    [StructLayout(LayoutKind.Explicit, Pack = 1)]
    public struct ID3V2FRAMEHEADER
    {
        [FieldOffset(0)]
        public System.Byte FID0;

        [FieldOffset(1)]
        public System.Byte FID1;

        [FieldOffset(2)]
        public System.Byte FID2;

        [FieldOffset(3)]
        public System.Byte FID3;

        [FieldOffset(4)]
        public SYNCHSAFEINT Length;

        [FieldOffset(8)]
        public ID3V2FrameStatusFlags Status;

        [FieldOffset(9)]
        public ID3V2FrameFormatFlags Format;

        // Frame ID as a string.
        public readonly System.String FrameID => $"{FID0.ToChar()}{FID1.ToChar()}{FID2.ToChar()}{FID3.ToChar()}";

        public readonly System.Boolean IsInvalid => FID0 == 0 && FID1 == 0 && FID2 == 0 && FID3 == 0;

        // May more exist , under how flags are defined.
    }

}