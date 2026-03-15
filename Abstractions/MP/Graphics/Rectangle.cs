
using System;
using System.Numerics;

namespace MP.Graphics
{
    /// <summary>
    /// Defines a new rectangle in a 2-dimensional space.
    /// </summary>
    public readonly struct Rectangle : 
        IEqualityOperators<Rectangle , Rectangle , System.Boolean>,
        IEquatable<Rectangle>,
        ICloneable
    {
        private readonly Point topleft, topright;
        private readonly Point bottomleft, bottomright;

        // Constructor allowing for cloning
        private Rectangle(Rectangle rect)
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
        public Rectangle(Point bottomleft, Point topright)
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
        public Rectangle(Point bottomleft, Size rectsize)
        {
            this.bottomleft = bottomleft;
            bottomright = new(bottomleft.X + rectsize.Width, bottomleft.Y);
            topright = new(bottomright.X, bottomleft.Y + rectsize.Height);
            topleft = new(bottomleft.X, topright.Y);
        }

        /// <summary>
        /// Creates a new rectangle by specifying the top-left corner and the desired size of the new rectangle.
        /// </summary>
        /// <param name="top_left">The top left corner coordinates.</param>
        /// <param name="rect_size">The desired size of the rectangle.</param>
        /// <returns>The created <see cref="Rectangle"/>.</returns>
        public static Rectangle FromTopLeft(Point top_left, Size rect_size)
        {
            Point bottom_left = new(top_left.X , top_left.Y - rect_size.Height);
            Point top_right = new(top_left.X + rect_size.Width, top_left.Y);
            return new(bottom_left , top_right);
        }

        /// <summary>
        /// Gets the entire size of the rectangle.
        /// </summary>
        public readonly Size Size => new(
            Math.Abs(bottomright.X - bottomleft.X),
            Math.Abs(topleft.Y - bottomleft.Y)
        );

        /// <summary>
        /// Enlarges this <see cref="Rectangle"/> by the specified size. <br />
        /// Final size will be <see cref="Size"/> + <paramref name="largensize"/>. <br />
        /// The rectangle is enlarged by the bottom-left corner (thus, the bottom-left corner will remain unchanged).
        /// </summary>
        /// <param name="largensize">The additional size for this rectangle to be enlarged to.</param>
        /// <returns>The enlarged rectangle.</returns>
        public readonly Rectangle Enlarge(Size largensize) => new(bottomleft, Size + largensize);

        /// <summary>
        /// Gets a value whether this rectangle is in the same size and coordinates with another <see cref="Rectangle"/>.
        /// </summary>
        /// <param name="other">The other rectangle to test for equality.</param>
        /// <returns>A value whether the two rectangles are the same rectangles.</returns>
        public readonly bool Equals(Rectangle other) =>
            other.bottomleft == bottomleft &&
            other.bottomright == bottomright &&
            other.topleft == topleft &&
            other.topright == topright;

        /// <inheritdoc />
        public readonly override bool Equals(object obj) => obj switch
        {
            Rectangle r => Equals(r),
            _ => false
        };

        /// <summary>
        /// Gets a value whether the specified point is in the rectangle's bounds.
        /// </summary>
        /// <param name="point">The point to see whether is in the rectangle's bounds.</param>
        /// <returns>A value whether the <paramref name="point"/> passed is in the rectangle's bounds.</returns>
        public System.Boolean PointIsInBounds(Point point) => point.X >= topleft.X && point.Y <= topleft.Y && point.X <= bottomright.X && point.Y >= bottomright.Y;

        readonly System.Object ICloneable.Clone() => Clone();

        /// <summary>Clones this rectangle into a new instance.</summary>
        /// <returns>The cloned instance of the current rectangle.</returns>
        public readonly Rectangle Clone() => new(this);

        /// <inheritdoc />
        public readonly override int GetHashCode() => unchecked(bottomleft.GetHashCode() + bottomright.GetHashCode() + topleft.GetHashCode() + topright.GetHashCode());

        /// <summary>
        /// Gets the bottom-left point of the rectangle.
        /// </summary>
        public readonly Point BottomLeft => bottomleft;

        /// <summary>
        /// Gets the bottom-right point of the rectangle.
        /// </summary>
        public readonly Point BottomRight => bottomright;

        /// <summary>
        /// Gets the top-left point of the rectangle.
        /// </summary>
        public readonly Point TopLeft => topleft;

        /// <summary>
        /// Gets the top-right point of the rectangle.
        /// </summary>
        public readonly Point TopRight => topright;

        /// <summary>
        /// Gets the lower-left triangle, which is the half of the rectangle. <br />
        /// The triangle is always orthogonal.
        /// </summary>
        public readonly Triangle LowerLeftTriangle => Triangle.CreateOrthogonal(bottomleft, topright);

        /// <summary>
        /// Gets the upper-right triangle, which is the half of the rectangle. <br />
        /// The triangle is always orthogonal.
        /// </summary>
        public readonly Triangle UpperRightTriangle => Triangle.CreateOrthogonal(topright, bottomleft);

        /// <summary>
        /// Determines whether two <see cref="Rectangle"/>s are equal.
        /// </summary>
        /// <param name="left">The first <see cref="Rectangle"/> to compare.</param>
        /// <param name="right">The second <see cref="Rectangle"/> to compare.</param>
        /// <returns>A value determining equality of <paramref name="left"/> and <paramref name="right"/>.</returns>
        public static bool operator ==(Rectangle left, Rectangle right) => left.Equals(right);

        /// <summary>
        /// Determines whether two <see cref="Rectangle"/>s are not equal.
        /// </summary>
        /// <param name="left">The first <see cref="Rectangle"/> to compare.</param>
        /// <param name="right">The second <see cref="Rectangle"/> to compare.</param>
        /// <returns>A value determining inequality of <paramref name="left"/> and <paramref name="right"/>.</returns>
        public static bool operator !=(Rectangle left, Rectangle right) =>
            left.bottomleft != right.bottomleft ||
            left.bottomright != right.bottomright ||
            left.topleft != right.topleft ||
            left.topright != right.topright;

        /// <summary>
        /// Returns the bounds of this rectangle in a formatted string. <br />
        /// For debugging purposes only.
        /// </summary>
        /// <returns>A formatted string containing the selected bounds of the rectangle.</returns>
        public readonly override System.String ToString() => String.Format(
            "Rectangle(2D) {{ Size: {0} BottomLeft: {1} BottomRight: {2} TopLeft: {3} TopRight: {4} }}",
            Size,
            bottomleft,
            bottomright,
            topleft,
            topright
        );
    }
}