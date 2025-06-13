
using System;
using MP.ExceptionSystem;

namespace MP.ExtensibilitySystem
{
    /// <summary>
    /// A base exception that all extensibility system component exceptions must derive from. <br />
    /// Additionally this identifies that a thrown exception that extends this class identifies it to originate from the extensibility system.
    /// </summary>
    public class ExtensibilitySystemException : BaseException
    {
        /// <summary>
        /// Creates a default instance of the <see cref="ExtensibilitySystemException"/> class.
        /// </summary>
        public ExtensibilitySystemException() : base() { }

        /// <summary>
        /// Creates a new instance of the <see cref="ExtensibilitySystemException"/> class and with the specified 
        /// detailed error message.
        /// </summary>
        /// <param name="message">The error message to show.</param>
        public ExtensibilitySystemException(System.String message) : base(message) { }

        /// <summary>
        /// Creates a new instance of the <see cref="ExtensibilitySystemException"/> class, with the specified 
        /// detailed error message and the inner exception that is the cause of this exception.
        /// </summary>
        /// <param name="message">The error message to show.</param>
        /// <param name="innerException">The exception that is the cause of this exception.</param>
        public ExtensibilitySystemException(System.String message, Exception innerException) : base(message, innerException) { }
    }
}