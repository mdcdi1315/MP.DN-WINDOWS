

using System;
using System.Runtime.Versioning;
using System.Diagnostics.CodeAnalysis;

namespace MP.Annotations
{
    /// <summary>
    /// Attribute defined to all the COM interfaces that are generated and used in the app. <br />
    /// Additionally, it provides the type to use for COM Dispatch services. <br />
    /// An interface using this attribute is typically crafted by the COM source generator.
    /// </summary>
    [SupportedOSPlatform(NativeInterop.Windows.WindowsVersions.NTDDI_VISTA)]
    [AttributeUsage(AttributeTargets.Interface, AllowMultiple = false , Inherited = false)]
    public sealed class COMInterfaceAttribute : Attribute
    {
        private readonly Guid interface_id;

        [AllowNull]
        private readonly Type dispatch_provider;

        /// <summary>
        /// Creates a new instance of the <see cref="COMInterfaceAttribute"/> class from the string that indicates the interface ID that the interface is bound to.
        /// </summary>
        /// <param name="interface_id">The ID of the interface.</param>
        public COMInterfaceAttribute(String interface_id)
        {
            this.interface_id = new(interface_id);
            dispatch_provider = null;
        }

        /// <summary>
        /// Creates a new instance of the <see cref="COMInterfaceAttribute"/> class from the string that indicates the interface ID that the interface is bound to,
        /// as well as the type providing the COM dispatch services for the interface.
        /// </summary>
        /// <param name="interface_id">The ID of the interface.</param>
        /// <param name="dispatch_provider">The type providing COM dispatch services for the COM interface.</param>
        public COMInterfaceAttribute(String interface_id, Type dispatch_provider)
        {
            this.interface_id = new(interface_id);
            this.dispatch_provider = dispatch_provider;
        }

        /// <summary>
        /// Gets the GUID of the attributed COM interface.
        /// </summary>
        public Guid GUID => interface_id;

        /// <summary>
        /// Gets the type that provides COM dispatch services.
        /// </summary>
        [MaybeNull]
        public Type DispatchProvider => dispatch_provider;
    }
}