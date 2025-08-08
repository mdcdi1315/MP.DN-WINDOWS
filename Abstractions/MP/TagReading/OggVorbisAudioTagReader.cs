

using System;
using MP.TagReading.Ogg;
using System.Runtime.InteropServices;

namespace MP.TagReading
{
    /// <summary>
    /// Reads an Ogg Vorbis audio tag , which is only consisting of an Ogg Vorbis comment.
    /// </summary>
    public sealed class OggVorbisAudioTagReader : IOggTagReaderBase
    {
        [Flags]
        private enum HeaderTypeFlags : byte
        {
            None = 0,
            ContinuedPacket = 0x01,
            FirstLogicalBitstreamPage = 0x02,
            LastLogicalBitstreamPage = 0x04
        }

        [StructLayout(LayoutKind.Explicit , Pack = 1)]
        private struct PACKET
        {
            // Decoder's capture_pattern. Must be 'OggS'
            [FieldOffset(0)]
            public byte CP0;
            [FieldOffset(1)]
            public byte CP1;
            [FieldOffset(2)]
            public byte CP2;
            [FieldOffset(3)]
            public byte CP3;

            [FieldOffset(4)]
            public byte StreamStructureVersion; // Set currently to 0.

            [FieldOffset(5)]
            public HeaderTypeFlags HeaderTypeFlag; // Bitflags for the header

            [FieldOffset(6)]
            public long AbsoluteGranulePosition;

            [FieldOffset(14)]
            public int StreamSerialNumber;

            [FieldOffset(18)]
            public int PageSequenceNumber;

            [FieldOffset(22)]
            public uint PageCRCNumber;

            [FieldOffset(26)]
            public byte NumberOfSegments; // The number of segments contained in the stream

            // Segment table , each one byte represents a segment length , it's length is into a single byte.
            // Combining all the segment values gives you the stream's length in bytes

            public bool IsCorrectHeader => CP0 == 0x4f && CP1 == 0x67 && CP2 == 0x67 && CP3 == 0x53;
        }

        private System.IO.Stream stream;
        private OggVorbisComment comment;
        private long packetlength , packetstartpos;

        /// <summary>
        /// Creates a new Ogg Vorbis audio tag reader by reading the specified Ogg Vorbis stream (NOT RTP STREAM!!!)
        /// </summary>
        /// <param name="stream">The stream to read data from.</param>
        /// <exception cref="ArgumentNullException"><paramref name="stream"/> was <see langword="null"/>.</exception>
        /// <exception cref="ArgumentException"><paramref name="stream"/> was not both readable and seekable.</exception>
        /// <exception cref="System.IO.EndOfStreamException"><paramref name="stream"/> ended prematurely.</exception>
        public OggVorbisAudioTagReader(System.IO.Stream stream)
        {
            if (stream is null) { throw new ArgumentNullException(nameof(stream)); }
            if (stream.CanRead == false) { throw new ArgumentException("Stream must be a readable stream.", nameof(stream)); }
            if (stream.CanSeek == false) { throw new ArgumentException("Stream must be a seekable stream.", nameof(stream)); }
            this.stream = stream;
            FindCommentPacket();
            // Ignore and bail the next byte
            long actpos = packetstartpos + 7; // Ignore the header bytes as well.
            stream.ReadByte();
            // Next 6 bytes must match the string 'vorbis'.
            if (stream.ReadASCIIString(6) != "vorbis") { throw new System.IO.EndOfStreamException("Stream ended prematurely. Vorbis header not found."); }
            comment = new(stream, actpos, packetlength);
            if (comment.Count == 0) { throw new System.IO.EndOfStreamException("The comment stream does not contain useful data to get from."); }
        }

