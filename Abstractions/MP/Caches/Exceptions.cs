

namespace MP.Caches
{
    /// <summary>
    /// Defines the base exception class for errors that are coming from cache readers and writers that implement the 
    /// Music Player Cache logic.
    /// </summary>
    public class CachesFormatBaseException : ExceptionSystem.BaseException
    {
        /// <summary>
        /// Creates an empty instance of the <see cref="CacheFormatInvalidException"/> class.
        /// </summary>
        public CachesFormatBaseException() : base() { }

        /// <summary>
        /// Creates an instance of the <see cref="CacheFormatInvalidException"/> class with the specified message that describes the current exception.
        /// </summary>
        /// <param name="message">The message that describes this instance of the <see cref="CachesFormatBaseException"/> class.</param>
        public CachesFormatBaseException(System.String message) : base(message) { }

        /// <summary>
        /// Creates an instance of the <see cref="CacheFormatInvalidException"/> class with the specified message that describes the current exception,
        /// and the exception that is the cause of this exception.
        /// </summary>
        /// <param name="message">The message that describes this instance of the <see cref="CachesFormatBaseException"/> class.</param>
        /// <param name="innerException">The exception which is the cause that this exception has occured.</param>
        public CachesFormatBaseException(System.String message, System.Exception innerException) : base(message, innerException) { }
    }

    /// <summary>
    /// Thrown when the header of the cache format is invalid.
    /// </summary>
    public sealed class CacheFormatInvalidException : CachesFormatBaseException
    {
        /// <summary>
        /// Constructs a new instance of the <see cref="CacheFormatInvalidException"/> , specifying the message that best describes the error.
        /// </summary>
        /// <param name="message">A message that describes the error occured while reading the cache format.</param>
        public CacheFormatInvalidException(System.String message) : base(message) { }
    }

    /// <summary>
    /// Thrown when a cache instance is not properly initialized with the properly derived readers and writers.
    /// </summary>
    public sealed class InvalidCacheFrontendTypeException : ExceptionSystem.BaseException
    {
        /// <summary>
        /// Constructs a new instance of the <see cref="InvalidCacheFrontendTypeException"/> , specifying the message that best describes the error.
        /// </summary>
        /// <param name="message">A message that describes the error occured while reading the cache format.</param>
        public InvalidCacheFrontendTypeException(System.String message) : base(message) { }
    }
}