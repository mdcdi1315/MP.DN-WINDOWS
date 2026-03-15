
using MP.IO;

namespace MP.AudioLibrary.Windows.Wave
{
    /// <summary>
    /// Provides the default <see cref="WaveCodecExtension"/> for PCM.
    /// </summary>
    public sealed class PCMWaveCodecExtension : WaveCodecExtension
    {
        private bool has_data_chunk;
        private WindowsAudioFormat format;

        /// <summary>Initializes a new instance of the <see cref="PCMWaveCodecExtension"/> class.</summary>
        /// <param name="stream">The data stream to read from.</param>
        public PCMWaveCodecExtension(DataStream stream) : base(stream)
        {
            format = null;
            has_data_chunk = false;
        }

        /// <inheritdoc />
        public override bool HasCompletelyDecoded => has_data_chunk;

        /// <inheritdoc />
        public override AudioFormat Format => format;

        /// <inheritdoc />
        protected override void ProcessChunk(ChunkInfo ch)
        {
            if (ch.ID == "fmt ") {
                format = DecodeAudioFormatDefault(ch);
                if (format.CommonFormat != CommonAudioFormat.PCM) { format = null; }
            } else if (format is not null && ch.ID == "data") {
                FillDataChunkInfo(ch);
                has_data_chunk = true;
            }
        }

        /// <inheritdoc />
        public override void DestroyResources()
        {
            format = null;
            has_data_chunk = false;
        }
    }
}