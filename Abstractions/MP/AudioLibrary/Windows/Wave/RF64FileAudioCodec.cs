
using MP.IO;
using System;
using MP.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace MP.AudioLibrary.Windows.Wave
{
    /// <summary>
    /// RF64 file format based on WAV <br />
    /// See for more information at <see href="https://tech.ebu.ch/docs/tech/tech3306v1_0.pdf"/> .
    /// </summary>
    public class RF64FileAudioCodec : BaseWaveAudioCodec
    {
        /// <summary>
        /// Constructs a new instance of the <see cref="RF64FileAudioCodec"/> class 
        /// from the specified data stream that reads the format, and the specified <see cref="WaveCodecExtension"/>s to use that can decode the format's data.
        /// </summary>
        /// <param name="stream">The data stream to initialize the reader from.</param>
        /// <param name="extensions">An enumerable of functions that can initialize a <see cref="WaveCodecExtension"/>.</param>
        /// <exception cref="ArgumentNullException"><paramref name="stream"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentException"><paramref name="stream"/> is not both readable and seekable.</exception>
        public RF64FileAudioCodec(DataStream stream, IEnumerable<Func<DataStream, WaveCodecExtension>> extensions) : base(stream, extensions) { }

        private static long VerifySizeAndReturn(ulong size)
        {
            if (size > System.Int64.MaxValue) {
                throw new OverflowException("The WAV file cannot be handled by the codec - it is too large.");
            } else {
                return size.ToInt64();
            }
        }

        /// <inheritdoc />
        protected unsafe override void ProcessChunks([DisallowNull] DataStream data_stream, long file_size, [DisallowNull] IList<WaveCodecExtension> extensions)
        {
            BTreeBasedDictionary<String, Int64> large_chunk_values = new(2, StringComparer.Ordinal);

            // First loop to find the ds64 chunk.
            long c_size = 4L + sizeof(System.UInt32);
            long p = 0L, ch_size = data_stream.Seek(0L, SeekDisplacement.Current);
            CHUNK ch = new();
            while (p + c_size < file_size)
            {
                ch.Load(data_stream);
                MarkChunkProlog();
                if (ch.ID == "ds64")
                {
                    RF64_DS64 large_file_sizes = new();
                    large_file_sizes.Load(data_stream);

                    file_size = VerifySizeAndReturn(large_file_sizes.RIFFSize);

                    large_chunk_values.Add("fact", VerifySizeAndReturn(large_file_sizes.FactSize));
                    large_chunk_values.Add("data", VerifySizeAndReturn(large_file_sizes.DataSize));

                    RF64_CHUNK64 c = new();

                    for (uint I = 0U; I < large_file_sizes.AdditionalChunksLength; I++)
                    {
                        c.Load(data_stream);
                        large_chunk_values.Add(c.ID, VerifySizeAndReturn(c.Length));
                    }

                    break;
                }
                p += MarkChunkEpilog(new(ch)) + c_size;
            }

            if (file_size == 0xFFFFFFFF) {
                throw new ArgumentException("Could not resolve file size for the RF64 file!");
            } else {
                data_stream.Position = ch_size;
                NextNextEnumerator<WaveCodecExtension> nn = new(extensions.GetEnumerator());
                try {
                    p = 0L;
                    while (p + c_size < file_size)
                    {
                        ch.Load(data_stream);
                        MarkChunkProlog();
                        if (ch.Size == 0xFFFFFFFF) {
                            if (!large_chunk_values.TryGetValue(ch.ID, out ch_size)) {
                                throw new ArgumentException("Cannot find 64-bit value for chunk " + ch.ID + "!!!");
                            }
                        } else {
                            ch_size = ch.Size;
                        }
                        ChunkInfo ci = new(ch.ID, ch_size);
                        if (!IsKnownChunkAndProcessed(data_stream, ci))
                        {
                            while (nn.MoveNext())
                            {
                                nn.Current.ConsumeChunk(ci);
                                if (nn.HasNextNextElement) { UnreadChunk(); }
                            }
                            nn.Reset();
                        }
                        p += c_size + MarkChunkEpilog(ci);
                    }
                } finally {
                    nn.Dispose();
                }
            }
        }

        /// <inheritdoc />
        protected override long VerifyFormat([DisallowNull] DataStream stream)
        {
            RIFF_HEADER header = new();
            header.Load(stream);
            if (header.ID != "RF64") {
                throw new ArgumentException("Stream is not a RF64 data stream.", nameof(stream));
            } else if (header.Type != "WAVE") {
                throw new ArgumentException("Stream is a RF64 data stream, but not a wave file.", nameof(stream));
            } else {
                return header.FileSize;
            }
        }
    }
}