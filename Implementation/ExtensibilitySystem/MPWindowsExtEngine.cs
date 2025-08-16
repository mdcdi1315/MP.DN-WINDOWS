
using System;
using Microsoft.IO;
using MP.CGISettings;
using MP.ExtSystemApi;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Microsoft.Security.Cryptography;

namespace MP.ExtensibilitySystem
{
    public sealed class MPWindowsExtEngine : ExtensionEngine
    {
        private sealed class VerInfo : EngineVersioningInformation
        {
            private Version engver;
            private IList<Version> vers;

            public VerInfo()
            {
                engver = new(1, 0, 0, 0);
                vers = [
                    new(AppInfo.Version),
                    typeof(NativeLibraryLoader).Assembly.GetName().Version,
                    typeof(SystemInfo).Assembly.GetName().Version,
                ];
            }

            public override Version EngineVersion => engver;

            public override IList<Version> AppVersions => vers;
        }

        private DirectoryInfo baseworkdir , basefinddir;

        public MPWindowsExtEngine(DirectoryInfo diwork , DirectoryInfo difind) : base(new VerInfo()) 
        {
            ArgumentNullException.ThrowIfNull(diwork);
            ArgumentNullException.ThrowIfNull(difind);
            baseworkdir = diwork; 
            basefinddir = difind; 
        }

        protected override void LoadSettings()
        {
            FileInfo fi = baseworkdir.GetFile("extsettings.cgi");
            if (fi is null) {
                DebugProvider.WriteLine("EXTENGINE: WARN: Not loading settings since they do not exist.");
                return;
            }
            FileStream fsm = null;
            CGISettingsReader rdr = null;
            try {
                fsm = fi.OpenRead();
                rdr = new CGISettingsReader(fsm);
                rdr.RegisterExtension(new ColorExtension());
                rdr.RegisterExtension(new DirectoryInfoExtension());
                rdr.RegisterExtension(new FileInfoExtension());
                foreach (var s in rdr)
                {
                    Settings.SetAttribute(s.Name, s.Value);
                }
            } catch (Exception ex) {
                DebugProvider.WriteLine($"EXTENGINE: WARN: Cannot load settings file due to an error: {ex}");
            } finally {
                rdr?.Dispose();
                rdr = null;
                fsm?.Dispose();
                fsm = null;
            }
        }

        protected override void SaveSettings()
        {
            FileStream fsm = null;
            CGISettingsWriter wr = null;
            try {
                fsm = baseworkdir.CreateFileStream("extsettings.cgi");
                wr = new CGISettingsWriter(fsm);
                wr.ApplicationName = "MP_EXTSYSTEMSETTINGS";
                wr.RegisterExtension(new ColorExtension());
                wr.RegisterExtension(new DirectoryInfoExtension());
                wr.RegisterExtension(new FileInfoExtension());
                foreach (var setting in Settings.Keys)
                {
                    wr.Add(new() { Name = setting, Value = Settings.GetAttribute(setting) });
                }
                wr.Generate();
            } catch (Exception ex) {
                DebugProvider.WriteLine($"EXTENGINE: WARN: Cannot save settings file due to an error: {ex}");
            } finally {
                wr?.Dispose(); 
                wr = null;
                try {
                    fsm?.Dispose();
                    fsm = null;
                } catch (Exception ex) {
                    DebugProvider.WriteLine($"EXTENGINE: WARN: Cannot save settings file due to an error: {ex}");
                }
            }
        }

        [return: MaybeNull]
        protected override IList<ExtensionPackage> FindAndLoadPackages()
        {
            List<ExtensionPackage> pkgs = new();
            FileStream fsm = null;
            foreach (var pak in basefinddir.GetFiles("*.zpkg"))
            {
                try {
                    fsm = pak.OpenRead();
                    pkgs.Add(new ZipBasedPackage(fsm) { IsStreamOwner = true });
                } catch (InvalidPackageLayoutException ex) {
                    fsm?.Dispose();
                    DebugProvider.WriteLine($"EXTENGINE: Cannot load package {pak.Name}: {ex}");
                } catch (Exception e) when (e switch { UnauthorizedAccessException => true , System.IO.IOException => true ,  _ => false }) {
                    DebugProvider.WriteLine($"EXTENGINE: Cannot load file {pak.Name}: {e}");
                } finally {
                    fsm = null;
                }
            }
            foreach (var pak in basefinddir.GetDirectories())
            {
                try {
                    pkgs.Add(new DirectoryBasedPackage(pak));
                } catch (InvalidPackageLayoutException ex) {
                    DebugProvider.WriteLine($"EXTENGINE: Cannot load package {pak.Name}: {ex}");
                }
            }
            foreach (var pak in basefinddir.GetFiles("*.pkgrsrc"))
            {
                try {
                    fsm = pak.OpenRead();
                    pkgs.Add(new ResourcesPackage(fsm) { IsStreamOwner = true });
                } catch (InvalidPackageLayoutException ex) {
                    fsm?.Dispose();
                    DebugProvider.WriteLine($"EXTENGINE: Cannot load package {pak.Name}: {ex}");
                } catch (Exception e) when (e switch { UnauthorizedAccessException => true, System.IO.IOException => true, _ => false }) {
                    DebugProvider.WriteLine($"EXTENGINE: Cannot load file {pak.Name}: {e}");
                } finally {
                    fsm = null;
                }
            }
            return pkgs;
        }

