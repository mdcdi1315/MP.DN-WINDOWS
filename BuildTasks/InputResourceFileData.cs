
using System;
using System.IO;
using Microsoft.Build.Framework;

namespace MusicPlayer.BuildTasks
{
    public sealed class InputResourceFileData
    {
        private ResourceFileType infiletype;
        private System.String fullpath , baseloadingpath;

        public InputResourceFileData(ITaskItem item) 
        {
            if (item is null) { throw new ArgumentNullException(nameof(item)); }
            fullpath = item.ItemSpec;
            infiletype = Path.GetExtension(fullpath) switch {
                ".resxx" => ResourceFileType.CustomXML,
                ".resj" => ResourceFileType.CustomJSON,
                _ => throw new ArgumentException("The specified resource file type is not supported. Only Custom XML and Custom JSON are supported.")
            };
            baseloadingpath = item.GetMetadata("BaseResourceLoadingPath");
        }

        public ResourceFileType DeterminedType => infiletype;

        public System.String ResourceFilePath => fullpath;

        public System.String FileReferencesLoadingPath => baseloadingpath;
    }
}