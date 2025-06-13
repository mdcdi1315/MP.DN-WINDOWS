
using System;

namespace MP.AudioLibrary
{
    /// <summary>
    /// Indicates the audio format used by an audio stream. <br />
    /// This information can be used by the decoders to find out how to decode the audio information contained in a stream.
    /// </summary>
    public class AudioFormat
    {
        private System.Int32 sr, br, blkalign;
        private CommonAudioFormat caf;
        private ChannelType[] layout;

        /// <summary>
        /// Creates an empty instance of the <see cref="AudioFormat"/> class.
        /// </summary>
        public AudioFormat() {
            sr = 0; 
            br = 0; 
            blkalign = 0;
            caf = CommonAudioFormat.Unknown;
            layout = null;
        }

        /// <summary>
        /// Creates a PCM audio format from the specified sample rate , bit rate and number of channels.
        /// </summary>
        /// <param name="samplerate">The sample rate of the audio format.</param>
        /// <param name="bitrate">The bit rate of the audio format.</param>
        /// <param name="nchannels">The number of channels.</param>
        /// <returns>The created PCM format.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Any or all of the parameters were less than 1.</exception>
        public static AudioFormat CreatePCM(System.Int32 samplerate , System.Int32 bitrate , System.Int32 nchannels)
        {
            if (samplerate < 1) {
                throw new ArgumentOutOfRangeException(nameof(samplerate), "Sample rate must be a positive numeric value.");
            }
            if (bitrate < 1) {
                throw new ArgumentOutOfRangeException(nameof(bitrate), "Bit rate must be a positive numeric value.");
            }
            if (nchannels < 1) {
                throw new ArgumentOutOfRangeException(nameof(nchannels), "The number of channels must be at least one.");
            }
            AudioFormat af = new();
            af.sr = samplerate;
            af.br = bitrate;
            af.layout = CommonChannelTypes.CreateLayoutFromNumberOfChannels(nchannels);
            af.caf = CommonAudioFormat.PCM;
            af.blkalign = nchannels * (bitrate / 8);
            return af;
        }

        /// <summary>
        /// Creates a IEEE floating point audio format from the specified sample rate and number of channels.
        /// </summary>
        /// <param name="samplerate">The sample rate of the audio format.</param>
        /// <param name="nchannels">The number of channels.</param>
        /// <returns>The created IEEE floating point format.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Any or all of the parameters were less than 1.</exception>
        public static AudioFormat CreateIEEEFloat(System.Int32 samplerate , System.Int32 nchannels)
        {
            if (samplerate < 1) {
                throw new ArgumentOutOfRangeException(nameof(samplerate), "Sample rate must be a positive numeric value.");
            }
            if (nchannels < 1) {
                throw new ArgumentOutOfRangeException(nameof(nchannels), "The number of channels must be at least one.");
            }
            AudioFormat af = new();
            af.sr = samplerate;
            af.br = 32; // Bit rate in IEEE float is always 32 (and it makes sense since a System.Single requires 32 bits of storage to be saved)
            af.layout = CommonChannelTypes.CreateLayoutFromNumberOfChannels(nchannels);
            af.caf = CommonAudioFormat.IEEEFloat;
            af.blkalign = 4 * nchannels;
            return af;
        }

        /// <summary>
        /// Creates a custom <see cref="AudioFormat"/> instance from the specified data.
        /// </summary>
        /// <param name="samplerate">The sample rate of the audio format.</param>
        /// <param name="bitrate">The bit rate of the audio format</param>
        /// <param name="channels">The channel layout of the audio format</param>
        /// <param name="blkalign">The block alignment of the audio format</param>
        /// <param name="commonfmt">The common audio format, if one exists</param>
        /// <returns>A custom <see cref="AudioFormat"/> class instance.</returns>
        public static AudioFormat CreateCustom(
            System.Int32 samplerate,
            System.Int32 bitrate,
            ChannelType[] channels,
            System.Int32 blkalign,
            CommonAudioFormat commonfmt = CommonAudioFormat.Unknown)
            => new() { 
                BitRate = bitrate,
                SampleRate = samplerate,
                ChannelLayout = channels,
                BlockAlignment = blkalign,
                CommonFormat = commonfmt
            };

        /// <summary>
        /// The sample rate of the stream, in samples per second in Hertz.
        /// </summary>
        public System.Int32 SampleRate 
        {
            get => sr;
            protected set
            {
                if (value < 1) {
                    throw new ArgumentOutOfRangeException(nameof(value) , "Sample rate must be a positive numeric value.");
                }
                sr = value;
            }
        }

        /// <summary>
        /// Gets the number of audio data bits per second. <br />
        /// Usually this is set to 32 or 16. You may also find cases where this is 8 or 24.
        /// </summary>
        public System.Int32 BitRate 
        {
            get => br;
            protected set
            {
                if (value < 1) {
                    throw new ArgumentOutOfRangeException(nameof(value), "Bit rate must be a positive numeric value.");
                }
                br = value;
            }
        }

        /// <summary>
        /// Gets the channel layout for the current audio format. <br />
        /// Getting the <see cref="System.Array.Length"/> property on the returned array should give you the number of channels.
        /// </summary>
        public ChannelType[] ChannelLayout 
        {
            get => layout;
            protected set
            {
                if (value is null) {
                    throw new ArgumentNullException(nameof(value));
                }
                layout = value;
            }
        }

        /// <summary>
        /// Gets the block alignment for the audio data. <br />
        /// May be different from format to format, so this must be calculated differently.
        /// </summary>
        public System.Int32 BlockAlignment 
        { 
            get => blkalign;
            protected set
            {
                if (value < 1) {
                    throw new ArgumentOutOfRangeException(nameof(value), "Audio data block alignment must be a positive numeric value.");
                }
                blkalign = value;
            }
        }

        /// <summary>
        /// Gets the common format of this audio format, if there is one.
        /// </summary>
        public CommonAudioFormat CommonFormat
        {
            get => caf;
            protected set => caf = value;
        }

        /// <summary>
        /// Gets the number of bytes that do comprise a second of the audio data. <br />
        /// Can be overriden if deemed necessary but this seems to be always <see cref="SampleRate"/> * <see cref="BlockAlignment"/>.
        /// </summary>
        // In fact it accesses the SampleRate and BlockAlignment fields 
        public virtual System.Int32 AverageBytesPerSecond => sr * blkalign;
    }
}