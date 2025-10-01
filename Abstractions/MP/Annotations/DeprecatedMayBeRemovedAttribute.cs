
using System;
using System.Diagnostics.CodeAnalysis;

namespace MP.Annotations
{
    /// <summary>
    /// Specified to any code element to indicate that it is deprecated and it may be removed in the future.
    /// </summary>
    [AttributeUsage(AttributeTargets.Struct | AttributeTargets.Enum | AttributeTargets.Class | AttributeTargets.Method | AttributeTargets.Property | AttributeTargets.Event | AttributeTargets.Interface | AttributeTargets.Constructor | AttributeTargets.Field, AllowMultiple = false)]
    public sealed class DeprecatedMayBeRemovedAttribute : Attribute
    {
        private readonly System.String versionwhen;

        /// <summary>
        /// Initializes an empty instance of the <see cref="DeprecatedMayBeRemovedAttribute"/> class instance.
        /// </summary>
        public DeprecatedMayBeRemovedAttribute() => versionwhen = null;

        /// <summary>
        /// Initializes a new <see cref="DeprecatedMayBeRemovedAttribute"/> class instance, specifying in which version of the associated library the API will be removed.
        /// </summary>
        /// <param name="removedwhen">The exact version when the API will be removed at.</param>
        public DeprecatedMayBeRemovedAttribute(System.String removedwhen) => versionwhen = removedwhen;

        /// <summary>
        /// Signifies the version when the linked API will be removed. <br />
        /// May be <see langword="null"/> indicating that there are no intentions to remove the attributed element, or that there is not decided yet a version to be removed.
        /// </summary>
        [MaybeNull]
        public System.String APIRemovalVersion => versionwhen;
    }
}