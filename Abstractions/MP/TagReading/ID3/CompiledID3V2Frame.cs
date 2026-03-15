
using System;

namespace MP.TagReading.ID3
{
    /// <summary>
    /// Provides the representation of a 'compiled' (aka processed) ID3 V2 frame.
    /// </summary>
    public sealed class CompiledID3V2Frame
    {
        /// <summary>
        /// The raw frame header.
        /// </summary>
        public ID3V2FRAMEHEADER Header;
        /// <summary>
        /// Will store a byte array on unknown frames, if known it will contain that specific object. <br />
        /// Note: this might be <see langword="null"/>, which it means that the frame must be discarded by the reader.
        /// </summary>
        public System.Object Value;
        /// <summary>
        /// Gets the type of this frame.
        /// </summary>
        public ID3V2FrameType Type;

        /// <summary>
        /// Reads an ID3V2 frame. This operation requires the ID3V2 header to have been provided.
        /// </summary>
        /// <param name="stream">The stream to read the frame from</param>
        /// <param name="hdr">The ID3V2 header to denote how the data should be read.</param>
        /// <exception cref="ArgumentNullException"><paramref name="stream"/> was <see langword="null"/>.</exception>
        public CompiledID3V2Frame(IO.DataStream stream, ID3V2HEADER hdr)
        {
            ArgumentNullException.ThrowIfNull(stream);
            Header = new();
            Header.Load(stream);
            if (Header.IsInvalid) {
                Value = null; // Discard the frame, invalid frame was read.
                return; 
            }
            System.Int32 hdrlen;
            if (hdr.VersionMajor < 4) {
                hdrlen = Header.Length.ToInt32Old();
            } else {
                hdrlen = Header.Length.ToInt32();
            }
            // If explicitly set by an writer , this frame must be skipped.
            // Value will be explicitly set to null to indicate to the implementing reader to discard the frame.
            if (Header.Status == ID3V2FrameStatusFlags.DiscardFrame)
            {
                stream.Seek(hdrlen, IO.SeekDisplacement.Current);
                Value = null;
                return;
            }
            if (Header.Format.HasFlag(ID3V2FrameFormatFlags.DataLengthPresent))
            {
                // Allocates 4 additional bytes at the start of the frame data
                stream.Seek(4 , IO.SeekDisplacement.Current);
                hdrlen -= 4;
            }
            if (Header.Format.HasFlag(ID3V2FrameFormatFlags.ContainsGroupInformation))
            {
                // Allocates 1 additional byte at the start of the frame
                stream.Seek(1, IO.SeekDisplacement.Current);
                hdrlen--;
            }
            IO.MemoryStream msr = new(hdrlen);
            try {
                stream.CopySpecificToStream(msr, hdrlen);
                msr.Position = 0;
                ProcessData(msr);
            } finally {
                msr.Dispose();
            }
        }

        private void ProcessData(IO.MemoryStream memory_stream)
        {
            Type = Header.FrameID switch {
                "TIT1" => ID3V2FrameType.Text,
                "TIT2" => ID3V2FrameType.Text,
                "TIT3" => ID3V2FrameType.Text,
                "TALB" => ID3V2FrameType.Text,
                "TOAL" => ID3V2FrameType.Text,
                "TRCK" => ID3V2FrameType.Text,
                "TPOS" => ID3V2FrameType.Text,
                "TSST" => ID3V2FrameType.Text,
                "TSRC" => ID3V2FrameType.Text,
                "TPE1" => ID3V2FrameType.Text,
                "TPE2" => ID3V2FrameType.Text,
                "TPE3" => ID3V2FrameType.Text,
                "TPE4" => ID3V2FrameType.Text,
                "TOPE" => ID3V2FrameType.Text,
                "TEXT" => ID3V2FrameType.Text,
                "TOLY" => ID3V2FrameType.Text,
                "TCOM" => ID3V2FrameType.Text,
                "TMCL" => ID3V2FrameType.Text,
                "TIPL" => ID3V2FrameType.Text,
                "TENC" => ID3V2FrameType.Text,
                "TCOP" => ID3V2FrameType.Text,
                "TPRO" => ID3V2FrameType.Text,
                "TPUB" => ID3V2FrameType.Text,
                "TOWN" => ID3V2FrameType.Text,
                "TRSN" => ID3V2FrameType.Text,
                "TRSO" => ID3V2FrameType.Text,
                "TCON" => ID3V2FrameType.Text,
                "TYER" => ID3V2FrameType.Text,
                "TDAT" => ID3V2FrameType.Text,
                "TBPM" => ID3V2FrameType.Text,
                "TLEN" => ID3V2FrameType.Text,
                "TXXX" => ID3V2FrameType.UserDefinedTextFrame,
                "WCOM" => ID3V2FrameType.URL,
                "WCOP" => ID3V2FrameType.URL,
                "WOAF" => ID3V2FrameType.URL,
                "WOAR" => ID3V2FrameType.URL,
                "WOAS" => ID3V2FrameType.URL,
                "WORS" => ID3V2FrameType.URL,
                "WPAY" => ID3V2FrameType.URL,
                "WPUB" => ID3V2FrameType.URL,
                "WXXX" => ID3V2FrameType.UserDefinedURLLinkFrame,
                "USLT" => ID3V2FrameType.UnsyncronizedLyrics,
                "SYLT" => ID3V2FrameType.SyncronizedLyrics,
                "APIC" => ID3V2FrameType.Picture,
                "COMM" => ID3V2FrameType.Comments,
                "OWNE" => ID3V2FrameType.Ownership,
                "USER" => ID3V2FrameType.TermsOfUse,
                _ => ID3V2FrameType.None,
            };

            switch (Type)
            {
                default:
                case ID3V2FrameType.None:
                    Value = memory_stream.ToArray();
                    break;
                case ID3V2FrameType.Text:
                    Value = ID3V2Utils.ReadTextFrame(memory_stream);
                    break;
                case ID3V2FrameType.UserDefinedTextFrame:
                    Value = ID3V2Utils.ReadUserDefinedTextFrame(memory_stream);
                    break;
                case ID3V2FrameType.URL:
                    Value = ID3V2Utils.ReadString_ISO_8859_3(memory_stream, false, out _, memory_stream.Length);
                    break;
                case ID3V2FrameType.UserDefinedURLLinkFrame:
                    Value = ID3V2Utils.ReadUserDefinedURLLinkFrame(memory_stream);
                    break;
                case ID3V2FrameType.UnsyncronizedLyrics:
                    Value = ID3V2Utils.ReadUnsynchronisedLyricsFrame(memory_stream);
                    break;
                case ID3V2FrameType.Ownership:
                    Value = ID3V2Utils.ReadOwnershipFrame(memory_stream);
                    break;
                case ID3V2FrameType.TermsOfUse:
                    Value = ID3V2Utils.ReadTermsOfUseFrame(memory_stream);
                    break;
                case ID3V2FrameType.Picture:
                    Value = ID3V2Utils.ReadAttachedPicture(memory_stream);
                    break;
                case ID3V2FrameType.Comments:
                    Value = ID3V2Utils.ReadCommentFrame(memory_stream);
                    break;
            }
        }
    }
}