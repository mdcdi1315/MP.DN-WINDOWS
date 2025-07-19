
using System;

namespace MP.Annotations
{
    /// <summary>
    /// Specified to any executable code element or a class to indicate that it heavily depends on the <see cref="SystemInfo"/> class and it's layering logic.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Constructor | AttributeTargets.Method | AttributeTargets.Property | AttributeTargets.Event , AllowMultiple = false , Inherited = false)]
    public sealed class RequiresNativeLayerAttribute : Attribute { }
}
