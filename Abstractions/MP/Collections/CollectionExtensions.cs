
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

        /// <summary>
        /// Converts an enumerable or another list to a disposable list.
        /// </summary>
        /// <typeparam name="T">The type of the elements supporting the <see cref="IDisposable"/> interface</typeparam>
        /// <param name="disposables">The enumerable to convert.</param>
        /// <returns>A list of items containing the ones provided by <paramref name="disposables"/> enumearable, and whose operations are specialized for <see cref="IDisposable"/> objects.</returns>
        public static DisposableList<T> AsDisposableList<T>(this IEnumerable<T> disposables)
            where T : IDisposable => new(disposables);

        /// <summary>
        /// Converts an enumerable or another list to a read-only list.
        /// </summary>
        /// <typeparam name="T">The type of the elements</typeparam>
        /// <param name="items">The enumerable to convert.</param>
        /// <returns>A list of items containing the ones provided by <paramref name="items"/> enumerable, and being read-only.</returns>
        public static ReadOnlyList<T> AsReadOnlyList<T>(this IEnumerable<T> items) => new(items);

    }
}