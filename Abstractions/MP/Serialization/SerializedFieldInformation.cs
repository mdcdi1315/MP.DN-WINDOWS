

using System;

namespace MP.Serialization
{
    /// <summary>
    /// Defines additional serialization information for the specified field. <br />
    /// This class can be derived so that special readers and writers provide more information about the serialized field, if applicable.
    /// </summary>
    public class SerializedFieldInformation
    {
        private System.String name;
        private SerializedFieldType type;

        /// <summary>
        /// Directly instantiates a new <see cref="SerializedFieldInformation"/> class. <br />
        /// This constructor is only meant to be used by derived classes.
        /// </summary>
        protected SerializedFieldInformation() { }

        /// <summary>
        /// Creates a new <see cref="SerializedFieldInformation"/> class, defining the name of the field and it's type as well.
        /// </summary>
        /// <param name="name">The name of the field to be decoded.</param>
        /// <param name="sft">The type of the field to be decoded.</param>
        public SerializedFieldInformation(System.String name, SerializedFieldType sft)
        {
            ArgumentException.ThrowIfNullOrEmpty(name);
            this.name = name;
            type = sft;
        }

        /// <summary>
        /// Retrieves the name of the serialzed field.
        /// </summary>
        public System.String Name => name;

        /// <summary>
        /// Retrieves the type of the serialized field.
        /// </summary>
        public SerializedFieldType Type => type;
    }
}