


using System;
using System.Diagnostics.CodeAnalysis;

namespace MP.Annotations
{
    /// <summary>
    /// Defined to a COM interface to request to the MP COM marshalling generator to run on the interface. <br />
    /// The interface must have been declared as <see langword="partial"/>, otherwise source merging will fail.
    /// </summary>
    [AttributeUsage(AttributeTargets.Interface, AllowMultiple = false, Inherited = false)]
    public sealed class COMInterfaceGeneratorAttribute : Attribute
    {
        private readonly string guid;

        [AllowNull]
        private readonly Type type;

        /// <summary>
        /// Initializes a new instance of the <see cref="COMInterfaceGeneratorAttribute"/> class.
        /// </summary>
        /// <param name="guid">The interface ID of the attributed interface.</param>
        public COMInterfaceGeneratorAttribute(string guid) => this.guid = guid;

        /// <summary>
        /// Initializes a new instance of the <see cref="COMInterfaceGeneratorAttribute"/> class.
        /// </summary>
        /// <param name="guid">The interface ID of the attributed interface.</param>
        /// <param name="base_interface_type">A type object to the interface you wish to be used as the base interface type.</param>
        public COMInterfaceGeneratorAttribute(string guid, Type base_interface_type)
        {
            this.guid = guid;
            type = base_interface_type;
        }

        /// <summary>
        /// Gets the interface ID of the COM interface.
        /// </summary>
        public string GUID => guid;

        /// <summary>
        /// 
        /// </summary>
        public Type BaseInterfaceType => type;
    }
}