

namespace MP.ExtensibilitySystem
{
    /// <summary>
    /// Defines the different verification hash algorithms that should be supported by an extension engine.
    /// </summary>
    public enum AssetVerificationHashType : System.Byte
    {
        /// <summary>The hash data are generated via the SHA-256 algorithm</summary>
        SHA256,
        /// <summary>The hash data are generated via the SHA-512 algorithm</summary>
        SHA512,
        /// <summary>The hash data are generated via the MD-5 algorithm</summary>
        MD5
    }
}