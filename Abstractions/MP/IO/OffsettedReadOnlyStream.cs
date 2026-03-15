
using System;

namespace MP.IO
{
    /// <summary>
    /// <see cref="WrapperStream"/> derivative. <br />
    /// It allows to reinterpret a data stream, that is, make any consumer to convince it that this is another data stream. <br />
    /// This is helpful for container formats and for cases that you need to pass smaller, in size, streams around.
    /// </summary>
    public class OffsettedReadOnlyStream : OffsettedStream
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OffsettedReadOnlyStream"/> class,
        /// specifying the wrapping stream, as well as the size, in bytes, of the stream.
        /// </summary>
        /// <param name="wrapped">The <see cref="DataStream"/> to be wrapped.</param>
        /// <param name="size">The length of the current stream.</param>
        public OffsettedReadOnlyStream(DataStream wrapped, long size) : base(wrapped)
        {
            InvalidDataStreamException.ThrowIfUnreadable(wrapped);
            OffsettedPosition = 0L;
            OffsettedLength = size;
        }

        /// <inheritdoc />
        public override DataStreamMode Mode => DataStreamMode.Seek | DataStreamMode.Read;

        /// <inheritdoc />
        public override int Read(byte[] buffer, int offset, int count)
        {
            int bytes_to_read = (OffsettedPosition + count > OffsettedLength) ? (OffsettedLength - OffsettedPosition).ToInt32() : count;
            if (bytes_to_read == 0) { 
                return 0; 
            } else {
                int read = base.Read(buffer, offset, bytes_to_read);
                OffsettedPosition += read;
                return read;
            }
        }

        /// <inheritdoc />
        public override int Read(Span<byte> buffer)
        {
            int read;
            if (OffsettedPosition + buffer.Length > OffsettedLength) {
                int l = (OffsettedLength - OffsettedPosition).ToInt32();
                read = (l == 0) ? 0 : base.Read(buffer.Slice(0, l));
            } else {
                read = base.Read(buffer);
            }
            OffsettedPosition += read;
            return read;
        }

        /// <inheritdoc />
        public override void Write(byte[] buffer, int offset, int count) => throw new NotSupportedException("Read-only data stream");

        /// <inheritdoc />
        public override void Write(ReadOnlySpan<byte> buffer) => throw new NotSupportedException("Read-only data stream");

        /// <inheritdoc />
        public override void WriteByte(byte value) => throw new NotSupportedException("Read-only data stream");
    }
}