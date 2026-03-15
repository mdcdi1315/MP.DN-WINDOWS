

using System;

namespace MP.TagReading.ID3
{
    /// <summary>
    /// Provides information for a ID3 V2 Terms Of Use frame.
    /// </summary>
    public readonly struct TermsOfUseFrameData
    {
        /// <summary>
        /// Gets the language under which the terms of use have been written.
        /// </summary>
        public readonly string Language;

        /// <summary>
        /// Gets the actual text of terms of use.
        /// </summary>
        public readonly string Text;

        /// <summary>
        /// Intializes a new instance of the <see cref="TermsOfUseFrameData"/> structure.
        /// </summary>
        /// <param name="language">The language under which the terms of use have been written.</param>
        /// <param name="text">The actual text of terms of use.</param>
        public TermsOfUseFrameData(string language, string text)
        {
            ArgumentNullException.ThrowIfNull(text);
            ArgumentNullException.ThrowIfNull(language);

            Text = text;
            Language = language;
        }
    }
}