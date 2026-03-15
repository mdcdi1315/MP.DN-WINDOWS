
using MP.IO;
using System;
using MP.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace MP.AudioLibrary.Windows.Wave
{
    /// <summary>
    /// Provides the <see cref="BaseWaveAudioCodec"/> for RIFF (.wav) files.
    /// </summary>
    public class RIFFFileAudioCodec : BaseWaveAudioCodec
    {
        /// <summary>
        /// Constructs a new instance of the <see cref="RIFFFileAudioCodec"/> class 
        /// from the specified data stream that reads the format, and the specified <see cref="WaveCodecExtension"/>s to use that can decode the format's data.
        /// </summary>
        /// <param name="stream">The data stream to initialize the reader from.</param>
        /// <param name="extensions">An enumerable of functions that can initialize a <see cref="WaveCodecExtension"/>.</param>
        /// <exception cref="ArgumentNullException"><paramref name="stream"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentException"><paramref name="stream"/> is not both readable and seekable.</exception>
        public RIFFFileAudioCodec(DataStream stream, IEnumerable<Func<DataStream, WaveCodecExtension>> extensions) : base(stream, extensions) { }

        /// <inheritdoc />
        protected unsafe override void ProcessChunks(DataStream stream, long file_size, IList<WaveCodecExtension> extensions)
        {
            NextNextEnumerator<WaveCodecExtension> nn = new(extensions.GetEnumerator());
            try {
                CHUNK cht = new();
                long p = 0L, c_size = 4L + sizeof(System.UInt32);
                while (p + c_size < file_size)
                {
                    cht.Load(stream);
                    ChunkInfo ch = new(cht);
                    MarkChunkProlog();
                    if (!IsKnownChunkAndProcessed(stream, ch))
                    {
                        while (nn.MoveNext())
                        {
                            nn.Current.ConsumeChunk(ch);
                            if (nn.HasNextNextElement) { UnreadChunk(); }
                        }
                        nn.Reset();
                    }
                    p += c_size + MarkChunkEpilog(ch);
                }
            } finally {
                nn.Dispose();
            }
        }

        /// <inheritdoc />
        protected override long VerifyFormat([DisallowNull] DataStream stream)
        {
            RIFF_HEADER header = stream.ReadStructure<RIFF_HEADER>();
            if (header.ID != "RIFF") {
                throw new ArgumentException("Stream is not a RIFF data stream.", "stream");
            } else if (header.Type != "WAVE") {
                throw new ArgumentException("Stream is a RIFF data stream, but not a wave file.", "stream");
            } else {
                return header.FileSize;
            }
        }
    }
}