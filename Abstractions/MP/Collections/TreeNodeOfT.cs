

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace MP.Collections
{
    /// <summary>Specifies a single node in a tree collection.</summary>
    /// <typeparam name="T">The backing type of the value of the new node.</typeparam>
    public sealed class TreeNode<T> : ITreeNodeGetter<TreeNode<T>>
    {
        private T value;
        private List<TreeNode<T>> children;
        private readonly TreeNode<T> parent;

        internal TreeNode()
        {
            parent = null;
            children = null;
        }

        internal TreeNode(TreeNode<T> parent, T value) : this()
        {
            this.parent = parent;
            this.value = value;
        }

        /// <summary>
        /// Gets a reference to the parent node of the current node.
        /// </summary>
        [MaybeNull]
        public TreeNode<T> Parent => parent;

        /// <summary>
        /// Gets a value whether this tree node is the root node. <br />
        /// Root tree nodes are considered all those nodes that do not have parent nodes.
        /// </summary>
        public System.Boolean IsRoot => parent is null;

        /// <summary>
        /// Gets or sets the value of this tree node.
        /// </summary>
        public T Value
        {
            get => value;
            set => this.value = value;
        }

        /// <summary>
        /// Gets the child tree nodes of this tree node.
        /// </summary>
        public IEnumerable<TreeNode<T>> Children => (IEnumerable<TreeNode<T>>)children ?? new EmptyEnumerable<TreeNode<T>>();

        IEnumerable<ITreeNodeGetterAccessor> ITreeNodeGetterAccessor.Children => Children;

        /// <summary>
        /// Creates a new child node with the specified initial value and appends it to the children nodes of the current node. 
        /// That means that the current node is it's parent.
        /// </summary>
        /// <param name="value">The value that the new node should have.</param>
        /// <returns>The created tree node.</returns>
        public TreeNode<T> CreateChildNode(T value)
        {
            var t = new TreeNode<T>(this, value);
            (children ??= new(2)).Add(t);
            return t;
        }

        /// <summary>
        /// Creates new child nodes, each one having it's respected value. <br />
        /// All the nodes that will be created will have this node as their parent.
        /// </summary>
        /// <param name="values">The values of the new children nodes.</param>
        /// <returns>The created tree nodes.</returns>
        public IEnumerable<TreeNode<T>> CreateChildNodes(params T[] values)
        {
            List<TreeNode<T>> added = new(values.Length);
            if (children is null) {
                children = new List<TreeNode<T>>(added.Count);
            } else {
                children.EnsureCapacity(children.Count + added.Count);
            }
            TreeNode<T> temp;
            foreach (T childvalue in values) {
                children.Add(temp = new(this, childvalue));
                added.Add(temp);
            }
            return added;
        }

        /// <summary>
        /// Creates a new child node and appends it to the children nodes of the current node. 
        /// That means that the current node is it's parent.
        /// </summary>
        /// <returns>The created tree node.</returns>
        public TreeNode<T> CreateChildNode()
        {
            var t = new TreeNode<T>(this, default);
            (children ??= new(2)).Add(t);
            return t;
        }

        /// <summary>
        /// Gets all the children nodes inheriting from this node and itself.
        /// </summary>
        /// <returns>A collection containing all the child nodes and itself.</returns>
        public IEnumerable<TreeNode<T>> ChildrenAndSelf()
        {
            List<TreeNode<T>> tempchildren;
            ArrayBasedQueue<TreeNode<T>> nodes = new(10);
            nodes.Enqueue(this);
            while (nodes.TryDequeue(out var node))
            {
                yield return node;
                if ((tempchildren = node.children) is not null) { nodes.EnqueueAll(tempchildren); }
            }
        }

        /// <summary>
        /// Gets all the ancestor nodes and itself.
        /// </summary>
        /// <returns>A collection containing all the ancestor nodes of this node and itself.</returns>
        public IEnumerable<TreeNode<T>> AncestorsAndSelf()
        {
            yield return this;
            var p = parent;
            while (p is not null)
            {
                yield return p;
                p = p.parent;
            }
        }

        /// <summary>
        /// Finds all the child nodes whose value passes the specified predicate.
        /// </summary>
        /// <param name="predicate">The predicate to match against.</param>
        /// <returns>A collection of all the child nodes that match on the predicate specified.</returns>
        public IEnumerable<TreeNode<T>> FindChildNodes(Predicate<T> predicate)
        {
            ArgumentNullException.ThrowIfNull(predicate);
            foreach (var i in ChildrenAndSelf())
            {
                if (predicate(i.Value)) { yield return i; }
            }
        }

        /// <summary>
        /// Finds all the child nodes that pass the specified predicate.
        /// </summary>
        /// <param name="predicate">The predicate to match against.</param>
        /// <returns>A collection of all the child nodes that match on the predicate specified.</returns>
        public IEnumerable<TreeNode<T>> FindChildNodes(Predicate<TreeNode<T>> predicate)
        {
            ArgumentNullException.ThrowIfNull(predicate);
            foreach (var i in ChildrenAndSelf())
            {
                if (predicate(i)) { yield return i; }
            }
        }

        /// <summary>
        /// Finds all the child nodes whose values are equal to <paramref name="value"/>. <br />
        /// Comparison is done by using the default comparer.
        /// </summary>
        /// <param name="value">The value to test against all the other nodes.</param>
        /// <returns>A collection of all the child nodes that match on the value specified.</returns>
        public IEnumerable<TreeNode<T>> FindChildNodes([MaybeNull] T value) => FindChildNodes(value, EqualityComparer<T>.Default);

        /// <summary>
        /// Finds all the child nodes whose values are equal to <paramref name="value"/>.
        /// </summary>
        /// <param name="value">The value to test against all the other nodes.</param>
        /// <param name="comparer">The comparer to use for comparing all the values.</param>
        /// <returns>A collection of all the child nodes that match on the value and comparer specified.</returns>
        public IEnumerable<TreeNode<T>> FindChildNodes([MaybeNull] T value, IEqualityComparer<T> comparer)
        {
            ArgumentNullException.ThrowIfNull(comparer);
            foreach (var i in ChildrenAndSelf())
            {
                if (comparer.Equals(i.Value, value)) { yield return i; }
            }
        }

        /// <summary>
        /// Gets a string containing all the element values of the current tree node and it's children. <br />
        /// For debugging purposes only.
        /// </summary>
        /// <returns>A string containing all the element values.</returns>
        public override System.String ToString()
        {
            System.String comma = ", ";
            System.Text.StringBuilder sb = new(2048);
            sb.Append("TreeNode<");
            sb.Append(typeof(T).FullName);
            sb.Append("> { ");
            foreach (TreeNode<T> item in ChildrenAndSelf())
            {
                sb.Append(item.Value);
                sb.Append(comma);
            }
            if (sb.Length > comma.Length) {
                sb.Remove(sb.Length - comma.Length, comma.Length);
            }
            sb.Append(" }");
            return sb.ToString();
        }
    }
}