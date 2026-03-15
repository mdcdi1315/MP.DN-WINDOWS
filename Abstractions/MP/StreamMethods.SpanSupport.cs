
using System;
using MP.Annotations.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace MP
{
    unsafe partial class StreamMethods
    {
        /// <summary>
        /// Writes a <see cref="ReadOnlySpan{T}"/> instance to a stream. <br />
        /// Note, however, that the method does not save type information to the stream, so 
        /// when you read back the span using the <see cref="ReadEncodedSpan{T}(MP.IO.IDataStreamAccess)"/> method , 
        /// you must provide the encoded type. <br />
        /// Otherwise , you have incorrectly read the span.
        /// </summary>
        /// <returns>The number of bytes written to the stream. This value does exclude the four-byte signed integer written before the array was written to the stream.</returns>
        /// <typeparam name="T">The span's element type to be encoded. Only supports the unmanaged types.</typeparam>
        /// <param name="strm">The stream to write the encoded span to.</param>
        /// <param name="span">The span to encode.</param>
        /// <seealso cref="ReadEncodedSpan{T}(MP.IO.IDataStreamAccess)"/>
        /// <seealso cref="ReadTypedArray{T}(MP.IO.IDataStreamAccess, int)"/>
        /// <seealso cref="ReadTypedSpan{T}(MP.IO.IDataStreamAccess, int)"/>
        public static long WriteEncodedSpan<T>(this MP.IO.IDataStreamAccess strm , System.ReadOnlySpan<T> span)
            where T : unmanaged
        { 
            strm.WriteInt32(span.Length);
            if (span.Length > 0) {
                System.Byte[] dt = new System.Byte[span.Length * sizeof(T)];
                Unsafe.CopyBlockUnaligned(ref dt[0], ref Unsafe.As<T, System.Byte>(ref Unsafe.AsRef(in span[0])), dt.LongLength.ToUInt32());
                strm.WriteBytes(dt);
                return dt.LongLength;
            } else {
                return 0;
            }
        }

        /// <summary>Reads <paramref name="count"/> bytes from the stream.</summary>
        /// <param name="stream">The stream to read the bytes from.</param>
        /// <param name="count">The number of bytes to read.</param>
        /// <returns>The read byte span.</returns>
        /// <exception cref="System.IO.IOException">An I/O exception was occurred.</exception>
        /// <exception cref="ObjectDisposedException">This stream has been disposed of.</exception>
        /// <exception cref="NotSupportedException">The read operation is not supported.</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="count"/> parameter was negative.</exception>
        [Throws(typeof(System.IO.IOException), typeof(NotSupportedException), typeof(ObjectDisposedException), typeof(ArgumentOutOfRangeException))]
        public static Span<System.Byte> ReadBytes_Span(this MP.IO.IDataStreamAccess stream, System.Int32 count)
        {
            if (count < 0) {
                throw new ArgumentOutOfRangeException(nameof(count), "The number of bytes to copy cannot be negative.");
            } else if (count == 0) {
                return Span<System.Byte>.Empty;
            } else if (count < BUFSIZE) {
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


        /// <summary>
        /// Reads a previously encoded <see cref="ReadOnlySpan{T}"/> by using the <see cref="WriteEncodedSpan{T}(MP.IO.IDataStreamAccess, System.ReadOnlySpan{T})"/> method
        /// and returns it back as a <see cref="Span{T}"/> instance.
        /// </summary>
        /// <typeparam name="T">The span's element type that was used during encoding. Only supports the unmanaged types.</typeparam>
        /// <param name="strm">The stream to read the encoded span from.</param>
        /// <returns>The decoded span of a previously encoded one.</returns>
        /// <seealso cref="WriteEncodedSpan{T}(MP.IO.IDataStreamAccess, System.ReadOnlySpan{T})"/>
        public static Span<T> ReadEncodedSpan<T>(this MP.IO.IDataStreamAccess strm)
            where T : unmanaged
        {
            T[] ret = new T[strm.ReadInt32()];
            System.Byte[] dt = strm.ReadBytes(ret.LongLength * sizeof(T));
            Unsafe.CopyBlockUnaligned(ref Unsafe.As<T, System.Byte>(ref ret[0]), ref dt[0], dt.LongLength.ToUInt32());
            return new(ret);
        }

        /// <summary>
        /// Works the same as <see cref="ReadTypedArray{T}(MP.IO.IDataStreamAccess, int)"/> but returns the data into a equivalent <see cref="System.Span{T}"/> instance.
        /// </summary>
        /// <typeparam name="T">The type that each element of the new array will have.</typeparam>
        /// <param name="strm">The stream to read the typed array from.</param>
        /// <param name="elementcount">The number of <typeparamref name="T"/> elements to read from the stream.</param>
        /// <returns>The typed span read from the stream.</returns>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="elementcount"/> was negative.</exception>
        /// <seealso cref="ReadTypedArray{T}(MP.IO.IDataStreamAccess, int)"/>
        public static Span<T> ReadTypedSpan<T>(this MP.IO.IDataStreamAccess strm , System.Int32 elementcount)
            where T : unmanaged => new(strm.ReadTypedArray<T>(elementcount));
    }
}