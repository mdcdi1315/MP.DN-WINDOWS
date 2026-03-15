

using System;
using System.Diagnostics.CodeAnalysis;

namespace MP.Serialization.Transcoders
{
    /// <summary>
    /// Defines a type transcoder for <see cref="TimeSpan"/> values. <br />
    /// The values are decoded and encoded by the value of the <see cref="TimeSpan.Ticks"/> property.
    /// </summary>
    public sealed class EncodedTimeSpanTranscoder : ITypeTranscoder
    {
        /// <inheritdoc />
        public SerializedFieldType SerializedType => SerializedFieldType.Int64;

        /// <inheritdoc />
        public Type ActualType => typeof(TimeSpan);

        /// <inheritdoc />
        public bool StrictTypeMatch => true;

        /// <inheritdoc />
        [return: NotNull]
        public object Decode([DisallowNull] object value, [DisallowNull] Type actual) => new TimeSpan((long)value);

        /// <inheritdoc />
        public object Encode([DisallowNull] object value) => ((TimeSpan)value).Ticks;
    }
}