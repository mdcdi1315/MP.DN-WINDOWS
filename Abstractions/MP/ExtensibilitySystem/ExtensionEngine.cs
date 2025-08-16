
using System;
using MP.Utilities;
using MP.ExceptionSystem;
using System.Collections.Generic;
using MP.Annotations.CodeAnalysis;
using System.Diagnostics.CodeAnalysis;

namespace MP.ExtensibilitySystem
{
    /// <summary>
    /// Defines the base implementation for a compatible extensibility system engine. <br />
    /// This class must be overriden.
    /// </summary>
    public abstract class ExtensionEngine : IDisposable
    {
        [Flags]
        private enum ExtEngineStateFlags : System.Byte
        {
            None = 0,
            LoadedCleanly = 1,
            Unloaded = 2,
        }

        /// <summary>
        /// Accesses the collective settings object for all the extensions for this extension engine instance.
        /// </summary>
        protected readonly ExtensionsSettingsHolder Settings;

        private ExtEngineStateFlags flags;
        private IList<ExtensionPackage> pkgs;
        private EngineVersioningInformation versioninginfo;
        private IList<ExtensibilitySystemExtension> mgdextensions;
        private List<ManagedExtensionLoadFailedException> extsfailed;

        /// <summary>
        /// Constructs a new <see cref="ExtensionEngine"/> instance, and additionally passing in versioning information required for the extensions to work.
        /// </summary>
        /// <param name="verinfo">The versioning information to pass. Must be non-null.</param>
        protected ExtensionEngine(EngineVersioningInformation verinfo)
        {
            if (verinfo is null) {
                throw new ArgumentNullException(nameof(verinfo));
            }
            versioninginfo = verinfo;
            Settings = new();
            extsfailed = new List<ManagedExtensionLoadFailedException>(1);
            mgdextensions = new List<ExtensibilitySystemExtension>(3);
        }

        /// <summary>
        /// Loads sequentially all the resources and managed extensions that do exist in the current engine instance.
        /// </summary>
        public void Load()
        {
            DebugProvider.WriteLine("EXTENGINE: MP Extension Engine instance up and running now!");
            DebugProvider.WriteLine("EXTENGINE: Executing user code...");
            OnLoad();
            DebugProvider.WriteLine("EXTENGINE: Loading extensions settings...");
            LoadSettings();
            DebugProvider.WriteLine("EXTENGINE: Discovering extension packages...");
            pkgs = FindAndLoadPackages();
            if (pkgs is null) {
                DebugProvider.WriteLine("EXTENGINE: Cannot load any packages, terminating load sequence.");
                flags |= ExtEngineStateFlags.LoadedCleanly;
                return; 
            }
            DebugProvider.WriteLine($"EXTENGINE: Discovered {pkgs.Count} packages!!!");
            foreach (var pkg in pkgs)
            {
                DebugProvider.WriteLine($"EXTENGINE: Executing user code for package {pkg.Name}...");
                OnPackageLoadAdditionalUserCode(pkg);
                DebugProvider.WriteLine($"EXTENGINE: Loading assets of package {pkg.Name} ...");
                foreach (var asset in pkg.Assets)
                {
                    DebugProvider.WriteLine($"EXTENGINE: Verifying asset {asset.Name} ...");
                    if (VerifyAssetHash(asset , pkg) == false) {
                        throw new AssetIntegrityFailedException(asset.Name , pkg.Name);
                    }
                    DebugProvider.WriteLine($"EXTENGINE: Verification successfull!!!");
                    OnAssetLoadAdditionalUserCode(asset , pkg);
                    if (asset is ManagedAsset mgd) {
                        DebugProvider.WriteLine($"EXTENGINE: Loading managed asset {mgd.Name} into context.");
                        LoadManagedAssetAndRegister(mgd, pkg.GetAssetStream(mgd));
                        DebugProvider.WriteLine($"EXTENGINE: Managed asset loaded into context successfully!!!");
                    }
                }
            }
            DebugProvider.WriteLine("EXTENGINE: Done loading extensions!!");
            DebugProvider.WriteLine("EXTENGINE: Loading all the managed extensions...");
            for (System.Int32 I = 0; I < mgdextensions.Count; I++) 
            {
                try {
                    mgdextensions[I].OnLoad();
                } catch (Exception ex) {
                    DebugProvider.WriteLine($"EXTENGINE: A managed extension failed to be loaded cleanly into context: \n{ex}");
                    extsfailed.Add(new(ex));
                    mgdextensions.RemoveAt(I);
                }
            }
            DebugProvider.WriteLine($"EXTENGINE: Done loading! Sucessfully loaded {mgdextensions.Count} managed extensions.");
            flags |= ExtEngineStateFlags.LoadedCleanly;
        }

