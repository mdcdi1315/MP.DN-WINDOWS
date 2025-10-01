
using System;

namespace MP.Serialization
{
    /// <summary>
    /// A convenience interface for accessing the <see cref="OptionalFieldAttribute{T}"/> data directly.
    /// </summary>
    internal interface IOptionalFieldAttributeDataAccessor
    {
        public System.Object GetValue();
    }

    /// <summary>
    /// Defines an optional field in a serializable class. <br />
    /// This attribute can only be applied to primitive types due to .NET attribute restrictions. <br />
    /// Note additionally that this attribute cannot be specified with the <see cref="DerivedTypeBindingAttribute"/> since it can cause ambiguation issues.
    /// </summary>
    /// <typeparam name="T">The field type providing the default value.</typeparam>
    [AttributeUsage(AttributeTargets.Field , AllowMultiple = false)]
    public sealed class OptionalFieldAttribute<T> : Attribute , IOptionalFieldAttributeDataAccessor
    {
        private readonly T value;

        /// <summary>
        /// Initializes a new instance of the <see cref="OptionalFieldAttribute{T}"/> class, providing the default value for the field in case that the field declaration is not found.
        /// </summary>
        /// <param name="value">The default value to use when the field is not found.</param>
        public OptionalFieldAttribute(T value) => this.value = value;

        /// <summary>
        /// The default value to be used by the serialization manager when the field where this attribute is applied to is not found in the record.
        /// </summary>
        public T Value => value;

        object IOptionalFieldAttributeDataAccessor.GetValue() => value;
    }
}