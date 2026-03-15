
using System;
using System.Threading;
using System.Collections;
using System.Collections.Generic;

namespace MP.Collections
{
    /// <summary>Provides a list for registering and retrieveing delegates.</summary>
    /// <typeparam name="T">The specific type of the delegate to be stored.</typeparam>
    public sealed class SyncronizedDelegateEventListHolder<T> : IList<T>, ISyncronizedByObject
        where T : class, Delegate
    {
        private sealed class Node
        {
            public T Value;
            public Node Next;

            public Node(T value)
            { 
                Value = value;
                Next = null;
            }
        }

        private sealed class Enumerator : IEnumerator<T>
        {
            private Node rt;
            private bool reset;
            private Node current;

            public Enumerator(Node root)
            {
                rt = root;
                current = null;
            }

            public T Current => current.Value;

            object IEnumerator.Current => Current;

            public bool MoveNext()
            {
                if (rt is null) {
                    return false;
                } else if (reset) {
                    reset = false;
                    return (current = rt) is not null;
                } else if (current.Next is not null) {
                    current = current.Next;
                    return true;
                } else {
                    return false;
                }
            }

            public void Reset() => reset = true;

            public void Dispose()
            {
                rt = null;
                current = null;
            }
        }

        private int count;
        private Node root;
        private readonly System.Object sync_object;

        /// <summary>
        /// Initializes a new instance of the <see cref="SyncronizedDelegateEventListHolder{T}"/> class.
        /// </summary>
        public SyncronizedDelegateEventListHolder()
        {
            count = 0;
            root = null;
            sync_object = new();
        }

        /// <inheritdoc />
        public T this[int index] 
        { 
            get {
                if (index.ToUInt32() >= count.ToUInt32()) {
                    throw new ArgumentOutOfRangeException(nameof(index), "Index cannot be outside of the list's bounds.");
                } else {
                    int I = 0;
                    Node n = root;
                    while (I < index && n is not null)
                    {
                        I++;
                        n = n.Next;
                    }
                    return n.Value;
                }
            }
            set => throw new NotSupportedException("This method call is not supported.");
        }

        /// <inheritdoc />
        public int Count => count;

        /// <inheritdoc />
        public bool IsReadOnly => false;

        /// <inheritdoc />
        public object SyncObject => sync_object;

        /// <inheritdoc />
        public void Add(T item)
        {
            ArgumentNullException.ThrowIfNull(item);
            Node n = new(item);
            Monitor.Enter(sync_object);
            try {
                Node c = root, p = null;
                while (c is not null)
                {
                    if (c.Value == item) {
                        throw new ArgumentException("The specified delegate is already added to the object.");
                    } else {
                        p = c;
                        c = c.Next;
                    }
                }
                if (p is null) { root = n; } else { p.Next = n; }
                count++;
            } finally {
                Monitor.Exit(sync_object);
            }
        }

        /// <summary>
        /// Removes all the delegates from the <see cref="SyncronizedDelegateEventListHolder{T}"/> object.
        /// </summary>
        public void Clear()
        {
            Monitor.Enter(sync_object);
            try {
                count = 0;
                root = null;
            } finally {
                Monitor.Exit(sync_object);
            }
        }

        /// <summary>
        /// Finds out whether the specified delegate exists in the current object.
        /// </summary>
        /// <param name="item">The delegate to find out.</param>
        /// <returns>A value whether <paramref name="item"/> is contained in this object or not.</returns>
        public bool Contains(T item)
        {
            if (item is null) { return false; }
            Node n = root;
            while (n is not null)
            {
                if (n.Value == item) { 
                    return true; 
                } else {
                    n = n.Next;
                }
            }
            return false;
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            ArgumentNullException.ThrowIfNull(array);
            if (arrayIndex < 0) {
                throw new ArgumentOutOfRangeException(nameof(arrayIndex), "Array index cannot be a negative value.");
            } else if (arrayIndex + count > array.Length) {
                throw new ArgumentException("The array does not have enough space to place all the elements of the current SyncronizedDelegateEventListHolder object.", nameof(array));
            } else {
                Node n = root;
                for (int I = arrayIndex; n is not null; n = n.Next) { array[I++] = n.Value; }
            }
        }

        /// <inheritdoc />
        public IEnumerator<T> GetEnumerator() => new Enumerator(root);

        /// <inheritdoc />
        public int IndexOf(T item)
        {
            int I = 0;
            Node n = root;
            while (n is not null)
            {
                if (n.Value == item) {
                    return I; 
                } else {
                    n = n.Next;
                    I++;
                }
            }
            return -1;
        }

        /// <inheritdoc />
        public bool Remove(T item)
        {
            if (count == 0) {
                return false;
            } else {
                Node n = root, prev = null;
                while (n is not null)
                {
                    if (n.Value == item) {
                        if (prev is null) {
                            root = n.Next;
                        } else {
                            prev.Next = n.Next;
                        }
                        count--;
                        return true;
                    } else {
                        prev = n;
                        n = n.Next;
                    }
                }
                return false;
            }
        }

        /// <summary>This is not supported and will always throw <see cref="NotSupportedException"/>.</summary>
        public void RemoveAt(int index) => throw new NotSupportedException("This method call is not supported.");

        /// <summary>This is not supported and will always throw <see cref="NotSupportedException"/>.</summary>
        public void Insert(int index, T item) => throw new NotSupportedException("This method call is not supported.");

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
