
namespace MP.Serialization
{
    /// <summary>
    /// Thrown when an invalid state (bug) in an <see cref="ISerializedClassReader"/> implementation is found.
    /// </summary>
    public sealed class InvalidSerializationReaderLayoutException : SerializationException
    {
        /// <summary>
        /// Creates a default instance of the <see cref="InvalidSerializationReaderLayoutException"/> class.
        /// </summary>
        public InvalidSerializationReaderLayoutException() : base() { }

        /// <summary>
        /// Creates a new instance of the <see cref="InvalidSerializationReaderLayoutException"/> class and with the specified 
        /// detailed error message.
        /// </summary>
        /// <param name="message">The error message to show.</param>
        public InvalidSerializationReaderLayoutException(System.String message) : base(message) { }

        /// <summary>
        /// Creates a new instance of the <see cref="InvalidSerializationReaderLayoutException"/> class, with the specified 
        /// detailed error message and the inner exception that is the cause of this exception.
        /// </summary>
        /// <param name="message">The error message to show.</param>
        /// <param name="innerException">The exception that is the cause of this exception.</param>
        public InvalidSerializationReaderLayoutException(System.String message, System.Exception innerException) : base(message, innerException) { }

    }
}