

using System.Numerics;

namespace MP.Serialization.Constraints
{
    /// <summary>
    /// Defines that the specified number must be a positive number. <br />
    /// This only applies for primitives that are signed.
    /// </summary>
    /// <typeparam name="T">The numeric type to apply the constraint on.</typeparam>
    public class NumberMustBePositiveAttribute<T> : GenericSerializationConstraintAttribute<T>
        where T : INumber<T>
    {
        /// <inheritdoc /> 
        public sealed override SerializedFieldType[] AppliesTo => new[] {
            SerializedFieldType.SByte,
            SerializedFieldType.Int16,
            SerializedFieldType.Int32,
            SerializedFieldType.Int64,
            SerializedFieldType.Single,
            SerializedFieldType.Double
        };

        /// <inheritdoc /> 
        public override ConstraintApplicationTime AppliesDuring => ConstraintApplicationTime.Both;

        /// <inheritdoc /> 
        public override bool IsSatisfied(T value, out SerializationException exception)
        {
            exception = null;
            return T.IsPositive(value);
        }
    }
}