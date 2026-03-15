
using System;
using MP.IO.Buffers;
using MP.Annotations.CodeAnalysis;
using System.Diagnostics.CodeAnalysis;

namespace MP
{
    unsafe partial class StreamMethods
    {
        /// <summary>
        /// Provides the default padding string used by the <see cref="WritePadString(MP.IO.IDataStreamAccess, string, int)"/> method. <br />
        /// It is publicly exposed if you want to explicitly use this.
        /// </summary>
        public const System.String DefaultPadString = "PAD";

        /// <summary>
        /// Writes an ASCII-encoded string to the specified stream. <br />
        /// For Unicode values bigger than 255 , the question mark character '?' is written.
        /// </summary>
        /// <param name="stream">The stream to write the specified string.</param>
        /// <param name="str">The string to write to the stream.</param>
        [Throws(typeof(System.IO.IOException), typeof(NotSupportedException), typeof(ObjectDisposedException))]
        public static void WriteASCIIString(this MP.IO.IDataStreamAccess stream, [AllowNull] System.String str)
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
        public static void WriteUTF16LEString(this MP.IO.IDataStreamAccess stream, [AllowNull] System.String str)
        {
            if (System.String.IsNullOrEmpty(str)) {
                return;
            } else {
                WriteString(stream, str, System.Text.Encoding.Unicode);
            }
        }

        /// <summary>
        /// Writes a string to the specified stream , under the specified encoding.
        /// </summary>
        /// <param name="stream">The stream to write the specified string.</param>
        /// <param name="str">The string to write.</param>
        /// <param name="enc">The character encoding under which <paramref name="str"/> will be saved.</param>
        /// <exception cref="ArgumentNullException"><paramref name="enc"/> was null.</exception>
        /// <returns>The number of bytes written for saving the string into the data stream.</returns>
        [Throws(typeof(ArgumentNullException), typeof(System.IO.IOException), typeof(NotSupportedException), typeof(ObjectDisposedException))]
        public static long WriteString(this MP.IO.IDataStreamAccess stream, System.String str, System.Text.Encoding enc)
        {
            ArgumentNullException.ThrowIfNull(enc);
            if (System.String.IsNullOrEmpty(str)) { return 0; }

            ArrayPoolBufferAcquireContext<System.Byte> temp_1 = System.Buffers.ArrayPool<System.Byte>.Shared.RentByContext(2048);

            try {

                System.Text.Encoder encoder = enc.GetEncoder();

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

                        if (bytes_written > 0)
                        {
                            stream.Write(temp_1, 0, bytes_written);
                            total_bytes += bytes_written;
                        }
                    } while (!completed);
                }