        /// <summary>
        /// Unloads and does most of the uninitialization tasks.
        /// </summary>
        public void Unload()
        {
            if (flags.HasFlag(ExtEngineStateFlags.LoadedCleanly) == false) {
                throw new InvalidOperationException("Cannot unload when not loaded!");
            }
            DebugProvider.WriteLine("EXTENGINE: Shutting down Extension Engine services...");
            DebugProvider.WriteLine("EXTENGINE: Executing user code...");
            OnUnload();
            DebugProvider.WriteLine("EXTENGINE: Unloading now continues to run...");
            // First send gracefully the OnUnload message to the managed extensions
            foreach (var ext in mgdextensions)
            {
                try {
                    ext.OnUnload();
                } catch (Exception ex) {
                    DebugProvider.WriteLine($"EXTENGINE: A managed extension failed to be unloaded cleanly: \n{ex}");
                    extsfailed.Add(new(ex));
                }
            }
            // Then, destroy all the packages
            foreach (var pkg in pkgs)
            {
                pkg.Dispose();
            }
            DebugProvider.WriteLine("EXTENGINE: Successfull shut-down. The engine was unloaded.");
            SaveSettings();
            // Finally set the flag.
            flags |= ExtEngineStateFlags.Unloaded;
        }

        /// <summary>
        /// Calls <see cref="ExtensibilitySystemExtension.GetService"/> on each of the currently registered extensions.
        /// </summary>
        /// <param name="type">The type of the request to dispatch.</param>
        /// <param name="additional">Additional data required to additionally pass. May be null as well.</param>
        /// <returns>The results returned from all the extensions. If no extensions can provide this request with data , this returns an empty list.</returns>
        /// <exception cref="AggregateException">One or more exceptions were thrown and none did provide data.</exception>
        [return: MaybeReturnEmptyCollectionButNeverNull]
        public IList<System.Object> DispatchRequest(SystemRequestType type, [MaybeNull] System.Object additional)
        {
            DebugProvider.WriteLine($"EXTENGINE: Dispatching request, type is {type}.");
            System.Object ret;
            List<System.Object> results = new();
            List<BaseException> exceptions = new();
            foreach (var ext in mgdextensions)
            {
                if (ext.GetService(type, additional, out ret))
                {
                    DebugProvider.WriteLine("EXTENGINE: GetService succeeded, registering results.");
                    results.Add(ret);
                }
                else if (ext.LastException is not null)
                {
                    DebugProvider.WriteLine($"EXTENGINE: Recording exception {ext.LastException.GetType()}. This exception may be thrown if the request is not handled by any extensions after all.");
                    exceptions.Add(ext.LastException);
                }
            }
            if (results.Count > 0)
            {
                if (exceptions.Count > 0)
                {
                    DebugProvider.WriteLine("EXTENGINE: Popping out exceptions since we have a successfull dispatch.");
#if DEBUG
                    DebugProvider.WriteLine("EXTENGINE: Reporting the recoded exceptions for completeness: ");
                    for (System.Int32 I = 0; I < exceptions.Count; I++)
                    {
                        DebugProvider.WriteLine($"EXTENGINE: Exception {I + 1}: \n{exceptions[I]}");
                    }
#endif
                    exceptions.Clear();
                }
                return results;
            }
            else if (exceptions.Count == 0)
            {
                return results;
            }
            throw new AggregateException("The dispatching request was failed.", exceptions);
        }

