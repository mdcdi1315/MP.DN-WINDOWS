
using System;
using MP.Annotations.CodeAnalysis;
using System.Diagnostics.CodeAnalysis;

namespace MP.NativeInterop.OSX.ObjectiveC
{
    /// <summary>
    /// Provides the base declaration for Objective-C classes.
    /// </summary>
    [SuppressMessage("Design", "CA1067:Override Object.Equals(object) when implementing IEquatable<T>", Justification = "Sealed Equals implementation ObjectiveCType class")]
    public abstract class ObjectiveCClass : ObjectiveCType, IEquatable<ObjectiveCClass>
    {
        /// <summary>
        /// Gets the version of the current Objective-C class.
        /// </summary>
        public abstract int Version { get; }

        /// <summary>
        /// Gets a value whether the current Objective-C class is a meta-class.
        /// </summary>
        public abstract bool IsMetaClass { get; }

        /// <summary>
        /// Gets the name of the library that the current Objective-C class was originated from.
        /// </summary>
        public abstract string LibraryName { get; }

        /// <summary>Creates a new instance of the current Objective-C type. Note that the instance is uninitialized.</summary>
        /// <param name="extra_bytes">Extra bytes to allocate at the end of the memory block.</param>
        /// <returns>A typeless instance of the <see cref="IObjectiveCObject"/> interface.</returns>
        [Throws]
        [return: NotNull]
        public abstract IObjectiveCObject AllocateInstance(ulong extra_bytes = 0UL);

        /// <summary>
        /// Defines a function that checks whether the current and the specified class are the same classes.
        /// </summary>
        /// <param name="other">The other class instance to compare against.</param>
        /// <returns>A value whether the two Objective-C classes are equal.</returns>
        public abstract bool Equals(ObjectiveCClass other);

        /// <summary>Gets a string that describes the current Objective-C class.</summary>
        /// <returns>Some details of this Objective-C class instance.</returns>
        public override string ToString() => $"ObjectiveCClass {{ FullName = {LibraryName}#{Name}, Version = {Version} }}";
    }
}