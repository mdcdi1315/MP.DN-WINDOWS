

using System;
using System.Diagnostics.CodeAnalysis;

namespace MP.Serialization.Transcoders
{
    /// <summary>
    /// Defines a type transcoder for <see cref="TimeSpan"/> values.
    /// </summary>
    public sealed class TimeSpanTranscoder : ITypeTranscoder
    {
        /// <inheritdoc />
        public SerializedFieldType SerializedType => SerializedFieldType.String;

        /// <inheritdoc />
        public Type ActualType => typeof(TimeSpan);

        /// <inheritdoc />
        public bool StrictTypeMatch => true;

        /// <inheritdoc />
        [return: NotNull]
        public object Decode([DisallowNull] object value, [DisallowNull] Type actual)
        {
            try {
                return TimeSpan.Parse(value.ToString() , System.Globalization.CultureInfo.InvariantCulture);
            } catch (FormatException fmt) {
                throw new SerializationException("The timespan value failed to be decoded.", fmt);
            }
        }

        /// <inheritdoc />
        public object Encode([DisallowNull] object value) => ((TimeSpan)value).ToString("c", System.Globalization.CultureInfo.InvariantCulture);
    }
}