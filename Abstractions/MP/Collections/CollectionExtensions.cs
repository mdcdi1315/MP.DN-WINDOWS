
using System;
using System.Collections.Generic;

namespace MP.Collections
{
    /// <summary>
    /// Defines extension methods around the <see cref="IEnumerable{T}"/> interface.
    /// </summary>
    public static class CollectionExtensions
    {

        /// <summary>
        /// Disposes all the items contained in the specified collection or array.
        /// </summary>
        /// <typeparam name="T">The type of the item to be found it's <see cref="IDisposable.Dispose"/> implementation.</typeparam>
        /// <param name="values">The objects to be disposed of.</param>
        public static void DisposeAll<T>(this IEnumerable<T> values)
            where T : IDisposable
        {
            foreach (var item in values) { item?.Dispose(); }
        }

    }
}