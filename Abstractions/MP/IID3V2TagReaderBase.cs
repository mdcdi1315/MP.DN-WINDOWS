namespace MP
{
    /// <summary>
    /// Optional <see langword="interface"/> that extends the <see cref="ITagReader2"/> interface for ID3V2 audio tags.
    /// </summary>
    public interface IID3V2TagReaderBase : ITagReader2
    {
        /// <summary>
        /// [ID3 Reader Specific] Gets the unsyncronized lyrics for this track that the tag is associated with.
        /// </summary>
        public System.String Lyrics { get; }
    }
}