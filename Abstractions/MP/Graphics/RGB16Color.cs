
using System;
using System.Numerics;
using MP.Annotations.CodeAnalysis;
using System.Runtime.InteropServices;

namespace MP.Graphics
{
    /// <summary>
    /// Defines a 16-bit-color containing RGB values. <br />
    /// Data are depicted by the red channel first.
    /// </summary>
    [StructLayout(LayoutKind.Explicit, Pack = sizeof(ushort), Size = sizeof(ushort) * 3)]
    public readonly struct RGB16Color : 
        IEqualityOperators<RGB16Color, RGB16Color, bool>,
        IEquatable<IColor>,
        IEquatable<IColor16>,
        IEquatable<RGBColor>,
        IEquatable<RGB16Color>,
        ITruncatable<RGBColor>,
        ICloneable,
        IColor16
    {
        [FieldOffset(0)]
        private readonly ushort red;

        [FieldOffset(sizeof(ushort))]
        private readonly ushort green;

        [FieldOffset(sizeof(ushort) * 2)]
        private readonly ushort blue;

        /// <summary>
        /// Creates a RGB color from the specified red, green and blue channels.
        /// </summary>
        /// <param name="red">The red channel value.</param>
        /// <param name="green">The green channel value.</param>
        /// <param name="blue">The blue channel value.</param>
        [MustNotReportException]
        public RGB16Color(ushort red , ushort green, ushort blue)
        {
            this.red = red;
            this.green = green;
            this.blue = blue;
        }

        /// <inheritdoc/>
        public readonly ushort A => ushort.MaxValue;

        /// <inheritdoc/>
        public readonly ushort R => red;

        /// <inheritdoc/>
        public readonly ushort G => green;

        /// <inheritdoc/>
        public readonly ushort B => blue;

        /// <summary>
        /// Gets a value whether this instance has the same color intensities as another <see cref="RGBColor"/> instance.
        /// </summary>
        /// <param name="other">The other instance to compare this instance against.</param>
        /// <returns>A value whether the current and the <paramref name="other"/> structures are considered equal.</returns>
        public readonly bool Equals(RGBColor other) => other.B * 257 == blue && other.G * 257 == green && other.R * 257 == red;

        /// <summary>
        /// Gets a value whether this instance has the same color intensities as another <see cref="RGB16Color"/> instance.
        /// </summary>
        /// <param name="other">The other instance to compare this instance against.</param>
        /// <returns>A value whether the current and the <paramref name="other"/> structures are considered equal.</returns>
        public readonly bool Equals(RGB16Color other) => other.red == red && other.green == green && other.blue == blue;

         /// <summary>
        /// Gets a value whether this instance has the same color intensities as any object supporting the <see cref="IColor"/> interface.
        /// </summary>
        /// <param name="other">The other instance to compare this instance against.</param>
        /// <returns>A value whether the current and the <paramref name="other"/> objects are considered equal.</returns>
        public readonly bool Equals(IColor other) => other.B * 257 == blue && other.G * 257 == green && other.R * 257 == red;

         /// <summary>
        /// Gets a value whether this instance has the same color intensities as any object supporting the <see cref="IColor16"/> interface.
        /// </summary>
        /// <param name="other">The other instance to compare this instance against.</param>
        /// <returns>A value whether the current and the <paramref name="other"/> objects are considered equal.</returns>
        public readonly bool Equals(IColor16 other) => other.R == red && other.G == green && other.B == blue;

        /// <summary>
        /// Gets a value whether this <see cref="BGRColor"/> instance and an object are equal instances.
        /// </summary>
        /// <param name="obj">The other object instance to compare this instance against.</param>
        /// <returns>A value whether the current and the <paramref name="obj"/> objects are considered equal.</returns>
        public override readonly bool Equals(object obj) => obj switch
        {
            RGB16Color c => Equals(c),
            RGBColor c => Equals(c),
            IColor cg => Equals(cg),
            IColor16 c => Equals(c),
            _ => false,
        };

        /// <summary>Creates a copy of this <see cref="RGBColor"/> instance to a new instance.</summary>
        /// <returns>A new instance of the <see cref="RGBColor"/> structure, having the same color intensities as this <see cref="RGBColor"/> instance.</returns>
        public readonly RGB16Color Clone() => new(red , green , blue);

        readonly System.Object ICloneable.Clone() => Clone();

        /// <summary>
        /// Truncates this instance to an instance of type <see cref="RGBColor"/>.
        /// </summary>
        /// <returns>An instance of the <see cref="RGBColor"/> structure.</returns>
        public RGBColor Truncate() => new((red / 257).ToByte(), (green / 257).ToByte(), (blue / 257).ToByte());

        /// <summary>This method call is not supported and will always throw <see cref="NotSupportedException"/>.</summary>
        /// <exception cref="NotSupportedException">Not supported on 16-bit colors.</exception>
        public readonly override int GetHashCode() => throw new NotSupportedException("16-bit color values require a packed long integer to properly distinguish their values.");

        /// <summary>Gets a string describing this color.</summary>
        /// <returns>The color description.</returns>
        public readonly override System.String ToString() => $"Color<RGB>(16-bit) {{ Red: {red} , Green: {green} , Blue: {blue} }}";

        /// <summary>Defines whether two <see cref="RGB16Color"/> instances are equal.</summary>
        /// <param name="left">The first <see cref="RGB16Color"/> to compare.</param>
        /// <param name="right">The second <see cref="RGB16Color"/> to compare.</param>
        /// <returns>A value whether the two passed <see cref="RGB16Color"/> instances are indeed equal.</returns>
        public static bool operator ==(RGB16Color left, RGB16Color right) => left.Equals(right);

        /// <summary>Defines whether two <see cref="RGB16Color"/> instances are inequal.</summary>
        /// <param name="left">The first <see cref="RGB16Color"/> to compare.</param>
        /// <param name="right">The second <see cref="RGB16Color"/> to compare.</param>
        /// <returns>A value whether the two passed <see cref="RGB16Color"/> instances are indeed inequal.</returns>
        public static bool operator !=(RGB16Color left, RGB16Color right) => !left.Equals(right);
    }
}