


using System;
using System.Runtime.CompilerServices;

namespace MP.Utilities
{
    /// <summary>
    /// Specialization of the <see cref="System.Text.StringBuilder"/> class for placing indenting characters before each append operation.
    /// </summary>
    public sealed class IndentedStringBuilder 
    {
        private long count;
        private char tab_ch;
        private char[] chars;
        private uint indents;
        private bool use_indents;

        /// <summary>
        /// Initializes a new and empty instance of the <see cref="IndentedStringBuilder"/> class.
        /// </summary>
        public IndentedStringBuilder()
        {
            count = 0;
            indents = 0U;
            tab_ch = '\t';
            use_indents = true;
            chars = Array.Empty<System.Char>();
        }

        private void Grow(long n_chars)
        {
            long new_cap = count + n_chars;
            if (new_cap > chars.LongLength)
            {
                char[] char_temp = new char[new_cap];
                Array.Copy(chars, char_temp, count);
                chars = char_temp;
            }
        }

        private void AppendIndentingCharacter()
        {
            if (use_indents)
            {
                Grow(indents);
                for (uint ts = indents; ts > 0U; ts--) {
                    chars[count++] = tab_ch;
                }
            }
        }

        /// <summary>
        /// Appends a character to the end of this <see cref="IndentedStringBuilder"/> instance.
        /// </summary>
        /// <param name="ch">The character to append.</param>
        public void Append(System.Char ch)
        {
            Grow(1L);
            AppendIndentingCharacter();
            chars[count++] = ch;
        }

        /// <summary>
        /// Appends an array of characters to the current string builder instance.
        /// </summary>
        /// <param name="chars">The characters to copy to this instance.</param>
        /// <param name="index">The starting index in <paramref name="chars"/> to start copying from.</param>
        /// <param name="count">The number of characters to copy from <paramref name="chars"/>.</param>
        /// <exception cref="ArgumentNullException"><paramref name="chars"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentException"><paramref name="index"/> + <paramref name="count"/> exceed the source array bounds.</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="index"/> and/or <paramref name="count"/> are negative value.</exception>
        public void Append(System.Char[] chars, System.Int32 index, System.Int32 count)
        {
            ArgumentNullException.ThrowIfNull(chars);
            if (index < 0) {
                throw new ArgumentOutOfRangeException(nameof(index), "Index cannot be negative.");
            } else if (count < 0) {
                throw new ArgumentOutOfRangeException(nameof(count), "Count cannot be negative.");
            } else if (index + count > chars.LongLength) {
                throw new ArgumentException("The specified combination of index and count parameters exceed the source array bounds.");
            }
            Grow(count);
            AppendIndentingCharacter();
            Unsafe.CopyBlockUnaligned(ref Unsafe.As<System.Char, System.Byte>(ref this.chars[this.count]) , ref Unsafe.As<System.Char, System.Byte>(ref chars[index]) , count.ToUInt32() * sizeof(System.Char));
            this.count += count;
        }

        /// <summary>
        /// Appends an array of characters to the current string builder instance.
        /// </summary>
        /// <param name="chars">The characters to copy to this instance.</param>
        /// <exception cref="ArgumentNullException"><paramref name="chars"/> is <see langword="null"/>.</exception>
        public void Append(System.Char[] chars)
        {
            ArgumentNullException.ThrowIfNull(chars);
            Grow(chars.LongLength);
            AppendIndentingCharacter();
            Unsafe.CopyBlockUnaligned(ref Unsafe.As<System.Char, System.Byte>(ref this.chars[count]) , ref Unsafe.As<System.Char, System.Byte>(ref chars[0]) , chars.LongLength.ToUInt32() * sizeof(System.Char));
            count += chars.LongLength;
        }

