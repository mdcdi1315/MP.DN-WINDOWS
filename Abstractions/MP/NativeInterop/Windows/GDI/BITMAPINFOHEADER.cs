
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;

namespace MP.NativeInterop.Windows.GDI
{
    /// <summary>
    /// Defines the header of a GDI bitmap.
    /// </summary>
    [StructLayout(LayoutKind.Explicit, Size = 40, Pack = 4)]
    public unsafe struct BITMAPINFOHEADER
    {
        // The below fields are being loaded and being written by ReadStructure and WriteStructure of UnsafeMethods class.
        /// <summary>The size of this <see cref="BITMAPINFOHEADER"/> structure.</summary>
        [FieldOffset(0)]
        public System.UInt32 Size;

        /// <summary>The width, in pixels, of the bitmap.</summary>
        [FieldOffset(4)]
        public System.Int32 Width;

        /// <summary>The height, in pixels, of the bitmap.</summary>
        [FieldOffset(8)]
        public System.Int32 Height;

        /// <summary>The number of planes that the bitmap contains. Typically this is set to 1.</summary>
        [FieldOffset(12)]
        public System.UInt16 Planes;

        /// <summary>The number of bits required to store a single pixel.</summary>
        [FieldOffset(14)]
        public System.UInt16 BitCount;

        /// <summary>The compression method used to store the bitmap.</summary>
        [FieldOffset(16)]
        public BitmapImageType Compression;

        /// <summary>If the bitmap is compressed, this value is set to the size of the compressed data.</summary>
        [FieldOffset(20)]
        public System.UInt32 ImageSize;

        /// <summary>Horizontal resolution.</summary>
        [FieldOffset(24)]
        public System.Int32 HorizontalResolution;

        /// <summary>Vertical resolution.</summary>
        [FieldOffset(28)]
        public System.Int32 VerticalResolution;

        /// <summary>Number of color indices contained in the bitmap.</summary>
        [FieldOffset(32)]
        public System.UInt32 ColorIndices;

        /// <summary>Number of color indices required to decode and display the bitmap.</summary>
        [FieldOffset(36)]
        public System.UInt32 ImportantIndices;

        /// <summary>Gets the value of the <see cref="Height"/> field in absolute value.</summary>
        public readonly System.Int32 AbsoluteHeight
        {
            // Inline it agressively, if the JIT can do that.
            // Here inlining can happen because this property
            // does not call any other functions.
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => Height < 0 ? -Height : Height;
        }

        /// <summary>
        /// Gets the image size in bytes.
        /// </summary>
        public readonly System.UInt32 DataSize => (ImageSize > 0U) ? ImageSize : (AbsoluteHeight * ((((Width * BitCount.ToInt64()) + 31L) & ~31L) >> 3)).ToUInt32();

        /// <summary>
        /// Gets a value of how many color tables must/or exist in the image. <br />
        /// A value of 0 means that the image type is invalid to compute color indices , 
        /// or the image does not require color tables.
        /// </summary>
        public readonly System.UInt32 ColorTablesCount
        {
            get {
                System.UInt32 result = 0;
                // Color tables are not required for 16-bit bitmaps and above.
                switch (Compression)
                {
                    case BitmapImageType.BI_RGB:
                    case BitmapImageType.BI_RLE8:
                    case BitmapImageType.BI_RLE4:
                        // No meaning to compute the color tables if the bpp is more than 8!
                        if (BitCount <= 8) {
                            result = (ColorIndices == 0) ? (System.UInt32)System.Math.Pow(2, BitCount) : ColorIndices;
                        }
                        break;
                    default:
                        // But if these are existing for 16-bit and above , return them.
                        if (ColorIndices > 0) { result = ColorIndices; }
                        break;
                }
                // Additionally , if the important color indices is not zero , return that instead.
                if (ImportantIndices > 0 && ColorIndices > 0) { result = ImportantIndices; }
                return result;
            }
        }

        /// <summary>
        /// Gets the final size of the color tables in bytes. <br />
        /// This value can be zero when this value is not required or when the image type is invalid.
        /// </summary>
        public readonly System.UInt32 ColorTablesSize
        {
            // The below code is pretty-much based on multiple articles that specify how color tables are laid out.
            // For a basic grasp you can see https://learn.microsoft.com/en-us/windows/win32/api/wingdi/ns-wingdi-bitmapinfoheader#color-tables
            get
            {
                // Size of a single color table. (It is named that way because the original color table structure is named RGBQUAD) 
                const System.Int32 RGBQUADSIZE = 4;
                System.UInt32 result = 0;
                switch (Compression)
                {
                    case BitmapImageType.BI_RGB:
                    case BitmapImageType.BI_RLE8:
                    case BitmapImageType.BI_RLE4:
                        result = ColorTablesCount * RGBQUADSIZE;
                        break;
                    case BitmapImageType.BI_BITFIELDS:
                        // In this case we have 3 masks , each of which is a DWORD so it is 3 * the size of a System.UInt32.
                        // If we also have color tables , add them too!
                        // It works correctly even when ColorTablesCount == 0 because all the expression will evaluate to zero.
                        result = (sizeof(BITFIELDS) + (ColorTablesCount * RGBQUADSIZE)).ToUInt32();
                        break;
                }
                return result;
            }
        }
    }
}