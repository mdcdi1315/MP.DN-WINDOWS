

using System;
using System.Collections;
using MP.ExceptionSystem;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace MP.Collections
{
    /// <summary>
    /// A record is an object implementing the <see cref="ICollectableAttributeable"/> interface
    /// to support immutable fields and data that cannot be changed throughout the object's lifetime.
    /// </summary>
    public sealed class Record : ICollectableAttributeable
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

        private Dictionary<System.String, System.Object> data;

        /// <summary>
        /// Creates a new record instance from the specified enumerable that contains attribute key-value pair data.
        /// </summary>
        /// <param name="enumerable">The enumerable containing the data to initialize the record.</param>
        public Record(IEnumerable<AttributeKeyValuePair> enumerable)
        {
            ArgumentNullException.ThrowIfNull(enumerable);
            if (enumerable is ICollection<AttributeKeyValuePair> pl)
            {
                data = new(pl.Count);
            }
            else if (enumerable is ICollectableAttributeable attr)
            {
                data = new(attr.Count);
            }
            else
            {
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
                case IDictionary<System.String, System.Object> d:
                    data = new(d);
                    break;
                case ICollection<KeyValuePair<System.String, System.Object>> c:
                    data = new(c.Count);
                    CommonInit_1(enumerable);
                    break;
                default:
                    data = new(10);
                    CommonInit_1(enumerable);
                    break;
            }
        }

        /// <summary>
        /// Statically initializes a <see cref="Record"/> with the specified attributes.
        /// </summary>
        /// <param name="attributes">The set of attributes to initialize this record from.</param>
        /// <returns>A new <see cref="Record"/> containing the attributes specified by <paramref name="attributes"/>.</returns>
        public static Record With(params AttributeKeyValuePair[] attributes) => new(attributes ?? Array.Empty<AttributeKeyValuePair>());

        private void CommonInit_1(IEnumerable<KeyValuePair<System.String, System.Object>> e)
        {
            foreach (var i in e)
            {
                data.Add(i.Key, i.Value);
            }
        }

        /// <summary>
        /// Gets the number of fields contained in the current record.
        /// </summary>
        public int Count => data.Count;

        /// <summary>
        /// Gets all the field names defined in the current record.
        /// </summary>
        public IEnumerable<string> Keys => data.Keys;

        /// <summary>
        /// Gets a field from the current record.
        /// </summary>
        /// <param name="name">The field's name to retrieve.</param>
        /// <returns>The field's value provided during construction.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="name"/> was <see langword="null"/>.</exception>
        /// <exception cref="AttributeNotFoundException"><paramref name="name"/> was not found in this record instance.</exception>
        public object GetAttribute(string name)
        {
            ArgumentNullException.ThrowIfNull(name);
            foreach (var attr in data)
            {
                if (attr.Key == name)
                {
                    return attr.Value;
                }
            }
            throw new AttributeNotFoundException(name);
        }

        /// <summary>
        /// Gets an enumerator that can iterate through all the fields in the record.
        /// </summary>
        /// <returns>An enumerator returning <see cref="AttributeKeyValuePair"/> instances, representing each one of the fields existing in this record.</returns>
        public IEnumerator<AttributeKeyValuePair> GetEnumerator() => new RecordEnumerator(data.GetEnumerator());

        /// <summary>
        /// A <see cref="Record"/> instance is immutable and cannot be modfied directly.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <exception cref="NotSupportedException">Attempted to modify the record directly.</exception>
        [DoesNotReturn]
        public void SetAttribute(string name, object value)
            => throw new NotSupportedException("Cannot modify a stable record.");

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}