

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace MP.Collections
{
    /// <summary>Specifies a B-Tree node.</summary>
    /// <typeparam name="T">The type of value the B-Tree node holds.</typeparam>
    public sealed class BTreeNode<T> : ITreeNodeGetter<BTreeNode<T>>
    {
        private T value;
        private BTreeNode<T> left, right;
        private readonly BTreeNode<T> parent;

        /// <summary>Creates a new B-Tree node.</summary>
        public BTreeNode()
        {
            value = default;
            left = null;
            right = null;
            parent = null;
        }

        /// <summary>Creates a new B-Tree node, which has the specified parent node.</summary>
        /// <param name="parent">The B-Tree node that will be the parent of this node.</param>
        public BTreeNode(BTreeNode<T> parent) : this()
        {
            ArgumentNullException.ThrowIfNull(parent);
            this.parent = parent;
        }

        /// <summary>
        /// Gets/sets the value of this B-Tree node.
        /// </summary>
        public T Value
        {
            get => value;
            set => this.value = value;
        }

        /// <summary>Creates a left child node that will have the specified value.</summary>
        /// <param name="value">The value of the left child node.</param>
        /// <returns>The <see cref="BTreeNode{T}"/> created as a left-child node of the current B-Tree node.</returns>
        /// <exception cref="InvalidOperationException">A left child has been already specified.</exception>
        public BTreeNode<T> CreateLeftChild(T value)
        {
            if (left is not null) {
                throw new InvalidOperationException("A left child node has already been assigned!");
            } else {
                BTreeNode<T> n = new(this);
                n.value = value;
                left = n;
                return n;
            }
        }

        /// <summary>Creates a right child node that will have the specified value.</summary>
        /// <param name="value">The value of the right child node.</param>
        /// <returns>The <see cref="BTreeNode{T}"/> created as a right-child node of the current B-Tree node.</returns>
        /// <exception cref="InvalidOperationException">A right child has been already specified.</exception>
        public BTreeNode<T> CreateRightChild(T value)
        {
            if (right is not null) {
                throw new InvalidOperationException("A right child node has already been assigned!");
            } else {
                BTreeNode<T> n = new(this);
                n.value = value;
                right = n;
                return n;
            }
        }

        /// <inheritdoc />
        [MaybeNull]
        public BTreeNode<T> Parent => parent;

        /// <summary>Gets the left child node of this B-Tree node.</summary>
        [MaybeNull]
        public BTreeNode<T> LeftChild => left;

        /// <summary>Gets the right child node of this B-Tree node.</summary>
        [MaybeNull]
        public BTreeNode<T> RightChild => right;

        /// <inheritdoc />
        public IEnumerable<BTreeNode<T>> Children
        {
            get {
                ArrayBuilder<BTreeNode<T>> b = new();
                if (left is not null) { b.Add(left); }
                if (right is not null) { b.Add(right); }
                return b.Build();
            }
        }

        /// <inheritdoc />
        public bool IsRoot => parent is null;
    }
}