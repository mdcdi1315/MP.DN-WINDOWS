
using System;
using System.Collections;
using System.Collections.Generic;
using MP.Annotations.CodeAnalysis;
using System.Diagnostics.CodeAnalysis;

namespace MP.Collections
{
    /// <summary>
    /// Defines the base framework for working with tree node getters without having access to any generic instance.
    /// </summary>
    public interface ITreeNodeGetterAccessor
    {
        /// <summary>
        /// Gets a value whether this node getter is representing a root tree node, that is, a node without any parents.
        /// </summary>
        public bool IsRoot { get; }

        /// <summary>
        /// Gets the parent node getter of this node getter accessor. <br />
        /// This should return the object as a <see cref="ITreeNodeGetterAccessor"/> instance for further inspection.
        /// </summary>
        [MaybeNull]
        public ITreeNodeGetterAccessor Parent { get; }

        /// <summary>
        /// Gets an enumerable containing all the children node getters of this node getter.
        /// </summary>
        public IEnumerable<ITreeNodeGetterAccessor> Children {
            [return: MaybeReturnEmptyCollectionButNeverNull]
            get; 
        }
    }

    /// <summary>
    /// Defines an interface for accessing children and parent nodes in a tree node.
    /// </summary>
    /// <typeparam name="T">The actual tree node type.</typeparam>
    public interface ITreeNodeGetter<T> : ITreeNodeGetterAccessor
        where T : ITreeNodeGetter<T>
    {
        private sealed class WrappedEnumerable
            : IEnumerable<ITreeNodeGetterAccessor>
        {
            private readonly IEnumerable<T> original;

            public WrappedEnumerable(IEnumerable<T> original) => this.original = original;

            private sealed class WrappedEnumerator
                : IEnumerator<ITreeNodeGetterAccessor>
            {
                private readonly IEnumerator<T> original;

                public WrappedEnumerator(IEnumerator<T> original) => this.original = original;

                public ITreeNodeGetterAccessor Current => original.Current;

                object IEnumerator.Current => Current;

                public void Dispose() => original.Dispose();

                public bool MoveNext() => original.MoveNext();

                public void Reset() => original.Reset();
            }

            public IEnumerator<ITreeNodeGetterAccessor> GetEnumerator() => new WrappedEnumerator(original.GetEnumerator());

            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        }

        /// <summary>
        /// Gets the parent node getter of this node getter.
        /// </summary>
        [MaybeNull]
        public new T Parent { get; }

        /// <summary>
        /// Gets an enumerable containing all the children of this node getter.
        /// </summary>
        public new IEnumerable<T> Children {
            [return: MaybeReturnEmptyCollectionButNeverNull]
            get; 
        }

        ITreeNodeGetterAccessor ITreeNodeGetterAccessor.Parent => Parent;

        IEnumerable<ITreeNodeGetterAccessor> ITreeNodeGetterAccessor.Children => new WrappedEnumerable(Children);
    }

    /// <summary>
    /// Defines useful collection extensions for working with <see cref="ITreeNodeGetter{T}"/> instances.
    /// </summary>
    public static class TreeNodeGetterExtensions
    {
        /// <summary>
        /// Gets all the children nodes inheriting from this node getter and itself.
        /// </summary>
        /// <param name="getter"></param>
        /// <returns>A collection containing all the child nodes and itself.</returns>
        public static IEnumerable<T> ChildrenAndSelf<T>(this T getter)
            where T : ITreeNodeGetter<T>
        {
            ArrayBasedQueue<T> nodes = new(10);
            nodes.Enqueue(getter);
            while (nodes.TryDequeue(out T node))
            {
                yield return node;
                nodes.EnqueueAll(getter.Children);
            }
        }

        /// <summary>
        /// Finds all the child nodes that pass the specified predicate.
        /// </summary>
        /// <param name="getter"></param>
        /// <param name="predicate">The predicate to match against.</param>
        /// <returns>A collection of all the child nodes that match on the predicate specified.</returns>
        public static IEnumerable<T> FindChildNodes<T>(this T getter, Predicate<T> predicate)
            where T : ITreeNodeGetter<T>
        {
            ArgumentNullException.ThrowIfNull(predicate);
            foreach (var i in getter.ChildrenAndSelf())
            {
                if (predicate(i)) { yield return i; }
            }
        }

        /// <summary>Gets all the ancestor nodes and itself.</summary>
        /// <returns>A collection containing all the ancestor nodes of this node and itself.</returns>
        public static IEnumerable<T> AncestorsAndSelf<T>(this T getter)
            where T : ITreeNodeGetter<T>
        {
            yield return getter;
            var p = getter.Parent;
            while (!p.IsRoot)
            {
                yield return p;
                p = p.Parent;
            }
        }

        /// <summary>Gets all the ancestor nodes.</summary>
        /// <returns>A collection containing all the ancestor nodes of this node.</returns>
        public static IEnumerable<T> GetAncestors<T>(this T getter)
            where T : ITreeNodeGetter<T>
        {
            var p = getter.Parent;
            while (!p.IsRoot)
            {
                yield return p;
                p = p.Parent;
            }
        }

        /// <summary>
        /// Finds all the ancestor nodes that pass the specified predicate.
        /// </summary>
        /// <param name="getter"></param>
        /// <param name="predicate">The predicate to match against.</param>
        /// <returns>A collection of all the ancestor nodes that match on the predicate specified.</returns>
        public static IEnumerable<T> FindAncestorNodes<T>(this T getter, Predicate<T> predicate)
            where T : ITreeNodeGetter<T>
        {
            ArgumentNullException.ThrowIfNull(predicate);
            foreach (T ancestor in GetAncestors(getter)) {
                if (predicate(ancestor)) { yield return ancestor; }
            }
        }

        /// <summary>
        /// Gets an enumerator implementation able to enumerate through all the nodes and their children nodes.
        /// </summary>
        /// <typeparam name="T">The tree node getter to enumerate it's nodes.</typeparam>
        /// <param name="getter">The tree node getter instance.</param>
        /// <returns>A new <see cref="TreeNodeGetterEnumerator{T}"/> instance.</returns>
        public static TreeNodeGetterEnumerator<T> GetEnumerator<T>(this T getter)
            where T : ITreeNodeGetter<T>  => new(getter);
    }
}