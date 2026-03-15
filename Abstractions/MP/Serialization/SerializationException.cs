
using MP.ExceptionSystem;

namespace MP.Serialization
{
    /// <summary>
    /// Defines the base exception class for all serialization-related exceptions.
    /// </summary>
    public class SerializationException : BaseException
    {
        /// <summary>
        /// Creates a default instance of the <see cref="SerializationException"/> class.
        /// </summary>
        public SerializationException() : base() { }

        /// <summary>
        /// Creates a new instance of the <see cref="SerializationException"/> class and with the specified 
        /// detailed error message.
        /// </summary>
        /// <param name="message">The error message to show.</param>
        public SerializationException(System.String message) : base(message) { }

        /// <summary>
        /// Creates a new instance of the <see cref="SerializationException"/> class, with the specified 
        /// detailed error message and the inner exception that is the cause of this exception.
        /// </summary>
        /// <param name="message">The error message to show.</param>
        /// <param name="innerException">The exception that is the cause of this exception.</param>
        public SerializationException(System.String message, System.Exception innerException) : base(message, innerException) { }
        
        /// <summary>
        /// Creates a new instance of the <see cref="SerializationException"/> class, with the specified 
        /// detailed error message and the inner exception that is the cause of this exception.
        /// </summary>
        /// <param name="message">The error message to show.</param>
        /// <param name="innerException">The exception that is the cause of this exception.</param>
        public SerializationException(System.String message, BaseException innerException) : base(message , innerException) { }
    }
}