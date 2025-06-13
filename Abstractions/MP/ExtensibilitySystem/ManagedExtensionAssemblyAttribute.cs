
using System;

namespace MP.ExtensibilitySystem
{
    /// <summary>
    /// Marks an assembly as an Extensibility system extension. <br />
    /// Must be applied for all those assemblies that are supposed to be loaded as extensions.
    /// </summary>
    [AttributeUsage(AttributeTargets.Assembly , AllowMultiple = false , Inherited = false)]
    public sealed class ManagedExtensionAssemblyAttribute : Attribute { }
}