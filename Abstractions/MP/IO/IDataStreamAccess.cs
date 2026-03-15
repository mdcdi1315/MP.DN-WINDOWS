
using System;
using MP.Annotations.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace MP.IO
{
    /// <summary>
    /// Describes the low-level interface of a <see cref="DataStream"/> instance,
    /// providing only basic operations of reading from and writing to the stream. <br />
    /// Classes not deriving from <see cref="DataStream"/> are allowed to implement this interface,
    /// primarily for two reasons: <br />
    /// <list type="bullet">
    ///     <item>There is an abstraction that uses data streams, but does not specify seeking services.</item>
    ///     <item>The need to disallow the user to explicitly cast to <see cref="DataStream"/>.</item>
    /// </list>
    /// </summary>
    public interface IDataStreamAccess : IDisposable
    {
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
        System.Int32 Read(Span<System.Byte> buffer);

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
        void Write(ReadOnlySpan<System.Byte> buffer);

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
        virtual System.Int16 ReadByte()
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
        virtual void WriteByte(System.Byte value) => Write(new ReadOnlySpan<System.Byte>(value));

        /// <summary>Gets the mode under which the data stream is opened as.</summary>
        /// <remarks>This value should be able to be queried even when the data stream is disposed of.</remarks>
        DataStreamMode Mode 
        {
            [MustNotReportException] 
            get; 
        }

        /// <summary>Gets a value whether the data stream can be read from.</summary>
        /// <remarks>This is equivalent as calling the <see cref="Mode"/> property and checking the <see cref="DataStreamMode.Read"/> flag.</remarks>
        public sealed System.Boolean CanRead
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => (Mode & DataStreamMode.Read) != DataStreamMode.Invalid;
        }

        /// <summary>Gets a value whether the data stream can be written to.</summary>
        /// <remarks>This is equivalent as calling the <see cref="Mode"/> property and checking the <see cref="DataStreamMode.Write"/> flag.</remarks>
        public sealed System.Boolean CanWrite
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => (Mode & DataStreamMode.Write) != DataStreamMode.Invalid;
        }
    }
}
