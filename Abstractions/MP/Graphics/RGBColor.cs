

using System.Runtime.InteropServices;

namespace MP.Graphics
{
    /// <summary>
    /// Defines a color containing RGB values. <br />
    /// Data are depicted by the red channel first.
    /// </summary>
    [StructLayout(LayoutKind.Explicit, Pack = 1, Size = 3)]
    public readonly struct RGBColor : IColor
    {
        [FieldOffset(0)]
        private readonly byte r;

        [FieldOffset(1)]
        private readonly byte g;

        [FieldOffset(2)]
        private readonly byte b;

        /// <summary>
        /// Creates a RGB color from the specified red, green and blue channels.
        /// </summary>
        /// <param name="r">The red channel value.</param>
        /// <param name="g">The green channel value.</param>
        /// <param name="b">The blue channel value.</param>
        public RGBColor(byte r, byte g, byte b)
        {
            this.r = r;
            this.g = g;
            this.b = b;
        }

        /// <inheritdoc />
        public readonly byte A => 255;

        /// <inheritdoc />
        public readonly byte R => r;

        /// <inheritdoc />
        public readonly byte G => g;

        /// <inheritdoc />
        public readonly byte B => b;

        /// <summary>Gets a string describing this color.</summary>
        /// <returns>The color description.</returns>
        public override System.String ToString() => $"Color<RGB> {{ Red: {r} , Green: {g} , Blue: {b} }}";
    }
}