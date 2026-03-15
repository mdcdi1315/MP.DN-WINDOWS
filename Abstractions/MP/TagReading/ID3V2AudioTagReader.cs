
using MP.IO;
using System;
using MP.Collections;
using MP.TagReading.ID3;
using MP.Annotations.CodeAnalysis;
using System.Diagnostics.CodeAnalysis;

namespace MP.TagReading
{
    /// <summary>
    /// Gets raw data from an ID3V2 stream. <br />
    /// Pass it a valid stream that is the tag data. <br />
    /// Then , use the properties of this class so as to get the representing track data. <br />
    /// Now , it is anymore fully compliant respective to the format and supports both 2.3. and 2.4. versions.
    /// </summary>
    public sealed class ID3V2AudioTagReader : IID3V2TagReaderBase
    {
        private ArrayBasedList<CompiledID3V2Frame> frames;

        /// <summary>
        /// Reads an ID3V2 tag from the specified stream.
        /// </summary>
        /// <param name="stream">The data stream to read the tag from.</param>
        /// <exception cref="ArgumentNullException"><paramref name="stream"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentException"><paramref name="stream"/> was not both readable and seekable.</exception>
        public ID3V2AudioTagReader(DataStream stream)
        {
            if (stream is null) { throw new ArgumentNullException(nameof(stream)); }
            if (stream.CanSeek == false) { throw new ArgumentException("Stream must be seekable.", nameof(stream)); }
            if (stream.CanRead == false) { throw new ArgumentException("Stream must be readable.", nameof(stream)); }
            frames = new(10);
            ReadFrames(stream);
        }

        private unsafe void ReadFrames(DataStream stream)
        {
            ID3V2HEADER header = new();
            header.Load(stream);
            if (header.IsValid == false) { throw new InvalidOperationException("This stream does not contain ID3V2 data."); }
            if (header.Flags.HasFlag(ID3V2Flags.ExtendedHeader))
            {
                ID3V2EXTENDEDHEADER exthdr = new();
                exthdr.Load(stream);
                stream.Seek(header.VersionMajor < 4 ? exthdr.Length.ToInt32Old() : exthdr.Length.ToInt32(), SeekDisplacement.Current);
            }
            // ID3V2 format clearly instructs that between minor versions no data should be accepted.
            // However , the reader supports both V2.3 and 2.4 so there is not a problem to do this below:
            if (header.VersionMajor < 3) { throw new AggregateException("Reader cannot read older ID3V2 tags."); }
            long lenfinal = stream.Position + header.Length.ToInt32();
            CompiledID3V2Frame frame;
            while (stream.Position + sizeof(ID3V2FRAMEHEADER) < lenfinal) // Ensure that we can read a next frame 
            {
                frame = new CompiledID3V2Frame(stream, header);
                if (frame.Value is not null) { frames.Add(frame); }
            }
        }

        /// <summary>
        /// Gets the number of frames read from the stream.
        /// </summary>
        public int FrameCount => frames.Count;

        /// <inheritdoc />
        public string WebSiteEncoderUrl => StringValueFrameById("WOAS");

        /// <inheritdoc />
        public string EncodedBy => StringValueFrameById("TENC");

        /// <inheritdoc />
        public string AlbumName => StringValueFrameById("TALB");

        /// <inheritdoc />
        public string Title => StringValueFrameById("TIT2");

        /// <inheritdoc />
        public string DiscOrdinal => StringValueFrameById("TPOS");

        /// <inheritdoc />
        public string TrackNumber => StringValueFrameById("TRCK");

        /// <inheritdoc />
        public string ContributingArtists => StringValueFrameById("TPE1");

        /// <inheritdoc />
        public string AlbumArtist => StringValueFrameById("TPE2");

        /// <inheritdoc />
        public string Comments
        {
            get {
                if (TryGetComment(out var comment)) {
                    return comment.Text;
                } else {
                    return System.String.Empty;
                }
            }
        }

        /// <inheritdoc />
        public string SubTitle => StringValueFrameById("TIT3");

        /// <inheritdoc />
        public string PublisherURL => StringValueFrameById("WOAR");

        /// <inheritdoc />
        public string Genre => StringValueFrameById("TGEN");

        /// <inheritdoc />
        public string ImageFormat
        {
            get {
                CompiledID3V2Frame frame = FrameById("APIC");
                if (frame is null) {
                    return null;
                } else {
                    return ((AttachedPictureFrameData)frame.Value).MimeType;
                }
            }
        }

        /// <inheritdoc />
        public string Publisher => StringValueFrameById("TPUB");

        /// <inheritdoc />
        public string Copyright => StringValueFrameById("TCOP");

        /// <inheritdoc />
        public string CreationDate => StringValueFrameById("TDEN");

        /// <inheritdoc />
        public DataStream Image
        {
            get {
                CompiledID3V2Frame frame = FrameById("APIC");
                if (frame is null) {
                    return null;
                } else {
                    return ((AttachedPictureFrameData)frame.Value).PictureData;
                }
            }
        }

        /// <summary>
        /// Gets the frame at the specified index.
        /// </summary>
        /// <param name="index">The index of the frame to retrieve.</param>
        /// <returns>The compiled ID3V2 frame.</returns>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="index"/> was out of the range [0..<see cref="FrameCount"/>].</exception>
        [Throws(typeof(ArgumentOutOfRangeException))]
        public CompiledID3V2Frame GetFrame(int index) => frames[index];

        /// <summary>
        /// Disposes this <see cref="ID3V2AudioTagReader"/> class instance.
        /// </summary>
        public void Dispose()
        {
            if (frames is not null)
            {
                foreach (CompiledID3V2Frame cf in frames)
                {
                    if (cf.Value is IDisposable d) { d.Dispose(); }
                }
                frames.Clear();
                frames = null;
            }
        }

        private CompiledID3V2Frame FrameById(System.String name)
        {
            foreach (CompiledID3V2Frame f in frames)
            {
                if (f.Header.FrameID == name) {
                    return f;
                }
            }
            return null;
        }

        private String StringValueFrameById(System.String name)
        {
            var frame = FrameById(name);
            if (frame is null) { 
                return System.String.Empty; 
            } else if (frame.Value is System.String s) {
                return s;
            } else if (frame.Value is System.String[] sa) {
                return System.String.Join(", ", sa);
            } else {
                return System.String.Empty;
            }
        }

        /// <inheritdoc />
        public bool TryGetLyrics(out UnsynchronisedLyricsFrameData lyrics)
        {
            var frame = FrameById("USLT");
            if (frame is null) {
                lyrics = default;
                return false;
            } else {
                lyrics = (UnsynchronisedLyricsFrameData)frame.Value;
                return true;
            }
        }

        /// <inheritdoc />
        public bool TryGetComment(out CommentFrameData comment)
        {
            var frame = FrameById("COMM");
            if (frame is null) {
                comment = default;
                return false;
            } else {
                comment = (CommentFrameData)frame.Value;
                return true;
            }
        }

        /// <inheritdoc />
        [return: MaybeNull]
        public object GetProperty(string name) => FrameById(name)?.Value;
    }
}