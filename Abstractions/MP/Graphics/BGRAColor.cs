
using System;
using System.Numerics;
using System.Runtime.InteropServices;

namespace MP.Graphics
{
    /// <summary>
    /// Defines a color containing BGRA values. <br />
    /// Data are depicted by the blue channel first. <br />
    /// This is the same as <see cref="RGBAColor"/>; however, it defines a different data depiction.
    /// </summary>
    [StructLayout(LayoutKind.Explicit, Pack = 1, Size = 4)]
    public readonly struct BGRAColor :
        IEqualityOperators<BGRAColor, BGRAColor, bool>,
        ITruncatable<BGRColor>,
        IEquatable<RGBAColor>,
        IEquatable<ARGBColor>,
        IEquatable<BGRAColor>,
        IEquatable<IColor>,
        ICloneable,
        IColor
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

        /// <summary>
        /// Truncates this <see cref="BGRAColor"/> instance to an <see cref="BGRColor"/> instance. <br />
        /// The alpha channel is lost.
        /// </summary>
        /// <returns>A new <see cref="BGRColor"/> instance, representing the truncated result.</returns>
        public readonly BGRColor Truncate() => new(b, g, r);

        /// <summary>Creates a copy of this <see cref="BGRAColor"/> instance to a new instance.</summary>
        /// <returns>A new instance of the <see cref="BGRAColor"/> structure, having the same color intensities as this <see cref="BGRAColor"/> instance.</returns>
        public readonly BGRAColor Clone() => new(b, g, r, a);

        readonly object ICloneable.Clone() => Clone();

        /// <summary>
        /// Gets a value whether this instance has the same color intensities as another <see cref="BGRAColor"/> instance.
        /// </summary>
        /// <param name="other">The other instance to compare this instance against.</param>
        /// <returns>A value whether the current and the <paramref name="other"/> structures are considered equal.</returns>
        public readonly bool Equals(BGRAColor other) =>
            b == other.b &&
            g == other.g &&
            r == other.r &&
            a == other.a;

        /// <summary>
        /// Gets a value whether this instance has the same color intensities as a <see cref="RGBAColor"/> instance.
        /// </summary>
        /// <param name="other">The other instance to compare this instance against.</param>
        /// <returns>A value whether the current and the <paramref name="other"/> structures are considered equal.</returns>
        public readonly bool Equals(RGBAColor other) =>
            b == other.B &&
            g == other.G &&
            r == other.R &&
            a == other.A;

        /// <summary>
        /// Gets a value whether this instance has the same color intensities as a <see cref="ARGBColor"/> instance.
        /// </summary>
        /// <param name="other">The other instance to compare this instance against.</param>
        /// <returns>A value whether the current and the <paramref name="other"/> structures are considered equal.</returns>
        public readonly bool Equals(ARGBColor other) =>
            b == other.B &&
            g == other.G &&
            r == other.R &&
            a == other.A;

        /// <summary>
        /// Gets a value whether this instance has the same color intensities as a <see cref="ARGBColor"/> instance.
        /// </summary>
        /// <param name="other">The other instance to compare this instance against.</param>
        /// <returns>A value whether the current and the <paramref name="other"/> structures are considered equal.</returns>
        public readonly bool Equals(IColor other) =>
            b == other.B &&
            g == other.G &&
            r == other.R &&
            a == other.A;

        /// <summary>
        /// Gets a value whether this <see cref="BGRAColor"/> instance and an object are equal instances.
        /// </summary>
        /// <param name="obj">The other object instance to compare this instance against.</param>
        /// <returns>A value whether the current and the <paramref name="obj"/> objects are considered equal.</returns>
        public readonly override bool Equals(object obj) => obj switch 
        { 
            BGRAColor c => Equals(c),
            RGBAColor c => Equals(c),
            ARGBColor c => Equals(c),
            IColor c => Equals(c),
            _ => false
        };

        /// <summary>Gets a hash code for this <see cref="BGRAColor"/> instance.</summary>
        /// <returns>A hash code for this instance.</returns>
        public readonly override int GetHashCode() => this.GetCommonHashCode();

        /// <summary>Gets a string describing this color.</summary>
        /// <returns>The color description.</returns>
        public readonly override System.String ToString() => $"Color<BGRA> {{ Red: {r} , Green: {g} , Blue: {b} , Alpha: {a} }}";

        /// <summary>Defines whether two <see cref="BGRAColor"/> instances are equal.</summary>
        /// <param name="left">The first <see cref="BGRAColor"/> to compare.</param>
        /// <param name="right">The second <see cref="BGRAColor"/> to compare.</param>
        /// <returns>A value whether the two passed <see cref="BGRAColor"/> instances are indeed equal.</returns>
        public static bool operator ==(BGRAColor left, BGRAColor right) => left.Equals(right);

        /// <summary>Defines whether two <see cref="BGRAColor"/> instances are inequal.</summary>
        /// <param name="left">The first <see cref="BGRAColor"/> to compare.</param>
        /// <param name="right">The second <see cref="BGRAColor"/> to compare.</param>
        /// <returns>A value whether the two passed <see cref="BGRAColor"/> instances are indeed inequal.</returns>
        public static bool operator !=(BGRAColor left, BGRAColor right) => !left.Equals(right);
    }
}