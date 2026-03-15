namespace MP.NativeInterop.OSX.ObjectiveC
{
    /// <summary>
    /// Defines an Objective-C class or protocol method.
    /// </summary>
    public abstract class ObjectiveCMethod
    {
        /// <summary>
        /// Gets the name of the current Objective-C method.
        /// </summary>
        public abstract string Name { get; }

        /// <summary>
        /// Gets the Objective-C type that this method originates from.
        /// </summary>
        public abstract ObjectiveCType DeclaringType { get; }

        /// <summary>
        /// Gets the return type of the current Objective-C method, as described by the runtime.
        /// </summary>
        public abstract string ReturnType { get; }

        /// <summary>
        /// Gets the declared arguments of the current Objective-C method, as described by the runtime. <br />
        /// The returned array has in each element the type of the in question argument.
        /// </summary>
        public abstract string[] Arguments { get; }

        /// <summary>
        /// Gets the implementation of the current method.
        /// </summary>
        public abstract IMP Implementation { get; }
    }
}