
using System;
using MP.Utilities;
using MP.Annotations.CodeAnalysis;
using System.Diagnostics.CodeAnalysis;

namespace MP.Resources
{
    /// <summary>
    /// Defines the placeholder for resource entries.
    /// </summary>
    public readonly struct ResourceEntry : INullable
    {
        private readonly string name;
        private readonly object value;

        /// <summary>
        /// Constructs an empty resource entry. <br />
        /// (That is, an invalid resource entry.)
        /// </summary>
        public ResourceEntry()
        {
            name = System.String.Empty;
            value = null;
        }

        /// <summary>
        /// Constructs a new <see cref="ResourceEntry"/> from the specified resource name and resource value.
        /// </summary>
        /// <param name="name">The name of the resource.</param>
        /// <param name="value">The value of the resource. Can be null as well.</param>
        /// <exception cref="ArgumentNullException"><paramref name="name"/> was <see langword="null"/>.</exception>
        [Throws(typeof(ArgumentNullException))]
        public ResourceEntry(string name, [MaybeNull] object value)
        {
            ArgumentNullException.ThrowIfNull(name);
            this.name = name;
            this.value = value;
        }

        /// <summary>
        /// Gets a value whether this resource entry is invalid.
        /// </summary>
        public readonly System.Boolean IsNull => name == System.String.Empty;

        /// <summary>
        /// Gets the name of this resource entry.
        /// </summary>
        public readonly System.String Name => name;

        /// <summary>
        /// Gets the value of this resource entry. <br />
        /// May be <see langword="null"/>.
        /// </summary>
        [MaybeNull]
        public readonly System.Object Value => value;
    }
}
