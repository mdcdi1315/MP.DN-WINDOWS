

using System;
using System.Text.Json;
using MP.ExtensibilitySystem;
using System.Collections.Generic;

namespace MP.ExtSystemApi
{
    public sealed class PackagesJsonDataReader 
    {
        private sealed class JS_ResourceOrNativeAsset : IAsset
        {
            private System.String name, hash;
            private AssetVerificationHashType type;

            public JS_ResourceOrNativeAsset(JsonElement element)
            {
                JsonElement val;
                if (element.TryGetProperty("name", out val) == false)  
                {
                    throw new InvalidPackageJsonFormatException("Cannot find the required property 'name' of the given asset.");
                }
                if (val.ValueKind != JsonValueKind.String)
                {
                    throw new InvalidPackageJsonFormatException("The 'name' property must be a JSON string.");
                }
                name = val.GetString();
                if (element.TryGetProperty("hash", out val) == false)
                {
                    throw new InvalidPackageJsonFormatException($"Cannot find the required property 'hash' of the asset {name}.");
                }
                if (val.ValueKind != JsonValueKind.String)
                {
                    throw new InvalidPackageJsonFormatException($"The 'hash' property of {name} must be a JSON string.");
                }
                hash = val.GetString();
                if (element.TryGetProperty("hashtype", out val) == false)
                {
                    throw new InvalidPackageJsonFormatException($"Cannot find the required property 'hashtype' of the asset {name}.");
                }
                if (val.ValueKind != JsonValueKind.String)
                {
                    throw new InvalidPackageJsonFormatException($"The 'hashtype' property of {name} must be a JSON string.");
                }
                switch (val.GetString().ToUpperInvariant())
                {
                    case "SHA256":
                        type = AssetVerificationHashType.SHA256;
                        break;
                    case "SHA512":
                        type = AssetVerificationHashType.SHA512;
                        break;
                    case "MD5":
                        type = AssetVerificationHashType.MD5;
                        break;
                    default:
                        throw new InvalidPackageJsonFormatException($"The 'hashtype' property of the asset {name} must match either the \"SHA256\" , \"SHA512\" or \"MD5\" strings.");
                }
            }

            public string Name { get => name; set => name = value; }

            public string VerificationHash
            {
                get => hash;
                set => hash = value;
            }

            public AssetVerificationHashType HashType
            {
                get => type;
                set => type = value;
            }
        }

        private sealed class JS_ManagedAsset : ManagedAsset
        {
            private System.String name, hash , tn;
            private AssetVerificationHashType type;

            public JS_ManagedAsset(JsonElement element)
            {
                JsonElement val;
                if (element.TryGetProperty("name", out val) == false)
                {
                    throw new InvalidPackageJsonFormatException("Cannot find the required property 'name' of the given asset.");
                }
                if (val.ValueKind != JsonValueKind.String)
                {
                    throw new InvalidPackageJsonFormatException("The 'name' property must be a JSON string.");
                }
                name = val.GetString();
                if (element.TryGetProperty("hash", out val) == false)
                {
                    throw new InvalidPackageJsonFormatException($"Cannot find the required property 'hash' of the asset {name}.");
                }
                if (val.ValueKind != JsonValueKind.String)
                {
                    throw new InvalidPackageJsonFormatException($"The 'hash' property of {name} must be a JSON string.");
                }
                hash = val.GetString();
                if (element.TryGetProperty("hashtype", out val) == false)
                {
                    throw new InvalidPackageJsonFormatException($"Cannot find the required property 'hashtype' of the asset {name}.");
                }
                if (val.ValueKind != JsonValueKind.String)
                {
                    throw new InvalidPackageJsonFormatException($"The 'hashtype' property of {name} must be a JSON string.");
                }
                switch (val.GetString().ToUpperInvariant())
                {
                    case "SHA256":
                        type = AssetVerificationHashType.SHA256;
                        break;
                    case "SHA512":
                        type = AssetVerificationHashType.SHA512;
                        break;
                    case "MD5":
                        type = AssetVerificationHashType.MD5;
                        break;
                    default:
                        throw new InvalidPackageJsonFormatException($"The 'hashtype' property of the asset {name} must match either the \"SHA256\" , \"SHA512\" or \"MD5\" strings.");
                }
                if (element.TryGetProperty("entrypointname", out val) == false)
                {
                    throw new InvalidPackageJsonFormatException($"Cannot find the required property 'entrypointname' of the asset {name}.");
                }
                if (val.ValueKind != JsonValueKind.String)
                {
                    throw new InvalidPackageJsonFormatException($"The 'entrypointname' property of {name} must be a JSON string.");
                }
                tn = val.GetString();
            }

