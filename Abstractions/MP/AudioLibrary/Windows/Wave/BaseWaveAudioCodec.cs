
using MP.IO;
using System;
using System.IO;
using MP.Collections;
using MP.TagReading;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace MP.AudioLibrary.Windows.Wave
{
    /// <summary>
    /// Provides additional and special data that a <see cref="WaveFileAudioCodec"/> cannot normally handle.
    /// </summary>
    public abstract class BaseWaveAudioCodec : WaveFileAudioCodec
    {
        private IID3V2TagReaderBase id3tag;
        private BTreeBasedDictionary<String, String> info_chunk;

        /// <summary>
        /// Constructs a new instance of the <see cref="BaseWaveAudioCodec"/> class 
        /// from the specified data stream that reads the format, and the specified <see cref="WaveCodecExtension"/>s to use that can decode the format's data.
        /// </summary>
        /// <param name="stream">The data stream to initialize the reader from.</param>
        /// <param name="extensions">An enumerable of functions that can initialize a <see cref="WaveCodecExtension"/>.</param>
        /// <exception cref="ArgumentNullException"><paramref name="stream"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentException"><paramref name="stream"/> is not both readable and seekable.</exception>
        protected BaseWaveAudioCodec(DataStream stream, IEnumerable<Func<DataStream, WaveCodecExtension>> extensions) : base(stream, extensions) { }

        /// <inheritdoc />
        protected override void Initialize()
        {
            id3tag = null;
            info_chunk = null;
        }

        /// <summary>
        /// Gets a value whether the passed chunk is recognized by the current <see cref="BaseWaveAudioCodec"/> class
        /// and it has been processed.
        /// </summary>
        /// <param name="stream">The data stream.</param>
        /// <param name="info">The chunk information.</param>
        /// <returns>A value whether the passed chunk was processed successfully.</returns>
        protected virtual bool IsKnownChunkAndProcessed(DataStream stream, ChunkInfo info)
        {
            if (info.ID == "id3 ") {
                try { id3tag = new ID3V2AudioTagReader(new OffsettedReadOnlyStream(stream, info.Length));  } catch { }
                return true;
            } else if (info.ID == "LIST") {
                string type = stream.ReadString(System.Text.Encoding.ASCII, 4L);
                if (type == "INFO") {
                    info_chunk = new(5);
                    // A collection containing further chunks with NULL-terminated strings.
                    // Assumming that it is a 32-bit CHUNK.
                    long align;
                    long I = 0L;
                    CHUNK ch = new();
                    long c_size = 4L + sizeof(System.UInt32);
                    while (I + c_size < info.Length)
                    {
                        ch.Load(stream);

                        info_chunk.Add(ch.ID, stream.ReadString(System.Text.Encoding.UTF8, ch.Size - 1));
                        stream.DiscardBytes(align = UnsafeMethods.PadAlignment(2L, ch.Size));
                        stream.ReadByte();

                        I += c_size + ch.Size + align;
                    }
                }
                return true;
            } else {
                return false;
            }
        }

        /// <summary>
        /// If the file has an ID3V2 tag, this property will return it. <br />
        /// Otherwise, it will be <see langword="null"/>.
        /// </summary>
        [MaybeNull]
        public IID3V2TagReaderBase Tag => id3tag;

        /// <summary>
        /// Provides an enumerable that provides the key-value pairs defined in the INFO section of the LIST chunk. <br />
        /// If the INFO section was not found, <see langword="null"/> is returned.
        /// </summary>
        [MaybeNull]
        public IEnumerable<KeyValuePair<String, String>> ListChunkInfo => info_chunk;

        /// <inheritdoc />
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            if (id3tag is not null) {
                id3tag.Dispose();
                id3tag = null;
            }
        }
    }
}