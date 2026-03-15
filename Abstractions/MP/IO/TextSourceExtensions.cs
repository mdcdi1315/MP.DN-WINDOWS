
using System;
using MP.Utilities;
using System.Text;
using MP.IO.Buffers;
using MP.Collections;
using System.Threading;
using System.Collections.Generic;
using MP.Annotations.CodeAnalysis;
using System.Diagnostics.CodeAnalysis;

namespace MP.IO
{
    /// <summary>
    /// Provides extension methods for the <see cref="ITextSource"/> interface.
    /// </summary>
    public static class TextSourceExtensions
    {
        /// <summary>
        /// Reads a single character from the text source. <br />
        /// The return value indicates whether a character was actually read from the source.
        /// </summary>
        /// <param name="source">The text source to use.</param>
        /// <param name="ch">The character that was read.</param>
        /// <returns>A value whether a character was read from the text source.</returns>
        /// <exception cref="IOException">An I/O error was occured. (For those text sources implemented by a data stream)</exception>
        /// <exception cref="ObjectDisposedException">The data stream has been disposed of.</exception>
        /// <exception cref="NotSupportedException">Read operation is not supported on this data stream.</exception>
        [Throws(
            typeof(IOException),
            typeof(NotSupportedException),
            typeof(ObjectDisposedException)
        )]
        public static System.Boolean ReadChar(this ITextSource source, [NotNull] out System.Char ch)
        {
            Span<System.Char> c = stackalloc System.Char[1];
            System.Boolean has_data = source.Read(c) > 0;
            ch = c[0];
            return has_data;
        }

        /// <summary>Writes a single character to the text source.</summary>
        /// <param name="source">The text source to use.</param>
        /// <param name="ch">The character to write.</param>
        /// <exception cref="IOException">An I/O error was occured. (For those text sources implemented by a data stream)</exception>
        /// <exception cref="ObjectDisposedException">The data stream has been disposed of.</exception>
        /// <exception cref="NotSupportedException">Read operation is not supported on this data stream.</exception>
        [Throws(
            typeof(IOException),
            typeof(NotSupportedException),
            typeof(ObjectDisposedException)
        )]
        public static void WriteChar(this ITextSource source, System.Char ch) => source.Write(new ReadOnlySpan<System.Char>(ch));

        private sealed class ReadAllLinesEnumerator : BaseEnumerator<System.String>
        {
            private string saved;
            private ITextSource text_source;
            private StringBuilder temp_builder;
            private int buffer_index, buffer_read_limit;
            private bool has_carriage_return, terminated;
            private ArrayPoolBufferAcquireContext<System.Char> context;

            public ReadAllLinesEnumerator(ITextSource source)
            {
                text_source = source;
                temp_builder = new(512);
                buffer_index = buffer_read_limit = 0;
                has_carriage_return = terminated = false;
                context = System.Buffers.ArrayPool<System.Char>.Shared.RentByContext(2048);
            }

            public override System.String Current => saved;

            private bool ProcessBufferData()
            {
                if (has_carriage_return) {
                    if (context.Buffer[buffer_index] == '\n') { buffer_index++; }
                    saved = temp_builder.ToStringAndClear();
                    // Clear the flag, and continue.
                    has_carriage_return = false;
                    return true;
                } else {
                    char c;
                    while (buffer_index < buffer_read_limit)
                    {
                        // Note the ++ notation on buffer_index below.
                        // This done so that we move to the next character at the next loop iteration.
                        // This increment MUST happen here; specifying it earlier or later will make the ENTIRE enumerator implementation to stuck into an inifinite loop.
                        c = context.Buffer[buffer_index++];
                        if (c == '\r') {
                            if (buffer_index < buffer_read_limit) {
                                saved = temp_builder.ToStringAndClear();
                                if ((c = context.Buffer[buffer_index++]) != '\n') { temp_builder.Append(c); }
                                return true;
                            } else {
                                has_carriage_return = true;
                            }
                        } else if (c == '\n') {
                            saved = temp_builder.ToStringAndClear();
                            return true;
                        } else {
                            temp_builder.Append(c);
                        }
                    }
                    return false;
                }
            }

            protected override bool MoveNextImpl()
            {
                if (terminated) {
                    return false;
                } else if (ProcessBufferData()) {
                    return true;
                } else {
                    while ((buffer_read_limit = text_source.Read(context.Buffer)) > 0)
                    {
                        buffer_index = 0;
                        if (ProcessBufferData()) { return true; }
                    }
                    // If we cannot read more characters from the text source, 
                    // and the string builder has some characters appended to it, make a final entry and return the data.
                    // In either way, a next call to MoveNext() will indicate end of iteration, since of the 'terminated' variable assignment we do below.
                    terminated = true;
                    if (temp_builder.Length > 0) {
                        saved = temp_builder.ToStringAndClear();
                        return true;
                    } else {
                        return false;
                    }
                }
            }

            protected override void ResetImpl() => throw new NotSupportedException("Cannot reset in this enumerator implementation.");

            public override void Dispose()
            {
                Monitor.Enter(this);
                try {
                    if (context is not null)
                    {
                        context.Dispose();
                        context = null;
                        text_source = null;
                        temp_builder = null;
                    }
                } finally {
                    base.Dispose();
                    Monitor.Exit(this);
                }
            }
        }

        /// <summary>
        /// Reads ALL the lines that can be read from the current text source.
        /// </summary>
        /// <param name="source">The text source to read the lines from.</param>
        /// <returns>A new <see cref="IEnumerable{T}"/> instance that returns <see cref="string"/>s, each one representing a line from the text source.</returns>
        public static IEnumerable<System.String> ReadAllLines(this ITextSource source)
            => new SingletonEnumeratorEnumerable<System.String>(new ReadAllLinesEnumerator(source));