            public override string Name => name;

            public override string VerificationHash => hash;

            public override AssetVerificationHashType HashType => type;

            public override string EntryPointTypeName => tn;
        }

        private Version ver;
        private List<IAsset> assets;
        private System.String name, desc;

        public PackagesJsonDataReader(System.IO.Stream stream)
        {
            ArgumentNullException.ThrowIfNull(stream, nameof(stream));
            if (stream.CanRead == false)
            {
                throw new ArgumentException("Stream was unreadable." , nameof(stream));
            }
            assets = new(1);
            Read(stream);
        }

        private static System.String GetStringProp(JsonElement el , System.String pn)
        {
            if (el.TryGetProperty(pn , out var val)) {
                if (val.ValueKind != JsonValueKind.String)
                {
                    throw new InvalidPackageJsonFormatException($"Required property {pn} is not a string.");
                }
                return val.GetString();
            }
            throw new InvalidPackageJsonFormatException($"Cannot find the required property {pn}.");
        }

        private void Read(System.IO.Stream stream) 
        {
            JsonElement temp;
            JsonDocument jdt = null;
            try {
                jdt = JsonDocument.Parse(stream, new() { CommentHandling = JsonCommentHandling.Skip, MaxDepth = 4 });
                name = GetStringProp(jdt.RootElement, "id");
                desc = GetStringProp(jdt.RootElement, "description");
                ver = new(GetStringProp(jdt.RootElement, "version"));
                if (jdt.RootElement.TryGetProperty("managedassets" , out temp))
                {
                    switch (temp.ValueKind)
                    {
                        case JsonValueKind.Null:
                            // Just no need of managed assets.
                            break;
                        case JsonValueKind.Array:
                            assets.EnsureCapacity(assets.Count + temp.GetArrayLength());
                            foreach (var el in temp.EnumerateArray())
                            {
                                if (el.ValueKind != JsonValueKind.Object) {
                                    throw new InvalidPackageJsonFormatException("All the values in the 'managedassets' array must be JSON objects!!");
                                }
                                assets.Add(new JS_ManagedAsset(el));
                            }
                            break;
                        default:
                            throw new InvalidPackageJsonFormatException("'managedassets' was not null or a JSON array!!");
                    }
                }
                if (jdt.RootElement.TryGetProperty("nativeassets", out temp))
                {
                    switch (temp.ValueKind)
                    {
                        case JsonValueKind.Null:
                            // Just no need of managed assets.
                            break;
                        case JsonValueKind.Array:
                            assets.EnsureCapacity(assets.Count + temp.GetArrayLength());
                            foreach (var el in temp.EnumerateArray())
                            {
                                if (el.ValueKind != JsonValueKind.Object)
                                {
                                    throw new InvalidPackageJsonFormatException("All the values in the 'nativeassets' array must be JSON objects!!");
                                }
                                assets.Add(new JS_ResourceOrNativeAsset(el));
                            }
                            break;
                        default:
                            throw new InvalidPackageJsonFormatException("'nativeassets' was not null or a JSON array!!");
                    }
                }
                if (jdt.RootElement.TryGetProperty("resources", out temp))
                {
                    switch (temp.ValueKind)
                    {
                        case JsonValueKind.Null:
                            // Just no need of managed assets.
                            break;
                        case JsonValueKind.Array:
                            assets.EnsureCapacity(assets.Count + temp.GetArrayLength());
                            foreach (var el in temp.EnumerateArray())
                            {
                                if (el.ValueKind != JsonValueKind.Object)
                                {
                                    throw new InvalidPackageJsonFormatException("All the values in the 'resources' array must be JSON objects!!");
                                }
                                assets.Add(new JS_ResourceOrNativeAsset(el));
                            }
                            break;
                        default:
                            throw new InvalidPackageJsonFormatException("'resources' was not null or a JSON array!!");
                    }
                }
            } finally {
                jdt?.Dispose();
                jdt = null;
            }
        }

        public Version Version => ver;

        public System.String Name => name;

        public IList<IAsset> Assets => assets;

        public System.String Description => desc;
    }
}