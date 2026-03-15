
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace MP.Collections
{
    /// <summary>
    /// Provides a layout over how generic dictionary enumerators should be. <br />
    /// This class also implements the <see cref="IDictionaryEnumerator"/> interface.
    /// </summary>
    /// <typeparam name="TKey">The type of keys that are to be enumerated.</typeparam>
    /// <typeparam name="TValue">The type of values that are to be enumerated.</typeparam>
    public abstract class GenericDictionaryEnumerator<TKey, TValue> : BaseEnumerator<KeyValuePair<TKey, TValue>>, IDictionaryEnumerator
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GenericDictionaryEnumerator{TKey, TValue}"/> class.
        /// </summary>
        protected GenericDictionaryEnumerator() : base() { }

        DictionaryEntry IDictionaryEnumerator.Entry => new(Current.Key, Current.Value);

        object IDictionaryEnumerator.Key => Current.Key;

        object IDictionaryEnumerator.Value => Current.Value;

        /// <summary>
        /// Gets the key of the current dictionary entry.
        /// </summary>
        /// <returns>The key of the current element of the enumeration.</returns>
        /// <exception cref="InvalidOperationException">The <see cref="GenericDictionaryEnumerator{TKey, TValue}"/> is positioned before the first entry of the dictionary or after the last entry.</exception>
        public TKey Key => Current.Key;

        /// <summary>Gets the value of the current dictionary entry.</summary>
        /// <returns>The value of the current element of the enumeration.</returns>
        /// <exception cref="InvalidOperationException">The <see cref="GenericDictionaryEnumerator{TKey, TValue}"/> is positioned before the first entry of the dictionary or after the last entry.</exception>
        public TValue Value => Current.Value;

        /// <inheritdoc />
        [MaybeNull]
        public override abstract KeyValuePair<TKey, TValue> Current { get; }
    }
}