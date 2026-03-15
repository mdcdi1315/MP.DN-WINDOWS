
using System;
using System.Diagnostics.CodeAnalysis;

namespace MP.NativeInterop.OSX.ObjectiveC
{
    /// <summary>
    /// Defines an Objective-C protocol type. 
    /// Extends from the <see cref="ObjectiveCType"/> class.
    /// </summary>
    [SuppressMessage("Design", "CA1067", Justification = "Sealed Equals implementation ObjectiveCType class")]
    public abstract class ObjectiveCProtocol : ObjectiveCType, IEquatable<ObjectiveCProtocol>
    {
        /// <summary>
        /// Defines a function that checks whether the current and the specified protocol are the same protocols.
        /// </summary>
        /// <param name="other">The other protocol instance to compare against.</param>
        /// <returns>A value whether the two Objective-C protocols are equal.</returns>
        public abstract bool Equals([AllowNull] ObjectiveCProtocol other);
    }
}