
using System;
using Microsoft.IO;
using MP.ExtensibilitySystem;
using System.Collections.Generic;

namespace MP.ExtSystemApi
{
    public sealed class DirectoryBasedPackage : ExtensionPackage
    {
        private DirectoryInfo basedir;
        private PackagesJsonDataReader readerjson;

        public DirectoryBasedPackage(System.String path) : this(new DirectoryInfo(path)) { }

        public DirectoryBasedPackage(DirectoryInfo di)
        {
            ArgumentNullException.ThrowIfNull(di , nameof(di));
            if (di.Exists == false)
            {
                throw new ArgumentException("The current directory info object points to a non-existent directory." , nameof(di));
            }
            basedir = di;
            ReadPackage();
        }

        private void ReadPackage()
        {
            FileStream fsm = null;
            try {
                fsm = basedir.OpenReadFileStream("package.json");
                readerjson = new(fsm);
            } catch (UnauthorizedAccessException uae) {
                throw new InvalidPackageLayoutException("package.json file cannot be loaded due to a security violation.", uae);
            } catch (System.IO.IOException ioex) {
                throw new InvalidPackageLayoutException("Cannot find or load the package.json file.", ioex);
            } catch (InvalidPackageJsonFormatException jef) {
                throw new InvalidPackageLayoutException("package.json file has invalid content." , jef);
            } finally {
                fsm?.Dispose();
                fsm = null;
            }
        }

        public override IList<IAsset> Assets => readerjson.Assets;

        public override string Name => readerjson.Name;

        public override string Description => readerjson.Description;

        public override Version Version => readerjson.Version;

        protected override System.IO.Stream GetAssetStreamImpl(IAsset asset)
        {
            FileInfo fe = basedir.GetFile(asset.Name);
            if (fe.Exists == false) {
                throw new UnreachableAssetException($"Cannot find the file {fe.Name} into the directory {fe.DirectoryName} .");
            }
            try {
                return fe.OpenRead();
            } catch (UnauthorizedAccessException uae) {
                throw new UnreachableAssetException("File cannot be loaded because of a security violation.", uae);
            } catch (System.IO.IOException ioex) {
                throw new UnreachableAssetException("File cannot be opened due to an I/O error." , ioex);
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing) { 
                readerjson = null;
                basedir = null;
            }
            base.Dispose(disposing);
        }
    }
}