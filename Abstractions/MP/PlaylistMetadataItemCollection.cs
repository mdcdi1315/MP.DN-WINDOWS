

using System.Collections;
using System.Collections.Generic;

namespace MP
{
    /// <summary>
    /// Referenced by the <see cref="IPlaylist"/> interface. <br />
    /// It is the primary implementation for private cached data inside the playlists. <br />
    /// Implements the <see cref="IList{T}"/> interface.
    /// </summary>
    public sealed class PlaylistMetadataItemCollection : IList<PlaylistMetadataItem>
    {
        private List<PlaylistMetadataItem> items;

        /// <summary>
        /// Creates a new <see cref="PlaylistMetadataItemCollection"/> empty class instance.
        /// </summary>
        public PlaylistMetadataItemCollection() 
        {
            items = new(10);
        }

        /// <summary>
        /// Creates a new metadata item collection with the specified initial capacity to allocate in the internal array.
        /// </summary>
        /// <param name="capacity">The initial capacity to allocate for the internal data array.</param>
        /// <exception cref="System.ArgumentOutOfRangeException"><paramref name="capacity"/> was negative.</exception>
        public PlaylistMetadataItemCollection(System.Int32 capacity)
        {
            items = new(capacity);
        }

        /// <summary>
        /// Creates a new <see cref="PlaylistMetadataItemCollection"/> class instance from the specified existing items.
        /// </summary>
        /// <param name="items">The existing item collection to initialize this instance from.</param>
        /// <exception cref="System.ArgumentNullException"><paramref name="items"/> parameter was <see langword="null"/>.</exception>
        public PlaylistMetadataItemCollection(IEnumerable<PlaylistMetadataItem> items)
        {
            this.items = new(items);
        }

        /// <summary>
        /// Retrieves an metadata item at the specified index of the internal backing array of the collection. <br />
        /// Directly setting an item instance is not supported and will always throw <see cref="System.NotSupportedException"/>.
        /// </summary>
        /// <param name="index">The item to retrieve from the collection specified by it's index.</param>
        /// <returns>The metadata item at <paramref name="index"/>.</returns>
        /// <exception cref="System.NotSupportedException">Attempted to directly set an metadata item at <paramref name="index"/>.</exception>
        public PlaylistMetadataItem this[System.Int32 index] 
        {
            get => items[index]; 
            set => throw new System.NotSupportedException("Setting directly an metadata item is not supported."); 
        }

        /// <summary>
        /// Gets or sets the metadata item value with the specified name to the collection. <br />
        /// When setting a value, if the specified name does not exist, it is created on the fly.
        /// </summary>
        /// <param name="name">The name of the metadata item whose value is to be got or set.</param>
        /// <returns>The value of the metadata item provided by the <paramref name="name"/> parameter.</returns>
        /// <exception cref="System.ArgumentNullException"><paramref name="name"/> was <see langword="null"/>.</exception>
        /// <exception cref="ExceptionSystem.MetadataItemNotFoundException">When getting a metadata item , the specified item was not found inside the collection.</exception>
        public System.Object this[System.String name]
        {
            get {
                System.Int32 idx = IndexOf(name);
                if (idx > -1) {
                    return items[idx].Value;
                }
                throw new ExceptionSystem.MetadataItemNotFoundException(name);
            }
            set {
                System.Int32 idx = IndexOf(name);
                if (idx > -1) {
                    items[idx].Value = value;
                } else {
                    Add(new(name, value));
                }
            }
        }

        /// <summary>
        /// Attempts to get a metadata item's value , if that exists. <br />
        /// The return value of this method indicates whether retrieval was successfull or not.
        /// </summary>
        /// <param name="name">The name of the metadata item to retrieve it's value.</param>
        /// <param name="value">The retrieved value from the metadata item.</param>
        /// <returns>A value whether the metadata item requested by <paramref name="name"/> was found and returned.</returns>
        /// <exception cref="System.ArgumentNullException"><paramref name="name"/> was <see langword="null"/> or represented the empty string (&quot;&quot;).</exception>
        public System.Boolean TryGetValue(System.String name , out System.Object value)
        {
            System.Int32 idx = IndexOf(name);
            if (idx > -1) {
                value = items[idx].Value;
                return true;
            }
            value = null;
            return false;
        }

        /// <summary>
        /// Gets the number of metadata items contained in the <see cref="PlaylistMetadataItemCollection"/>.
        /// </summary>
        public System.Int32 Count => items.Count;

        /// <summary>
        /// <see cref="PlaylistMetadataItemCollection"/> instances are mutable , so this always returns false.
        /// </summary>
        public System.Boolean IsReadOnly => false;

        /// <summary>
        /// Adds a new playlist metadata item into the current collection. <br />
        /// While it is perfectly valid to add two items of the same name , it should be avoided <br />
        /// because it can cause conflicting issues.
        /// </summary>
        /// <param name="item">The metadata item to add.</param>
        /// <exception cref="System.ArgumentNullException"><paramref name="item"/> was <see langword="null"/>.</exception>
        public void Add(PlaylistMetadataItem item)
        {
            if (item is null) {
                throw new System.ArgumentNullException(nameof(item));
            }
            items.Add(item);
        }

        /// <summary>
        /// Removes all the metadata items existing in the current <see cref="PlaylistMetadataItemCollection"/> instance.
        /// </summary>
        public void Clear() => items.Clear();

