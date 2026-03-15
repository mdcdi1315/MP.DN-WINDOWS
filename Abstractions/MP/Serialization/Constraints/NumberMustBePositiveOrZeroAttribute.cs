

using System.Numerics;

namespace MP.Serialization.Constraints
{
    /// <summary>
    /// A constraint specifying that the number in the attributed field must be positive or zero.
    /// </summary>
    /// <typeparam name="T">The numeric type to test the constraint against.</typeparam>
    public sealed class NumberMustBePositiveOrZeroAttribute<T> : NumberMustBePositiveAttribute<T>
        where T : INumber<T>
    {
        /// <inheritdoc />
        public override bool IsSatisfied(T value, out SerializationException exception)
        {
            exception = null;
            return T.IsZero(value) || T.IsPositive(value);
        }
    }
}