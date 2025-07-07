
using System;
using MP.Utilities;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace MP.Collections
{
    /// <summary>
    /// Supports the <see cref="ICollectableAttributeable"/> interface, 
    /// by providing a custom key-value pair for differentiating between <see cref="KeyValuePair{TKey,TValue}"/> and attributes of the <see cref="IAttributeable"/> interface. 
    /// </summary>
    public struct AttributeKeyValuePair : INullable
    {
        private string key;
        private object value;

        /// <summary>Creates a new key-value pair instance, describing an attribute.</summary>
        /// <param name="key">The attribute's key.</param>
        /// <param name="value">The attribute's value. Can be <see langword="null"/>.</param>
        /// <exception cref="ArgumentNullException"><paramref name="key"/> was <see langword="null"/>.</exception>
        public AttributeKeyValuePair(string key, object value)
        {
            ArgumentNullException.ThrowIfNull(key);
            this.key = key;
            this.value = value;
        }

        /// <summary>
        /// Gets the key of the attribute, or the attribute's name.
        /// </summary>
        public readonly string Key => key;

        /// <summary>
        /// Gets the currently defined value of the attribute.
        /// </summary>
        [MaybeNull]
        public readonly object Value => value;

        /// <summary>
        /// A 'null' key value pair is any key-value pair that it's <see cref="Key"/> property is <see langword="null"/>. <br />
        /// Such case occurs by the <see langword="default"/> keyword or the parameterless constructor.
        /// </summary>
        public readonly bool IsNull => key is null;
    }
}