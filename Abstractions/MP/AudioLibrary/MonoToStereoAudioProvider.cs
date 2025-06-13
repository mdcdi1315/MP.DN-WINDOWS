

using System;

namespace MP.AudioLibrary
{
    /// <summary>
    /// Provides a simple wrapper for converting monaural audio providers to stereo providers by duplicating the monaural channel. <br />
    /// Supports also variable-format audio providers
    /// </summary>
    public sealed class MonoToStereoAudioProvider : IAudioProvider
    {
        private IAudioProvider original;
        private AudioRenderingBuffer buffer;

        /// <summary>
        /// Creates a new <see cref="MonoToStereoAudioProvider"/> instance , providing the monaural audio provider. <br />
        /// If it is attempted to provide a non-monaural provider, this constructor throws an <see cref="ArgumentException"/>.
        /// </summary>
        /// <param name="monoprovider">The monaural audio provider to provide.</param>
        /// <exception cref="ArgumentNullException"><paramref name="monoprovider"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentException"><paramref name="monoprovider"/> is not a monophonic audio provider.</exception>
        public MonoToStereoAudioProvider(IAudioProvider monoprovider)
        {
            ArgumentNullException.ThrowIfNull(monoprovider);
            if (monoprovider.Format.ChannelLayout.IsEqualTo(CommonChannelTypes.Mono) == false) {
                throw new ArgumentException("The provided audio provider is not a monophonic audio provider." , nameof(monoprovider));
            }
            buffer = new();
            original = monoprovider;
        }

        /// <summary>
        /// Gets the audio format, after passed through this audio provider
        /// </summary>
        public AudioFormat Format
        {
            get {
                AudioFormat fmt = original.Format;
                return AudioFormat.CreateCustom(
                    fmt.SampleRate,
                    fmt.BitRate,
                    CommonChannelTypes.Stereo,
                    fmt.BlockAlignment * 2,
                    fmt.CommonFormat);
            }
        }

        /// <inheritdoc/>
        public System.Int32 Read(System.Byte[] buffer, System.Int32 offset, System.Int32 count) 
        {
            if (count == 0) { return 0; }

            // Read count / finalsize bytes from the original provider
            System.Int32 monosamplesize = original.Format.BlockAlignment , finalsamplesize = monosamplesize * 2;
            System.Int32 rb = (count / finalsamplesize) * monosamplesize;
            this.buffer.UpdateBufferSize(rb);
            System.Int32 rc = original.Read(this.buffer.Buffer , 0 , rb);
            if (rc == 0) { return 0; }
            System.UInt32 msint32 = monosamplesize.ToUInt32();
            // Back now to the number of stereo samples that are to be written...
            System.Int32 stereosize = (rc / monosamplesize) * finalsamplesize;
            for (System.Int32 I = offset , J = 0; I < offset + stereosize; I += finalsamplesize , J += monosamplesize)
            {
                this.buffer.CopyTo(ref buffer[I] , J , msint32);
                this.buffer.CopyTo(ref buffer[I+monosamplesize], J, msint32);
            }
            return stereosize; 
        }

        /// <summary>Disposes the underlying audio provider.</summary>
        public void Dispose()
        {
            buffer = null;
            original?.Dispose();
            original = null;
        }
    }
}