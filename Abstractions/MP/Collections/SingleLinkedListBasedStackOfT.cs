
using System;
using System.Threading;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace MP.Collections
{
    /// <summary>
    /// An implementation of the <see cref="ITraversableStack{T}"/> interface by using a technique similar to the <see cref="LinkedList{T}"/> class implementation.
    /// </summary>
    /// <typeparam name="T">The type of the elements that this stack will hold.</typeparam>
    public class SingleLinkedListBasedStack<T> : ITraversableStack<T>
    {
        private sealed class Node
        {
            [MaybeNull]
            public Node Parent;
            public readonly T Value;
            
            public Node(T value) => Value = value;

            public Node(T value, Node parent) : this(value) => Parent = parent;
        }

        private sealed class Enumerator : IEnumerator<T>
        {
            private bool finished;
            private Node point, current;

            public Enumerator(Node point)
            {
                this.point = point;
                current = null;
                finished = false;
            }

            public T Current => (current is null) ? default : current.Value;

            object IEnumerator.Current => (current is null) ? default : current.Value;

            public void Dispose()
            {
                point = null;
                current = null;
                finished = true;
            }

            public bool MoveNext()
            {
                if (finished) { 
                    return false; 
                } else if ((current = (current is null) ? point : current.Parent) is null) {
                    finished = true;
                    return false;
                } else {
                    return true;
                }
            }

            public void Reset()
            {
                current = null;
                finished = false;
            }
        }

        private sealed class Syncronized : SingleLinkedListBasedStack<T>, ISyncronizedByObject
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
        }

        private int count;
        private Node current;

        /// <summary>
        /// Initializes a new instance of the <see cref="SingleLinkedListBasedStack{T}"/> class.
        /// </summary>
        public SingleLinkedListBasedStack()
        {
            count = 0;
            current = null;
        }

        /// <summary>Creates a thread-safe stack.</summary>
        /// <returns>An object extending the <see cref="SingleLinkedListBasedStack{T}"/> class and is thread-safe.</returns>
        public static SingleLinkedListBasedStack<T> CreateSyncronized() => new Syncronized();

        /// <inheritdoc />
        public virtual T this[int index]
        {
            get {
                if (index < 0) {
                    throw new ArgumentOutOfRangeException(nameof(index), "The specified index was negative.");
                } else if (index >= count) {
                    throw new ArgumentOutOfRangeException(nameof(index), "The specified index was out of the stack bounds.");
                } else {
                    Node p = current;
                    int t = 0;
                    while (t < index)
                    {
                        p = p.Parent;
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
            current = null;
            count = 0;
        }

        /// <summary>
        /// Returns an eunumerator that is able to traverse all the pushed stack elements.
        /// </summary>
        /// <returns>An enumerator instance returning all the pushed elements.</returns>
        public virtual IEnumerator<T> GetEnumerator() => new Enumerator(current);

        /// <inheritdoc />
        public virtual void Push([MaybeNull] T value)
        {
            Node new_node = new(value, current);
            current = new_node;
            count++;
        }

        /// <inheritdoc />
        public virtual bool TryPop(out T value)
        {
            if (current is null) {
                value = default;
                return false;
            } else {
                count--;
                value = current.Value;
                current = current.Parent;
                return true;
            }
        }

        /// <inheritdoc />
        public virtual bool TryPeek(out T value)
        {
            if (current is null) {
                value = default;
                return false;
            } else {
                value = current.Value;
                return true;
            }
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
