

using System;

namespace MP.Networking
{
    /// <summary>
    /// Defines bare bones for interoperating with the network.
    /// </summary>
    public abstract class NetworkConnection : INetworkConnection
    {
        protected System.IntPtr ConnectionHandle;
        private NetworkConnectionParameters parameters;

        /// <summary>
        /// Default instance constructor.
        /// </summary>
        protected NetworkConnection() { ConnectionHandle = System.IntPtr.Zero; }

        /// <summary>
        /// Gets the parameters that will be submitted to this network connection. <br />
        /// This can also be a derived instance of <see cref="NetworkConnectionParameters"/> class.
        /// </summary>
        public NetworkConnectionParameters Parameters
        {
            get => parameters;
            protected set => parameters = value;
        }

        /// <summary>
        /// Initiates the network request.
        /// </summary>
        public abstract void Submit();

        /// <summary>
        /// After the request succeeds you can use this stream to get the downloading data.
        /// </summary>
        public abstract System.IO.Stream GetResultStream();

        /// <summary>
        /// Disposes any native data allocated by an implementing class. <br />
        /// Setting <paramref name="disposing"/> to true disposes the managed data too.
        /// </summary>
        /// <param name="disposing">A value whether to dispose and the managed information.</param>
        protected virtual void Dispose(bool disposing) {
            ConnectionHandle = System.IntPtr.Zero;
            if (disposing) { parameters = null; }
        }

        private System.Int64 GetInt64(Interop.WinInet.HttpQueryInfoFlags flgs)
        {
            if (ConnectionHandle == IntPtr.Zero) { throw new InvalidOperationException("The request has not been submitted yet."); }
            System.UInt32 index = 0;
            if (Interop.WinInet.HttpQueryInfo_GetUInt64(ConnectionHandle, flgs, out var nr, ref index) == Interop.BOOL.FALSE)
            {
                throw new MP.ExceptionSystem.NativeWindowsException();
            }
            return nr.ToInt64();
        }

        private System.Int32 GetInt32(Interop.WinInet.HttpQueryInfoFlags flgs)
        {
            if (ConnectionHandle == IntPtr.Zero) { throw new InvalidOperationException("The request has not been submitted yet."); }
            System.UInt32 index = 0;
            if (Interop.WinInet.HttpQueryInfo_GetUInt32(ConnectionHandle, flgs, out var nr, ref index) == Interop.BOOL.FALSE)
            {
                throw new MP.ExceptionSystem.NativeWindowsException();
            }
            return nr.ToInt32();
        }

        private System.DateTime GetDateTime(Interop.WinInet.HttpQueryInfoFlags flgs)
        {
            if (ConnectionHandle == IntPtr.Zero) { throw new InvalidOperationException("The request has not been submitted yet."); }
            System.UInt32 index = 0;
            if (Interop.WinInet.HttpQueryInfo_GetSYSTEMTIME(ConnectionHandle, flgs, out var st, ref index) == Interop.BOOL.FALSE)
            {
                throw new MP.ExceptionSystem.NativeWindowsException();
            }
            return st.ToFileTime().ToDateTimeUtc();
        }

        private System.String GetString(Interop.WinInet.HttpQueryInfoFlags flgs)
        {
            if (ConnectionHandle == IntPtr.Zero) { throw new InvalidOperationException("The request has not been submitted yet."); }
            System.UInt32 index = 0;
            if (Interop.WinInet.HttpQueryInfo_GetString(ConnectionHandle, flgs, out var str, ref index) == Interop.BOOL.FALSE)
            {
                throw new MP.ExceptionSystem.NativeWindowsException();
            }
            return str;
        }

        public System.DateTime Expires => GetDateTime(Interop.WinInet.HttpQueryInfoFlags.HTTP_QUERY_EXPIRES);

        public System.DateTime LastModified => GetDateTime(Interop.WinInet.HttpQueryInfoFlags.HTTP_QUERY_LAST_MODIFIED);

        public System.String ContentType => GetString(Interop.WinInet.HttpQueryInfoFlags.HTTP_QUERY_CONTENT_TYPE);

        public System.Int64 ContentLength => GetInt64(Interop.WinInet.HttpQueryInfoFlags.HTTP_QUERY_CONTENT_LENGTH);

        public System.String ContentEncoding => GetString(Interop.WinInet.HttpQueryInfoFlags.HTTP_QUERY_CONTENT_ENCODING);

        public System.String StatusText => GetString(Interop.WinInet.HttpQueryInfoFlags.HTTP_QUERY_STATUS_TEXT);

        /// <summary>
        /// Disposes the data allocated by an deriving instance of the <see cref="NetworkConnection"/> class.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Default finalizer.
        /// </summary>
        ~NetworkConnection() => Dispose(true);
    }
}