

using System;

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
        /// <param name="offset">The offset inside the <paramref name="buffer"/> to start placing data from.</param>
        /// <param name="count">The number of bytes to place into <paramref name="buffer"/>, if these can be actually placed.</param>
        /// <returns>The number of bytes read into <paramref name="buffer"/>.</returns>
        public System.Int32 Read(System.Byte[] buffer, System.Int32 offset, System.Int32 count);
    }
}