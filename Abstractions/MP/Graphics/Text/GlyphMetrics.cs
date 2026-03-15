

namespace MP.Graphics.Text
{
    /// <summary>
    /// Provides the metrics data for a given textual glyph.
    /// </summary>
    public readonly struct GlyphMetrics
    {
        /// <summary>The left-side bearing. In FreeType, this corressponds to the <c>horiBearingX</c> value in the <see href="https://freetype.org/freetype2/docs/reference/ft2-glyph_retrieval.html#ft_glyph_metrics">FT_Glyph_Metrics</see> structure.</summary>
        public readonly int LeftSideBearing;

        /// <summary>The right-side bearing. In FreeType, this corressponds to the <c>horiBearingY</c> value in the <see href="https://freetype.org/freetype2/docs/reference/ft2-glyph_retrieval.html#ft_glyph_metrics">FT_Glyph_Metrics</see> structure.</summary>
        public readonly int RightSideBearing;

        /// <summary>The horizontal advance. In FreeType, this corressponds to the <c>horiAdvance</c> value in the <see href="https://freetype.org/freetype2/docs/reference/ft2-glyph_retrieval.html#ft_glyph_metrics">FT_Glyph_Metrics</see> structure.</summary>
        public readonly int AdvanceWidth;

        /// <summary>The top-side bearing for vertical layout. In FreeType, this corressponds to the <c>vertBearingY</c> value in the <see href="https://freetype.org/freetype2/docs/reference/ft2-glyph_retrieval.html#ft_glyph_metrics">FT_Glyph_Metrics</see> structure.</summary>
        public readonly int TopSideBearing;

        /// <summary>The bottom-side bearing.</summary>
        public readonly int BottomSideBearing;

        /// <summary>The vertical advance. In FreeType, this corressponds to the <c>vertAdvance</c> value in the <see href="https://freetype.org/freetype2/docs/reference/ft2-glyph_retrieval.html#ft_glyph_metrics">FT_Glyph_Metrics</see> structure.</summary>
        public readonly int AdvanceHeight;

        /// <summary>Initializes the fields of the <see cref="GlyphMetrics"/> structure.</summary>
        /// <param name="left_bearing">The left side bearing.</param>
        /// <param name="right_bearing">The right side bearing.</param>
        /// <param name="wd_advance">The advance width.</param>
        /// <param name="top_bearing">The top side bearing.</param>
        /// <param name="bottom_bearing">The bottom side bearing.</param>
        /// <param name="advance_height">The advance height.</param>
        public GlyphMetrics(int left_bearing, int right_bearing, int wd_advance, int top_bearing, int bottom_bearing, int advance_height)
        {
            AdvanceWidth = wd_advance;
            TopSideBearing = top_bearing;
            LeftSideBearing = left_bearing;
            RightSideBearing = right_bearing;
            AdvanceHeight = advance_height;
            BottomSideBearing = bottom_bearing;
        }
    }
}