        /// <summary>
        /// Appends a portion of a character sequence to the current string builder instance.
        /// </summary>
        /// <param name="str">The character sequence to copy to this instance.</param>
        /// <param name="index">The starting index in <paramref name="str"/> to start copying from.</param>
        /// <param name="count">The number of characters to copy from <paramref name="str"/>.</param>
        /// <exception cref="ArgumentNullException"><paramref name="str"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentException"><paramref name="index"/> + <paramref name="count"/> exceed the source array bounds.</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="index"/> and/or <paramref name="count"/> are negative value.</exception>
        public void Append(System.String str, System.Int32 index, System.Int32 count)
        {
            ArgumentNullException.ThrowIfNull(str);
            if (index < 0) {
                throw new ArgumentOutOfRangeException(nameof(index), "Index cannot be negative.");
            } else if (count < 0) {
                throw new ArgumentOutOfRangeException(nameof(count), "Count cannot be negative.");
            } else if (index + count > str.Length) {
                throw new ArgumentException("The specified combination of index and count parameters exceed the source array bounds.");
            }
            Grow(count);
            AppendIndentingCharacter();
            Unsafe.CopyBlockUnaligned(ref Unsafe.As<System.Char, System.Byte>(ref chars[count]) , ref Unsafe.As<System.Char, System.Byte>(ref Unsafe.Add(ref Unsafe.AsRef(str.GetPinnableReference()), index)) , count.ToUInt32() * sizeof(System.Char));
            this.count += count;
        }

        /// <summary>
        /// Appends a character sequence to the current string builder instance.
        /// </summary>
        /// <param name="str">The character sequence to copy to this instance.</param>
        /// <exception cref="ArgumentNullException"><paramref name="str"/> is <see langword="null"/>.</exception>
        public void Append(System.String str)
        {
            ArgumentNullException.ThrowIfNull(str);
            int ct = str.Length;
            AppendIndentingCharacter();
            if (ct == 0) { return; }
            Grow(ct);
            Unsafe.CopyBlockUnaligned(ref Unsafe.As<System.Char, System.Byte>(ref chars[count]), ref Unsafe.As<System.Char, System.Byte>(ref Unsafe.AsRef(str.GetPinnableReference())) , ct.ToUInt32() * sizeof(System.Char));
            count += ct;
        }

        /// <summary>
        /// Appends a read-only character sequence to the current string builder instance.
        /// </summary>
        /// <param name="span">The character sequence to copy to this instance.</param>
        public void Append(ReadOnlySpan<System.Char> span)
        {
            AppendIndentingCharacter();
            if (span.Length == 0) { return; }
            Grow(span.Length);
            Unsafe.CopyBlockUnaligned(ref Unsafe.As<System.Char, System.Byte>(ref chars[count]), ref Unsafe.As<System.Char, System.Byte>(ref Unsafe.AsRef(span[0])) , span.Length.ToUInt32() * sizeof(System.Char));
            count += span.Length;
        }

        /// <summary>
        /// All appending operations will additionally append one more indent character before they do execute.
        /// </summary>
        public void PushIndent() => indents++;

        /// <summary>
        /// Removes an indent character previously pushed with <see cref="PushIndent"/> method.
        /// </summary>
        public void PopIndent()
        {
            if (indents == 0) { return; }
            indents--;
        }

        /// <summary>
        /// Gets or sets the character to use for indenting.
        /// </summary>
        public System.Char IndentCharacter
        {
            get => tab_ch;
            set => tab_ch = value;
        }

        /// <summary>
        /// Controls whether indenting is actually applied to the current instance. <br />
        /// This overrides even the <see cref="PopIndent"/> and <see cref="PushIndent"/> methods.
        /// </summary>
        public System.Boolean UseIndenting
        {
            get => use_indents;
            set => use_indents = value;
        }

        /// <summary>
        /// Returns the concatenated contents of this <see cref="IndentedStringBuilder"/> instance to a string.
        /// </summary>
        /// <returns>A new <see cref="System.String"/> containing the contents of this instance.</returns>
        /// <exception cref="OverflowException">The stored contents on this instance are too large to fit into a <see cref="System.String"/>.</exception>
        public override string ToString()
        {
            if (count > System.Int32.MaxValue) {
                throw new OverflowException("The produced value from the string builder is too large to fit into a single string instance.");
            } else {
                return new(chars, 0, count.ToInt32());
            }
        }
    }
}