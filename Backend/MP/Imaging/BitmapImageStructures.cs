
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;

namespace MP.Imaging
{
    internal enum ImageType : System.UInt32
    {
        BI_RGB = 0,
        BI_RLE8 = 1,
        BI_RLE4 = 2,
        BI_BITFIELDS = 3,
        BI_JPEG = 4,
        BI_PNG = 5
    }

    internal enum BitmapColorSpace : System.UInt32
    {
        CALIBRATED_RGB = 0,
        SRGB = 1111970419,
        WINDOWS_COLOR_SPACE = 544106839
    }

    // Native Bitmap Information header so as to get the bitmap information.
    [StructLayout(LayoutKind.Explicit, Size = 40 , Pack = 1)] // although that we can safely say Pack = 2 do not do it so as to save exactly the structure.
    internal unsafe struct BITMAPINFOHEADER
    {
        // The below fields are being loaded and being written by ReadStructure and WriteStructure of UnsafeMethods class.
        [FieldOffset(0)]
        public System.UInt32 Size;

        [FieldOffset(4)]
        public System.Int32 Width;

        [FieldOffset(8)]
        public System.Int32 Height;

        [FieldOffset(12)]
        public System.UInt16 Planes;

        [FieldOffset(14)]
        public System.UInt16 BitCount;

        [FieldOffset(16)]
        public ImageType Compression;

        [FieldOffset(20)]
        public System.UInt32 ImageSize;

        [FieldOffset(24)]
        public System.Int32 HorizontalResolution;

        [FieldOffset(28)]
        public System.Int32 VerticalResolution;

        [FieldOffset(32)]
        public System.UInt32 ColorIndices;

        [FieldOffset(36)]
        public System.UInt32 ImportantIndices;

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
        public readonly System.Int32 DataSize
        {
            get {
                if (ImageSize > 0) { return ImageSize.ToInt32(); }
                return AbsoluteHeight * ((((Width * BitCount) + 31) & ~31) >> 3);
            }
        }

        /// <summary>
        /// Gets a value of how many color tables must/or exist in the image. <br />
        /// A value of 0 means that the image type is invalid to compute color indices , 
        /// or the image does not require color tables.
        /// </summary>
        public readonly System.UInt32 ColorTablesCount
        {
            get
            {
                System.UInt32 result = 0;
                // Color tables are not required for 16-bit bitmaps and above.
                switch (Compression)
                {
                    case ImageType.BI_RGB:
                    case ImageType.BI_RLE8:
                    case ImageType.BI_RLE4:
                        // No meaning to compute the color tables if the bpp is more than 8!
                        if (BitCount <= 8)
                        {
                            if (ColorIndices == 0) {
                                result = (System.UInt32)System.Math.Pow(2, BitCount);
                            } else {
                                result = ColorIndices;
                            }
                        }
                        break;
                    default:
                        // But if these are existing for 16-bit and above , return them.
                        if (ColorIndices > 0) { result = ColorIndices; }
                        break;
                }
                // Additionally , if the important color indices is not zero , return that instead.
                if (ImportantIndices > 0 && ColorIndices > 0)
                {
                    result = ImportantIndices;
                }
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
            get {
                // Size of a single color table. (It is named that way because the original color table structure is named RGBQUAD) 
                const System.Int32 RGBQUADSIZE = 4;
                System.UInt32 result = 0;
                switch (Compression)
                {
                    case ImageType.BI_RGB:
                    case ImageType.BI_RLE8:
                    case ImageType.BI_RLE4:
                        result = ColorTablesCount * RGBQUADSIZE;
                        break;
                    case ImageType.BI_BITFIELDS:
                        // In this case we have 3 masks , each of which is a DWORD so it is 3 * the size of a System.UInt32.
                        // If we also have color tables , add them too!
                        // It works correctly even when ColorTablesCount == 0 because all the expression will evaluate to zero.
                        result = (3 * sizeof(System.UInt32)) + (ColorTablesCount * RGBQUADSIZE);
                        break;
                }
                return result;
            }
        }
    }

    // Extended version of BITMAPINFOHEADER.
    // Defines additional data that may be saved with the image.
    [StructLayout(LayoutKind.Explicit, Size = 108, Pack = 1)]
    internal unsafe struct BITMAPV4HEADER
    {
        // Do not re-write again all the fields, just re-use them!!
        [FieldOffset(0)]
        public BITMAPINFOHEADER CoreHeader;

        // In the normal structure the fields of this structure are also laid out here.
        [FieldOffset(40)]
        public BITFIELDS Masks;

        // Alpha mask is not defined by the BITFIELDS structure, so define it here :-)
        [FieldOffset(52)]
        public System.UInt32 AlphaMask;

        [FieldOffset(56)]
        public BitmapColorSpace ColorSpace;

        [FieldOffset(60)]
        public CIEXYZTRIPLE Endpoints;

        [FieldOffset(96)]
        public System.UInt32 RedGamma;

        [FieldOffset(100)]
        public System.UInt32 GreenGamma;

        [FieldOffset(104)]
        public System.UInt32 BlueGamma;

