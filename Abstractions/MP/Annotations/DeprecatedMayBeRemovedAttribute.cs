
using System;

namespace MP.Annotations
{
    /// <summary>
    /// Specified to any code element to indicate that it is deprecated and it may be removed in the future.
    /// </summary>
    [AttributeUsage(AttributeTargets.Struct | AttributeTargets.Enum | AttributeTargets.Class | AttributeTargets.Method | AttributeTargets.Property | AttributeTargets.Event | AttributeTargets.Interface | AttributeTargets.Constructor | AttributeTargets.Field , AllowMultiple = false)]
    public sealed class DeprecatedMayBeRemovedAttribute : Attribute { }
}