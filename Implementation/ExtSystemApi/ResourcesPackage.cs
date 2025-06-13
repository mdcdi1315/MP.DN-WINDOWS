
using System;
using System.IO;
using MP.Utilities;
using System.Collections;
using MP.ExtensibilitySystem;
using System.Collections.Generic;
using DotNetResourcesExtensions.Internal.DotNetResources;

namespace MP.ExtSystemApi
{
    public sealed class ResourcesPackage : ExtensionPackage , IStreamOwnerBase
    {
        private System.Boolean strmown;
        private System.IO.Stream stream;
        private PackagesJsonDataReader readerjson;
        private DeserializingResourceReader reader;

        public ResourcesPackage(Stream resstream)
        {
            reader = new(resstream);
            strmown = false;
            stream = resstream;
            ReadPackage();
        }

        private void ReadPackage()
        {
            Microsoft.IO.MemoryStream ms = null;
            foreach (DictionaryEntry ent in reader)
            {
                if (ent.Key.ToString().Equals("package.json", StringComparison.InvariantCultureIgnoreCase))
                {
                    try {
                        ms = new(ent.Value as System.Byte[]);
                        readerjson = new(ms);
                        break;
                    } catch (InvalidPackageJsonFormatException ex) { 
                        throw new InvalidPackageLayoutException("Cannot load this extension package, due to a parsing exception." , ex);
                    } finally {
                        ms?.Dispose();
                        ms = null;
                    }
                }
            }
            if (readerjson is null) {
                throw new InvalidPackageLayoutException("Cannot load this extension package. Central archive entry was not found.");
            }
        }

        public override IList<IAsset> Assets => readerjson.Assets;

        public override System.String Name => readerjson.Name;

        public override System.String Description => readerjson.Description;

        public override Version Version => readerjson.Version;

        public System.Boolean IsStreamOwner
        {
            get => strmown;
            set => strmown = value;
        }

        protected override Stream GetAssetStreamImpl(IAsset asset)
        {
            foreach (DictionaryEntry ent in reader) 
            {
                if (asset.Name.Equals(ent.Key.ToString() , StringComparison.InvariantCultureIgnoreCase))
                {
                    return new Microsoft.IO.MemoryStream(ent.Value as System.Byte[]);
                }
            }
            throw new UnreachableAssetException($"Cannot locate the asset {asset.Name} inside the package {readerjson.Name}.");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing) {
                reader?.Dispose();
                reader = null;
                if (strmown && stream is not null) { stream.Dispose(); }
                stream = null;
                readerjson = null;
            }
            base.Dispose(disposing);
        }
    }
}