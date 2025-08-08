

using MP.Utilities;

namespace MP.TagReading.Ogg
{
    /// <summary>
    /// Defines a key-value pair of a single entry for Ogg Comments.
    /// </summary>
    public struct OggCommentEntry : INullable
    {
        /// <summary>
        /// The key referencing the value of the comment.
        /// </summary>
        public System.String Key;

        /// <summary>
        /// The value of the current Ogg comment.
        /// </summary>
        public System.String Value;

        /// <summary>
        /// Gets a value whether this entry is invalid.
        /// </summary>
        public System.Boolean IsNull => Key is null;
    }

}