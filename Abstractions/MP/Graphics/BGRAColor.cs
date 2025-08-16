
using System.Runtime.InteropServices;

namespace MP.Graphics
{
    /// <summary>
    /// Defines a color containing BGRA values. <br />
    /// Data are depicted by the blue channel first. <br />
    /// This is the same as <see cref="RGBAColor"/>; however, it defines a different data depiction.
    /// </summary>
    [StructLayout(LayoutKind.Explicit, Pack = 1, Size = 4)]
    public readonly struct BGRAColor : IColor
    {
        [FieldOffset(0)]
        private readonly byte b;

        [FieldOffset(1)]
        private readonly byte g;

        [FieldOffset(2)]
        private readonly byte r;

        [FieldOffset(3)]
        private readonly byte a;

        /// <summary>
        /// Creates a BGRA color from the specified red, green, blue and alpha channels.
        /// </summary>
        /// <param name="r">The red channel value.</param>
        /// <param name="g">The green channel value.</param>
        /// <param name="b">The blue channel value.</param>
        /// <param name="a">The alpha channel value.</param>
        public BGRAColor(byte b, byte g, byte r, byte a)
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
        public override System.String ToString() => $"Color<BGRA> {{ Red: {r} , Green: {g} , Blue: {b} , Alpha: {a} }}";
    }
}