        /// <summary>
        /// Gets a value whether the specified <paramref name="item"/>'s name is existing in the current collection.
        /// </summary>
        /// <param name="item">The item to get it's name to be tested.</param>
        /// <returns>A value whether the item with the specified name does exist into the current instance.</returns>
        /// <exception cref="System.ArgumentNullException"><paramref name="item"/> was <see langword="null"/>.</exception>
        public System.Boolean Contains(PlaylistMetadataItem item)
        {
            if (item is null) {
                throw new System.ArgumentNullException(nameof(item));
            }
            return Contains(item.Name);
        }

        /// <summary>
        /// Gets a value whether the specified item name is already declared into the current collection.
        /// </summary>
        /// <param name="nameofitem">The name of the item to test.</param>
        /// <returns>A value whether the name provided is declared as an item into the current collection.</returns>
        /// <exception cref="System.ArgumentNullException"><paramref name="nameofitem"/> was <see langword="null"/> or represented the empty string (&quot;&quot;).</exception>
        public System.Boolean Contains(System.String nameofitem) => IndexOf(nameofitem) >= 0;

        /// <summary>
        /// Copies the entire <see cref="PlaylistMetadataItemCollection"/> to a compatible one-dimensional array, starting at the specified index of the target array.
        /// </summary>
        /// <param name="array">The one-dimensional <see cref="System.Array"/> that is the destination of the elements copied from <see cref="PlaylistMetadataItemCollection"/>. The <see cref="System.Array"/> must have zero-based indexing.</param>
        /// <param name="arrayIndex">The zero-based index in <paramref name="array"/> at which copying begins.</param>
        /// <exception cref="System.ArgumentNullException"><paramref name="array"/> is <see langword="null"/>.</exception>
        /// <exception cref="System.ArgumentOutOfRangeException"><paramref name="arrayIndex"/> is negative.</exception>
        /// <exception cref="System.ArgumentException">The number of elements in the source <see cref="PlaylistMetadataItemCollection"/> is greater than the available space from <paramref name="arrayIndex"/> to the end of the destination array.</exception>
        public void CopyTo(PlaylistMetadataItem[] array, System.Int32 arrayIndex) => items.CopyTo(array, arrayIndex);

        /// <summary>
        /// Gets an enumerator that is able to enumerate all the metadata items contained in the collection.
        /// </summary>
        /// <returns>An enumerator implementation that implements the <see cref="IEnumerator{T}"/> interface.</returns>
        public IEnumerator<PlaylistMetadataItem> GetEnumerator() => items.GetEnumerator();

        /// <summary>
        /// Gets an index inside the internal backing array of the collection where the provided item can be retrieved.
        /// </summary>
        /// <param name="item">The item to find inside the collection.</param>
        /// <returns>An index representing the location of the item , or -1 if not found.</returns>
        /// <exception cref="System.ArgumentNullException"><paramref name="item"/> was <see langword="null"/>.</exception>
        public System.Int32 IndexOf(PlaylistMetadataItem item)
        {
            if (item is null) {
                throw new System.ArgumentNullException(nameof(item));
            }
            return IndexOf(item.Name);
        }

        /// <summary>
        /// Gets an index inside the internal backing array of the collection where the item with the provided name can be retrieved.
        /// </summary>
        /// <param name="itemname">The name of the item to find inside the collection.</param>
        /// <returns>An index representing the location of the item , or -1 if not found.</returns>
        /// <exception cref="System.ArgumentNullException"><paramref name="itemname"/> was <see langword="null"/> or represented the empty string (&quot;&quot;).</exception>
        public System.Int32 IndexOf(System.String itemname)
        {
            if (System.String.IsNullOrEmpty(itemname))
            {
                throw new System.ArgumentNullException(nameof(itemname));
            }
            for (System.Int32 I = 0; I < items.Count; I++)
            {
                if (items[I].Name == itemname)
                {
                    return I;
                }
            }
            return -1;
        }

        /// <summary>
        /// Inserts a new item into the current <see cref="PlaylistMetadataItemCollection"/> instance , at the specified index.
        /// </summary>
        /// <param name="index">The position where the provided <paramref name="item"/> should be saved to.</param>
        /// <param name="item">The item to be inserted.</param>
        /// <exception cref="System.ArgumentNullException"><paramref name="item"/> was <see langword="null"/>.</exception>
        /// <exception cref="System.ArgumentOutOfRangeException"><paramref name="index"/> was negative or excceeded the collection upper bound.</exception>
        public void Insert(System.Int32 index, PlaylistMetadataItem item)
        {
            if (item is null) {
                throw new System.ArgumentNullException(nameof(item));
            }
            items.Insert(index, item);
        }

        /// <summary>
        /// Removes the first occurence of the specified metadta item from the collection.
        /// </summary>
        /// <param name="item">The item to be removed.</param>
        /// <returns>A value whether the item was found and removed successfully.</returns>
        /// <exception cref="System.ArgumentNullException"><paramref name="item"/> was <see langword="null"/>.</exception>
        public System.Boolean Remove(PlaylistMetadataItem item)
        {
            System.Int32 idx = IndexOf(item);
            if (idx > -1)
            {
                items.RemoveAt(idx);
                return true;
            }
            return false;
        }

        /// <summary>
        /// Removes the metadata item at the specified index inside the internal backing array of the collection.
        /// </summary>
        /// <param name="index">The index of the metadata item to remove.</param>
        /// <exception cref="System.ArgumentOutOfRangeException"><paramref name="index"/> was negative or it exceeded the element count of the collection.</exception>
        public void RemoveAt(System.Int32 index) => items.RemoveAt(index);

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}