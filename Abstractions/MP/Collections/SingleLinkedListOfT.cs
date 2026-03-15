
using System;
using System.Threading;
using System.Collections;
using System.Collections.Generic;
using MP.Annotations.CodeAnalysis;
using System.Diagnostics.CodeAnalysis;

namespace MP.Collections
{
    /// <summary>
    /// Provides an implementation of the <see cref="IList{T}"/> interface implemented using a pointer to the next node.
    /// </summary>
    /// <typeparam name="T">The type of the elements to be stored to this single linked list object.</typeparam>
    public class SingleLinkedList<T> : IList<T>, IGettableSettable<System.Int32, T>, ITraversableCollection<T>
        where T : notnull
    {
        private sealed class Node
        {
            public T Value;
            public Node Next;

            public Node(T value)
            {
                Next = null;
                Value = value;
            }
        }

        private sealed class Enumerator : IEnumerator<T>
        {
            private bool reset;
            private Node current, root;

            public Enumerator(Node root)
            {
                reset = true;
                this.root = root;
                current = null;
            }

            public T Current => current.Value;

            object IEnumerator.Current => Current;

            public void Dispose()
            {
                root = null;
                current = null;
            }

            public bool MoveNext()
            {
                if (reset) {
                    reset = false;
                    return (current = root) is not null;
                } else if (current.Next is not null) {
                    current = current.Next;
                    return true;
                } else {
                    return false;
                }
            }

            public void Reset() => reset = true;
        }

        private sealed class Syncronized : SingleLinkedList<T>, ISyncronizedByObject
        {
            private readonly object sync_object;

            public Syncronized() : base() => sync_object = new();

            public Syncronized(IEqualityComparer<T> comparer) : base(comparer) => sync_object = new();

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

            public override int IndexOf([AllowNull] T item)
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

            public override bool Remove([AllowNull] T item)
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

            // No meaning to override the GetEnumerator method for taking it through the lock object.
            // The Enumerator implementation captures the entire state of the list in the moment that is called, so it is relatively thread-safe.
        }

        /// <summary>
        /// Creates an empty thread-safe list, using the default equality comparer for comparing elements.
        /// </summary>
        /// <returns>An object extending the <see cref="SingleLinkedList{T}"/> class and is thread-safe.</returns>
        public static SingleLinkedList<T> CreateSyncronized() => new Syncronized();

        /// <summary>
        /// Creates an empty thread-safe list, using the specified equality comparer for comparing elements.
        /// </summary>
        /// <param name="comparer">The <see cref="IEqualityComparer{T}"/> instance to be used for comparing elements contained in the returned instance.</param>
        /// <returns>An object extending the <see cref="SingleLinkedList{T}"/> class and is thread-safe.</returns>
        public static SingleLinkedList<T> CreateSyncronized([AllowNull] IEqualityComparer<T> comparer) => new Syncronized(comparer);

        private int count;
        [AllowNull]
        private Node root, current;
        [NotNull]
        private readonly IEqualityComparer<T> comparer;

        /// <summary>
        /// Initializes a new and empty instance of the <see cref="SingleLinkedList{T}"/> class.
        /// </summary>
        public SingleLinkedList()
        {
            count = 0;
            root = current = null;
            comparer = EqualityComparer<T>.Default;
        }

        /// <summary>
        /// Initializes a new and empty instance of the <see cref="SingleLinkedList{T}"/> class, 
        /// which does utilize the specified <see cref="IEqualityComparer{T}"/> for comparing and determining equality of the list's items.
        /// </summary>
        /// <param name="comparer">The equality comparer to be used. Can be <see langword="null"/>, in which case the default equality comparer will be instead used.</param>
        public SingleLinkedList([AllowNull] IEqualityComparer<T> comparer)
        {
            count = 0;
            root = current = null;
            this.comparer = comparer ?? EqualityComparer<T>.Default;
        }

        /// <summary>
        /// Gets the number of elements contained in the current <see cref="SingleLinkedList{T}"/> object.
        /// </summary>
        public int Count => count;

        /// <summary>
        /// Gets a value whether this <see cref="SingleLinkedList{T}"/> object is read-only. <br />
        /// This value is always <see langword="false"/>.
        /// </summary>
        public bool IsReadOnly => false;

        /// <summary>
        /// Gets/sets the item at <paramref name="index"/>.
        /// </summary>
        /// <param name="index">The location in the list to retrieve/set the current item.</param>
        /// <returns>The value being stored at <paramref name="index"/>.</returns>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="index"/> is a negative value -or- it exceeds the current object bounds.</exception>
        public virtual T this[int index]
        {
            [Throws(typeof(ArgumentOutOfRangeException))]
            get {
                if (index < 0) {
                    throw new ArgumentOutOfRangeException(nameof(index), "The specified index was negative.");
                } else if (index >= count) {
                    throw new ArgumentOutOfRangeException(nameof(index), "The specified index was out of the list's bounds.");
                } else {
                    int c = 0;
                    Node p = root;
                    while (c < index) { c++; p = p.Next; }
                    return p.Value;
                }
            }
            [Throws(typeof(ArgumentOutOfRangeException))]
            set {
                if (index < 0) {
                    throw new ArgumentOutOfRangeException(nameof(index), "The specified index was negative.");
                } else if (index >= count) {
                    throw new ArgumentOutOfRangeException(nameof(index), "The specified index was out of the list's bounds.");
                } else {
                    int c = 0;
                    Node p = root;
                    while (c < index) { c++; p = p.Next; }
                    p.Value = value;
                }
            }
        }

        /// <summary>
        /// Determines the index of a specific value in the current <see cref="SingleLinkedList{T}"/> object.
        /// </summary>
        /// <param name="item">The item to determine it's index.</param>
        /// <returns>The value that indicates the location of <paramref name="item"/> in the current object; otherwise, -1 if <paramref name="item"/> was not found.</returns>
        [MustNotReportException]
        public virtual int IndexOf([AllowNull] T item)
        {
            int index = 0;
            Node p = root;
            while (p is not null) {
                if (comparer.Equals(p.Value, item)) { return index; }
                index++;
                p = p.Next;
            }
            return -1;
        }

        /// <inheritdoc />
        [Throws(typeof(ArgumentOutOfRangeException))]
        public virtual void Insert(int index, [AllowNull] T item)
        {
            if (index < 0) {
                throw new ArgumentOutOfRangeException(nameof(index), "The specified index was negative.");
            } else if (index > count) {
                throw new ArgumentOutOfRangeException(nameof(index), "The specified index was out of the list's bounds.");
            } else if (index == count) {
                // When index == count, it is like adding an item, so most appropriate here is to call the Add method.
                Add(item);
            } else {
                Node p = root;
                while (p.Next is not null) { p = p.Next; }
                p.Next = new(item);
                count++;
            }
        }

        /// <summary>
        /// Removes the specified item from this <see cref="SingleLinkedList{T}"/> object, located at the specified <paramref name="index"/>.
        /// </summary>
        /// <param name="index">The location of the item you wish to be removed.</param>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="index"/> is a negative value.</exception>
        [Throws(typeof(ArgumentOutOfRangeException))]
        public virtual void RemoveAt(int index)
        {
            if (index < 0) {
                throw new ArgumentOutOfRangeException(nameof(index), "The specified index was negative.");
            } else if (index >= count) {
                throw new ArgumentOutOfRangeException(nameof(index), "The specified index was out of the list's bounds.");
            } else {
                if (count > 1) {
                    int c = 0, i = index - 1;
                    Node p = root;
                    while (c < i) { c++; p = p.Next; }
                    // p.Next will be the element that is to be deleted
                    p.Next = p.Next.Next;
                } else {
                    root = null;
                }
                count--;
            }
        }

        /// <summary>
        /// Adds a new item at the end of this <see cref="SingleLinkedList{T}"/> object.
        /// </summary>
        /// <param name="item">The item you wish to be added to the current object.</param>
        public virtual void Add([AllowNull] T item)
        {
            if (count == 0) {
                root = current = new(item);
            } else {
                Node n = new(item);
                current.Next = n;
                current = n;
            }
            count++;
        }

        /// <summary>
        /// Removes all the currently added items from this <see cref="SingleLinkedList{T}"/> object.
        /// </summary>
        [MustNotReportException]
        public virtual void Clear()
        {
            count = 0;
            root = current = null;
        }

        /// <summary>
        /// Determines whether the specified <paramref name="item"/> is part of this <see cref="SingleLinkedList{T}"/> object.
        /// </summary>
        /// <param name="item">The item that you wish to check for it's existense in this <see cref="SingleLinkedList{T}"/> object.</param>
        /// <returns><see langword="true"/> if <paramref name="item"/> was found in this <see cref="SingleLinkedList{T}"/> object; otherwise, <see langword="false"/>.</returns>
        [MustNotReportException]
        public virtual bool Contains(T item)
        {
            Node p = root;
            while (p is not null) {
                if (comparer.Equals(p.Value, item)) { return true; }
                p = p.Next;
            }
            return false;
        }

        /// <summary>
        /// Copies the elements contained in the current <see cref="SingleLinkedList{T}"/> object to the specified one-dimensional array,
        /// starting copying to it at <paramref name="arrayIndex"/>.
        /// </summary>
        /// <param name="array">The array to copy all the elements to.</param>
        /// <param name="arrayIndex">The index inside the <paramref name="array"/> to begin copying to.</param>
        /// <exception cref="ArgumentNullException"><paramref name="array"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="arrayIndex"/> is a negative value.</exception>
        /// <exception cref="ArgumentException"><see cref="Count"/> + <paramref name="arrayIndex"/> are exceeding the <paramref name="array"/> bounds.</exception>
        [Throws(typeof(ArgumentOutOfRangeException), typeof(ArgumentException), typeof(ArgumentNullException))]
        public virtual void CopyTo(T[] array, int arrayIndex)
        {
            ArgumentNullException.ThrowIfNull(array);
            if (arrayIndex < 0) {
                throw new ArgumentOutOfRangeException(nameof(arrayIndex), "Array index cannot be a negative value.");
            } else if (arrayIndex + count > array.Length) {
                throw new ArgumentException("The array does not have enough space to place all the elements of the current SingleLinkedList object.", nameof(array));
            } else {
                Node p = root;
                for (int I = arrayIndex; p is not null; p = p.Next) { array[I++] = p.Value; }
            }
        }

        /// <summary>Removes a specific item from this <see cref="SingleLinkedList{T}"/> instance.</summary>
        /// <param name="item">The item that you wish to be removed from this <see cref="SingleLinkedList{T}"/> object.</param>
        /// <returns><see langword="true"/> if <paramref name="item"/> was found and was removed from this <see cref="SingleLinkedList{T}"/> object; otherwise, <see langword="false"/>.</returns>
        [MustNotReportException]
        public virtual bool Remove([AllowNull] T item)
        {
            Node c = root, prev = null;
            while (c is not null)
            {
                if (comparer.Equals(c.Value , item)) {
                    if (prev is null) {
                        root = c.Next;
                    } else {
                        prev.Next = c.Next;
                    }
                    count--;
                    return true;
                }
                prev = c;
                c = c.Next;
            }
            return false;
        }

        /// <inheritdoc />
        public virtual IEnumerator<T> GetEnumerator() => new Enumerator(root);

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}