
using System.Collections;
using System.Collections.Generic;

namespace MP
{
    /// <summary>
    /// Defines a collection on how archived playlist attributes should be retrieved and set to a playlist archive.
    /// </summary>
    public sealed class ArchivedPlaylistAttributeCollection : IList<ArchivedPlaylistAttribute>
    {
        private List<ArchivedPlaylistAttribute> attributes;

        /// <summary>
        /// Creates an empty instance of the <see cref="ArchivedPlaylistAttributeCollection"/> class
        /// with the internal dynamic array capacity set to zero.
        /// </summary>
        public ArchivedPlaylistAttributeCollection()
        {
            attributes = new();
        }

        /// <summary>
        /// Creates an empty instance of the <see cref="ArchivedPlaylistAttributeCollection"/> class
        /// with the internal dynamic array capacity to be set with the <paramref name="recommendedcap"/> parameter.
        /// </summary>
        /// <param name="recommendedcap">The recommended capacity set by the user for the internal array to have.</param>
        /// <exception cref="System.ArgumentOutOfRangeException"><paramref name="recommendedcap"/> was negative.</exception>
        public ArchivedPlaylistAttributeCollection(System.Int32 recommendedcap)
        {
            if (recommendedcap < 0)
            {
                throw new System.ArgumentOutOfRangeException(nameof(recommendedcap), "The recommended capacity value must not be negative.");
            }
            attributes = new(recommendedcap);
        }

        /// <summary>
        /// Gets the archived playlist attribute at the specified <paramref name="index"/>. <br />
        /// Modifying directly attributes is not supported; see the <see cref="UpdateAttributeValue(string, string)"/> method on modifying an attribute value.
        /// </summary>
        /// <param name="index">The zero-based index of the element to get. Setting directly is not supported.</param>
        /// <returns>The attribute at the specified <paramref name="index"/>.</returns>
        /// <exception cref="System.ArgumentOutOfRangeException"><paramref name="index"/> is not a valid index in the <see cref="ArchivedPlaylistAttributeCollection"/>.</exception>
        /// <exception cref="System.NotSupportedException">The exception that is thrown when there is an attempt to set an attribute at <paramref name="index"/>.</exception>
        public ArchivedPlaylistAttribute this[System.Int32 index]
        { 
            get => attributes[index]; 
            set => throw new System.NotSupportedException("Direct modification of the attribute data is not supported."); 
        }

        /// <summary>
        /// Gets the number of attributes that the <see cref="ArchivedPlaylistAttributeCollection"/> contains.
        /// </summary>
        public System.Int32 Count => attributes.Count;

        /// <summary>
        /// (Inherited from <see cref="ICollection{T}"/>) <br />
        /// Such kind of playlists can always be modified, so this property does always return <see langword="true"/>.
        /// </summary>
        public System.Boolean IsReadOnly => true;

        /// <summary>
        /// Adds a new attribute to the end of the internal dynamic array.
        /// </summary>
        /// <param name="item">The attribute to add. It's name must be unique across the collection.</param>
        /// <exception cref="System.ArgumentNullException"><paramref name="item"/> was <see langword="null"/>.</exception>
        /// <exception cref="MP.ExceptionSystem.ArchiveAttributeDefinedException"><paramref name="item"/> has a name that is already defined in the collection.</exception>
        public void Add(ArchivedPlaylistAttribute item)
        {
            if (item is null) { throw new System.ArgumentNullException(nameof(item)); }
            if (IndexOf(item.Name) > -1) { throw new MP.ExceptionSystem.ArchiveAttributeDefinedException(item.Name); }
            attributes.Add(item);
        }

        /// <summary>
        /// Removes all the currently saved attributes on the <see cref="ArchivedPlaylistAttributeCollection"/>.
        /// </summary>
        public void Clear() => attributes.Clear();

        /// <summary>
        /// Updates the attribute's value specified by the <paramref name="name"/> parameter. <br />
        /// If the attribute does not exist , it does then create a new one and adds it.
        /// </summary>
        /// <param name="name">The name of the attribute to modify.</param>
        /// <param name="value">The value of the attribute to be modified and replaced with the contents of this parameter..</param>
        /// <returns><see langword="true"/> when the method needed to create a new attribute for the specified name; 
        /// otherwise it returns <see langword="false"/> that indicates that the method did successfully updated the value of the attribute.</returns>
        /// <exception cref="ExceptionSystem.ReadOnlyArchiveAttributeException">The method attempted to update an read-only attribute.</exception>
        public System.Boolean UpdateAttributeValue(System.String name , System.String value)
        {
            System.Int32 idx = IndexOf(name);
            if (idx < 0) {
                attributes.Add(new(name, value));
                return true;
            } else {
                attributes[idx].Value = value;
                return false;
            }
        }

        /// <summary>
        /// Gets a value whether the specified attribute is included in the <see cref="ArchivedPlaylistAttributeCollection"/>.
        /// </summary>
        /// <param name="item">The attribute to check.</param>
        /// <returns><see langword="true"/> when the attribute is included is included in the <see cref="ArchivedPlaylistAttributeCollection"/>; otherwise it returns <see langword="false"/>.</returns>
        /// <exception cref="System.ArgumentNullException"><paramref name="item"/> was <see langword="null"/>.</exception>
        public System.Boolean Contains(ArchivedPlaylistAttribute item)
        {
            if (item is null) { throw new System.ArgumentNullException(nameof(item)); }
            return IndexOf(item.Name) > -1;
        }

