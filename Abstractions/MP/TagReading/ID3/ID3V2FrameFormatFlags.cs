
using System;

namespace MP.TagReading.ID3
{
    /// <summary>
    /// Defines flags about the frame format, such as whether or not compression is applied.
    /// </summary>
    [Flags]
    public enum ID3V2FrameFormatFlags : System.Byte
    {
        /// <summary>
        /// No additional flags are defined.
        /// </summary>
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

}