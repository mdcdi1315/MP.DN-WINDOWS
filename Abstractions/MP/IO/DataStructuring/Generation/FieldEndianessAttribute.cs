
using System;

namespace MP.IO.DataStructuring.Generation
{
    /// <summary>Specifies constants related to the CPU's endianess.</summary>
    /// <remarks>This is used by the <see cref="FieldEndianessAttribute"/> class.</remarks>
    public enum Endianess : System.Byte
    {
        /// <summary>Little endianess should be used for the given numeric field.</summary>
        Little,
        /// <summary>Big endianess should be used for the given numeric field.</summary>
        Big
    }

    /// <summary>
    /// Provides the option to specify the endianess for a given numeric field.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, Inherited = false, AllowMultiple = false)]
    public sealed class FieldEndianessAttribute : Attribute
    {
        private readonly Endianess endianess;

        /// <summary>Initializes a new instance of the <see cref="FieldEndianessAttribute"/> class.</summary>
        /// <param name="desired_endianess">The endianess for the attributed field.</param>
        public FieldEndianessAttribute(Endianess desired_endianess) => endianess = desired_endianess;

        /// <summary>The endianess to use for the applied field.</summary>
        public Endianess Endianess => endianess;
    }
}