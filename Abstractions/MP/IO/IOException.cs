
using System;
using MP.ExceptionSystem;
using System.Diagnostics.CodeAnalysis;

namespace MP.IO
{
    /// <summary>
    /// Defines the exception that is thrown on any I/O error that occurs.
    /// </summary>
    public class IOException : BaseException
    {
        /// <summary>
        /// Creates a default instance of the <see cref="IOException"/> class.
        /// </summary>
        public IOException() : base() { }

        /// <summary>
        /// Creates a new instance of the <see cref="IOException"/> class with the specified detailed error message.
        /// </summary>
        /// <param name="message">The error message to show.</param>
        public IOException([AllowNull] System.String message) : base(message) { }

        /// <summary>
        /// Creates a new instance of the <see cref="IOException"/> class, with the specified 
        /// detailed error message and the inner exception that is the cause of this exception.
        /// </summary>
        /// <param name="message">The error message to show.</param>
        /// <param name="innerException">The exception that is the cause of this exception.</param>
        public IOException([AllowNull] System.String message, [AllowNull] Exception innerException) : base(message, innerException) { }

        /// <summary>
        /// Creates a new instance of the <see cref="IOException"/> class, with the specified 
        /// detailed error message and the inner I/O exception that is the cause of this exception.
        /// </summary>
        /// <param name="message">The error message to show.</param>
        /// <param name="innerException">The exception that is the cause of this exception.</param>
        public IOException([AllowNull] System.String message, [AllowNull] IOException innerException) : base(message, innerException) { }

        /// <inheritdoc />
        protected override string DefaultSourceValue => "MP Abstractions Library: I/O subsystem";
    }
}