

namespace MP.ExtensibilitySystem
{
    /// <summary>
    /// Thrown by the <see cref="ExtensionEngine"/> class when an asset in a package has failed the integrity tests.
    /// </summary>
    public sealed class AssetIntegrityFailedException : ExtensibilitySystemException
    {
        /// <summary>
        /// Creates a new instance of the <see cref="AssetIntegrityFailedException"/> class with the specified asset name and it's belonging package name that was failed.
        /// </summary>
        /// <param name="assetname">The asset whose integrity failed.</param>
        /// <param name="pkgname">The name of the package where this asset is contained.</param>
        public AssetIntegrityFailedException(System.String assetname, System.String pkgname)
            : base($"The asset with name {assetname} of extension package {pkgname} was failed to be verified due to an integrity error.") { }
    }
}