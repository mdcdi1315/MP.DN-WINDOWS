

using System;
using System.IO;

namespace MP.Networking
{
    public sealed class HttpNetworkConnection : NetworkConnection
    {
        private System.IntPtr internetopenhandle, internetconnecthandle;

        internal HttpNetworkConnection(HttpNetworkParameters parameters , System.IntPtr openhandle) : base()
        {
            Parameters = parameters;
            internetopenhandle = openhandle;
        }

        private HttpNetworkParameters ValidateParameters(NetworkConnectionParameters prs)
        {
            HttpNetworkParameters ret = prs as HttpNetworkParameters;
            if (System.String.IsNullOrEmpty(ret.Verb))
            {
                throw new System.ArgumentException("The HTTP verb must not be an empty string.");
            }
            if (System.String.IsNullOrEmpty(ret.URL))
            {
                throw new System.ArgumentException("The URL string must not be the empty string.");
            }
            return ret;
        }

        public override void Submit()
        {
            HttpNetworkParameters prs = ValidateParameters(Parameters);
            internetconnecthandle = Interop.WinInet.InternetConnect(internetopenhandle, prs.URL, (System.UInt16)prs.ServerPort, prs.UserName, prs.Password, Interop.WinInet.InternetConnectService.INTERNET_SERVICE_HTTP, Interop.WinInet.InternetConnectFlags.INTERNET_FLAG_IGNORE_REDIRECT_TO_HTTP | Interop.WinInet.InternetConnectFlags.INTERNET_FLAG_NO_UI);
            if (internetconnecthandle == IntPtr.Zero) {
                Interop.WinInet.ThrowAppropriateException();
            }
            ConnectionHandle = Interop.WinInet.HttpOpenRequest(internetopenhandle, prs.Verb, prs.Target, prs.HttpVersionString, prs.Referrer, prs.AcceptedMimeTypes, Interop.WinInet.InternetConnectFlags.None);
            if (ConnectionHandle == IntPtr.Zero)
            {
                System.Int32 error = Interop.Kernel32.GetLastError(); // Preserve last error
                // Close handles
                Interop.WinInet.InternetCloseHandle(internetconnecthandle);
                internetconnecthandle = IntPtr.Zero;
                // Throw the error now
                Interop.WinInet.ThrowAppropriateException(error);
            }
            if (Interop.WinInet.HttpSendRequest(ConnectionHandle, prs.Headers) == Interop.BOOL.FALSE)
            {
                // Command failed , so do these tasks:
                System.Int32 error = Interop.Kernel32.GetLastError(); // Preserve last error
                // Close handles
                Interop.WinInet.InternetCloseHandle(internetconnecthandle);
                Interop.WinInet.InternetCloseHandle(ConnectionHandle);
                ConnectionHandle = IntPtr.Zero;
                internetconnecthandle = IntPtr.Zero;
                // Throw the error now
                Interop.WinInet.ThrowAppropriateException(error);
            }
        }

        public override Stream GetResultStream()
        {
            if (internetopenhandle == IntPtr.Zero) { throw new ObjectDisposedException(nameof(HttpNetworkConnection)); }
            if (ConnectionHandle == IntPtr.Zero) { throw new InvalidOperationException("The request has not been submitted yet."); }
            return new WinInetNetworkStream(ConnectionHandle);
        }

        protected override void Dispose(bool disposing)
        {
            Interop.WinInet.InternetCloseHandle(ConnectionHandle);
            Interop.WinInet.InternetCloseHandle(internetconnecthandle);
            internetopenhandle = IntPtr.Zero; // This is not managed by this class
            base.Dispose(disposing);
        }
    }
}