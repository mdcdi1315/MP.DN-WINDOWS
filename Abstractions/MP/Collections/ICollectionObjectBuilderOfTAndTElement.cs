
using System;
using System.Collections.Generic;
using MP.Annotations.CodeAnalysis;

namespace MP.Collections
{
    /// <summary>
    /// Specialization of the <see cref="IObjectBuilder{T}"/> interface for creating collection objects.
    /// </summary>
    /// <typeparam name="T">The type of the collection to be returned.</typeparam>
    /// <typeparam name="TElement">The type of the elements that the newly created collection will have.</typeparam>
    public interface ICollectionObjectBuilder<T, TElement> : IObjectBuilder<T>
        where T : IEnumerable<TElement>
    {
        /// <summary>
        /// Adds an element to the collection builder.
        /// </summary>
        /// <param name="element">The element to be added.</param>
        void Add(TElement element);

        /// <summary>
        /// Adds a number of elements to the collection builder.
        /// </summary>
        /// <param name="elements">The elements to be added.</param>
        /// <exception cref="ArgumentNullException"><paramref name="elements"/> is <see langword="null"/>.</exception>
        void AddRange(IEnumerable<TElement> elements)
        {
            ArgumentNullException.ThrowIfNull(elements);
            foreach (var element in elements) { Add(element); }
        }

        /// <summary>
        /// If supported, it removes all the elements added so far.
        /// </summary>
        /// <exception cref="NotSupportedException">This operation is not supported.</exception>
        [Throws(typeof(NotSupportedException))]
        void Clear();
    }
}
