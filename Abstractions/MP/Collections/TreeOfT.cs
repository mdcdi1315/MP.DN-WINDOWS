

using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace MP.Collections
{
    /// <summary>
    /// A special collection for creating trees.
    /// </summary>
    /// <typeparam name="T">The type of the elements that this tree will hold.</typeparam>
    public sealed class Tree<T> : IEnumerable<T>
    {
        /// <summary>
        /// Defines a convenient enumerator class for <see cref="Tree{T}"/> objects.
        /// </summary>
        public sealed class Enumerator : IEnumerator<T>
        {
            private TreeNode<T> root, current;
            private Queue<TreeNode<T>> nodes;

            /// <summary>
            /// Creates a new tree enumerator, by starting the enumeration from the specified tree node.
            /// </summary>
            /// <param name="root">The root node where enumeration will begin from.</param>
            public Enumerator(TreeNode<T> root)
            {
                ArgumentNullException.ThrowIfNull(root);
                this.root = root;
                current = null;
                nodes = new(10);
                nodes.Enqueue(root);
            }

            /// <summary>
            /// Gets the node at the current position.
            /// </summary>
            public TreeNode<T> CurrentNode => current;

            /// <summary>
            /// Gets the current value of the current node.
            /// </summary>
            public T Current => current.Value;

            object IEnumerator.Current => Current;

            /// <summary>
            /// Moves to a next node in the tree.
            /// </summary>
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
                current = null;
                nodes.Clear();
                nodes.Enqueue(root);
            }

            /// <summary>
            /// Cleans internal state used by this <see cref="Enumerator"/> instance.
            /// </summary>
            public void Dispose()
            {
                current = null;
                nodes.Clear();
                nodes = null;
            }
        }

        private TreeNode<T> root;

        /// <summary>
        /// Creates an empty tree collection.
        /// </summary>
        public Tree() => root = null;

        // This can be inlined since the method itself is very explicative of what it does,
        // just it does exist to avoid calling 2 times the same thing when being on the GetEnumerator method.
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private TreeNode<T> GetNodePrivate() => root ??= new();

        /// <summary>
        /// Gets the root node of the tree.
        /// </summary>
        public TreeNode<T> Root => GetNodePrivate();

        /// <summary>
        /// Clears all the elements from the current tree, including the root node.
        /// </summary>
        public void Clear() => root = null;

        /// <summary>
        /// Gets an enumerator that can project all the tree elements as a simple collection.
        /// </summary>
        /// <returns>A new <see cref="Enumerator"/> instance, that starts enumerating from the root node of this tree.</returns>
        public Enumerator GetEnumerator() => new(GetNodePrivate());

        IEnumerator<T> IEnumerable<T>.GetEnumerator() => GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        /// <summary>
        /// Gets a string containing all the element values of the current tree. <br />
        /// For debugging purposes only.
        /// </summary>
        /// <returns>A string containing all the element values.</returns>
        public override System.String ToString()
        {
            System.String comma = ", ";
            System.Text.StringBuilder sb = new(2048);
            sb.Append("Tree<");
            sb.Append(typeof(T).FullName);
            sb.Append("> { ");
            foreach (T item in this)
            {
                sb.Append(item);
                sb.Append(", ");
            }
            if (sb.Length > comma.Length)
            {
                sb.Remove(sb.Length - comma.Length, comma.Length);
            }
            sb.Append(" }");
            return sb.ToString();
        }
    }
}