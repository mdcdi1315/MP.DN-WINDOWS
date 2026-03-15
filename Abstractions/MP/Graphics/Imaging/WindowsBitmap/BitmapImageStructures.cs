
using System.Runtime.InteropServices;

namespace MP.Graphics.Imaging.WindowsBitmap
{
    // This structure is not used in any native operation , it is just used to create or read typed bitmaps.
    [StructLayout(LayoutKind.Explicit, Size = 14 , Pack = 1)]
    internal unsafe struct BITMAPFILEHEADER
    {
        public const System.UInt16 BMTYPE = 0x4d42;

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
}