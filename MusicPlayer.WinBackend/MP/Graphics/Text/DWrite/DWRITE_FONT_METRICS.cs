
using System.Runtime.InteropServices;

namespace MP.Graphics.Text.DWrite
{
    [StructLayout(LayoutKind.Explicit, Size = (sizeof(System.UInt16) * 7) + (sizeof(System.Int16) * 3))]
    public struct DWRITE_FONT_METRICS
    {
        [FieldOffset(0)]
        public System.UInt16 designUnitsPerEm;
        [FieldOffset(sizeof(System.UInt16))]
        public System.UInt16 ascent;
        [FieldOffset(sizeof(System.UInt16) * 2)]
        public System.UInt16 descent;
        [FieldOffset(sizeof(System.UInt16) * 3)]
        public System.Int16 lineGap;
        [FieldOffset((sizeof(System.UInt16) * 3) + sizeof(System.Int16))]
        public System.UInt16 capHeight;
        [FieldOffset((sizeof(System.UInt16) * 4) + sizeof(System.Int16))]
        public System.UInt16 xHeight;
        [FieldOffset((sizeof(System.UInt16) * 5) + sizeof(System.Int16))]
        public System.Int16 underlinePosition;
        [FieldOffset((sizeof(System.UInt16) * 5) + (sizeof(System.Int16) * 2))]
        public System.UInt16 underlineThickness;
        [FieldOffset((sizeof(System.UInt16) * 6) + (sizeof(System.Int16) * 2))]
        public System.Int16 strikethroughPosition;
        [FieldOffset((sizeof(System.UInt16) * 6) + (sizeof(System.Int16) * 3))]
        public System.UInt16 strikethroughThickness;
    }
}