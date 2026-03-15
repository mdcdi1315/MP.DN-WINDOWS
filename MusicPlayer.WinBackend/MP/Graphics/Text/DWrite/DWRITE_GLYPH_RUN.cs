
using MP.NativeInterop.Windows;
using System.Runtime.InteropServices;

namespace MP.Graphics.Text.DWrite
{
    [StructLayout(LayoutKind.Sequential)]
    public unsafe struct DWRITE_GLYPH_RUN
    {
        public void* fontFace; // IDWriteFontFace
        public System.Single fontEmSize;
        public System.UInt32 glyphCount;
        public System.UInt16* glyphIndices;
        public System.Single* glyphAdvances;
        public DWRITE_GLYPH_OFFSET* glyphOffsets;
        public BOOL isSideways;
        public System.UInt32 bidiLevel;
    }
}