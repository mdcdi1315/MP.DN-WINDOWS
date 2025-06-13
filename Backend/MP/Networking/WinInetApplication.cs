

using System;

namespace MP.Networking
{
    /// <summary>
    /// Defines a bare class so that you can safely make use of WinInet's services.
    /// </summary>
    public sealed class WinInetApplication : IDisposable
    {
        private System.IntPtr openinternethandle;

        /// <summary>
        /// Creates a new instance of the <see cref="WinInetApplication"/> class, with the specified app name (aka user agent). <br />
        /// Note that the user agent string can be null , which in such case it will get a default string.
        /// </summary>
        /// <param name="useragent">The user agent string to use.</param>
        /// <exception cref="MP.ExceptionSystem.NativeWindowsException">Initialization failed.</exception>
        public WinInetApplication(System.String useragent) 
        {
            if (useragent is null)
            {
                useragent = $"{nameof(MP)}.{nameof(Networking)}.{nameof(WinInetApplication)}";
            }
            openinternethandle = Interop.WinInet.InternetOpen(useragent, Interop.WinInet.InternetAccessType.INTERNET_OPEN_TYPE_DIRECT, null, null, Interop.WinInet.InternetOpenFlags.None);
            if (openinternethandle == IntPtr.Zero) {
                throw new MP.ExceptionSystem.NativeWindowsException();
            }
        }

        /// <summary>
        /// Creates a new HTTP network connection and returns it as an instance of the <see cref="HttpNetworkConnection"/> class.
        /// </summary>
        /// <param name="parameters">The network connection parameters to initialize the request.</param>
        /// <returns>A new HTTP request connection object.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="parameters"/> was null.</exception>
        public HttpNetworkConnection CreateNewHttpConnection(HttpNetworkParameters parameters)
        {
            if (parameters is null) { throw new ArgumentNullException(nameof(parameters)); }
            return new(parameters, openinternethandle);
        }

        /// <summary>
        /// Creates a new generic network connection and returns it as an instance of the <see cref="GenericNetworkConnection"/> class.
        /// </summary>
        /// <param name="parameters">The network connection parameters to initialize the request.</param>
        /// <returns>A new connection request object.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="parameters"/> was null.</exception>
        public GenericNetworkConnection CreateNewGenericConnection(GenericNetworkConnectionParameters parameters)
        {
            if (parameters is null) { throw new ArgumentNullException(nameof(parameters)); }
            return new(parameters, openinternethandle);
        }

        /// <summary>
        /// Disposes the current instance. <br />
        /// May throw a native exception , and thus you must observe it.
        /// </summary>
        /// <exception cref="MP.ExceptionSystem.NativeWindowsException">An error occured while terminating WinInet.</exception>
        public void Dispose() 
        {
            if (openinternethandle == IntPtr.Zero) { return; }
            if (Interop.WinInet.InternetCloseHandle(openinternethandle) == Interop.BOOL.FALSE)
            {
                throw new MP.ExceptionSystem.NativeWindowsException();
            }
            openinternethandle = IntPtr.Zero;
        }
    }
}