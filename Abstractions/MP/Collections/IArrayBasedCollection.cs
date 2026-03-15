
using System;
using MP.Annotations.CodeAnalysis;

namespace MP.Collections
{
    /// <summary>
    /// Provides common services to all the collection types that are implemented using fixed arrays.
    /// </summary>
    public interface IArrayBasedCollection
    {
        /// <summary>
        /// Removes the elements that cannot be accessed by any means through the public members of the type to reclaim memory. <br />
        /// This is done by resizing the backed array.
        /// </summary>
        public void TrimExcess();

        /// <summary>
        /// Attempts to ensure to the current object that <paramref name="n_elements"/> elements are existing and empty. <br />
        /// If not, the backing array must be appropriately resized.
        /// </summary>
        /// <param name="n_elements">The number of elements needed to be ensured of.</param>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="n_elements"/> is negative.</exception>
        /// <exception cref="OverflowException">Adding <paramref name="n_elements"/> would cause the collection to overflow.</exception>
        [Throws(typeof(ArgumentOutOfRangeException) , typeof(OverflowException))]
        public void EnsureCapacity(int n_elements);
    }
}