
using System;
using MP.Utilities;
using MP.Annotations;
using MP.ExceptionSystem;
using System.Collections.Generic;
using MP.Annotations.CodeAnalysis;
using System.Diagnostics.CodeAnalysis;

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
        /// <remarks>
        /// Due to the fact that this method is powerful and can break encapsulation and/or parity with some custom classes that implement <see cref="IEnumerable{T}"/>,
        /// it is advised to mark them with the <see cref="DoesNotSupportDisposeAllExtensionMethodAttribute"/> attribute.
        /// </remarks>
        /// <exception cref="AggregateException">One or more exceptions occurred while executing a dispose method.</exception>
        /// <exception cref="NotSupportedException">Due to how <paramref name="values"/> is coded, executing this could behave unexpectedly.</exception>
        [Throws(typeof(NotSupportedException), typeof(AggregateException))]
        public static void DisposeAll<T>(this IEnumerable<T> values)
            where T : IDisposable
        {
            if (values.GetType().HasAttribute<DoesNotSupportDisposeAllExtensionMethodAttribute>()) {
                throw new NotSupportedException("The current instance is not allowed to dispose it's elements because it could break it's encapsulation.");
            } else {
                AggregateExceptionBuilder builder = new();
                builder.Message = "One or more elements failed to be properly disposed of.";
                foreach (T item in values) {
                    try {
                        item?.Dispose(); 
                    } catch (Exception ex) {
                        builder.Add(ex);
                    }
                }
                builder.ThrowIfHasExceptions();
            }
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

        /// <summary>Copies all the elements of the current collection object to a new array.</summary>
        /// <typeparam name="T">The type of elements to be copied to a new array.</typeparam>
        /// <param name="collection">The collection to copy the elements from.</param>
        /// <returns>A new array of type <typeparamref name="T"/> that has a size as reported by the <see cref="ICollection{T}.Count"/> property and contains the elements of <paramref name="collection"/>.</returns>
        /// <exception cref="NotSupportedException">This requires the <see cref="ICollection{T}.CopyTo(T[], int)"/> call to have been defined; the exception from that call is returned.</exception>
        /// <exception cref="OutOfMemoryException">Could not create an array that can contain all the elements of the current collection.</exception>
        [Throws(typeof(NotSupportedException), typeof(OutOfMemoryException))]
        public static T[] ToArray<T>(this ICollection<T> collection)
        {
            T[] new_arr = new T[collection.Count];
            collection.CopyTo(new_arr, 0);
            return new_arr;
        }

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
        /// If the specified dictionary contains the <paramref name="key"/>, it retruns the existing value for <paramref name="key"/>. <br />
        /// Otherwise, it computes the value of <paramref name="key"/> by the specified <paramref name="computing_action"/>, saves it to the dictionary and returns that instead.
        /// </summary>
        /// <typeparam name="TK">The type of the keys the dictionary does retain.</typeparam>
        /// <typeparam name="TV">The type of the values the dictionary does retain by the specified <typeparamref name="TK"/>.</typeparam>
        /// <param name="dict">The dictionary to compute or retrieve the specified value by the specified <paramref name="key"/>.</param>
        /// <param name="key">The key to retrieve it's value.</param>
        /// <param name="computing_action">The action that if the specified entry by the specified <paramref name="key"/> does not exist, it provides the value of the entry.</param>
        /// <returns>The value of the specified <paramref name="key"/> either that was retrieved by the dictionary or it was created by the specified <paramref name="computing_action"/>.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="key"/> and/or <paramref name="computing_action"/> are <see langword="null"/>.</exception>
        [return: MaybeNull]
        [Throws(typeof(ArgumentNullException))]
        public static TV ComputeIfAbsent<TK, TV>(this IDictionary<TK, TV> dict, TK key, Func<TK, TV> computing_action)
        {
            ArgumentNullException.ThrowIfNull(computing_action);
            if (!dict.TryGetValue(key, out TV value)) {
                dict.Add(key, value = computing_action(key));
            }
            return value;
        }

        /// <summary>
        /// Converts this <see cref="KeyValuePair{TKey, TValue}"/> structure to a <see cref="System.Collections.DictionaryEntry"/> structure.
        /// </summary>
        /// <typeparam name="TK">The type of the key applied to the current <see cref="KeyValuePair{TKey, TValue}"/> instance.</typeparam>
        /// <typeparam name="TV">The type of the value applied to the current <see cref="KeyValuePair{TKey, TValue}"/> instance.</typeparam>
        /// <param name="kvp">The key-value pair to convert.</param>
        /// <returns>The converted <see cref="System.Collections.DictionaryEntry"/> structure.</returns>
        public static System.Collections.DictionaryEntry ToDictionaryEntry<TK, TV>(this KeyValuePair<TK, TV> kvp) => new(kvp.Key, kvp.Value);

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

        /// <summary>
        /// Determines whether the <see cref="ITraversableRegister{T}"/> contains a specific value.
        /// </summary>
        /// <param name="reg">The <see cref="ITraversableRegister{T}"/> instance to inspect.</param>
        /// <param name="item">The object to locate in the <see cref="ITraversableRegister{T}"/>.</param>
        /// <returns><see langword="true"/> if <paramref name="item"/> is found in the <see cref="ITraversableRegister{T}"/>; otherwise, <see langword="false"/>.</returns>
        public static bool Contains<T>(this ITraversableRegister<T> reg, [AllowNull] T item) => reg.IndexOf(item) > -1;
    }
}