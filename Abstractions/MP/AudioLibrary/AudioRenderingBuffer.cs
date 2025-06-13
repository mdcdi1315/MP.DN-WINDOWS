
using System;
using System.Runtime.CompilerServices;

namespace MP.AudioLibrary
{
    /// <summary>
    /// Provides a way , and common utilities for a buffer that is used in audio rendering. <br />
    /// For this purpose, it contains specialized methods to read from, and write to, the provided data buffer. <br />
    /// This specialized class is also used in some channel conversion audio providers.
    /// </summary>
    public unsafe sealed class AudioRenderingBuffer
    {
        private System.Byte[] data;

        /// <summary>
        /// Creates an empty audio rendering buffer.
        /// </summary>
        public AudioRenderingBuffer() => data = Array.Empty<System.Byte>();

        /// <summary>
        /// Creates an audio rendering buffer, preallocating the initially required size.
        /// </summary>
        /// <param name="initialcapacity">The initial capacity to be preallocated for the buffer.</param>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="initialcapacity"/> was negative.</exception>
        public AudioRenderingBuffer(System.Int64 initialcapacity)
        {
            if (initialcapacity < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(initialcapacity) , "Initial buffer capacity must not be negative.");
            }
            data = new System.Byte[initialcapacity];
        }

        /// <summary>
        /// Gets the data buffer itself. Useful so as to pass it in managed code.
        /// </summary>
        public System.Byte[] Buffer => data;

        /// <summary>
        /// Updates the buffer's size, or leaves it as is if <paramref name="newsize"/> is less than the buffer size.
        /// </summary>
        /// <param name="newsize">The new, requested size of the buffer.</param>
        public void UpdateBufferSize(System.Int64 newsize)
        {
            if (data.LongLength < newsize) {
                data = new System.Byte[newsize];
            }
        }

        #region CopyTo overloads

        /// <summary>
        /// Copies data to a native memory block pointing at <paramref name="pnative"/>, from the beginning of the current buffer. <br />
        /// NOTE: The method does not check for a <see langword="null"/> pointer, so as this call can be as fast as possible.
        /// </summary>
        /// <param name="pnative">The native pointer to copy data to.</param>
        /// <param name="length">The number of bytes to copy</param>
        public void CopyTo(System.Byte* pnative, System.UInt32 length)
            => Unsafe.CopyBlockUnaligned(ref pnative[0], ref data[0] , length);

        /// <summary>
        /// Copies data to a managed pointer pointing at <paramref name="reference"/>, from the beginning of the current buffer. <br />
        /// NOTE: The method does not check for a <see langword="null"/> pointer, so as this call can be as fast as possible.
        /// </summary>
        /// <param name="reference">The managed reference to copy data to.</param>
        /// <param name="length">The number of bytes to copy</param>
        public void CopyTo(ref System.Byte reference, System.UInt32 length)
            => Unsafe.CopyBlockUnaligned(ref reference, ref data[0], length);

        /// <summary>
        /// Copies data to a native memory block pointing at <paramref name="pnative"/>, from the specified offset inside the buffer. <br />
        /// NOTE: The method does not check for a <see langword="null"/> pointer, so as this call can be as fast as possible.
        /// </summary>
        /// <param name="pnative">The native pointer to copy data to.</param>
        /// <param name="length">The number of bytes to copy</param>
        /// <param name="offset">The offset to start copying data to <paramref name="pnative"/> memory block.</param>
        public void CopyTo(System.Byte* pnative, System.Int32 offset, System.UInt32 length)
            => Unsafe.CopyBlockUnaligned(ref pnative[0], ref data[offset], length);

        /// <summary>
        /// Copies data to a managed pointer pointing at <paramref name="reference"/>, from the specified offset inside the buffer. <br />
        /// NOTE: The method does not check for a <see langword="null"/> pointer, so as this call can be as fast as possible.
        /// </summary>
        /// <param name="reference">The managed reference to copy data to.</param>
        /// <param name="length">The number of bytes to copy</param>
        /// <param name="offset">The offset to start copying data to <paramref name="reference"/> managed pointer.</param>
        public void CopyTo(ref System.Byte reference, System.Int32 offset, System.UInt32 length)
            => Unsafe.CopyBlockUnaligned(ref reference, ref data[offset], length);

        #endregion

        #region CopyFrom overloads

        /// <summary>
        /// Copies data to the current buffer, from a native memory block pointing at <paramref name="pnative"/>. <br />
        /// NOTE: The method does not check for a <see langword="null"/> pointer, so as this call can be as fast as possible.
        /// </summary>
        /// <param name="pnative">The native pointer to copy data from.</param>
        /// <param name="length">The number of bytes to copy</param>
        public void CopyFrom(System.Byte* pnative, System.UInt32 length)
            => Unsafe.CopyBlockUnaligned(ref data[0], ref pnative[0], length);

