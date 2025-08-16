

using System;
using System.IO;
using System.Collections.Generic;

namespace MP.Serialization
{
    /// <summary>
    /// Provides a utility class for class writer implementations to cope with recursive objects.
    /// </summary>
    public sealed class InMemorySerializedClassWriter : ISerializedClassWriter
    {
        private Dictionary<SerializedFieldInformation, System.Object> dict;

        /// <summary>
        /// Creates an empty instance of the <see cref="InMemorySerializedClassWriter"/> class.
        /// </summary>
        public InMemorySerializedClassWriter() => dict = new();

        /// <summary>
        /// Creates an empty instance of the <see cref="InMemorySerializedClassWriter"/> class, specifying the initial capacity of the internal dictionary.
        /// </summary>
        /// <param name="capacity">Specifies the initial capacity that the internal dictionary can hold.</param>
        public InMemorySerializedClassWriter(int capacity) => dict = new(capacity);

        /// <inheritdoc />
        public void AddField(SerializedFieldInformation sfi, object value)
        {
            if (value is ISerializedClassWriter)
            {
                if (value is InMemorySerializedClassWriter writer)
                {
                    dict.Add(sfi, writer);
                }
                else
                {
                    throw new InvalidOperationException("Not other serialized class writers are supported except other instance of the InMemorySerializedClassWriter class.");
                }
            }
            else
            {
                dict.Add(sfi, value);
            }
        }

        /// <summary>
        /// Disposes the current class writer, without flushing any information.
        /// </summary>
        public void Dispose()
        {
            dict?.Clear();
            dict = null;
        }

        /// <summary>
        /// Effectively cleans up the current class writer, preparing it to process a new operation.
        /// </summary>
        public void FinalizeWriteOp() => dict?.Clear();

        /// <inheritdoc />
        public ISerializedClassWriter GetEmptyWriter() => new InMemorySerializedClassWriter();

        /// <summary>
        /// This call is not supported and will always throw a <see cref="NotSupportedException"/>.
        /// </summary>
        /// <param name="stream">Unused for this kind of class writer.</param>
        /// <exception cref="NotSupportedException">This kind of writer does not require initialization.</exception>
        public void Initialize(Stream stream) => throw new NotSupportedException("Not supported for in-memory serialized class writers");

        /// <summary>
        /// Gets the number of elements contained in the <see cref="Fields" /> property.
        /// </summary>
        public int Count => dict.Count;

        /// <summary>
        /// Gets all the fields defined in the current in-memory class writer.
        /// </summary>
        public IEnumerable<KeyValuePair<SerializedFieldInformation, System.Object>> Fields => dict;
    }
}