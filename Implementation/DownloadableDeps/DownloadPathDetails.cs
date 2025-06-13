namespace MP.DownloadableDeps
{
    public enum PathDetailSaveType : System.Byte
    {
        /// <summary>
        /// The downloaded data must be saved to a file. It's name is specified by the path.
        /// </summary>
        File = 0,
        /// <summary>
        /// All the downloaded data should be extracted to the specified directory , if this is a zip download
        /// </summary>
        Directory
    }


    public sealed class DownloadPathDetails
    {
        private System.String relpathunder;
        private PathDetailSaveType savetype;

        public DownloadPathDetails() { }

        public System.String RelativePath
        {
            get => relpathunder;
            set => relpathunder = value;
        }

        public PathDetailSaveType Type
        {
            get => savetype;
            set => savetype = value;
        }
    }
}
