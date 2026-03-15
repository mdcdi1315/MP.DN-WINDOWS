using System;
using System.Diagnostics.CodeAnalysis;

namespace MP.Serialization.Transcoders
{
    /// <summary>
    /// Defines a type transcoder for <see cref="Version"/> instances.
    /// </summary>
    public sealed class VersionTranscoder : ITypeTranscoder
    {
        /// <inheritdoc />
        public SerializedFieldType SerializedType => SerializedFieldType.String;

        /// <inheritdoc />
        public Type ActualType => typeof(Version);

        /// <inheritdoc />
        public bool StrictTypeMatch => true; // It is a sealed class, no meaning to search for deriving classes

        /// <inheritdoc />
        [return: NotNull]
        public object Decode([DisallowNull] object value, [DisallowNull] Type actual)
        {
            try {
                return Version.Parse(value.ToString());
            } catch (Exception ex) {
                throw new SerializationException($"Cannot decode the specified value ('{value}') into a Version instance." , ex);
            }
        }

        /// <inheritdoc />
        public object Encode([DisallowNull] object value) => ((Version)value).ToString();
    }
}
