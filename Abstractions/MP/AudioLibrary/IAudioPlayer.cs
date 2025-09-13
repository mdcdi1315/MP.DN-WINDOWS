

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
        /// The only case that this should return <see langword="false"/> is when another audio provider has been already attached.
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
        /// Gets or sets the latency of the audio player, that is the time between two I/O requests on the audio provider. <br />
        /// Should not be able to be modified after <see cref="Initialize(IAudioProvider)"/> has been called successfully.
        /// </summary>
        /// <returns>The time that the audio device must wait until the next I/O request is performed.</returns>
        /// <exception cref="InvalidOperationException">Attempted to modify the latency after <see cref="Initialize(IAudioProvider)"/> was called.</exception>
        /// <exception cref="ArgumentOutOfRangeException">New requested latency was negative, while this is not allowed.</exception>
        public System.Int32 Latency { get; set; }

        /// <summary>
        /// Gets the current playback state of the audio device
        /// </summary>
        public PlaybackState State { get; }

        /// <summary>
        /// The event that is fired when a playback session was finished. <br />
        /// It provides to the users additional data to additionally inspect the player's state.
        /// </summary>
        /// <remarks>
        /// <strong>Implementation Notes</strong>: <br />
        /// It is important that this event is fully implemented. <br />
        /// All three reasons are needed so that the player's state can be fully inspected, and corresponding actions can be taken. Consider this example: <br />
        /// Let's say that we a consumer method listening to this event and has fully and transparently implemented all the cases. <br />
        /// But let's also presume that the <see cref="PlaybackStoppedReason.UserRequest"/> is never fired. <br />
        /// If the consumer is managing a playlist and is cycling through the songs (like repeat all mode), <br />
        /// the playlist will move to the next song only when the <see cref="PlaybackStoppedReason.EndOfStream"/> is fired.
        /// </remarks>
        public event PlaybackStoppedDelegate PlaybackStopped;
    }
}