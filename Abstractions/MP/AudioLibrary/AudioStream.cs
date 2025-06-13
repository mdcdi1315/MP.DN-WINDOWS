
using System;
using System.Runtime.CompilerServices;

namespace MP.AudioLibrary
{
    /// <summary>
    /// Audio Stream class definition. <br />
    /// Audio streams are a more closer and formal definition of how a codec can read data from an audio file or source.
    /// </summary>
    public abstract class AudioStream : IAudioProvider
    {
        /// <summary>
        /// Validates arguments provided to the <see cref="Read"/> method on <see cref="AudioStream"/>. <br />
        /// Use this to handle common errors regarding buffer arguments validity.
        /// </summary>
        /// <param name="buffer">The array "buffer" argument passed to the reading or writing method.</param>
        /// <param name="offset">The integer "offset" argument passed to the reading or writing method.</param>
        /// <param name="count">The integer "count" argument passed to the reading or writing method.</param>
        /// <exception cref="ArgumentNullException"><paramref name="buffer"/> was null.</exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="offset"/> was outside the bounds of <paramref name="buffer"/>, or
        /// <paramref name="count"/> was negative, or the range specified by the combination of
        /// <paramref name="offset"/> and <paramref name="count"/> exceed the length of <paramref name="buffer"/>.
        /// </exception>
        // Ported from the original ValidateBufferArguments of the System.IO.Stream class in .NET .
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected static void ValidateBufferArguments(System.Byte[] buffer, System.Int32 offset, System.Int32 count)
        {
            ArgumentNullException.ThrowIfNull(buffer);

            if (offset < 0) {
                throw new ArgumentOutOfRangeException(nameof(offset) , "Offset must not be negative.");
            }

            if ((System.UInt32)count > buffer.Length - offset) {
                throw new ArgumentOutOfRangeException(nameof(count), "Attempted to read beyond the tempoary buffer bounds.");
            }
        }

        /// <summary>
        /// Forwarded from the <see cref="IAudioProvider"/> interface. <br />
        /// Reads raw audio data and places them to the specified buffer. <br />
        /// The buffer must be valid. <br />
        /// Additionally, the returned number indicates the number of audio bytes placed into <paramref name="buffer"/>.
        /// </summary>
        /// <param name="buffer">The buffer to place audio data into</param>
        /// <param name="offset">The offset inside the <paramref name="buffer"/> to start placing data from.</param>
        /// <param name="count">The number of bytes to place into <paramref name="buffer"/>, if these can be actually placed.</param>
        /// <returns>The number of bytes read into <paramref name="buffer"/>.</returns>
        public abstract System.Int32 Read(System.Byte[] buffer, System.Int32 offset, System.Int32 count);

        /// <summary>
        /// Forwarded from the <see cref="IAudioProvider"/> interface. <br />
        /// Gets the audio format under which the current audio stream is.
        /// </summary>
        public abstract AudioFormat Format { get; }

        /// <summary>
        /// If possible, it gets and sets the time where the codec is in the audio data.
        /// </summary>
        public virtual TimeSpan CurrentTime 
        {
            get => TimeSpan.Zero;
            set => throw new NotSupportedException("Seeking services are not supported.");
        }

        /// <summary>
        /// If possible, it gets the total time of the entire audio stream.
        /// </summary>
        public virtual TimeSpan TotalTime
        {
            get => TimeSpan.Zero;
        }

        /// <summary>
        /// Disposes resources held by the current <see cref="AudioStream"/> instance. <br />
        /// All cleanup code that you need should be in here
        /// </summary>
        /// <param name="disposing">When <see langword="true"/>, this method was called from <see cref="Dispose()"/>. Otherwise, it was called from the finalizer.</param>
        protected virtual void Dispose(bool disposing) { }

        /// <summary>
        /// Disposes this <see cref="AudioStream"/> instance.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Default finalizer for <see cref="AudioStream"/>.
        /// </summary>
        ~AudioStream() => Dispose(false);
    }
}