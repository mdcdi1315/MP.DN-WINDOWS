

namespace MP.ExtensibilitySystem
{
    /// <summary>
    /// Defines an abstraction of a DLL that can be run into .NET . <br />
    /// The class defines the bare necessities in order this asset can be loaded in the runtime as required.
    /// </summary>
    public abstract class ManagedAsset : IAsset
    {
        // Directly inherited by IAsset - all these are becoming abstract

        /// <inheritdoc />
        public abstract System.String Name { get; }

        /// <inheritdoc />
        public abstract System.String VerificationHash { get; }

        /// <inheritdoc />
        public abstract AssetVerificationHashType HashType { get; }

        /// <summary>
        /// Defines the fully namespaced qualified name of the type that is decorated with the <see cref="ManagedExtensibilityModuleLoaderClassAttribute"/>. <br />
        /// No assembly information are required; just the namespaced type is enough, since the assembly is known and injected into run-time.
        /// </summary>
        public abstract System.String EntryPointTypeName { get; }
    }
}
