
using System;
using System.Collections.Generic;

namespace MP
{
    /// <summary>
    /// Defines send or recieve RCU engine command metadata , that is additional state that a command requires to operate.
    /// </summary>
    public sealed class CommandMetadata
    {
        private System.String maindata;
        private List<CommandMetadataItem> items;

        /// <summary>
        /// Creates an empty <see cref="CommandMetadata"/> object.
        /// </summary>
        public CommandMetadata()
        {
            items = new(10);
            maindata = null;
        }

        /// <summary>
        /// Gets primary metadata to use. Is a string.
        /// </summary>
        public System.String PrimaryData
        {
            get => maindata;
            set => maindata = value;
        }

        /// <summary>
        /// Gets a metadata item defined with the contents of <paramref name="name"/> parameter.
        /// </summary>
        /// <param name="name">The name of the metadata item to retrieve.</param>
        /// <returns>The metadata item corresponding to <paramref name="name"/>.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="name"/> was <see langword="null"/> or empty.</exception>
        public CommandMetadataItem GetItem(System.String name)
        {
            if (System.String.IsNullOrEmpty(name)) { throw new ArgumentNullException(nameof(name)); }
            foreach (var item in items) 
            {
                if (item.Name == name) { return item; }
            }
            return null;
        }

        /// <summary>
        /// Adds a new metadata item.
        /// </summary>
        /// <param name="item">The item to add.</param>
        /// <exception cref="ArgumentNullException"><paramref name="item"/> was <see langword="null"/>.</exception>
        public void AddItem(CommandMetadataItem item) 
        {
            if (item is null) { throw new ArgumentNullException(nameof(item)); }
            items.Add(item);
        }

        /// <summary>
        /// Removes an existing metadata item.
        /// </summary>
        /// <param name="name">The first occurence of the metadata item to remove by name.</param>
        /// <returns>A value whether at least an element with name as defined by the <paramref name="name"/> parameter was found and was removed.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="name"/> was <see langword="null"/> or empty.</exception>
        public System.Boolean RemoveItem(System.String name) 
        {
            if (System.String.IsNullOrEmpty(name)) { throw new ArgumentNullException(nameof(name)); }
            for (System.Int32 I = 0; I < items.Count; I++) 
            {
                if (items[I].Name == name) 
                {
                    items.RemoveAt(I);
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Gets metadata items that have same metadata types. <br />
        /// For a list of default metadata types see <see cref="CommandMetadataItemType"/> enumeration.
        /// </summary>
        /// <param name="type">The metadata type to query for.</param>
        /// <returns>A list that contains only the item metadata types defined in <paramref name="type"/> parameter. May also be empty.</returns>
        /// <exception cref="ArgumentException"><paramref name="type"/> parameter specified the dummy value, which is equal to <see cref="CommandMetadataItemType.None"/>.</exception>
        public IEnumerable<CommandMetadataItem> GetItemsMatchingCommonType(CommandMetadataItemType type)
        {
            if (type == CommandMetadataItemType.None) { throw new ArgumentException("Invalid value for \'type\' parameter." , nameof(type)); }
            foreach (var item in items)
            {
                if (item.Type == type) { yield return item; }
            }
        }

        /// <summary>
        /// Clears all the metadata items defined. <br />
        /// Should be called before dereferencing to explicitly free memory.
        /// </summary>
        public void Clean() { items.Clear(); }
    }
}