
using System;

namespace MP.Annotations
{
    /// <summary>
    /// Indicates that a class represents a native communication layer. <br />
    /// This is, for example, the entire Interop class defined in the backends, or the <see cref="SystemInfo"/> class itself.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public sealed class NativeLayerAttribute : Attribute { }
}