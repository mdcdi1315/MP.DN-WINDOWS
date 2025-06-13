

using System;

namespace MP.AudioLibrary
{
    /// <summary>
    /// Classes that implement <see cref="IAudioPlayer"/> are usually classes that are the audio device drivers of the system. <br />
    /// All the classes implementing this are at least expected to have all the members described with this interface.
    /// </summary>
    public interface IAudioPlayer : IDisposable
    {
        /// <summary>
        /// Initializes the current audio player instance with the specified audio provider to play audio data from.
        /// </summary>
        /// <param name="provider">The audio provider to associate</param>
        /// <returns>
        /// A value whether the audio provider was succesfully attached to the instance. <br />
        /// The only case that this should return <see langword="false"/> is when an another audio provider has been already attached.
        /// </returns>
        /// <exception cref="ArgumentNullException"><paramref name="provider"/> was <see langword="null"/>.</exception>
        public System.Boolean Initialize(IAudioProvider provider);

        /// <summary>
        /// If the playback has not begun, this method starts playback by creating a new playback thread. <br />
        /// Otherwise it should do nothing.
        /// </summary>
        public void Play();

        /// <summary>
        /// Playback thread remains active but it is blocked until again continued by the user.
        /// </summary>
        public void Pause();

        /// <summary>
        /// All the temporary buffers are now flushed. <br />
        /// Playback may need partial initialization on the next <see cref="Play"/> call.
        /// </summary>
        public void Stop();

        /// <summary>
        /// Gets the current playback state of the audio device
        /// </summary>
        public PlaybackState State { get; }

        /// <summary>
        /// The event that is fired when a playback session was finished. <br />
        /// It provides to the users additional data to additionally inspect the player's state.
        /// </summary>
        public event PlaybackStoppedDelegate PlaybackStopped;
    }
}