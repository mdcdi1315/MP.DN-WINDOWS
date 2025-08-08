
using System;
using MP.TagReading.ID3;
using System.Collections.Generic;

namespace MP.TagReading
{
    /// <summary>
    /// Gets raw data from an ID3V2 stream. <br />
    /// Pass it a valid stream that is the tag data. <br />
    /// Then , use the properties of this class so as to get the representing track data. <br />
    /// Now , it is anymore fully compliant respective to the format and supports both 2.3. and 2.4. versions.
    /// </summary>
    public sealed class ID3V2DataReader : IID3V2TagReaderBase
    {
        private sealed class ID3V2FrameData
        {
            public string Key;
            public string Value;
            public byte[] RawValue; // Only set for specific data or when the data are of unknown nature.
        }

        private List<ID3V2FrameData> frames;
        private string imgformat;

        /// <summary>
        /// Reads an ID3V2 tag from the specified stream.
        /// </summary>
        /// <param name="stream">The data stream to read the tag from.</param>
        /// <exception cref="ArgumentNullException"><paramref name="stream"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentException"><paramref name="stream"/> was not both readable and seekable.</exception>
        public ID3V2DataReader(System.IO.Stream stream)
        {
            if (stream is null) { throw new ArgumentNullException(nameof(stream)); }
            if (stream.CanSeek == false) { throw new ArgumentException("Stream must be seekable.", nameof(stream)); }
            if (stream.CanRead == false) { throw new ArgumentException("Stream must be readable.", nameof(stream)); }
            frames = new();
            ReadFrames(stream);
        }

        private unsafe void ReadFrames(System.IO.Stream strm)
        {
            ID3V2HEADER header = strm.ReadStructure<ID3V2HEADER>(); 
            if (header.IsValid == false) { throw new InvalidOperationException("This stream does not contain ID3V2 data."); }
            if (header.Flags.HasFlag(ID3V2Flags.ExtendedHeader))
            {
                var exthdr = strm.ReadStructure<ID3V2EXTENDEDHEADER>();
                strm.Seek(header.VersionMajor < 4 ? exthdr.Length.ToInt32Old() : exthdr.Length.ToInt32(), System.IO.SeekOrigin.Current);
            }
            // ID3V2 format clearly instructs that between minor versions no data should be accepted.
            // However , the reader supports both V2.3 and 2.4 so there is not a problem to do this below:
            if (header.VersionMajor < 3) { throw new AggregateException("Reader cannot read older ID3V2 tags."); }
            long lenfinal = strm.Position + header.Length.ToInt32();
            while (strm.Position + sizeof(ID3V2FRAMEHEADER) < lenfinal) // Ensure that we can read a next frame 
            {
                var frame = new ID3V2CompleteFrame(strm , header);
                if (frame.Header.IsInvalid) { break; } // We have fell into the padding area , so tag reading was factually finished
                ID3V2FrameData dt = new();
                switch (frame.Type)
                {
                    case ID3V2FrameType.Picture:
                        imgformat = frame.Key;
                        dt.RawValue = frame.RawValue;
                        dt.Key = "Picture";
                        break;
                    case ID3V2FrameType.Comments:
                        dt.Key = "Comment";
                        dt.Value = frame.TextData;
                        break;
                    case ID3V2FrameType.UnsyncronizedLyrics:
                        dt.Value = frame.TextData;
                        dt.Key = "LyricsU";
                        break;
                    case ID3V2FrameType.URL:
                    case ID3V2FrameType.Text:
                        dt.Key = frame.Header.FrameID;
                        dt.Value = frame.TextData;
                        break;
                    case ID3V2FrameType.UserDefinedTextFrame:
                    case ID3V2FrameType.UserDefinedURLLinkFrame:
                        if (string.IsNullOrEmpty(dt.Key) && frame.Type == ID3V2FrameType.UserDefinedURLLinkFrame) {
                            dt.Key = "%$$$WebSiteEncoderUrl_Internal";
                            dt.Value = frame.TextData.TrimStart('\0');
                        } else {
                            dt.Key = frame.Key;
                            dt.Value = frame.TextData;
                        }
                        break;
                    default:
                        dt.Key = frame.Header.FrameID;
                        dt.RawValue = frame.RawValue;
                        break;
                }
                frames.Add(dt);
            }
        }

