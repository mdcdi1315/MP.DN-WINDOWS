


using MP.Serialization;

namespace MP.ExtensibilitySystem.Serialization
{
    /// <summary>
    /// Defines a serializable class specifying a way to serialize <see cref="IAsset"/> instances.
    /// </summary>
    public class Asset : ISerializableClass
    {
        /// <summary>Defines the asset's name.</summary>
        [FieldName("name")]
        public System.String Name;

        /// <summary>Defines the asset's verification hash.</summary>
        [FieldName("hash")]
        public System.String VerificationHash;

        /// <summary>Defines the asset's verification hash algorithm that was used.</summary>
        [FieldName("hash_type")]
        public AssetVerificationHashType VerificationHashType;
    }
}