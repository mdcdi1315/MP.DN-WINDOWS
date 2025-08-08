

using System;
using System.Numerics;

namespace MP.Graphics
{
    /// <summary>
    /// Defines a two-dimensional integer point.
    /// </summary>
    public readonly struct Point :
        IEqualityOperators<Point, Point, System.Boolean>,
        IAdditionOperators<Point , Point , Point>,
        ISubtractionOperators<Point , Point , Point>,
        IUnaryNegationOperators<Point , Point>,
        ICloneable, IEquatable<Point>
    {
        /// <summary>The X-coordinate of the point.</summary>
        public readonly int X;

        /// <summary>The Y-coordinate of the point.</summary>
        public readonly int Y;

        /// <summary>
        /// Constructs a new <see cref="Point"/> instance, specifying explicitly the x and y coordinates.
        /// </summary>
        /// <param name="X">The X-coordinate of the point.</param>
        /// <param name="Y">The Y-coordinate of the point.</param>
        public Point(int X, int Y)
        {
            this.X = X;
            this.Y = Y;
        }

        readonly System.Object ICloneable.Clone() => Clone();

        /// <summary>
        /// Clones the current <see cref="Point"/> instance.
        /// </summary>
        /// <returns>The cloned <see cref="Point"/> instance.</returns>
        public readonly Point Clone() => new(X, Y);

        /// <summary>
        /// Offsets a point by the specified values.
        /// </summary>
        /// <param name="dx">The times to offset the current X value.</param>
        /// <param name="dy">The times to offset the current Y value.</param>
        /// <returns>A new <see cref="Point"/> instance representing the offseted point.</returns>
        public readonly Point Offset(int dx, int dy) => new(X + dx, Y + dy);

        /// <summary>
        /// Offsets a point by another <see cref="Point"/> instance.
        /// </summary>
        /// <param name="other">The other <see cref="Point"/> instance that the current <see cref="Point"/> will be offseted by.</param>
        /// <returns>A new <see cref="Point"/> instance representing the offseted point.</returns>
        public readonly Point Offset(Point other) => new(X + other.X, Y + other.Y);

        /// <summary>
        /// Calculates the distance of the current and another point and returns their distance as a <see cref="Size"/> structure.
        /// </summary>
        /// <param name="other">The other point to calculate the distance between these two points.</param>
        /// <returns>The distance of the two points as absolute values.</returns>
        public readonly Size Distance(Point other) => new(
           (
            (X < 0) ?
                (
                    (other.X < 0) ?
                        Math.Abs(X - other.X) :
                        (-X + other.X)
                ) :
                (
                    (other.X < 0) ?
                        (-other.X + X) :
                        Math.Abs(X - other.X)
                )
           ),
           (
            (Y < 0) ?
                (
                    (other.Y < 0) ?
                        Math.Abs(Y - other.Y) :
                        (-Y + other.Y)
                ) :
                (
                    (other.Y < 0) ?
                        (-other.Y + Y) :
                        Math.Abs(Y - other.Y)
                )
           )
        );

        /// <summary>
        /// Calculates the distance of the current and another point and returns their distance as a <see cref="Size"/> structure. <br />
        /// This calculation also includes the current point if the points are different.
        /// </summary>
        /// <param name="other">The other point to calculate the distance between these two points.</param>
        /// <returns>The distance of the two points as absolute values.</returns>
        public readonly Size DistanceIncludingThisPoint(Point other)
            => Distance(other) + new Size(X == other.X ? 0 : 1, Y == other.Y ? 0 : 1);
        
        /// <summary>
        /// For debugging purposes only. <br />
        /// Returns the contents of the current structure.
        /// </summary>
        /// <returns>The X and Y coordinates into a nicely formatted string.</returns>
        public readonly override System.String ToString() => $"Point(2D) {{ X = {X} , Y = {Y} }}";

        /// <summary>
        /// Gets a value whether the two <see cref="Point"/> instances are equal. <br />
        /// Two <see cref="Point"/> instances are considered equal if and only if both coordinates are equal.
        /// </summary>
        /// <param name="other">The other <see cref="Point"/> instance to compare this structure against.</param>
        /// <returns>A value representing the equality of both structures.</returns>
        public readonly bool Equals(Point other) => X == other.X && Y == other.Y;

        /// <inheritdoc />
        public readonly override bool Equals(System.Object obj) => obj switch
        {
            Point p => Equals(p),
            _ => false,
        };

        /// <summary>
        /// Gets a hash code for the current point. <br />
        /// The hash code is effectively computed by the sum of both <see cref="X"/> and <see cref="Y"/> fields.
        /// </summary>
        /// <returns>The computed hash code for this instance.</returns>
        public readonly override int GetHashCode() => X + Y;

        /// <summary>
        /// Determines whether two <see cref="Point"/>s are equal.
        /// </summary>
        /// <param name="left">The first <see cref="Point"/> to compare.</param>
        /// <param name="right">The second <see cref="Point"/> to compare.</param>
        /// <returns>A value determining equality of <paramref name="left"/> and <paramref name="right"/>.</returns>
        public static bool operator ==(Point left, Point right) => left.Equals(right);

        /// <summary>
        /// Determines whether two <see cref="Point"/>s are not equal.
        /// </summary>
        /// <param name="left">The first <see cref="Point"/> to compare.</param>
        /// <param name="right">The second <see cref="Point"/> to compare.</param>
        /// <returns>A value determining inequality of <paramref name="left"/> and <paramref name="right"/>.</returns>
        public static bool operator !=(Point left, Point right) => left.X != right.X || left.Y != right.Y;

        /// <summary>
        /// Performs unary negation on the specified <see cref="Point"/>.
        /// </summary>
        /// <param name="point">The <see cref="Point"/> instance to perform unary negation on.</param>
        /// <returns>The unary negation result of <paramref name="point"/>.</returns>
        public static Point operator -(Point point) => new(-point.X, -point.Y);

        /// <summary>
        /// Performs the addition of two <see cref="Point"/> structures.
        /// </summary>
        /// <param name="left">The first <see cref="Point"/> of the sum operation.</param>
        /// <param name="right">The second <see cref="Point"/> of the sum operation.</param>
        /// <returns>The addition result of adding <paramref name="left"/> and <paramref name="right"/>.</returns>
        public static Point operator +(Point left, Point right) => left.Offset(right);

        /// <summary>
        /// Performs the subtraction of two <see cref="Point"/> structures.
        /// </summary>
        /// <param name="left">The first <see cref="Point"/> of the sub operation.</param>
        /// <param name="right">The second <see cref="Point"/> of the sub operation.</param>
        /// <returns>The subtraction result of removing <paramref name="left"/> by <paramref name="right"/>.</returns>
        public static Point operator -(Point left, Point right) => left.Offset(-right);
    }
}