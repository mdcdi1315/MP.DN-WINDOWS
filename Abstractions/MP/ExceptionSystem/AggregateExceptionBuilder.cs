
using System;
using MP.Annotations.CodeAnalysis;
using System.Diagnostics.CodeAnalysis;

namespace MP.ExceptionSystem
{
    /// <summary>
    /// Builds an <see cref="AggregateException"/> by providing exceptions to register to it, as well as the message to provide.
    /// </summary>
    public sealed class AggregateExceptionBuilder : ExceptionBuilder
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AggregateExceptionBuilder"/> class.
        /// </summary>
        public AggregateExceptionBuilder() : base() { }

        /// <inheritdoc />
        [DoesNotReturn]
        [Throws(typeof(AggregateException))]
        protected override void ThrowException([AllowNull] string message, [DisallowNull] Exception[] exceptions) => throw new AggregateException(message, exceptions);
    }
}