        /// <summary>Writes all the lines to the current text source.</summary>
        /// <param name="source">The text source to write the lines to.</param>
        /// <param name="lines">The lines to write to the text source.</param>
        public static void WriteAllLines(this ITextSource source, IEnumerable<System.String> lines)
        {
            ArgumentNullException.ThrowIfNull(lines);

            foreach (System.String line in lines)
            {
                source.Write(line);
                source.Write("\r\n");
            }
        }

        /// <summary>
        /// Like <see cref="StreamMethods.DirectCopyToStream(IDataStreamAccess, IDataStreamAccess)"/>, this method
        /// copies all the characters from the current text source to the specified text source.
        /// </summary>
        /// <param name="source">The text source to read all the characters from.</param>
        /// <param name="destination">The text source to write all the characters from the current source.</param>
        /// <exception cref="ArgumentNullException"><paramref name="destination"/> is <see langword="null"/>.</exception>
        [Throws(typeof(ArgumentNullException))]
        public static void DirectCopyToTextSource(this ITextSource source, ITextSource destination) => DirectCopyToTextSource(source, destination, 2048);

        /// <summary>
        /// Like <see cref="StreamMethods.DirectCopyToStream(IDataStreamAccess, IDataStreamAccess)"/>, this method
        /// copies all the characters from the current text source to the specified text source.
        /// </summary>
        /// <param name="source">The text source to read all the characters from.</param>
        /// <param name="destination">The text source to write all the characters from the current source.</param>
        /// <param name="buffer_size">The size of the 'copy' buffer to use. Must be more or equal to 1024 bytes.</param>
        /// <exception cref="IOException">An I/O exception was occurred.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="destination"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="buffer_size"/> is less than 1024 bytes.</exception>
        /// <exception cref="NotSupportedException">Reading from the current source or writing to the specified source is not supported.</exception>
        [Throws(typeof(ArgumentNullException), typeof(ArgumentOutOfRangeException), typeof(IOException), typeof(NotSupportedException))]
        public static void DirectCopyToTextSource(this ITextSource source, ITextSource destination, int buffer_size)
        {
            ArgumentNullException.ThrowIfNull(destination);
            if (buffer_size < 1024) {
                throw new ArgumentOutOfRangeException(nameof(buffer_size), "Buffer size was too small to be used for copying between the two text sources.");
            } else {
                int read_chars;

                using (var cxt = System.Buffers.ArrayPool<System.Char>.Shared.RentByContext(buffer_size))
                {
                    Span<System.Char> buffer = cxt.Buffer;
                    while ((read_chars = source.Read(buffer)) > 0)
                    {
                        destination.Write(buffer.Slice(0, read_chars));
                    }
                }
            }
        }

        /// <summary>
        /// Consecutively reads the specified number of characters from the specified text source and returns a string providing the read characters. <br />
        /// The number of characters in the returned string object indicate how many characters were actually read from the text source.
        /// </summary>
        /// <param name="source">The text source to read characters from.</param>
        /// <param name="n_chars">Maximum number of characters to read from the source.</param>
        /// <param name="buffer_size">The intermediate buffer size, in characters.</param>
        /// <returns>A new string containing the read characters. It can have a maximum length of <paramref name="n_chars"/>.</returns>
        /// <exception cref="IOException">An I/O exception was occurred.</exception>
        /// <exception cref="NotSupportedException">Reading from the specified source is not supported.</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="n_chars"/> is negative -or- <paramref name="buffer_size"/> is less than 1024.</exception>
        [Throws(typeof(ArgumentOutOfRangeException), typeof(IOException), typeof(NotSupportedException))]
        public static System.String ReadString(this ITextSource source, int n_chars, int buffer_size)
        {
            if (n_chars < 0) {
                throw new ArgumentOutOfRangeException(nameof(n_chars), "Number of characters cannot be negative.");
            } else if (n_chars == 0) {
                return System.String.Empty;
            } else if (buffer_size < 1024) {
                throw new ArgumentOutOfRangeException(nameof(buffer_size), "Buffer size is too small.");
            } else {
                StringBuilder sb = new(n_chars);

                using (var temp_cb = System.Buffers.ArrayPool<System.Char>.Shared.RentByContext(buffer_size))
                {
                    Span<System.Char> tsp = temp_cb.Buffer;

                    int t, read = 0;

                    while (read < n_chars)
                    {
                        t = source.Read(tsp.Slice(0, MathHelpers.ComputeBufferSize(read, n_chars, buffer_size)));
                        if (t < 0) { break; } else { sb.Append(tsp.Slice(0, t)); }
                        read += t;
                    }
                }

                return sb.ToString();
            }
        }

        /// <summary>
        /// Consecutively reads the specified number of characters from the specified text source and returns a string providing the read characters. <br />
        /// The number of characters in the returned string object indicate how many characters were actually read from the text source.
        /// </summary>
        /// <param name="source">The text source to read characters from.</param>
        /// <param name="n_chars">Maximum number of characters to read from the source.</param>
        /// <returns>A new string containing the read characters. It can have a maximum length of <paramref name="n_chars"/>.</returns>
        /// <exception cref="IOException">An I/O exception was occurred.</exception>
        /// <exception cref="NotSupportedException">Reading from the specified source is not supported.</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="n_chars"/> is negative.</exception>
        [Throws(typeof(ArgumentOutOfRangeException), typeof(IOException), typeof(NotSupportedException))]
        public static System.String ReadString(this ITextSource source, int n_chars) => ReadString(source, n_chars, 2048);
    }
}