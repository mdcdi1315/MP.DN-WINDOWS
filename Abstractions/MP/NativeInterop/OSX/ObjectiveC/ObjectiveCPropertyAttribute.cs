
using System;
using System.Diagnostics.CodeAnalysis;

namespace MP.NativeInterop.OSX.ObjectiveC
{
    /// <summary>
    /// Declares a new property attribute on the specified Objective-C property.
    /// </summary>
    public readonly struct ObjectiveCPropertyAttribute
    {
        /// <summary>
        /// Gets the name of the current Objective-C property.
        /// </summary>
        public readonly string Name;

        /// <summary>
        /// Gets the value of the current Objective-C property. <br />
        /// Can be <see langword="null"/> or empty.
        /// </summary>
        [MaybeNull]
        public readonly string Value;

        /// <summary>
        /// Gets the property that declares this attribute.
        /// </summary>
        public readonly ObjectiveCProperty Property;

        /// <summary>
        /// Constructs a new instance of the <see cref="ObjectiveCPropertyAttribute"/> structure.
        /// </summary>
        /// <param name="property">The property that declares the current property attribute.</param>
        /// <param name="name">The name of the property's attribute.</param>
        /// <param name="value">The value of the property's attribute.</param>
        public ObjectiveCPropertyAttribute(ObjectiveCProperty property, string name, [AllowNull] string value)
        {
            ArgumentNullException.ThrowIfNull(name);
            ArgumentNullException.ThrowIfNull(property);
            Name = name;
            Value = value;
            Property = property;
        }

        /// <summary>Returns a string that describes the current Objective-C property attribute.</summary>
        /// <returns>A string describing the details of the current property attribute.</returns>
        public override string ToString() => $"ObjectiveCPropertyAttribute: {{ Name = {Name}, Value = {Value} }}";
    }
}