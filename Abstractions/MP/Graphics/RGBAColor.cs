
using System;
using System.Numerics;
using System.Runtime.InteropServices;

namespace MP.Graphics
{
    /// <summary>
    /// Defines a color containing RGBA values. <br />
    /// Data are depicted by the red channel first.
    /// </summary>
    [StructLayout(LayoutKind.Explicit, Pack = 1, Size = 4)]
    public readonly struct RGBAColor :
        IEqualityOperators<RGBAColor, RGBAColor, bool>,
        ITruncatable<RGBColor>, 
        IEquatable<RGBAColor>,
        IEquatable<ARGBColor>,
        IEquatable<BGRAColor>,
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
        /// Gets a value whether this instance has the same color intensities as another <see cref="RGBAColor"/> instance.
        /// </summary>
        /// <param name="other">The other instance to compare this instance against.</param>
        /// <returns>A value whether the current and the <paramref name="other"/> structures are considered equal.</returns>
        public readonly bool Equals(RGBAColor other) => 
            r == other.r &&
            g == other.g &&
            b == other.b &&
            a == other.a;

        /// <summary>
        /// Gets a value whether this instance has the same color intensities as a <see cref="ARGBColor"/> instance.
        /// </summary>
        /// <param name="other">The other instance to compare this instance against.</param>
        /// <returns>A value whether the current and the <paramref name="other"/> structures are considered equal.</returns>
        public readonly bool Equals(ARGBColor other) =>
            r == other.R &&
            g == other.G &&
            b == other.B &&
            a == other.A;

        /// <summary>
        /// Gets a value whether this instance has the same color intensities as a <see cref="BGRAColor"/> instance.
        /// </summary>
        /// <param name="other">The other instance to compare this instance against.</param>
        /// <returns>A value whether the current and the <paramref name="other"/> structures are considered equal.</returns>
        public readonly bool Equals(BGRAColor other) =>
            r == other.R &&
            g == other.G &&
            b == other.B &&
            a == other.A;

        /// <summary>
        /// Gets a value whether this instance has the same color intensities as any object supporting the <see cref="IColor"/> interface.
        /// </summary>
        /// <param name="other">The other instance to compare this instance against.</param>
        /// <returns>A value whether the current and the <paramref name="other"/> objects are considered equal.</returns>
        public readonly bool Equals(IColor other) =>
            r == other.R &&
            g == other.G &&
            b == other.B &&
            a == other.A;

        /// <summary>
        /// Gets a value whether this <see cref="RGBAColor"/> instance and an object are equal instances.
        /// </summary>
        /// <param name="obj">The other object instance to compare this instance against.</param>
        /// <returns>A value whether the current and the <paramref name="obj"/> objects are considered equal.</returns>
        public readonly override bool Equals(object obj) => obj switch 
        { 
            RGBAColor c => Equals(c),
            ARGBColor c => Equals(c),
            BGRAColor c => Equals(c),
            IColor c => Equals(c),
            _ => false
        };

        /// <summary>Gets a hash code for this <see cref="RGBAColor"/> instance.</summary>
        /// <returns>A hash code for this instance.</returns>
        public readonly override int GetHashCode() => this.GetCommonHashCode();

        /// <summary>Gets a string describing this color.</summary>
        /// <returns>The color description.</returns>
        public readonly override System.String ToString() => $"Color<RGBA> {{ Red: {r} , Green: {g} , Blue: {b} , Alpha: {a} }}";

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

        /// <summary>Defines whether two <see cref="RGBAColor"/> instances are equal.</summary>
        /// <param name="left">The first <see cref="RGBAColor"/> to compare.</param>
        /// <param name="right">The second <see cref="RGBAColor"/> to compare.</param>
        /// <returns>A value whether the two passed <see cref="RGBAColor"/> instances are indeed equal.</returns>
        public static bool operator ==(RGBAColor left, RGBAColor right) => left.Equals(right);

        /// <summary>Defines whether two <see cref="RGBAColor"/> instances are inequal.</summary>
        /// <param name="left">The first <see cref="RGBAColor"/> to compare.</param>
        /// <param name="right">The second <see cref="RGBAColor"/> to compare.</param>
        /// <returns>A value whether the two passed <see cref="RGBAColor"/> instances are indeed inequal.</returns>
        public static bool operator !=(RGBAColor left, RGBAColor right) => !left.Equals(right);
    }
}