

namespace MP
{
    /// <summary>
    /// Optional <see langword="interface"/> that extends the <see cref="ITagReader2"/> 
    /// interface for the readers that are implementing the Ogg Comment logic.
    /// </summary>
    public interface IOggTagReaderBase : ITagReader2
    {
        /// <summary>
        /// Gets the vendor string that was used to produce the tag.
        /// </summary>
        public System.String Vendor { get; }
    }
}