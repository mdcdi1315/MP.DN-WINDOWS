
using System.Runtime.InteropServices;

namespace MP.NativeInterop.Windows.GDI
{
    /// <summary>Specifies a single endpoint in a <see cref="BITMAPV4HEADER"/> structure.</summary>
    [StructLayout(LayoutKind.Explicit, Size = 12, Pack = 4)]
    public struct CIEXYZ
    {
        /// <summary>X coordinate of the endpoint.</summary>
        [FieldOffset(0)]
        public System.Int32 X;

        /// <summary>Y coordinate of the endpoint.</summary>
        [FieldOffset(4)]
        public System.Int32 Y;

        /// <summary>Z coordinate of the endpoint.</summary>
        [FieldOffset(8)]
        public System.Int32 Z;
    }

    /// <summary>Specifies the endpoints of custom color space.</summary>
    [StructLayout(LayoutKind.Explicit, Size = 36)]
    public struct CIEXYZTRIPLE
    {
        /// <summary>The red color endpoint.</summary>
        [FieldOffset(0)]
        public CIEXYZ Red;

        /// <summary>The green color endpoint.</summary>
        [FieldOffset(12)]
        public CIEXYZ Green;

        /// <summary>The blue color endpoint.</summary>
        [FieldOffset(24)]
        public CIEXYZ Blue;
    }
}