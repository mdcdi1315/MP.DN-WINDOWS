

namespace MP.AudioLibrary
{
    /// <summary>
    /// The method implementation required, so that <see cref="IAudioPlayer"/> users can listen to stopped playback events.
    /// </summary>
    /// <param name="eventinfo">Additional event data describing the reason why this event was fired for.</param>
    public delegate void PlaybackStoppedDelegate(PlaybackStoppedEventInfo eventinfo);
}