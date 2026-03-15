

using System;

namespace MP.AudioLibrary.Windows.Wave
{
    /// <summary>
    /// Extensibility attachment for the <see cref="WaveFileAudioCodec"/> class. <br />
    /// It provides the codec basis for .wav files.
    /// </summary>
    public abstract class WaveCodecExtension
    {
        /// <summary>
        /// The data stream passed to the <see cref="WaveFileAudioCodec"/> class.
        /// </summary>
        protected readonly MP.IO.DataStream data_stream;
        private long data_chunk_position, data_chunk_start, data_chunk_length;

        /// <summary>
        /// Initializes a new instance of the <see cref="WaveCodecExtension"/> class.
        /// </summary>
        /// <param name="stream">The data stream passed through the <see cref="WaveFileAudioCodec"/> class.</param>
        public WaveCodecExtension(MP.IO.DataStream stream)
        {
            ArgumentNullException.ThrowIfNull(stream);
            data_stream = stream;
        }

        /// <summary>
        /// Processes the given chunk. <br />
        /// Indirectly called by the <see cref="ConsumeChunk(ChunkInfo)"/> method.
        /// </summary>
        /// <param name="ch">The chunk identifier and size.</param>
        protected abstract void ProcessChunk(ChunkInfo ch);

        /// <summary>
        /// Fills information as read by the 'data' chunk. <br />
        /// Provided as this way due to extensions of the format which may need to override the length.
        /// </summary>
        /// <param name="ch">The <see cref="CHUNK"/> associated with the data.</param>
        /// <param name="data_chunk_length">The exact and actual length of the chunk.</param>
        protected void FillDataChunkInfo(ChunkInfo ch, long data_chunk_length = 0L)
        {
            data_chunk_start = data_chunk_position = data_stream.Seek(0L, MP.IO.SeekDisplacement.Current);
            if (data_chunk_length > System.Int64.MaxValue) {
                throw new OverflowException("Cannot process this very long data chunk with size: " + data_chunk_length);
            } else {
                this.data_chunk_length = data_chunk_length == 0L ? ch.Length : data_chunk_length;
            }
        }

        /// <summary>
        /// Decodes the audio format contained in a fmt chunk into a <see cref="WindowsAudioFormat"/> class.
        /// </summary>
        /// <param name="ch">The <see cref="ChunkInfo"/> that identifies size and id information for the format.</param>
        /// <returns>The decoded <see cref="WindowsAudioFormat"/>.</returns>
        /// <exception cref="InvalidOperationException">The passed chunk information is not a fmt chunk.</exception>
        protected unsafe WindowsAudioFormat DecodeAudioFormatDefault(ChunkInfo ch)
        {
            if (ch.ID != "fmt ") {
                throw new InvalidOperationException("NOT a 'fmt ' chunk: " + ch.ID);
            } else if (ch.Length == sizeof(WAVEFORMATEXTENSIBLE)) {
                return new(data_stream.ReadStructure<WAVEFORMATEXTENSIBLE>());
            } else {
                return new(data_stream.ReadStructure<WAVEFORMATEX>());
            }
        }

        /// <summary>
        /// Consumes the current chunk. <br />
        /// The current chunk data are passed to the <see cref="ProcessChunk(ChunkInfo)"/> and then the method can read data from the stream for it if needed. <br />
        /// If the chunk does not read any data or omit some, the method will properly update the seek pointer.
        /// </summary>
        /// <param name="ch">The <see cref="ChunkInfo"/> to process.</param>
        public void ConsumeChunk(ChunkInfo ch)
        {
            long pos_before = data_stream.Seek(0, MP.IO.SeekDisplacement.Current), processed;

            ProcessChunk(ch);

            processed = data_stream.Seek(0, IO.SeekDisplacement.Current) - pos_before;

            if (processed > ch.Length) {
                throw new OverflowException("ProcessChunk call left out of the chunk bound! Chunk: " + ch.ID);
            } else {
                data_stream.Seek(ch.Length - processed, MP.IO.SeekDisplacement.Current);
            }
        }

        /// <summary>
        /// Gets/sets the data chunk position.
        /// </summary>
        public System.Int64 DataPosition
        {
            get => data_chunk_position;
            set {
                if (value > data_chunk_start + data_chunk_length) {
                    throw new ArgumentOutOfRangeException(nameof(value), "Value does overpass the data chunk bounds.");
                } else if (value < data_chunk_start) {
                    throw new ArgumentOutOfRangeException(nameof(value),  "Value does overpass the data chunk bounds.");
                } else {
                    data_chunk_position = data_stream.Position = value;
                }
            }
        }

        /// <summary>
        /// Gets the absolute position where the data chunk begins.
        /// </summary>
        public System.Int64 DataStart => data_chunk_start;

        /// <summary>
        /// Gets the data chunk length.
        /// </summary>
        public System.Int64 DataLength => data_chunk_length;

        /// <summary>
        /// Gets a value whether this extension has completely bissected the format and can be used for decoding the data stream.
        /// </summary>
        public abstract bool HasCompletelyDecoded { get; }

        /// <summary>
        /// Gets the audio format that the current codec decodes into.
        /// </summary>
        public abstract AudioFormat Format { get; }

        /// <summary>Reads data from this codec extension.</summary>
        /// <param name="buffer">The temporary buffer to copy data to.</param>
        /// <returns>How many bytes were actually read and put into <paramref name="buffer"/>.</returns>
        public virtual int Read(Span<System.Byte> buffer)
        {
            int rb;
            long bound = data_chunk_start + data_chunk_length;
            long new_pos = data_chunk_position + buffer.Length;
            if (new_pos > bound) {
                rb = data_stream.Read(buffer.Slice(0, (new_pos - bound).ToInt32()));
            } else {
                rb = data_stream.Read(buffer);
            }

            data_chunk_position += rb;

            return rb;
        }

        /// <summary>
        /// Destroys the resources that need to be disposed. <br />
        /// Called by the <see cref="WaveFileAudioCodec.Dispose(bool)"/> method.
        /// </summary>
        public virtual void DestroyResources()
        {
            data_chunk_length = data_chunk_position = data_chunk_start = 0L;
        }
    }
}