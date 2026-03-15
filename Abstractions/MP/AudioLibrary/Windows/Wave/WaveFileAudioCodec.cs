
using System;
using MP.Utilities;
using MP.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace MP.AudioLibrary.Windows.Wave
{
    /// <summary>
    /// Represents the base class for waveform (.wav) files readers. <br />
    /// Specific formats are adopted by deriving this class. <br />
    /// Some information can be found here: <see href="https://www.aelius.com/njh/wavemetatools/doc/riffmci.pdf"/>
    /// </summary>
    public abstract class WaveFileAudioCodec : AudioStream
    {
        private long p_position;
        private IO.DataStream stream;
        private WaveCodecExtension selected;
        private SingleLinkedList<WaveCodecExtension> creators;

        /// <summary>
        /// Constructs a new instance of the <see cref="WaveFileAudioCodec"/> class 
        /// from the specified data stream that reads the format, and the specified <see cref="WaveCodecExtension"/>s to use that can decode the format's data.
        /// </summary>
        /// <param name="stream">The data stream to initialize the reader from.</param>
        /// <param name="extensions">An enumerable of functions that can initialize a <see cref="WaveCodecExtension"/>.</param>
        /// <exception cref="ArgumentNullException"><paramref name="stream"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentException"><paramref name="stream"/> is not both readable and seekable.</exception>
        public WaveFileAudioCodec(IO.DataStream stream, IEnumerable<Func<IO.DataStream, WaveCodecExtension>> extensions)
        {
            ArgumentNullException.ThrowIfNull(stream);
            ArgumentNullException.ThrowIfNull(extensions);
            if (!stream.CanSeek) {
                throw new ArgumentException("Stream was unseekable.", nameof(stream));
            } else if (!stream.CanRead) {
                throw new ArgumentException("Stream was unreadable.", nameof(stream));
            } else {
                Initialize();
                long s = VerifyFormat(this.stream = stream);
                selected = null;
                creators = new();
                foreach (var extension in extensions) { creators.Add(extension(this.stream)); }
                ProcessChunks(s);
            }
        }

        /// <summary>
        /// Initializes the current <see cref="WaveFileAudioCodec"/>. <br />
        /// Useful for not overwriting fields after the base constructor has executed.
        /// </summary>
        protected virtual void Initialize() { }

        /// <summary>Verifies that the file format is correct.</summary>
        /// <param name="stream">The data stream to verify data from.</param>
        /// <returns>The actual file size.</returns>
        protected abstract long VerifyFormat([DisallowNull] IO.DataStream stream);

        /// <summary>
        /// Processes all the chunks of the current data stream. <br />
        /// This should dispatch to the <see cref="WaveCodecExtension"/>s the <see cref="WaveCodecExtension.ConsumeChunk(ChunkInfo)"/> method.
        /// </summary>
        /// <param name="extensions">Registered extensions to use.</param>
        /// <param name="file_size">The file size as identified from the header.</param>
        /// <param name="data_stream">The data stream as passed to the <see cref="WaveFileAudioCodec"/> constructor.</param> 
        protected abstract void ProcessChunks([DisallowNull] IO.DataStream data_stream, long file_size, [DisallowNull] IList<WaveCodecExtension> extensions);

        private unsafe void ProcessChunks(long fs)
        {
            long initial = stream.Seek(0, IO.SeekDisplacement.Current);
            
            ProcessChunks(stream, fs, creators);

            foreach (WaveCodecExtension ext in creators)
            {
                if (ext.HasCompletelyDecoded) { selected = ext; break; }
            }

            creators = null;

            if (selected is null) {
                throw new InvalidOperationException("Cannot completely decode the current audio stream. The format the audio stream indicates is not supported.");
            } else {
                stream.Position = initial;
            }
        }

        /// <summary>
        /// Marks the beginning of a new chunk, after the chunk itself is read.
        /// </summary>
        protected void MarkChunkProlog() => p_position = stream.Seek(0, IO.SeekDisplacement.Current);

        /// <summary>Moves the seeking pointer past to the end of the current chunk.</summary>
        /// <param name="chunk">The chunk information to use for seeking past to the end of the chunk.</param>
        /// <returns>The number of bytes that were actually skipped. This might be more than the chunk's size due to alignment.</returns>
        protected long MarkChunkEpilog(ChunkInfo chunk)
        {
            long actual = chunk.Length + UnsafeMethods.PadAlignment(2L, chunk.Length);
            stream.Position = p_position + actual;
            return actual;
        }

        /// <summary>
        /// Completely unreads the current chunk that was processed with the <see cref="MarkChunkProlog"/> method.
        /// </summary>
        protected void UnreadChunk() => stream.Position = p_position;

        /// <inheritdoc />
        public override AudioFormat Format => selected.Format;

        /// <inheritdoc />
        public override TimeSpan CurrentTime 
        { 
            get => TimeSpan.FromSeconds((selected.DataPosition - selected.DataStart) / (double)selected.Format.AverageBytesPerSecond); 
            set {
                selected.DataPosition = selected.DataStart + MathHelpers.Floor(selected.Format.AverageBytesPerSecond * value.TotalSeconds);
                FireCurrentTimeInvalidatedEvent();
            }
        }

        /// <inheritdoc />
        public override TimeSpan TotalTime => TimeSpan.FromSeconds(selected.DataLength / selected.Format.AverageBytesPerSecond);

        /// <inheritdoc />
        public override int Read(Span<byte> buffer) => selected.Read(buffer);

        /// <inheritdoc />
        protected override void Dispose(bool disposing)
        {
            try {
                selected?.DestroyResources();
            } finally {
                stream?.Dispose();
                selected = null;
                stream = null;
            }
        }
    }
}