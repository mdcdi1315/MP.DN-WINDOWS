namespace MP.AudioLibrary
{
    /// <summary>
    /// Defines the class that is thrown when a conversion was requested, but there was not found a converter for the provided type.
    /// </summary>
    public sealed class ConverterNotFoundException : AudioLibraryException
    {
        /// <summary>
        /// Creates a new instance of the <see cref="ConverterNotFoundException"/> class with the specified error message.
        /// </summary>
        /// <param name="message">The error message to specify.</param>
        public ConverterNotFoundException(string message) : base(message) { }
    }
}