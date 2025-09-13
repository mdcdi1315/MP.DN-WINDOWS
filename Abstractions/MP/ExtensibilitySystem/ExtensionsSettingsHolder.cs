
using System;
using MP.ExceptionSystem;
using System.Collections.Generic;

namespace MP.ExtensibilitySystem
{
    /// <summary>
    /// Used as the base settings class instance that is passed into the extension instances. <br />
    /// However, these classes can only see the <see cref="IAttributeable"/> part of this class implementation. <br />
    /// This is exported only and only to further support the <see cref="ExtensionEngine.LoadSettings"/> and <see cref="ExtensionEngine.SaveSettings"/> methods.
    /// </summary>
    public sealed class ExtensionsSettingsHolder : IAttributeable
    {
        private Dictionary<System.String, System.Object> items;

        /// <summary>
        /// Creates a default <see cref="ExtensionsSettingsHolder"/> class instance.
        /// </summary>
        public ExtensionsSettingsHolder() => items = new(10);

        /// <inheritdoc />
        public System.Object GetAttribute(System.String name)
        {
            if (System.String.IsNullOrEmpty(name)) { throw new ArgumentNullException(nameof(name)); }
            if (items.TryGetValue(name, out var value))
            {
                return value;
            }
            throw new AttributeNotFoundException(name);
        }

        /// <inheritdoc />
        public void SetAttribute(System.String name, System.Object value) => items[name] = value;

        /// <summary>
        /// Gets the number of entries/attributes defined in the current instance.
        /// </summary>
        public System.Int32 Count => items.Count;

        /// <summary>
        /// Gets all the keys of the entries defined in the current instance.
        /// </summary>
        public IEnumerable<System.String> Keys => items.Keys;
    }
}