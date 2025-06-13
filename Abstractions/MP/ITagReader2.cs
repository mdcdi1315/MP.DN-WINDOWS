

namespace MP
{
    /// <summary>
    /// Optional <see langword="interface"/> that extends the <see cref="ITagReader"/> interface. <br />
    /// The interface was created because not all tag readers do save all the information that can be found in a tag.
    /// </summary>
    public interface ITagReader2 : ITagReader
    {
        /// <summary>
        /// Gets a URI or a format string that specifies the image format that the <see cref="ITagReader.Image"/> property contains. <br />
        /// If the <see cref="ITagReader.Image"/> property is effectively <see langword="null"/> , then this property will also be <see langword="null"/>.
        /// </summary>
        public System.String ImageFormat { get; }

        /// <summary>
        /// Gets the publishing company of this track. <br />
        /// Returns <see cref="System.String.Empty"/> if this field is not present.
        /// </summary>
        public System.String Publisher { get; }

        /// <summary>
        /// Gets the company's legal copyrights of this track. <br />
        /// Returns <see cref="System.String.Empty"/> if this field is not present.
        /// </summary>
        public System.String Copyright { get; }

        /// <summary>
        /// Gets the track creation date. <br />
        /// Returns <see cref="System.String.Empty"/> if this field is not present.
        /// </summary>
        public System.String CreationDate { get; }
    }

}