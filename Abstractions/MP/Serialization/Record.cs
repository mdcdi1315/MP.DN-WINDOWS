
using System;
using System.Collections;
using System.Collections.Generic;

namespace MP.Serialization
{
    /// <summary>
    /// Defines the median interface for de/serializing a class. <br />
    /// It provides information about the fields of the current class instance to de/encode.
    /// </summary>
    public sealed class Record : IEnumerable<SerializedField>
    {
        private struct RecordEnumerator : IEnumerator<SerializedField>
        {
            private long index;
            private SerializedField current;
            private SerializedField[] fields;

            public RecordEnumerator(SerializedField[] fields)
            {
                this.fields = fields;
                current = null;
                index = -1;
            }

            public SerializedField Current => current;

            object IEnumerator.Current => Current;

            public void Dispose() {
                index = 0;
                current = null;
                fields = null;
            }

            public bool MoveNext()
            {
                if (++index < fields.LongLength) {
                    current = fields[index];
                    return true;
                } else {
                    return false;
                }
            }

            public void Reset()
            {
                index = -1;
                current = null;
            }
        }

        /// <summary>
        /// Defines a class that can efficiently build <see cref="Record"/> instances.
        /// </summary>
        public sealed class Builder
        {
            private List<SerializedField> list;

            /// <summary>
            /// Creates a new <see cref="Builder"/> instance.
            /// </summary>
            public Builder() => list = new(20);

            /// <summary>
            /// Creates a new <see cref="Builder"/> instance, additionally providing an integer that will be the initial capacity of the internal array for faster field addition.
            /// </summary>
            /// <param name="capacity">The initially desired capacity of the internal array so that add operations can be faster.</param>
            /// <exception cref="ArgumentOutOfRangeException"><paramref name="capacity"/> was negative.</exception>
            public Builder(int capacity) => list = new(capacity);

            /// <summary>Adds a new field to this builder instance.</summary>
            /// <param name="field">The serialized field to be added.</param>
            /// <exception cref="ArgumentNullException"><paramref name="field"/> was <see langword="null"/>.</exception>
            public void Add(SerializedField field)
            {
                ArgumentNullException.ThrowIfNull(field);
                list.Add(field);
            }

            /// <summary>
            /// Builds a new <see cref="Record"/> instance from the currently defined serialized fields.
            /// </summary>
            /// <returns>The built <see cref="Record"/> instance.</returns>
            public Record Build() => new(list);
        }

        private readonly SerializedField[] fields;

        /// <summary>
        /// Initializes a new <see cref="Record"/> instance from the specified fields.
        /// </summary>
        /// <param name="fields">The fields to initialize this <see cref="Record"/> from.</param>
        public Record(IEnumerable<SerializedField> fields)
        {
            ArgumentNullException.ThrowIfNull(fields);
            if (fields is SerializedField[] fd) { // Direct assignment. It is our lucky day!
                this.fields = fd;
            } else if (fields is ICollection<SerializedField> collection) { // Collection fallback
                this.fields = new SerializedField[collection.Count];
                int I = 0;
                foreach (SerializedField field in collection) {
                    this.fields[I++] = field;
                }
            } else { // Default fallback
                List<SerializedField> list = new(20);
                foreach (SerializedField field in fields) { 
                    list.Add(field);
                }
                this.fields = list.ToArray();
            }
        }

        /// <summary>
        /// Gets the number of fields contained in the current record.
        /// </summary>
        public int Count => fields.Length;

        /// <summary>
        /// Gets an enumerator able to enumerate through all the fields of this record.
        /// </summary>
        /// <returns>A new enumerator instance.</returns>
        public IEnumerator<SerializedField> GetEnumerator() => new RecordEnumerator(fields);

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}