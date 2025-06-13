
using System;
using Microsoft.Build.Framework;
using DotNetResourcesExtensions;

namespace MusicPlayer.BuildTasks
{
    public sealed class InputFileReferenceFileData : IFileReference
    {
        private FileReferenceStandardType standard;
        private FileReferenceEncoding filerefenc;
        private System.String filepathfinal , cmt , ennname;

        public InputFileReferenceFileData(System.String basepath , ITaskItem item)
        {
            if (System.String.IsNullOrEmpty(basepath)) { throw new ArgumentNullException(nameof(basepath)); }
            if (item is null) { throw new ArgumentNullException(nameof(item)); }
            filepathfinal = System.IO.Path.Combine(basepath, item.ItemSpec);
            if (System.IO.File.Exists(filepathfinal) == false)
            {
                throw new System.IO.FileNotFoundException("The given file does not exist." , filepathfinal);
            }
            ennname = item.GetMetadata("Name");
            if (System.String.IsNullOrWhiteSpace(ennname)) { throw new ArgumentException("The Name metadata item is not defined.", nameof(item)); }
            cmt = item.GetMetadata("Comment");
            System.String temp = item.GetMetadata("SaveType");
            if (temp is null)
            {
                throw new ArgumentException("The SaveType metadata item must exist." , nameof(item));
            }
            standard = temp.ToLower() switch { 
                "string" => FileReferenceStandardType.String,
                "bytearray" or "file" => FileReferenceStandardType.ByteArray,
                _ => FileReferenceStandardType.None,
            };
            if (standard == FileReferenceStandardType.String) 
            {
                temp = item.GetMetadata("StringEncoding");
                if (temp is null) {
                    throw new ArgumentException("The StringEncoding metadata item must exist, since the save type is a string.", nameof(item));
                }
                filerefenc = temp.ToLower() switch { 
                    "bin" => FileReferenceEncoding.Binary,
                    "utf-8" => FileReferenceEncoding.UTF8,
                    "utf-16" => FileReferenceEncoding.UTF16LE,
                    "utf-16be" => FileReferenceEncoding.UTF16BE,
                    "utf-32" => FileReferenceEncoding.UTF32LE,
                    "utf-32be" => FileReferenceEncoding.UTF32BE,
                    _ => FileReferenceEncoding.Binary
                };
            }
        }

        public System.String TransformedEntryName => ennname;

        public string FileName => filepathfinal;

        public Type SavingType => standard switch { 
            FileReferenceStandardType.String => typeof(System.String) , 
            FileReferenceStandardType.ByteArray => typeof(System.Byte[]),
            _ => null
        };

        public FileReferenceStandardType StandardType => standard;

        public FileReferenceEncoding Encoding => filerefenc;

        public System.String Comment => cmt;
    }
}