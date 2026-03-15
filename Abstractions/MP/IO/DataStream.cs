using System;
using System.Threading;
using MP.Annotations.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace MP.IO
{
    /// <summary>A newer evolution of the <see cref="System.IO.Stream"/> class.</summary>
    public abstract class DataStream : IDataStreamAccess
    {
        private bool disposed;

        /// <summary>Validates arguments provided to reading and writing methods on <see cref="DataStream"/>.</summary>
        /// <param name="buffer">The array "buffer" argument passed to the reading or writing method.</param>
        /// <param name="offset">The integer "offset" argument passed to the reading or writing method.</param>
        /// <param name="count">The integer "count" argument passed to the reading or writing method.</param>
        /// <exception cref="ArgumentNullException"><paramref name="buffer"/> was null.</exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="offset"/> was outside the bounds of <paramref name="buffer"/>, or
        /// <paramref name="count"/> was negative, or the range specified by the combination of
        /// <paramref name="offset"/> and <paramref name="count"/> exceed the length of <paramref name="buffer"/>.
        /// </exception>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected static void ValidateBufferArguments(byte[] buffer, int offset, int count)
        {
            ArgumentNullException.ThrowIfNull(buffer);

            if (offset < 0) {
                throw new ArgumentOutOfRangeException(nameof(offset), "Offset cannot be a negative value.");
            }

            if ((uint)count > buffer.Length - offset) {
                throw new ArgumentOutOfRangeException(nameof(count), "The specified count exceeds the buffer's bounds.");
            }
        }

        /// <summary>Initializes the <see cref="DataStream"/> instance.</summary>
        protected DataStream() => disposed = false;

        /// <summary>Gets a value whether the data stream is seekable.</summary>
        /// <remarks>This is equivalent as calling the <see cref="Mode"/> property and checking the <see cref="DataStreamMode.Seek"/> flag.</remarks>
        public System.Boolean CanSeek
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => (Mode & DataStreamMode.Seek) != 0;
        }

        /// <summary>Gets a value whether the data stream can be read from.</summary>
        /// <remarks>This is equivalent as calling the <see cref="Mode"/> property and checking the <see cref="DataStreamMode.Read"/> flag.</remarks>
        public System.Boolean CanRead
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => (Mode & DataStreamMode.Read) != DataStreamMode.Invalid;
        }

        /// <summary>Gets a value whether the data stream can be written to.</summary>
        /// <remarks>This is equivalent as calling the <see cref="Mode"/> property and checking the <see cref="DataStreamMode.Write"/> flag.</remarks>
        public System.Boolean CanWrite
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => (Mode & DataStreamMode.Write) != DataStreamMode.Invalid;
        }

        /// <summary>
        /// Gets the length of this data stream. <br />
        /// Will be 0 if the length is not known.
        /// </summary>
        public virtual System.Int64 Length => 0L;

        /// <summary>
        /// Gets/sets the seek pointer position of this data stream. <br />
        /// Returns 0 if not supported and throws <see cref="NotSupportedException"/> if not supported to seek altogether.
        /// </summary>
        /// <exception cref="NotSupportedException">Seeking is not supported.</exception>
        /// <exception cref="IOException">An I/O error occurred while trying to get or set the stream's seek pointer.</exception>
        public virtual System.Int64 Position
        {
            [Throws(
                typeof(IOException),
                typeof(ObjectDisposedException)
            )]
            get => 0L;
            [Throws(
                typeof(IOException),
                typeof(OverflowException),
                typeof(NotSupportedException),
                typeof(ObjectDisposedException)
            )]
            set {
                if (value > System.Int64.MaxValue) {
                    throw new OverflowException("This large seek value is not supported: " + value);
                } else {
                    Seek(value, SeekDisplacement.Begin);
                }
            }
        }

        /// <summary>Gets the mode under which the data stream is opened as.</summary>
        public virtual DataStreamMode Mode => DataStreamMode.Invalid;

        /// <summary>Seeks within the data stream.</summary>
        /// <param name="offset">The offset to seek by.</param>
        /// <param name="displacement">The seeking mode to use.</param>
        /// <returns>The value of the seek pointer after the seeking operation has been completed.</returns>
        /// <exception cref="IOException">An I/O error was occured.</exception>
        /// <exception cref="NotSupportedException">Seeking is not supported.</exception>
        /// <exception cref="ObjectDisposedException">The data stream has been disposed of.</exception>
        /// <exception cref="NotSupportedException">Seek operation is not supported on this data stream.</exception>
        /// <exception cref="ArgumentException">
        /// The <paramref name="offset"/> and <paramref name="displacement"/> parameters that were specified were out of the stream's bounds, when in reading mode. <br />
        /// In writing mode, this exception should not be thrown but rather attempt to 'extend' the data stream.
        /// </exception>
        [Throws(
            typeof(IOException),
            typeof(ArgumentException),
            typeof(NotSupportedException),
            typeof(ObjectDisposedException),
            typeof(ArgumentOutOfRangeException)
        )]
        public virtual System.Int64 Seek(System.Int64 offset, SeekDisplacement displacement) => throw new NotSupportedException();

        /// <summary>Reads data from the data stream.</summary>
        /// <param name="buffer">The buffer to place the read data into.</param>
        /// <returns>
        /// Number of bytes actually in use inside the <paramref name="buffer"/>. <br />
        /// A value of 0 indicates that the buffer is empty or no data could be read. <br />
        /// A value of -1 indicates that the end of the stream has been reached.
        /// </returns>
        /// <exception cref="IOException">An I/O error was occured.</exception>
        /// <exception cref="ObjectDisposedException">The data stream has been disposed of.</exception>
        /// <exception cref="NotSupportedException">Read operation is not supported on this data stream.</exception>
        [Throws(
            typeof(IOException),
            typeof(NotSupportedException),
            typeof(ObjectDisposedException)
        )]
        public abstract System.Int32 Read(Span<System.Byte> buffer);

        /// <summary>Writes data to the data stream.</summary>
        /// <param name="buffer">The buffer that contains the data to write.</param>
        /// <exception cref="IOException">An I/O error was occured.</exception>
        /// <exception cref="ObjectDisposedException">The data stream has been disposed of.</exception>
        /// <exception cref="NotSupportedException">Write operation is not supported on this data stream.</exception>
        [Throws(
            typeof(IOException),
            typeof(NotSupportedException),
            typeof(ObjectDisposedException)
        )]
        public abstract void Write(ReadOnlySpan<System.Byte> buffer);

        /// <summary>Reads data from the data stream.</summary>
        /// <param name="buffer">The buffer to place the read data into.</param>
        /// <param name="offset">The starting offset in <paramref name="buffer"/> to start copying data from.</param>
        /// <param name="count">The number of bytes to copy from <paramref name="buffer"/> to the data stream.</param>
        /// <returns>
        /// Number of bytes actually in use inside the <paramref name="buffer"/>. <br />
        /// A value of 0 indicates that the buffer is empty or no data could be read. <br />
        /// A value of -1 indicates that the end of the stream has been reached.
        /// </returns>
        /// <exception cref="IOException">An I/O error was occured.</exception>
        /// <exception cref="ObjectDisposedException">The data stream has been disposed of.</exception>
        /// <exception cref="NotSupportedException">Read operation is not supported on this data stream.</exception>
        [Throws(
            typeof(IOException),
            typeof(ArgumentNullException),
            typeof(NotSupportedException),
            typeof(ObjectDisposedException),
            typeof(ArgumentOutOfRangeException)
        )]
        public virtual System.Int32 Read(System.Byte[] buffer, System.Int32 offset, System.Int32 count)
        {
            ArgumentNullException.ThrowIfNull(buffer);
            return Read(new Span<System.Byte>(buffer, offset, count));
        }

        /// <summary>Writes data to the data stream.</summary>
        /// <param name="buffer">The buffer that contains the data to write.</param>
        /// <param name="offset">The starting offset in <paramref name="buffer"/> to start copying data to.</param>
        /// <param name="count">The number of bytes to copy from the data stream to <paramref name="buffer"/>.</param>
        /// <exception cref="IOException">An I/O error was occured.</exception>
        /// <exception cref="ObjectDisposedException">The data stream has been disposed of.</exception>
        /// <exception cref="NotSupportedException">Write operation is not supported on this data stream.</exception>
        [Throws(
            typeof(IOException),
            typeof(ArgumentNullException),
            typeof(NotSupportedException),
            typeof(ObjectDisposedException),
            typeof(ArgumentOutOfRangeException)
        )]
        public virtual void Write(System.Byte[] buffer, System.Int32 offset, System.Int32 count)
        {
            ArgumentNullException.ThrowIfNull(buffer);
            Write(new ReadOnlySpan<System.Byte>(buffer, offset, count));
        }

        /// <summary>Reads a byte from the data stream.</summary>
        /// <remarks>Implementations having a better and faster alternative to handle this kind of reading should override this method.</remarks>
        /// <returns>
        /// The value of the read byte, cast to <see cref="System.Int16"/>. <br />
        /// If the end of stream has been reached or no data could be read, -1 is returned.
        /// </returns>
        /// <exception cref="IOException">An I/O error was occured.</exception>
        /// <exception cref="ObjectDisposedException">The data stream has been disposed of.</exception>
        /// <exception cref="NotSupportedException">Read operation is not supported on this data stream.</exception>
        [Throws(
            typeof(IOException),
            typeof(NotSupportedException),
            typeof(ObjectDisposedException)
        )]
        public virtual System.Int16 ReadByte()
        {
            Span<System.Byte> b = stackalloc System.Byte[1];
            return (Read(b) < 1) ? (System.Int16)(-1) : b[0];
        }

        /// <summary>Writes a byte to the data stream.</summary>
        /// <remarks>Implementations having a better and faster alternative to handle this kind of writing should override this method.</remarks>
        /// <param name="value">The byte to be written to the data stream.</param>
        /// <exception cref="IOException">An I/O error was occured.</exception>
        /// <exception cref="ObjectDisposedException">The data stream has been disposed of.</exception>
        /// <exception cref="NotSupportedException">Write operation is not supported on this data stream.</exception>
        [Throws(
            typeof(IOException),
            typeof(NotSupportedException),
            typeof(ObjectDisposedException)
        )]
        public virtual void WriteByte(System.Byte value) => Write(new ReadOnlySpan<System.Byte>(value));

        /// <summary>
        /// Throws an <see cref="ObjectDisposedException"/> if the current <see cref="DataStream"/> is disposed.
        /// </summary>
        [Throws(typeof(ObjectDisposedException))]
        protected void ThrowIfDisposed() => ObjectDisposedException.ThrowIf(disposed, this);

        /// <summary>Dispose code for derived <see cref="DataStream"/> classes.</summary>
        /// <param name="disposing">A value whether this call is performed from the <see cref="Dispose()"/> method.</param>
        protected virtual void Dispose(System.Boolean disposing) { }

        /// <summary>Default finalizer implementation.</summary>
        // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        // The finalizer always calls the Dispose(bool) method only once, so no lock is required.
        ~DataStream() => Dispose(disposing: false);

        /// <summary>
        /// Disposes this <see cref="DataStream"/> object. <br />
        /// Note that this call is thread-safe; that is, multiple threads can call it. <br />
        /// As such, syncronization assures that the <see cref="Dispose(bool)"/> method will be called only once.
        /// </summary>
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Monitor.Enter(this);
            try {
                if (!disposed) { Dispose(disposing: true); }
            } finally {
                if (!disposed) { GC.SuppressFinalize(this); }
                disposed = true;
                Monitor.Exit(this);
            }
        }
    }
}
