
using System;
using System.IO;
using System.Runtime.CompilerServices;

namespace MP.IO
{
    /// <summary>
    /// Adapts any object implementing the <see cref="IDataStreamAccess"/> interface to a .NET <see cref="Stream"/> object.
    /// </summary>
    public sealed class DataStreamToStreamAdapter : Stream
    {
        private IDataStreamAccess underlying;

        /// <summary>
        /// Creates a new instance of the <see cref="DataStreamToStreamAdapter"/> class.
        /// </summary>
        /// <param name="underlying">The stream object to be wrapped.</param>
        /// <exception cref="ArgumentNullException"><paramref name="underlying"/> is <see langword="null"/>.</exception>
        public DataStreamToStreamAdapter(IDataStreamAccess underlying) : base() => this.underlying = underlying ?? throw new ArgumentNullException(nameof(underlying));

        /// <inheritdoc />
        public override bool CanRead => underlying.CanRead;

        /// <inheritdoc />
        public override bool CanSeek
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => (underlying.Mode & DataStreamMode.Seek) != DataStreamMode.Invalid;
        }

        /// <inheritdoc />
        public override bool CanWrite => underlying.CanWrite;

        /// <inheritdoc />
        public override System.Int64 Length
        {
            get {
                if (underlying is DataStream d) {
                    return d.Length;
                } else {
                    throw new NotSupportedException();
                }
            }
        }

        /// <inheritdoc />
        public override System.Int64 Position 
        { 
            get {
                if (underlying is DataStream d) {
                    return d.Position;
                } else {
                    throw new NotSupportedException();
                }
            }
            set {
                if (value < 0L) {
                    throw new ArgumentOutOfRangeException(nameof(value), "Position cannot be a negative value.");
                } else if (underlying is DataStream d) {
                    d.Seek(value, SeekDisplacement.Begin);
                } else {
                    throw new NotSupportedException();
                }
            }
        }

        /// <inheritdoc />
        public override void Flush()
        {
            if (underlying is AbstractFileStream f) { f.Flush(); }
        }

        /// <inheritdoc />
        public override int Read(Span<byte> buffer)
        {
            int rb = underlying.Read(buffer);
            return (rb < 0) ? 0 : rb;
        }

        /// <inheritdoc />
        public override int Read(System.Byte[] buffer, int offset, int count)
        {
            int rb = underlying.Read(buffer, offset, count);
            return (rb < 0) ? 0 : rb;
        }

        /// <inheritdoc />
        public override void Write(ReadOnlySpan<System.Byte> buffer) => underlying.Write(buffer);

        /// <inheritdoc />
        public override void Write(System.Byte[] buffer, int offset, int count) => underlying.Write(buffer, offset, count);

        /// <inheritdoc />
        public override System.Int64 Seek(System.Int64 offset, SeekOrigin origin)
        {
            if (underlying is DataStream d) {
                return d.Seek(offset, (SeekDisplacement)origin);
            } else {
                throw new NotSupportedException();
            }
        }

        /// <inheritdoc />
        public override void SetLength(System.Int64 value)
        {
            if (value < 0L) {
                throw new ArgumentOutOfRangeException(nameof(value), "Value cannot be negative.");
            } else if (underlying is AbstractFileStream f) { 
                f.SetLength(value); 
            }
        }

        /// <inheritdoc />
        public override int ReadByte() => underlying.ReadByte();

        /// <inheritdoc />
        public override void WriteByte(byte value) => underlying.WriteByte(value);

        /// <inheritdoc />
        protected override void Dispose(bool disposing)
        {
            if (disposing && underlying is not null) { underlying.Dispose(); underlying = null; }
        }
    }
}
