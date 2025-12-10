
using System;
using System.Numerics;
using System.Runtime.InteropServices;

namespace MP.Graphics
{
    /// <summary>
    /// Defines a color containing RGB values. <br />
    /// Data are depicted by the red channel first.
    /// </summary>
    [StructLayout(LayoutKind.Explicit, Pack = 1, Size = 3)]
    public readonly struct RGBColor :
        IEqualityOperators<RGBColor, RGBColor, bool>,
        IEquatable<BGRColor>,
        IEquatable<RGBColor>,
        IEquatable<IColor>,
        ICloneable,
        IColor
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

        /// <summary>
        /// Gets a value whether this instance has the same color intensities as a <see cref="BGRColor"/> instance.
        /// </summary>
        /// <param name="other">The other instance to compare this instance against.</param>
        /// <returns>A value whether the current and the <paramref name="other"/> structures are considered equal.</returns>
        public readonly bool Equals(BGRColor other) => other.R == r && other.G == g && other.B == b;

        /// <summary>
        /// Gets a value whether this instance has the same color intensities as another <see cref="RGBColor"/> instance.
        /// </summary>
        /// <param name="other">The other instance to compare this instance against.</param>
        /// <returns>A value whether the current and the <paramref name="other"/> structures are considered equal.</returns>
        public readonly bool Equals(RGBColor other) => other.b == r && other.b == b && other.b == b;

        /// <summary>
        /// Gets a value whether this instance has the same color intensities as any object supporting the <see cref="IColor"/> interface.
        /// </summary>
        /// <param name="other">The other instance to compare this instance against.</param>
        /// <returns>A value whether the current and the <paramref name="other"/> objects are considered equal.</returns>
        public readonly bool Equals(IColor other) => other.R == r && other.G == b && other.B == b;

        /// <summary>
        /// Gets a value whether this <see cref="BGRColor"/> instance and an object are equal instances.
        /// </summary>
        /// <param name="obj">The other object instance to compare this instance against.</param>
        /// <returns>A value whether the current and the <paramref name="obj"/> objects are considered equal.</returns>
        public override readonly bool Equals(object obj) => obj switch
        {
            BGRColor c => Equals(c),
            RGBColor c => Equals(c),
            IColor cg => Equals(cg),
            _ => false,
        };

        /// <summary>Creates a copy of this <see cref="RGBColor"/> instance to a new instance.</summary>
        /// <returns>A new instance of the <see cref="RGBColor"/> structure, having the same color intensities as this <see cref="RGBColor"/> instance.</returns>
        public readonly RGBColor Clone() => new(r,g,b);

        readonly System.Object ICloneable.Clone() => Clone();

        /// <summary>Gets a hash code for this <see cref="RGBColor"/> instance.</summary>
        /// <returns>A hash code for this instance.</returns>
        public readonly override int GetHashCode() => this.GetCommonHashCode();

        /// <summary>Gets a string describing this color.</summary>
        /// <returns>The color description.</returns>
        public readonly override System.String ToString() => $"Color<RGB> {{ Red: {r} , Green: {g} , Blue: {b} }}";

        /// <summary>Defines whether two <see cref="RGBColor"/> instances are equal.</summary>
        /// <param name="left">The first <see cref="RGBColor"/> to compare.</param>
        /// <param name="right">The second <see cref="RGBColor"/> to compare.</param>
        /// <returns>A value whether the two passed <see cref="RGBColor"/> instances are indeed equal.</returns>
        public static bool operator ==(RGBColor left, RGBColor right) => left.Equals(right);

        /// <summary>Defines whether two <see cref="RGBColor"/> instances are inequal.</summary>
        /// <param name="left">The first <see cref="RGBColor"/> to compare.</param>
        /// <param name="right">The second <see cref="RGBColor"/> to compare.</param>
        /// <returns>A value whether the two passed <see cref="RGBColor"/> instances are indeed inequal.</returns>
        public static bool operator !=(RGBColor left, RGBColor right) => !left.Equals(right);
    }
}