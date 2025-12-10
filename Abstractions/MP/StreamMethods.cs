
using System;
using MP.Annotations.CodeAnalysis;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace MP
{
    /// <summary>
    /// Provides extension methods for fast reading and writing data to streams.
    /// </summary>
    public unsafe static partial class StreamMethods
    {
        // A typed length for temporary buffers.
        private const System.Int32 BUFSIZE = 2048;

        /// <summary>
        /// Provides the default padding string used by the <see cref="WritePadString(System.IO.Stream, string, int)"/> method. <br />
        /// It is publicly exposed if you want to explicitly use this.
        /// </summary>
        public const System.String DefaultPadString = "PAD";

        /// <summary>
        /// Defines a temporary buffer size for operations that must use intermediate buffers.
        /// </summary>
        public const System.Int32 MaxTypicalBufferSize = BUFSIZE;

        /// <summary>
        /// Writes an ASCII-encoded string to the specified stream. <br />
        /// For Unicode values bigger than 255 , the question mark character '?' is written.
        /// </summary>
        /// <param name="stream">The stream to write the specified string.</param>
        /// <param name="str">The string to write to the stream.</param>
        [Throws(typeof(System.IO.IOException), typeof(NotSupportedException), typeof(ObjectDisposedException))]
        public static void WriteASCIIString(this System.IO.Stream stream, [MaybeNull] System.String str)
        {
            if (System.String.IsNullOrEmpty(str)) { return; }
            System.Byte[] data = new System.Byte[str.Length];
            System.Char temp;
            for (System.Int32 I = 0; I < str.Length; I++)
            {
                temp = str[I];
                data[I] = ((temp > 255) ? 63 : temp).ToByte();
            }
            stream.Write(data, 0, data.Length);
            data = null;
        }

        /// <summary>
        /// Writes an UTF16-encoded string with little endianess to the specified stream.
        /// </summary>
        /// <param name="stream">The stream to write the specified string.</param>
        /// <param name="str">The string to write to the stream.</param>
        [Throws(typeof(System.IO.IOException), typeof(NotSupportedException), typeof(ObjectDisposedException))]
        public static void WriteUTF16LEString(this System.IO.Stream stream, [MaybeNull] System.String str)
        {
            if (System.String.IsNullOrEmpty(str)) { 
                return; 
            } else {
                WriteString(stream, str, System.Text.Encoding.Unicode);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static int ComputeStreamBufferSize(long consumed, long total, int buffer_size) => ((consumed + buffer_size) < total) ? buffer_size : (int)(total - consumed);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static uint ComputeStreamBufferSize(long consumed , long total , uint buffer_size) => ((consumed + buffer_size) < total) ? buffer_size : (uint)(total - consumed);

        /// <summary>
        /// Writes a string to the specified stream , under the specified encoding.
        /// </summary>
        /// <param name="stream">The stream to write the specified string.</param>
        /// <param name="str">The string to write.</param>
        /// <param name="enc">The character encoding under which <paramref name="str"/> will be saved.</param>
        /// <exception cref="ArgumentNullException"><paramref name="enc"/> was null.</exception>
        /// <returns>The number of bytes written for saving the string into the data stream.</returns>
        [Throws(typeof(ArgumentNullException), typeof(System.IO.IOException) , typeof(NotSupportedException), typeof(ObjectDisposedException))]
        public static long WriteString(this System.IO.Stream stream , System.String str , System.Text.Encoding enc)
        {
            ArgumentNullException.ThrowIfNull(enc);
            if (System.String.IsNullOrEmpty(str)) { return 0; }

            System.Byte[] temp_1 = null;

            try {

                System.Text.Encoder encoder = enc.GetEncoder();

                temp_1 = System.Buffers.ArrayPool<System.Byte>.Shared.Rent(2048);

                int len = str.Length, chars_consumed = 0, bytes_written;
                long total_bytes = 0;

                bool completed;

                fixed (System.Char* pstring = str)
                {
                    System.Char* mutable = pstring;

                    do {
                        // Process the string buffer, getting it by 2048 byte chunks and repeating if the string is too large to directly fit in 2048 characters.
                        fixed (System.Byte* pdest = temp_1)
                            encoder.Convert(mutable, len, pdest, 2048, len == 0, out chars_consumed, out bytes_written, out completed);

                        mutable += chars_consumed;
                        len -= chars_consumed;

                        if (bytes_written > 0) {
                            stream.Write(temp_1, 0, bytes_written);
                            total_bytes += bytes_written;
                        }
                    } while (!completed);
                }

                return total_bytes;
            } finally {
                if (temp_1 is not null) {
                    System.Buffers.ArrayPool<System.Byte>.Shared.Return(temp_1);
                    temp_1 = null;
                }
            }
        }

        /// <summary>
        /// Reads a string value previously written with the <see cref="WriteString(System.IO.Stream, string, System.Text.Encoding)"/> method.
        /// </summary>
        /// <param name="stream">The data stream to read the specified string from.</param>
        /// <param name="enc">The character encoding under which the string will be read back.</param>
        /// <param name="nbytes">The number of bytes comprising the string data</param>
        /// <returns>The read string.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="enc"/> was <see langword="null"/>.</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="nbytes"/> was negative.</exception>
        [Throws(
            typeof(System.IO.IOException), 
            typeof(ArgumentNullException),
            typeof(NotSupportedException),
            typeof(ObjectDisposedException),
            typeof(ArgumentOutOfRangeException)
        )]
        public static System.String ReadString(this System.IO.Stream stream, System.Text.Encoding enc, long nbytes)
        {
            if (enc is null) { throw new System.ArgumentNullException(nameof(enc)); }
            if (nbytes < 0) { 
                throw new System.ArgumentOutOfRangeException(nameof(nbytes), "Number of bytes cannot be negative!!");
            } else if (nbytes == 0) {
                return System.String.Empty;
            }

            System.Byte[] temp_1 = null;
            System.Char[] temp_2 = null;

            try {
                // Get the decoder to use
                System.Text.Decoder dec = enc.GetDecoder();

                // Allocate temporary processing buffers
                temp_1 = System.Buffers.ArrayPool<System.Byte>.Shared.Rent(2048);
                temp_2 = System.Buffers.ArrayPool<System.Char>.Shared.Rent(2048);

                // OK. Now allocate our string builder
                System.Text.StringBuilder sb = new((nbytes / 4).ToInt32());

                int temp_bytes_consumed = 0, temp_chars_used, proc_bytes_used, proc_byte_index;

                // Assume that the end of stream is not reached yet.
                // This is done so that the loop can enter the first time.
                bool completed , end_of_stream = false;

                // Read bytes to a temporary buffer, process the buffer through the decoder, and append the decoded data to the string builder.
                // Continue doing that until: 
                // -> End of stream is not reached yet
                // -> The number of consumed bytes is less than the expected length in bytes of the string.
                for (long consumed = 0; !end_of_stream && consumed < nbytes; consumed += temp_bytes_consumed)
                {
                    end_of_stream = (temp_bytes_consumed = stream.Read(temp_1, 0, ComputeStreamBufferSize(consumed , nbytes , 2048))) == 0;

                    proc_byte_index = 0;

                    // Process string data
                    // If we reached end of stream, process decoder leftovers
                    do {
                        dec.Convert(temp_1, proc_byte_index, temp_bytes_consumed - proc_byte_index
                            , temp_2, 0, 2048,
                            end_of_stream,
                            out proc_bytes_used,
                            out temp_chars_used,
                            out completed);

                        sb.Append(temp_2, 0, temp_chars_used);

                        proc_byte_index += proc_bytes_used;
                    } while (!completed);
                }

                // Combine all the buffers and return the results as one string.
                return sb.ToString();

            } finally {
                if (temp_1 is not null) {
                    System.Buffers.ArrayPool<System.Byte>.Shared.Return(temp_1);
                    temp_1 = null;
                }
                if (temp_2 is not null) {
                    System.Buffers.ArrayPool<System.Char>.Shared.Return(temp_2);
                    temp_2 = null;
                }
            }

        }

        /// <summary>
        /// Writes a fixed-length string of the specified encoding to the stream.
        /// </summary>
        /// <param name="stream">The stream where the fixed-length string will be written to.</param>
        /// <param name="str">The string to write.</param>
        /// <param name="enc">The character encoding under which <paramref name="str"/> will be saved.</param>
        /// <returns>The length, in bytes, written to the stream, for writing the value contained in <paramref name="str"/>.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="enc"/> was null.</exception>
        [Throws(
            typeof(System.IO.IOException),
            typeof(NotSupportedException),
            typeof(ObjectDisposedException),
            typeof(ArgumentNullException)
        )]
        public static long WriteFixedLengthString(this System.IO.Stream stream, System.String str , System.Text.Encoding enc)
        {
            ArgumentNullException.ThrowIfNull(enc);
            int bc = (str is null) ? 0 : enc.GetByteCount(str);
            Write7BitEncodedInt(stream, bc);
            return (bc == 0) ? 0 : WriteString(stream, str, enc); 
        }

        /// <summary>
        /// Writes characters from a given pad string as many times as it is required by the <paramref name="pads"/> parameter. <br />
        /// If <paramref name="pads"/> has a value greater than the <paramref name="pad"/> parameter length , the string is repeated.
        /// </summary>
        /// <param name="stream">The stream to write the pad string to.</param>
        /// <param name="pad">The pad string to use to write the pads. Can have any content , as long as it is does only use ASCII characters.</param>
        /// <param name="pads">The number of pad bytes to write.</param>
        [Throws(typeof(System.IO.IOException), typeof(NotSupportedException), typeof(ObjectDisposedException))]
        public static void WritePadString(this System.IO.Stream stream , System.String pad, System.Int32 pads)
        {
            // If no pads required , do not throw an exception. Consider it as a valid call and instead
            // do not write anyting on the target stream.
            // For safety , the method will not also write a padding that is very large (e.g. 8000 pads) , 
            // which a padding will never be such large in a real format.
            if (pads < 1 || pads > 512) { return; }
            // Assign a default pad string if the user failed to give a proper one.
            if (System.String.IsNullOrEmpty(pad)) { pad = DefaultPadString; } 
            System.Int32 padidx = 0;
            // Create a padding array which will be written to the final target.
            // We do not care about severe memory allocations since this method
            // will only get some random pads which will be usually less than 512 bytes in total.
            System.Byte[] pds = new System.Byte[pads];
            for (System.Int32 I = 0; I < pads; I++)
            {
                pds[I] = pad[padidx].ToByte();
                padidx++;
                if (padidx >= pad.Length) { padidx = 0; }
            }
            // Write all the generated pads at once.
            stream.Write(pds, 0, pds.Length);
            pds = null;
        }

        /// <summary>
        /// Reads an ASCII-encoded string from the stream. 
        /// </summary>
        /// <param name="stream">The stream to read from.</param>
        /// <param name="length">The length, in bytes , of the string to read.</param>
        /// <returns>The read string contents.</returns>
        [Throws(typeof(System.IO.IOException), typeof(NotSupportedException), typeof(ObjectDisposedException))]
        public static System.String ReadASCIIString(this System.IO.Stream stream , System.Int32 length) => (length < 0) ? null : ReadString(stream, System.Text.Encoding.ASCII, length);

        /// <summary>
        /// Reads an UTF16-encoded string with little endianess from the stream. 
        /// </summary>
        /// <param name="stream">The stream to read from.</param>
        /// <param name="length">The length, in bytes , of the string to be read.</param>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="length"/> was negative.</exception>
        [Throws(typeof(System.IO.IOException), typeof(NotSupportedException), typeof(ObjectDisposedException), typeof(ArgumentOutOfRangeException))]
        public static System.String ReadUTF16LEString(this System.IO.Stream stream , System.Int32 length) => ReadString(stream, System.Text.Encoding.Unicode , length);

        /// <summary>
        /// Reads an UTF16-encoded string with little endianess from the stream. 
        /// </summary>
        /// <param name="stream">The stream to read from.</param>
        /// <param name="length">The length, in bytes , of the string to be read.</param>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="length"/> was negative.</exception>
        [Throws(typeof(System.IO.IOException), typeof(NotSupportedException), typeof(ObjectDisposedException), typeof(ArgumentOutOfRangeException))]
        public static System.String ReadUTF16LEString(this System.IO.Stream stream, System.UInt32 length) => ReadString(stream, System.Text.Encoding.Unicode, length);

        /// <summary>
        /// Reads a fixed-length string from the specified string , with the specified text encoding.
        /// </summary>
        /// <param name="stream">The stream to read the fixed-length string from.</param>
        /// <param name="enc">The encoding under the string was written.</param>
        /// <returns>The decoded string.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="enc"/> is <see langword="null"/>.</exception>
        /// <exception cref="FormatException">The read number of bytes was negative, possibly indicating a corrupt stream.</exception>
        [Throws(typeof(System.IO.IOException), typeof(NotSupportedException), typeof(ObjectDisposedException), typeof(FormatException), typeof(ArgumentNullException))]
        public static System.String ReadFixedLengthString(this System.IO.Stream stream , System.Text.Encoding enc)
        {
            ArgumentNullException.ThrowIfNull(enc);
            // Length of the string in bytes, not chars
            int stringLength = Read7BitEncodedInt(stream);
            // The older method was not very performant, so fall back now to ReadString instead.
            return stringLength switch {
                < 0 => throw new System.FormatException($"Invalid fixed string length: {stringLength}."),
                0 => System.String.Empty,
                _ => ReadString(stream, enc, stringLength),
            };
        }

        /// <summary>
        /// Writes a signed byte to the stream.
        /// </summary>
        /// <param name="stream">The stream to write the signed byte.</param>
        /// <param name="signedbyte">The signed byte to write.</param>
        [Throws(typeof(System.IO.IOException), typeof(NotSupportedException), typeof(ObjectDisposedException))]
        public static void WriteSByte(this System.IO.Stream stream, System.SByte signedbyte) => stream.WriteByte(signedbyte.ToByte());

        /// <summary>
        /// Writes a signed short integer to the stream.
        /// </summary>
        /// <param name="stream">The stream to write the signed integer.</param>
        /// <param name="value">The signed short integer to write.</param>
        [Throws(typeof(System.IO.IOException), typeof(NotSupportedException), typeof(ObjectDisposedException))]
        public static void WriteInt16(this System.IO.Stream stream , System.Int16 value)
        {
            System.Byte[] dt = value.GetBytes();
            stream.Write(dt, 0, dt.Length);
            dt = null;
        }

        /// <summary>
        /// Writes an unsigned short integer to the stream.
        /// </summary>
        /// <param name="stream">The stream to write the unsigned integer.</param>
        /// <param name="value">The unsigned short integer to write.</param>
        [Throws(typeof(System.IO.IOException), typeof(NotSupportedException), typeof(ObjectDisposedException))]
        public static void WriteUInt16(this System.IO.Stream stream, System.UInt16 value)
        {
            System.Byte[] dt = value.GetBytes();
            stream.Write(dt, 0, dt.Length);
            dt = null;
        }

        /// <summary>
        /// Writes an unsigned integer to the stream.
        /// </summary>
        /// <param name="stream">The stream to write the unsigned integer.</param>
        /// <param name="value">The unsigned integer to write.</param>
        [Throws(typeof(System.IO.IOException), typeof(NotSupportedException), typeof(ObjectDisposedException))]
        public static void WriteUInt32(this System.IO.Stream stream, System.UInt32 value)
        {
            System.Byte[] dt = value.GetBytes();
            stream.Write(dt, 0, dt.Length);
            dt = null;
        }

        /// <summary>
        /// Writes a signed integer to the stream.
        /// </summary>
        /// <param name="stream">The stream to write the signed integer.</param>
        /// <param name="value">The signed integer to write.</param>
        [Throws(typeof(System.IO.IOException), typeof(NotSupportedException), typeof(ObjectDisposedException))]
        public static void WriteInt32(this System.IO.Stream stream, System.Int32 value)
        {
            System.Byte[] dt = value.GetBytes();
            stream.Write(dt, 0, dt.Length);
            dt = null;
        }

        /// <summary>
        /// Writes a signed long integer to the stream.
        /// </summary>
        /// <param name="stream">The stream to write the signed integer.</param>
        /// <param name="value">The signed integer to write.</param>
        [Throws(typeof(System.IO.IOException), typeof(NotSupportedException), typeof(ObjectDisposedException))]
        public static void WriteInt64(this System.IO.Stream stream, System.Int64 value)
        {
            System.Byte[] dt = value.GetBytes();
            stream.Write(dt, 0, dt.Length);
            dt = null;
        }

        /// <summary>
        /// Writes an unsigned long integer to the stream.
        /// </summary>
        /// <param name="stream">The stream to write the unsigned integer.</param>
        /// <param name="value">The unsigned long integer to write.</param>
        [Throws(typeof(System.IO.IOException), typeof(NotSupportedException), typeof(ObjectDisposedException))]
        public static void WriteUInt64(this System.IO.Stream stream, System.UInt64 value)
        {
            System.Byte[] dt = value.GetBytes();
            stream.Write(dt, 0, dt.Length);
            dt = null;
        }

        /// <summary>
        /// Reads a signed byte from the stream. 
        /// </summary>
        /// <param name="stream">The stream to read the signed byte.</param>
        /// <returns>The read signed byte.</returns>
        [Throws(typeof(System.IO.IOException), typeof(NotSupportedException), typeof(System.IO.EndOfStreamException), typeof(ObjectDisposedException))]
        public static System.SByte ReadSByte(this System.IO.Stream stream) => ReadLiteralByte(stream).ToSByte();

        /// <summary>
        /// Reads a signed short integer from the stream.
        /// </summary>
        /// <param name="stream">The stream to read from.</param>
        /// <returns>The read signed short integer.</returns>
        public static System.Int16 ReadInt16(this System.IO.Stream stream) => ReadBytes(stream, sizeof(System.Int16)).ToInt16(0);

        /// <summary>
        /// Reads a signed integer from the stream.
        /// </summary>
        /// <param name="stream">The stream to read from.</param>
        /// <returns>The read signed integer.</returns>
        public static System.Int32 ReadInt32(this System.IO.Stream stream) => ReadBytes(stream, sizeof(System.Int32)).ToInt32(0);

        /// <summary>
        /// Reads a signed long integer from the stream.
        /// </summary>
        /// <param name="stream">The stream to read from.</param>
        /// <returns>The read signed long integer.</returns>
        public static System.Int64 ReadInt64(this System.IO.Stream stream) => ReadBytes(stream, sizeof(System.Int64)).ToInt64(0);

        /// <summary>
        /// Reads an unsigned short integer from the stream.
        /// </summary>
        /// <param name="stream">The stream to read from.</param>
        /// <returns>The read unsigned short integer.</returns>
        public static System.UInt16 ReadUInt16(this System.IO.Stream stream) => ReadBytes(stream, sizeof(System.UInt16)).ToUInt16(0);

        /// <summary>
        /// Reads an unsigned integer from the stream.
        /// </summary>
        /// <param name="stream">The stream to read from.</param>
        /// <returns>The read unsigned integer.</returns>
        public static System.UInt32 ReadUInt32(this System.IO.Stream stream) => ReadBytes(stream, sizeof(System.UInt32)).ToUInt32(0);

        /// <summary>
        /// Reads an unsigned integer from the stream.
        /// </summary>
        /// <param name="stream">The stream to read from.</param>
        /// <returns>The read unsigned integer.</returns>
        public static System.UInt64 ReadUInt64(this System.IO.Stream stream) => ReadBytes(stream, sizeof(System.UInt64)).ToUInt64(0);

        /// <summary>
        /// Writes a boolean to the stream.
        /// </summary>
        /// <param name="stream">The stream to write.</param>
        /// <param name="value">The boolean value to write.</param>
        public static void WriteBoolean(this System.IO.Stream stream, System.Boolean value) => stream.WriteByte((value ? 1 : 0).ToByte());

        /// <summary>
        /// Reads a boolean from the stream.
        /// </summary>
        /// <param name="stream">The stream to read from.</param>
        /// <returns>The read boolean value.</returns>
        public static System.Boolean ReadBoolean(this System.IO.Stream stream) => ReadLiteralByte(stream) != 0;

        /// <summary>
        /// Writes a decimal to the stream.
        /// </summary>
        /// <param name="stream">The stream to write the decimal.</param>
        /// <param name="dec">The decimal to write.</param>
        public static void WriteDecimal(this System.IO.Stream stream , System.Decimal dec)
        {
            System.Byte[] dt = dec.GetBytes();
            stream.Write(dt, 0, dt.Length);
            dt = null;
        }

        /// <summary>
        /// Reads a decimal from the stream.
        /// </summary>
        /// <param name="stream">The stream to read the decimal from.</param>
        /// <returns>The read decimal value.</returns>
        public static System.Decimal ReadDecimal(this System.IO.Stream stream) 
            => ReadBytes(stream, sizeof(System.Decimal)).ToDecimal(0);

        /// <summary>
        /// Writes a double-precision floating-point value to the stream.
        /// </summary>
        /// <param name="stream">The stream to write the number to.</param>
        /// <param name="value">The double-precision floating-point value to write.</param>
        public static void WriteDouble(this System.IO.Stream stream , System.Double value)
        {
            System.Byte[] dt = value.GetBytes();
            stream.Write(dt, 0, dt.Length);
            dt = null;
        }

        /// <summary>
        /// Writes a single-precision floating-point value to the stream.
        /// </summary>
        /// <param name="stream">The stream to write the number to.</param>
        /// <param name="value">The single-precision floating-point value to write.</param>
        public static void WriteSingle(this System.IO.Stream stream , System.Single value)
        {
            System.Byte[] dt = value.GetBytes();
            stream.Write(dt , 0 , dt.Length);
            dt = null;
        }

        /// <summary>
        /// Reads a double-precision floating-point value from the stream.
        /// </summary>
        /// <param name="stream">The stream to read.</param>
        /// <returns>The read double-precision floating-point value.</returns>
        public static System.Double ReadDouble(this System.IO.Stream stream)
            => ReadBytes(stream, sizeof(System.Double)).ToDouble(0);

        /// <summary>
        /// Reads a single-precision floating-point value from the stream.
        /// </summary>
        /// <param name="stream">The stream to read.</param>
        /// <returns>The read single-precision floating-point value.</returns>
        public static System.Single ReadSingle(this System.IO.Stream stream)
            => ReadBytes(stream, sizeof(System.Single)).ToSingle(0);

        /// <summary>Reads a byte from the stream.</summary>
        /// <param name="stream">The stream to read from.</param>
        /// <returns>The read byte value.</returns>
        /// <exception cref="System.IO.EndOfStreamException">The stream was ended.</exception>
        public static System.Byte ReadLiteralByte(this System.IO.Stream stream)
        {
            int byte2int = stream.ReadByte();
            return byte2int switch {
                -1 => throw new System.IO.EndOfStreamException("Stream was ended prematurely."),
                _ => byte2int.ToByte(),
            };
        }

        /// <summary>
        /// Writes a structure of type <typeparamref name="T"/> to the stream.
        /// </summary>
        /// <typeparam name="T">The type of the structure to write.</typeparam>
        /// <param name="stream">The stream to write the structure.</param>
        /// <param name="structure">The structure to write.</param>
        /// <seealso cref="ReadStructure{T}(System.IO.Stream)"/>
        public static void WriteStructure<T>(this System.IO.Stream stream, T structure) where T : struct 
        {
            System.Byte[] temp = structure.WriteStructureToNewArray();
            stream.Write(temp , 0 , temp.Length);
            temp = null;
        }

        /// <summary>
        /// Reads a structure of type <typeparamref name="T"/> from the stream.
        /// </summary>
        /// <typeparam name="T">The type of the structure to read.</typeparam>
        /// <param name="stream">The stream to read the structure from.</param>
        /// <returns>The read structure.</returns>
        /// <exception cref="System.IO.EndOfStreamException">The structure could not be read.</exception>
        /// <seealso cref="WriteStructure{T}(System.IO.Stream, T)"/>
        public static T ReadStructure<T>(this System.IO.Stream stream) where T : struct
        {
            int rem = 0, read;
            System.Byte[] temp = new System.Byte[Unsafe.SizeOf<T>()];
            do {
                 rem += (read = stream.Read(temp, rem, temp.Length - rem));
            } while (read > 0 && rem < temp.Length);
            if (rem != temp.Length) { throw new System.IO.EndOfStreamException($"Cannot read the structure of type {typeof(T).FullName}: Expected to read {temp.Length} bytes while read {rem} bytes."); }
            return temp.ReadStructure<T>(0);
        }

        /// <summary>
        /// Reads a 7-bit encoded integer from the stream.
        /// </summary>
        /// <param name="reader">The stream to read from.</param>
        /// <returns>The given 7-bit encoded integer.</returns>
        /// <exception cref="System.FormatException">The number was too long.</exception>
        /// <exception cref="System.IO.EndOfStreamException">The stream ended before the actual reading was finished.</exception>
        /// <seealso cref="Write7BitEncodedInt(System.IO.Stream, int)"/>
        public static System.Int32 Read7BitEncodedInt(this System.IO.Stream reader)
        {
            System.Int32 num = 0 , bits = 0;
            System.Byte b;
            do {
                if (bits == 35) {
                    throw new System.FormatException("Too many bytes of what should have been a 7-bit encoded Int32.");
                }
                b = ReadLiteralByte(reader);
                num |= (b & 0x7F) << bits;
                bits += 7;
            } while ((b & 0x80u) != 0);
            return num;
        }

        /// <summary>
        /// Writes a 7-bit encoded integer to the specified stream.
        /// </summary>
        /// <param name="writer">The stream to write to.</param>
        /// <param name="value">The integer to write.</param>
        /// <seealso cref="Read7BitEncodedInt(System.IO.Stream)"/>
        public static void Write7BitEncodedInt(this System.IO.Stream writer, int value)
        {
            System.UInt32 num;
            for (num = value.ToUInt32(); num >= 128; num >>= 7)
            {
                writer.WriteByte((num | 0x80u).ToByte());
            }
            writer.WriteByte(num.ToByte());
        }

        /// <summary>
        /// Reads <paramref name="count"/> bytes from the stream.
        /// </summary>
        /// <param name="stream">The stream to read the bytes from.</param>
        /// <param name="count">The number of bytes to read.</param>
        /// <returns>The read array.</returns>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="count"/> parameter was negative.</exception>
        [Throws(typeof(System.IO.IOException), typeof(NotSupportedException), typeof(ObjectDisposedException), typeof(ArgumentOutOfRangeException))]
        public static System.Byte[] ReadBytes(this System.IO.Stream stream, System.Int32 count)
        {
            if (count < 0) {
                throw new System.ArgumentOutOfRangeException(nameof(count), "The number of bytes to copy cannot be negative.");
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

        /// <summary>
        /// Reads <paramref name="bytes_to_copy"/> bytes from the stream.
        /// </summary>
        /// <param name="stream">The stream to read the bytes from.</param>
        /// <param name="bytes_to_copy">The number of bytes to read.</param>
        /// <returns>The read array.</returns>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="bytes_to_copy"/> parameter was negative.</exception>
        [Throws(typeof(System.IO.IOException), typeof(NotSupportedException), typeof(ObjectDisposedException), typeof(ArgumentOutOfRangeException))]
        public static System.Byte[] ReadBytes(this System.IO.Stream stream, long bytes_to_copy) => ReadBytes(stream, bytes_to_copy, BUFSIZE);

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
        [Throws(typeof(System.IO.IOException), typeof(NotSupportedException), typeof(ObjectDisposedException), typeof(ArgumentOutOfRangeException))]
        public static System.Byte[] ReadBytes(this System.IO.Stream stream , long bytes_to_copy, int buffer_size)
        {
            if (bytes_to_copy < 0) {
                throw new ArgumentOutOfRangeException(nameof(bytes_to_copy), "The number of bytes to copy cannot be negative.");
            } else if (bytes_to_copy == 0) {
                return Array.Empty<System.Byte>();
            } else if (buffer_size < 1024) {
                throw new ArgumentOutOfRangeException(nameof(buffer_size), "The buffer_size parameter is too small and could degrade performance.");
            }

            System.Byte[] ret = new System.Byte[bytes_to_copy], buffer = null;

            int temp_read_bytes;

            try {

                buffer = System.Buffers.ArrayPool<System.Byte>.Shared.Rent(buffer_size);

                for (long consumed = 0; consumed < bytes_to_copy; consumed += temp_read_bytes)
                {
                    if ((temp_read_bytes = stream.Read(buffer, 0, ComputeStreamBufferSize(consumed, bytes_to_copy, buffer_size))) > 0) {
                        Unsafe.CopyBlockUnaligned(ref ret[consumed], ref buffer[0], temp_read_bytes.ToUInt32());
                    } else {
                        break;
                    }
                }

            } finally {
                if (buffer is not null)
                {
                    System.Buffers.ArrayPool<System.Byte>.Shared.Return(buffer);
                    buffer = null;
                }
            }
            return ret;
        }

        /// <summary>
        /// Writes all the data directly to the stream by writing these sequentially.
        /// </summary>
        /// <param name="stream">The stream to write the bytes to.</param>
        /// <param name="data">The bytes to write to the stream.</param>
        [Throws(typeof(System.IO.IOException), typeof(NotSupportedException), typeof(ObjectDisposedException), typeof(ArgumentNullException))]
        public static void WriteBytes(this System.IO.Stream stream, System.Byte[] data)
        {
            // Abstract: Gets all the bytes defined in the 'data' array and copies them to the stream.
            // To avoid hard limits such as int integer limits, the data copy is instead managed by an temporary buffer
            // dispatching writes with the temporary buffer instead.
            // This would be the same as writing directly the array, however this assures that these implicit limits do not longer pose problems.

            uint transferred; // # of bytes actually transferred to the temporary buffer
            long len = data.LongLength; // The length of the 'data' buffer
            System.Byte[] temp = null;

            try {
                temp = System.Buffers.ArrayPool<System.Byte>.Shared.Rent(2048);

                for (long consumed = 0; consumed < len; consumed += transferred)
                {
                    Unsafe.CopyBlockUnaligned(ref temp[0], ref data[consumed], transferred = ComputeStreamBufferSize(consumed, len, 2048U));

                    stream.Write(temp, 0, transferred.ToInt32());
                }

            } finally {
                if (temp is not null) {
                    System.Buffers.ArrayPool<System.Byte>.Shared.Return(temp);
                    temp = null;
                }
            }
        }

        /// <summary>
        /// Reads a typed array from the stream , specifying the number of <typeparamref name="T"/> elements to read.
        /// </summary>
        /// <typeparam name="T">The type that each element of the new array will have.</typeparam>
        /// <param name="stream">The stream to read the typed array from.</param>
        /// <param name="elementcount">The number of <typeparamref name="T"/> elements to read from the <paramref name="stream"/>.</param>
        /// <returns>The typed array read from the stream.</returns>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="elementcount"/> was negative.</exception>
        /// <seealso cref="ReadTypedSpan{T}(System.IO.Stream, int)"/>
        [Throws(
            typeof(System.IO.IOException), 
            typeof(NotSupportedException), 
            typeof(ObjectDisposedException), 
            typeof(ArgumentOutOfRangeException)
        )]
        public static T[] ReadTypedArray<T>(this System.IO.Stream stream , System.Int32 elementcount)
            where T : unmanaged
        {
            if (elementcount < 0) { throw new System.ArgumentOutOfRangeException(nameof(elementcount), "Number of elements to read must not be negative."); }
            T[] ret = new T[elementcount];
            System.Byte[] dataread = stream.ReadBytes(elementcount * sizeof(T));
            Unsafe.CopyBlockUnaligned(ref Unsafe.As<T, System.Byte>(ref ret[0]), ref dataread[0], dataread.LongLength.ToUInt32());
            return ret;
        }

        /// <summary>
        /// Directly copies over all the bytes contained from the current to another stream. <br />
        /// Both positions will be respectively updated.
        /// </summary>
        /// <param name="input">The source stream.</param>
        /// <param name="output">The target stream.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="output"/> parameter was null.</exception>
        [Throws(
            typeof(System.IO.IOException),
            typeof(ArgumentNullException),
            typeof(NotSupportedException),
            typeof(ObjectDisposedException)
        )]
        public static void DirectCopyToStream(this System.IO.Stream input, System.IO.Stream output)
            => DirectCopyToStream(input, output, BUFSIZE);

        /// <summary>
        /// Directly copies over all the bytes contained from the current to another stream. <br />
        /// Both positions will be respectively updated.
        /// </summary>
        /// <param name="input">The source stream.</param>
        /// <param name="output">The target stream.</param>
        /// <param name="buffersize">The buffer size, in bytes, that the method should allocate. This buffer will be used to copy data from the one stream to the another.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="output"/> parameter was null.</exception>
        /// <exception cref="ArgumentOutOfRangeException">The <paramref name="buffersize"/> parameter was less than 1024 bytes.</exception>
        [Throws(
            typeof(System.IO.IOException),
            typeof(ArgumentNullException),
            typeof(NotSupportedException),
            typeof(ObjectDisposedException),
            typeof(ArgumentOutOfRangeException)
        )]
        public static void DirectCopyToStream(this System.IO.Stream input , System.IO.Stream output , System.Int32 buffersize)
        {
            ArgumentNullException.ThrowIfNull(output);
            if (buffersize < 1024) { throw new System.ArgumentOutOfRangeException(nameof(buffersize) , "The buffersize parameter is too small and could degrade performance."); }
            System.Byte[] buffer = null;
            System.Int32 readin;
            try {

                buffer = System.Buffers.ArrayPool<System.Byte>.Shared.Rent(buffersize);

                while ((readin = input.Read(buffer, 0, buffer.Length)) > 0)
                {
                    output.Write(buffer, 0, readin);
                }

            } finally {
                if (buffer is not null)
                {
                    System.Buffers.ArrayPool<System.Byte>.Shared.Return(buffer);
                    buffer = null;
                }
            }
        }

        /// <summary>
        /// Directly copies the specified number of bytes from the current stream to the destination stream. <br />
        /// Both positions will be respectively updated by <paramref name="bytes_copy"/>.
        /// </summary>
        /// <param name="input">The source stream.</param>
        /// <param name="output">The target stream.</param>
        /// <param name="bytes_copy">The exact number of bytes to copy from this stream to <paramref name="output"/>.</param>
        /// <param name="buffer_size">The buffer size, in bytes, that the method should allocate. This buffer will be used to copy data from the one stream to the another.</param>
        /// <exception cref="System.ArgumentNullException">The <paramref name="output"/> parameter was null.</exception>
        /// <exception cref="System.ArgumentOutOfRangeException">The <paramref name="buffer_size"/> parameter was less than 1024 bytes, -or- the <paramref name="bytes_copy"/> parameter was negative.</exception>
        [Throws(
            typeof(System.IO.IOException),
            typeof(ArgumentNullException),
            typeof(NotSupportedException),
            typeof(ObjectDisposedException),
            typeof(ArgumentOutOfRangeException)
        )]
        public static void CopySpecificToStream(this System.IO.Stream input, System.IO.Stream output, System.Int64 bytes_copy, System.Int32 buffer_size)
        {
            ArgumentNullException.ThrowIfNull(output);
            if (bytes_copy < 0) { throw new System.ArgumentOutOfRangeException(nameof(bytes_copy), "The number of bytes to copy cannot be negative."); }
            if (buffer_size < 1024) { throw new System.ArgumentOutOfRangeException(nameof(buffer_size), "The buffer_size parameter is too small and could degrade performance."); }
            System.Byte[] buffer = null;
            int bytes_copied;
            try {
                buffer = System.Buffers.ArrayPool<System.Byte>.Shared.Rent(buffer_size);

                for (long consumed = 0; consumed < bytes_copy; consumed += bytes_copied)
                {
                    if ((bytes_copied = input.Read(buffer, 0, ComputeStreamBufferSize(consumed, bytes_copy, buffer_size))) > 0) {
                        output.Write(buffer, 0, bytes_copied);
                    } else {
                        break;
                    }
                }

            } finally {
                if (buffer is not null) {
                    System.Buffers.ArrayPool<System.Byte>.Shared.Return(buffer);
                    buffer = null;
                }
            }
        }

        /// <summary>
        /// Directly copies the specified number of bytes from the current stream to the destination stream. <br />
        /// Both positions will be respectively updated by <paramref name="bytes_copy"/>.
        /// </summary>
        /// <param name="input">The source stream.</param>
        /// <param name="output">The target stream.</param>
        /// <param name="bytes_copy">The exact number of bytes to copy from this stream to <paramref name="output"/>.</param>
        /// <exception cref="System.ArgumentNullException">The <paramref name="output"/> parameter was null.</exception>
        /// <exception cref="System.ArgumentOutOfRangeException">The <paramref name="bytes_copy"/> parameter was negative.</exception>
        [Throws(
            typeof(System.IO.IOException),
            typeof(ArgumentNullException),
            typeof(NotSupportedException),
            typeof(ObjectDisposedException),
            typeof(ArgumentOutOfRangeException)
        )]
        public static void CopySpecificToStream(this System.IO.Stream input, System.IO.Stream output, System.Int64 bytes_copy)
            => CopySpecificToStream(input , output, bytes_copy, BUFSIZE);
    }
}