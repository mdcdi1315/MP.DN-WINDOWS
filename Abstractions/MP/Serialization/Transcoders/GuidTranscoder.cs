

using System;
using System.Diagnostics.CodeAnalysis;

namespace MP.Serialization.Transcoders
{
    /// <summary>
    /// Defines a transcoder for <see cref="Guid"/> instances.
    /// </summary>
    public sealed class GuidTranscoder : ITypeTranscoder
    {
        /// <inheritdoc />
        public SerializedFieldType SerializedType => SerializedFieldType.String;

        /// <inheritdoc />
        public Type ActualType => typeof(Guid);

        /// <inheritdoc />
        public bool StrictTypeMatch => true; // We need strict type matching since structures are implicitly sealed class types.

        /// <inheritdoc />
        [return: NotNull]
        public object Decode([DisallowNull] object value, [DisallowNull] Type actual)
        {
            try {
                return Guid.Parse(value.ToString());
            } catch (FormatException ex) {
                throw new SerializationException("Cannot decode the specified GUID." , ex);
            }
        }

        /// <inheritdoc />
        public object Encode([DisallowNull] object value) => ((Guid)value).ToString(); // I am unboxing on purpose to throw ICE's on user-defined serialization managers.
    }
}