        private static System.String GetUpperHashString(System.Byte[] data)
        {
            System.Text.StringBuilder sb = new(data.Length * 2);
            foreach (System.Byte b in data) {
                sb.Append(b.ToString("X2"));
            }
            return sb.ToString();
        }

        private static System.Boolean VerifyAssetCommon(System.IO.Stream ds , System.String hashexpected , AssetVerificationHashType type)
        {
            switch (type)
            {
                case AssetVerificationHashType.SHA256:
                    return GetUpperHashString(SHA256.HashData(ds)).Equals(hashexpected, StringComparison.InvariantCultureIgnoreCase);
                case AssetVerificationHashType.SHA512:
                    return GetUpperHashString(SHA512.HashData(ds)).Equals(hashexpected, StringComparison.InvariantCultureIgnoreCase);
                case AssetVerificationHashType.MD5:
                    return GetUpperHashString(MD5.HashData(ds)).Equals(hashexpected, StringComparison.InvariantCultureIgnoreCase);
                default:
                    DebugProvider.WriteLine($"EXTENGINE: Cannot recognize algorithm type {type}. Presuming that the hash exists and is invalid.");
                    return false;
            }
        }

        protected override System.Boolean VerifyAssetHash([DisallowNull] IAsset asset, [DisallowNull] ExtensionPackage pkg)
        {
            System.IO.Stream strm = null;
            try {
                strm = pkg.GetAssetStream(asset);
                return VerifyAssetCommon(strm, asset.VerificationHash, asset.HashType);
            } finally {
                strm?.Dispose();
                strm = null;
            }
        }

        protected override void OnAssetLoadAdditionalUserCode([DisallowNull] IAsset asset, [DisallowNull] ExtensionPackage pkg)
        {
            // If the given asset name ends in .DLL it is a native DLL and must be copied to a custom directory and AddDllDirectory to it
            // so that the managed components can find the required native DLL's.
            if (asset.Name.EndsWith(".dll" , StringComparison.InvariantCultureIgnoreCase))
            {
                DebugProvider.WriteLine($"EXTENGINE: WARN: Asset named as {asset.Name} seems to be a native library adding it to the search path.");
                // OK. Now create the native directory if that does not exist.
                DirectoryInfo di = baseworkdir.GetSubDirectory("Native");
                FileStream fsm = null;
                System.IO.Stream source = null;
                try {
                    if (di.Exists == false) { di.Create(); }
                    if (di.FileExists(asset.Name))
                    {
                        FileStream fs2 = null;
                        DebugProvider.WriteLine("EXTENGINE: A file named the same does already exist. Verifying that the package can use it...");
                        try {
                            fs2 = di.OpenReadFileStream(asset.Name);
                            if (VerifyAssetCommon(fs2 , asset.VerificationHash, asset.HashType) == false)
                            {
                                DebugProvider.WriteLine($"EXTENGINE: ERROR: The hashes do not match for the package named as {pkg.Name} and version {pkg.Version}. This might cause loading errors in the other extensions as well.");
                            } else {
                                DebugProvider.WriteLine($"EXTENGINE: The file is usuable by the package {pkg.Name}.");
                            }
                        } catch (Exception ex) {
                            DebugProvider.WriteLine($"EXTENGINE: WARN: Cannot run verification checks around the asset {asset.Name}. Operation failed.");
                            DebugProvider.WriteLine($"EXTENGINE: WARN: Reporting the exception that made it fail: {ex}");
                            return;
                        } finally {
                            fs2?.Dispose();
                            fs2 = null;
                        }
                        return;
                    }
                    fsm = di.CreateFileStream(asset.Name);
                    source = pkg.GetAssetStream(asset);
                    source.DirectCopyToStream(fsm);
                } catch (Exception ex) {
                    DebugProvider.WriteLine($"EXTENGINE: Cannot write the native DLL. Error occured. Error details: \n{ex}");
                } finally {
                    fsm?.Dispose();
                    fsm = null;
                    source?.Dispose();
                    source = null;
                }
            }
        }

        protected override void OnLoad()
        {
            var nativedir = baseworkdir.GetSubDirectory("Native");
            nativedir.Create();
            DebugProvider.WriteLine($"EXTENGINE: Registering Native DLL directory for extensions: {nativedir.FullName}");
            NativeLibraryLoader.AddDLLSearchDirectory(nativedir.FullName);
        }
    }
}