        /// <summary>
        /// Loads the settings for all the currently defined extensions.
        /// The settings must be written to the object returned by the <see cref="Settings"/> field.
        /// </summary>
        protected abstract void LoadSettings();

        /// <summary>
        /// Saves the settings for all the currently defined extensions.
        /// The settings must be written from the object returned by the <see cref="Settings"/> field.
        /// </summary>
        protected abstract void SaveSettings();

        /// <summary>
        /// Finds and loads all the extension packages as defined by the app developer.
        /// </summary>
        /// <returns>A list of all the loaded extension packages</returns>
        [return: MaybeNull]
        protected abstract IList<ExtensionPackage> FindAndLoadPackages();

        /// <summary>Verifies the asset's integrity.</summary>
        /// <param name="asset">The asset to check for it's integrity.</param>
        /// <param name="pkg">The package where this asset was loaded from.</param>
        /// <returns><see langword="true"/> if the asset's integrity is OK; otherwise <see langword="false"/>.</returns>
        protected abstract System.Boolean VerifyAssetHash([DisallowNull] IAsset asset, [DisallowNull] ExtensionPackage pkg);

        /// <summary>
        /// Defines a placeholder method for checking and further performing actions after an asset has been verified.
        /// </summary>
        /// <param name="asset">The asset to do additional operations on it.</param>
        /// <param name="pkg">The package where this asset is loaded from</param>
        protected virtual void OnAssetLoadAdditionalUserCode([DisallowNull] IAsset asset , [DisallowNull] ExtensionPackage pkg) { }

        /// <summary>
        /// Defines a placeholder method for further performing actions just after a package has been loaded.
        /// </summary>
        /// <param name="pkg">The package that was loaded.</param>
        protected virtual void OnPackageLoadAdditionalUserCode([DisallowNull] ExtensionPackage pkg) { }

        /// <summary>
        /// This method is called when the <see cref="Load"/> method has been invoked. <br />
        /// Override this method instead to provide your own initialization code to be run along with the Load contents.
        /// </summary>
        protected virtual void OnLoad() { }

        /// <summary>
        /// This method is called when the <see cref="Unload"/> method has been invoked. <br />
        /// Override this method instead to provide your own uninitialization code to be run along with the Unload contents.
        /// </summary>
        protected virtual void OnUnload() { }

        [System.Diagnostics.StackTraceHidden]
        private void LoadManagedAssetAndRegister(ManagedAsset asset , System.IO.Stream stream)
        {
            System.Reflection.Assembly assy;
            System.Byte[] assydata = null;
            try {
                DebugProvider.WriteLine($"EXTENGINE: Reading assembly {asset.Name}.");
                assydata = stream.ReadBytes(stream.Length);
                DebugProvider.WriteLine($"EXTENGINE: Loading assembly {asset.Name}.");
                assy = System.Reflection.Assembly.Load(assydata);
                DebugProvider.WriteLine($"EXTENGINE: Assembly loaded: {assy.GetName()}");
            } catch (System.Exception ex) { 
                throw new UnloadableManagedAssetException(asset.Name, ex);
            } finally {
                stream?.Dispose();
                stream = null;
                assydata = null;
            }
            if (assy.HasAttribute(typeof(ManagedExtensionAssemblyAttribute)) == false)
            {
                throw new InvalidManagedAssetLayoutException(asset.Name , "The extension assembly has not been decorated with the ManagedExtensionAssemblyAttribute.");
            }
            Type t;
            try {
                t = assy.GetType(asset.EntryPointTypeName, true, true);
            } catch (Exception ex) {
                throw new UnloadableManagedAssetException(asset.Name, ex);
            }
            if (t.HasAttribute(typeof(ManagedExtensibilityModuleLoaderClassAttribute)) == false)
            {
                throw new InvalidManagedAssetLayoutException(asset.Name, $"The type {t.Name} has not been decorated with the ManagedExtensibilityModuleLoaderClassAttribute.");
            }
            if (t.DerivesFrom(typeof(ExtensibilitySystemExtension)))
            {
                throw new InvalidManagedAssetLayoutException(asset.Name , $"The type {t.Name} does not derive from the ExtensibilitySystemExtension class.");
            }
            try {
                var ext = Activator.CreateInstance(t, Settings) as ExtensibilitySystemExtension;
                if (ext.IsSupported(versioninginfo) == false)
                {
                    DebugProvider.WriteLine("EXTENGINE: Cannot load the extension. The extension reports that based on the current versioning information, it is not supported.");
                    return;
                }
                mgdextensions.Add(ext);
            } catch (Exception ex) { 
                throw new UnloadableManagedAssetException(asset.Name, ex);
            }
        }

