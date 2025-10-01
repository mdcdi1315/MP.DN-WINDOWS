
using MP.Serialization;

namespace MP.ExtensibilitySystem.Serialization
{
    /// <summary>
    /// Defines a serializable class specifying a way to serialize <see cref="ExtensibilitySystem.ManagedAsset"/> instances.
    /// </summary>
    public class ManagedAsset : Asset
    {
        /// <summary>
        /// Defines the .NET type which is the entry point for the managed asset.
        /// </summary>
        [FieldName("entry_point")]
        public System.String EntryPointTypeName;
    }
}