
using System;

namespace MP.Annotations
{
    /// <summary>
    /// Indicates that a class, interface, structure or an entire assembly is into development stage and it's features may break at any time.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Interface | AttributeTargets.Assembly, AllowMultiple = false)]
    public sealed class PreliminaryAttribute : Attribute { }
}