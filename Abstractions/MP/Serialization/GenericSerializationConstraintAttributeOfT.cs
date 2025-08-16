

using System;
using MP.Annotations.CodeAnalysis;

namespace MP.Serialization
{
    /// <summary>
    /// Defines the base class for serialization constraints with generics in the attribute definitions.
    /// </summary>
    /// <typeparam name="T">The type of the field that the constraint is applied to.</typeparam>
    public abstract class GenericSerializationConstraintAttribute<T> : SerializationConstraintAttribute
    {
        /// <inheritdoc />
        public sealed override bool IsSatisfied(object value, out SerializationException exception)
        {
            try {
                return IsSatisfied((T)value, out exception);
            } catch (InvalidCastException ice) {
                exception = new("Satisfaction test failed due to an internal exception.", ice);
                return false;
            }
        }

        /// <summary>
        /// Tests whether the current constraint is satisfied for a value of type <typeparamref name="T"/>. Usually this is to be retrieved by the attributed field.
        /// </summary>
        /// <param name="value">The value to test whether it is satisfied against something.</param>
        /// <param name="exception">A <see cref="SerializationException"/> object describing the reason why serialization failed, or null if no reason could be obtained for the value.</param>
        /// <returns><see langword="true"/> when the constraint is satisfied; otherwise, <see langword="false"/> and maybe an exception explaining the reason why the constraint failed at the <paramref name="exception"/> parameter.</returns>
        [MustNotReportException]
        public abstract System.Boolean IsSatisfied(T value, out SerializationException exception);
    }
}