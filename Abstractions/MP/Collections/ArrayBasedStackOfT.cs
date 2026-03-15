

using System;
using System.Threading;
using System.Collections;
using System.Collections.Generic;
using MP.Annotations.CodeAnalysis;
using System.Diagnostics.CodeAnalysis;

namespace MP.Collections
{
    /// <summary>
    /// An <see cref="ITraversableStack{T}"/> implementation by using an array as the backing storage.
    /// </summary>
    /// <typeparam name="T">The type of the elements this stack will hold.</typeparam>
    public class ArrayBasedStack<T> : ITraversableStack<T>, IGettableSettable<System.Int32, T>, IArrayBasedCollection
    {
        private int count;
        private T[] elements;

        private sealed class Syncronized : ArrayBasedStack<T>, ISyncronizedByObject
        {
            private readonly object lock_object;

            public Syncronized() : base() => lock_object = new object();

            public object SyncObject => lock_object;

            public override T this[int index]
            {
                get {
                    Monitor.Enter(lock_object);
                    try {
                        return base[index];
                    } finally {
                        Monitor.Exit(lock_object);
                    }
                }
            }

            public override void Clear()
            {
                Monitor.Enter(lock_object);
                try {
                    base.Clear();
                } finally {
                    Monitor.Exit(lock_object);
                }
            }

            public override void Push([MaybeNull] T value)
            {
                Monitor.Enter(lock_object);
                try {
                    base.Push(value);
                } finally {
                    Monitor.Exit(lock_object);
                }
            }

            public override bool TryPop(out T value)
            {
                Monitor.Enter(lock_object);
                try {
                    return base.TryPop(out value);
                } finally {
                    Monitor.Exit(lock_object);
                }
            }

            public override bool TryPeek(out T value)
            {
                Monitor.Enter(lock_object);
                try {
                    return base.TryPeek(out value);
                } finally {
                    Monitor.Exit(lock_object);
                }
            }

            public override void PushAll(IEnumerable<T> items)
            {
                Monitor.Enter(lock_object);
                try {
                    base.PushAll(items);
                } finally {
                    Monitor.Exit(lock_object);
                }
            }

            public override void PushAll(params T[] items)
            {
                Monitor.Enter(lock_object);
                try { 
                    base.PushAll(items); 
                } finally { 
                    Monitor.Exit(lock_object); 
                }
            }

            public override IEnumerator<T> GetEnumerator()
            {
                Monitor.Enter(lock_object);
                try {
                    return ReversedArrayEnumerator<T>.ByBoundsCopied(elements, 0, count);
                } finally {
                    Monitor.Exit(lock_object);
                }
            }

            public override void TrimExcess()
            {
                Monitor.Enter(lock_object);
                try {
                    base.TrimExcess();
                } finally { 
                    Monitor.Exit(lock_object); 
                }
            }

            public override void EnsureCapacity(int n_elements)
            {
                Monitor.Enter(lock_object);
                try {
                    base.EnsureCapacity(n_elements);
                } finally {
                    Monitor.Exit(lock_object);
                }
            }
        }

        /// <summary>
        /// Creates a thread-safe stack.
        /// </summary>
        /// <returns>An object extending the <see cref="ArrayBasedStack{T}"/> class and is thread-safe.</returns>
        public static ArrayBasedStack<T> CreateSyncronized() => new Syncronized();

        /// <summary>
        /// Creates a new instance of the <see cref="ArrayBasedStack{T}"/> class.
        /// </summary>
        public ArrayBasedStack()
        {
            count = 0;
            elements = Array.Empty<T>();
        }

        /// <inheritdoc />
        public virtual T this[int index]
        {
            get {
                if (index < 0) {
                    throw new ArgumentOutOfRangeException(nameof(index), "The specified index was negative.");
                } else if (index >= count) {
                    throw new ArgumentOutOfRangeException(nameof(index), "The specified index was out of the stack bounds.");
                } else {
                    return elements[count - index];
                }
            }
            set => throw new NotSupportedException("This operation is not supported on stack-based collections.");
        }

        /// <inheritdoc />
        public int Count => count;

        /// <inheritdoc />
        public virtual void Clear() => count = 0;

        /// <inheritdoc />
        public virtual IEnumerator<T> GetEnumerator() => ReversedArrayEnumerator<T>.ByBounds(elements, 0, count);

        /// <inheritdoc />
        public virtual void Push([MaybeNull] T value)
        {
            EnlargeArray(1);
            elements[count++] = value;
        }

        /// <inheritdoc />
        public virtual bool TryPeek(out T value)
        {
            if (count > 0) {
                value = elements[count - 1];
                return true;
            } else {
                value = default;
                return false;
            }
        }

        /// <inheritdoc />
        public virtual bool TryPop(out T value)
        {
            if (count > 0) {
                value = elements[--count];
                return true;
            } else {
                value = default;
                return false;
            }
        }

        /// <inheritdoc />
        public virtual void PushAll(IEnumerable<T> items)
        {
            ArgumentNullException.ThrowIfNull(items);
            if (items is T[] array) {
                PushByArray(array);
            } else if (items is IList<T> list) {
                PushByList(list);
            } else if (items is ICollection<T> collection) {
                PushByCollection(collection);
            } else {
                foreach (T item in items) { Push(item); }
            }
        }

        /// <summary>
        /// Specialization of the <see cref="PushAll(IEnumerable{T})"/> method, for pushing elements statically known.
        /// </summary>
        /// <param name="items">The items to push on the stack.</param>
        [Throws(typeof(ArgumentNullException))]
        public virtual void PushAll(params T[] items)
        {
            ArgumentNullException.ThrowIfNull(items);
            PushByArray(items);
        }

        /// <summary>
        /// Removes the entries that are inaccessible because they were popped. <br />
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

        private void EnlargeArray(int by)
        {
            int new_count = unchecked(elements.Length + by);
            if (new_count < 0) {
                // Overflow detected, throw
                throw new OverflowException("The stack has reached it's maximum capacity.");
            } else if (new_count > elements.Length) {
                T[] copy = new T[new_count];
                if (count > 0) {
                    Array.Copy(elements, 0, copy, 0, count);
                }
                elements = copy;
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

        private void PushByArray(T[] array)
        {
            EnlargeArray(array.Length);
            Array.Copy(array, 0, elements, count, array.Length);
            count += array.Length;
        }

        private void PushByList(IList<T> items)
        {
            int c = items.Count;
            EnlargeArray(c);
            for (int I = 0; I < c; I++) { elements[count+I] = items[I]; }
            count += c;
        }

        private void PushByCollection(ICollection<T> items)
        {
            int c = items.Count;
            EnlargeArray(c);
            IEnumerator<T> enumerator = items.GetEnumerator();
            try {
                for (int I = 0; enumerator.MoveNext(); I++) { elements[count + I] = enumerator.Current; }
            } finally {
                enumerator.Dispose();
            }
            count += c;
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}