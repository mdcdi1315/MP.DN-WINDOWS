
using System.Runtime.CompilerServices;

namespace MP
{
    unsafe partial class StreamMethods
    {
        /// <summary>
        /// Writes a <see cref="System.ReadOnlySpan{T}"/> instance to a stream. <br />
        /// Note, however, that the method does not save type information to the stream, so 
        /// when you read back the span using the <see cref="ReadEncodedSpan{T}(System.IO.Stream)"/> method , 
        /// you must provide the encoded type. <br />
        /// Otherwise , you have incorrectly read the span.
        /// </summary>
        /// <returns>The number of bytes written to the stream. This value does exclude the four-byte signed integer written before the array was written to the stream.</returns>
        /// <typeparam name="T">The span's element type to be encoded. Only supports the unmanaged types.</typeparam>
        /// <param name="strm">The stream to write the encoded span to.</param>
        /// <param name="span">The span to encode.</param>
        /// <seealso cref="ReadEncodedSpan{T}(System.IO.Stream)"/>
        /// <seealso cref="ReadTypedArray{T}(System.IO.Stream, int)"/>
        /// <seealso cref="ReadTypedSpan{T}(System.IO.Stream, int)"/>
        public static long WriteEncodedSpan<T>(this System.IO.Stream strm , System.ReadOnlySpan<T> span)
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

        /// <summary>
        /// Reads a previously encoded <see cref="System.ReadOnlySpan{T}"/> by using the <see cref="WriteEncodedSpan{T}(System.IO.Stream, System.ReadOnlySpan{T})"/> method
        /// and returns it back as a <see cref="System.Span{T}"/> instance.
        /// </summary>
        /// <typeparam name="T">The span's element type that was used during encoding. Only supports the unmanaged types.</typeparam>
        /// <param name="strm">The stream to read the encoded span from.</param>
        /// <returns>The decoded span of a previously encoded one.</returns>
        /// <seealso cref="WriteEncodedSpan{T}(System.IO.Stream, System.ReadOnlySpan{T})"/>
        public static System.Span<T> ReadEncodedSpan<T>(this System.IO.Stream strm)
            where T : unmanaged
        {
            T[] ret = new T[strm.ReadInt32()];
            System.Byte[] dt = strm.ReadBytes(ret.LongLength * sizeof(T));
            Unsafe.CopyBlockUnaligned(ref Unsafe.As<T, System.Byte>(ref ret[0]), ref dt[0], dt.LongLength.ToUInt32());
            return new(ret);
        }

        /// <summary>
        /// Works the same as <see cref="ReadTypedArray{T}(System.IO.Stream, int)"/> but returns the data into a equivalent <see cref="System.Span{T}"/> instance.
        /// </summary>
        /// <typeparam name="T">The type that each element of the new array will have.</typeparam>
        /// <param name="strm">The stream to read the typed array from.</param>
        /// <param name="elementcount">The number of <typeparamref name="T"/> elements to read from the stream.</param>
        /// <returns>The typed span read from the stream.</returns>
        /// <exception cref="System.ArgumentOutOfRangeException"><paramref name="elementcount"/> was negative.</exception>
        /// <seealso cref="ReadTypedArray{T}(System.IO.Stream, int)"/>
        public static System.Span<T> ReadTypedSpan<T>(this System.IO.Stream strm , System.Int32 elementcount)
            where T : unmanaged => new(strm.ReadTypedArray<T>(elementcount));
    }
}