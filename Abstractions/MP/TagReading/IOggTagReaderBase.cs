namespace MP.TagReading
{
    /// <summary>
    /// Optional <see langword="interface"/> that extends the <see cref="ITagReader"/> 
    /// interface for the readers that are implementing the Ogg Comment logic.
    /// </summary>
    public interface IOggTagReaderBase : ITagReader
    {
        /// <summary>
        /// Gets the vendor string that was used to produce the tag.
        /// </summary>
        public System.String Vendor { get; }
    }
}