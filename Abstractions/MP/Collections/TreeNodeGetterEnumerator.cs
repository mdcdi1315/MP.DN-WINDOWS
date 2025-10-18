
using System;
using System.Collections;
using System.Collections.Generic;
using MP.Annotations.CodeAnalysis;

namespace MP.Collections
{
    /// <summary>
    /// Defines an extendable enumerator implementation for implementing traversal through tree nodes and their children.
    /// </summary>
    /// <typeparam name="T">The type of the tree nodes to traverse.</typeparam>
    public class TreeNodeGetterEnumerator<T> : IEnumerator<T>
        where T : ITreeNodeGetter<T>
    {
        private T root, current;
        private Queue<T> nodes;

        /// <summary>
        /// Creates a new instance of the <see cref="TreeNodeGetterEnumerator{T}"/> class 
        /// </summary>
        /// <param name="root">the root node to start traversing from.</param>
        /// <exception cref="ArgumentNullException"><paramref name="root"/> was <see langword="null"/>.</exception>
        [Throws(typeof(ArgumentNullException))]
        public TreeNodeGetterEnumerator(T root)
        {
            ArgumentNullException.ThrowIfNull(root);
            this.root = root;
            current = default;
            nodes = new(15);
            nodes.Enqueue(root);
        }

        /// <summary>
        /// Gets the node at the current position.
        /// </summary>
        public T Current => current;

        object IEnumerator.Current => Current;

        /// <summary>Moves to a next node in the tree.</summary>
        /// <returns><see langword="true"/> when a new child was retrieved; <see langword="false"/> if no more child elements are found.</returns>
        public bool MoveNext()
        {
            if (current is not null) {
                foreach (var c in current.Children) { nodes.Enqueue(c); }
            }
            return nodes.TryDequeue(out current);
        }

        /// <summary>
        /// Resets the enumerator before the root node was specified.
        /// </summary>
        public void Reset()
        {
            current = default;
            nodes.Clear();
            nodes.Enqueue(root);
        }

        /// <summary>
        /// Cleans internal state used by this <see cref="TreeNodeGetterEnumerator{T}"/> instance.
        /// </summary>
        public void Dispose()
        {
            current = default;
            nodes.Clear();
            nodes = null;
        }
    }
}