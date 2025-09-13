
using System;
using MP.ExceptionSystem;

namespace MP.Graphics.OpenGL
{
    /// <summary>
    /// Defines the base exception class for all OpenGL-based operations.
    /// </summary>
    public class OpenGLException : BaseException , INativeException
    {
        /// <summary>
        /// Creates a default instance of the <see cref="OpenGLException"/> class.
        /// </summary>
        public OpenGLException() : base() { }

        /// <summary>
        /// Creates a new instance of the <see cref="OpenGLException"/> class and with the specified 
        /// detailed error message.
        /// </summary>
        /// <param name="message">The error message to show.</param>
        public OpenGLException(System.String message) : base(message) { }

        /// <summary>
        /// Creates a new instance of the <see cref="OpenGLException"/> class, with the specified 
        /// detailed error message and the inner exception that is the cause of this exception.
        /// </summary>
        /// <param name="message">The error message to show.</param>
        /// <param name="innerException">The exception that is the cause of this exception.</param>
        public OpenGLException(System.String message, Exception innerException) : base(message, innerException) { }
    }
}