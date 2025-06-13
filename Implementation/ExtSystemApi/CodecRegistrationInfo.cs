

using MP.AudioLibrary;

namespace MP.ExtSystemApi
{
    public sealed class CodecRegistrationInfo
    {
        /// <summary>
        /// For variable bit-rate codecs only. <br />
        /// When this is not null , a resampler resampling to this format is automatically inserted
        /// </summary>
        public AudioFormat StableCodecFormat;
        /// <summary>
        /// Gets the audio file's stream to use.
        /// </summary>
        public AbstractPropertyStream FileStream;
    }
}