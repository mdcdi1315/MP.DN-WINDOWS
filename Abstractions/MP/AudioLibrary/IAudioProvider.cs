

using System;
using MP.Annotations.CodeAnalysis;

namespace MP.AudioLibrary
{
    /// <summary>
    /// Defines the bare implementation requirement so that an audio player can play audio data. <br />
    /// Audio providers are usually implemented by codecs and audio engines.
    /// </summary>
    public interface IAudioProvider : IDisposable
    {
        /// <summary>
        /// Indicates the audio format of this audio provider
        /// </summary>
        public AudioFormat Format { get; }

        /// <summary>
        /// Reads raw audio data and places them to the specified buffer. <br />
        /// The buffer must be valid. <br />
        /// Additionally, the returned number indicates the number of audio bytes placed into <paramref name="buffer"/>.
        /// </summary>
        /// <param name="buffer">The buffer to place audio data into</param>
        /// <returns>The number of bytes read into <paramref name="buffer"/>.</returns>
        public System.Int32 Read(Span<System.Byte> buffer);

        /// <summary>
        /// Reads raw audio data and places them to the specified buffer. <br />
        /// The buffer must be valid. <br />
        /// Additionally, the returned number indicates the number of audio bytes placed into <paramref name="buffer"/>.
        /// </summary>
        /// <param name="buffer">The buffer to place audio data into</param>
        /// <param name="index">The starting index inside <paramref name="buffer"/> to start placing data to.</param>
        /// <param name="count">The number of bytes to place in <paramref name="buffer"/>.</param>
        /// <returns>The number of bytes read into <paramref name="buffer"/>.</returns>
        [Throws(typeof(ArgumentNullException), typeof(ArgumentOutOfRangeException))]
        public System.Int32 Read(System.Byte[] buffer, int index, int count)
        {
            ArgumentNullException.ThrowIfNull(buffer);
            return Read(new Span<byte>(buffer, index, count));
        }
    }
}