        private string GetString(string key)
        {
            if (string.IsNullOrEmpty(key)) { return string.Empty; }
            foreach (var f in frames)
            {
                if (f.Key == key) { return f.Value.TrimEnd(65279.ToChar() , '\0'); }
            }
            return string.Empty;
        }

        private string TryForMultipleFrames(params string[] framekeys)
        {
            if (framekeys is null || framekeys.Length == 0) { return string.Empty; }
            System.String vt;
            System.Char[] trimcs = new System.Char[] { 65279.ToChar(), '\0' };
            foreach (var fk in framekeys)
            {
                foreach (var f in frames)
                {
                    if (f.Key == fk) { 
                        vt = f.Value?.TrimEnd(trimcs);

                        if (System.String.IsNullOrWhiteSpace(vt) == false) {
                            trimcs = null;
                            return vt; 
                        }
                    }
                }
            }
            trimcs = null;
            return string.Empty;
        }

        private byte[] GetData(string key)
        {
            if (string.IsNullOrEmpty(key)) { return null; }
            foreach (var f in frames)
            {
                if (f.Key == key) { return f.RawValue; }
            }
            return null;
        }

        /// <summary>
        /// Gets a frame that is not specified by any derivants of the <see cref="ITagReader"/> interface.
        /// </summary>
        /// <param name="key">The key of the frame you wish to be retrieved.</param>
        /// <returns>The frame's value as a string.</returns>
        public string GetStringFrame(string key) => GetString(key);

        /// <summary>
        /// Gets a frame that is not specified by any derivants of the <see cref="ITagReader"/> interface.
        /// </summary>
        /// <param name="key">The key of the frame you wish to be retrieved.</param>
        /// <returns>The frame's value as a raw array.</returns>
        public byte[] GetDataFrame(string key) => GetData(key);

        // NOTE: For multiple frames , always define first the formal frames and then any other frame names you find.
        /// <inheritdoc/>
        public string ImageFormat => imgformat;

        /// <inheritdoc/>
        public string Publisher => GetString("TPUB");

        /// <inheritdoc/>
        public string Copyright => GetString("TCOP");

        /// <inheritdoc/>
        public string CreationDate => TryForMultipleFrames("TDEN", "TDRC", "TDAT", "TYER");

        /// <inheritdoc/>
        public string WebSiteEncoderUrl => TryForMultipleFrames("WOAS", "%$$$WebSiteEncoderUrl_Internal");

        /// <inheritdoc/>
        public string EncodedBy => GetString("TENC");

        /// <inheritdoc/>
        public string AlbumName => GetString("TALB");

        /// <inheritdoc/>
        public string Title1 => GetString("TIT1");

        /// <inheritdoc/>
        public string Title2 => GetString("TIT2");

        /// <inheritdoc/>
        public string DiscOrdinal => GetString("TPOS");

        /// <inheritdoc/>
        public string TrackNumber => GetString("TRCK");

        /// <inheritdoc/>
        public string ContributingArtists => TryForMultipleFrames("TPE1", "ARTISTS");

        /// <inheritdoc/>
        public string AlbumArtist => GetString("TPE2");

        /// <inheritdoc/>
        public string Comments => GetString("Comment");

        /// <inheritdoc/>
        public string SubTitle => GetString("TIT3");

        /// <inheritdoc/>
        public string PublisherURL => GetString("WOAR");

        /// <inheritdoc/>
        public string Genre => TryForMultipleFrames("TGEN", "TCON");

        /// <inheritdoc/>
        public byte[] Image => GetData("Picture");
        
        /// <summary>
        /// [ID3 Reader Specific] Gets the unsyncronized lyrics for this track that the tag is associated with.
        /// </summary>
        public string Lyrics => GetString("LyricsU");

        /// <summary>
        /// Clears all the resources used by this class. <br />
		/// Be noted that you cannot access any data after disposal.
        /// </summary>
        public void Dispose()
        {
            imgformat = null;
            frames?.Clear();
            frames = null;
        }
    }
}