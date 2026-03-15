

using System;
using System.Diagnostics.CodeAnalysis;

namespace MP.Serialization.Transcoders
{
    /// <summary>
    /// Defines a type transcoder for <see cref="DateTime"/> values.
    /// </summary>
    public sealed class DateTimeTranscoder : ITypeTranscoder
    {
        /// <inheritdoc />
        public SerializedFieldType SerializedType => SerializedFieldType.String;

        /// <inheritdoc />
        public Type ActualType => typeof(DateTime);

        /// <inheritdoc />
        public bool StrictTypeMatch => true;

        /// <inheritdoc />
        [return: NotNull]
        public object Decode([DisallowNull] object value, [DisallowNull] Type actual)
        {
            try {
                return DateTime.Parse(value.ToString() , System.Globalization.CultureInfo.InvariantCulture);
            } catch (FormatException fmt) {
                throw new SerializationException("The date-time value failed to be decoded." , fmt);
            }
        }

        /// <inheritdoc />
        public object Encode([DisallowNull] object value) => ((DateTime)value).ToString(System.Globalization.CultureInfo.InvariantCulture);
    }
}