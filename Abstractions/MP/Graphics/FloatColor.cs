
using System;
using MP.Utilities;
using System.Numerics;
using MP.Annotations.CodeAnalysis;
using System.Runtime.InteropServices;

namespace MP.Graphics
{
    /// <summary>
    /// Defines a color in normalized ([0..1]) floating point format. <br />
    /// The data are packed as RGBA, if needed for native data depiction.
    /// </summary>
    [StructLayout(LayoutKind.Explicit, Pack = sizeof(float) , Size = sizeof(float) * 4)]
    public readonly struct FloatColor : 
        IEqualityOperators<FloatColor , FloatColor , bool>,
        ITruncatable<RGBAColor>,
        ITruncatable<ARGBColor>,
        IEquatable<FloatColor>,
        IEquatable<RGBAColor>,
        IEquatable<ARGBColor>,
        IEquatable<IColor>,
        ICloneable
    {
        [FieldOffset(0)]
        private readonly float red;
        [FieldOffset(sizeof(float))]
        private readonly float green;
        [FieldOffset(sizeof(float) * 2)]
        private readonly float blue;
        [FieldOffset(sizeof(float) * 3)]
        private readonly float alpha;

        /// <summary>
        /// Creates a new <see cref="FloatColor"/> structure instance, ensuring that all parameters are into the range [0..1]. If not, they are appropriately clamped.
        /// </summary>
        /// <param name="red">The red channel.</param>
        /// <param name="green">The green channel.</param>
        /// <param name="blue">The blue channel.</param>
        /// <param name="alpha">The alpha channel.</param>
        public FloatColor(float red , float green, float blue, float alpha)
        {
            this.red = MathHelpers.Clamp(red , 0,  1);
            this.green = MathHelpers.Clamp(green, 0, 1);
            this.blue = MathHelpers.Clamp(blue, 0, 1);
            this.alpha = MathHelpers.Clamp(alpha, 0, 1);
        }

        /// <summary>
        /// Creates a new <see cref="FloatColor"/> structure instance from byte values. <br />
        /// The byte values are appropriately transformed to floating-point values during construction time.
        /// </summary>
        /// <param name="red">The red channel.</param>
        /// <param name="green">The green channel.</param>
        /// <param name="blue">The blue channel.</param>
        /// <param name="alpha">The alpha channel.</param>
        public FloatColor(byte red , byte green , byte blue , byte alpha)
        {
            this.red = red / 255f;
            this.green = green / 255f;
            this.blue = blue / 255f;
            this.alpha = alpha / 255f;
        }

        /// <summary>
        /// Creates a new <see cref="FloatColor"/> structure from an existing <see cref="IColor"/> instance. <br />
        /// The byte values contained by the <see cref="IColor"/> instance are appropriately transformed to floating-point values during construction time.
        /// </summary>
        /// <param name="color">The <see cref="IColor"/> instance to create a new floating-point color from.</param>
        /// <exception cref="ArgumentNullException"><paramref name="color"/> was <see langword="null"/>.</exception>
        [Throws(typeof(ArgumentNullException))]
        public FloatColor(IColor color)
        {
            ArgumentNullException.ThrowIfNull(color);
            red = color.R / 255f;
            green = color.G / 255f;
            blue = color.B / 255f;
            alpha = color.A / 255f;
        }

        /// <summary>
        /// Gets the red channel/component of the color.
        /// </summary>
        public readonly float Red => red;

        /// <summary>
        /// Gets the green channel/component of the color.
        /// </summary>
        public readonly float Green => green;

        /// <summary>
        /// Gets the blue channel/component of the color.
        /// </summary>
        public readonly float Blue => blue;

        /// <summary>
        /// Gets the alpha channel/component of the color.
        /// </summary>
        public readonly float Alpha => alpha;

        /// <summary>
        /// Truncates this <see cref="FloatColor"/> instance to a <see cref="ARGBColor"/> instance.
        /// </summary>
        /// <returns>The truncated result to RGB color intensities.</returns>
        public readonly ARGBColor Truncate() => new((byte)(alpha * 255f), (byte)(red * 255f), (byte)(green * 255f) , (byte)(blue * 255f));

        /// <summary>
        /// Truncates this <see cref="FloatColor"/> instance to a <see cref="RGBAColor"/> instance.
        /// </summary>
        /// <returns>The truncated result to RGB color intensities.</returns>
        readonly RGBAColor ITruncatable<RGBAColor>.Truncate() => new((byte)(red * 255f), (byte)(green * 255f), (byte)(blue * 255f) , (byte)(alpha * 255f));

        /// <summary>
        /// Gets a value whether this <see cref="FloatColor"/> instance has the same color intensities as another <see cref="FloatColor"/> instance.
        /// </summary>
        /// <param name="other">The other instance to compare this instance against.</param>
        /// <returns>A value whether the current and the <paramref name="other"/> structures are considered equal.</returns>
        public readonly bool Equals(FloatColor other) =>
            other.blue == blue && other.green == green && other.red == red && other.alpha == alpha;

        /// <summary>
        /// Gets a value whether this <see cref="FloatColor"/> instance has the same color intensities as the passed <see cref="RGBAColor"/> instance.
        /// </summary>
        /// <param name="other">The other instance to compare this instance against.</param>
        /// <returns>A value whether the current and the <paramref name="other"/> structures are considered equal.</returns>
        public readonly bool Equals(RGBAColor other) => 
            other.R / 255f == red && 
            other.G / 255f == green &&
            other.B / 255f == blue &&
            other.A / 255f == alpha;

        /// <summary>
        /// Gets a value whether this <see cref="FloatColor"/> instance has the same color intensities as the passed <see cref="ARGBColor"/> instance.
        /// </summary>
        /// <param name="other">The other instance to compare this instance against.</param>
        /// <returns>A value whether the current and the <paramref name="other"/> structures are considered equal.</returns>
        public readonly bool Equals(ARGBColor other) =>
            other.R / 255f == red &&
            other.G / 255f == green &&
            other.B / 255f == blue &&
            other.A / 255f == alpha;

        /// <summary>
        /// Gets a value whether this <see cref="FloatColor"/> instance has the same color intensities as the passed <see cref="IColor"/> instance.
        /// </summary>
        /// <param name="other">The other instance to compare this instance against.</param>
        /// <returns>A value whether the current and the <paramref name="other"/> instances are considered equal.</returns>
        public readonly bool Equals(IColor other) => 
            other.R / 255f == red &&
            other.G / 255f == green &&
            other.B / 255f == blue &&
            other.A / 255f == alpha;

        /// <summary>
        /// Gets a value whether this <see cref="FloatColor"/> instance and an object are equal instances.
        /// </summary>
        /// <param name="obj">The other object instance to compare this instance against.</param>
        /// <returns>A value whether the current and the <paramref name="obj"/> objects are considered equal.</returns>
        public readonly override bool Equals(object obj) => obj switch
        {
            FloatColor fc => Equals(fc),
            RGBAColor rgba => Equals(rgba),
            ARGBColor argb => Equals(argb),
            IColor color => Equals(color),
            _ => false,
        };

        /// <summary>Gets a hash code for this <see cref="FloatColor"/> instance.</summary>
        /// <returns>A hash code for this instance.</returns>
        public readonly override int GetHashCode() => (int)((alpha * 255f) + (red * 255f) + (green * 255f) + (blue * 255f)); // Convert to RGB intensities and return that instead, rounded to int.

        /// <summary>Creates a copy this <see cref="FloatColor"/> instance to another instance.</summary>
        /// <returns>A new <see cref="FloatColor"/> instance, but having the same values as this <see cref="FloatColor"/> instance.</returns>
        public readonly FloatColor Clone() => new(red, green, blue, alpha);

        readonly object ICloneable.Clone() => Clone();

        /// <summary>Describes this color instance into a <see cref="String"/>.</summary>
        /// <returns>A <see cref="String"/> describing this color.</returns>
        public readonly override String ToString() => $"FloatColor {{ Red = {red} , Green = {green} , Blue = {blue} , Alpha = {alpha} }}";

        /// <summary>Defines whether two <see cref="FloatColor"/> instances are equal.</summary>
        /// <param name="left">The first <see cref="FloatColor"/> to compare.</param>
        /// <param name="right">The second <see cref="FloatColor"/> to compare.</param>
        /// <returns>A value whether the two passed <see cref="FloatColor"/> instances are indeed equal.</returns>
        public static bool operator ==(FloatColor left, FloatColor right) => left.Equals(right);

        /// <summary>Defines whether two <see cref="FloatColor"/> instances are inequal.</summary>
        /// <param name="left">The first <see cref="FloatColor"/> to compare.</param>
        /// <param name="right">The second <see cref="FloatColor"/> to compare.</param>
        /// <returns>A value whether the two passed <see cref="FloatColor"/> instances are indeed inequal.</returns>
        public static bool operator !=(FloatColor left, FloatColor right) => !left.Equals(right);
    }
}
