

namespace MP.ExtensibilitySystem
{
    /// <summary>
    /// Thrown when the engine did not find the expected criteria in this managed asset.
    /// </summary>
    public sealed class InvalidManagedAssetLayoutException : ExtensibilitySystemException
    {
        /// <summary>
        /// Creates a new instance of the <see cref="InvalidManagedAssetLayoutException"/> class with the specified asset that was failed.
        /// </summary>
        /// <param name="assetname">The name of the managed asset that was invalid.</param>
        /// <param name="detailmessage">Additional details about the failure.</param>
        public InvalidManagedAssetLayoutException(System.String assetname , System.String detailmessage)
            : base($"The managed asset named as {assetname} was invalid and was rejected.\nDetails: {detailmessage}") { }
    }
}