        /// <summary>
        /// Copies data to the current buffer, from a managed pointer pointing at <paramref name="reference"/>. <br />
        /// NOTE: The method does not check for a <see langword="null"/> pointer, so as this call can be as fast as possible.
        /// </summary>
        /// <param name="reference">The managed reference to copy data from.</param>
        /// <param name="length">The number of bytes to copy</param>
        public void CopyFrom(ref System.Byte reference, System.UInt32 length)
            => Unsafe.CopyBlockUnaligned(ref data[0], ref reference, length);

        /// <summary>
        /// Copies data to the current buffer, from a native memory block pointing at <paramref name="pnative"/>, and with the specified offset inside the data buffer. <br />
        /// NOTE: The method does not check for a <see langword="null"/> pointer, so as this call can be as fast as possible.
        /// </summary>
        /// <param name="pnative">The native pointer to copy data from.</param>
        /// <param name="length">The number of bytes to copy</param>
        /// <param name="offset">The offset from the buffer's beginning to start copying the data to.</param>
        public void CopyFrom(System.Byte* pnative , System.Int32 offset , System.UInt32 length)
            => Unsafe.CopyBlockUnaligned(ref data[offset] , ref pnative[offset], length);

        /// <summary>
        /// Copies data to the current buffer, from a managed pointer pointing at <paramref name="reference"/>, and with the specified offset inside the data buffer. <br />
        /// NOTE: The method does not check for a <see langword="null"/> pointer, so as this call can be as fast as possible.
        /// </summary>
        /// <param name="reference">The managed reference to copy data from.</param>
        /// <param name="length">The number of bytes to copy</param>
        /// <param name="offset">The offset from the buffer's beginning to start copying the data to.</param>
        public void CopyFrom(ref System.Byte reference , System.Int32 offset, System.UInt32 length)
            => Unsafe.CopyBlockUnaligned(ref data[offset], ref reference, length);

        /// <summary>
        /// Copies data to the current buffer, from a native memory block pointing at <paramref name="pnative"/>, and with the specified offset inside the data buffer. <br />
        /// NOTE: The method does not check for a <see langword="null"/> pointer, so as this call can be as fast as possible.
        /// </summary>
        /// <param name="pnative">The native pointer to copy data from.</param>
        /// <param name="length">The number of bytes to copy</param>
        /// <param name="offset">The offset from the buffer's beginning to start copying the data to.</param>
        public void CopyFrom(System.Byte* pnative, System.Int64 offset, System.UInt32 length)
            => Unsafe.CopyBlockUnaligned(ref data[offset], ref pnative[offset], length);

        /// <summary>
        /// Copies data to the current buffer, from a managed pointer pointing at <paramref name="reference"/>, and with the specified offset inside the data buffer. <br />
        /// NOTE: The method does not check for a <see langword="null"/> pointer, so as this call can be as fast as possible.
        /// </summary>
        /// <param name="reference">The managed reference to copy data from.</param>
        /// <param name="length">The number of bytes to copy</param>
        /// <param name="offset">The offset from the buffer's beginning to start copying the data to.</param>
        public void CopyFrom(ref System.Byte reference, System.Int64 offset, System.UInt32 length)
            => Unsafe.CopyBlockUnaligned(ref data[offset], ref reference, length);
        
        #endregion

        #region Zeroize overloads
        
        /// <summary>
        /// From a given starting point, this method clears the memory block so that it can be empty.
        /// </summary>
        /// <param name="dataindex">The index to start writing zeroes from</param>
        /// <param name="zeroesblocklength">The number of zero bytes to write</param>
        public void Zeroize(System.Int32 dataindex , System.UInt32 zeroesblocklength)
            => Unsafe.InitBlockUnaligned(ref data[dataindex] , 0 , zeroesblocklength);

        /// <summary>
        /// From a given starting point, this method clears the memory block so that it can be empty.
        /// </summary>
        /// <param name="dataindex">The index to start writing zeroes from</param>
        /// <param name="zeroesblocklength">The number of zero bytes to write</param>
        public void Zeroize(System.UInt32 dataindex, System.UInt32 zeroesblocklength)
            => Unsafe.InitBlockUnaligned(ref data[dataindex], 0, zeroesblocklength);

        /// <summary>
        /// From a given starting point, this method clears the memory block so that it can be empty.
        /// </summary>
        /// <param name="dataindex">The index to start writing zeroes from</param>
        /// <param name="zeroesblocklength">The number of zero bytes to write</param>
        public void Zeroize(System.Int64 dataindex, System.UInt32 zeroesblocklength)
            => Unsafe.InitBlockUnaligned(ref data[dataindex], 0, zeroesblocklength);
        
        #endregion
    }
}