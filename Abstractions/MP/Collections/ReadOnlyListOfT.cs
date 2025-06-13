
using System;
using System.Collections;
using System.Collections.Generic;

namespace MP.Collections
{
    /// <summary>
    /// Represents a read-only list of <typeparamref name="T"/> objects. <br />
    /// It wraps any <see cref="IList{T}"/> object by just selecting those methods that ensure the collection's readonlyness.
    /// </summary>
    /// <typeparam name="T">The type of the object that this list will hold.</typeparam>
    public sealed class ReadOnlyList<T> : IList<T> , ICloneable
    {
        private IList<T> list;

        /// <summary>
        /// Creates a new <see cref="ReadOnlyList{T}"/> object from the specified list, which is to be made read-only.
        /// </summary>
        /// <param name="list">The list object to make it as a read-only collection.</param>
        /// <exception cref="ArgumentNullException"><paramref name="list"/> was <see langword="null"/>.</exception>
        public ReadOnlyList(IList<T> list)
        {
            ArgumentNullException.ThrowIfNull(list);
            this.list = list;
        }

        /// <summary>
        /// Creates a new <see cref="ReadOnlyList{T}"/> object from the specified items, which are to be made read-only.
        /// </summary>
        /// <param name="items">Any enumerable of type <typeparamref name="T"/> that you want to make it a read-only list.</param>
        /// <exception cref="ArgumentNullException"><paramref name="items"/> was <see langword="null"/>.</exception>
        public ReadOnlyList(IEnumerable<T> items)
        {
            ArgumentNullException.ThrowIfNull(items);
            list = new List<T>(items);
        }

        /// <summary>
        /// Gets the element at the specified index. <br />
        /// Attempting to set an element always throws <see cref="NotSupportedException"/>.
        /// </summary>
        /// <param name="index">The element index inside the list to retrieve.</param>
        /// <returns>The retrieved element at <paramref name="index"/>.</returns>
        /// <exception cref="NotSupportedException">Attempted to modify the list.</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="index"/> is negative or outside the list's bounds.</exception>
        public T this[System.Int32 index] 
        { 
            get => list[index]; 
            set => throw new NotSupportedException("The collection is read-only and cannot be modified.");
        }

        /// <summary>
        /// Gets the number of elements contained in the current <see cref="ReadOnlyList{T}"/> object.
        /// </summary>
        public System.Int32 Count => list.Count;

        /// <summary>
        /// Gets a value whether the current collection is read-only. <br />
        /// <see cref="ReadOnlyList{T}"/> objects are always read-only collections, so this does always return <see langword="true"/>.
        /// </summary>
        public System.Boolean IsReadOnly => true;

        /// <summary>
        /// This operation is not supported and will always throw <see cref="NotSupportedException"/>.
        /// </summary>
        /// <param name="item"></param>
        /// <exception cref="NotSupportedException">Attempted to modify the collection.</exception>
        public void Add(T item)
            => throw new NotSupportedException("The collection is read-only and cannot be modified.");

        /// <summary>
        /// This operation is not supported and will always throw <see cref="NotSupportedException"/>.
        /// </summary>
        /// <exception cref="NotSupportedException">Attempted to modify the collection.</exception>
        public void Clear()
            => throw new NotSupportedException("The collection is read-only and cannot be modified.");

        /// <summary>
        /// Creates a new cloned instance of this <see cref="ReadOnlyList{T}"/> object.
        /// </summary>
        /// <returns>A new <see cref="ReadOnlyList{T}"/> but initialized from the held collection reference of this <see cref="ReadOnlyList{T}"/> object.</returns>
        public ReadOnlyList<T> Clone() => new(list);

        /// <summary>
        /// Determines whether a specific item is found into the current <see cref="ReadOnlyList{T}"/> object.
        /// </summary>
        /// <param name="item">The item to locate in the <see cref="ReadOnlyList{T}"/>.</param>
        /// <returns><see langword="true"/> if <paramref name="item"/> is found in the <see cref="ReadOnlyList{T}"/>; otherwise, <see langword="false"/>.</returns>
        public System.Boolean Contains(T item) => list.Contains(item);

        /// <summary>
        /// Copies the elements of the <see cref="ReadOnlyList{T}"/> to an <see cref="Array"/>, starting at a particular <see cref="Array"/> index.
        /// </summary>
        /// <param name="array">The one-dimensional <see cref="Array"/> that is the destination of the elements copied from <see cref="ReadOnlyList{T}"/>. The <see cref="Array"/> must have zero-based indexing.</param>
        /// <param name="arrayIndex">The zero-based index in <paramref name="array"/> at which copying begins.</param>
        /// <exception cref="ArgumentException">The number of elements in the source <see cref="ReadOnlyList{T}"/> is greater than the available space from <paramref name="arrayIndex"/> to the end of the destination array.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="array"/> was <see langword="null"/>.</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="arrayIndex"/> is negative.</exception>
        public void CopyTo(T[] array, System.Int32 arrayIndex) => list.CopyTo(array, arrayIndex);

        /// <summary>
        /// Returns an enumerator that is able to iterate through all the elements of the <see cref="ReadOnlyList{T}"/>.
        /// </summary>
        /// <returns>A new enumerator object.</returns>
        public IEnumerator<T> GetEnumerator() => list.GetEnumerator();

        /// <summary>
        /// Determines the index of a specific item in the <see cref="ReadOnlyList{T}"/>.
        /// </summary>
        /// <param name="item">The item to find.</param>
        /// <returns>The index that specifies the location of <paramref name="item"/> inside the collection, or -1 if <paramref name="item"/> was not found.</returns>
        public System.Int32 IndexOf(T item) => list.IndexOf(item);

        /// <summary>
        /// This operation is not supported and will always throw <see cref="NotSupportedException"/>.
        /// </summary>
        /// <exception cref="NotSupportedException">Attempted to modify the collection.</exception>
        public void Insert(System.Int32 index, T item)
            => throw new NotSupportedException("The collection is read-only and cannot be modified.");

        /// <summary>
        /// This operation is not supported and will always throw <see cref="NotSupportedException"/>.
        /// </summary>
        /// <exception cref="NotSupportedException">Attempted to modify the collection.</exception>
        public System.Boolean Remove(T item)
           => throw new NotSupportedException("The collection is read-only and cannot be modified.");

        /// <summary>
        /// This operation is not supported and will always throw <see cref="NotSupportedException"/>.
        /// </summary>
        /// <exception cref="NotSupportedException">Attempted to modify the collection.</exception>
        public void RemoveAt(System.Int32 index)
            => throw new NotSupportedException("The collection is read-only and cannot be modified.");

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        System.Object ICloneable.Clone() => Clone();

        /// <summary>
        /// Returns a string representing the contents of the current <see cref="ReadOnlyList{T}"/>.
        /// </summary>
        /// <returns>A string representation of the current <see cref="ReadOnlyList{T}"/>. For debugging purposes only.</returns>
        public override System.String ToString()
        {
            System.Text.StringBuilder sb = new(4096);
            sb.Append("ReadOnlyList<");
            sb.Append(typeof(T).Name);
            sb.Append(">(");
            sb.Append(list.Count);
            sb.Append(") { ");
            System.Int32 c = 0;
            foreach (T item in list)
            {
                sb.Append(item);
                if (c + 1 < list.Count) { sb.Append(", "); }
                c++;
            }
            sb.Append(" }");
            return sb.ToString();
        }
    }
}