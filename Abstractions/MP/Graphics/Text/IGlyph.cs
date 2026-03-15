using MP.Graphics.Imaging;

namespace MP.Graphics.Text
{
    /// <summary>
    /// A glyph is a specialized type of image that provides additional information over how to render it in a graphic surface.
    /// </summary>
    public interface IGlyph : IImage
    {
        /// <summary>Gets the metrics for this glyph.</summary>
        public GlyphMetrics Metrics { get; }

        /// <summary>
        /// Gets the Unicode character that this glyph is representing.
        /// </summary>
        public System.Char Character { get; }
    }
}
