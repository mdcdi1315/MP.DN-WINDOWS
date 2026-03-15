namespace MP.NativeInterop.Windows.GDI
{
    /// <summary>
    /// Defines the type of image data contained in a GDI bitmap.
    /// </summary>
    public enum BitmapImageType : System.UInt32
    {
        /// <summary>The bitmap contains raw RGB values.</summary>
        BI_RGB = 0,
        /// <summary>The bitmap contains Run Length Encoded values. Encoding is done with 8 bits.</summary>
        BI_RLE8 = 1,
        /// <summary>The bitmap contains Run Length Encoded values. Encoding is done with 4 bits.</summary>
        BI_RLE4 = 2,
        /// <summary>The bitmap contains bit fields that specifies masks to be appended to all the color values of the bitmap.</summary>
        BI_BITFIELDS = 3,
        /// <summary>The bitmap is written using the JPEG format.</summary>
        BI_JPEG = 4,
        /// <summary>The bitmap is written using the PNG format.</summary>
        BI_PNG = 5
    }
}
