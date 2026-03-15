

using System;
using MP.AudioLibrary;

namespace MP
{
    /// <summary>
    /// Defines an abstraction of a 'player instance'. <br />
    /// A player instance is a single playback of a single source in a given time.
    /// </summary>
    public abstract class AbstractPlayerInstance : IDisposable
    {
        private static void RepeatRequestSuccessfullDummyFunction(System.Object obj , EventArgs e) { }

        private static void PlaybackStoppedDummyFunction(PlaybackStoppedEventInfo info) { }

        /// <summary>
        /// Creates an empty instance of the <see cref="AbstractPlayerInstance"/> class.
        /// </summary>
        protected AbstractPlayerInstance() : base()
        {
            RepeatRequestSuccessfull = new(RepeatRequestSuccessfullDummyFunction);
            PlaybackStopped = new(PlaybackStoppedDummyFunction);
        }

        /// <summary>
        /// Fired when <see cref="RepeatEnabled"/> property is <see langword="true"/> and a playback stop request had been fired with no errors.
        /// </summary>
        public event EventHandler RepeatRequestSuccessfull;

        /// <summary>
        /// Fired when the instance has finished playing,
        /// or whence an exception has been occured on the playback thread.
        /// </summary>
        public event PlaybackStoppedDelegate PlaybackStopped;

        /// <summary>
        /// Fires the <see cref="RepeatRequestSuccessfull"/> event.
        /// </summary>
        protected void OnRepeatRequestSuccessfull() => RepeatRequestSuccessfull?.Invoke(this, EventArgs.Empty);

        /// <summary>
        /// Fires the <see cref="PlaybackStopped"/> event.
        /// </summary>
        /// <param name="evs">The stop event arguments to additionally pass.</param>
        protected void OnPlaybackStopped(PlaybackStoppedEventInfo evs) => PlaybackStopped?.Invoke(evs);

        /// <summary>
        /// Gets the initial player creation information that created this instance.
        /// </summary>
        public abstract IPlayerInstanceData InitialCreationInfo { get; }

        /// <summary>
        /// Gets the current instance playback state.
        /// </summary>
        public abstract PlaybackState CurrentState { get; }

        /// <summary>
        /// Gets or sets the current time point that the player has reached. <br />
        /// When setting this value , it means that you skip the track to the specified time.
        /// </summary>
        public abstract TimeSpan CurrentReachedTime { get; set; }

        /// <summary>
        /// Gets the total time of this track.
        /// </summary>
        public abstract TimeSpan TotalTrackTime { get; }

        /// <summary>
        /// Gets the remaining life time of this track , whether it is playing or not.
        /// </summary>
        public virtual TimeSpan RemainingTime => TotalTrackTime.Subtract(CurrentReachedTime);

        /// <summary>
        /// Gets the count of channels contained in this track.
        /// </summary>
        public abstract System.Int32 Channels { get; }

        /// <summary>
        /// Gets the count of samples that are 'played' in a second.
        /// </summary>
        public abstract System.Int32 SamplesPerSecond { get; }

        /// <summary>
        /// Gets the track bitrate. The value is counted in bits on a track sample , 
        /// which if it is multiplied with <see cref="SamplesPerSecond"/> property , 
        /// will give you the total bits in a second.
        /// </summary>
        public abstract System.Int32 BitsPerSample { get; }

        /// <summary>Starts the playback , or if the player has been paused it continues from the <see cref="CurrentReachedTime"/>.</summary>
        /// <exception cref="InvalidOperationException">The player instance has not been created using the <see cref="Create"/> method.</exception>
        public abstract void Play();

        /// <summary>
        /// Stops playback and sets the elapsed playback time to zero. <br />
        /// If in any case any event is attached and this method is called with the <paramref name="forcestop"/>
        /// parameter to <see langword="true"/> , then it will generate a special playback stop event, even overriding the repeat mode.
        /// </summary>
        /// <param name="forcestop">A value whether to fully stop playback. By default, even when Repeat mode is enabled, playback must begin from the start.</param>
        /// <exception cref="InvalidOperationException">The player instance has not been created using the <see cref="Create"/> method.</exception>
        public abstract void Stop(System.Boolean forcestop = false);

        /// <summary>Pauses playback.</summary>
        /// <exception cref="InvalidOperationException">The player instance has not been created using the <see cref="Create"/> method.</exception>
        public abstract void Pause();

        /// <summary>
        /// Sets the elapsed time ten seconds ahead.
        /// </summary>
        public abstract void TenSecondsAhead();

        /// <summary>
        /// Sets the elapsed time ten seconds back.
        /// </summary>
        public abstract void TenSecondsBehind();

        /// <summary>
        /// Adjusts the volume on the left speaker. <br />
        /// The acceptable range is from 0 (No Sound) to 100 (Max volume).
        /// </summary>
        public abstract System.Byte LeftSpeakerVolume { get; set; }

        /// <summary>
        /// Adjusts the volume on the right speaker. <br />
        /// The acceptable range is from 0 (No Sound) to 100 (Max volume).
        /// </summary>
        public abstract System.Byte RightSpeakerVolume { get; set; }

        /// <summary>
        /// Gets a value whether Repeat Mode is enabled.
        /// </summary>
        public abstract System.Boolean RepeatEnabled { get; }

        /// <summary>
        /// Enables or disables Repeat Mode. The action performed is based on these returned values:
        /// <list type="bullet">
        /// <item>
        /// When the return value is <see langword="true"/>: <br />
        /// It means that the method enabled Repeat Mode.
        /// </item>
        /// <item>
        /// When the return value is <see langword="false"/>: <br />
        /// It means that the method disabled Repeat Mode.
        /// </item>
        /// </list> <br />
        /// You may also query the Repeat Mode state by reading the <see cref="RepeatEnabled"/> property.
        /// </summary>
        /// <returns>A value whether Repeat Mode is enabled or not.</returns>
        public abstract System.Boolean Repeat();

        /// <summary>
        /// Creates the player instance. <br />
        /// The instance must be created before it is able to play! <br />
        /// For safety and robustness , this method can be called as many times as long as the first creation succeeds.
        /// </summary>
        public abstract void Create();

        /// <summary>
        /// Disposes all the data used by this player instance. <br />
        /// Called by the <see cref="Dispose"/> method.
        /// </summary>
        protected abstract void DestroyInstance();

        /// <summary>
        /// Disposes the current instance , ensuring that all events have been properly detached.
        /// </summary>
        public void Dispose()
        {
            DestroyInstance();
            if (PlaybackStopped is not null)
            {
                foreach (var dlg in PlaybackStopped.GetInvocationList())
                {
                    Delegate.Remove(PlaybackStopped, dlg);
                }
                PlaybackStopped = null;
            }
            if (RepeatRequestSuccessfull is not null) 
            {
                foreach (var dlg in RepeatRequestSuccessfull.GetInvocationList())
                {
                    Delegate.Remove(RepeatRequestSuccessfull, dlg);
                }
                RepeatRequestSuccessfull = null;
            }
            GC.SuppressFinalize(this);
        }
    }
}