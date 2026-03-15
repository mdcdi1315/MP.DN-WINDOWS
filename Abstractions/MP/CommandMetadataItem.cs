
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace MP
{
    /// <summary>
    /// A command metadata item type. <br />
    /// The types provided by this enum are common values used by the Music Player Implementation; 
    /// you should add also your own constant values in a static class and cast to this enum type.
    /// </summary>
    public enum CommandMetadataItemType : System.Byte
    {
        /// <summary>
        /// Dummy value indicating that this metadata item does not belong in any metadata type.
        /// </summary>
        None = 0,
        /// <summary>
        /// The item is a list view column.
        /// </summary>
        Column,
        /// <summary>
        /// The item is a list view element.
        /// </summary>
        Element,
        /// <summary>
        /// The item is an image for a single <see cref="Element"/> item.
        /// </summary>
        Image
    }

    /// <summary>
    /// Defines a single RCU Engine command metadata item. <br />
    /// May be also extended.
    /// </summary>
    public class CommandMetadataItem : IAttributeable
    {
        private System.String name;
        private System.Object value;
        private CommandMetadataItemType type;
        private Dictionary<System.String, System.Object> attributes;

        /// <summary>
        /// Initializes a default instance of the <see cref="CommandMetadataItem"/> class.
        /// </summary>
        protected CommandMetadataItem() 
        {
            type = CommandMetadataItemType.None;
            attributes = new(5);
        }

        /// <summary>
        /// Creates a new <see cref="CommandMetadataItem"/> from the specified name and value to bind to this item.
        /// </summary>
        /// <param name="name">The name of the new item.</param>
        /// <param name="value">The value of the new item.</param>
        public CommandMetadataItem(System.String name, System.Object value) : this()
        {
            this.name = name;
            this.value = value;
        }

        /// <summary>
        /// Gets or sets the name of this metadata item.
        /// </summary>
        public System.String Name
        {
            get => name;
            set => name = value;
        }

        /// <summary>
        /// Gets or sets the value of this metadata item.
        /// </summary>
        public System.Object Value
        {
            get => value;
            set => this.value = value;
        }

        /// <summary>
        /// Defines a classification for this metadata item. Optional.
        /// </summary>
        public CommandMetadataItemType Type
        {
            get => type;
            set => type = value;
        }
    
        /// <inheritdoc />
        public void SetAttribute(System.String name , System.Object value)
        {
            if (System.String.IsNullOrEmpty(name)) { throw new ArgumentNullException(nameof(name), "Attribute name must not be the empty string."); }
            attributes[name] = value;
        }

        /// <inheritdoc />
        public bool TryGetAttribute(string attribute, [MaybeNullWhen(true)] out object value)
        {
            if (System.String.IsNullOrEmpty(attribute)) { throw new ArgumentNullException(nameof(attribute), "Attribute name must not be the empty string."); }
            return attributes.TryGetValue(attribute, out value);
        }
    }
}