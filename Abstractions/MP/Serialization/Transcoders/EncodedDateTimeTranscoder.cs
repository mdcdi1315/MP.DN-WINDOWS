

using System;
using System.Diagnostics.CodeAnalysis;

namespace MP.Serialization.Transcoders
{
    /// <summary>
    /// Defines a type transcoder for <see cref="DateTime"/> values. <br />
    /// The values are decoded and encoded by the value of the <see cref="DateTime.Ticks"/> property.
    /// </summary>
    public sealed class EncodedDateTimeTranscoder : ITypeTranscoder
    {
        /// <inheritdoc />
        public SerializedFieldType SerializedType => SerializedFieldType.Int64;

        /// <inheritdoc />
        public Type ActualType => typeof(DateTime);

        /// <inheritdoc />
        public bool StrictTypeMatch => true;

        /// <inheritdoc />
        [return: NotNull]
        public object Decode([DisallowNull] object value, [DisallowNull] Type actual)
        {
            try {
                return new DateTime((long)value);
            } catch (ArgumentOutOfRangeException ex) {
                throw new SerializationException("Cannot decode the date-time value." , ex);
            }
        }

        /// <inheritdoc />
        public object Encode([DisallowNull] object value) => ((DateTime)value).Ticks;
    }
}