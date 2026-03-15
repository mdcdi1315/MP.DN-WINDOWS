
using System;
using MP.IO.Buffers;
using MP.Annotations.CodeAnalysis;
using System.Runtime.CompilerServices;
using MP.Utilities;

namespace MP
{
    unsafe partial class StreamMethods
    {
        /// <summary>
        /// Defines a temporary buffer size for operations that must use intermediate buffers.
        /// </summary>
        public const System.Int32 MaxTypicalBufferSize = BUFSIZE;

        /// <summary>
        /// Reads <paramref name="count"/> bytes from the stream.
        /// </summary>
        /// <param name="stream">The stream to read the bytes from.</param>
        /// <param name="count">The number of bytes to read.</param>
        /// <returns>The read array.</returns>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="count"/> parameter was negative.</exception>
        /// <seealso cref="ReadBytes_Span"/>
        [Throws(typeof(System.IO.IOException), typeof(NotSupportedException), typeof(ObjectDisposedException), typeof(ArgumentOutOfRangeException))]
        public static System.Byte[] ReadBytes(this MP.IO.IDataStreamAccess stream, System.Int32 count)
        {
            if (count < 0) {
                throw new ArgumentOutOfRangeException(nameof(count), "The number of bytes to copy cannot be negative.");
            } else if (count == 0) {
                return System.Array.Empty<System.Byte>();
            } else if (count < BUFSIZE) {
                // Trivial array / buffer reading, use fast path
                System.Byte[] ret = new System.Byte[count];
                int read_total = 0, read;
                // If not all bytes requested were able to be fetched in a single pass,
                // the method is called again with the number of remaining bytes to read.
                do {
                    read_total += (read = stream.Read(ret, read_total, count - read_total));
                } while (read > 0 && read_total < count);
                return ret;
            } else {
                // Use our optimized workhorse method for all large arrays - they will benefit from buffering.
                return ReadBytes(stream, count, BUFSIZE);
            }
        }

        /// <summary>Reads <paramref name="bytes_to_copy"/> bytes from the stream.</summary>
        /// <param name="stream">The stream to read the bytes from.</param>
        /// <param name="bytes_to_copy">The number of bytes to read.</param>
        /// <returns>The read array.</returns>
        /// <exception cref="System.IO.IOException">An I/O exception was occurred.</exception>
        /// <exception cref="ObjectDisposedException">This stream has been disposed of.</exception>
        /// <exception cref="NotSupportedException">The read operation is not supported.</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="bytes_to_copy"/> parameter was negative.</exception>
        [Throws(typeof(System.IO.IOException), typeof(NotSupportedException), typeof(ObjectDisposedException), typeof(ArgumentOutOfRangeException))]
        public static System.Byte[] ReadBytes(this MP.IO.IDataStreamAccess stream, long bytes_to_copy) => ReadBytes(stream, bytes_to_copy, BUFSIZE);

        /// <summary>
        /// Reads <paramref name="bytes_to_copy"/> bytes from the stream.
        /// </summary>
        /// <param name="stream">The stream to read the bytes from.</param>
        /// <param name="bytes_to_copy">The number of bytes to read.</param>
        /// <param name="buffer_size">The temporary buffer size to use for copying the data to the newly created array.</param>
        /// <returns>The read array.</returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="bytes_to_copy"/> parameter was negative. <br /> 
        /// -or- <br />
        /// <paramref name="buffer_size"/> was too small to be used for a temporary buffer.
        /// </exception>
        /// <exception cref="System.IO.IOException">An I/O exception was occurred.</exception>
        /// <exception cref="ObjectDisposedException">This stream has been disposed of.</exception>
        /// <exception cref="NotSupportedException">The read operation is not supported.</exception>
        /// <seealso cref="ReadBytes(MP.IO.IDataStreamAccess, int)"/>
        /// <seealso cref="ReadBytes(MP.IO.IDataStreamAccess, long)"/>
        [Throws(typeof(System.IO.IOException), typeof(NotSupportedException), typeof(ObjectDisposedException), typeof(ArgumentOutOfRangeException))]
        public static System.Byte[] ReadBytes(this MP.IO.IDataStreamAccess stream, long bytes_to_copy, int buffer_size)
        {
            if (buffer_size < 1024) {
                throw new ArgumentOutOfRangeException(nameof(buffer_size), "The buffer_size parameter is too small and could degrade performance.");
            } else if (bytes_to_copy < 0L) {
                throw new ArgumentOutOfRangeException(nameof(bytes_to_copy), "The number of bytes to copy cannot be negative.");
            } else if (bytes_to_copy == 0L) {
                return Array.Empty<System.Byte>();
            } else {
                System.Byte[] ret = new System.Byte[bytes_to_copy];

                int temp_read_bytes, actual_buf_len;

                using (var buffer = System.Buffers.ArrayPool<System.Byte>.Shared.RentByContext(buffer_size))
                {
                    actual_buf_len = buffer.ActualLength;
                    ref System.Byte p0 = ref buffer.GetPinnableReference();
                    for (long consumed = 0; consumed < bytes_to_copy; consumed += temp_read_bytes)
                    {
                        if ((temp_read_bytes = stream.Read(buffer, 0, ComputeStreamBufferSize(consumed, bytes_to_copy, actual_buf_len))) > 0) {
                            Unsafe.CopyBlockUnaligned(ref ret[consumed], ref p0, temp_read_bytes.ToUInt32());
                        } else {
                            break;
                        }
                    }
                }

                return ret;
            }
        }

