using System;
using System.Diagnostics.CodeAnalysis;

namespace MP.Serialization.Transcoders
{
    /// <summary>
    /// Defines an <see cref="ITypeTranscoder"/> for encoding and decoding enumeration types.
    /// </summary>
    public sealed class EnumerationTypeTranscoder : ITypeTranscoder
    {
        /// <inheritdoc />
        /// <remarks>
        /// This transcoder de/encodes the enumeration values into strings.
        /// </remarks>
        public SerializedFieldType SerializedType => SerializedFieldType.String;

        /// <inheritdoc />
        /// <remarks>
        /// This property always returns the type object of the <see cref="Enum"/> type.
        /// </remarks>
        public Type ActualType => typeof(Enum);

        /// <inheritdoc />
        /// <remarks>For this type transcoder, loose type match must be used for matching derived <see cref="Enum"/> instances.</remarks>
        public bool StrictTypeMatch => false; // You can define a variable of type Enum but you cannot define a class deriving from enum , you must define an enumeration type.

        /// <inheritdoc />
        [return: NotNull]
        public object Decode([DisallowNull] object value, [DisallowNull] Type actual)
        {
            var vs = value.ToString();
            foreach (var v in actual.GetEnumValues())
            {
                if (v.ToString().Equals(vs , StringComparison.OrdinalIgnoreCase)) {
                    return v;
                }
            }
            throw new SerializationException($"Cannot find enumeration field of name {value} in the enumeration of type {actual.FullName}.");
        }

        /// <inheritdoc />
        public object Encode([DisallowNull] object value) => value.ToString();
    }
}