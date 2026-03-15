

namespace MP.AudioLibrary
{
    /// <summary>
    /// Documents common audio stream formats. <br />
    /// May more be added and documented in the future
    /// </summary>
    public enum CommonAudioFormat : System.UInt16
    {
        /// <summary>
        /// This is possibly not a common audio format. <br />
        /// Use this when a very custom audio format is specified by the codec
        /// </summary>
        Unknown = 0,
        /// <summary>
        /// This is an audio format based on the Pulse Code Modulated common format.
        /// </summary>
        PCM,
        /// <summary>
        /// This is an audio format that it's native representation of audio data is into IEEE floating numbers.
        /// </summary>
        IEEEFloat
    }
}