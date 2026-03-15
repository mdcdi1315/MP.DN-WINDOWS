namespace MP.NativeInterop.OSX.ObjectiveC
{
    /// <summary>
    /// Defines an Objective-C class or protocol property.
    /// </summary>
    public abstract class ObjectiveCProperty
    {
        /// <summary>
        /// Gets the name of the current Objective-C property.
        /// </summary>
        public abstract string Name { get; }

        /// <summary>
        /// Gets the Objective-C type that this property originates from.
        /// </summary>
        public abstract ObjectiveCType DeclaringType { get; }

        /// <summary>
        /// Gets the Objective-C attributes declared in the current property.
        /// </summary>
        public abstract ObjectiveCPropertyAttribute[] Attributes { get; }
    }
}