
using System;

namespace MP.IO.DataStructuring.Generation
{
    /// <summary>Indicates to the MP Data Structure generator that a given data structure field is a variable length string.</summary>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = false)]
    public sealed class VariableLengthStringAttribute : Attribute
    {
        private readonly string length_provider;
        private readonly StringFieldEncoding encoding;

        /// <summary>
        /// Initializes a new instance of the <see cref="VariableLengthStringAttribute"/> class,
        /// providing the name of the field providing the length in bytes of the string, as well as the desired encoding the string will have. <br />
        /// The default encoding is specified as <see cref="StringFieldEncoding.ASCII"/>.
        /// </summary>
        /// <param name="length_field">The name of the field that provides the length of the variable-length string, in bytes, not characters.</param>
        /// <param name="encoding">The encoding of the fixed string.</param>
        /// <exception cref="ArgumentNullException"><paramref name="length_field"/> is <see langword="null"/>.</exception>
        public VariableLengthStringAttribute(string length_field, StringFieldEncoding encoding = StringFieldEncoding.ASCII)
        {
            ArgumentNullException.ThrowIfNull(length_field);
            length_provider = length_field;
            this.encoding = encoding;
        }

        /// <summary>The name of the field providing the length, in bytes, of the string.</summary>
        public string LengthProvider => length_provider;

        /// <summary>Gets the desired encoding under which the string will be saved to and read as.</summary>
        public StringFieldEncoding Encoding => encoding;
    }
}