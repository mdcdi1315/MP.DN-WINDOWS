
using System;
using System.Threading;
using System.Collections;
using System.Collections.Generic;
using MP.Annotations.CodeAnalysis;
using System.Diagnostics.CodeAnalysis;

namespace MP.Collections
{
    /// <summary>
    /// A custom implementation of the <see cref="IList{T}"/> interface, backed by an array.
    /// </summary>
    /// <typeparam name="T">The type of the elements that this list will store.</typeparam>
    public class ArrayBasedList<T> : IList<T>, IGettableSettable<System.Int32, T>, ITraversableCollection<T>, IArrayBasedCollection
        where T : notnull
    {
        private int count;
        private T[] elements;
        private readonly IEqualityComparer<T> comparer;

        private sealed class Syncronized : ArrayBasedList<T>, ISyncronizedByObject
        {
            private readonly object sync_object;

            public Syncronized() : base() => sync_object = new object();

            public Syncronized(int capacity) : base(capacity) => sync_object = new object();

            public Syncronized(IEnumerable<T> items) : base(items) => sync_object = new object();

            public Syncronized([AllowNull] IEqualityComparer<T> comparer) : base(comparer) => sync_object = new object();

            public Syncronized(int capacity, [AllowNull] IEqualityComparer<T> comparer) : base(capacity, comparer) => sync_object = new object();

            public Syncronized(IEnumerable<T> items, [AllowNull] IEqualityComparer<T> comparer) : base(items, comparer) => sync_object = new object();

            public object SyncObject => sync_object;

            public override void Add([AllowNull] T item)
            {
                Monitor.Enter(sync_object);
                try {
                    base.Add(item);
                } finally {
                    Monitor.Exit(sync_object);
                }
            }

            public override void AddRange(IEnumerable<T> items)
            {
                Monitor.Enter(sync_object);
                try {
                    base.AddRange(items);
                } finally {
                    Monitor.Exit(sync_object);
                }
            }

            public override void Clear()
            {
                Monitor.Enter(sync_object);
                try {
                    base.Clear();
                } finally {
                    Monitor.Exit(sync_object);
                }
            }

            public override bool Contains(T item)
            {
                Monitor.Enter(sync_object);
                try {
                    return base.Contains(item);
                } finally {
                    Monitor.Exit(sync_object);
                }
            }

            public override void CopyTo(T[] array, int arrayIndex)
            {
                Monitor.Enter(sync_object);
                try {
                    base.CopyTo(array, arrayIndex);
                } finally {
                    Monitor.Exit(sync_object);
                }
            }

            public override IEnumerator<T> GetEnumerator()
            {
                Monitor.Enter(sync_object);
                try {
                    return ArrayEnumerator<T>.ByBoundsCopied(elements, 0, count);
                } finally {
                    Monitor.Exit(sync_object);
                }
            }

            public override int IndexOf(T item)
            {
                Monitor.Enter(sync_object);
                try {
                    return base.IndexOf(item);
                } finally {
                    Monitor.Exit(sync_object);
                }
            }

            public override void Insert(int index, [AllowNull] T item)
            {
                Monitor.Enter(sync_object);
                try {
                    base.Insert(index, item);
                } finally {
                    Monitor.Exit(sync_object);
                }
            }

            public override void InsertRange(int index, T[] items)
            {
                Monitor.Enter(sync_object);
                try {
                    base.InsertRange(index, items);
                } finally {
                    Monitor.Exit(sync_object);
                }
            }

            public override bool Remove(T item)
            {
                Monitor.Enter(sync_object);
                try {
                    return base.Remove(item);
                } finally {
                    Monitor.Exit(sync_object);
                }
            }

            public override void RemoveAt(int index)
            {
                Monitor.Enter(sync_object);
                try {
                    base.RemoveAt(index);
                } finally {
                    Monitor.Exit(sync_object);
                }
            }

            public override ArrayBasedList<T> Slice(int index, int count)
            {
                Monitor.Enter(sync_object);
                try {
                    return base.Slice(index, count);
                } finally {
                    Monitor.Exit(sync_object);
                }
            }

            public override T this[int index] 
            { 
                get {
                    Monitor.Enter(sync_object);
                    try {
                        return base[index];
                    } finally {
                        Monitor.Exit(sync_object);
                    }
                } 
                set {
                    Monitor.Enter(sync_object);
                    try {
                        base[index] = value;
                    } finally {
                        Monitor.Exit(sync_object);
                    }
                }
            }

            public override void TrimExcess()
            {
                Monitor.Enter(sync_object);
                try {
                    base.TrimExcess();
                } finally {
                    Monitor.Exit(sync_object);
                }
            }

            public override void EnsureCapacity(int n_elements)
            {
                Monitor.Enter(sync_object);
                try {
                    base.EnsureCapacity(n_elements);
                } finally {
                    Monitor.Exit(sync_object);
                }
            }
        }

        /// <summary>
        /// Creates an empty thread-safe list, using the default equality comparer for comparing elements.
        /// </summary>
        /// <returns>An object extending the <see cref="ArrayBasedList{T}"/> class and is thread-safe.</returns>
        public static ArrayBasedList<T> CreateSyncronized() => new Syncronized();

        /// <summary>
        /// Creates an empty thread-safe list, using the default equality comparer for comparing elements, and will have the specified initial capacity.
        /// </summary>
        /// <param name="capacity">The initial capacity that the returned object will have.</param>
        /// <returns>An object extending the <see cref="ArrayBasedList{T}"/> class and is thread-safe.</returns>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="capacity"/> is negative.</exception>
        [Throws(typeof(ArgumentOutOfRangeException))]
        public static ArrayBasedList<T> CreateSyncronized(int capacity) => new Syncronized(capacity);

        /// <summary>
        /// Creates an empty thread-safe list, using the specified equality comparer for comparing elements.
        /// </summary>
        /// <param name="comparer">The <see cref="IEqualityComparer{T}"/> instance to be used for comparing elements contained in the returned instance.</param>
        /// <returns>An object extending the <see cref="ArrayBasedList{T}"/> class and is thread-safe.</returns>
        public static ArrayBasedList<T> CreateSyncronized([AllowNull] IEqualityComparer<T> comparer) => new Syncronized(comparer);

        /// <summary>
        /// Creates an empty thread-safe list, using the specified equality comparer for comparing elements, and will have the specified initial capacity.
        /// </summary>
        /// <param name="capacity">The initial capacity that the returned object will have.</param>
        /// <param name="comparer">The <see cref="IEqualityComparer{T}"/> instance to be used for comparing elements contained in the returned instance.</param>
        /// <returns>An object extending the <see cref="ArrayBasedList{T}"/> class and is thread-safe.</returns>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="capacity"/> is negative.</exception>
        [Throws(typeof(ArgumentOutOfRangeException))]
        public static ArrayBasedList<T> CreateSyncronized(int capacity, [AllowNull] IEqualityComparer<T> comparer) => new Syncronized(capacity, comparer);

        /// <summary>
        /// Creates an empty thread-safe list from the specified items, using the default equality comparer for comparing elements.
        /// </summary>
        /// <param name="items">The items that the <see cref="ArrayBasedList{T}"/> class will initially have.</param>
        /// <returns>An object extending the <see cref="ArrayBasedList{T}"/> class and is thread-safe.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="items"/> is <see langword="null"/>.</exception>
        [Throws(typeof(ArgumentNullException))]
        public static ArrayBasedList<T> CreateSyncronized(IEnumerable<T> items) => new Syncronized(items);

        /// <summary>
        /// Creates an empty thread-safe list from the specified items, and using the specified equality comparer for comparing elements.
        /// </summary>
        /// <param name="items">The items that the <see cref="ArrayBasedList{T}"/> class will initially have.</param>
        /// <param name="comparer">The <see cref="IEqualityComparer{T}"/> instance to be used for comparing elements contained in the returned instance.</param>
        /// <returns>An object extending the <see cref="ArrayBasedList{T}"/> class and is thread-safe.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="items"/> is <see langword="null"/>.</exception>
        [Throws(typeof(ArgumentNullException))]
        public static ArrayBasedList<T> CreateSyncronized(IEnumerable<T> items, [AllowNull] IEqualityComparer<T> comparer) => new Syncronized(items, comparer);

        /// <summary>
        /// Initializes an empty instance of the <see cref="ArrayBasedList{T}"/> class, using the default equality comparer for comparing elements.
        /// </summary>
        public ArrayBasedList()
        {
            comparer = EqualityComparer<T>.Default;
            elements = Array.Empty<T>();
            count = 0;
        }

        /// <summary>
        /// Initializes an empty instance of the <see cref="ArrayBasedList{T}"/> class, using the default equality comparer for comparing elements, and will have the specified initial capacity.
        /// </summary>
        /// <param name="capacity">The initial capacity that the returned object will have.</param>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="capacity"/> is negative.</exception>
        [Throws(typeof(ArgumentOutOfRangeException))]
        public ArrayBasedList(int capacity)
        {
            if (capacity < 0) {
                throw new ArgumentOutOfRangeException(nameof(capacity), "Capacity cannot be a negative number.");
            } else {
                comparer = EqualityComparer<T>.Default;
                elements = new T[capacity];
                count = 0;
            }
        }

        /// <summary>
        /// Initializes an empty instance of the <see cref="ArrayBasedList{T}"/> class, using the specified equality comparer for comparing elements, and will have the specified initial capacity.
        /// </summary>
        /// <param name="capacity">The initial capacity that the returned object will have.</param>
        /// <param name="comparer">The <see cref="IEqualityComparer{T}"/> instance to be used for comparing elements contained in the returned instance.</param>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="capacity"/> is negative.</exception>
        [Throws(typeof(ArgumentOutOfRangeException))]
        public ArrayBasedList(int capacity, [AllowNull] IEqualityComparer<T> comparer)
        {
            if (capacity < 0) {
                throw new ArgumentOutOfRangeException(nameof(capacity), "Capacity cannot be a negative number.");
            } else {
                this.comparer = comparer ?? EqualityComparer<T>.Default;
                elements = new T[capacity];
                count = 0;
            }
        }

        /// <summary>
        /// Initializes an empty instance of the <see cref="ArrayBasedList{T}"/> class, using the specified equality comparer for comparing elements.
        /// </summary>
        /// <param name="comparer">The <see cref="IEqualityComparer{T}"/> instance to be used for comparing elements contained in the returned instance.</param>
        public ArrayBasedList([AllowNull] IEqualityComparer<T> comparer)
        {
            this.comparer = comparer ?? EqualityComparer<T>.Default;
            elements = Array.Empty<T>();
            count = 0;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ArrayBasedList{T}"/> class from the specified items, using the default equality comparer for comparing elements.
        /// </summary>
        /// <param name="items">The items that the <see cref="ArrayBasedList{T}"/> class will initially have.</param>
        /// <exception cref="ArgumentNullException"><paramref name="items"/> is <see langword="null"/>.</exception>
        [Throws(typeof(ArgumentNullException))]
        public ArrayBasedList(IEnumerable<T> items) : this() => AddRange(items);

        /// <summary>
        /// Initializes a new instance of the <see cref="ArrayBasedList{T}"/> class from the specified items, and using the specified equality comparer for comparing elements.
        /// </summary>
        /// <param name="items">The items that the <see cref="ArrayBasedList{T}"/> class will initially have.</param>
        /// <param name="comparer">The <see cref="IEqualityComparer{T}"/> instance to be used for comparing elements contained in the returned instance.</param>
        /// <exception cref="ArgumentNullException"><paramref name="items"/> is <see langword="null"/>.</exception>
        [Throws(typeof(ArgumentNullException))]
        public ArrayBasedList(IEnumerable<T> items, [AllowNull] IEqualityComparer<T> comparer) : this(comparer) => AddRange(items);

        /// <inheritdoc />
        public virtual T this[int index] 
        {
            get {
                if (index < 0) {
                    throw new ArgumentOutOfRangeException(nameof(index), "Index cannot be a negative value.");
                } else if (index >= count) {
                    throw new ArgumentOutOfRangeException(nameof(index), "Index is outside of the list's bounds.");
                } else {
                    return elements[index];
                }
            }
            set {
                if (index < 0) {
                    throw new ArgumentOutOfRangeException(nameof(index), "Index cannot be a negative value.");
                } else if (index >= count) {
                    throw new ArgumentOutOfRangeException(nameof(index), "Index is outside of the list's bounds.");
                } else {
                    elements[index] = value;
                }
            }
        }

        /// <inheritdoc />
        public int Count => count;

        /// <inheritdoc />
        public virtual bool IsReadOnly => false;

        /// <summary>
        /// Adds a new item to the end of the current <see cref="ArrayBasedList{T}"/> object.
        /// </summary>
        /// <param name="item">The item to add.</param>
        /// <exception cref="OverflowException">The list has reached it's maximum capacity.</exception>
        /// <exception cref="NotSupportedException">The specified operation is not supported.</exception>
        [Throws(typeof(OverflowException), typeof(NotSupportedException))]
        public virtual void Add([AllowNull] T item)
        {
            EnlargeArray(1);
            AddUnchecked(item);
        }

        private void AddUnchecked([AllowNull] T item) => elements[count++] = item;

        private void EnlargeArray(int by)
        {
            int new_count = unchecked(count + by);
            if (new_count < 0) {
                // Overflow detected, throw
                throw new OverflowException("The list has reached it's maximum capacity.");
            } else {
                // Check whether we can add 10 more elements to avoid additional resizes.
                int nc_additional = unchecked(new_count + 10);
                // If nc_additional > 0, we can do that, otherwise we have overflown by this and as such we need to resize by new_count.
                EnlargeInternal(nc_additional > 0 ? nc_additional : new_count);
            }
        }

        private void EnlargeInternal(int nc)
        {
            if (nc > elements.Length) {
                Array.Resize(ref elements, nc);
            }
        }

        /// <inheritdoc />
        public virtual void EnsureCapacity(int n_elements)
        {
            if (n_elements < 0) {
                throw new ArgumentOutOfRangeException(nameof(n_elements), "Number of elements to be ensured of should not be negative.");
            } else {
                EnlargeArray(n_elements);
            }
        }

        /// <summary>
        /// Removes all the items from the current <see cref="ArrayBasedList{T}"/> object.
        /// </summary>
        /// <exception cref="NotSupportedException">The specified operation is not supported.</exception>
        [Throws(typeof(NotSupportedException))]
        public virtual void Clear() => count = 0;

        /// <inheritdoc />
        public virtual bool Contains(T item) => IndexOf(item) > -1; // Rather simple wrapper around IndexOf.

        /// <inheritdoc />
        // Validation is done by the Array.Copy method; we do not need to do any validation checks.
        public virtual void CopyTo(T[] array, int arrayIndex) => Array.Copy(elements, 0, array, arrayIndex, count);

        /// <inheritdoc />
        public virtual IEnumerator<T> GetEnumerator() => ArrayEnumerator<T>.ByBounds(elements, 0, count);

        /// <inheritdoc />
        public virtual int IndexOf(T item) => ArrayHelpers.FindIndex(elements, 0, count, item, comparer);

        /// <summary>
        /// Inserts the specified item into the current <see cref="ArrayBasedList{T}"/> object.
        /// </summary>
        /// <param name="index">The zero-based index at which <paramref name="item"/> should be inserted.</param>
        /// <param name="item">The object to insert into the <see cref="ArrayBasedList{T}"/>.</param>
        /// <exception cref="OverflowException">The list has reached it's maximum capacity.</exception>
        /// <exception cref="NotSupportedException">This operation is not supported on the current instance.</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="index"/> is negative. -or- <paramref name="index"/> is out of the list's bounds.</exception>
        [Throws(typeof(ArgumentOutOfRangeException), typeof(OverflowException), typeof(NotSupportedException))]
        public virtual void Insert(int index, [AllowNull] T item) => InsertRange(index, new[] { item });

        /// <summary>
        /// Inserts a range of items into the current <see cref="ArrayBasedList{T}"/> object.
        /// </summary>
        /// <param name="index">The zero-based index at which the <paramref name="items"/> array should be inserted.</param>
        /// <param name="items">The items to insert.</param>
        /// <exception cref="OverflowException">The list has reached it's maximum capacity.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="items"/> is <see langword="null"/>.</exception>
        /// <exception cref="NotSupportedException">This operation is not supported on the current instance.</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="index"/> is negative. -or- <paramref name="index"/> is out of the list's bounds.</exception>
        [Throws(typeof(ArgumentOutOfRangeException), typeof(OverflowException), typeof(ArgumentNullException), typeof(NotSupportedException))]
        public virtual void InsertRange(int index, T[] items)
        {
            if (index < 0) {
                throw new ArgumentOutOfRangeException(nameof(index), "Index cannot be a negative value.");
            } else if (index > count) {
                // index == count is allowed because it is insertion op.
                throw new ArgumentOutOfRangeException(nameof(index), "Index was out of the array's bounds.");
            } else if (count > 0) {
                T[] constructed = new T[count + items.Length];

                // We are going to adapt the 'elements' array in the following way:
                // [0..index-1] -> Elements remain as is.
                // [index..index+items.Length] -> Elements from the 'items' array are put.
                // [index+items.Length..count] -> Element at index goes to index+items.Length, and the rest items are copied as-is.

                Array.Copy(elements, 0, constructed, 0, index);

                Array.Copy(items, 0, constructed, index, items.Length);

                int rem_items = count - index;

                if (rem_items > 0) {
                    Array.Copy(elements, index, constructed, index + items.Length, rem_items);
                }

                elements = constructed;
                count += items.Length;
            } else if (index == 0) {
                elements = new T[items.Length];
                Array.Copy(items, 0, elements, 0, items.Length);
                count = items.Length;
            }
        }

        /// <summary>
        /// Adds a range of items to the end of the current <see cref="ArrayBasedList{T}"/> object.
        /// </summary>
        /// <param name="items">The items to add.</param>
        /// <exception cref="OverflowException">The list has reached it's maximum capacity.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="items"/> is <see langword="null"/>.</exception>
        [Throws(typeof(ArgumentNullException), typeof(OverflowException))]
        public virtual void AddRange(IEnumerable<T> items)
        {
            ArgumentNullException.ThrowIfNull(items);
            if (items is T[] array) {
                AddRange_Array(array);
            } else if (items is ArrayBasedList<T> list) {
                AddRange_List(list);
            } else if (items is IList<T> other_list) {
                AddRange_List(other_list);
            } else {
                bool is_fast_path = true;
                if (items is ICollection<T> collection) {
                    EnlargeArray(collection.Count);
                } else if (items is ITraversableCollection<T> c2) {
                    EnlargeArray(c2.Count);
                } else {
                    is_fast_path = false;
                }
                IEnumerator<T> enumerator = items.GetEnumerator();
                try {
                    if (is_fast_path) {
                        while (enumerator.MoveNext()) { AddUnchecked(enumerator.Current); }
                    } else {
                        while (enumerator.MoveNext()) { Add(enumerator.Current); }
                    }
                } finally {
                    enumerator.Dispose();
                }
            }
        }

        private void AddRange_Array(T[] items)
        {
            int c = items.Length;
            EnlargeArray(c);
            Array.Copy(items, 0, elements, count, c);
            count += c;
        }

        private void AddRange_List(IList<T> items)
        {
            int ct = items.Count;
            EnlargeArray(ct);
            for (int I = 0; I < ct; I++) { elements[count + I] = items[I]; }
            count += ct;
        }

        private void AddRange_List(ArrayBasedList<T> list)
        {
            int ct = list.count;
            EnlargeArray(ct);
            Array.Copy(list.elements, 0, elements, count, ct);
            count += ct;
        }

        /// <summary>
        /// Removes the entries that are inaccessible because they were removed. <br />
        /// This optimizes the memory usage of the current object.
        /// </summary>
        public virtual void TrimExcess()
        {
            if (count == 0) {
                elements = Array.Empty<T>();
            } else if (count != elements.Length) {
                Array.Resize(ref elements, count);
            }
        }
        
        /// <inheritdoc />
        public virtual bool Remove(T item)
        {
            int index = IndexOf(item);
            if (index > -1) {
                RemoveAt(index);
                return true;
            } else {
                return false;
            }
        }

        /// <inheritdoc />
        public virtual void RemoveAt(int index)
        {
            if (index < 0) {
                throw new ArgumentOutOfRangeException(nameof(index), "Index cannot be a negative value.");
            } else if (index >= count) {
                throw new ArgumentOutOfRangeException(nameof(index), "Index is outside of the list's bounds.");
            } else {
                int index_after = index + 1;
                Array.Copy(elements, index_after, elements, index, count - index_after);
                count--;
            }
        }

        /// <summary>
        /// Converts all the elements of the current <see cref="ArrayBasedList{T}"/> class instance and creates a new instance of type <see cref="ArrayBasedList{TG}"/> that contains the converted elements.
        /// </summary>
        /// <typeparam name="TG">The type of the converted elements that the returned instance will contain.</typeparam>
        /// <param name="converter">The function that can convert an instance of type <typeparamref name="T"/> to an instance of type <typeparamref name="TG"/>.</param>
        /// <param name="comparer">If required by the user, an <see cref="IEqualityComparer{TG}"/> implementation to use for the newly created list object.</param>
        /// <returns>The converted list object that contains onl elements of type <typeparamref name="TG"/>.</returns>
        public virtual ArrayBasedList<TG> ConvertAll<TG>(Converter<T, TG> converter, [AllowNull] IEqualityComparer<TG> comparer = null)
        {
            ArgumentNullException.ThrowIfNull(converter);

            ArrayBasedList<TG> tg = new(count, comparer);
            for (int I = 0; I < count; I++) {
                tg.elements[I] = converter.Invoke(elements[I]);
            }
            tg.count = count;

            return tg;
        }

        /// <summary>
        /// Returns a portion of the <see cref="ArrayBasedList{T}"/> object, specified by the <paramref name="index"/> and <paramref name="count"/> parameters.
        /// </summary>
        /// <param name="index">The index to start the slice from.</param>
        /// <param name="count">The number of elements to include into the resulting <see cref="ArrayBasedList{T}"/> object.</param>
        /// <returns>A new <see cref="ArrayBasedList{T}"/> object that is the slice of the current object.</returns>
        /// <exception cref="ArgumentException"><paramref name="index"/> + <paramref name="count"/> value does exceed the list's bounds.</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="index"/> and/or <paramref name="count"/> are negative values.</exception>
        [Throws(typeof(ArgumentOutOfRangeException), typeof(ArgumentException))]
        public virtual ArrayBasedList<T> Slice(int index, int count)
        {
            if (index < 0) {
                throw new ArgumentOutOfRangeException(nameof(index), "Index cannot be a negative value.");
            } else if (count < 0) {
                throw new ArgumentOutOfRangeException(nameof(index), "Count cannot be a negative value.");
            } else if (index + count > this.count) {
                throw new ArgumentException("The specified combination of index and count parameters exceed the list's bounds.");
            } else {
                ArrayBasedList<T> ret = new(count, comparer);
                Array.Copy(elements, index, ret.elements, 0, count);
                ret.count = count;
                return ret;
            }
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        /// <summary>
        /// Provides a string representation of this object. <br />
        /// For debugging purposes only.
        /// </summary>
        /// <returns>A string representation of this object.</returns>
        [Throws]
        [return: NotNull]
        public override string ToString()
        {
            System.Text.StringBuilder builder = new();
            builder.AppendFormat("ArrayBasedList<{0}>({1}) {{ ", typeof(T).FullName, count);
            switch (count)
            {
                case 0:
                    builder.Append("<EMPTY>");
                    break;
                case 1:
                    builder.Append(elements[0]);
                    break;
                default:
                    int bound = count - 1;
                    for (int I = 0; I < bound; I++) {
                        builder.Append(elements[I]);
                        builder.Append(", ");
                    }
                    builder.Append(elements[bound]);
                    break;
            }
            builder.Append(" }");
            return builder.ToString();
        }
    }
}