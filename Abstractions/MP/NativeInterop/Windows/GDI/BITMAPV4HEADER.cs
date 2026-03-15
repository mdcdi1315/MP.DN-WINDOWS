
using MP.Utilities;
using System.Runtime.InteropServices;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace MP.NativeInterop.Windows.GDI
{
    /// <summary>
    /// Provides a newer header variant of the <see cref="BITMAPINFOHEADER"/> structure for bitmaps.
    /// </summary>
    [StructLayout(LayoutKind.Explicit, Size = 108)]
    public unsafe struct BITMAPV4HEADER : IDerivedStruct<BITMAPV4HEADER, BITMAPINFOHEADER>
    {
        // Do not re-write again all the fields, just re-use them!!
        /// <summary>
        /// The basic data inherited by this newer structure.
        /// </summary>
        [FieldOffset(0)]
        public BITMAPINFOHEADER CoreHeader;

        /// <summary>
        /// The bit masks, if any, for this bitmap.
        /// </summary>
        // In the normal structure the fields of this structure are also laid out here.
        [FieldOffset(40)]
        public BITFIELDS Masks;

        /// <summary>
        /// The alpha bit mask, if any, for this bitmap.
        /// </summary>
        // Alpha mask is not defined by the BITFIELDS structure, so define it here :-)
        [FieldOffset(52)]
        public System.UInt32 AlphaMask;

        /// <summary>The color space that the bitmap utilizes.</summary>
        [FieldOffset(56)]
        public BitmapColorSpace ColorSpace;

        /// <summary>The bitmap endpoints, if <see cref="BitmapColorSpace.CALIBRATED_RGB"/> is used.</summary>
        [FieldOffset(60)]
        public CIEXYZTRIPLE Endpoints;

        /// <summary>The red channel gamut, if any.</summary>
        [FieldOffset(96)]
        public System.UInt32 RedGamma;

        /// <summary>The green channel gamut, if any.</summary>
        [FieldOffset(100)]
        public System.UInt32 GreenGamma;

        /// <summary>The blue channel gamut, if any.</summary>
        [FieldOffset(104)]
        public System.UInt32 BlueGamma;

        /// <summary>
        /// Properly initializes a new instance of the <see cref="BITMAPV4HEADER"/> structure.
        /// </summary>
        public BITMAPV4HEADER()
        {
            // Whence creating a new structure instance , set the Size field in the CoreHeader appropriately.
            CoreHeader = new();
            CoreHeader.Size = sizeof(BITMAPV4HEADER).ToUInt32();
            Masks = new();
            Endpoints = new();
        }

        /// <inheritdoc />
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator BITMAPINFOHEADER([DisallowNull] BITMAPV4HEADER t) => t.CoreHeader;
    }
}