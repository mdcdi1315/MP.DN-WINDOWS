


using MP.Serialization;

namespace MP.ExtensibilitySystem.Serialization
{
    /// <summary>
    /// Defines a serializable class specifying a way to serialize <see cref="ExtensibilitySystem.NativeAsset"/> instances.
    /// </summary>
    public class NativeAsset : Asset
    {
        /// <summary>
        /// Gets the platform that this native asset can run to.
        /// </summary>
        [FieldName("platform")]
        public Platform Platform;

        /// <summary>
        /// Gets the processor architecture that this native asset contains valid code for.
        /// </summary>
        [FieldName("processor_arch")]
        public ProcessorArchitecture ProcessorArchitecture;
    }
}