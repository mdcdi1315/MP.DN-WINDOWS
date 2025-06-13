
using System;

namespace MP.Utilities
{
    /// <summary>
    /// Defines common extension methods around the <see cref="System.IO.Stream"/> class.
    /// </summary>
    public static class StreamExtensions
    {
        private const System.Int32 BufferSize = 4096;

        /// <summary>
        /// Copies data from an input stream to another , respecting both stream positions.
        /// </summary>
        /// <param name="input">The input stream to copy data from.</param>
        /// <param name="destination">The destination stream to save data to.</param>
        /// <param name="bufferSize">The temporary buffer size.</param>
        /// <exception cref="ArgumentNullException"><paramref name="input"/> and/or <paramref name="destination"/> were null.</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="bufferSize"/> is less than 512 and/or not a multiple of 512.</exception>
        public static void EnsuredCopyTo(this System.IO.Stream input, System.IO.Stream destination, int bufferSize)
        {
            if (input is null) { throw new ArgumentNullException(nameof(input)); }
            if (destination is null) { throw new ArgumentNullException(nameof(destination)); }
            if (bufferSize < 512 || bufferSize % 512 != 0) { throw new ArgumentOutOfRangeException(nameof(bufferSize) , "Buffer size must be a value more than 512 bytes and must be divisible by 512 bytes."); }
            byte[] array = new byte[bufferSize];
            int count;
            while ((count = input.Read(array, 0, array.Length)) > 0)
            {
                destination.Write(array, 0, count);
            }
        }

        /// <summary>
        /// Copies data from the current stream to another, respecting both stream positions. <br />
        /// The <paramref name="size"/> parameter specifies the exact amount of data, in bytes, to copy.
        /// </summary>
        /// <param name="input">The input stream to copy data from.</param>
        /// <param name="destination">The destination stream to save data to.</param>
        /// <param name="buffersize">The temporary copy buffer size window, in bytes.</param>
        /// <param name="size">The exact amount of data, in bytes, to copy to <paramref name="destination"/>.</param>
        /// <exception cref="ArgumentNullException"><paramref name="input"/> and/or <paramref name="destination"/> were null.</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="buffersize"/> is less than 512 and/or not a multiple of 512, or <paramref name="size"/> is not positive.</exception>
        public static void CopyToExactly(this System.IO.Stream input, System.IO.Stream destination, System.Int32 buffersize, System.Int64 size)
        {
            if (input is null) { throw new ArgumentNullException(nameof(input)); }
            if (destination is null) { throw new ArgumentNullException(nameof(destination)); }
            if (buffersize < 512 || buffersize % 512 != 0) { throw new ArgumentOutOfRangeException(nameof(buffersize), "Buffer size must be a value more than 512 bytes and must be divisible by 512 bytes."); }
            // When 0 do not fail but instead do nothing.
            if (size == 0) { return; }
            if (size < 0) { throw new ArgumentOutOfRangeException(nameof(size), "The amount of data to copy must be a positive integer."); }
            System.Byte[] array = new System.Byte[buffersize];
            System.Int32 count;
            // When the buffer size is larger than the number of bytes to copy...
            if (buffersize > size)
            {
                // Re-create the buffer to be 'size' bytes instead
                array = new System.Byte[size];
                if ((count = input.Read(array, 0, array.Length)) > 0)
                {
                    // And write the read buffer into the other stream.
                    destination.Write(array, 0, count);
                }
                // And exit...
                return;
            }
            // Otherwise, copy buffer by buffer until everything is processed.
            System.Int64 written = 0;
            System.Int32 bufd = buffersize;
            // Continually read buffers until all the bytes have been processed
            while (written + 1 < size && (count = input.Read(array, 0, bufd)) > 0)
            {
                destination.Write(array, 0, count);
                written += count;
                // If the next operation happens not to be a complete buffer...
                if (written + bufd > size) {
                    // adjust it so that only the exact required bytes are copied instead
                    bufd = (size - written).ToInt32();
                }
            }
        }

        /// <summary>
        /// Copies data from the current stream to another, respecting both stream positions. <br />
        /// The <paramref name="size"/> parameter specifies the exact amount of data to copy.
        /// </summary>
        /// <param name="input">The input stream to copy data from.</param>
        /// <param name="destination">The destination stream to save data to.</param>
        /// <param name="size">The exact amount of data to copy to <paramref name="destination"/>.</param>
        /// <exception cref="ArgumentNullException"><paramref name="input"/> and/or <paramref name="destination"/> were null.</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="size"/> is not positive.</exception>
        public static void CopyToExactly(this System.IO.Stream input, System.IO.Stream destination, System.Int64 size)
            => CopyToExactly(input, destination, BufferSize, size);

        /// <summary>
        /// Copies data from an input stream to another , respecting both stream positions.
        /// </summary>
        /// <param name="input">The input stream to copy data from.</param>
        /// <param name="destination">The destination stream to save data to.</param>
        /// <exception cref="ArgumentNullException"><paramref name="input"/> and/or <paramref name="destination"/> are null.</exception>
        public static void EnsuredCopyTo(this System.IO.Stream input, System.IO.Stream destination)
            => EnsuredCopyTo(input, destination, BufferSize);
    }
}