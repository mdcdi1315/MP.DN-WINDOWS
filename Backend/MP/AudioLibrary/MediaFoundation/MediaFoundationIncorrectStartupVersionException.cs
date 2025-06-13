

namespace MP.AudioLibrary.MediaFoundation
{
    /// <summary>
    /// Thrown in place of the <see cref="MediaFoundationErrorCodes.MF_E_BAD_STARTUP_VERSION"/> error code.
    /// </summary>
    public sealed class MediaFoundationIncorrectStartupVersionException : MediaFoundationException
    {
        private System.UInt32 version;

        public MediaFoundationIncorrectStartupVersionException(System.UInt32 version)
            : base($"Attempted to create Media Foundation with a version older than the version that Media Foundation is running.\nVersion code specified: {version}")
            => this.version = version;

        public System.UInt32 Version => version;
    }
}