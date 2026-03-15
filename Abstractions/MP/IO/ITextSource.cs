
using System;
using System.Text;
using MP.Annotations.CodeAnalysis;

namespace MP.IO
{
    /// <summary>
    /// Defines a text source, that is, a specialized data stream manipulating UTF-16 characters instead of bytes.
    /// </summary>
    public interface ITextSource : IDisposable
    {
        /// <summary>Reads a number of characters from the text source.</summary>
        /// <param name="buffer">The buffer to place the read characters into.</param>
        /// <returns>
        /// Number of characters processed and read from the text source. <br />
        /// A value of 0 indicates that the buffer is empty or no data could be read. <br />
        /// A value of -1 indicates that the end of the text source has been reached.
        /// </returns>
        /// <exception cref="IOException">An I/O error was occured. (For those text sources implemented by a data stream)</exception>
        /// <exception cref="ObjectDisposedException">The data stream has been disposed of.</exception>
        /// <exception cref="NotSupportedException">Read operation is not supported on this data stream.</exception>
        [Throws(
            typeof(IOException),
            typeof(NotSupportedException),
            typeof(ObjectDisposedException)
        )]
        public System.Int32 Read(Span<System.Char> buffer);

        /// <summary>Writes a number of characters to the text source.</summary>
        /// <param name="buffer">The buffer that contains the characters to write.</param>
        /// <exception cref="IOException">An I/O error was occured. (For those text sources implemented by a data stream)</exception>
        /// <exception cref="ObjectDisposedException">The data stream has been disposed of.</exception>
        /// <exception cref="NotSupportedException">Read operation is not supported on this data stream.</exception>
        [Throws(
            typeof(IOException),
            typeof(NotSupportedException),
            typeof(ObjectDisposedException)
        )]
        public void Write(ReadOnlySpan<System.Char> buffer);

        /// <summary>
        /// Gets the <see cref="System.Text.Encoding"/> object that this text source reinterprets the characters as.
        /// </summary>
        Encoding Encoding { get; }

        /// <summary>
        /// Mode of the text source. <br />
        /// Note that the <see cref="DataStreamMode.Seek"/> mode is not supported on a text source.
        /// </summary>
        DataStreamMode Mode { get; }
    }
}
