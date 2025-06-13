
using System;

namespace MP.ExtensibilitySystem
{
    /// <summary>
    /// Marking a class with this attribute makes it valid to be loaded as an extensible module loaded by the Extensibility system. <br />
    /// Additionally, you need to mark the assembly itself as a Extensibility system extension - see the <see cref="ManagedExtensionAssemblyAttribute"/> for more info.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class , AllowMultiple = false , Inherited = false)]
    public sealed class ManagedExtensibilityModuleLoaderClassAttribute : Attribute {}
}