        public BITMAPV4HEADER()
        {
            // Whence creating a new structure instance , set the Size field in the CoreHeader appropriately.
            CoreHeader = new();
            CoreHeader.Size = sizeof(BITMAPV4HEADER).ToUInt32();
            Masks = new();
            Endpoints = new();
        }
    }

    // This structure is not used in any native operation , it is just used to create or read typed bitmaps.
    [StructLayout(LayoutKind.Explicit, Size = 14 , Pack = 1)]
    internal unsafe struct BITMAPFILEHEADER
    {
        public const System.UInt16 BMTYPE = 0x4d42;

        private static System.Byte[] CreateHeaderData(System.Byte[] withoutheader, System.Int32 startindex, BITMAPFILEHEADER header)
        {
            System.UInt32 filehdrsize = sizeof(BITMAPFILEHEADER).ToUInt32();
            // Combine the data and return them.
            System.Byte[] result = new System.Byte[filehdrsize + (withoutheader.Length - startindex)];
            // The below unsafe calls avoid to create a new byte array ,
            // and improve performance.
            fixed (System.Byte* dst = result)
            {
                fixed (System.Byte* hdrptr = &Unsafe.AsRef(in header.pin))
                {
                    Unsafe.CopyBlockUnaligned(dst, hdrptr, filehdrsize);
                }
                fixed (System.Byte* src = &withoutheader[startindex])
                {
                    Unsafe.CopyBlockUnaligned(Unsafe.Add<System.Byte>(dst, filehdrsize.ToInt32()),
                        src, (withoutheader.Length - startindex).ToUInt32());
                }
            }
            return result;
        }

        public static System.Byte[] CreateBitmap(System.Byte[] withoutheader)
        {
            // The below variable is constant but keep it this way for compat.
            System.UInt32 filehdrsize = sizeof(BITMAPFILEHEADER).ToUInt32();
            BITMAPINFOHEADER data = withoutheader.ReadStructure<BITMAPINFOHEADER>(0);
            BITMAPFILEHEADER header = new();
            header.Type = 0x4d42; // Is the 'BM' string in ASCII.
            // Because the final size of the entire bitmap data is known due to the array ,
            // it is possible to just add the header size plus the raw data length themselves.
            header.Size = (filehdrsize + withoutheader.Length).ToUInt32();
            // The below field is set based on this article: https://learn.microsoft.com/en-us/windows/win32/gdi/storing-an-image
            header.Offset = filehdrsize + data.Size + data.ColorTablesSize;
            // Set reserved fields to zero (although that setting to them other values would not be a problem)
            header.RSVD1 = 0; header.RSVD2 = 0;
            return CreateHeaderData(withoutheader, 0, header);
        }

        [FieldOffset(0)]
        private System.Byte pin;

        [FieldOffset(0)]
        public System.UInt16 Type;

        [FieldOffset(2)]
        public System.UInt32 Size;

        [FieldOffset(6)]
        public System.UInt16 RSVD1;

        [FieldOffset(8)]
        public System.UInt16 RSVD2;

        [FieldOffset(10)]
        public System.UInt32 Offset;
    }

    // Usually this represents a color table quad.
    [StructLayout(LayoutKind.Explicit , Size = 4 , Pack = 1)]
    internal struct RGBQUAD
    {
        [FieldOffset(0)]
        public System.Byte B;

        [FieldOffset(1)]
        public System.Byte G;

        [FieldOffset(2)]
        public System.Byte R;

        [FieldOffset(3)]
        public System.Byte RSVD;
    }

    // A custom structure for when we have to decode a BI_BITFIELDS image
    [StructLayout(LayoutKind.Explicit , Size = 12 , Pack = 4)]
    internal struct BITFIELDS
    {
        [FieldOffset(0)]
        public System.UInt32 RedMask;

        [FieldOffset(4)]
        public System.UInt32 GreenMask;

        [FieldOffset(8)]
        public System.UInt32 BlueMask;

        // Usage of the below properties is to identify the color format to be decoded.

        public static BITFIELDS RGB555 // RGB555 color masks.
                => new() { RedMask = 0x7C00 , GreenMask = 0x3E0 , BlueMask = 0x7F };

        public static BITFIELDS RGB565 // RGB565 color masks.
                => new() { RedMask = 0xF800, GreenMask = 0x7E0, BlueMask = 0x1F };

        public static System.Boolean BitFieldsEqual(BITFIELDS one, BITFIELDS two)
            => one.RedMask == two.RedMask && one.GreenMask == two.GreenMask && one.BlueMask == two.BlueMask;
    }

    [StructLayout(LayoutKind.Explicit, Size = 12, Pack = 4)]
    internal struct CIEXYZ
    {
        [FieldOffset(0)]
        public System.Int32 X;

        [FieldOffset(4)]
        public System.Int32 Y;

        [FieldOffset(8)]
        public System.Int32 Z;
    }

    [StructLayout(LayoutKind.Explicit, Size = 36)]
    internal struct CIEXYZTRIPLE
    {
        [FieldOffset(0)]
        public CIEXYZ Red;

        [FieldOffset(12)]
        public CIEXYZ Green;

        [FieldOffset(24)]
        public CIEXYZ Blue;
    }
}