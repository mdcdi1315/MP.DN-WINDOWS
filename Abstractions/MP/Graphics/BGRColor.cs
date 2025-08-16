
using System.Runtime.InteropServices;

namespace MP.Graphics
{
    /// <summary>
    /// Defines a color containing BGR values. <br />
    /// Data are depicted by the blue channel first. <br />
    /// This is the same as <see cref="RGBColor"/>; however, it defines a different data depiction.
    /// </summary>
    [StructLayout(LayoutKind.Explicit, Pack = 1, Size = 3)]
    public readonly struct BGRColor : IColor
    {
        [FieldOffset(0)]
        private readonly byte b;

        [FieldOffset(1)]
        private readonly byte g;

        [FieldOffset(2)]
        private readonly byte r;

        /// <summary>
        /// Creates a BGR color from the specified red, green and blue channels.
        /// </summary>
        /// <param name="r">The red channel value.</param>
        /// <param name="g">The green channel value.</param>
        /// <param name="b">The blue channel value.</param>
        public BGRColor(byte b, byte g, byte r)
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
        public override System.String ToString() => $"Color<BGR> {{ Red: {r} , Green: {g} , Blue: {b} }}";
    }
}