
using System;
using System.Collections;
using System.Collections.Generic;
using MP.Annotations.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace MP.Collections
{
    /// <summary>A special collection for creating B-Trees.</summary>
    /// <typeparam name="T">The type of the elements that this tree will hold.</typeparam>
    public sealed class BTree<T> :
        ITreeNodeGetter<BTreeNode<T>>,
        IEnumerable<T>
    {
        /// <summary>
        /// Defines a convenient enumerator class for <see cref="Tree{T}"/> objects.
        /// </summary>
        public sealed class Enumerator : TreeNodeGetterEnumerator<BTreeNode<T>>, IEnumerator<T>
        {
            /// <summary>
            /// Creates a new tree enumerator, by starting the enumeration from the specified tree node.
            /// </summary>
            /// <param name="root">The root node where enumeration will begin from.</param>
            /// <exception cref="ArgumentNullException"><paramref name="root"/> was <see langword="null"/>.</exception>
            [Throws(typeof(ArgumentNullException))]
            public Enumerator(BTreeNode<T> root) : base(root) { }

            T IEnumerator<T>.Current => Current.Value;

            /// <summary>
            /// Gets the value of the currently pointed to tree node.
            /// </summary>
            public T NodeValue => Current.Value;
        }

        private BTreeNode<T> root;

        /// <summary>Creates an empty B-Tree collection.</summary>
        public BTree() => root = null;

        // This can be inlined since the method itself is very explicative of what it does,
        // just it does exist to avoid calling 2 times the same thing when being on the GetEnumerator method.
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private BTreeNode<T> GetNodePrivate() => root ??= new();

        /// <summary>Gets the root node of the tree.</summary>
        public BTreeNode<T> Root => GetNodePrivate();

        BTreeNode<T> ITreeNodeGetter<BTreeNode<T>>.Parent => null; // This is the top of the tree, no other parents do exist!!

        bool ITreeNodeGetterAccessor.IsRoot => true; // This is always true!!!

        /// <summary>
        /// Gets the children of the root node of this tree. 
        /// </summary>
        /// <remarks>
        /// This is equivalent to getting the root node from the <see cref="Root"/> property and from that calling the <see cref="TreeNode{T}.Children"/> property.
        /// </remarks>
        public IEnumerable<BTreeNode<T>> Children => GetNodePrivate().Children;

        /// <summary>
        /// Clears all the elements from the current tree, including the root node.
        /// </summary>
        public void Clear() => root = null;

        /// <summary>Gets an enumerator that can project all the B-Tree elements as a simple collection.</summary>
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
            sb.Append("BTree<");
            sb.Append(typeof(T).FullName);
            sb.Append("> { ");
            foreach (T item in (IEnumerable<T>)this) { sb.Append(item); sb.Append(comma); }
            if (sb.Length > comma.Length) {
                sb.Remove(sb.Length - comma.Length, comma.Length);
            }
            sb.Append(" }");
            return sb.ToString();
        }
    }
}