                return total_bytes;
            } finally {
                temp_1.Dispose();
            }
        }

        /// <summary>
        /// Reads a string value previously written with the <see cref="WriteString(MP.IO.IDataStreamAccess, string, System.Text.Encoding)"/> method.
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
        public static System.String ReadString(this MP.IO.IDataStreamAccess stream, System.Text.Encoding enc, long nbytes) => ReadStringAsBuilder(stream , enc , nbytes).ToString();

        /// <summary>
        /// Reads a string value previously written with the <see cref="WriteString(MP.IO.IDataStreamAccess, string, System.Text.Encoding)"/> method. <br />
        /// The string value is read as a string builder.
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
        public static System.Text.StringBuilder ReadStringAsBuilder(this MP.IO.IDataStreamAccess stream, System.Text.Encoding enc, long nbytes)
        {
            ArgumentNullException.ThrowIfNull(enc);
            if (nbytes < 0) {
                throw new ArgumentOutOfRangeException(nameof(nbytes), "Number of bytes cannot be negative!!");
            } else if (nbytes == 0) {
                return new();
            }

            ArrayPoolBufferAcquireContext<System.Byte> temp_1 = default;
            ArrayPoolBufferAcquireContext<System.Char> temp_2 = default;

            try {
                // Get the decoder to use
                System.Text.Decoder dec = enc.GetDecoder();

                // Allocate temporary processing buffers
                temp_1 = System.Buffers.ArrayPool<System.Byte>.Shared.RentByContext(2048);
                temp_2 = System.Buffers.ArrayPool<System.Char>.Shared.RentByContext(2048);

                // OK. Now allocate our string builder
                System.Text.StringBuilder sb = new((nbytes / 4).ToInt32());

                int temp_bytes_consumed = 0, temp_chars_used, proc_bytes_used, proc_byte_index;

                // Assume that the end of stream is not reached yet.
                // This is done so that the loop can enter the first time.
                bool completed, end_of_stream = false;

                // Read bytes to a temporary buffer, process the buffer through the decoder, and append the decoded data to the string builder.
                // Continue doing that until: 
                // -> End of stream is not reached yet
                // -> The number of consumed bytes is less than the expected length in bytes of the string.
                for (long consumed = 0; !end_of_stream && consumed < nbytes; consumed += temp_bytes_consumed)
                {
                    end_of_stream = (temp_bytes_consumed = stream.Read(temp_1, 0, ComputeStreamBufferSize(consumed, nbytes, 2048))) < 0;

                    proc_byte_index = 0;

                    // temp_bytes_consumed will be -1 unless we do the below
                    if (end_of_stream) { temp_bytes_consumed = 0; }

                    // Process string data
                    // If we reached end of stream, process decoder leftovers
                    do
                    {
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

                // Return our string builder.
                return sb;
            } finally {
                temp_1.Dispose();
                temp_2.Dispose();
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
        public static long WriteFixedLengthString(this MP.IO.IDataStreamAccess stream, System.String str, System.Text.Encoding enc)
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
        public static void WritePadString(this MP.IO.IDataStreamAccess stream, System.String pad, System.Int32 pads)
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
        public static System.String ReadASCIIString(this MP.IO.IDataStreamAccess stream, System.Int32 length) => (length < 0) ? null : ReadString(stream, System.Text.Encoding.ASCII, length);

        /// <summary>
        /// Reads an UTF16-encoded string with little endianess from the stream. 
        /// </summary>
        /// <param name="stream">The stream to read from.</param>
        /// <param name="length">The length, in bytes , of the string to be read.</param>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="length"/> was negative.</exception>
        [Throws(typeof(System.IO.IOException), typeof(NotSupportedException), typeof(ObjectDisposedException), typeof(ArgumentOutOfRangeException))]
        public static System.String ReadUTF16LEString(this MP.IO.IDataStreamAccess stream, System.Int32 length) => ReadString(stream, System.Text.Encoding.Unicode, length);

        /// <summary>
        /// Reads an UTF16-encoded string with little endianess from the stream. 
        /// </summary>
        /// <param name="stream">The stream to read from.</param>
        /// <param name="length">The length, in bytes , of the string to be read.</param>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="length"/> was negative.</exception>
        [Throws(typeof(System.IO.IOException), typeof(NotSupportedException), typeof(ObjectDisposedException), typeof(ArgumentOutOfRangeException))]
        public static System.String ReadUTF16LEString(this MP.IO.IDataStreamAccess stream, System.UInt32 length) => ReadString(stream, System.Text.Encoding.Unicode, length);

        /// <summary>
        /// Reads a fixed-length string from the specified string , with the specified text encoding.
        /// </summary>
        /// <param name="stream">The stream to read the fixed-length string from.</param>
        /// <param name="enc">The encoding under the string was written.</param>
        /// <returns>The decoded string.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="enc"/> is <see langword="null"/>.</exception>
        /// <exception cref="FormatException">The read number of bytes was negative, possibly indicating a corrupt stream.</exception>
        [Throws(typeof(System.IO.IOException), typeof(NotSupportedException), typeof(ObjectDisposedException), typeof(FormatException), typeof(ArgumentNullException))]
        public static System.String ReadFixedLengthString(this MP.IO.IDataStreamAccess stream, System.Text.Encoding enc)
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
    }
}