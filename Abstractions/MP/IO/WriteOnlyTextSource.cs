
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
    /// This class can only write to the data stream, even if the data stream is opened in reading mode.
    /// </summary>
    public sealed class WriteOnlyTextSource : ITextSource, IStreamOwnerBase
    {
        private Encoder encoder;
        private bool stream_owner;
        private readonly Encoding encoding;
        private IDataStreamAccess data_sink;
        private ArrayPoolBufferAcquireContext<System.Byte> buffer;

        /// <summary>
        /// Initializes a new instance of the <see cref="WriteOnlyTextSource"/> class, specifying the data stream to write characters to,
        /// the <see cref="System.Text.Encoding"/> to use for encoding characters, and a value whether to append the encoding's 
        /// preamble to the beginning of the data stream.
        /// </summary>
        /// <param name="data_stream">The <see cref="IDataStreamAccess"/> object to be wrapped.</param>
        /// <param name="encoding">The desired encoding.</param>
        /// <param name="append_preamble">A value whether to append the preamble to the beginning of the data stream.</param>
        /// <exception cref="ArgumentNullException"><paramref name="data_stream"/> and/or <paramref name="encoding"/> are <see langword="null"/>.</exception>
        /// <exception cref="InvalidDataStreamException"><paramref name="data_stream"/> is unwritable.</exception>
        [Throws(typeof(ArgumentNullException), typeof(InvalidDataStreamException))]
        public WriteOnlyTextSource(IDataStreamAccess data_stream, Encoding encoding, bool append_preamble)
        {
            ArgumentNullException.ThrowIfNull(this.encoding = encoding, nameof(encoding));
            ArgumentNullException.ThrowIfNull(data_sink = data_stream, nameof(data_stream));
            InvalidDataStreamException.ThrowIfUnwritable(data_sink, nameof(data_stream));
            stream_owner = false;
            encoder = encoding.GetEncoder();
            if (append_preamble) { data_stream.Write(encoding.Preamble); }
            buffer = System.Buffers.ArrayPool<System.Byte>.Shared.RentByContext(2048);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="WriteOnlyTextSource"/> class, specifying the data stream to write characters to, as well as the <see cref="System.Text.Encoding"/> to use for encoding characters.
        /// </summary>
        /// <param name="data_stream">The <see cref="IDataStreamAccess"/> object to be wrapped.</param>
        /// <param name="encoding">The desired encoding.</param>
        /// <exception cref="ArgumentNullException"><paramref name="data_stream"/> and/or <paramref name="encoding"/> are <see langword="null"/>.</exception>
        /// <exception cref="InvalidDataStreamException"><paramref name="data_stream"/> is unwritable.</exception>
        [Throws(typeof(ArgumentNullException), typeof(InvalidDataStreamException))]
        public WriteOnlyTextSource(IDataStreamAccess data_stream, Encoding encoding) : this(data_stream, encoding, false) { }

        /// <inheritdoc />
        public Encoding Encoding => encoding;

        /// <inheritdoc />
        public DataStreamMode Mode => DataStreamMode.Write;

        /// <inheritdoc />
        public bool IsStreamOwner 
        { 
            get => stream_owner; 
            set => stream_owner = value;
        }

        /// <inheritdoc />
        public int Read(Span<char> buffer) 
        {
            ObjectDisposedException.ThrowIf(this.buffer is null, this);
            throw new NotSupportedException("Write-only text source"); 
        }

        /// <inheritdoc />
        public void Write(ReadOnlySpan<char> buffer)
        {
            ObjectDisposedException.ThrowIf(this.buffer is null, this);
            if (buffer.Length == 0) { return; }

            Span<System.Byte> b = this.buffer.Buffer;

            bool completed;

            do {
                encoder.Convert(buffer, b, false, out int cu, out int bu, out completed);

                data_sink.Write(b.Slice(0, bu));

                buffer = buffer.Slice(cu);
            } while (!completed);
        }

        private void FlushFinalData()
        {
            Span<System.Byte> b = buffer.Buffer;

            bool completed;

            do {
                encoder.Convert(ReadOnlySpan<System.Char>.Empty, b, true, out _, out int bu, out completed);
                data_sink.Write(b.Slice(0, bu));
            } while (!completed);
        }

        /// <summary>
        /// Disposes this <see cref="WriteOnlyTextSource"/> class instance.
        /// </summary>
        public void Dispose()
        {
            Monitor.Enter(this);
            try {
                if (buffer is not null)
                {
                    FlushFinalData();
                    buffer.Dispose();
                    buffer = null;
                }
                if (encoder is not null)
                {
                    encoder.Reset();
                    encoder = null;
                }
                if (stream_owner && data_sink is not null)
                {
                    data_sink.Dispose();
                    data_sink = null;
                }
            } finally {
                Monitor.Exit(this);
            }
        }
    }
}