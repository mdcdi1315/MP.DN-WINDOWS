


using System;
using System.Diagnostics.CodeAnalysis;

namespace MP.Serialization.Transcoders
{
    /// <summary>
    /// Provides a type transcoder for <see cref="Uri"/> instances.
    /// </summary>
    public sealed class UriTranscoder : ITypeTranscoder
    {
        /// <inheritdoc />
        public SerializedFieldType SerializedType => SerializedFieldType.String;

        /// <inheritdoc />
        public Type ActualType => typeof(Uri);

        /// <inheritdoc />
        public bool StrictTypeMatch => false; // Derived types of Uri may exist as well.

        /// <inheritdoc />
        [return: NotNull]
        public object Decode([DisallowNull] object value, [DisallowNull] Type actual)
        {
            try {
                return new Uri(value.ToString());
            } catch (UriFormatException ufe) {
                throw new SerializationException("Cannot construct the URI. The URI may be invalid." , ufe);
            }
        }

        /// <inheritdoc />
        public object Encode([DisallowNull] object value) => ((Uri)value).ToString();
    }
}