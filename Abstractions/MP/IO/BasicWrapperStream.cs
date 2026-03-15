
using System;

namespace MP.IO
{
    /// <summary>
    /// Defines a wrapper stream that wraps the specified stream. <br />
    /// This is mainly to avoid calling <see cref="DataStream.Dispose()"/> and related methods on the wrapped stream,
    /// due to how an API is developed.
    /// </summary>
    public class BasicWrapperStream : DataStream
    {
        /// <summary>
        /// Contains the wrapped stream object.
        /// </summary>
        protected IDataStreamAccess wrapped;

        /// <summary>
        /// Creates a new instance of the <see cref="BasicWrapperStream"/> class, specifying the stream to wrap.
        /// </summary>
        /// <param name="wrap">The <see cref="IDataStreamAccess"/> object to wrap.</param>
        public BasicWrapperStream(IDataStreamAccess wrap)
        {
            ArgumentNullException.ThrowIfNull(wrap);
            wrapped = wrap;
        }

        /// <inheritdoc />
        public override DataStreamMode Mode => wrapped.Mode;

        /// <inheritdoc />
        public override int Read(byte[] buffer, int offset, int count) => wrapped.Read(buffer, offset, count);

        /// <inheritdoc />
        public override void Write(byte[] buffer, int offset, int count) => wrapped.Write(buffer, offset, count);

        /// <inheritdoc />
        public override int Read(Span<byte> buffer) => wrapped.Read(buffer);

        /// <inheritdoc />
        public override void Write(ReadOnlySpan<byte> buffer) => wrapped.Write(buffer);

        /// <inheritdoc />
        public override short ReadByte() => wrapped.ReadByte();

        /// <inheritdoc />
        public override void WriteByte(byte value) => wrapped.WriteByte(value);

        /// <summary>
        /// Disposes the current stream by unreferencing it.
        /// </summary>
        /// <param name="disposing">A value whether a full disposal is performed.</param>
        protected override void Dispose(bool disposing) { if (disposing) { wrapped = null; } }

        /// <inheritdoc />
        public override string ToString() => wrapped?.ToString() ?? "Disposed";
    }
}