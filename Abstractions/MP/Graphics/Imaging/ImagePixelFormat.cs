

namespace MP.Graphics.Imaging
{
    /// <summary>
    /// Depicts the data organization inside the <see cref="IImage.NativePointer"/> property data. <br />
    /// Used by the <see cref="IImage"/> interface and derivatives.
    /// </summary>
    public enum ImagePixelFormat : System.Byte
    {
        /// <summary>
        /// Data are packed only with the red channel. 
        /// In this category the monochrome bitmaps are also defined in.
        /// </summary>
        R = 0,
        /// <summary>
        /// Data are packed as the red channel first , then the green channel follows.
        /// </summary>
        RG = 1,
        /// <summary>
        /// Data are packed as the red channel first , then the green channel and blue channels do follow.
        /// </summary>
        RGB = 2,
        /// <summary>
        /// Data are packed as the red channel first , then the green , blue and alpha channels do follow.
        /// </summary>
        RGBA = 3,
        /// <summary>
        /// Data are packed as the alpha channel first , then the red, green and blue channels do follow. <br />
        /// This is also the most common format.
        /// </summary>
        ARGB = 4,
        /// <summary>
        /// Data are packed as the blue channel first , then the green, red and alpha channels do follow.
        /// </summary>
        BGRA = 5,
    }
}