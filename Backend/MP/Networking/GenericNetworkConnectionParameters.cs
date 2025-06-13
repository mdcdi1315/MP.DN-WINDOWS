

namespace MP.Networking
{
    public sealed class GenericNetworkConnectionParameters : NetworkConnectionParameters
    {
        private System.String headers;

        public GenericNetworkConnectionParameters() { }

        public System.String Headers
        {
            get => headers;
            set => headers = value;
        }
    }
}