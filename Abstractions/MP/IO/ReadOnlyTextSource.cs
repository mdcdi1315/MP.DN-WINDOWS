
using System;
using MP.Utilities;
using System.Text;
using MP.IO.Buffers;
using System.Threading;
using MP.Annotations.CodeAnalysis;

namespace MP.IO
{
    /// <summary>
    /// Provides a defualt implementation of the <see cref="ITextSource"/> interface 
    /// by wrapping a <see cref="IDataStreamAccess"/> object. <br />
    /// This class can only read from the data stream, even if the data stream is opened in writing mode.
    /// </summary>
    public sealed class ReadOnlyTextSource : ITextSource, IStreamOwnerBase
    {
        private Decoder decoder;
        private bool stream_owner;
        private readonly Encoding encoding;
        private IDataStreamAccess data_stream;
        private int byte_buffer_filled, char_buffer_filled;
        private ArrayPoolBufferAcquireContext<System.Byte> byte_buffer;
        private ArrayPoolBufferAcquireContext<System.Char> char_buffer;

        /// <summary>
        /// Initializes a new instance of the <see cref="ReadOnlyTextSource"/> class,
        /// specifying the data stream that contains the actual text to be subsequently processed.
        /// </summary>
        /// <param name="stream">The <see cref="IDataStreamAccess"/> object to process.</param>
        /// <exception cref="InvalidDataStreamException">The data stream is not readable.</exception>
        /// <exception cref="InvalidDataStreamFormatException">The data stream is possibly malformed and it is not a text file.</exception>
        public ReadOnlyTextSource(IDataStreamAccess stream)
        {
            ArgumentNullException.ThrowIfNull(stream);
            InvalidDataStreamException.ThrowIfUnreadable(stream);
            int used = 0;
            ReadOnlySpan<System.Byte> b = (data_stream = stream).ReadBytes(8L);
            Encoding d = null;
            foreach (EncodingInfo i in Encoding.GetEncodings())
            {
                d = i.GetEncoding();
                if (b.StartsWith(d.Preamble))
                {
                    used = d.Preamble.Length;
                    break;
                }
            }
            if ((encoding = d) is null) {
                throw new InvalidDataStreamFormatException("Can't detect the encoding to use!");
            } else {
                Init();
                for (int I = used; I < 8; I++, byte_buffer_filled++) { byte_buffer.Buffer[byte_buffer_filled] = b[I]; }
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ReadOnlyTextSource"/> class,
        /// specifying the data stream that contains the actual text to be subsequently processed. <br />
        /// This constructor does instead directly specify the <see cref="System.Text.Encoding"/> object to use, and
        /// as such it avoids the overhead of looking up the preamble.
        /// </summary>
        /// <param name="stream">The <see cref="IDataStreamAccess"/> object to process.</param>
        /// <param name="encoding">The <see cref="System.Text.Encoding"/> to use for decoding characters.</param>
        /// <exception cref="InvalidDataStreamException">The data stream is not readable.</exception>
        /// <exception cref="InvalidDataStreamFormatException">The data stream is possibly malformed and it is not a text file.</exception>
        public ReadOnlyTextSource(IDataStreamAccess stream, Encoding encoding)
        {
            ArgumentNullException.ThrowIfNull(stream);
            ArgumentNullException.ThrowIfNull(encoding);
            InvalidDataStreamException.ThrowIfUnreadable(stream);
            data_stream = stream;
            this.encoding = encoding;
            Init();
        }

        private void Init()
        {
            stream_owner = false;
            byte_buffer_filled = 0;
            char_buffer_filled = 0;
            decoder = encoding.GetDecoder();
            byte_buffer = System.Buffers.ArrayPool<System.Byte>.Shared.RentByContext(2048);
            try {
                // The char buffer length is adjusted based on how many bytes can be decoded given the byte buffer length.
                char_buffer = System.Buffers.ArrayPool<System.Char>.Shared.RentByContext(encoding.GetMaxCharCount(byte_buffer.ActualLength));
            } catch {
                byte_buffer.Dispose();
                throw;
            }
        }

        /// <inheritdoc />
        public Encoding Encoding => encoding;

        /// <inheritdoc />
        public DataStreamMode Mode => DataStreamMode.Read;

        /// <summary>
        /// Gets a value whether this <see cref="ReadOnlyTextSource"/> owns the contained <see cref="IDataStreamAccess"/> object.
        /// </summary>
        public bool IsStreamOwner
        { 
            get => stream_owner; 
            set => stream_owner = value; 
        }

        /// <inheritdoc />
        public int Read(Span<System.Char> buffer)
        {
            ObjectDisposedException.ThrowIf(byte_buffer is null, this);
            bool end_of_stream = false; // Indicates end of the underlying data stream.
            int placedfromtemp = ReadFromTempCharBuffer(buffer);
            while (end_of_stream == false && placedfromtemp < buffer.Length)
            {
                // We need to fetch a buffer, or even buffers from the underlying data stream.
                Span<System.Byte> src = byte_buffer.Buffer;
                int read_bytes = data_stream.Read(src.Slice(byte_buffer_filled));
                if (read_bytes > -1) { byte_buffer_filled += read_bytes; } else { end_of_stream = true; }
                if (byte_buffer_filled > 1) {
                    // Convert now the byte buffer data and place them to our char buffer.
                    bool completed;
                    int total_bytes_used = 0;
                    src = src.Slice(0, byte_buffer_filled);
                    Span<System.Char> dest = char_buffer.Buffer;
                    do {
                        decoder.Convert(src, dest, end_of_stream, out int bu, out int cu, out completed);
                        total_bytes_used += bu;
                        char_buffer_filled += cu;
                        src = src.Slice(bu);
                        dest = dest.Slice(cu);
                    } while (!completed);
                    byte_buffer_filled -= total_bytes_used;
                    if (byte_buffer_filled > 0) {
                        Array.Copy(byte_buffer.Buffer, total_bytes_used, byte_buffer.Buffer, 0, byte_buffer_filled);
                    }
                    // If we can now, place read characters into the provided buffer.
                    placedfromtemp += ReadFromTempCharBuffer(buffer.Slice(placedfromtemp));
                }
            }
            // NOTE: If we have managed to place some data to the 'buffer' parameter but we are at the end of the data stream, we need to return those read characters.
            // In the next call, the below will validly return -1 to indicate end of text source data.
            return (end_of_stream && placedfromtemp == 0) ? (-1) : placedfromtemp;
        }

        private int ReadFromTempCharBuffer(Span<System.Char> buffer)
        {
            int cpy = buffer.Length > char_buffer_filled ? char_buffer_filled : buffer.Length;
            if (cpy == 0) { 
                return 0; 
            } else {
                ((Span<System.Char>)char_buffer.Buffer).Slice(0, cpy).CopyTo(buffer);
                char_buffer_filled -= cpy;
                if (char_buffer_filled > 0) {
                    Array.Copy(char_buffer.Buffer, cpy, char_buffer.Buffer, 0, char_buffer_filled);
                }
                return cpy;
            }
        }

        /// <inheritdoc />
        public void Write(ReadOnlySpan<char> buffer) 
        {
            ObjectDisposedException.ThrowIf(byte_buffer is null, this);
            throw new NotSupportedException("Read-only text source"); 
        }

        /// <summary>
        /// Resets the state of this object, such as clearing internal buffers and other stuff
        /// </summary>
        /// <exception cref="ObjectDisposedException">This read-only text source has been disposed of.</exception>
        [Throws(typeof(ObjectDisposedException))]
        public void Reset()
        {
            ObjectDisposedException.ThrowIf(byte_buffer is null, this);
            decoder.Reset();
            byte_buffer_filled = 0;
            char_buffer_filled = 0;
        }

        /// <summary>
        /// Destroys this <see cref="ReadOnlyTextSource"/> object.
        /// </summary>
        public void Dispose()
        {
            Monitor.Enter(this);
            try {
                if (byte_buffer is not null)
                {
                    byte_buffer.Dispose();
                    byte_buffer = null;
                }
                if (char_buffer is not null)
                {
                    char_buffer.Dispose();
                    char_buffer = null;
                }
                if (stream_owner && data_stream is not null)
                {
                    data_stream.Dispose();
                    data_stream = null;
                }
                decoder?.Reset();
                decoder = null;
            } finally {
                Monitor.Exit(this);
            }
        }
    }
}
