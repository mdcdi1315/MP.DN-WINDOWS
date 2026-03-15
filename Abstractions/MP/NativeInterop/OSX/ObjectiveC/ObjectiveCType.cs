using System;
using MP.Annotations.CodeAnalysis;
using System.Diagnostics.CodeAnalysis;

namespace MP.NativeInterop.OSX.ObjectiveC
{
    /// <summary>
    /// Defines type information of a single type in the Objective-C type system.
    /// </summary>
    public abstract class ObjectiveCType
    {
        /// <summary>
        /// Gets the name of the current Objective-C type.
        /// </summary>
        public abstract string Name { get; }

        /// <summary>
        /// Gets the super type of the current Objective-C type.
        /// </summary>
        public abstract ObjectiveCType BaseType { get; }

        /// <summary>
        /// Gets all the methods declared and/or inherited by the current Objective-C type.
        /// </summary>
        /// <param name="instance">A value whether to get instance or static methods.</param>
        /// <returns>An array <see cref="ObjectiveCMethod"/> instances describing the methods of the current type.</returns>
        public abstract ObjectiveCMethod[] GetMethods(bool instance = true);
        
        /// <summary>Gets all the protocols adopted by the current type.</summary>
        /// <returns>The protocols adopted by the current type.</returns>
        public abstract ObjectiveCProtocol[] GetAdoptedProtocols();

        /// <summary>
        /// Defines a way to compare the current Objective-C type against the specified object. <br />
        /// This method cannot be overriden.
        /// </summary>
        /// <param name="obj">The object to compare against this instance.</param>
        /// <returns>A value whether <paramref name="obj"/> and the current instance are equal objects.</returns>
        public sealed override bool Equals(object obj)
        {
            if (ReferenceEquals(this, obj)) {
                return true;
            } else if (this is ObjectiveCClass c1 && obj is ObjectiveCClass c2) {
                return c1.Equals(c2);
            } else if (this is ObjectiveCProtocol p1 && obj is ObjectiveCProtocol p2) {
                return p1.Equals(p2);
            } else {
                return false;
            }
        }

        /// <summary>Gets a string that describes the current Objective-C type.</summary>
        /// <returns>Some details of this Objective-C type instance.</returns>
        public override string ToString() => $"ObjectiveCType {{ FullName = {Name} }}";

        /// <summary>
        /// This method is not supported and will always throw <see cref="PlatformNotSupportedException"/>.
        /// </summary>
        /// <exception cref="PlatformNotSupportedException">Objective-C does not honour class comparisons by hash codes.</exception>
        [DoesNotReturn]
        [Throws(typeof(PlatformNotSupportedException))]
        public sealed override int GetHashCode() => throw new PlatformNotSupportedException("Disallowed by inherent Objective-C runtime platform rules.");
    }
}