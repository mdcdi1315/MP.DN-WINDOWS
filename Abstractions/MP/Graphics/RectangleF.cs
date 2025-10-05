
using System;
using System.Numerics;

namespace MP.Graphics
{
    /// <summary>
    /// Defines a new rectangle in a 2-dimensional space. <br />
    /// It is expressed as floating-point numbers instead.
    /// </summary>
    public readonly struct RectangleF :
        IEqualityOperators<RectangleF, RectangleF, System.Boolean>,
        IEqualityOperators<RectangleF , Rectangle , System.Boolean>,
        ITruncatable<Rectangle>,
        IEquatable<RectangleF>,
        IEquatable<Rectangle>,
        ICloneable
    {
        private readonly PointF topleft, topright;
        private readonly PointF bottomleft, bottomright;

        // Constructor allowing for cloning
        private RectangleF(RectangleF rect)
        {
            topleft = rect.topleft;
            topright = rect.topright;
            bottomleft = rect.bottomleft;
            bottomright = rect.bottomright;
        }

        /// <summary>
        /// Creates a new rectangle by just specifying the bottom-left and top-right corner points of the rectangle.
        /// </summary>
        /// <param name="bottomleft">The bottom-left corner point of the rectangle.</param>
        /// <param name="topright">The top-right corner point of the rectangle.</param>
        public RectangleF(PointF bottomleft, PointF topright)
        {
            this.bottomleft = bottomleft;
            this.topright = topright;
            bottomright = new(topright.X, bottomleft.Y);
            topleft = new(bottomleft.X, topright.Y);
        }

        /// <summary>
        /// Creates a new rectangle by specifying the bottom-left corner and the desired size of the new rectangle.
        /// </summary>
        /// <param name="bottomleft">The bottom-left corner point of the rectangle.</param>
        /// <param name="rectsize">The desired size of the rectangle.</param>
        public RectangleF(PointF bottomleft, SizeF rectsize)
        {
            this.bottomleft = bottomleft;
            bottomright = new(bottomleft.X + rectsize.Width, bottomleft.Y);
            topright = new(bottomright.X, bottomleft.Y + rectsize.Height);
            topleft = new(bottomleft.X, topright.Y);
        }

        readonly System.Object ICloneable.Clone() => Clone();

        /// <summary>Clones this <see cref="RectangleF"/> instance to another instance of <see cref="RectangleF"/>.</summary>
        /// <returns>The cloned <see cref="RectangleF"/> instance.</returns>
        public readonly RectangleF Clone() => new(this);

        /// <summary>
        /// Gets a value whether this rectangle is in the same size and coordinates with another <see cref="RectangleF"/>.
        /// </summary>
        /// <param name="other">The other rectangle to test for equality.</param>
        /// <returns>A value whether the two rectangles are the same rectangles.</returns>
        public readonly bool Equals(RectangleF other) =>
            bottomleft == other.bottomleft &&
            bottomright == other.bottomright &&
            topleft == other.topleft &&
            bottomright == other.topright;

        /// <summary>
        /// Gets a value whether this rectangle is in the same size and coordinates with another <see cref="Rectangle"/>.
        /// </summary>
        /// <param name="other">The other rectangle to test for equality.</param>
        /// <returns>A value whether the two rectangles are the same rectangles.</returns>
        public readonly bool Equals(Rectangle other) =>
            bottomleft == other.BottomLeft &&
            bottomright == other.BottomRight &&
            topleft == other.TopLeft &&
            bottomright == other.TopRight;

        /// <inheritdoc />
        public readonly override bool Equals(object obj) => obj switch { 
            Rectangle rc => Equals(rc),
            RectangleF rcf => Equals(rcf),
            _ => false,
        };

        /// <summary>Gets a value whether the specified point is in the rectangle's bounds.</summary>
        /// <param name="point">The point to see whether is in the rectangle's bounds.</param>
        /// <returns>A value whether the <paramref name="point"/> passed is in the rectangle's bounds.</returns>
        public System.Boolean PointIsInBounds(PointF point) => point.X >= topleft.X && point.Y <= topleft.Y && point.X <= bottomright.X && point.Y >= bottomright.Y;

        /// <summary>Gets a value whether the specified point is in the rectangle's bounds.</summary>
        /// <param name="point">The point to see whether is in the rectangle's bounds.</param>
        /// <returns>A value whether the <paramref name="point"/> passed is in the rectangle's bounds.</returns>
        public System.Boolean PointIsInBounds(Point point) => point.X >= topleft.X && point.Y <= topleft.Y && point.X <= bottomright.X && point.Y >= bottomright.Y;

        /// <inheritdoc />
        public readonly override int GetHashCode() => bottomleft.GetHashCode() + bottomright.GetHashCode() + topleft.GetHashCode() + topright.GetHashCode();

        /// <inheritdoc />
        public Rectangle Truncate() => new(bottomleft.Truncate() , topright.Truncate());

        /// <summary>
        /// Gets the bottom-left point of the rectangle.
        /// </summary>
        public readonly PointF BottomLeft => bottomleft;

        /// <summary>
        /// Gets the bottom-right point of the rectangle.
        /// </summary>
        public readonly PointF BottomRight => bottomright;

        /// <summary>
        /// Gets the top-left point of the rectangle.
        /// </summary>
        public readonly PointF TopLeft => topleft;

        /// <summary>
        /// Gets the top-right point of the rectangle.
        /// </summary>
        public readonly PointF TopRight => topright;

        /// <summary>
        /// Gets the lower-left triangle, which is the half of the rectangle. <br />
        /// The triangle is always orthogonal.
        /// </summary>
        public readonly TriangleF LowerLeftTriangle => TriangleF.CreateOrthogonal(bottomleft, topright);

        /// <summary>
        /// Gets the upper-right triangle, which is the half of the rectangle. <br />
        /// The triangle is always orthogonal.
        /// </summary>
        public readonly TriangleF UpperRightTriangle => TriangleF.CreateOrthogonal(topright, bottomleft);

        /// <summary>
        /// Determines whether two <see cref="RectangleF"/>s are equal.
        /// </summary>
        /// <param name="left">The first <see cref="RectangleF"/> to compare.</param>
        /// <param name="right">The second <see cref="RectangleF"/> to compare.</param>
        /// <returns>A value determining equality of <paramref name="left"/> and <paramref name="right"/>.</returns>
        public static bool operator ==(RectangleF left, RectangleF right) => left.Equals(right);

        /// <summary>
        /// Determines whether two <see cref="RectangleF"/>s are inequal.
        /// </summary>
        /// <param name="left">The first <see cref="RectangleF"/> to compare.</param>
        /// <param name="right">The second <see cref="RectangleF"/> to compare.</param>
        /// <returns>A value determining inequality of <paramref name="left"/> and <paramref name="right"/>.</returns>
        public static bool operator !=(RectangleF left, RectangleF right) => !left.Equals(right);

        /// <summary>
        /// Determines whether a <see cref="RectangleF"/> and a <see cref="Rectangle"/> are equal.
        /// </summary>
        /// <param name="left">The first <see cref="RectangleF"/> to compare.</param>
        /// <param name="right">The second <see cref="Rectangle"/> to compare.</param>
        /// <returns>A value determining equality of <paramref name="left"/> and <paramref name="right"/>.</returns>
        public static bool operator ==(RectangleF left, Rectangle right) => left.Equals(right);

        /// <summary>
        /// Determines whether a <see cref="RectangleF"/> and a <see cref="Rectangle"/> are inequal.
        /// </summary>
        /// <param name="left">The first <see cref="RectangleF"/> to compare.</param>
        /// <param name="right">The second <see cref="Rectangle"/> to compare.</param>
        /// <returns>A value determining inequality of <paramref name="left"/> and <paramref name="right"/>.</returns>
        public static bool operator !=(RectangleF left, Rectangle right) => !left.Equals(right);
    }
}