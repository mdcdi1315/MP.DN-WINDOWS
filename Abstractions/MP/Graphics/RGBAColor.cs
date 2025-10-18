
using System;
using System.Runtime.InteropServices;

namespace MP.Graphics
{
    /// <summary>
    /// Defines a color containing RGBA values. <br />
    /// Data are depicted by the red channel first.
    /// </summary>
    [StructLayout(LayoutKind.Explicit, Pack = 1, Size = 4)]
    public readonly struct RGBAColor : 
        ITruncatable<RGBColor>, 
        ICloneable,
        IColor
    {
        [FieldOffset(0)]
        private readonly byte r;

        [FieldOffset(1)]
        private readonly byte g;

        [FieldOffset(2)]
        private readonly byte b;

        [FieldOffset(3)]
        private readonly byte a;

        /// <summary>
        /// Creates a RGBA color from the specified red, green, blue and alpha channels.
        /// </summary>
        /// <param name="r">The red channel value.</param>
        /// <param name="g">The green channel value.</param>
        /// <param name="b">The blue channel value.</param>
        /// <param name="a">The alpha channel value.</param>
        public RGBAColor(byte r, byte g, byte b, byte a)
        {
            this.r = r;
            this.g = g;
            this.b = b;
            this.a = a;
        }

        /// <inheritdoc />
        public readonly byte A => a;

        /// <inheritdoc />
        public readonly byte R => r;

        /// <inheritdoc />
        public readonly byte G => g;

        /// <inheritdoc />
        public readonly byte B => b;

        /// <summary>
        /// Truncates this <see cref="RGBAColor"/> instance to an <see cref="RGBColor"/> instance. <br />
        /// The alpha channel is lost.
        /// </summary>
        /// <returns>A new <see cref="RGBColor"/> instance, representing the truncated result.</returns>
        public readonly RGBColor Truncate() => new(r, g, b);

        /// <summary>Creates a copy of this <see cref="RGBAColor"/> instance to a new instance.</summary>
        /// <returns>A new instance of the <see cref="RGBAColor"/> structure, having the same color intensities as this <see cref="RGBAColor"/> instance.</returns>
        public readonly RGBAColor Clone() => new(r, g, b, a);

        readonly object ICloneable.Clone() => Clone();

        /// <summary>Gets a string describing this color.</summary>
        /// <returns>The color description.</returns>
        public override System.String ToString() => $"Color<RGBA> {{ Red: {r} , Green: {g} , Blue: {b} , Alpha: {a} }}";
    }
}