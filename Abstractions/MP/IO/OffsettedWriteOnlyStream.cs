
using System;

namespace MP.IO
{
    /// <summary>
    /// <see cref="WrapperStream"/> derivative. <br />
    /// It allows to reinterpret a data stream, that is, make any consumer to convince it that this is another data stream. <br />
    /// This is helpful for container formats and for cases that you need to pass smaller, in size, streams around.
    /// </summary>
    public class OffsettedWriteOnlyStream : OffsettedStream
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OffsettedWriteOnlyStream"/> class, specifying the wrapping stream.
        /// </summary>
        /// <param name="wrapped">The <see cref="DataStream"/> to be wrapped.</param>
        /// <exception cref="InvalidDataStreamException"><paramref name="wrapped"/> is not writable.</exception>
        public OffsettedWriteOnlyStream(DataStream wrapped) : base(wrapped)
        {
            InvalidDataStreamException.ThrowIfUnwritable(wrapped);
            OffsettedPosition = 0L;
            OffsettedLength = 0L;
        }

        /// <inheritdoc />
        public override DataStreamMode Mode => DataStreamMode.Seek | DataStreamMode.Write;

        /// <inheritdoc />
        public override int Read(byte[] buffer, int offset, int count) => throw new NotSupportedException("Read-only data stream");

        /// <inheritdoc />
        public override int Read(Span<byte> buffer) => throw new NotSupportedException("Read-only data stream");

        /// <inheritdoc />
        public override short ReadByte() => throw new NotSupportedException("Read-only data stream");

        private void UpdatePositionValuesHelper(int by)
        {
            OffsettedPosition += by;
            if (OffsettedPosition > OffsettedLength) { OffsettedLength = OffsettedPosition; }
        }

        /// <inheritdoc />
        public override void WriteByte(byte value)
        {
            base.WriteByte(value);
            UpdatePositionValuesHelper(1);
        }

        /// <inheritdoc />
        public override void Write(byte[] buffer, int offset, int count)
        {
            base.Write(buffer, offset, count);
            UpdatePositionValuesHelper(count);
        }

        /// <inheritdoc />
        public override void Write(ReadOnlySpan<byte> buffer)
        {
            base.Write(buffer);
            UpdatePositionValuesHelper(buffer.Length);
        }
    }
}