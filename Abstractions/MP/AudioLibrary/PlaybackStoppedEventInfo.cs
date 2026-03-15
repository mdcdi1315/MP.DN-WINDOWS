

namespace MP.AudioLibrary
{
    /// <summary>
    /// Additional information provided along the <see cref="IAudioPlayer.PlaybackStopped"/> event. <br />
    /// May be extended because an audio driver may have more useful stopped event information. <br />
    /// However, for the users to access such information, they will need to cast to the more derived class.
    /// </summary>
    public class PlaybackStoppedEventInfo
    {
        private PlaybackStoppedReason reason;
        private System.Exception exception;

        /// <summary>
        /// Creates a new instance of the <see cref="PlaybackStoppedEventInfo"/> class defining the 
        /// playback stopped event data.
        /// </summary>
        /// <param name="reason">The reason why this event if firing for.</param>
        /// <param name="exceptionifany">The exception that caused this event to fire.</param>
        public PlaybackStoppedEventInfo(PlaybackStoppedReason reason , System.Exception exceptionifany = null)
        {
            this.reason = reason;
            exception = exceptionifany;
        }

        /// <summary>
        /// Gets the reason why the playback was stopped.
        /// </summary>
        public PlaybackStoppedReason Reason => reason;

        /// <summary>
        /// Gets the exception caused the playback to stop.
        /// </summary>
        public System.Exception Exception => exception;

    }

    /// <summary>
    /// Defines reasons why a <see cref="IAudioPlayer.PlaybackStopped"/> event can fire.
    /// </summary>
    public enum PlaybackStoppedReason : System.Byte
    {
        /// <summary>The end of the audio stream has been reached.</summary>
        EndOfStream,
        /// <summary>The playback was stopped due to the user's request.</summary>
        UserRequest,
        /// <summary>
        /// An exception occured during playback. <br />
        /// Inspect the <see cref="PlaybackStoppedEventInfo.Exception"/> property for more information about the error.
        /// </summary>
        Exception
    }
}