
using System;
using System.Threading;
using System.Collections;
using System.Collections.Generic;
using MP.Annotations.CodeAnalysis;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace MP.Collections
{
    /// <summary>
    /// An <see cref="ITraversableQueue{T}"/> implementation by using an array as the backing storage.
    /// </summary>
    /// <typeparam name="T">The type of the elements this queue will hold.</typeparam>
    public class ArrayBasedQueue<T> : ITraversableQueue<T>, IGettableSettable<System.Int32, T>, IArrayBasedCollection
    {
        // This class manages the 'elements' array in a reverse manner.
        // That is, the item that will be dequeued (namely the 'head') is located 
        // close to the beginning of the array.

        private int count;
        private int head; // -1 means no head (empty collection)
        private int tail; // -1 means no tail (empty collection)
        private T[] elements;

        // A threshold value for when it is more appropriate to shift
        // elements rather than resizing the array to enqueue a new value.
        private const int SHIFT_THRESHOLD = 4;

        private sealed class Syncronized : ArrayBasedQueue<T>, ISyncronizedByObject
        {
            private readonly object sync_object;

            public Syncronized() : base() => sync_object = new();

            public object SyncObject => sync_object;

            public override void Enqueue([AllowNull] T item)
            {
                Monitor.Enter(sync_object);
                try {
                    base.Enqueue(item);
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

            public override IEnumerator<T> GetEnumerator()
            {
                Monitor.Enter(sync_object);
                try {
                    return ArrayEnumerator<T>.ByBoundsCopied(elements, head, count);
                } finally {
                    Monitor.Exit(sync_object);
                }
            }

            public override bool TryDequeue([MaybeNull] out T item)
            {
                Monitor.Enter(sync_object);
                try {
                    return base.TryDequeue(out item);
                } finally {
                    Monitor.Exit(sync_object);
                }
            }

            public override bool TryPeek([MaybeNull] out T value)
            {
                Monitor.Enter(sync_object);
                try {
                    return base.TryPeek(out value);
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
            }

            public override void EnqueueAll(IEnumerable<T> items)
            {
                Monitor.Enter(sync_object);
                try {
                    base.EnqueueAll(items);
                } finally {
                    Monitor.Exit(sync_object);
                }
            }

            public override void EnqueueAll(params T[] items)
            {
                Monitor.Enter(sync_object);
                try {
                    base.EnqueueAll(items);
                } finally {
                    Monitor.Exit(sync_object);
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
        /// Initializes a new, empty instance of the <see cref="ArrayBasedQueue{T}"/> class.
        /// </summary>
        public ArrayBasedQueue()
        {
            tail = -1;
            count = 0;
            head = -1;
            elements = Array.Empty<T>();
        }

        /// <summary>
        /// Initializes a new, empty instance of the <see cref="ArrayBasedQueue{T}"/> class, with the specified initial capacity.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="capacity"/> is negative.</exception>
        [Throws(typeof(ArgumentOutOfRangeException))]
        public ArrayBasedQueue(int capacity)
        {
            if (capacity < 0) {
                throw new ArgumentOutOfRangeException(nameof(capacity), "Capacity cannot be a negative number.");
            } else {
                tail = -1;
                count = 0;
                head = -1;
                elements = new T[capacity];
            }
        }

        /// <summary>Creates a thread-safe queue.</summary>
        /// <returns>An object extending the <see cref="ArrayBasedQueue{T}"/> class and is thread-safe.</returns>
        public static ArrayBasedQueue<T> CreateSyncronized() => new Syncronized();

        /// <inheritdoc />
        public virtual T this[int index]
        {
            get {
                if (index < 0) {
                    throw new ArgumentOutOfRangeException(nameof(index), "Index cannot be a negative value.");
                } else if (index >= count) {
                    throw new ArgumentOutOfRangeException(nameof(index), "Index cannot be larger than the queue's current bounds.");
                } else {
                    return elements[head+index];
                }
            }
            set => throw new NotSupportedException("This operation is not supported on queue-based collections.");
        }

        /// <inheritdoc />
        public int Count => count;

        /// <inheritdoc />
        public virtual void Clear()
        {
            head = tail = -1;
            count = 0;
        }

        /// <inheritdoc />
        public virtual void Enqueue([AllowNull] T item)
        {
            if (count == 0) {
                if (elements.Length == 0) {
                    elements = new T[1] { item };
                } else {
                    elements[0] = item;
                }
                tail = head = 0;
            } else if (tail + 1 < elements.Length) {
                PutAtTail(item);
            } else {
                // OK. We need to enlarge the array.
                // Let's see first if we can avoid the enlarge by shifting the elements.
                // Note: shifting is not best to be done at all times because we must
                // consume time to shift ALL the elements at the beginning.
                // Since we already need that time, it is best to enlarge and shift at the same time instead.
                if (head > SHIFT_THRESHOLD) {
                    // We can avoid the enlarge, shift the elements.
                    ShiftElements();
                } else {
                    EnlargeAndShift(1);
                }
                // Now we can put our element
                PutAtTail(item);
            }
            count++;
        }

        /// <inheritdoc />
        public virtual void EnsureCapacity(int n_elements)
        {
            if (n_elements < 0) {
                throw new ArgumentOutOfRangeException(nameof(n_elements), "Number of elements to be ensured of should not be negative.");
            } else {
                EnlargeAndShift(n_elements);
            }
        }

        // This is just primitive instructions, we can replace this method call with it's body.
        // It is just defined for convenience, nothing else.
        [MethodImpl(MethodImplOptions.AggressiveInlining)] 
        private void PutAtTail(T item) => elements[++tail] = item;

        // Shifts ALL the queue's elements at the beginning.
        // It updates head and tail values once done.
        private void ShiftElements()
        {
            if (count == 0) { return; } // No meaning to execute if we do not have any items to process
            for (int I = head, J = 0; I <= tail; I++)
            {
                elements[J++] = elements[I];
            }
            head = 0;
            tail = count - 1;
        }

        // Enlarges the array + shifting it's valid elements to the beginning.
        // It updates head and tail values if the queue has valid data in it.
        private void EnlargeAndShift(int by)
        {
            int new_count = unchecked(count + by);
            if (new_count < 0) {
                // Overflow detected, throw
                throw new OverflowException("The list has reached it's maximum capacity.");
            } else if (new_count > elements.Length) {
                T[] copy = new T[count + by];
                if (count > 0)
                {
                    Array.Copy(elements, head, copy, 0, count);
                    head = 0;
                    tail = count - 1;
                }
                elements = copy;
            }
        }

        /// <inheritdoc />
        public virtual IEnumerator<T> GetEnumerator() => ArrayEnumerator<T>.ByBounds(elements, head, count);

        /// <inheritdoc />
        public virtual bool TryDequeue([MaybeNull] out T item)
        {
            if (count < 1) {
                item = default;
                return false;
            } else {
                item = elements[head++];
                count--;
                return true;
            }
        }

        /// <inheritdoc />
        public virtual bool TryPeek([MaybeNull] out T value)
        {
            if (count < 1) {
                value = default;
                return false;
            } else {
                value = elements[head];
                return true;
            }
        }

        /// <inheritdoc />
        public virtual void EnqueueAll(IEnumerable<T> items)
        {
            ArgumentNullException.ThrowIfNull(items);
            if (items is T[] array) {
                // This is our lucky day!
                // Just copy the array into our elements array.
                EnqueueArray(array);
            } else if (items is IList<T> list) {
                EnqueueList(list);
            } else if (items is ICollection<T> collection) {
                EnqueueCollection(collection);
            } else {
                foreach (T item in items) { Enqueue(item); }
            }
            // If it happens that the first enqueue happened through this method, the head might not be appropriately updated.
            // As such, we need to update it here ourselves.
            if (head == -1) { head = 0; }
        }

        /// <summary>
        /// Specialization of the <see cref="EnqueueAll(IEnumerable{T})"/> method, for enqueuing elements statically known.
        /// </summary>
        /// <param name="items">The items to enqueue on the queue.</param>
        [Throws(typeof(ArgumentNullException))]
        public virtual void EnqueueAll(params T[] items)
        {
            ArgumentNullException.ThrowIfNull(items);
            EnqueueArray(items);
            // If it happens that the first enqueue happened through this method, the head might not be appropriately updated.
            // As such, we need to update it here ourselves.
            if (head == -1) { head = 0; }
        }

        /// <summary>
        /// Removes the entries that are inaccessible because they were dequeued. <br />
        /// This optimizes the memory usage of the current object.
        /// </summary>
        public virtual void TrimExcess()
        {
            if (count == 0) {
                elements = Array.Empty<T>();
            } else {
                // Maybe we should call here the EnlargeAndShift method with ct == 0
                // but using ShiftElements followed by Array.Resize is more robust.
                ShiftElements();
                Array.Resize(ref elements, count);
            }
        }

        private void EnqueueArray(T[] array)
        {
            int array_len = array.Length;
            if (array_len == 0) { return; }
            // Check first that we have such space.
            // Check for enlarge measures - will be done automatically if needed for us
            EnlargeAndShift(array_len);
            // Now copy our elements and we are then done...
            // Special handling is required for tail == -1.
            Array.Copy(array, 0, elements, tail == -1 ? 0 : tail + 1, array_len);
            // Update tail...
            tail += array_len;
            // Update count...
            count += array_len;
        }

        private void EnqueueList(IList<T> list)
        {
            int ct = list.Count;
            if (ct == 0) { return; }
            // Check first that we have such space.
            // Check for enlarge measures - will be done automatically if needed for us
            EnlargeAndShift(ct);
            // Now copy our elements and we are then done...
            // Special handling is required for tail == -1.
            int offset = (tail == -1) ? 0 : tail + 1;
            for (int I = 0; I < ct; I++) {
                elements[offset + I] = list[I];
            }
            // Update tail...
            tail += ct;
            // Update count...
            count += ct;
        }

        private void EnqueueCollection(ICollection<T> collection)
        {
            int ct = collection.Count;
            if (ct == 0) { return; }
            // Check for enlarge measures - will be done automatically if needed for us
            EnlargeAndShift(ct);
            // Now copy our elements and we are then done...
            // Special handling is required for tail == -1.
            int I = (tail == -1) ? 0 : tail + 1;
            foreach (T item in collection) { elements[I++] = item; }
            // Update tail...
            tail += ct;
            // Update count...
            count += ct;
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}