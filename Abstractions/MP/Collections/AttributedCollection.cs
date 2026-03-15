

using System;
using System.Collections;
using MP.ExceptionSystem;
using System.Collections.Generic;

namespace MP.Collections
{
    /// <summary>
    /// A special type to create collection-based attributable instances.
    /// </summary>
    public sealed class AttributedCollection :
        ICollectableAttributeable,
        ICollection<AttributeKeyValuePair>,
        IGettableSettable<System.Int32 , AttributeKeyValuePair>,
        IGettableSettable<System.String , System.Object>
    {
        private List<AttributeKeyValuePair> data;
        private volatile System.Boolean sorted;

        private sealed class FastKeyComparer : IComparer<AttributeKeyValuePair>
        {
            public int Compare(AttributeKeyValuePair x, AttributeKeyValuePair y) => x.Key.CompareTo(y.Key);
        }

        /// <summary>
        /// Creates an empty collection instance.
        /// </summary>
        public AttributedCollection() : this(5) { }

        /// <summary>
        /// Creates an empty collection instance with the specified initial capacity.
        /// </summary>
        /// <param name="capacity">The initial capacity that the internal holder will initially have.</param>
        public AttributedCollection(int capacity)
        {
            data = new(capacity);
            // The empty list is always a sorted list
            sorted = true;
        }

        /// <summary>
        /// Gets the number of attributes contained in the current collection.
        /// </summary>
        public int Count => data.Count;

        /// <summary>
        /// Instances of <see cref="AttributedCollection"/> classes are always mutable.
        /// </summary>
        public bool IsReadOnly => false;

        private static System.String KeyConverter(AttributeKeyValuePair kvp) => kvp.Key;

        /// <summary>
        /// Gets all the keys defined in the current attributed collection instance.
        /// </summary>
        public IEnumerable<string> Keys => data.ConvertAll<System.String>(new(KeyConverter));

        /// <summary>
        /// Gets or sets an attribute at the specified index.
        /// </summary>
        /// <param name="index">The index in the internal array where to get or set the value.</param>
        /// <returns>The requested attribute at <paramref name="index"/>.</returns>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="index"/> is less than 0. -or- <paramref name="index"/> is equal to or greater than <see cref="Count"/>.</exception>
        public AttributeKeyValuePair this[int index]
        {
            get => data[index];
            set => data[index] = value;
        }

        /// <summary>
        /// Forwards to <see cref="IAttributeableExtensions.GetAttribute(IAttributeable, string)"/> when using the getter and to <see cref="SetAttribute"/> when using the setter.
        /// </summary>
        /// <param name="index">The key of the attribute to get or set.</param>
        /// <returns>The current value of the attribute, if there is any.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="index"/> was null or empty.</exception>
        /// <exception cref="AttributeNotFoundException"><paramref name="index"/> was not found while getting the attribute's value.</exception>
        public System.Object this[System.String index]
        {
            get => this.GetAttribute(index);
            set => SetAttribute(index, value);
        }

        /// <summary>
        /// Adds a new attribute into the current collection instance.
        /// </summary>
        /// <param name="item">The item to add.</param>
        /// <exception cref="ArgumentNullException"><see cref="AttributeKeyValuePair.Key"/> was <see langword="null"/>.</exception>
        public void Add(AttributeKeyValuePair item)
        {
            if (item.IsNull) { throw new ArgumentNullException(nameof(item), "The key in the key-value pair must not be null."); }
            data.Add(item);
            sorted = false;
        }

        /// <summary>
        /// Adds a new attribute into the current collection instance.
        /// </summary>
        /// <param name="name">The name of the new attribute.</param>
        /// <param name="value">The value of the new attribute.</param>
        /// <exception cref="ArgumentNullException"><paramref name="name"/> was <see langword="null"/>.</exception>
        public void Add(System.String name, System.Object value)
        {
            data.Add(new AttributeKeyValuePair(name, value));
            sorted = false;
        }

        /// <summary>
        /// Removes all the attributes from the current <see cref="AttributedCollection"/> instance.
        /// </summary>
        public void Clear() => data.Clear();

        /// <summary>
        /// Finds the internal array index of the specified attribute. <br />
        /// If the collection is sorted, this falls back to binary search semantics.
        /// </summary>
        /// <param name="name">The name of the attribute to search.</param>
        /// <returns>The internal array index of the specified attribute.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="name"/> was <see lsangword="null"/>.</exception>
        /// <exception cref="ArgumentException"><paramref name="name"/> was representing the empty string.</exception>
        public System.Int32 IndexOfAttribute(System.String name)
        {
            ArgumentNullException.ThrowIfNullOrEmpty(name);
            if (sorted) { return IndexOfAttributeFast(name); }
            for (int I = 0; I < data.Count; I++)
            {
                if (data[I].Key == name)
                {
                    return I;
                }
            }
            return -1;
        }

        private System.Int32 IndexOfAttributeFast(System.String name)
        {
            System.Int32 r = data.BinarySearch(new(name, null), new FastKeyComparer());
            if (r < 0)
            {
                return -1;
            }
            return r;
        }

        /// <summary>
        /// Finds whether the specified attribute exists in this collection instance. <br />
        /// Equivalent as calling the <see cref="IndexOfAttribute"/> method and testing whether the returned number is greater than -1.
        /// </summary>
        /// <param name="name">The name of the attribute to see whether it does exist.</param>
        /// <returns>A value whether the specified attribute does exist or not.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="name"/> was <see lsangword="null"/>.</exception>
        /// <exception cref="ArgumentException"><paramref name="name"/> was representing the empty string.</exception>
        public System.Boolean Contains(System.String name) => IndexOfAttribute(name) > -1;

        /// <summary>
        /// Finds whether the specified attribute exists in this collection instance. <br />
        /// Equivalent as calling the <see cref="IndexOfAttribute"/> method and testing whether the returned number is greater than -1.
        /// </summary>
        /// <param name="item">The key-value pair of the attribute to see whether it does exist.</param>
        /// <returns>A value whether the specified attribute does exist or not.</returns>
        /// <exception cref="ArgumentException"><see cref="AttributeKeyValuePair.Key"/> was representing the empty string.</exception>
        public System.Boolean Contains(AttributeKeyValuePair item)
        {
            if (item.IsNull) { return false; }
            return Contains(item.Key);
        }

        /// <inheritdoc />
        public void CopyTo(AttributeKeyValuePair[] array, int arrayIndex) => data.CopyTo(array, arrayIndex);

        /// <summary>
        /// Gets an enumerator that is able to enumerate all the contained attributes of this instance.
        /// </summary>
        /// <returns>An enumerator able to iterate through all the attributes stored as <see cref="AttributeKeyValuePair"/> instances.</returns>
        public IEnumerator<AttributeKeyValuePair> GetEnumerator() => data.GetEnumerator();

        /// <summary>
        /// Removes the first occurence of the attribute whose name is provided by the <paramref name="name"/> parameter.
        /// </summary>
        /// <param name="name">The name of the attribute to remove.</param>
        /// <returns>A value whether removal succeeded.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="name"/> was <see lsangword="null"/>.</exception>
        /// <exception cref="ArgumentException"><paramref name="name"/> was representing the empty string.</exception>
        public bool Remove(System.String name)
        {
            System.Int32 I = IndexOfAttribute(name);
            if (I > -1)
            {
                data.RemoveAt(I);
                sorted = false;
                return true;
            }
            return false;
        }

        /// <summary>
        /// Removes the first occurence of the attribute provided by <paramref name="item"/>.
        /// </summary>
        /// <param name="item">The attribute to remove.</param>
        /// <returns>A value whether removal succeeded.</returns>
        /// <exception cref="ArgumentException"><see cref="AttributeKeyValuePair.Key"/> was representing the empty string.</exception>
        public bool Remove(AttributeKeyValuePair item)
        {
            if (item.IsNull) { return false; }
            return Remove(item.Key);
        }

        /// <inheritdoc />
        /// <exception cref="ArgumentException"><paramref name="name"/> was representing the empty string.</exception>
        public void SetAttribute(string name, object value)
        {
            System.Int32 I = IndexOfAttribute(name);
            AttributeKeyValuePair pair = new(name, value);
            if (I > -1)
            {
                data[I] = pair;
            }
            else
            {
                data.Add(pair);
                sorted = false;
            }
        }

        /// <summary>
        /// Sorts the collection, if required.
        /// </summary>
        public void Sort()
        {
            if (sorted) { return; }
            try { data.Sort(new FastKeyComparer()); } catch (ArgumentException) { return; }
            sorted = true;
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        /// <inheritdoc />
        public bool TryGetAttribute(string attribute, out object value)
        {
            System.Int32 I = IndexOfAttribute(attribute);
            if (I > -1) {
                value = data[I].Value;
                return true;
            } else {
                value = null;
                return false;
            }
        }
    }
}