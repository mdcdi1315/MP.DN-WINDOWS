

namespace MP.ExtensibilitySystem
{
    /// <summary>
    /// Thrown generically when a passed asset definition is invalid for the specified extnensibilty package.
    /// </summary>
    public sealed class InvalidAssetDefinitionException : ExtensibilitySystemException
    {
        /// <summary>
        /// Creates a new <see cref="InvalidAssetDefinitionException"/> class with the specified asset name that was invalid for the package.
        /// </summary>
        /// <param name="name">The asset name causing this exception class to be created</param>
        public InvalidAssetDefinitionException(System.String name) : base($"The provided asset named as {name} is invalid for this extensibility package.") { }
    }
}