namespace MP.NativeInterop.Windows.GDI
{
    /// <summary>
    /// Defines the color space of a bitmap. <br />
    /// This enumeration is used in the newer bitmap header structure, the <see cref="BITMAPV4HEADER"/>.
    /// </summary>
    public enum BitmapColorSpace : System.UInt32
    {
        /// <summary>The color space is calibrated RGB where it's control points are specified by the <see cref="BITMAPV4HEADER.Endpoints"/> field.</summary>
        CALIBRATED_RGB = 0,
        /// <summary>The color space is the sRGB color space (The most common one).</summary>
        SRGB = 1111970419,
        /// <summary>The color space is the Windows color space.</summary>
        WINDOWS_COLOR_SPACE = 544106839
    }
}