        /// <summary>
        /// Copies the elements of the <see cref="ArchivedPlaylistAttributeCollection"/> to an attribute array, starting at a particular array index.
        /// </summary>
        /// <param name="array">The one-dimensional <see cref="ArchivedPlaylistAttribute"/> array that is the 
        /// destination of the elements copied from the <see cref="ArchivedPlaylistAttributeCollection"/> instance. <br />
        /// The array must have zero-based indexing.</param>
        /// <param name="arrayIndex">The zero-based index in <paramref name="array"/> at which copying begins.</param>
        /// <exception cref="System.ArgumentNullException"><paramref name="array"/> is <see langword="null"/>.</exception>
        /// <exception cref="System.ArgumentOutOfRangeException"><paramref name="arrayIndex"/> is negative.</exception>
        /// <exception cref="System.ArgumentException">The number of elements in the source <see cref="ArchivedPlaylistAttributeCollection"/> is greater that the available space from <paramref name="arrayIndex"/> to the end of the destination array.</exception>
        public void CopyTo(ArchivedPlaylistAttribute[] array, int arrayIndex) => attributes.CopyTo(array, arrayIndex);

        /// <summary>
        /// Returns a new enumerator instance that has the ability to enumerate through the elements that are defined in the <see cref="ArchivedPlaylistAttributeCollection"/>.
        /// </summary>
        /// <returns>A new enumerator instance.</returns>
        public IEnumerator<ArchivedPlaylistAttribute> GetEnumerator() => attributes.GetEnumerator();

        /// <summary>
        /// Returns an array index where the archive attribute with the name provided by the contents <paramref name="name"/> parameter is located to.
        /// </summary>
        /// <param name="name">The archive attribute name to look up.</param>
        /// <returns>An array index where the attribute with name <paramref name="name"/> 
        /// is located to, or -1 indicating that the specified attribute was not found.</returns>
        public System.Int32 IndexOf(System.String name)
        {
            for (System.Int32 I = 0; I < attributes.Count; I++) 
            {
                if (attributes[I].Name == name) {
                    return I;
                }
            }
            return -1;
        }

        /// <summary>
        /// Returns an array index where the specified archive attribute is located to.
        /// </summary>
        /// <param name="attribute">The attribute to get it's array index inside the internal array.</param>
        /// <returns>An array index where the attribute  is located to, or -1 indicating that the specified attribute was not found.</returns>
        /// <exception cref="System.ArgumentNullException"><paramref name="attribute"/> was null.</exception>
        public System.Int32 IndexOf(ArchivedPlaylistAttribute attribute) 
        {
            if (attribute is null) { throw new System.ArgumentNullException(nameof(attribute)); }
            return IndexOf(attribute.Name);
        }

        /// <summary>
        /// Inserts an item into the <see cref="ArchivedPlaylistAttributeCollection"/> at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index at which <paramref name="item"/> should be inserted.</param>
        /// <param name="item">The attribute to insert into the <see cref="ArchivedPlaylistAttributeCollection"/> instance.</param>
        /// <exception cref="System.ArgumentNullException"><paramref name="item"/> was <see langword="null"/>.</exception>
        /// <exception cref="System.ArgumentOutOfRangeException"><paramref name="index"/> is not a valid index in the <see cref="ArchivedPlaylistAttributeCollection"/>.</exception>
        public void Insert(int index, ArchivedPlaylistAttribute item)
        {
            if (item is null) { throw new System.ArgumentNullException(nameof(item)); }
            attributes.Insert(index, item);
        }

        /// <summary>
        /// Removes the attribute with the specified name from the <see cref="ArchivedPlaylistAttributeCollection"/>.
        /// </summary>
        /// <param name="item">The attribute to remove from the <see cref="ArchivedPlaylistAttributeCollection"/>.</param>
        /// <returns><see langword="true"/> if <paramref name="item"/> was successfully removed from the <see cref="ArchivedPlaylistAttributeCollection"/>;
        /// otherwise, <see langword="false"/>. This method also returns <see langword="false"/> if <paramref name="item"/> is not found in the
        /// internal dynamic array.</returns>
        /// <exception cref="System.ArgumentNullException"><paramref name="item"/> was <see langword="null"/>.</exception>
        public System.Boolean Remove(ArchivedPlaylistAttribute item)
        {
            if (item is null) { throw new System.ArgumentNullException(nameof(item)); }
            System.Int32 idx = IndexOf(item.Name);
            if (idx >= 0) {
                RemoveAt(idx);
                return true;
            }
            return false;
        }

        /// <summary>
        /// Removes an archive attribute that is located at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index of the attribute to remove.</param>
        /// <exception cref="System.ArgumentOutOfRangeException">(Inherited from <see cref="List{T}"/>) <paramref name="index"/> is not a valid attribute index in the <see cref="ArchivedPlaylistAttributeCollection"/>.</exception>
        public void RemoveAt(int index) => attributes.RemoveAt(index);

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}