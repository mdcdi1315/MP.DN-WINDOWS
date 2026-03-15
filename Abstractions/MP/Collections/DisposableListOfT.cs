

using System;
using System.Collections;
using System.Collections.Generic;

namespace MP.Collections
{
    /// <summary>
    /// Represents a list that does contain objects that implement the <see cref="IDisposable"/> interface.
    /// </summary>
    /// <typeparam name="T">The type of objects for this list object to manage. All objects must implement the <see cref="IDisposable"/> interface.</typeparam>
    public sealed class DisposableList<T> : IList<T> , IGettableSettable<System.Int32 , T>, ITraversableCollection<T> , IDisposable
        where T : IDisposable
    {
        /// <summary>
        /// Defines the enumerator implementation of the <see cref="DisposableList{T}"/> collection.
        /// </summary>
        public sealed class Enumerator : IEnumerator<T>
        {
            private System.Boolean valuetype;
            private IEnumerator<T> actual;
            private T lastitem;

            internal Enumerator(IEnumerator<T> actual)
            {
                this.actual = actual;
                valuetype = typeof(T).IsValueType;
            }

            /// <summary>
            /// Gets the element in the collection at the current position of the enumerator.
            /// </summary>
            public T Current
            {
                get {
                    if (valuetype || lastitem is null) {
                        lastitem = actual.Current;
                    }
                    return lastitem;
                }
            }

            /// <summary>
            /// Advances the enumerator to the next element of the collection.
            /// </summary>
            /// <returns><see langword="true"/> if the enumerator was successfully advanced to the next element; <see langword="false"/> if the enumerator has passed the end of the collection.</returns>
            /// <exception cref="InvalidOperationException">The collection was modified after the enumerator was created.</exception>
            public System.Boolean MoveNext()
            {
                System.Boolean result = actual.MoveNext();
                lastitem?.Dispose();
                lastitem = default;
                return result;
            }

            /// <summary>
            /// Sets the enumerator to its initial position, which is before the first element in the collection.
            /// </summary>
            /// <exception cref="InvalidOperationException">The collection was modified after the enumerator was created.</exception>
            public void Reset()
            {
                actual.Reset();
                lastitem?.Dispose();
                lastitem = default;
            }

            /// <summary>
            /// Disposes this <see cref="Enumerator"/> instance.
            /// </summary>
            public void Dispose()
            {
                actual.Dispose();
                lastitem?.Dispose();
                lastitem = default;
            }

            System.Object IEnumerator.Current => Current;
        }

        private readonly IList<T> list;

        /// <summary>
        /// Creates a new <see cref="DisposableList{T}"/> based on the <see cref="List{T}"/> class.
        /// </summary>
        public DisposableList() : this(10) { }

        /// <summary>
        /// Creates a new <see cref="DisposableList{T}"/> based on the <see cref="List{T}"/> class, and with 
        /// the specified initial capacity that the <see cref="DisposableList{T}"/> will have.
        /// </summary>
        /// <param name="capacity">The initial capacity of the <see cref="DisposableList{T}"/>.</param>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="capacity"/> was negative.</exception>
        public DisposableList(System.Int32 capacity) => list = new List<T>(capacity);

        /// <summary>
        /// Creates a new <see cref="DisposableList{T}"/> based on the <see cref="List{T}"/> class. <br />
        /// The <see cref="DisposableList{T}"/> will initially contain the items found into the <paramref name="items"/> parameter.
        /// </summary>
        /// <param name="items">An enumerable of items to put into the newly created <see cref="DisposableList{T}"/>.</param>
        /// <exception cref="ArgumentNullException"><paramref name="items"/> was <see langword="null"/>.</exception>
        public DisposableList(IEnumerable<T> items) => list = new List<T>(items);

        /// <summary>
        /// Gets or sets the element at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index of the element to get or set.</param>
        /// <returns>The element at the specified index.</returns>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="index"/> is not a valid index in the <see cref="DisposableList{T}"/>.</exception>
        public T this[System.Int32 index] 
        { 
            get => list[index]; 
            set => list[index] = value; 
        }

        /// <summary>
        /// Gets the number of items contained in the <see cref="DisposableList{T}"/>.
        /// </summary>
        public System.Int32 Count => list.Count;

        /// <summary>
        /// Gets a value whether the specified <see cref="DisposableList{T}"/> is read-only. <br />
        /// However, this value will always be <see langword="false"/>.
        /// </summary>
        public System.Boolean IsReadOnly => list.IsReadOnly;

        /// <summary>
        /// Adds an item into the current <see cref="DisposableList{T}"/>.
        /// </summary>
        /// <param name="item">The disposable item to add.</param>
        public void Add(T item) => list.Add(item);

        /// <summary>
        /// Clears all the items contained into the current <see cref="DisposableList{T}"/>,
        /// ensuring that all the items have been disposed of.
        /// </summary>
        public void Clear()
        {
            foreach (T disposable in list) { disposable?.Dispose(); }
            list.Clear();
        }

        /// <summary>
        /// Determines whether the <see cref="DisposableList{T}"/> contains a specific element.
        /// </summary>
        /// <param name="item">The element to see if it exists into the current <see cref="DisposableList{T}"/>.</param>
        /// <returns><see langword="true"/> if <paramref name="item"/> is found in the <see cref="DisposableList{T}"/>; otherwise, <see langword="false"/>.</returns>
        public System.Boolean Contains(T item) => list.Contains(item);

        /// <summary>
        /// Copies the elements of the <see cref="ReadOnlyList{T}"/> to an <see cref="Array"/>, starting at a particular <see cref="Array"/> index.
        /// </summary>
        /// <param name="array">The one-dimensional <see cref="Array"/> that is the destination of the elements copied from <see cref="ReadOnlyList{T}"/>. The <see cref="Array"/> must have zero-based indexing.</param>
        /// <param name="arrayIndex">The zero-based index in <paramref name="array"/> at which copying begins.</param>
        /// <exception cref="ArgumentException">The number of elements in the source <see cref="ReadOnlyList{T}"/> is greater than the available space from <paramref name="arrayIndex"/> to the end of the destination array.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="array"/> was <see langword="null"/>.</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="arrayIndex"/> is negative.</exception>
        public void CopyTo(T[] array, int arrayIndex) => list.CopyTo(array, arrayIndex);

        /// <summary>
        /// Returns an enumerator that is able to iterate through all the elements of the <see cref="DisposableList{T}"/>.
        /// </summary>
        /// <returns>A new enumerator object.</returns>
        public Enumerator GetEnumerator() => new(list.GetEnumerator());

        /// <summary>
        /// Determines the index of a specific item in the <see cref="DisposableList{T}"/>.
        /// </summary>
        /// <param name="item">The item to find.</param>
        /// <returns>The index that specifies the location of <paramref name="item"/> inside the collection, or -1 if <paramref name="item"/> was not found.</returns>
        public System.Int32 IndexOf(T item) => list.IndexOf(item);

        /// <summary>
        /// Inserts an item into the <see cref="DisposableList{T}"/> at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index at which item should be inserted.</param>
        /// <param name="item">The object to insert into the <see cref="DisposableList{T}"/>.</param>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="index"/> is not a valid index in the <see cref="DisposableList{T}"/>.</exception>
        public void Insert(System.Int32 index, T item) => list.Insert(index, item);

        /// <summary>
        /// Removes the first occurrence of a specific object from the <see cref="DisposableList{T}"/>.
        /// </summary>
        /// <param name="item">The object to remove from the <see cref="DisposableList{T}"/>.</param>
        /// <returns><see langword="true"/> if <paramref name="item"/> was successfully removed from the <see cref="DisposableList{T}"/>; otherwise, <see langword="false"/>. 
        /// This method also returns <see langword="false"/> if <paramref name="item"/> is not found in the original <see cref="DisposableList{T}"/>.</returns>
        public System.Boolean Remove(T item)
        {
            System.Int32 idx = list.IndexOf(item);
            if (idx < 0) { 
                return false; 
            }
            list[idx]?.Dispose();
            list.RemoveAt(idx);
            return true;
        }

        /// <summary>
        /// Removes the <see cref="DisposableList{T}"/> item at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index of the item to remove.</param>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="index"/> is not a valid index in the <see cref="DisposableList{T}"/>.</exception>
        public void RemoveAt(System.Int32 index)
        {
            if (index < 0 || index >= list.Count) {
                throw new ArgumentOutOfRangeException(nameof(index) , "The index was outside of the collection bounds.");
            }
            list[index]?.Dispose();
            list.RemoveAt(index);
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        IEnumerator<T> IEnumerable<T>.GetEnumerator() => GetEnumerator();

        /// <summary>
        /// Calls <see cref="Clear"/> on the current object, effectively disposing the list's items. <br />
        /// Can be called repeatedly
        /// </summary>
        public void Dispose() => Clear();
    }
}