
using System;
using System.Threading;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace MP.Collections
{
    /// <summary>
    /// An implementation of the <see cref="ITraversableQueue{T}"/> interface by using a technique similar to the <see cref="LinkedList{T}"/> class implementation.
    /// </summary>
    /// <typeparam name="T">The type of the elements that this queue will hold.</typeparam>
    public class SingleLinkedListBasedQueue<T> : ITraversableQueue<T>
    {
        private sealed class Node
        {
            [AllowNull]
            public Node Before;
            
            public readonly T Value;

            public Node(T value, Node before)
            {
                Value = value;
                Before = before;
            }
        }

        private sealed class Enumerator : IEnumerator<T>
        {
            private Node head, current;

            public Enumerator(Node head)
            {
                this.head = head;
                current = null;
            }

            public T Current => current.Value;

            object IEnumerator.Current => current.Value;

            public void Dispose() => current = head = null;

            public bool MoveNext()
            {
                if (head is null) {
                    return false;
                } else if (current is null) {
                    current = head;
                    return true;
                } else {
                    return (current = current.Before) is not null;
                }
            }

            public void Reset() => current = null;
        }

        private sealed class Syncronized : SingleLinkedListBasedQueue<T>, ISyncronizedByObject
        {
            private readonly object sync_object;

            public Syncronized() : base() => sync_object = new object();

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
                    return base.GetEnumerator();
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
        }

        private int count;
        private Node tail, head;

        /// <summary>
        /// Initializes a new instance of the <see cref="SingleLinkedListBasedQueue{T}"/> class.
        /// </summary>
        public SingleLinkedListBasedQueue()
        {
            count = 0;
            tail = head = null;
        }

        /// <summary>Creates a thread-safe queue.</summary>
        /// <returns>An object extending the <see cref="SingleLinkedListBasedQueue{T}"/> class and is thread-safe.</returns>
        public static SingleLinkedListBasedQueue<T> CreateSyncronized() => new Syncronized();

        /// <inheritdoc />
        public virtual T this[int index]
        {
            get {
                if (index < 0) {
                    throw new ArgumentOutOfRangeException(nameof(index), "The specified index was negative.");
                } else if (index >= count) {
                    throw new ArgumentOutOfRangeException(nameof(index), "The specified index was out of the queue bounds.");
                } else {
                    Node p = head;
                    int t = 0;
                    while (t < index)
                    {
                        p = p.Before;
                        t++;
                    }
                    return p.Value;
                }
            }
        }

        /// <inheritdoc />
        public int Count => count;

        /// <inheritdoc />
        public virtual void Clear()
        {
            count = 0;
            tail = head = null;
        }

        /// <inheritdoc />
        public virtual void Enqueue([AllowNull] T item)
        {
            switch (count++)
            {
                case 0:
                    head = new(item, null);
                    break;
                case 1:
                    head.Before = tail = new(item, null);
                    break;
                default:
                    Node t = new(item, null);
                    tail.Before = t;
                    tail = t;
                    break;
            }
        }

        /// <inheritdoc />
        public virtual IEnumerator<T> GetEnumerator() => new Enumerator(head);

        /// <inheritdoc />
        public virtual bool TryDequeue([MaybeNull] out T item)
        {
            if (count < 1) {
                item = default;
                return false;
            } else {
                item = head.Value;
                head = head.Before;
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
                value = head.Value;
                return true;
            }
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}