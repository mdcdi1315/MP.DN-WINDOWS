
using System.Collections;
using Microsoft.Build.Utilities;
using Microsoft.Build.Framework;
using DotNetResourcesExtensions;

namespace MusicPlayer.BuildTasks
{
    public sealed class DntResExtResourcesGenerator : Task
    {
        private IDotNetResourcesExtensionsReader readertemp;
        private IDotNetResourcesExtensionsWriter target;
        private ITaskItem[] inputfiles;
        private ITaskItem[] additionalitems;
        private System.String outfilepath, outfilerefsloadpath;
        private ResourceFileType outfiletype;

        public DntResExtResourcesGenerator()
        {
            readertemp = null;
            target = null;
            inputfiles = null;
            additionalitems = null;
            outfilepath = null;
            outfiletype = ResourceFileType.DotNetBinary;
        }

        private System.Boolean CreateTargetInstance()
        {
            try
            {
                switch (outfiletype)
                {
                    case ResourceFileType.DotNetBinary:
                        target = new DotNetResourcesExtensions.Internal.DotNetResources.PreserializedResourceWriter(outfilepath);
                        break;
                    case ResourceFileType.CustomJSON:
                        target = new JSONResourcesWriter(outfilepath);
                        break;
                    case ResourceFileType.CustomXML:
                        target = new XMLResourcesWriter(outfilepath);
                        break;
                }
            } catch (System.IO.DirectoryNotFoundException ex1) {
                LogErrorCommon("Task", "MPRG0001" , "The output path is possibly incomplete. Original message: {0}." , ex1.Message);
                return false;
            } catch (System.Exception ex) {
                LogErrorCommon("Task", "MPRG0002", "Unknown error occured while attempting to create the target object. \nDetails: {0}" , ex);
                return false;
            }
            return true;
        }

        private void CreateReaderInstance(InputResourceFileData dt)
        {
            switch (dt.DeterminedType)
            {
                case ResourceFileType.CustomJSON:
                    readertemp = new JSONResourcesReader(dt.ResourceFilePath);
                    break;
                case ResourceFileType.CustomXML:
                    readertemp = new XMLResourcesReader(dt.ResourceFilePath);
                    break;
            }
        }

        private void TransferInputFiles()
        {
            IDictionaryEnumerator de;
            InputResourceFileData dt;
            foreach (var fe in inputfiles) 
            {
                Log.LogMessage(MessageImportance.Normal, "Opening file {0}..." , fe.ItemSpec);
                dt = new(fe);
                Log.LogMessage(MessageImportance.Low, "Creating reader instance...");
                CreateReaderInstance(dt);
                de = readertemp.GetEnumerator();
                Log.LogMessage(MessageImportance.Low, "Moving resources....");
                while (de.MoveNext()) 
                {
                    target.AddResourceEntry(de.Entry.AsResourceEntry());
                }
                de = null;
                Log.LogMessage(MessageImportance.Low, "Transfer successfull, closing the file.");
                readertemp.Dispose();
                readertemp = null;
            }
        }

        private void TransferFileRefs()
        {
            InputFileReferenceFileData dt;
            InputFileReferenceReader rdr;
            try {
                foreach (var fe in additionalitems)
                {
                    dt = new(outfilerefsloadpath, fe);
                    rdr = new(dt);
                    rdr.ReadAndCreateObject();
                    target.AddResourceEntry(rdr);
                    rdr = null;
                    dt = null;
                }
            } finally {
                dt = null;
                rdr = null;
            }
        }

        private void LogErrorCommon(System.String source , System.String code , System.String msgformat , params System.Object[] arguments)
            => Log.LogError(source , code , "" , "" , "" , 0 , 0 ,0 ,0 , msgformat , messageArgs: arguments);

        public override bool Execute()
        {
            try {
                Log.LogMessage(MessageImportance.Normal, "Initiating Resource Generator...");
                if (System.IO.Directory.Exists(outfilerefsloadpath) == false)
                {
                    LogErrorCommon("Task", "MPRG0000", "The specified file reference loading directory does not exist.\nDirectory: {0}" , outfilerefsloadpath);
                    return false;
                }
                Log.LogMessage(MessageImportance.Normal, "Creating final transform file...");
                if (CreateTargetInstance() == false) {
                    return false;
                }
                Log.LogMessage(MessageImportance.Normal, "Transferring resources from resource files...");
                TransferInputFiles();
                Log.LogMessage(MessageImportance.Normal, "Transferring additional external file references...");
                TransferFileRefs();
                Log.LogMessage(MessageImportance.Normal, "Resource Generator completed!");
            } catch (System.Exception ex) {
                Log.LogError("Parser", "MPRG9999" , "" , "" , "" , 0 , 0 , 0 , 0 , "An exception occured while doing resource generation: {0}" , ex);
                return false;
            } finally {
                target?.Dispose();
                readertemp?.Dispose();
            }
            return true;  
        }

        [Required]
        public ITaskItem[] InputFiles
        {
            get => inputfiles;
            set => inputfiles = value;
        }

        [Required]
        public ITaskItem[] AdditionalFilesToEmbed
        {
            get => additionalitems;
            set => additionalitems = value;
        }

        [Required]
        public System.String OutputFilePath
        {
            get => outfilepath;
            set => outfilepath = value;
        }

        public System.String OutputFileType
        {
            get => outfiletype.ToString();
            set {
                if (value is null) { outfiletype = ResourceFileType.DotNetBinary; }
                outfiletype = value.ToLower() switch {
                    "dotnetbinary" => ResourceFileType.DotNetBinary,
                    "customjson" => ResourceFileType.CustomJSON,
                    "customxml" => ResourceFileType.CustomXML,
                    _ => ResourceFileType.DotNetBinary,
                };
            }
        }

        [Required]
        public System.String FileReferencesLoadingPath
        {
            get => outfilerefsloadpath;
            set => outfilerefsloadpath = value;
        }
    }
}