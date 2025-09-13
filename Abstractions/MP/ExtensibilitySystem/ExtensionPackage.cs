
using System;
using System.Collections.Generic;

namespace MP.ExtensibilitySystem
{
    /// <summary>
    /// Defines a single location from where extension assets are loaded by the extensibility engine. <br />
    /// This class must be overriden.
    /// </summary>
    public abstract class ExtensionPackage : IDisposable
    {
        /// <summary>Creates a new extension package instance.</summary>
        protected ExtensionPackage() {}

        /// <summary>
        /// Defines the assets to be loaded by the extensibility engine.
        /// </summary>
        public abstract IList<IAsset> Assets { get; }

        /// <summary>Gets a unique identifier of this package.</summary>
        public abstract System.String Name { get; }

        /// <summary>Gets a small textual description of the package.</summary>
        public abstract System.String Description { get; }

        /// <summary>Gets the version of this package.</summary>
        public abstract Version Version { get; }

        /// <summary>
        /// The actual implementation of <see cref="GetAssetStream(IAsset)"/>. 
        /// Through this method the caller must recieve the data stream of the current asset.
        /// </summary>
        /// <param name="asset">The asset to retrieve it's data.</param>
        /// <returns>The data stream of the <paramref name="asset"/>.</returns>
        protected abstract System.IO.Stream GetAssetStreamImpl(IAsset asset);

        /// <summary>
        /// Gets the asset's data returned through a <see cref="System.IO.Stream"/> object.
        /// </summary>
        /// <param name="asset">The asset to retrieve it's data.</param>
        /// <returns>A new <see cref="System.IO.Stream"/> reprsenting the passed asset data from <paramref name="asset"/> parameter.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="asset"/> was <see langword="null"/>.</exception>
        /// <exception cref="InvalidAssetDefinitionException">The asset provided is not part of the current package.</exception>
        public System.IO.Stream GetAssetStream(IAsset asset)
        {
            ArgumentNullException.ThrowIfNull(asset);
            foreach (var item in Assets) 
            {
                if (asset.Name == item.Name)
                {
                    return GetAssetStreamImpl(asset);
                }
            }
            throw new InvalidAssetDefinitionException(asset.Name);
        }

        /// <summary>
        /// Common placeholder for disposing resources.
        /// </summary>
        /// <param name="disposing">Has the value <see langword="true"/> when this method is called from <see cref="Dispose()"/>, otherwise <see langword="false"/> and it was called by the finalizer.</param>
        protected virtual void Dispose(System.Boolean disposing) { }

        /// <summary>
        /// Frees the resources that the <see cref="ExtensionPackage"/> instance was used.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>Default finalizer.</summary>
        ~ExtensionPackage() => Dispose(false);
    }
}