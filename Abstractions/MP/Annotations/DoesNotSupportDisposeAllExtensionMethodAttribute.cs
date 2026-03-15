
using System;

namespace MP.Annotations
{
    /// <summary>
    /// Marker attribute specified to a type that implements the <see cref="System.Collections.Generic.IEnumerable{T}"/> interface 
    /// and T is an extendant of the <see cref="IDisposable"/> interface. <br />
    /// It is applied for cases where due to how the type is coded, using the <see cref="Collections.CollectionExtensions.DisposeAll{T}(System.Collections.Generic.IEnumerable{T})"/> could destroy the type's encapsulation. <br />
    /// When the aforementioned method is executing and this attribute is applied to the IEnumerable implementation, the method will throw an exception indicating that is not supported. 
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, AllowMultiple = false , Inherited = true)]
    public sealed class DoesNotSupportDisposeAllExtensionMethodAttribute : Attribute { }
}