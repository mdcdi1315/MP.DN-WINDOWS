


namespace MP.AudioLibrary
{
    /// <summary>
    /// The exception that is thrown when attempting to register a converter that is already provided for the given type.
    /// </summary>
    public sealed class ConverterAlreadyRegisteredException : AudioLibraryException
    {
        private readonly System.Type type;

        /// <summary>
        /// Initializes a new instance of the <see cref="ConverterAlreadyRegisteredException"/> class, providing the audio format type that it was attempted to be double-registered.
        /// </summary>
        /// <param name="type">The type that was attempted to be double-registered.</param>
        public ConverterAlreadyRegisteredException(System.Type type) => this.type = type;

        /// <summary>
        /// Gets the audio format type that would be registered if an converter with the same type was not registered before.
        /// </summary>
        public System.Type Type => type;

        /// <summary>
        /// Gets the message to provide in the exception description.
        /// </summary>
        public override System.String Message => $"Attempted to double-register an converter for the audio format type {type.FullName}.\nFully qualified name: {type.AssemblyQualifiedName}";
    }
}