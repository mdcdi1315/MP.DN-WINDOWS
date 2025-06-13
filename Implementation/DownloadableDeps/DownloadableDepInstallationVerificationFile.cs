
using System;
using System.Collections.Generic;

namespace MP.DownloadableDeps
{
    public sealed class DownloadableDepsVerificationFReader : IDisposable
    {
        private System.Text.Json.JsonElement jelbase;
        private System.Text.Json.JsonDocument jdt;

        public DownloadableDepsVerificationFReader(System.IO.Stream stream)
        {
            jdt = System.Text.Json.JsonDocument.Parse(stream , new() { MaxDepth = 6 , CommentHandling = System.Text.Json.JsonCommentHandling.Skip });
            jelbase = jdt.RootElement;
            try
            {
                if (GetBaseStringProperty("schema") != "MDCDI1315.MP.DDEPINST")
                {
                    throw new MP.ExceptionSystem.InvalidDDepsDownloadFormatException("Schema string does not agree with the expected schema string.");
                }
                if (GetBaseIntProperty("version") > 1)
                {
                    throw new MP.ExceptionSystem.InvalidDDepsDownloadFormatException("This reader can only read the version 1 of the Downloadable Dependencies JSON format.");
                }
            } catch (System.Exception ex)
            {
                throw new MP.ExceptionSystem.InvalidDDepsDownloadFormatException("Cannot parse this Downloadable Dependencies JSON stream.", ex);
            }
        }

        private System.String GetBaseStringProperty(System.String name) => jelbase.GetProperty(name).GetString();

        private System.Int32 GetBaseIntProperty(System.String name) => jelbase.GetProperty(name).GetInt32();

        private static VerifierDependencyCheck ParseDependencyFromElement(System.Text.Json.JsonElement el)
        {
            if (el.ValueKind != System.Text.Json.JsonValueKind.Object) { throw new MP.ExceptionSystem.InvalidDDepsDownloadFormatException("The given element was not an object dependency."); }
            try
            {
                System.String name = el.GetProperty("Name").GetString(), desc = el.GetProperty("Description").GetString(), url = el.GetProperty("URL").GetString();
                System.Boolean zipdownload = el.GetProperty("ZipDownload").GetBoolean();
                DownloadableDependency dep = new(name, desc, url, zipdownload);
                System.Text.Json.JsonElement temp = el.GetProperty("Results");
                dep.SavePath = new() {
                    RelativePath = temp.GetProperty("SavePath").GetString(),
                    Type =
                    (temp.GetProperty("IsDirectory").ValueKind == System.Text.Json.JsonValueKind.True)
                    ? PathDetailSaveType.Directory :
                    PathDetailSaveType.File
                };
                temp = el.GetProperty("Installed");
                Dictionary<System.String, System.String> hashes = new();
                foreach (var instf in temp.EnumerateArray()) 
                {
                    if (instf.ValueKind != System.Text.Json.JsonValueKind.Object) { throw new MP.ExceptionSystem.InvalidDDepsDownloadFormatException("All Installed Files elements must be an array that contains verification hash objects!"); }
                    hashes.Add(instf.GetProperty("Name").GetString() , instf.GetProperty("Hash").GetString());
                }
                return new() { VerificationHashes = hashes , Dependency = dep };
            } catch (KeyNotFoundException ex1)
            {
                throw new MP.ExceptionSystem.InvalidDDepsDownloadFormatException($"The JSON element layout expected to have a specified property, but that is missing.", ex1);
            }
        }

        public VerifierDependencyCheck[] DepsInstalled
        {
            get {
                if (jelbase.TryGetProperty("Installed", out var deps) == false)
                {
                    throw new MP.ExceptionSystem.InvalidDDepsDownloadFormatException("Installed property does not exist. If you do not want sub-dependencies you should use an empty array.");
                }
                if (deps.ValueKind != System.Text.Json.JsonValueKind.Array)
                {
                    throw new MP.ExceptionSystem.InvalidDDepsDownloadFormatException("Installed property was not an array. If you do not want sub-dependencies you should use an empty array.");
                }
                System.Int32 arrlen = deps.GetArrayLength();
                if (arrlen == 0) { return Array.Empty<VerifierDependencyCheck>(); }
                VerifierDependencyCheck[] data = new VerifierDependencyCheck[arrlen];
                for (System.Int32 I = 0; I < arrlen; I++)
                {
                    var element = deps[I];
                    if (element.ValueKind != System.Text.Json.JsonValueKind.Object)
                    {
                        throw new MP.ExceptionSystem.InvalidDDepsDownloadFormatException("Installed property was not an array of sub-dependencies objects.");
                    }
                    data[I] = ParseDependencyFromElement(element);
                }
                return data;
            }
        }

        public void Dispose() 
        {
            jdt?.Dispose();
            jdt = null;
        }
    }

    public sealed class DownloadableDepsVerificationFWriter : IDisposable
    {
        private System.Boolean strmown;
        private System.IO.Stream underlying;
        private System.Text.Json.Utf8JsonWriter jw;

        public DownloadableDepsVerificationFWriter(System.IO.Stream strm)
        {
            if (strm is null) { throw new ArgumentNullException(nameof(strm)); }
            underlying = strm;
            jw = new(underlying, new() { MaxDepth = 6 , Indented = true });
            strmown = false;
            WriteInitialData();
        }

        private void WriteInitialData()
        {
            jw.WriteStartObject();
            jw.WriteNumber("version", 1);
            jw.WriteString("schema", "MDCDI1315.MP.DDEPINST");
            jw.WriteStartArray("Installed");
        }

        public System.Boolean IsStreamOwner
        {
            get => strmown;
            set => strmown = value;
        }

        public void WriteInstalledDep(VerifierDependencyCheck dep)
        {
            if (dep is null) { throw new ArgumentNullException(nameof(dep)); }
            if (dep.Dependency is null) { throw new ArgumentException("The dependency details cannot be empty." , nameof(dep)); }
            if (dep.VerificationHashes is null) { throw new ArgumentException("The verification hashes dictionary cannot be empty." , nameof(dep)); }
            if (dep.Dependency.SavePath is null) { throw new ArgumentException("The save path details cannot be empty." , nameof(dep)); }
            jw.WriteStartObject();
            jw.WriteString("Name" , dep.Dependency.Name);
            jw.WriteString("Description" , dep.Dependency.Description);
            jw.WriteString("URL" , dep.Dependency.DownloadURL);
            jw.WriteBoolean("ZipDownload" , dep.Dependency.IsZipFile);
            jw.WriteStartArray("Installed");
            foreach (var f in dep.VerificationHashes) 
            {
                jw.WriteStartObject();
                jw.WriteString("Name" , f.Key);
                jw.WriteString("Hash", f.Value);
                jw.WriteEndObject();
            }
            jw.WriteEndArray();
            jw.WriteStartObject("Results");
            jw.WriteString("SavePath" , dep.Dependency.SavePath.RelativePath);
            jw.WriteBoolean("IsDirectory" , dep.Dependency.SavePath.Type == PathDetailSaveType.Directory);
            jw.WriteEndObject();
            jw.WriteEndObject();
        }

        public void EnsureFlushed() => jw.Flush();

        public void Dispose()
        {
            if (jw is not null)
            {
                jw.WriteEndArray();
                jw.WriteEndObject();
                jw.Flush();
                jw.Dispose();
                jw = null;
            }
            if (underlying is not null) { 
                if (strmown)
                {
                    underlying.Dispose();
                }
                underlying = null;
            }
        }
    }
}