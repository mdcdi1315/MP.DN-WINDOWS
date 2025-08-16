

using System;

namespace MP.Serialization
{
    /// <summary>
    /// Defines an abstraction for a serializable class reader. <br />
    /// This interface does not control the serialization process; this is done by the <see cref="SerializationManager{T}"/> class.
    /// </summary>
    public interface ISerializedClassReader : IDisposable
    {
        /// <summary>
        /// Reads the field information of the next field existing on the reader.
        /// </summary>
        /// <returns>The information of the serialized field.</returns>
        /// <exception cref="InvalidOperationException">The reader is not ready to read a new field, or no more fields to read do exist.</exception>
        public SerializedFieldInformation GetFieldInformation();

        /// <summary>
        /// Decodes the specified field and loads it as a .NET object. <br />
        /// If the field information denotes another serialized object, then this should return an instance of the <see cref="ISerializedClassReader"/> interface, representing the reader of THAT class.
        /// </summary>
        /// <returns>The decoded field value, or an instance of the <see cref="ISerializedClassReader"/> interface if the <see cref="SerializedFieldInformation.Type"/> property is <see cref="SerializedFieldType.Object"/>.</returns>
        /// <exception cref="InvalidOperationException">The reader is not ready to read a new field, or no more fields to read do exist.</exception>
        public System.Object DecodeField();

        /// <summary>
        /// Moves to a next field in the serialized object.
        /// </summary>
        /// <returns><see langword="true"/> when a new field was found into the serialized object; otherwise, <see langword="false"/>.</returns>
        public System.Boolean MoveNext();

        /// <summary>Initializes the reader from a data stream, reading serialized data from the specified stream.</summary>
        /// <remarks>You will not need to call this method if this object is returned by the <see cref="DecodeField"/> method.</remarks>
        /// <exception cref="ArgumentNullException"><paramref name="stream"/> was <see langword="null"/>.</exception>
        public void Initialize(System.IO.Stream stream);
    }
}