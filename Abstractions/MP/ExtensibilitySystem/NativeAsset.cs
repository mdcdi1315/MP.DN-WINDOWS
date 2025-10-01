


namespace MP.ExtensibilitySystem
{
    /// <summary>
    /// Defines a native asset in an extension package. <br />
    /// This class provides information about the asset itself, as well as on which platforms and architectures this asset can run.
    /// </summary>
    public abstract class NativeAsset : IAsset
    {
        // Directly inherited by IAsset - all these are becoming abstract
        
        /// <inheritdoc />
        public abstract string Name { get; }
        
        /// <inheritdoc />
        public abstract string VerificationHash { get; }
        
        /// <inheritdoc />
        public abstract AssetVerificationHashType HashType { get; }

        /// <summary>
        /// Gets the platform that this native asset can run to.
        /// </summary>
        public abstract Platform Platform { get; }

        /// <summary>
        /// Gets the processor architecture that this native asset contains valid code for.
        /// </summary>
        public abstract ProcessorArchitecture ProcessorArchitecture { get; }
    }
}