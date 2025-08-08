
using System.Runtime.InteropServices;

namespace MP.Graphics
{
    /// <summary>
    /// Defines a color containing ARGB values. <br />
    /// Data are depicted by the alpha channel first.
    /// </summary>
    [StructLayout(LayoutKind.Explicit, Pack = 1, Size = 4)]
    public readonly struct ARGBColor : IColor
    {
        [FieldOffset(0)]
        private readonly byte a;

        [FieldOffset(1)]
        private readonly byte r;

        [FieldOffset(2)]
        private readonly byte g;

        [FieldOffset(3)]
        private readonly byte b;

        /// <summary>
        /// Creates an ARGB color from the specified red, green, blue and alpha channels.
        /// </summary>
        /// <param name="r">The red channel value.</param>
        /// <param name="g">The green channel value.</param>
        /// <param name="b">The blue channel value.</param>
        /// <param name="a">The alpha channel value.</param>
        public ARGBColor(byte r, byte g, byte b, byte a)
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

        /// <summary>Gets a string describing this color.</summary>
        /// <returns>The color description.</returns>
        public override System.String ToString() => $"Color<ARGB> {{ Red: {r} , Green: {g} , Blue: {b} , Alpha: {a} }}";
    }
}