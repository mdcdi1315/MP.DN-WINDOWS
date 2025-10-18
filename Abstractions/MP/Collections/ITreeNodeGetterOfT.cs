
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using MP.Annotations.CodeAnalysis;

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
        public ITreeNodeGetterAccessor Parent { get; }

        /// <summary>
        /// Gets an enumerable containing all the children node getters of this node getter.
        /// </summary>
        public IEnumerable<ITreeNodeGetterAccessor> Children { get; }
    }

    /// <summary>
    /// Defines an interface for accessing children and parent nodes in a tree node.
    /// </summary>
    /// <typeparam name="T">The actual tree node type.</typeparam>
    public interface ITreeNodeGetter<T> : ITreeNodeGetterAccessor
        where T : ITreeNodeGetter<T>
    {
        /// <summary>
        /// Gets the parent node getter of this node getter.
        /// </summary>
        public new T Parent { get; }

        /// <summary>
        /// Gets an enumerable containing all the children of this node getter.
        /// </summary>
        public new IEnumerable<T> Children {
            [return: MaybeReturnEmptyCollectionButNeverNull]
            get; 
        }
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
            Queue<T> nodes = new(10);
            nodes.Enqueue(getter);
            while (nodes.TryDequeue(out var node))
            {
                yield return node;
                foreach (var c in getter.Children) { nodes.Enqueue(c); }
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
            if (getter.IsRoot) {
                yield break;
            } else {
                var p = getter.Parent;
                while (!getter.IsRoot)
                {
                    yield return p;
                    p = p.Parent;
                }
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