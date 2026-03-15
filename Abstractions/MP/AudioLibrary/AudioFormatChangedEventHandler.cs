
using System.Diagnostics.CodeAnalysis;

namespace MP.AudioLibrary
{
    /// <summary>
    /// Provides the signature for audio format changed events.
    /// </summary>
    /// <param name="format">The new audio format that is enforced. Should not be <see langword="null"/>.</param>
    public delegate void AudioFormatChangedEventHandler([DisallowNull] AudioFormat format);
}