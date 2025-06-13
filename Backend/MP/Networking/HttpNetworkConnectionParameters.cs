

using System.Collections.Generic;

namespace MP.Networking
{
    /// <summary>
    /// Extends the network connection parameters for specific HTTP connections.
    /// </summary>
    public sealed class HttpNetworkParameters : NetworkConnectionParameters
    {
        private List<System.String> acceptmimetypes;
        private System.String verb, target, httpversion , referrer, headers; 

        public HttpNetworkParameters() {
            acceptmimetypes = new();
            verb = "GET";
            target = null;
            referrer = null;
            headers = null;
            httpversion = "HTTP/1.0";
        }

        public System.String Verb
        {
            get => verb;
            set => verb = value;
        }

        public System.String Target
        {
            get => target;
            set => target = value;
        }

        public System.String Referrer
        {
            get => referrer;
            set => referrer = value;
        }

        public System.String Headers
        {
            get => headers;
            set => headers = value;
        }

        public HttpVersion Version
        {
            get => httpversion == "HTTP/1.0" ? HttpVersion.Version1 : HttpVersion.Version1_1;
            set => httpversion = value switch {
                HttpVersion.Version1 => "HTTP/1.0",
                HttpVersion.Version1_1 => "HTTP/1.1",
                _ => throw new System.ArgumentException("Only Version 1 and 1.1 of the HTTP protocol is accepted."),
            };
        }

        public System.String HttpVersionString => httpversion;

        public void AddAcceptedMimeType(System.String mimeType) 
        {
            if (System.String.IsNullOrEmpty(mimeType)) { throw new System.ArgumentNullException(nameof(mimeType)); }
            acceptmimetypes.Add(mimeType);
        }

        public System.Boolean RemoveAcceptedMimeType(System.String mimeType) => acceptmimetypes.Remove(mimeType);

        public System.String[] AcceptedMimeTypes => acceptmimetypes.ToArray();
    }
}