

namespace MP.Networking
{
    /// <summary>
    /// Provides networking features to the Music Player App.
    /// </summary>
    public static class NetworkProvider
    {
        /// <summary>
        /// Asserts that an Internet Connection can be established. <br />
        /// It throws a native exception in case that the PC cannot connect to the Internet , so you must always catch this exception.
        /// </summary>
        /// <exception cref="MP.ExceptionSystem.NativeWindowsException">The exception thrown on connection failure. It's message property returns a detailed error message.</exception>
        public static void VerifyConnection()
        {
            System.UInt32 err = Interop.WinInet.InternetAttemptConnect();
            if (err != Interop.Errors.ERROR_SUCCESS) { throw new MP.ExceptionSystem.NativeWindowsException(err.ToInt32()); }
        }

        /// <summary>
        /// Creates a new network instance.
        /// </summary>
        /// <returns>A new network instance.</returns>
        public static WinInetApplication CreateNetworkInstance() => CreateNetworkInstance(null);

        /// <summary>
        /// Creates a new network instance.
        /// </summary>
        /// <param name="ua">The user-agent aka the app name to be shown on the requests.</param>
        /// <returns>A new network instance.</returns>
        public static WinInetApplication CreateNetworkInstance(System.String ua = null) => new(ua);
    }
}