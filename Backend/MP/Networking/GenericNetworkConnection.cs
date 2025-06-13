
using System;
using System.IO;

namespace MP.Networking
{
    public sealed class GenericNetworkConnection : NetworkConnection
    {
        private System.IntPtr internetopenhandle;

        internal GenericNetworkConnection(GenericNetworkConnectionParameters parameters, System.IntPtr openhandle) : base()
        {
            Parameters = parameters;
            internetopenhandle = openhandle;
        }

        public override Stream GetResultStream()
        {
            if (internetopenhandle == IntPtr.Zero) { throw new ObjectDisposedException(nameof(GenericNetworkConnection)); }
            if (ConnectionHandle == IntPtr.Zero) { throw new InvalidOperationException("The request has not been submitted yet."); }
            System.Int64 len = -1;
            try { len = ContentLength; } catch (MP.ExceptionSystem.NativeWindowsException) { }
            return new WinInetNetworkStream(ConnectionHandle , len);
        }

        public override void Submit()
        {
            if (internetopenhandle == IntPtr.Zero) { throw new ObjectDisposedException(nameof(GenericNetworkConnection)); }
            if (ConnectionHandle != IntPtr.Zero) { return; }
            GenericNetworkConnectionParameters ps = Parameters as GenericNetworkConnectionParameters;
            ConnectionHandle = Interop.WinInet.InternetOpenUrl(internetopenhandle, ps.URL , ps.Headers , Interop.WinInet.InternetConnectFlags.INTERNET_FLAG_SECURE);
            if (ConnectionHandle == IntPtr.Zero) {
                Interop.WinInet.ThrowAppropriateException();
            }
        }

        protected override void Dispose(bool disposing)
        {
            internetopenhandle = System.IntPtr.Zero;
            Interop.WinInet.InternetCloseHandle(ConnectionHandle);
            base.Dispose(disposing);
        }
    }
}