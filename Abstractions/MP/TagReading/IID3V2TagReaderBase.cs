using MP.TagReading.ID3;

namespace MP.TagReading
{
    /// <summary>
    /// Optional <see langword="interface"/> that extends the <see cref="ITagReader"/> interface for ID3V2 audio tags.
    /// </summary>
    public interface IID3V2TagReaderBase : ITagReader
    {
        /// <summary>
        /// [ID3 Reader Specific] Gets the unsyncronized lyrics for this track that the tag is associated with.
        /// </summary>
        /// <param name="lyrics">On return, this contains the unsyncronized lyrics data if the return value is <see langword="true"/>.</param>
        /// <returns>A value whether the unsyncronized lyrics exist for this tag and were successfully retrieved.</returns>
        public bool TryGetLyrics(out UnsynchronisedLyricsFrameData lyrics);

        /// <summary>
        /// [ID3 Reader Specific] Gets any additional detailed information about the comment contained in this tag.
        /// </summary>
        /// <param name="comment">On return, this contains the comment data if the return value is <see langword="true"/>.</param>
        /// <returns>A value whether the comment exist for this tag and was successfully retrieved.</returns>
        public bool TryGetComment(out CommentFrameData comment);
    }
}