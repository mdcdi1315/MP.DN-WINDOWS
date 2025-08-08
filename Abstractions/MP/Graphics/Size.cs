

using System;
using System.Numerics;

namespace MP.Graphics
{
    /// <summary>
    /// Defines a two-dimensional size offsetting from point (0 , 0)
    /// </summary>
    public readonly struct Size :
        IEqualityOperators<Size, Size, System.Boolean>,
        IAdditionOperators<Size , Size , Size>,
        ISubtractionOperators<Size , Size , Size>,
        IUnaryNegationOperators<Size , Size>,
        ICloneable, IEquatable<Size>
    {
        /// <summary>
        /// The width or the displacement starting from X = 0 to the current value.
        /// </summary>
        public readonly int Width;

        /// <summary>
        /// The height or the displacement starting from Y = 0 to the current value.
        /// </summary>
        public readonly int Height;

        /// <summary>
        /// Constructs a new <see cref="Size"/> instance by explicitly specifying the X and Y displacements from point (0 , 0).
        /// </summary>
        /// <param name="width">The desired X displacement.</param>
        /// <param name="height">The desired Y displacement.</param>
        public Size(int width, int height)
        {
            Width = width;
            Height = height;
        }

        /// <summary>
        /// Constructs a new <see cref="Size"/> instance from a point, assuming that the central coordinate system that the point represents has as first point the (0 , 0).
        /// </summary>
        /// <param name="point">The <see cref="Point"/> to create this instance from.</param>
        public Size(Point point)
        {
            Width = point.X;
            Height = point.Y;
        }

        readonly System.Object ICloneable.Clone() => Clone();

        /// <summary>
        /// Clones the current <see cref="Size"/> instance.
        /// </summary>
        /// <returns>The cloned <see cref="Size"/> instance.</returns>
        public readonly Size Clone() => new(Width, Height);

        /// <summary>
        /// For debugging purposes only. <br />
        /// Returns the contents of the current structure.
        /// </summary>
        /// <returns>The width and height into a nicely formatted string.</returns>
        public readonly override System.String ToString() => $"Size(2D) {{ Width = {Width} , Height = {Height} }}";

        /// <summary>
        /// Gets a value whether the two <see cref="Size"/> instances are equal. <br />
        /// Two <see cref="Size"/> instances are considered equal if and only if both coordinates are equal.
        /// </summary>
        /// <param name="other">The other <see cref="Size"/> instance to compare this structure against.</param>
        /// <returns>A value representing the equality of both structures.</returns>
        public readonly bool Equals(Size other) => Width == other.Width && Height == other.Height;

        /// <inheritdoc />
        public readonly override bool Equals(System.Object obj) => obj switch
        {
            Size p => Equals(p),
            _ => false,
        };

        /// <summary>
        /// Gets a hash code for the current size. <br />
        /// The hash code is effectively computed by the sum of both <see cref="Width"/> and <see cref="Height"/> fields.
        /// </summary>
        /// <returns>The computed hash code for this instance.</returns>
        public readonly override int GetHashCode() => Width + Height;

        /// <summary>
        /// Determines whether two <see cref="Size"/>s are equal.
        /// </summary>
        /// <param name="left">The first <see cref="Size"/> to compare.</param>
        /// <param name="right">The second <see cref="Size"/> to compare.</param>
        /// <returns>A value determining equality of <paramref name="left"/> and <paramref name="right"/>.</returns>
        public static bool operator ==(Size left, Size right) => left.Equals(right);

        /// <summary>
        /// Determines whether two <see cref="Size"/>s are not equal.
        /// </summary>
        /// <param name="left">The first <see cref="Size"/> to compare.</param>
        /// <param name="right">The second <see cref="Size"/> to compare.</param>
        /// <returns>A value determining inequality of <paramref name="left"/> and <paramref name="right"/>.</returns>
        public static bool operator !=(Size left, Size right) => left.Width != right.Width || left.Height != right.Height;

        /// <summary>
        /// Performs unary negation on the specified <see cref="Size"/>.
        /// </summary>
        /// <param name="size">The <see cref="Size"/> instance to perform unary negation on.</param>
        /// <returns>The unary negation result of <paramref name="size"/>.</returns>
        public static Size operator -(Size size) => new(-size.Width, -size.Height);
        
        /// <summary>
        /// Performs the addition of two <see cref="Size"/> structures.
        /// </summary>
        /// <param name="left">The first <see cref="Size"/> of the sum operation.</param>
        /// <param name="right">The second <see cref="Size"/> of the sum operation.</param>
        /// <returns>The addition result of adding <paramref name="left"/> and <paramref name="right"/>.</returns>
        public static Size operator +(Size left, Size right) => new(left.Width + right.Width , left.Height + right.Height);

        /// <summary>
        /// Performs the subtraction of two <see cref="Size"/> structures.
        /// </summary>
        /// <param name="left">The first <see cref="Size"/> of the sub operation.</param>
        /// <param name="right">The second <see cref="Size"/> of the sub operation.</param>
        /// <returns>The subtraction result of removing <paramref name="left"/> by <paramref name="right"/>.</returns>
        public static Size operator -(Size left, Size right) => new(left.Width - right.Width , left.Height - right.Height);
    }
}