

namespace MP.ExtensibilitySystem
{
    /// <summary>
    /// Defines a single asset in a extension package.
    /// </summary>
    public interface IAsset
    {
        /// <summary>Gets the file name of the current asset.</summary>
        public System.String Name { get; }

        /// <summary>Gets the assets' verification hash.</summary>
        public System.String VerificationHash { get; }

        /// <summary>Gets the algorithm that was used to hash this asset and it is the one used when the extension engine will verify the data.</summary>
        public AssetVerificationHashType HashType { get; }
    }
}