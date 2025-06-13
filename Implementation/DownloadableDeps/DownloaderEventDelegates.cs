

namespace MP.DownloadableDeps
{
    public delegate void DownloadFailedDelegate(System.Exception reason);

    public delegate void DownloadSucceededDelegate(System.String component);

    public delegate void DownloadReportProgressDelegate(System.Byte newprogress);

    public delegate void DownloadRequestInitiatedDelegate(System.String component);

    public delegate void DownloadStartsDownloadDelegate(System.String component);

    public delegate void DownloadExtractingFilesDelegate(System.Int32 newprogress);
}