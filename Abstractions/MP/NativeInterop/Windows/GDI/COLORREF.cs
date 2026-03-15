
using System;
using MP.Graphics;
using System.Runtime.InteropServices;

namespace MP.NativeInterop.Windows.GDI
{
    /// <summary>
    /// The <see cref="COLORREF"/> structure represents a color in Windows GDI.
    /// </summary>
    [StructLayout(LayoutKind.Explicit)]
    public readonly struct COLORREF : IColor
    {
        [FieldOffset(0)]
        private readonly byte RSVD;

        [FieldOffset(1)]
        private readonly byte Blue;

        [FieldOffset(2)]
        private readonly byte Green;

        [FieldOffset(3)]
        private readonly byte Red;

        /// <inheritdoc />
        public byte A => 255;

        /// <inheritdoc />
        public byte R => Red;

        /// <inheritdoc />
        public byte B => Blue;

        /// <inheritdoc />
        public byte G => Green;

        /// <summary>
        /// Constructs a new instance of the <see cref="COLORREF"/> from the specified raw values.
        /// </summary>
        /// <param name="red">The red intensity of the color.</param>
        /// <param name="green">The green intensity of the color.</param>
        /// <param name="blue">The blue intensity of the color.</param>
        public COLORREF(byte red, byte green, byte blue)
        {
            RSVD = 0;
            Red = red;
            Green = green;
            Blue = blue;
        }

        /// <summary>
        /// Constructs a new instance of the <see cref="COLORREF"/> structure from the specified <see cref="IColor"/> instance.
        /// </summary>
        /// <param name="color">The color to initialize this <see cref="COLORREF"/> structure from.</param>
        public COLORREF(IColor color)
        {
            ArgumentNullException.ThrowIfNull(color);
            RSVD = 0;
            Red = color.R;
            Blue = color.B;
            Green = color.G;
        }
    }
}