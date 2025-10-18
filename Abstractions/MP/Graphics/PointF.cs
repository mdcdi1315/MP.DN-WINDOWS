
using System;
using System.Numerics;
using System.Runtime.InteropServices;

namespace MP.Graphics
{
    /// <summary>
    /// Defines a two-dimensional single-presicion floating integer point.
    /// </summary>
    [StructLayout(LayoutKind.Explicit, Pack = sizeof(float), Size = sizeof(float) * 2)]
    public readonly struct PointF :
        IEqualityOperators<PointF, PointF, System.Boolean>,
        IEqualityOperators<PointF , Point , System.Boolean>,
        IAdditionOperators<PointF, PointF, PointF>,
        IAdditionOperators<PointF , Point , PointF>,
        ISubtractionOperators<PointF, PointF, PointF>,
        ISubtractionOperators<PointF , Point , PointF>,
        IUnaryNegationOperators<PointF, PointF>,
        ITruncatable<Point>,
        IEquatable<PointF>, 
        IEquatable<Point>,
        ICloneable
    {
        /// <summary>The X-coordinate of the point.</summary>
        [FieldOffset(0)]
        public readonly float X;

        /// <summary>The Y-coordinate of the point.</summary>
        [FieldOffset(sizeof(float))]
        public readonly float Y;

        /// <summary>
        /// Constructs a new <see cref="PointF"/> instance, specifying explicitly the x and y coordinates. <br />
        /// This constructor variant allows to create a <see cref="PointF"/> instance from two integers instead.
        /// </summary>
        /// <param name="X">The X-coordinate of the point.</param>
        /// <param name="Y">The Y-coordinate of the point.</param>
        public PointF(int X, int Y)
        {
            this.X = X;
            this.Y = Y;
        }

        /// <summary>
        /// Constructs a new <see cref="PointF"/> instance, specifying explicitly the x and y coordinates. 
        /// </summary>
        /// <param name="X">The X-coordinate of the point.</param>
        /// <param name="Y">The Y-coordinate of the point.</param>
        public PointF(float X, float Y)
        {
            this.X = X;
            this.Y = Y;
        }

        /// <summary>
        /// Constructs a new <see cref="PointF"/> instance, from another <see cref="Point"/> instance. This cast is always safe.
        /// </summary>
        /// <param name="p">The <see cref="Point"/> instance to get the coordinates from.</param>
        public PointF(Point p)
        {
            X = p.X;
            Y = p.Y;
        }

        readonly System.Object ICloneable.Clone() => Clone();

        /// <summary>
        /// Clones the current <see cref="Point"/> instance.
        /// </summary>
        /// <returns>The cloned <see cref="Point"/> instance.</returns>
        public readonly PointF Clone() => new(X, Y);

        /// <summary>Offsets a point by the specified values.</summary>
        /// <param name="dx">The times to offset the current X value.</param>
        /// <param name="dy">The times to offset the current Y value.</param>
        /// <returns>A new <see cref="Point"/> instance representing the offseted point.</returns>
        public readonly PointF Offset(int dx, int dy) => new(X + dx, Y + dy);

        /// <summary>
        /// Offsets a point diagnonally. <br />
        /// This is like calling <see cref="Offset(int, int)"/> passing to both arguments the <paramref name="offset"/> parameter.
        /// </summary>
        /// <param name="offset">The displacement offset to use.</param>
        /// <returns>A new <see cref="PointF"/> instance representing the diagonally offseted point.</returns>
        public readonly PointF OffsetDiagonally(int offset) => new(X + offset, Y + offset);

        /// <summary>Offsets a point by the specified values.</summary>
        /// <param name="dx">The times to offset the current X value.</param>
        /// <param name="dy">The times to offset the current Y value.</param>
        /// <returns>A new <see cref="PointF"/> instance representing the offseted point.</returns>
        public readonly PointF Offset(float dx, float dy) => new(X + dx, Y + dy);

        /// <summary>
        /// Offsets a point diagnonally. <br />
        /// This is like calling <see cref="Offset(float, float)"/> passing to both arguments the <paramref name="offset"/> parameter.
        /// </summary>
        /// <param name="offset">The displacement offset to use.</param>
        /// <returns>A new <see cref="PointF"/> instance representing the diagonally offseted point.</returns>
        public readonly PointF OffsetDiagonally(float offset) => new(X + offset, Y + offset);

        /// <summary>Offsets a point by another <see cref="Point"/> instance.</summary>
        /// <param name="other">The other <see cref="Point"/> instance that the current <see cref="PointF"/> will be offseted by.</param>
        /// <returns>A new <see cref="PointF"/> instance representing the offseted point.</returns>
        public readonly PointF Offset(Point other) => new(X + other.X, Y + other.Y);

        /// <summary>Offsets a point by another <see cref="PointF"/> instance.</summary>
        /// <param name="other">The other <see cref="PointF"/> instance that the current <see cref="PointF"/> will be offseted by.</param>
        /// <returns>A new <see cref="Point"/> instance representing the offseted point.</returns>
        public readonly PointF Offset(PointF other) => new(X + other.X, Y + other.Y);

        /// <summary>Offsets a point by a <see cref="Size"/> instance.</summary>
        /// <param name="other">The <see cref="Size"/> instance to offset this point by.</param>
        /// <returns>A new <see cref="PointF"/> structure representing the offseted point by adding the specified size.</returns>
        public readonly PointF Offset(Size other) => new(X + other.Width , Y + other.Height);

        /// <summary>Offsets a point by a <see cref="SizeF"/> instance.</summary>
        /// <param name="other">The <see cref="SizeF"/> instance to offset this point by.</param>
        /// <returns>A new <see cref="PointF"/> structure representing the offseted point by adding the specified size.</returns>
        public readonly PointF Offset(SizeF other) => new(X + other.Width, Y + other.Height);

        /// <summary>
        /// For debugging purposes only. <br />
        /// Returns the contents of the current structure.
        /// </summary>
        /// <returns>The X and Y coordinates into a nicely formatted string.</returns>
        public readonly override System.String ToString() => $"PointF(2D) {{ X = {X} , Y = {Y} }}";

        /// <inheritdoc />
        public readonly override bool Equals(System.Object obj) => obj switch
        {
            Point p => Equals(p),
            PointF pf => Equals(pf),
            _ => false,
        };

        /// <summary>
        /// Gets a value whether the two <see cref="PointF"/> instances are equal. <br />
        /// Two <see cref="PointF"/> instances are considered equal if and only if both coordinates are equal.
        /// </summary>
        /// <param name="other">The other <see cref="PointF"/> instance to compare this structure against.</param>
        /// <returns>A value representing the equality of both structures.</returns>
        public bool Equals(PointF other) => X == other.X && Y == other.Y;

        /// <summary>
        /// Gets a value whether a <see cref="Point"/> instance and the current one are equal. <br />
        /// They are considered equal if and only if both coordinates are equal.
        /// </summary>
        /// <param name="other">The other <see cref="Point"/> instance to compare this structure against.</param>
        /// <returns>A value representing the equality of both structures.</returns>
        /// <remarks>
        /// This equality method is useful when you want to compare for equality a <see cref="Point"/>
        /// and a <see cref="PointF"/> instance, without needing to cast to <see cref="PointF"/> first.
        /// </remarks>
        public bool Equals(Point other) => X == other.X && Y == other.Y;

        /// <summary>
        /// Gets a hash code for the current point. <br />
        /// The hash code is effectively computed by the sum of both <see cref="X"/> and <see cref="Y"/> fields.
        /// </summary>
        /// <returns>The computed hash code for this instance.</returns>
        public readonly override int GetHashCode() => (System.Int32)(X + Y);

        /// <inheritdoc />
        public Point Truncate() => new((System.Int32)X, (System.Int32)Y);

        /// <summary>
        /// Performs the addition of two <see cref="PointF"/> structures.
        /// </summary>
        /// <param name="left">The first <see cref="PointF"/> of the sum operation.</param>
        /// <param name="right">The second <see cref="PointF"/> of the sum operation.</param>
        /// <returns>The addition result of adding <paramref name="left"/> and <paramref name="right"/>.</returns>
        public static PointF operator +(PointF left, PointF right) => left.Offset(right);

        /// <summary>
        /// Performs the addition of a <see cref="PointF"/> and <see cref="Point"/> 
        /// structure, returning the result as a <see cref="PointF"/> structure.
        /// </summary>
        /// <param name="left">The first <see cref="PointF"/> of the sum operation.</param>
        /// <param name="right">The second <see cref="Point"/> of the sum operation.</param>
        /// <returns>The addition result of adding <paramref name="left"/> and <paramref name="right"/>.</returns>
        public static PointF operator +(PointF left, Point right) => new(left.X + right.X, left.Y + right.Y);

        /// <summary>Performs unary negation on the specified <see cref="PointF"/>.</summary>
        /// <param name="point">The <see cref="PointF"/> instance to perform unary negation on.</param>
        /// <returns>The unary negation result of <paramref name="point"/>.</returns>
        public static PointF operator -(PointF point) => new(-point.X, -point.Y);

        /// <summary>
        /// Performs the subtraction of two <see cref="PointF"/> structures.
        /// </summary>
        /// <param name="left">The first <see cref="PointF"/> of the sub operation.</param>
        /// <param name="right">The second <see cref="PointF"/> of the sub operation.</param>
        /// <returns>The subtraction result of removing <paramref name="left"/> by <paramref name="right"/>.</returns>
        public static PointF operator -(PointF left, PointF right) => left.Offset(-right);

        /// <summary>
        /// Performs the subtraction of a <see cref="PointF"/> and <see cref="Point"/> 
        /// structure, returning the result as a <see cref="PointF"/> structure.
        /// </summary>
        /// <param name="left">The first <see cref="PointF"/> of the sum operation.</param>
        /// <param name="right">The second <see cref="Point"/> of the sum operation.</param>
        /// <returns>The subtraction result of removing <paramref name="left"/> and <paramref name="right"/>.</returns>
        public static PointF operator -(PointF left, Point right) => new(left.X - right.X , left.Y - right.Y);

        /// <summary>
        /// Determines whether two <see cref="PointF"/>s are equal.
        /// </summary>
        /// <param name="left">The first <see cref="PointF"/> to compare.</param>
        /// <param name="right">The second <see cref="PointF"/> to compare.</param>
        /// <returns>A value determining equality of <paramref name="left"/> and <paramref name="right"/>.</returns>
        public static bool operator ==(PointF left, PointF right) => left.Equals(right);

        /// <summary>
        /// Determines whether two <see cref="PointF"/>s are inequal.
        /// </summary>
        /// <param name="left">The first <see cref="PointF"/> to compare.</param>
        /// <param name="right">The second <see cref="PointF"/> to compare.</param>
        /// <returns>A value determining equality of <paramref name="left"/> and <paramref name="right"/>.</returns>
        public static bool operator !=(PointF left, PointF right) => !left.Equals(right);

        /// <summary>
        /// Implcitly converts a <see cref="Point"/> to a <see cref="PointF"/> instance. <br />
        /// See <see cref="PointF(Point)"/> constructor for more information.
        /// </summary>
        /// <param name="point">The <see cref="Point"/> to convert as a <see cref="PointF"/> instance.</param>
        public static implicit operator PointF(Point point) => new(point);

        /// <summary>
        /// Determines whether a <see cref="PointF"/> and a <see cref="Point"/> are equal.
        /// </summary>
        /// <param name="left">The first <see cref="PointF"/> to compare.</param>
        /// <param name="right">The second <see cref="Point"/> to compare.</param>
        /// <returns>A value determining equality of <paramref name="left"/> and <paramref name="right"/>.</returns>
        public static bool operator ==(PointF left, Point right) => left.Equals(right);

        /// <summary>
        /// Determines whether a <see cref="PointF"/> and a <see cref="Point"/> are inequal.
        /// </summary>
        /// <param name="left">The first <see cref="PointF"/> to compare.</param>
        /// <param name="right">The second <see cref="Point"/> to compare.</param>
        /// <returns>A value determining equality of <paramref name="left"/> and <paramref name="right"/>.</returns>
        public static bool operator !=(PointF left, Point right) => !left.Equals(right);
    }
}