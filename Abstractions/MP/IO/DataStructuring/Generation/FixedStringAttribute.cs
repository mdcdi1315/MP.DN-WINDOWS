
using System;

namespace MP.IO.DataStructuring.Generation
{
    /// <summary>Attribute to specify in a data structure that a string field is a fixed string of the specified size.</summary>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = false)]
    public sealed class FixedStringAttribute : Attribute
    {
        private readonly int length;
        private readonly StringFieldEncoding encoding;

        /// <summary>
        /// Initializes a new instance of the <see cref="FixedStringAttribute"/> class,
        /// specifying the length of the fixed string, as well as the desired encoding the string will have. <br />
        /// The default encoding is specified as <see cref="StringFieldEncoding.ASCII"/>.
        /// </summary>
        /// <param name="length">The length of the fixed string, in characters.</param>
        /// <param name="encoding">The encoding of the fixed string.</param>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="length"/> is a negative value.</exception>
        public FixedStringAttribute(int length, StringFieldEncoding encoding = StringFieldEncoding.ASCII)
        {
            if (length < 0) {
                throw new ArgumentOutOfRangeException(nameof(length));
            } else {
                this.length = length;
                this.encoding = encoding;
            }
        }

        /// <summary>Gets the exact length, in characters, of the current string field.</summary>
        public int StringLength => length;

        /// <summary>Gets the desired encoding under which the string will be saved to and read as.</summary>
        public StringFieldEncoding Encoding => encoding;
    }
}