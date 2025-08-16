
using System;

namespace MP.Serialization
{
    /// <summary>
    /// Defines an abstraction for a serializable class writer. <br />
    /// This interface does not control the serialization process; this is done by the <see cref="SerializationManager{T}"/> class.
    /// </summary>
    public interface ISerializedClassWriter : IDisposable
    {
        /// <summary>
        /// Adds a field with the specified field information and value. <br />
        /// Note: You must provide an instance of the <see cref="ISerializedClassWriter"/> instance if you want to serialize an object as a value!!
        /// </summary>
        /// <param name="sfi">The field information that describes the field to add.</param>
        /// <param name="value">The value that the new field will have.</param>
        public void AddField(SerializedFieldInformation sfi, System.Object value);

        /// <summary>Initializes the writer from a data stream, writing serialized data to the specified stream.</summary>
        /// <exception cref="ArgumentNullException"><paramref name="stream"/> was <see langword="null"/>.</exception>
        public void Initialize(System.IO.Stream stream);

        /// <summary>
        /// Finalizes the current write operation, performing any additional writing operations required and then destroying the current writer instance.
        /// </summary>
        public void FinalizeWriteOp();

        /// <summary>
        /// Gets an another instance of the <see cref="ISerializedClassWriter"/> instance to cope with serializable classes that may occur during serialization. <br />
        /// This instance is then fed to <see cref="AddField"/> method to start analyzing the created graph.
        /// </summary>
        /// <returns>An empty and ready instance of the <see cref="ISerializedClassWriter"/> instance.</returns>
        public ISerializedClassWriter GetEmptyWriter();
    }
}