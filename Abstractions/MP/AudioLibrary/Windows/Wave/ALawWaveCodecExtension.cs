
using MP.IO;
using System;
using MP.AudioLibrary.Codecs;

namespace MP.AudioLibrary.Windows.Wave
{
    /// <summary>
    /// Wave file codec extension for A-Law data.
    /// </summary>
    public sealed class ALawWaveCodecExtension : WaveCodecExtension
    {
        private bool is_ok;
        private AudioFormat format;

        /// <summary>Initializes a new instance of the <see cref="ALawWaveCodecExtension"/> class.</summary>
        /// <param name="stream">The data stream passed through the <see cref="WaveFileAudioCodec"/> class.</param>
        public ALawWaveCodecExtension(DataStream stream) : base(stream) { }

        /// <inheritdoc />
        public override bool HasCompletelyDecoded => is_ok;

        /// <inheritdoc />
        public override AudioFormat Format => format;

        /// <inheritdoc />
        protected unsafe override void ProcessChunk(ChunkInfo ch)
        {
            if (ch.ID == "fmt ") {
                if (ch.Length == sizeof(WAVEFORMATEXTENSIBLE)) {
                    WAVEFORMATEXTENSIBLE ext = data_stream.ReadStructure<WAVEFORMATEXTENSIBLE>();
                    if (ext.SubFormatGUID.ToString() == "00000006-0000-0010-8000-00aa00389b71") {
                        format = AudioFormat.CreatePCM(ext.BaseFormat.SampleRate.ToInt32(), 16, ext.BaseFormat.Channels);
                    } else {
                        format = null;
                    }
                } else {
                    WAVEFORMATEX ex = data_stream.ReadStructure<WAVEFORMATEX>();
                    if (ex.Tag == WAVEFORMATTAG.ALaw) {
                        format = AudioFormat.CreatePCM(ex.SampleRate.ToInt32(), 16, ex.Channels);
                    } else {
                        format = null;
                    }
                }
            } else if (format is not null && ch.ID == "data") {
                FillDataChunkInfo(ch);
                is_ok = true;
            }
        }

        /// <inheritdoc />
        public override int Read(Span<byte> buffer)
        {
            // We are decoding into 16-bit values.
            int actually_reading = buffer.Length / sizeof(short);
            System.Byte[] temp = new System.Byte[actually_reading];
            int read = base.Read(temp);
            if (read > 0) {
                int I = 0, G = 0;
                while (I < read)
                {
                    buffer.WriteStructure(G, ALawDecoder.ALawToLinearSample(temp[I++]));
                    G += sizeof(short);
                }
                return read * sizeof(short);
            } else {
                return 0;
            }
        }
    }
}