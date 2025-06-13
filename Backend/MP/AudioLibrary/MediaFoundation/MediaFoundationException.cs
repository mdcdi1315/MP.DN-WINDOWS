
using System;
using MP.ExceptionSystem;

namespace MP.AudioLibrary.MediaFoundation
{
    /// <summary>
    /// All the errors that are occuring from COM calls and cannot be handled, they should be 
    /// created as exceptions deriving from this central exception. <br />
    /// ONLY MEDIA FOUNDATION ERRORS MUST BE REPRESENTED WITH THIS CLASS!
    /// </summary>
    public abstract class MediaFoundationException : BaseException
    {
        /// <summary>
        /// Creates a default instance of the <see cref="MediaFoundationException"/> class.
        /// </summary>
        public MediaFoundationException() : base() { }

        /// <summary>
        /// Creates a new instance of the <see cref="MediaFoundationException"/> class and with the specified 
        /// detailed error message.
        /// </summary>
        /// <param name="message">The error message to show.</param>
        public MediaFoundationException(System.String message) : base(message) { }

        /// <summary>
        /// Creates a new instance of the <see cref="MediaFoundationException"/> class, with the specified 
        /// detailed error message and the inner exception that is the cause of this exception.
        /// </summary>
        /// <param name="message">The error message to show.</param>
        /// <param name="innerException">The exception that is the cause of this exception.</param>
        public MediaFoundationException(System.String message, Exception innerException) : base(message, innerException) { }
    }
}