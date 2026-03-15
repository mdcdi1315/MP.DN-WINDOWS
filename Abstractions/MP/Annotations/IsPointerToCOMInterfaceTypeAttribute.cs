
using System;
using System.Runtime.Versioning;

namespace MP.Annotations
{
    /// <summary>
    /// Marked to a <see langword="void*"/> parameter, return value or structure field to indicate that the pointer is a COM interface, 
    /// and the given type argument in the constructor indicates the COM interface that specifies the .NET bridge.
    /// </summary>
    [SupportedOSPlatform("windows")] // This is Windows-specific.
    [AttributeUsage(AttributeTargets.Parameter | AttributeTargets.ReturnValue | AttributeTargets.Field, AllowMultiple = false)]
    public sealed class IsPointerToCOMInterfaceTypeAttribute : Attribute
    {
        private readonly Type comtype;

        /// <summary>
        /// Creates a new instance of the attribute, specifying the .NET type that represents the marshalled interface.
        /// </summary>
        /// <param name="COMtype">The marshalled COM interface type.</param>
        public IsPointerToCOMInterfaceTypeAttribute(Type COMtype) => comtype = COMtype;

        /// <summary>
        /// The .NET interface that indicates how the void* parameter should be marshalled as.
        /// </summary>
        public Type Interface => comtype;
    }
}