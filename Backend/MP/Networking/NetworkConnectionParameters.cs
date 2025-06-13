

namespace MP.Networking
{
    /// <summary>
    /// Common options for a network connection session.
    /// </summary>
    public class NetworkConnectionParameters
    {
        private System.String url, username , password;
        private NetworkPort port;

        public NetworkConnectionParameters() { }

        public System.String URL
        {
            get => url;
            set => url = value;
        }

        public NetworkPort ServerPort
        {
            get => port;
            set => port = value;
        }

        public System.String UserName
        {
            get => username;
            set => username = value;
        }

        public System.String Password
        {
            get => password;
            set => password = value;
        }
    }
}