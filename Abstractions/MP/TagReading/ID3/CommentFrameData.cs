
using System;

namespace MP.TagReading.ID3
{
    /// <summary>
    /// Provides information for an ID3 V2 comment frame.
    /// </summary>
    public readonly struct CommentFrameData
    {
        /// <summary>
        /// Gets the actual text of this comment frame.
        /// </summary>
        public readonly string Text;

        /// <summary>
        /// Gets the language under which the text was written.
        /// </summary>
        public readonly string Language;

        /// <summary>
        /// Gets the content description for this comment frame.
        /// </summary>
        public readonly string ContentDescription;

        /// <summary>
        /// Intializes a new instance of the <see cref="CommentFrameData"/> structure.
        /// </summary>
        /// <param name="text">The actual raw text of this comment.</param>
        /// <param name="language">The language under which <paramref name="text"/> was written.</param>
        /// <param name="content_description">The content description of this comment.</param>
        public CommentFrameData(string text, string language, string content_description)
        {
            ArgumentNullException.ThrowIfNull(text);
            ArgumentNullException.ThrowIfNull(language);
            ArgumentNullException.ThrowIfNull(content_description);

            Text = text;
            Language = language;
            ContentDescription = content_description;
        }
    }
}