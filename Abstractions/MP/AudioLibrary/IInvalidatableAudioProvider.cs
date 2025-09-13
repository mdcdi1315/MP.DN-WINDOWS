namespace MP.AudioLibrary
{
    /// <summary>
    /// A special derivant of the <see cref="IAudioProvider"/> interface that allows to invalidate the state of the audio provider <br />
    /// This is usually useful in the terms of invalidating the buffers of an resampler, for example.
    /// </summary>
    public interface IInvalidatableAudioProvider : IAudioProvider
    {
        /// <summary>
        /// Invalidate the state of the audio provider
        /// </summary>
        void Invalidate();
    }
}
