
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace MP.Collections
{
    /// <summary>
    /// A record is an object implementing the <see cref="ICollectableAttributeable"/> interface
    /// to support immutable fields and data that cannot be changed throughout the object's lifetime.
    /// </summary>
    public sealed class Record : ICollectableAttributeable, ISyncronized
    {
        private sealed class RecordEnumerator : IEnumerator<AttributeKeyValuePair>
        {
            private IEnumerator<KeyValuePair<System.String, System.Object>> e;

            public RecordEnumerator(IEnumerator<KeyValuePair<System.String, System.Object>> en) => e = en;

            public AttributeKeyValuePair Current
            {
                get
                {
                    var c = e.Current;
                    return new(c.Key, c.Value);
                }
            }

            object IEnumerator.Current => Current;

            public bool MoveNext() => e.MoveNext();

            public void Reset() => e.Reset();

            public void Dispose() => e.Dispose();
        }

        private readonly Dictionary<System.String, System.Object> data;

        private Record() => data = null;

        /// <summary>
        /// Creates a new record instance from the specified enumerable that contains attribute key-value pair data.
        /// </summary>
        /// <param name="enumerable">The enumerable containing the data to initialize the record.</param>
        public Record(IEnumerable<AttributeKeyValuePair> enumerable)
        {
            ArgumentNullException.ThrowIfNull(enumerable);
            if (enumerable is ICollection<AttributeKeyValuePair> pl) {
                data = new(pl.Count);
            } else if (enumerable is ICollectableAttributeable attr) {
                data = new(attr.Count);
            } else {
                data = new(10);
            }
            foreach (var i in enumerable)
            {
                data.Add(i.Key, i.Value);
            }
        }

        /// <summary>
        /// Creates a new record instance from the specified enumerable that contains attribute key-value pair data.
        /// </summary>
        /// <param name="enumerable">The enumerable containing the data to initialize the record.</param>
        public Record(IEnumerable<KeyValuePair<System.String, System.Object>> enumerable)
        {
            ArgumentNullException.ThrowIfNull(enumerable);
            switch (enumerable)
            {
                case ICollection<KeyValuePair<System.String, System.Object>> c:
                    data = new(c);
                    break;
                default:
                    data = new(10);
                    foreach (var i in enumerable) { data.Add(i.Key, i.Value); }
                    break;
            }
        }

        /// <summary>
        /// Statically initializes a <see cref="Record"/> with the specified attributes.
        /// </summary>
        /// <param name="attributes">The set of attributes to initialize this record from.</param>
        /// <returns>A new <see cref="Record"/> containing the attributes specified by <paramref name="attributes"/>.</returns>
        public static Record With(params AttributeKeyValuePair[] attributes) => new(attributes ?? Array.Empty<AttributeKeyValuePair>());

        /// <summary>Gets a <see cref="Record"/> instance that is empty.</summary>
        public static Record Empty => new();

        /// <summary>
        /// Gets the number of fields contained in the current record.
        /// </summary>
        public int Count => data is null ? 0 : data.Count;

        /// <summary>
        /// Gets all the field names defined in the current record.
        /// </summary>
        [NotNull]
        public IEnumerable<System.String> Keys => data is null ? new EmptyEnumerable<System.String>() : data.Keys;

        /// <summary>Gets an enumerator that can iterate through all the attributes in the record.</summary>
        /// <returns>An enumerator returning <see cref="AttributeKeyValuePair"/> instances, representing each one of the attributes existing in this record.</returns>
        [return: NotNull]
        public IEnumerator<AttributeKeyValuePair> GetEnumerator() => data is null ? new EmptyEnumerator<AttributeKeyValuePair>() : new RecordEnumerator(data.GetEnumerator());

        /// <summary>
        /// A <see cref="Record"/> instance is immutable and cannot be modfied directly.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <exception cref="NotSupportedException">Attempted to modify the record directly.</exception>
        [DoesNotReturn]
        public void SetAttribute(System.String name, System.Object value)
            => throw new NotSupportedException("Cannot modify a stable record.");

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        
        /// <inheritdoc />
        public bool TryGetAttribute(System.String attribute, [NotNullWhen(true)] out System.Object value)
        {
            ArgumentNullException.ThrowIfNull(attribute);
            if (data is null) { goto g_fail; }
            foreach (var attr in data)
            {
                if (attr.Key == attribute)
                {
                    value = attr.Value;
                    return true;
                }
            }
        g_fail:
            value = null;
            return false;
        }
    }
}