

using System;

namespace MP.Networking
{
    /// <summary>
    /// A lightweight abstraction of a network connection.
    /// </summary>
    public interface INetworkConnection : IDisposable
    {
        /// <summary>
        /// Gets the parameters that will be submitted to this network connection. <br />
        /// This can also be a derived instance of <see cref="NetworkConnectionParameters"/> class.
        /// </summary>
        public NetworkConnectionParameters Parameters { get; }

        /// <summary>
        /// Initiates the network request.
        /// </summary>
        public void Submit();

        /// <summary>
        /// After the request succeeds you can use this stream to get the downloading data.
        /// </summary>
        public System.IO.Stream GetResultStream();
    }
}