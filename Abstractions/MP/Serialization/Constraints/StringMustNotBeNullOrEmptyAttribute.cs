

using System;

namespace MP.Serialization.Constraints
{
    /// <summary>
    /// Defines a constraint for string types, which does not allow any string fields to be null or empty during read or write.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = false)]
    public class StringMustNotBeNullOrEmptyAttribute : SerializationConstraintAttribute
    {
        /// <inheritdoc />
        public sealed override SerializedFieldType[] AppliesTo => new[] { SerializedFieldType.String };

        /// <inheritdoc />
        public override ConstraintApplicationTime AppliesDuring => ConstraintApplicationTime.Both;

        /// <inheritdoc />
        public override bool IsSatisfied(object value, out SerializationException exception)
        {
            if (value is null || (value is System.String str && str == System.String.Empty))
            {
                exception = new("Specified string field was null or empty while this is not allowed.");
                return false;
            }
            exception = null;
            return true;
        }
    }
}