        private void FindCommentPacket()
        {
            while (stream.Position < stream.Length)
            {
                PACKET packet = stream.ReadStructure<PACKET>();
                if (packet.IsCorrectHeader == false) { throw new System.IO.EndOfStreamException("Invalid packet header. Stream ended prematurely."); }
                byte[] lengths = stream.ReadBytes(packet.NumberOfSegments);
                long datalength = 0;
                for (int I = 0; I < lengths.Length; I++) { datalength += lengths[I]; }
                lengths = null;
                if (packet.HeaderTypeFlag.HasFlag(HeaderTypeFlags.FirstLogicalBitstreamPage) ||
                     packet.HeaderTypeFlag.HasFlag(HeaderTypeFlags.ContinuedPacket)) {
                    stream.Seek(datalength, System.IO.SeekOrigin.Current);
                    continue;
                }
                if (packet.HeaderTypeFlag.HasFlag(HeaderTypeFlags.LastLogicalBitstreamPage)) {
                    throw new System.IO.EndOfStreamException("We are invalidly on the last logical bit stream. Stream ended prematurely.");
                }
                packetstartpos = stream.Position;
                packetlength = datalength;
                break;
            }
        }

        /// <summary>
        /// Clears all the internal references held by the current instance.
        /// </summary>
        public void Dispose()
        {
            comment = null;
            stream = null;
            packetlength = 0;
            packetstartpos = 0;
        }

        /// <summary>
        /// [OGG Reader Specific] Gets the OGG Vorbis library vendor , if any.
        /// </summary>
        public string OGGVendorLibrary => comment.Vendor;

        /// <summary>
        /// Forwards the <see cref="OGGVendorLibrary"/> property.
        /// </summary>
        public string Vendor => comment.Vendor;

        /// <summary>
        /// This property is not supported and will always throw <see cref="NotSupportedException"/>.
        /// </summary>
        public string ImageFormat => throw new NotSupportedException("Cover image format is not supported in Ogg Vorbis files.");

        /// <inheritdoc />
        public string Publisher => comment.GetValue("ORGANIZATION");

        /// <inheritdoc />
        public string Copyright => comment.GetValue("COPYRIGHT");

        /// <inheritdoc />
        public string CreationDate => comment.GetValue("DATE");

        /// <inheritdoc />
        public string WebSiteEncoderUrl => comment.GetValue("CONTACT");

        /// <inheritdoc />
        public string EncodedBy
        {
            get
            {
                string final = comment.GetValue("ENCODER");
                if (string.IsNullOrEmpty(final))
                {
                    final = comment.GetValue("EncodedBy");
                }
                return final;
            }
        }

        /// <inheritdoc />
        public string AlbumName => comment.GetValue("ALBUM");

        /// <summary>
        /// This property is not supported and will always throw <see cref="NotSupportedException"/>.
        /// </summary>
        public string Title1 => throw new NotSupportedException("Ogg Comments do not honor or support the Title1 property.");

        /// <inheritdoc />
        public string Title2 => comment.GetValue("TITLE");

        /// <inheritdoc />
        public string DiscOrdinal
        {
            get
            {
                // The disc ordinal can be found either simply as DISC or even DISCNUMBER.
                string final = comment.GetValue("DISC");
                if (string.IsNullOrEmpty(final))
                {
                    final = comment.GetValue("DISCNUMBER");
                }
                return final;
            }
        }

        /// <inheritdoc />
        public string TrackNumber => comment.GetValue("TRACKNUMBER");

        /// <inheritdoc />
        public string ContributingArtists => comment.ConcatenateMultipleValues("ARTIST");

        /// <inheritdoc />
        public string AlbumArtist => comment.GetValue("ALBUMARTIST");

        /// <inheritdoc />
        public string Comments => comment.GetValue("Comments");

        /// <summary>
        /// This property is not supported and will always throw <see cref="NotSupportedException"/>.
        /// </summary>
        public string SubTitle => throw new NotSupportedException("Ogg Comments do not honor or support the SubTitle property.");

        /// <inheritdoc />
        public string PublisherURL => comment.GetValue("URL");

        /// <inheritdoc />
        public string Genre => comment.GetValue("GENRE");

        /// <inheritdoc />
        public byte[] Image
        {
            get
            {
                string val = comment.GetValue("Image");
                if (string.IsNullOrEmpty(val)) { return null; }
                return Convert.FromBase64String(val);
            }
        }
    }
}