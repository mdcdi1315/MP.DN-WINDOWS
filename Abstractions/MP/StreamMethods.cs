
using System;
using MP.Utilities;
using MP.Annotations.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace MP
{
    /// <summary>
    /// Provides extension methods for fast reading and writing data to streams.
    /// </summary>
    public static partial class StreamMethods
    {
        // A typed length for temporary buffers.
        private const System.Int32 BUFSIZE = 2048;

        // Acquired from https://github.com/dotnet/runtime/blob/main/src/libraries/System.Private.CoreLib/src/System/SpanHelpers.cs
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void ReverseByteReference(ref byte elements, int length)
        {
            if (length < 2) { return; }

            ref byte first = ref elements;
            ref byte last = ref Unsafe.Add(ref first, length - 1);
            do {
                byte temp = first;
                first = last;
                last = temp;
                first = ref Unsafe.Add(ref first, 1);
                last = ref Unsafe.Subtract(ref last, 1);
            } while (Unsafe.IsAddressLessThan(ref first, ref last));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static int ComputeStreamBufferSize(long consumed, long total, int buffer_size) => MathHelpers.ComputeBufferSize(consumed, total, buffer_size);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static uint ComputeStreamBufferSize(long consumed, long total, uint buffer_size) => MathHelpers.ComputeBufferSize(consumed, total, buffer_size);

        /// <summary>
        /// Writes a signed byte to the stream.
        /// </summary>
        /// <param name="stream">The stream to write the signed byte.</param>
        /// <param name="signedbyte">The signed byte to write.</param>
        [Throws(typeof(IO.IOException), typeof(NotSupportedException), typeof(ObjectDisposedException))]
        public static void WriteSByte(this IO.IDataStreamAccess stream, System.SByte signedbyte) => stream.WriteByte(signedbyte.ToByte());

        /// <summary>
        /// Reads a signed byte from the stream. 
        /// </summary>
        /// <param name="stream">The stream to read the signed byte.</param>
        /// <returns>The read signed byte.</returns>
        [Throws(typeof(IO.IOException), typeof(NotSupportedException), typeof(IO.StreamReachedEndException), typeof(ObjectDisposedException))]
        public static System.SByte ReadSByte(this IO.IDataStreamAccess stream) => ReadLiteralByte(stream).ToSByte();

        /// <summary>
        /// Writes a boolean to the stream.
        /// </summary>
        /// <param name="stream">The stream to write.</param>
        /// <param name="value">The boolean value to write.</param>
        public static void WriteBoolean(this IO.IDataStreamAccess stream, System.Boolean value) => stream.WriteByte((value ? 1 : 0).ToByte());

        /// <summary>
        /// Reads a boolean from the stream.
        /// </summary>
        /// <param name="stream">The stream to read from.</param>
        /// <returns>The read boolean value.</returns>
        [Throws(typeof(IO.IOException), typeof(NotSupportedException), typeof(ObjectDisposedException))]
        public static System.Boolean ReadBoolean(this IO.IDataStreamAccess stream) => ReadLiteralByte(stream) != 0;

        /// <summary>Reads a byte from the stream.</summary>
        /// <param name="stream">The stream to read from.</param>
        /// <returns>The read byte value.</returns>
        /// <exception cref="IO.StreamReachedEndException">The stream was ended.</exception>
        [Throws(typeof(IO.IOException), typeof(NotSupportedException), typeof(IO.StreamReachedEndException), typeof(ObjectDisposedException))]
        public static System.Byte ReadLiteralByte(this IO.IDataStreamAccess stream)
        {
            short byte2int = stream.ReadByte();
            return byte2int switch {
                -1 => throw new IO.StreamReachedEndException("Stream was ended prematurely."),
                _ => byte2int.ToByte(),
            };
        }

        /// <summary>
        /// Writes a structure of type <typeparamref name="T"/> to the stream.
        /// </summary>
        /// <typeparam name="T">The type of the structure to write.</typeparam>
        /// <param name="stream">The stream to write the structure.</param>
        /// <param name="structure">The structure to write.</param>
        /// <seealso cref="ReadStructure{T}(MP.IO.IDataStreamAccess)"/>
        [Throws(typeof(IO.IOException), typeof(NotSupportedException), typeof(ObjectDisposedException))]
        public static void WriteStructure<T>(this IO.IDataStreamAccess stream, T structure) where T : struct 
        {
            System.Byte[] temp = structure.WriteStructureToNewArray();
            if (!BitConverter.IsLittleEndian) {
                ReverseByteReference(ref temp[0], temp.Length);
            }
            stream.Write(temp);
        }

        /// <summary>
        /// Reads a structure of type <typeparamref name="T"/> from the stream.
        /// </summary>
        /// <typeparam name="T">The type of the structure to read.</typeparam>
        /// <param name="stream">The stream to read the structure from.</param>
        /// <returns>The read structure.</returns>
        /// <exception cref="System.IO.EndOfStreamException">The structure could not be read.</exception>
        /// <seealso cref="WriteStructure{T}(MP.IO.IDataStreamAccess, T)"/>
        [Throws(typeof(IO.IOException), typeof(NotSupportedException), typeof(IO.StreamReachedEndException), typeof(ObjectDisposedException))]
        public static T ReadStructure<T>(this IO.IDataStreamAccess stream) where T : struct
        {
            int rem = 0, read;
            Span<System.Byte> temp = new System.Byte[Unsafe.SizeOf<T>()];
            do {
                 rem += (read = stream.Read(temp.Slice(rem, temp.Length - rem)));
            } while (read > 0 && rem < temp.Length);
            if (rem != temp.Length) { throw new IO.StreamReachedEndException($"Cannot read the structure of type {typeof(T).FullName}: Expected to read {temp.Length} bytes while read {rem} bytes."); }
            if (!BitConverter.IsLittleEndian) {
                ReverseByteReference(ref temp[0], temp.Length);
            }
            return temp.ReadStructure<T>(0);
        }

        /// <summary>
        /// Writes a structure of type <typeparamref name="T"/> to the stream.
        /// </summary>
        /// <typeparam name="T">The type of the structure to write.</typeparam>
        /// <param name="stream">The stream to write the structure.</param>
        /// <param name="structure">The structure to write.</param>
        /// <seealso cref="ReadStructure{T}(MP.IO.IDataStreamAccess)"/>
        [Throws(typeof(IO.IOException), typeof(NotSupportedException), typeof(ObjectDisposedException))]
        public static void WriteStructureBE<T>(this IO.IDataStreamAccess stream, T structure) where T : struct
        {
            System.Byte[] temp = structure.WriteStructureToNewArray();
            if (BitConverter.IsLittleEndian) {
                ReverseByteReference(ref temp[0], temp.Length);
            }
            stream.Write(temp);
        }

        /// <summary>
        /// Reads a structure of type <typeparamref name="T"/> from the stream.
        /// </summary>
        /// <typeparam name="T">The type of the structure to read.</typeparam>
        /// <param name="stream">The stream to read the structure from.</param>
        /// <returns>The read structure.</returns>
        /// <exception cref="System.IO.EndOfStreamException">The structure could not be read.</exception>
        /// <seealso cref="WriteStructure{T}(MP.IO.IDataStreamAccess, T)"/>
        [Throws(typeof(IO.IOException), typeof(NotSupportedException), typeof(IO.StreamReachedEndException), typeof(ObjectDisposedException))]
        public static T ReadStructureBE<T>(this IO.IDataStreamAccess stream) where T : struct
        {
            int rem = 0, read;
            Span<System.Byte> temp = new System.Byte[Unsafe.SizeOf<T>()];
            do {
                rem += (read = stream.Read(temp.Slice(rem, temp.Length - rem)));
            } while (read > 0 && rem < temp.Length);
            if (rem != temp.Length) { throw new IO.StreamReachedEndException($"Cannot read the structure of type {typeof(T).FullName}: Expected to read {temp.Length} bytes while read {rem} bytes."); }
            if (BitConverter.IsLittleEndian) {
                ReverseByteReference(ref temp[0], temp.Length);
            }
            return temp.ReadStructure<T>(0);
        }

        /// <summary>
        /// Reads a 7-bit encoded integer from the stream.
        /// </summary>
        /// <param name="reader">The stream to read from.</param>
        /// <returns>The given 7-bit encoded integer.</returns>
        /// <exception cref="FormatException">The number was too long.</exception>
        /// <exception cref="System.IO.EndOfStreamException">The stream ended before the actual reading was finished.</exception>
        /// <seealso cref="Write7BitEncodedInt(MP.IO.IDataStreamAccess, int)"/>
        [Throws(typeof(IO.IOException), typeof(NotSupportedException), typeof(IO.StreamReachedEndException), typeof(ObjectDisposedException))]
        public static System.Int32 Read7BitEncodedInt(this IO.IDataStreamAccess reader)
        {
            System.Int32 num = 0 , bits = 0;
            System.Byte b;
            do {
                if (bits == 35) {
                    throw new FormatException("Too many bytes of what should have been a 7-bit encoded Int32.");
                }
                b = ReadLiteralByte(reader);
                num |= (b & 0x7F) << bits;
                bits += 7;
            } while ((b & 0x80U) != 0U);
            return num;
        }

        /// <summary>Writes a 7-bit encoded integer to the specified stream.</summary>
        /// <param name="writer">The stream to write to.</param>
        /// <param name="value">The integer to write.</param>
        /// <seealso cref="Read7BitEncodedInt(MP.IO.IDataStreamAccess)"/>
        [Throws(typeof(IO.IOException), typeof(NotSupportedException), typeof(ObjectDisposedException))]
        public static void Write7BitEncodedInt(this IO.IDataStreamAccess writer, System.Int32 value)
        {
            for (; (value & -128) != 0; value >>>= 7)
            {
                writer.WriteByte(unchecked((byte)((value | 0x80) & 0xFF)));
            }
            writer.WriteByte(unchecked((byte)value));
        }

        /// <summary>
        /// Reads a 7-bit encoded long integer from the stream.
        /// </summary>
        /// <param name="reader">The stream to read from.</param>
        /// <returns>The 7-bit encoded long integer.</returns>
        /// <exception cref="FormatException">The number was too long.</exception>
        /// <exception cref="System.IO.EndOfStreamException">The stream ended before the actual reading was finished.</exception>
        /// <seealso cref="Write7BitEncodedLong(MP.IO.IDataStreamAccess, long)"/>
        [Throws(typeof(IO.IOException), typeof(NotSupportedException), typeof(IO.StreamReachedEndException), typeof(ObjectDisposedException))]
        public static System.Int64 Read7BitEncodedLong(this IO.IDataStreamAccess reader)
        {
            System.Int64 num = 0;
            System.Int32 bits = 0;
            System.Byte b;
            do {
                if (bits == 63) {
                    throw new FormatException("Too many bytes of what should have been a 7-bit encoded Int64.");
                }
                b = ReadLiteralByte(reader);
                num |= (b & 0x7FL) << bits;
                bits += 7;
            } while ((b & 0x80U) != 0U);
            return num;
        }

        /// <summary>Writes a 7-bit encoded long integer to the specified stream.</summary>
        /// <param name="writer">The stream to write to.</param>
        /// <param name="value">The integer to write.</param>
        /// <seealso cref="Read7BitEncodedLong(MP.IO.IDataStreamAccess)"/>
        [Throws(typeof(IO.IOException), typeof(NotSupportedException), typeof(ObjectDisposedException))]
        public static void Write7BitEncodedLong(this IO.IDataStreamAccess writer, System.Int64 value)
        {
            System.UInt64 num;
            for (num = value.ToUInt64(); num >= 0x7FUL; num >>= 7)
            {
                writer.WriteByte((num | 0x80UL).ToByte());
            }
            writer.WriteByte(num.ToByte());
        }
    }
}