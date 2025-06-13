
using MP.AudioLibrary;

namespace MP.ExtSystemApi
{
    public sealed class CodecProbeRequestResult
    {
        /// <summary>
        /// For variable bit-rate codecs only. <br />
        /// When this is not null , a resampler resampling to this format is automatically inserted
        /// </summary>
        public AudioFormat StableCodecFormat;
        /// <summary>
        /// Gets the actual audio stream. <br />
        /// This does represent the actual codec.
        /// </summary>
        public AudioStream AudioStream;
    }
}