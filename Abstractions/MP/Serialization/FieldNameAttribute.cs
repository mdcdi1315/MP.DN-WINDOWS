

using System;

namespace MP.Serialization
{
    /// <summary>
    /// Specifies the name of the specified field, when passing the serialization barrier.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
    public sealed class FieldNameAttribute : Attribute
    {
        private System.String name;

        /// <summary>
        /// Constructs a new instance of the <see cref="FieldNameAttribute"/>, specifying the name of the field when serialized.
        /// </summary>
        /// <param name="name">The name of the serialized field, bound to the specified .NET field of a class.</param>
        public FieldNameAttribute(System.String name) => this.name = name;

        /// <summary>
        /// Gets the name of the field in serialized data.
        /// </summary>
        public System.String FieldName => name;
    }
}