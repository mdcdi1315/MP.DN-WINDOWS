namespace MP.Graphics.Text.DWrite
{
    /// <summary>
    /// Represents the internal structure of a device pixel (i.e., the physical arrangement of red,
    /// green, and blue color components) that is assumed for purposes of rendering text.
    /// </summary>
    public enum DWRITE_PIXEL_GEOMETRY
    {
        // The prefix DWRITE_PIXEL_GEOMETRY is ommitted for brevity.

        /// <summary>
        /// The red, green, and blue color components of each pixel are assumed to occupy the same point.
        /// </summary>
        FLAT,

        /// <summary>
        /// Each pixel comprises three vertical stripes, with red on the left, green in the center, and 
        /// blue on the right. This is the most common pixel geometry for LCD monitors.
        /// </summary>
        RGB,

        /// <summary>
        /// Each pixel comprises three vertical stripes, with blue on the left, green in the center, and 
        /// red on the right.
        /// </summary>
        BGR
    }
}