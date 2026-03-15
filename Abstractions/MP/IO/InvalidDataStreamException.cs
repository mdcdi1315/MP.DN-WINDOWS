
using System;
using MP.Annotations.CodeAnalysis;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace MP.IO
{
    /// <summary>
    /// Exception class thrown where a class that requires a <see cref="IDataStreamAccess"/> 
    /// object at a parameter of a constructor or method is invalid for it's purpose.
    /// </summary>
    public class InvalidDataStreamException : IOException
    {
        /// <summary>
        /// Creates a default instance of the <see cref="InvalidDataStreamException"/> class.
        /// </summary>
        public InvalidDataStreamException() : base() { }

        /// <summary>
        /// Creates a new instance of the <see cref="InvalidDataStreamException"/> class with the specified detailed error message.
        /// </summary>
        /// <param name="message">The error message to show.</param>
        public InvalidDataStreamException([AllowNull] System.String message) : base(message) { }

        /// <summary>
        /// Creates a new instance of the <see cref="InvalidDataStreamException"/> class, with the specified 
        /// detailed error message and the inner exception that is the cause of this exception.
        /// </summary>
        /// <param name="message">The error message to show.</param>
        /// <param name="innerException">The exception that is the cause of this exception.</param>
        public InvalidDataStreamException([AllowNull] System.String message, [AllowNull] Exception innerException) : base(message, innerException) { }

        /// <summary>
        /// Creates a new instance of the <see cref="InvalidDataStreamException"/> class, with the specified 
        /// detailed error message and the inner I/O exception that is the cause of this exception.
        /// </summary>
        /// <param name="message">The error message to show.</param>
        /// <param name="innerException">The exception that is the cause of this exception.</param>
        public InvalidDataStreamException([AllowNull] System.String message, [AllowNull] IOException innerException) : base(message, innerException) { }

        /// <summary>
        /// Creates a new instance of the <see cref="InvalidDataStreamException"/> class, with the specified 
        /// detailed error message and the inner <see cref="InvalidDataStreamException"/> object that is the cause of this exception.
        /// </summary>
        /// <param name="message">The error message to show.</param>
        /// <param name="innerException">The <see cref="InvalidDataStreamException"/> that is the cause of this exception.</param>
        public InvalidDataStreamException([AllowNull] System.String message, [AllowNull] InvalidDataStreamException innerException) : base(message, innerException) { }

        /// <summary>
        /// Throws an <see cref="InvalidDataStreamException"/> if <paramref name="access"/> is not readable.
        /// </summary>
        /// <param name="access">The data stream access object to test.</param>
        /// <param name="parameter_name">The name of the symbol passed to <paramref name="access"/>. For compiler access only.</param>
        /// <exception cref="InvalidDataStreamException"><paramref name="access"/> is unreadable.</exception>
        [Throws(typeof(InvalidDataStreamException))]
        public static void ThrowIfUnreadable([DisallowNull] IDataStreamAccess access, [CallerArgumentExpression(nameof(access))] System.String parameter_name = null)
        {
            if (access.CanRead == false) {
                String s = "Specified data stream is unreadable.";
                if (parameter_name is not null) { s += String.Concat("\nParameter Name: ", parameter_name); }
                var ids = new InvalidDataStreamException(s);
                ids.Data.Add("ParameterName", parameter_name);
                throw ids;
            }
        }

        /// <summary>
        /// Throws an <see cref="InvalidDataStreamException"/> if <paramref name="access"/> is not writable.
        /// </summary>
        /// <param name="access">The data stream access object to test.</param>
        /// <param name="parameter_name">The name of the symbol passed to <paramref name="access"/>. For compiler access only.</param>
        /// <exception cref="InvalidDataStreamException"><paramref name="access"/> is unwritable.</exception>
        [Throws(typeof(InvalidDataStreamException))]
        public static void ThrowIfUnwritable([DisallowNull] IDataStreamAccess access, [CallerArgumentExpression(nameof(access))] System.String parameter_name = null)
        {
            if (access.CanWrite == false) {
                String s = "Specified data stream is unwritable.";
                if (parameter_name is not null) { s += String.Concat("\nParameter Name: ", parameter_name); }
                var ids = new InvalidDataStreamException(s);
                ids.Data.Add("ParameterName", parameter_name);
                throw ids;
            }
        }
    }
}