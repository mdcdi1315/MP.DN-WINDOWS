
using System;
using MP.Collections;
using System.Collections;
using System.Collections.Generic;
using MP.Annotations.CodeAnalysis;
using System.Diagnostics.CodeAnalysis;

namespace MP.Serialization
{
    /// <summary>
    /// Defines the median interface for de/serializing a class. <br />
    /// It provides information about the fields of the current class instance to de/encode.
    /// </summary>
    public sealed class Record : IEnumerable<SerializedField>
    {
        /// <summary>
        /// Defines a class that can efficiently build <see cref="Record"/> instances.
        /// </summary>
        public sealed class Builder : IObjectBuilder<Record>
        {
            private readonly ArrayBuilder<SerializedField> builder_internal;

            /// <summary>
            /// Creates a new <see cref="Builder"/> instance.
            /// </summary>
            public Builder() => builder_internal = new();

            /// <summary>Adds a new field to this builder instance.</summary>
            /// <param name="field">The serialized field to be added.</param>
            /// <exception cref="ArgumentNullException"><paramref name="field"/> was <see langword="null"/>.</exception>
            public void Add(SerializedField field)
            {
                ArgumentNullException.ThrowIfNull(field);
                builder_internal.Add(field);
            }

            /// <summary>
            /// Builds a new <see cref="Record"/> instance from the currently defined serialized fields.
            /// </summary>
            /// <returns>The built <see cref="Record"/> instance.</returns>
            public Record Build() => new(builder_internal.Build());
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
                collection.CopyTo(this.fields, 0);
            } else { // Default fallback
                List<SerializedField> list = new(20);
                foreach (SerializedField field in fields) { list.Add(field); }
                this.fields = list.ToArray();
            }
        }

        /// <summary>
        /// Gets the number of fields contained in the current record.
        /// </summary>
        public int Count => fields.Length;

        /// <summary>
        /// Based on the specified <paramref name="name"/>, a field of that name is looked up on the current record 
        /// and returns the <see cref="SerializedField"/> information that is associated with that field.
        /// </summary>
        /// <param name="name">The name of the serialized field to look up.</param>
        /// <returns>The <see cref="SerializedField"/> instance that has a name matching the contents of the <paramref name="name"/> parameter.</returns>
        [return: MaybeNull]
        [Throws(typeof(ArgumentNullException))]
        public SerializedField LookupField([NotNull] string name)
        {
            ArgumentNullException.ThrowIfNull(name);
            foreach (SerializedField field in fields) {
                if (field.Name.Equals(name, StringComparison.Ordinal)) { return field; }
            }
            return null;
        }

        /// <summary>
        /// Gets an enumerator able to enumerate through all the fields of this record.
        /// </summary>
        /// <returns>A new enumerator instance.</returns>
        public IEnumerator<SerializedField> GetEnumerator() => ArrayEnumerator<SerializedField>.Of(fields);

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}