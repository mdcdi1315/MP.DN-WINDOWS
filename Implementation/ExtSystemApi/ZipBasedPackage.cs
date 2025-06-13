
using System;
using System.IO;
using MP.Utilities;
using MP.ExtensibilitySystem;
using System.IO.ManagedZip.Zip;
using System.Collections.Generic;

namespace MP.ExtSystemApi
{
    public sealed class ZipBasedPackage : ExtensionPackage , IStreamOwnerBase
    {
        private Stream stream;
        private System.Boolean strmown;
        private PackagesJsonDataReader readerjson;

        public ZipBasedPackage(Stream stream)
        {
            ArgumentNullException.ThrowIfNull(stream);
            if (stream.CanSeek == false || stream.CanRead == false) {
                throw new ArgumentException("The stream must be both readable and seekable.");
            }
            this.stream = stream;
            ReadPackage();
        }

        public override IList<IAsset> Assets => readerjson.Assets;

        public override string Name => readerjson.Name;

        public override string Description => readerjson.Description;

        public override Version Version => readerjson.Version;

        public bool IsStreamOwner 
        { 
            get => strmown; 
            set => strmown = value; 
        }

        protected override Stream GetAssetStreamImpl(IAsset asset)
        {
            stream.Seek(0, SeekOrigin.Begin);
            using (ZipInputStream zis = new(stream, 4096) { IsStreamOwner = false })
            {
                ZipEntry ze;
                Microsoft.IO.MemoryStream ms = null;
                while ((ze = zis.GetNextEntry()) is not null)
                {
                    if (ze.IsFile && ze.Name.Equals(asset.Name, StringComparison.InvariantCultureIgnoreCase))
                    {
                        try {
                            ms = new();
                            zis.CopyToExactly(ms, ze.Size);
                            ms.Position = 0;
                            return ms;
                        } catch {
                            ms?.Dispose();
                            ms = null;
                            throw;
                        }
                    }
                }
            }
            throw new UnreachableAssetException($"Cannot find the asset {asset.Name} inside the zipped package.");
        }

        private void ReadPackage()
        {
            stream.Seek(0, System.IO.SeekOrigin.Begin);
            using (ZipInputStream zis = new(stream , 4096) { IsStreamOwner = false }) 
            {
                ZipEntry ze;
                Microsoft.IO.MemoryStream ms = null;
                while ((ze = zis.GetNextEntry()) is not null)
                {
                    if (ze.IsFile && ze.Name.Equals("package.json", StringComparison.InvariantCultureIgnoreCase))
                    {
                        try {
                            ms = new(ze.Size);
                            zis.CopyToExactly(ms , ze.Size);
                            readerjson = new PackagesJsonDataReader(ms);
                            break;
                        } finally {
                            ms?.Dispose();
                            ms = null;
                        }
                    }
                }
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing) { 
                readerjson = null;
                if (strmown && stream is not null) { stream.Dispose(); }
                stream = null;
            }
            base.Dispose(disposing);
        }
    }
}