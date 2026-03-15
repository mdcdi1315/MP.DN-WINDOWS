

using MP.ExceptionSystem;

namespace MP.AudioLibrary
{
    /// <summary>
    /// Defines the base exception type for all the exception types originating from the Audio Library.
    /// </summary>
    public class AudioLibraryException : BaseException
    {
        /// <summary>
        /// Creates a default instance of the <see cref="AudioLibraryException"/> class.
        /// </summary>
        public AudioLibraryException() : base() { }

        /// <summary>
        /// Creates a new instance of the <see cref="AudioLibraryException"/> class with the specified detailed error message.
        /// </summary>
        /// <param name="message">The error message to append to this exception object.</param>
        public AudioLibraryException(System.String message) : base(message) { }

        /// <summary>
        /// Creates a new instance of the <see cref="AudioLibraryException"/> class, with the specified 
        /// detailed error message and the inner exception that is the cause of this exception.
        /// </summary>
        /// <param name="message">The error message to show.</param>
        /// <param name="innerException">The exception that is the cause of this exception object to be created.</param>
        public AudioLibraryException(System.String message, System.Exception innerException) : base(message, innerException) { }

        /// <inheritdoc />
        protected override string DefaultSourceValue => "MP Abstractions Library: Audio Library subsystem";
    }
}