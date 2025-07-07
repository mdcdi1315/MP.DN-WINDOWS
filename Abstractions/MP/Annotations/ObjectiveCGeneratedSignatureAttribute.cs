
using System;
using System.Runtime.Versioning;

namespace MP.Annotations
{
    /// <summary>
    /// Defined to every exposed method or property on a Objective-C interop class to generate bolierplate code for the marked signature.
    /// </summary>
    [Preliminary]
    [SupportedOSPlatform("ios")]
    [SupportedOSPlatform("macos")]
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Property, AllowMultiple = false)]
    public sealed class ObjectiveCGeneratedSignatureAttribute : Attribute
    {
        private String invokename;

        /// <summary>
        /// Constructs a new instance of the <see cref="ObjectiveCGeneratedSignatureAttribute"/> class from the specified name of the interop signature to be generated.
        /// </summary>
        /// <param name="originalname">The original name of the member to be invoked.</param>
        public ObjectiveCGeneratedSignatureAttribute(String originalname = null) => invokename = originalname;

        /// <summary>
        /// The original member name so that the member can be invoked.
        /// </summary>
        public String OriginalMemberName => invokename;
    }
}