
using System;
using System.Text;
using System.Buffers;
using MP.Collections;
using System.Collections;
using System.Collections.Generic;
using MP.Annotations.CodeAnalysis;
using System.Diagnostics.CodeAnalysis;

namespace MP.Utilities
{
    /// <summary>
    /// Adds various and useful extension methods for the <see cref="StringBuilder"/> class.
    /// </summary>
    public static class StringBuilderExtensions
    {
        /// <summary>
        /// Appends the string returned by processing a composite format string, which contains zero or more format items, to this instance. <br />
        /// Each format item is replaced by the string representation of a corresponding argument in a parameter array. <br />
        /// Finally, the builder appends the default line terminator, as specified by the <see cref="StringBuilder.AppendLine()"/> method.
        /// </summary>
        /// <param name="builder">The string builder instance to use.</param>
        /// <param name="format">A composite format string.</param>
        /// <param name="format_args">An array of objects to be formatted.</param>
        /// <returns>
        /// A reference to this instance with <paramref name="format"/> appended and the default line terminator at the end of the formatted string. <br />
        /// Each format item in <paramref name="format"/> is replaced by the string representation of the corresponding object argument.
        /// </returns>
        /// <exception cref="ArgumentNullException"><paramref name="format"/> and/or <paramref name="format_args"/> are <see langword="null"/>.</exception>
        /// <exception cref="ArgumentException"><paramref name="format"/> is invalid.
        /// -or- 
        /// The index of a format item is less than 0 (zero), or greater than or equal to the length of the <paramref name="format_args"/> array.
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">The length of the expanded string would exceed <see cref="StringBuilder.MaxCapacity"/>.</exception>
        [Throws(typeof(ArgumentException), typeof(ArgumentNullException), typeof(ArgumentOutOfRangeException))]
        public static StringBuilder AppendFormatLine(this StringBuilder builder, [StringSyntax(StringSyntaxAttribute.CompositeFormat)] string format, params object[] format_args)
        {
            builder.AppendFormat(format, args: format_args);
            return builder.AppendLine();
        }

        /// <summary>
        /// Converts the value of this instance to a <see cref="System.String"/>. <br />
        /// The contents of this instance will have been cleared once this method returns.
        /// </summary>
        /// <param name="builder">The string builder instance to use.</param>
        /// <returns>A string whose value is the same as this instance.</returns>
        [return: NotNull]
        public static string ToStringAndClear(this StringBuilder builder)
        {
            string str = builder.ToString();
            builder.Clear();
            return str;
        }

        private sealed class NextNextChunkEnumerator : NextNextEnumerator<ReadOnlyMemory<System.Char>>
        {
            private sealed class ChunkEnumeratorIEnumerator : IEnumerator<ReadOnlyMemory<System.Char>>
            {
                private readonly StringBuilder.ChunkEnumerator ce;

                public ChunkEnumeratorIEnumerator(StringBuilder.ChunkEnumerator c) => ce = c;

                public ReadOnlyMemory<char> Current => ce.Current;

                object IEnumerator.Current => Current;

                public void Dispose() {}

                public bool MoveNext() => ce.MoveNext();

                public void Reset() => throw new InvalidOperationException("Not supported");
            }

            public NextNextChunkEnumerator(StringBuilder.ChunkEnumerator ce) : base(new ChunkEnumeratorIEnumerator(ce)) {}
        }

        /// <summary>
        /// Gets a <see cref="NextNextEnumerator{T}"/> chunk enumerator for the given <see cref="StringBuilder"/> instance.
        /// </summary>
        /// <param name="builder">The string builder instance to use.</param>
        /// <returns>A new instance of a <see cref="NextNextEnumerator{T}"/>.</returns>
        [return: NotNull]
        public static NextNextEnumerator<ReadOnlyMemory<System.Char>> GetNextNextChunkEnumerator(this StringBuilder builder) => new NextNextChunkEnumerator(builder.GetChunks());

        /// <summary>
        /// Writes the contents of the current string builder instance to the specified data stream with the specified <see cref="Encoding"/>.
        /// </summary>
        /// <param name="builder">The string builder instance to use.</param>
        /// <param name="stream">The data stream to write the contents of the string builder as.</param>
        /// <param name="encoding">The text encoding to use for storing the characters contained in the <paramref name="builder"/>.</param>
        /// <exception cref="ArgumentNullException"><paramref name="stream"/> and/or <paramref name="encoding"/> are <see langword="null"/>.</exception>
        /// <exception cref="ArgumentException"><paramref name="stream"/> is not a writeable data stream.</exception>
        public static void WriteToStream(this StringBuilder builder, System.IO.Stream stream, Encoding encoding)
        {
            ArgumentNullException.ThrowIfNull(stream);
            ArgumentNullException.ThrowIfNull(encoding);

            if (!stream.CanWrite) { throw new ArgumentException("Stream is unwriteable." , nameof(stream)); }

            System.Byte[] bytes = null;

            Encoder encoder = encoding.GetEncoder();

            try {
                bytes = ArrayPool<System.Byte>.Shared.Rent(2048);

                bool completed;
                ReadOnlySpan<System.Char> mem_span;

                NextNextEnumerator<ReadOnlyMemory<System.Char>> en = new NextNextChunkEnumerator(builder.GetChunks());

                try {
                    while (en.MoveNext())
                    {
                        mem_span = en.Current.Span;
                        do {
                            encoder.Convert(mem_span, bytes, en.HasNextNextElement == false, out int cu, out int bu, out completed);

                            stream.Write(bytes, 0, bu);

                            mem_span = mem_span.Slice(cu);
                        } while (!completed);
                    }
                } finally {
                    en.Dispose();
                }
            } finally {
                ArrayPool<System.Byte>.Shared.Return(bytes);
            }
        }
    }

}