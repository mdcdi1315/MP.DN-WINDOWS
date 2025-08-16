using System;

namespace MP.ExceptionSystem
{
    /// <summary>
    /// Defines all the custom-defined MP app and libraries exceptions: <br />
    /// From native errors to file format errors and networking issues. <br />
    /// Generally, any exception for code defined inside the Music Player should be routed through this exception.
    /// </summary>
    public class BaseException : ApplicationException, IMusicPlayerException
    {
        /// <summary>
        /// Creates a default instance of the <see cref="BaseException"/> class.
        /// </summary>
        public BaseException() : base() { }

        /// <summary>
        /// Creates a new instance of the <see cref="BaseException"/> class and with the specified 
        /// detailed error message.
        /// </summary>
        /// <param name="message">The error message to show.</param>
        public BaseException(System.String message) : base(message) { }

        /// <summary>
        /// Creates a new instance of the <see cref="BaseException"/> class, with the specified 
        /// detailed error message and the inner exception that is the cause of this exception.
        /// </summary>
        /// <param name="message">The error message to show.</param>
        /// <param name="innerException">The exception that is the cause of this exception.</param>
        public BaseException(System.String message, Exception innerException) : base(message, innerException) { }
        
        /// <summary>
        /// Creates a new instance of the <see cref="BaseException"/> class, with the specified 
        /// detailed error message and the inner exception that is the cause of this exception.
        /// </summary>
        /// <param name="message">The error message to show.</param>
        /// <param name="innerException">The exception that is the cause of this exception.</param>
        public BaseException(System.String message, BaseException innerException) : base(message , innerException) { }
    }
}
