
using System;
using System.Numerics;
using MP.Annotations.CodeAnalysis;
using System.Runtime.InteropServices;

namespace MP.Graphics
{
    /// <summary>
    /// Defines a 16-bit color containing ARGB values. <br />
    /// Data are depicted by the alpha channel first.
    /// </summary>
    [StructLayout(LayoutKind.Explicit, Pack = sizeof(ushort) , Size = sizeof(ushort) * 4)]
    public readonly struct ARGB16Color :
        IEqualityOperators<ARGB16Color, ARGB16Color, bool>,
        ITruncatable<ARGBColor>,
        ITruncatable<RGB16Color>,
        IEquatable<IColor>,
        IEquatable<IColor16>,
        IEquatable<ARGB16Color>,
        ICloneable,
        IColor16
    {
        [FieldOffset(0)]
        private readonly ushort alpha;

        [FieldOffset(sizeof(ushort))]
        private readonly ushort red;

        [FieldOffset(sizeof(ushort) * 2)]
        private readonly ushort green;

        [FieldOffset(sizeof(ushort) * 3)]
        private readonly ushort blue;

        /// <summary>
        /// Creates an ARGB color from the specified red, green, blue and alpha channels.
        /// </summary>
        /// <param name="red">The red channel value.</param>
        /// <param name="green">The green channel value.</param>
        /// <param name="blue">The blue channel value.</param>
        /// <param name="alpha">The alpha channel value.</param>
        [MustNotReportException]
        public ARGB16Color(ushort alpha, ushort red, ushort green, ushort blue)
        {
            this.alpha = alpha;
            this.red = red;
            this.green = green;
            this.blue = blue;
        }

        /// <inheritdoc/>
        public readonly ushort A => alpha;

        /// <inheritdoc/>
        public readonly ushort R => red;

        /// <inheritdoc/>
        public readonly ushort G => green;

        /// <inheritdoc/>
        public readonly ushort B => blue;

        /// <summary>
        /// Gets a value whether this instance has the same color intensities as another <see cref="ARGB16Color"/> instance.
        /// </summary>
        /// <param name="other">The other instance to compare this instance against.</param>
        /// <returns>A value whether the current and the <paramref name="other"/> structures are considered equal.</returns>
        public readonly bool Equals(ARGB16Color other) => 
            alpha == other.alpha && 
            red == other.red &&
            green == other.green && 
            blue == other.blue;

        /// <summary>
        /// Gets a value whether this instance has the same color intensities as any object supporting the <see cref="IColor"/> interface.
        /// </summary>
        /// <param name="other">The other instance to compare this instance against.</param>
        /// <returns>A value whether the current and the <paramref name="other"/> objects are considered equal.</returns>
        public readonly bool Equals(IColor other) =>
            alpha == other.A * 257 &&
            red == other.R * 257 &&
            green == other.G * 257 &&
            blue == other.B * 257;

        /// <summary>
        /// Gets a value whether this instance has the same color intensities as any object supporting the <see cref="IColor16"/> interface.
        /// </summary>
        /// <param name="other">The other instance to compare this instance against.</param>
        /// <returns>A value whether the current and the <paramref name="other"/> objects are considered equal.</returns>
        public readonly bool Equals(IColor16 other) =>
            alpha == other.A &&
            red == other.R &&
            green == other.G &&
            blue == other.B;

        /// <summary>
        /// Gets a value whether this <see cref="RGBAColor"/> instance and an object are equal instances.
        /// </summary>
        /// <param name="obj">The other object instance to compare this instance against.</param>
        /// <returns>A value whether the current and the <paramref name="obj"/> objects are considered equal.</returns>
        public readonly override bool Equals(object obj) => obj switch 
        { 
            ARGB16Color c => Equals(c),
            IColor c => Equals(c),
            IColor16 c => Equals(c),
            _ => false
        };

        /// <summary>Creates a copy of this <see cref="ARGB16Color"/> instance to a new instance.</summary>
        /// <returns>A new instance of the <see cref="ARGB16Color"/> structure, having the same color intensities as this <see cref="ARGB16Color"/> instance.</returns>
        public readonly ARGB16Color Clone() => new(alpha, red, green, blue);

        readonly object ICloneable.Clone() => Clone();

        /// <summary>This method call is not supported and will always throw <see cref="NotSupportedException"/>.</summary>
        /// <exception cref="NotSupportedException">Not supported on 16-bit colors.</exception>
        public override int GetHashCode() => throw new NotSupportedException("16-bit color values require a packed long integer to properly distinguish their values.");

        /// <summary>Gets a string describing this color.</summary>
        /// <returns>The color description.</returns>
        public readonly override System.String ToString() => $"Color<ARGB>(16-bit) {{ Red: {red} , Green: {green} , Blue: {blue} , Alpha: {alpha} }}";

        ARGBColor ITruncatable<ARGBColor>.Truncate() => new((alpha / 257).ToByte() , (red / 257).ToByte() , (green / 257).ToByte() , (blue / 257).ToByte());

        /// <summary>
        /// Truncates the current <see cref="ARGB16Color"/> instance to an instance of type <see cref="RGB16Color"/>, omitting the alpha channel in the returned value.
        /// </summary>
        /// <returns>The truncated result.</returns>
        public RGB16Color Truncate() => new(red , green , blue);

        /// <summary>Defines whether two <see cref="ARGB16Color"/> instances are equal.</summary>
        /// <param name="left">The first <see cref="ARGB16Color"/> to compare.</param>
        /// <param name="right">The second <see cref="ARGB16Color"/> to compare.</param>
        /// <returns>A value whether the two passed <see cref="ARGB16Color"/> instances are indeed equal.</returns>
        public static bool operator ==(ARGB16Color left, ARGB16Color right) => left.Equals(right);

        /// <summary>Defines whether two <see cref="ARGB16Color"/> instances are inequal.</summary>
        /// <param name="left">The first <see cref="ARGB16Color"/> to compare.</param>
        /// <param name="right">The second <see cref="ARGB16Color"/> to compare.</param>
        /// <returns>A value whether the two passed <see cref="ARGB16Color"/> instances are indeed inequal.</returns>
        public static bool operator !=(ARGB16Color left, ARGB16Color right) => !left.Equals(right);
    }
}