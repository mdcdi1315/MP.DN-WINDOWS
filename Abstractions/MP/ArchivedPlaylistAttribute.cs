

using System;

namespace MP
{
    /// <summary>
    /// Defines flags that modify the behavior of an <see cref="ArchivedPlaylistAttribute"/> instance.
    /// </summary>
    [Flags]
    public enum ArchivedPlaylistAttributeFlags : System.UInt16
    {
        /// <summary>
        /// No additional flags are defined for this attribute.
        /// </summary>
        None = 0,
        /// <summary>
        /// The attribute is read-only. 
        /// This means that it's value can only be inserted before construction. <br />
        /// Additionally, the attribute flags cannot be modified after this is specified.
        /// </summary>
        ReadOnly = 0x0001
    }

    /// <summary>
    /// Defines an archived playlist attribute. <br />
    /// Such attributes are additional informational data that the user does add to an archived playlist. <br />
    /// There are no factual restrictions on what names and values an attribute can have. <br />
    /// However, the Music Player does reserve some attribute names for it's own use...
    /// </summary>
    public sealed class ArchivedPlaylistAttribute
    {
        private ArchivedPlaylistAttributeFlags flags;
        private System.String name, value;

        /// <summary>
        /// Creates an empty instance of the <see cref="ArchivedPlaylistAttribute"/> class.
        /// </summary>
        public ArchivedPlaylistAttribute()
        {
            flags = ArchivedPlaylistAttributeFlags.None;
            name = null;
            value = null;
        }

        /// <summary>
        /// Creates an instance of the <see cref="ArchivedPlaylistAttribute"/> class from the specified
        /// name and value.
        /// </summary>
        /// <param name="name">The name of the new attribute.</param>
        /// <param name="value">The value of the new attribute.</param>
        public ArchivedPlaylistAttribute(System.String name , System.String value) : this()
        {
            this.name = name;
            this.value = value;
        }

        private void EnsureNotReadOnly()
        {
            if (flags.HasFlag(ArchivedPlaylistAttributeFlags.ReadOnly)) {
                throw new MP.ExceptionSystem.ReadOnlyArchiveAttributeException(name);
            }
        }

        /// <summary>
        /// Gets or sets the flags currently defined on the attribute.
        /// </summary>
        /// <exception cref="ExceptionSystem.ReadOnlyArchiveAttributeException">The attribute is read-only and cannot be modified.</exception>
        public ArchivedPlaylistAttributeFlags Flags
        {
            get => flags;
            set {
                EnsureNotReadOnly();
                flags = value;
            }
        }

        /// <summary>
        /// Gets or sets the name of the current attribute. <br />
        /// Even <see langword="null"/> is valid for an archive attribute name.
        /// </summary>
        /// <exception cref="ExceptionSystem.ReadOnlyArchiveAttributeException">The attribute is read-only and cannot be modified.</exception>
        public System.String Name
        {
            get => name;
            set {
                EnsureNotReadOnly();
                name = value;
            }
        }

        /// <summary>
        /// Gets or sets the value of the current attribute.
        /// </summary>
        /// <exception cref="ExceptionSystem.ReadOnlyArchiveAttributeException">The attribute is read-only and cannot be modified.</exception>
        public System.String Value
        {
            get => value;
            set {
                EnsureNotReadOnly();
                this.value = value;
            }
        }
    }
}