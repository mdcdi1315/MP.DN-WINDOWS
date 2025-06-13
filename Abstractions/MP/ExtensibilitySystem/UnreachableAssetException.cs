

namespace MP.ExtensibilitySystem
{
    /// <summary>
    /// Must be thrown when a package cannot locate a specific asset inside it's package or it had a discrepancy and thus it could not access the stream.
    /// </summary>
    public sealed class UnreachableAssetException : ExtensibilitySystemException
    {
        /// <summary>
        /// Creates a new instance of the <see cref="UnreachableAssetException"/> class and with the specified 
        /// detailed error message.
        /// </summary>
        /// <param name="message">The error message to show.</param>
        public UnreachableAssetException(System.String message) : base(message) { }

        /// <summary>
        /// Creates a new instance of the <see cref="UnreachableAssetException"/> class, with the specified 
        /// detailed error message and the inner exception that is the cause of this exception.
        /// </summary>
        /// <param name="message">The error message to show.</param>
        /// <param name="innerException">The exception that is the cause of this exception.</param>
        public UnreachableAssetException(System.String message, System.Exception innerException) : base(message, innerException) { }
    }
}