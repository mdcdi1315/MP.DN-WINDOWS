namespace MP.ExceptionSystem
{
    /// <summary>
    /// Thrown when the CGI format while reading settings was found invalid.
    /// </summary>
    public sealed class CGIInvalidFormatException : BaseException
    {
        /// <summary>
        /// Creates a new instance of the <see cref="CGIInvalidFormatException"/> class with the specified message.
        /// </summary>
        /// <param name="message">The message that explains the reason that this exception was thrown.</param>
        public CGIInvalidFormatException(System.String message) : base(message) { }

        /// <inheritdoc />
        protected override string DefaultSourceValue => "MP Abstractions Library: CGI Settings subsystem";
    }
}