        /// <summary>
        /// Gets a collection that contains all the extensions that were failed during the load stage.
        /// </summary>
        public IEnumerable<ManagedExtensionLoadFailedException> FailedExtensions => extsfailed;

        /// <summary>
        /// Gets a collection that contains all the packages that are part of the current engine instance.
        /// </summary>
        public IEnumerable<ExtensionPackage> Packages => pkgs;

        /// <summary>
        /// Gets the number of loaded extension packages for this engine session.
        /// </summary>
        public System.Int32 LoadedPackages => pkgs.Count;

        /// <summary>
        /// Gets the number of failed extensions for this engine session.
        /// </summary>
        public System.Int32 FailedExtensionsCount => extsfailed.Count;

        /// <summary>
        /// Gets the version of the underlying extension engine that is running.
        /// </summary>
        public EngineVersioningInformation EngineVersioning => versioninginfo;

        /// <summary>
        /// Given a package name, the method searches through all the loaded extension packages to see if the name provided is loaded as a package.
        /// </summary>
        /// <param name="packageid">The package name to find.</param>
        /// <returns>A value whether the package was loaded into this instance.</returns>
        public System.Boolean IsPackageLoaded(System.String packageid)
        {
            if (flags.HasFlag(ExtEngineStateFlags.Unloaded))
            {
                throw new InvalidOperationException("The results are undefined now that the engine is in unload stage.");
            }
            foreach (var p in pkgs)
            {
                if (p.Name.Equals(packageid, StringComparison.InvariantCultureIgnoreCase)) { return true; }
            }
            return false;
        }

        /// <summary>
        /// Disposes all the resources used by this <see cref="ExtensionEngine"/> instance. <br />
        /// For correct disposal of all the resources, you should call this method too by using the <see langword="base"/> convention.
        /// </summary>
        /// <param name="disposing"><see langword="true"/> when this is called from <see cref="Dispose()"/>; otherwise, <see langword="false"/> and it was called from the finalizer.</param>
        protected virtual void Dispose(System.Boolean disposing) 
        {
            if (disposing) 
            {
                if (pkgs is not null)
                {
                    foreach (ExtensionPackage pkg in pkgs)
                    {
                        pkg.Dispose();
                    }
                    pkgs.Clear();
                    pkgs = null;
                }
                if (extsfailed is not null)
                {
                    extsfailed.Clear();
                    extsfailed = null;
                }
                if (mgdextensions is not null)
                {
                    mgdextensions.Clear();
                    mgdextensions = null;
                }
            }
        }

        /// <summary>
        /// Disposes this <see cref="ExtensionEngine"/> instance.
        /// </summary>
        public void Dispose()
        {
            if (flags.HasFlag(ExtEngineStateFlags.Unloaded) == false) {
                throw new InvalidOperationException("The engine has not been unloaded yet.");
            }
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>Default finalizer.</summary>
        ~ExtensionEngine() => Dispose(false);
    }
}