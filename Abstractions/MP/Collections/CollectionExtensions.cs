
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

        /// <summary>Adds an existing key-value pair to the specified dictionary.</summary>
        /// <typeparam name="TK">The type of the keys that the dictionary holds.</typeparam>
        /// <typeparam name="TV">The type of the values for the keys that the dictionary holds.</typeparam>
        /// <param name="dict">The <see cref="IDictionary{TKey,TValue}"/> to add the specified key-value pair to.</param>
        /// <param name="kvp">The key value pair to add to the specified dictionary object.</param>
        /// <exception cref="ArgumentNullException">The <see cref="KeyValuePair{TK , TV}.Key"/> property was <see langword="null"/>.</exception>
        /// <exception cref="ArgumentException">The <see cref="KeyValuePair{TK , TV}.Key"/> property was already existing in the specified dictionary.</exception>
        /// <exception cref="NotSupportedException">The dictionary is read-only.</exception>
        public static void Add<TK, TV>(this IDictionary<TK, TV> dict, KeyValuePair<TK, TV> kvp) => dict.Add(kvp.Key, kvp.Value);

        /// <summary>Adds an existing attribute key-value pair to the specified dictionary.</summary>
        /// <param name="dict">The <see cref="IDictionary{TKey,TValue}"/> to add the specified key-value pair to.</param>
        /// <param name="pair">The attribute key value pair to add to the specified dictionary object.</param>
        /// <exception cref="ArgumentNullException">The <see cref="KeyValuePair{TK , TV}.Key"/> property was <see langword="null"/>.</exception>
        /// <exception cref="ArgumentException">The <see cref="KeyValuePair{TK , TV}.Key"/> property was already existing in the specified dictionary.</exception>
        /// <exception cref="NotSupportedException">The dictionary is read-only.</exception>
        public static void Add(this IDictionary<System.String, System.Object> dict, AttributeKeyValuePair pair) => dict.Add(pair.Key, pair.Value);

        /// <summary>
        /// Re-translates this mutable collection object as an immutable <see cref="Record"/>, which it does provide support for the <see cref="ICollectableAttributeable"/> interface.
        /// </summary>
        /// <param name="data">The collection object to mutate</param>
        /// <returns>A new immutable record instance.</returns>
        public static Record GetRecord(this ICollection<KeyValuePair<System.String, System.Object>> data) => new(data);

        /// <summary>
        /// Defines the property getter of the specified gettable/settable object as a method.
        /// </summary>
        /// <typeparam name="TIndex">The type of the index to use.</typeparam>
        /// <typeparam name="TOut">The type of the value retrieved through the indexer property.</typeparam>
        /// <param name="gs">The gettable/settable object to retrieve the specified value from.</param>
        /// <param name="index">The index of the object of type <typeparamref name="TOut"/> you wish to retrieve.</param>
        /// <returns>The value located at <paramref name="index"/>.</returns>
        public static TOut GetValue<TIndex, TOut>(this IGettableSettable<TIndex, TOut> gs, TIndex index) => gs[index];

        /// <summary>
        /// Defines the property setter of the specified gettable/settable object as a method.
        /// </summary>
        /// <typeparam name="TIndex">The type of the index to use.</typeparam>
        /// <typeparam name="TOut">The type of the value set through the indexer property.</typeparam>
        /// <param name="gs">The gettable/settable object to set the specified value to.</param>
        /// <param name="index">The index of the object of type <typeparamref name="TOut"/> you wish to be set to.</param>
        /// <param name="value">The value of type <typeparamref name="TOut"/> to set at <paramref name="index"/>.</param>
        public static void SetValue<TIndex, TOut>(this IGettableSettable<TIndex, TOut> gs, TIndex index, TOut value) => gs[index] = value;
    }
}