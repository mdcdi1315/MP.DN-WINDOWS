using System.Collections.Generic;

namespace MP.DownloadableDeps
{
    public sealed class DownloadableDependency
    {
        private bool zipfile;
        private DownloadPathDetails savepathdetails;
        private List<string> importantfiles, inclusionrules;
        private string name, desc, downloadurl;

        public DownloadableDependency(string name, string desc, string downloadurl, bool iszipdownload)
        {
            this.name = name;
            this.desc = desc;
            this.downloadurl = downloadurl;
            importantfiles = new();
            inclusionrules = new();
            savepathdetails = new();
            zipfile = iszipdownload;
        }

        public string Name => name;

        public string Description => desc;

        public bool IsZipFile => zipfile;

        public string DownloadURL => downloadurl;

        public DownloadPathDetails SavePath
        {
            get => savepathdetails;
            set => savepathdetails = value;
        }

        public IList<string> InclusionRules => inclusionrules;

        public IList<string> ImportantFileList => importantfiles;

    }
}