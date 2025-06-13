

namespace MP.Networking
{
    /// <summary>
    /// Selects the port from which the connection to the server will be established through. <br />
    /// The server must be able to communicate on the specified port. <br />
    /// Note that the defined enum values are only default values; you are free to cast any port number to this enum type.
    /// </summary>
    public enum NetworkPort : System.UInt16
    {
        Default = 0,
        Http = Interop.WinInet.INTERNET_DEFAULT_HTTP_PORT,
        Https = Interop.WinInet.INTERNET_DEFAULT_HTTPS_PORT,
        Ftp = Interop.WinInet.INTERNET_DEFAULT_FTP_PORT,
        Socks = Interop.WinInet.INTERNET_DEFAULT_SOCKS_PORT
    }
}