        /// <summary>
        /// Writes all the data directly to the stream by writing these sequentially.
        /// </summary>
        /// <param name="stream">The stream to write the bytes to.</param>
        /// <param name="data">The bytes to write to the stream.</param>
        /// <exception cref="System.IO.IOException">An I/O exception was occurred.</exception>
        /// <exception cref="ObjectDisposedException">This stream has been disposed of.</exception>
        /// <exception cref="NotSupportedException">The write operation is not supported.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="data"/> is <see langword="null"/>.</exception>
        [Throws(typeof(System.IO.IOException), typeof(NotSupportedException), typeof(ObjectDisposedException), typeof(ArgumentNullException))]
        public static void WriteBytes(this MP.IO.IDataStreamAccess stream, System.Byte[] data)
        {
            ArgumentNullException.ThrowIfNull(data);

            // Abstract: Gets all the bytes defined in the 'data' array and copies them to the stream.
            // To avoid hard limits such as int integer limits, the data copy is instead managed by an temporary buffer that is dispatching writes to the stream.
            // This would be the same as writing directly the array, however this assures that these implicit limits do not longer pose problems.

            uint transferred; // # of bytes actually transferred to the temporary buffer
            long len = data.LongLength; // The length of the 'data' buffer

            using (var temp = System.Buffers.ArrayPool<System.Byte>.Shared.RentByContext(2048))
            {
                ref System.Byte p0 = ref temp.GetPinnableReference();
                for (long consumed = 0; consumed < len; consumed += transferred)
                {
                    Unsafe.CopyBlockUnaligned(ref p0, ref data[consumed], transferred = ComputeStreamBufferSize(consumed, len, 2048U));

                    stream.Write(temp, 0, transferred.ToInt32());
                }
            }
        }

        /// <summary>
        /// Reads the specified number of bytes, but it does not return them. <br />
        /// It does only return the number of bytes that were read but not used, which might be less than <paramref name="count"/>.
        /// </summary>
        /// <param name="stream">The stream to read the specified bytes.</param>
        /// <param name="count">The number of bytes to discard.</param>
        /// <returns>The actual number of bytes that were discarded.</returns>
        /// <exception cref="System.IO.IOException">An I/O exception was occurred.</exception>
        /// <exception cref="ObjectDisposedException">This stream has been disposed of.</exception>
        /// <exception cref="NotSupportedException">The read operation is not supported.</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="count"/> is negative.</exception>
        [Throws(typeof(System.IO.IOException), typeof(NotSupportedException), typeof(ObjectDisposedException), typeof(ArgumentOutOfRangeException))]
        public static long DiscardBytes(this MP.IO.IDataStreamAccess stream, long count)
        {
            if (count < 0L) {
                throw new ArgumentOutOfRangeException(nameof(count), "Count must not be negative.");
            } else if (count == 0L) {
                return 0L;
            } else {
                long discarded;

                using (var temp = System.Buffers.ArrayPool<System.Byte>.Shared.RentByContext(BUFSIZE))
                {
                    int read_bytes, p_buf_size = temp.ActualLength;

                    for (discarded = 0L; discarded < count; discarded += read_bytes)
                    {
                        if ((read_bytes = stream.Read(temp, 0, ComputeStreamBufferSize(discarded, count, p_buf_size))) < 1) { break; }
                    }
                }

                return discarded;
            }
        }

        /// <summary>
        /// Reads the specified number of bytes, but it does not return them. <br />
        /// It does only return the number of bytes that were read but not used, which might be less than <paramref name="count"/>.
        /// </summary>
        /// <param name="stream">The stream to read the specified bytes.</param>
        /// <param name="count">The number of bytes to discard.</param>
        /// <returns>The actual number of bytes that were discarded.</returns>
        /// <exception cref="System.IO.IOException">An I/O exception was occurred.</exception>
        /// <exception cref="ObjectDisposedException">This stream has been disposed of.</exception>
        /// <exception cref="NotSupportedException">The read operation is not supported.</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="count"/> is negative.</exception>
        [Throws(typeof(System.IO.IOException), typeof(NotSupportedException), typeof(ObjectDisposedException), typeof(ArgumentOutOfRangeException))]
        public static int DiscardBytes(this MP.IO.IDataStreamAccess stream, int count) => DiscardBytes(stream, count.ToInt64()).ToInt32();

        /// <summary>
        /// Reads a typed array from the stream , specifying the number of <typeparamref name="T"/> elements to read.
        /// </summary>
        /// <typeparam name="T">The type that each element of the new array will have.</typeparam>
        /// <param name="stream">The stream to read the typed array from.</param>
        /// <param name="elementcount">The number of <typeparamref name="T"/> elements to read from the <paramref name="stream"/>.</param>
        /// <returns>The typed array read from the stream.</returns>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="elementcount"/> was negative.</exception>
        /// <seealso cref="ReadTypedSpan{T}(MP.IO.IDataStreamAccess, int)"/>
        [Throws(
            typeof(System.IO.IOException),
            typeof(NotSupportedException),
            typeof(ObjectDisposedException),
            typeof(ArgumentOutOfRangeException)
        )]
        public static T[] ReadTypedArray<T>(this MP.IO.IDataStreamAccess stream, System.Int32 elementcount)
            where T : unmanaged
        {
            if (elementcount < 0) { 
                throw new System.ArgumentOutOfRangeException(nameof(elementcount), "Number of elements to read must not be negative."); 
            } else {
                T[] ret = new T[elementcount];
                System.Byte[] dataread = stream.ReadBytes(ret.LongLength * sizeof(T).ToInt64());
                Unsafe.CopyBlockUnaligned(ref Unsafe.As<T, System.Byte>(ref ret[0]), ref dataread[0], dataread.LongLength.ToUInt32());
                return ret;
            }
        }
    }
}