using System;
using System.Diagnostics.CodeAnalysis;

namespace MP.IO
{
    /// <summary>
    /// The invalid data stream format exception class is a specialized case of the <see cref="InvalidDataStreamException"/>,
    /// that indicates that the class expected a different stream format than that that was actually was.
    /// </summary>
    public class InvalidDataStreamFormatException : InvalidDataStreamException
    {
        /// <summary>
        /// Creates a default instance of the <see cref="InvalidDataStreamFormatException"/> class.
        /// </summary>
        public InvalidDataStreamFormatException() : base() { }

        /// <summary>
        /// Creates a new instance of the <see cref="InvalidDataStreamFormatException"/> class with the specified detailed error message.
        /// </summary>
        /// <param name="message">The error message to show.</param>
        public InvalidDataStreamFormatException([AllowNull] System.String message) : base(message) { }

        /// <summary>
        /// Creates a new instance of the <see cref="InvalidDataStreamFormatException"/> class, with the specified 
        /// detailed error message and the inner exception that is the cause of this exception.
        /// </summary>
        /// <param name="message">The error message to show.</param>
        /// <param name="innerException">The exception that is the cause of this exception.</param>
        public InvalidDataStreamFormatException([AllowNull] System.String message, [AllowNull] Exception innerException) : base(message, innerException) { }

        /// <summary>
        /// Creates a new instance of the <see cref="InvalidDataStreamFormatException"/> class, with the specified 
        /// detailed error message and the inner I/O exception that is the cause of this exception.
        /// </summary>
        /// <param name="message">The error message to show.</param>
        /// <param name="innerException">The exception that is the cause of this exception.</param>
        public InvalidDataStreamFormatException([AllowNull] System.String message, [AllowNull] IOException innerException) : base(message, innerException) { }

        /// <summary>
        /// Creates a new instance of the <see cref="InvalidDataStreamFormatException"/> class, with the specified 
        /// detailed error message and the inner <see cref="InvalidDataStreamException"/> object that is the cause of this exception.
        /// </summary>
        /// <param name="message">The error message to show.</param>
        /// <param name="innerException">The <see cref="InvalidDataStreamException"/> that is the cause of this exception.</param>
        public InvalidDataStreamFormatException([AllowNull] System.String message, [AllowNull] InvalidDataStreamException innerException) : base(message, innerException) { }

        /// <summary>
        /// Creates a new instance of the <see cref="InvalidDataStreamFormatException"/> class, with the specified 
        /// detailed error message and the inner <see cref="InvalidDataStreamFormatException"/> object that is the cause of this exception.
        /// </summary>
        /// <param name="message">The error message to show.</param>
        /// <param name="innerException">The <see cref="InvalidDataStreamException"/> that is the cause of this exception.</param>
        public InvalidDataStreamFormatException([AllowNull] System.String message, [AllowNull] InvalidDataStreamFormatException innerException) : base(message, innerException) { }
    }
}