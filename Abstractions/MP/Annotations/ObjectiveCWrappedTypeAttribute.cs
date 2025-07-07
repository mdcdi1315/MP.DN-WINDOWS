
using System;
using System.Runtime.Versioning;

namespace MP.Annotations
{
    /// <summary>
    /// For the OSX Objective-C interop system. <br />
    /// This attribute defines the Objective-C type name to be wrapped, plus it marks that type to be processed by the Objective-C interop generator.  
    /// </summary>
    [Preliminary]
    [SupportedOSPlatform("ios")]
    [SupportedOSPlatform("macos")]
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface, AllowMultiple = false)]
    public sealed class ObjectiveCWrappedTypeAttribute : Attribute
    {
        private String originaltype;

        /// <summary>
        /// Constructs a new instance of the <see cref="ObjectiveCWrappedTypeAttribute"/> class, specifying the original type name of the imported class.
        /// </summary>
        /// <param name="name">The original type name.</param>
        public ObjectiveCWrappedTypeAttribute(String name) => originaltype = name;

        /// <summary>
        /// Gets the original type name of the imported Objective-C class.
        /// </summary>
        public String TypeName => originaltype;
    }
}