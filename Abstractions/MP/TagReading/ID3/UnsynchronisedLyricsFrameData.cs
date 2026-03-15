
using System;

namespace MP.TagReading.ID3
{
    /// <summary>
    /// Defines data for an unsyncronised lyrics data frame.
    /// </summary>
    public readonly struct UnsynchronisedLyricsFrameData
    {
        /// <summary>
        /// Gets the value, the lyrics, of the frame data.
        /// </summary>
        public readonly string Value;

        /// <summary>
        /// Gets the language that the lyrics are written into.
        /// </summary>
        public readonly string Language;

        /// <summary>
        /// Gets the content descriptor for this lyrics frame.
        /// </summary>
        public readonly string ContentDescriptor;

        /// <summary>
        /// Intializes a new instance of the <see cref="UnsynchronisedLyricsFrameData"/> structure.
        /// </summary>
        /// <param name="value">The text value (lyrics) of the frame data.</param>
        /// <param name="language">The lyrics language of the frame data.</param>
        /// <param name="cd">The content descriptor of the frame data.</param>
        public UnsynchronisedLyricsFrameData(string language, string value, string cd)
        {
            ArgumentNullException.ThrowIfNull(value);
            ArgumentNullException.ThrowIfNull(language);

            Value = value;
            Language = language;
            ContentDescriptor = cd;
        }
    }
}