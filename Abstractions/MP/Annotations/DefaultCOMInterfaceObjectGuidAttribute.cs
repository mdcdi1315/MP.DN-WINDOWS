
using System;
using System.Runtime.Versioning;

namespace MP.Annotations
{
    /// <summary>
    /// Specified to a COM interface to indicate that an instance of the interface is known and can be created. <br />
    /// To create such, this attribute must mark that interface and specify the GUID that is required. <br />
    /// Then, an API must be implemented on the internals of the app to specify how this can be used.<br />
    /// Although that instances of a COM interface may be created from different GUID's, only one must be specified
    /// for avoiding ambiguous states when this attribute will be read.
    /// </summary>
    [SupportedOSPlatform("windows")]
    [AttributeUsage(AttributeTargets.Interface, AllowMultiple = false)]
    public sealed class DefaultCOMInterfaceObjectGuidAttribute : Attribute
    {
        private System.String guid;

        /// <summary>
        /// Creates a new instance of the <see cref="DefaultCOMInterfaceObjectGuidAttribute"/>,
        /// specifying the GUID that can create an instance of the interface where this attribute is applied to.
        /// </summary>
        /// <param name="guid">The GUID of the COM CoClass that can create an instance of the interface where this attribute is applied to.</param>
        public DefaultCOMInterfaceObjectGuidAttribute(System.String guid) => this.guid = guid;

        /// <summary>
        /// The GUID that identifies the object implementing the interface
        /// </summary>
        public System.String ObjectIdentifier => guid;
    }
}