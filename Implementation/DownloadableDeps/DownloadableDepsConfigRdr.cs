

using System;
using System.Collections.Generic;

namespace MP.DownloadableDeps
{
    public sealed class DownloadableDepConfigReader : IDisposable
    {
        private System.Text.Json.JsonDocument jdt;
        private System.Text.Json.JsonElement jelbase;

        public DownloadableDepConfigReader(System.IO.Stream strm)
        {
            try {
                jdt = System.Text.Json.JsonDocument.Parse(strm, new() { MaxDepth = 4, CommentHandling = System.Text.Json.JsonCommentHandling.Skip });
                jelbase = jdt.RootElement;
            } catch (System.Text.Json.JsonException ex) {
                throw new MP.ExceptionSystem.InvalidDDepsDownloadFormatException("The given stream is not a Downloadable Dependencies JSON stream." , ex);
            }
            if (jelbase.ValueKind != System.Text.Json.JsonValueKind.Object)
            {
                jdt.Dispose();
                throw new MP.ExceptionSystem.InvalidDDepsDownloadFormatException($"This is not correct JSON format. Expected object but got {jelbase.ValueKind} instead.");
            }
            try {
                if (GetBaseStringProperty("schema") != "MDCDI1315.MP.DDEPFMT")
                {
                    throw new MP.ExceptionSystem.InvalidDDepsDownloadFormatException("Schema string does not agree with the expected schema string.");
                }
                if (GetBaseIntProperty("version") > 1)
                {
                    throw new MP.ExceptionSystem.InvalidDDepsDownloadFormatException("This reader can only read the version 1 of the Downloadable Dependencies JSON format.");
                }
            } catch (System.Exception ex) {
                throw new MP.ExceptionSystem.InvalidDDepsDownloadFormatException("Cannot parse this Downloadable Dependencies JSON stream." , ex); 
            }
        }

        private System.String GetBaseStringProperty(System.String name) => jelbase.GetProperty(name).GetString();

        private System.Int32 GetBaseIntProperty(System.String name) => jelbase.GetProperty(name).GetInt32();

        private static DownloadableDependency ParseDependencyFromElement(System.Text.Json.JsonElement el)
        {
            if (el.ValueKind != System.Text.Json.JsonValueKind.Object) { throw new MP.ExceptionSystem.InvalidDDepsDownloadFormatException("The given element was not an object dependency."); }
            try {
                System.String name = el.GetProperty("Name").GetString(), desc = el.GetProperty("Description").GetString(), url = el.GetProperty("URL").GetString();
                System.Boolean zipdownload = el.GetProperty("ZipDownload").GetBoolean();
                DownloadableDependency dep = new(name, desc, url, zipdownload);
                System.Text.Json.JsonElement temp = el.GetProperty("Rules");
                foreach (var rule in temp.EnumerateArray()) 
                {
                    if (rule.ValueKind != System.Text.Json.JsonValueKind.String) { throw new MP.ExceptionSystem.InvalidDDepsDownloadFormatException("All Rules elements must be strings only!"); }
                    dep.InclusionRules.Add(rule.GetString());
                }
                temp = el.GetProperty("UseOnlyFiles");
                foreach (var file in temp.EnumerateArray())
                {
                    if (file.ValueKind != System.Text.Json.JsonValueKind.String) { throw new MP.ExceptionSystem.InvalidDDepsDownloadFormatException("All Included Files elements must be strings only!"); }
                    dep.ImportantFileList.Add(file.GetString());
                }
                temp = el.GetProperty("Results");
                dep.SavePath = new() { RelativePath = temp.GetProperty("SavePath").GetString() , Type = 
                    (temp.GetProperty("IsDirectory").ValueKind == System.Text.Json.JsonValueKind.True) 
                    ? PathDetailSaveType.Directory : 
                    PathDetailSaveType.File };
                return dep;
            } catch (KeyNotFoundException ex1) {
                throw new MP.ExceptionSystem.InvalidDDepsDownloadFormatException($"The JSON element layout expected to have a specified property, but that is missing." , ex1);
            }
        }

        public DownloadableDependency MainDependency => ParseDependencyFromElement(jelbase.GetProperty("MainDependency"));

        public DownloadableDependency[] SubDependencies
        {
            get {
                if (jelbase.TryGetProperty("SubDependencies" , out var deps) == false) {
                    throw new MP.ExceptionSystem.InvalidDDepsDownloadFormatException("SubDependencies property does not exist. If you do not want sub-dependencies you should use an empty array.");
                }
                if (deps.ValueKind != System.Text.Json.JsonValueKind.Array) {
                    throw new MP.ExceptionSystem.InvalidDDepsDownloadFormatException("SubDependencies property was not an array. If you do not want sub-dependencies you should use an empty array.");
                }
                System.Int32 arrlen = deps.GetArrayLength();
                if (arrlen == 0) { return Array.Empty<DownloadableDependency>(); }
                DownloadableDependency[] data = new DownloadableDependency[arrlen];
                for (System.Int32 I = 0; I < arrlen; I++) 
                {
                    var element = deps[I];
                    if (element.ValueKind != System.Text.Json.JsonValueKind.Object) { 
                        throw new MP.ExceptionSystem.InvalidDDepsDownloadFormatException("SubDependencies property was not an array of sub-dependencies objects."); 
                    }
                    data[I] = ParseDependencyFromElement(element);
                }
                return data;
            }
        }

        public void Dispose() 
        {
            jelbase = default;
            jdt.Dispose();
        }
    }
}