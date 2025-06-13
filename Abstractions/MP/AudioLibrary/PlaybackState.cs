

namespace MP.AudioLibrary
{
    /// <summary>
    /// Defines common status codes that an <see cref="IAudioPlayer"/>-implementing instance can be into.
    /// </summary>
    public enum PlaybackState : System.Byte
    {
        /// <summary>Data are not being rendered into the target audio device. Temporary buffers are now flushed.</summary>
        Stopped,
        /// <summary>Data are being rendered into the target audio device. When needed, a <see cref="IAudioProvider.Read"/> call is performed.</summary>
        Playing,
        /// <summary>Data are not being rendered into the target audio device. The temporary buffers are not yet flushed.</summary>
        Paused
    }
}