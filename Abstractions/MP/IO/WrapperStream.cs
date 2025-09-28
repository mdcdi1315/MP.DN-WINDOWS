


using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace MP.IO
{
    /// <summary>
    /// Defines a wrapper stream that wraps the specified stream. <br />
    /// This is mainly to avoid calling <see cref="Stream.Dispose()"/> and related methods on the wrapped stream,
    /// due to how an API is developed.
    /// </summary>
    public sealed class WrapperStream : Stream
    {
        private Stream wrapped;

        /// <summary>
        /// Creates a new instance of the <see cref="WrapperStream"/> class, specifying the stream to wrap.
        /// </summary>
        /// <param name="wrap">The <see cref="Stream"/> object to wrap.</param>
        public WrapperStream(Stream wrap)
        {
            ArgumentNullException.ThrowIfNull(wrap);
            wrapped = wrap;
        }

        /// <inheritdoc />
        public override bool CanRead => wrapped.CanRead;

        /// <inheritdoc />
        public override bool CanSeek => wrapped.CanSeek;

        /// <inheritdoc />
        public override bool CanWrite => wrapped.CanWrite;

        /// <inheritdoc />
        public override long Length => wrapped.Length;

        /// <inheritdoc />
        public override long Position { get => wrapped.Position; set => wrapped.Position = value; }

        /// <inheritdoc />
        public override bool CanTimeout => wrapped.CanTimeout;

        /// <inheritdoc />
        public override int ReadTimeout { get => wrapped.ReadTimeout; set => wrapped.ReadTimeout = value; }

        /// <inheritdoc />
        public override int WriteTimeout { get => wrapped.WriteTimeout; set => wrapped.WriteTimeout = value; }

        /// <inheritdoc />
        public override void Flush() => wrapped.Flush();

        /// <inheritdoc />
        public override int Read(byte[] buffer, int offset, int count) => wrapped.Read(buffer, offset, count);

        /// <inheritdoc />
        public override long Seek(long offset, SeekOrigin origin) => wrapped.Seek(offset, origin);

        /// <inheritdoc />
        public override void SetLength(long value) => wrapped.SetLength(value);

        /// <inheritdoc />
        public override void Write(byte[] buffer, int offset, int count) => wrapped.Write(buffer, offset, count);

        /// <inheritdoc />
        public override IAsyncResult BeginRead(byte[] buffer, int offset, int count, AsyncCallback callback, object state) => wrapped.BeginRead(buffer, offset, count, callback, state);

        /// <inheritdoc />
        public override IAsyncResult BeginWrite(byte[] buffer, int offset, int count, AsyncCallback callback, object state) => wrapped.BeginWrite(buffer, offset, count, callback, state);

        /// <inheritdoc />
        public override void Close() { } // Do nothing instead

        /// <inheritdoc />
        public override void Write(ReadOnlySpan<byte> buffer) => wrapped.Write(buffer);

        /// <inheritdoc />
        public override int Read(Span<byte> buffer) => wrapped.Read(buffer);

        /// <inheritdoc />
        public override int EndRead(IAsyncResult asyncResult) => wrapped.EndRead(asyncResult);
        
        /// <inheritdoc />
        public override void EndWrite(IAsyncResult asyncResult) => wrapped.EndWrite(asyncResult);

        /// <inheritdoc />
        public override Task FlushAsync(CancellationToken cancellationToken) => wrapped.FlushAsync(cancellationToken);

        /// <inheritdoc />
        public override int ReadByte() => wrapped.ReadByte();

        /// <inheritdoc />
        public override void WriteByte(byte value) => wrapped.WriteByte(value);

        /// <inheritdoc />
        public override Task<int> ReadAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken) => wrapped.ReadAsync(buffer , offset, count, cancellationToken);

        /// <inheritdoc />
        public override ValueTask<int> ReadAsync(Memory<byte> buffer, CancellationToken cancellationToken = default) => wrapped.ReadAsync(buffer, cancellationToken);

        /// <inheritdoc />
        public override Task WriteAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken) => wrapped.WriteAsync(buffer, offset, count , cancellationToken);

        /// <inheritdoc />
        public override ValueTask WriteAsync(ReadOnlyMemory<byte> buffer, CancellationToken cancellationToken = default) => wrapped.WriteAsync(buffer, cancellationToken);

        /// <inheritdoc />
        public override void CopyTo(Stream destination, int bufferSize) => wrapped.CopyTo(destination, bufferSize);

        /// <inheritdoc />
        public override Task CopyToAsync(Stream destination, int bufferSize, CancellationToken cancellationToken) => wrapped.CopyToAsync(destination, bufferSize, cancellationToken);

        /// <inheritdoc />
        public override ValueTask DisposeAsync() => new(Task.Factory.StartNew(Dispose)); // Forward to the current Dispose implementation instead.

        /// <summary>
        /// Disposes the current stream by unreferencing it.
        /// </summary>
        /// <param name="disposing">A value whether a full disposal is performed.</param>
        protected override void Dispose(bool disposing) {
            if (disposing) {
                wrapped = null;
            }
        }

        /// <inheritdoc />
        public override string ToString() => wrapped?.ToString() ?? "Disposed";
    }
}