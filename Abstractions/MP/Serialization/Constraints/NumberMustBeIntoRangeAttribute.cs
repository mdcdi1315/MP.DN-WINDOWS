

using System.Numerics;

namespace MP.Serialization.Constraints
{
    /// <summary>
    /// A constraint specifying that the value of the attributed field must be into a range by the values specified in the attribute's constructor.
    /// </summary>
    /// <typeparam name="T">The type of the number to test whether it is into range.</typeparam>
    public sealed class NumberMustBeIntoRangeAttribute<T> : GenericSerializationConstraintAttribute<T>
        where T : INumber<T>
    {
        private T minimum_inclusive;
        private T maximum_inclusive;

        /// <summary>
        /// Creates a new instance of the <see cref="NumberMustBeIntoRangeAttribute{T}"/> class, specifying the range of valid values that the specified number can have.
        /// </summary>
        /// <param name="minimum_inclusive">The minimum inclusive value for which the current constraint will pass.</param>
        /// <param name="maximum_inclusive">The maximum inclusive value for which the current constraint will pass.</param>
        public NumberMustBeIntoRangeAttribute(T minimum_inclusive, T maximum_inclusive)
        {
            this.minimum_inclusive = minimum_inclusive;
            this.maximum_inclusive = maximum_inclusive;
        }

        /// <inheritdoc />
        public override SerializedFieldType[] AppliesTo => new[] {
            SerializedFieldType.Byte,
            SerializedFieldType.SByte,
            SerializedFieldType.Int16,
            SerializedFieldType.UInt16,
            SerializedFieldType.Int32,
            SerializedFieldType.UInt32,
            SerializedFieldType.Int64,
            SerializedFieldType.UInt64,
            SerializedFieldType.Single,
            SerializedFieldType.Double
        };

        /// <inheritdoc />
        public override ConstraintApplicationTime AppliesDuring => ConstraintApplicationTime.Both;

        /// <inheritdoc />
        public override bool IsSatisfied(T value, out SerializationException exception)
        {
            exception = null;
            return value >= minimum_inclusive && value <= maximum_inclusive;
        }
    }
}