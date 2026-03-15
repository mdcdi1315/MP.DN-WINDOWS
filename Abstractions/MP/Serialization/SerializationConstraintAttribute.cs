
using System;
using MP.Annotations.CodeAnalysis;

namespace MP.Serialization
{
    /// <summary>
    /// Base class for defining a 'serialization constraint'. <br />
    /// These constraints generally enforce additional behavior that a field should have. <br />
    /// This class is a placeholder for other constraint attributes.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, Inherited = true)]
    public abstract class SerializationConstraintAttribute : Attribute
    {
        /// <summary>
        /// Gets a value of the serialized field types that the derived attribute can be applied to. <br />
        /// Note: It is very important that this is set correctly. <br />
        /// During de/serialization, the serialization manager will sort out which attributes can be applied for the field that the reader/writer are currently pointing to. <br />
        /// Thus, it WILL NOT THROW for constraints that cannot be applied to the type of the specified field.
        /// </summary>
        public abstract SerializedFieldType[] AppliesTo { get; }

        /// <summary>
        /// Gets the time that this constraint can be applied to (i.e. read or write time, or both).
        /// </summary>
        public abstract ConstraintApplicationTime AppliesDuring { get; }

        /// <summary>
        /// Tests whether the current constraint is satisfied for a value. Usually this is to be retrieved by the attributed field.
        /// </summary>
        /// <param name="value">The value to test whether it is satisfied against something.</param>
        /// <param name="exception">A <see cref="SerializationException"/> object describing the reason why serialization failed, or null if no reason could be obtained for the value.</param>
        /// <returns><see langword="true"/> when the constraint is satisfied; otherwise, <see langword="false"/> and maybe an exception explaining the reason why the constraint failed at the <paramref name="exception"/> parameter.</returns>
        [MustNotReportException]
        public abstract System.Boolean IsSatisfied(System.Object value, out SerializationException exception);
    }
}