namespace MP.AudioLibrary.MediaFoundation
{
    /// <summary>
    /// This exception class should be used where the <see cref="MediaFoundationErrorCodes.MF_E_SHUTDOWN"/> error code is shown up.
    /// </summary>
    public sealed class MediaFoundationIsShutdownException : MediaFoundationException
    {
        /// <summary>
        /// Creates a new instance <see cref="MediaFoundationIsShutdownException"/> class, providing the required message to the user code.
        /// </summary>
        public MediaFoundationIsShutdownException() : base("Media Foundation is shut down, while it is required to have been initialized.") {}
    }
}
