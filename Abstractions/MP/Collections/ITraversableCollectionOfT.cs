
using MP.Annotations.CodeAnalysis;
using System.Diagnostics.CodeAnalysis;

namespace MP.Collections
{
    /// <summary>
    /// Specifies the base interface for traversable collections;
    /// that is, collections that their individual elements can be accessed by simple 32-bit integers.
    /// </summary>
    /// <typeparam name="T">The type of the elements to be traversed.</typeparam>
    public interface ITraversableCollection<T>
    {
        /// <summary>
        /// Gets the number of elements contained in the current traversable collection.
        /// </summary>
        public int Count { get; }

        /// <summary>Gets the specified item at the specified index.</summary>
        /// <param name="index">The item located at <paramref name="index"/>.</param>
        /// <returns>The item's value at <paramref name="index"/>.</returns>
        /// <exception cref="System.ArgumentOutOfRangeException"><paramref name="index"/> was negative or outside of the current collection bounds.</exception>
        [MaybeNull]
        public T this[int index] {
            [Throws(typeof(System.ArgumentOutOfRangeException))]
            get;
        }
    }
}