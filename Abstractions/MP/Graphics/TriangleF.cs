
using System;
using System.Numerics;

namespace MP.Graphics
{
    /// <summary>
    /// Defines a triangle in a 2-dimensional space.
    /// </summary>
    public readonly struct TriangleF :
        IEqualityOperators<TriangleF, TriangleF, System.Boolean>,
        IEqualityOperators<TriangleF, Triangle, System.Boolean>,
        IEquatable<TriangleF>,
        IEquatable<Triangle>,
        ITruncatable<Triangle>,
        ICloneable
    {
        private readonly PointF tip, bottomleft, bottomright;

        private TriangleF(PointF tip, PointF bottomleft, PointF bottomright)
        {
            this.tip = tip;
            this.bottomleft = bottomleft;
            this.bottomright = bottomright;
        }

        /// <summary>
        /// Creates a new triangle, specifying the point where the tip will be and the point where the bottom-left corner will be. <br />
        /// The bottom-right corner is specified by the bottom-left corner offseted by <paramref name="bottomrightdist"/> parameter.
        /// </summary>
        /// <param name="tip">The tip corner of the triangle.</param>
        /// <param name="bottomleft">The bottom-left corner of the triangle.</param>
        /// <param name="bottomrightdist">The bottom-right X-displacement from the bottom-left corner of the triangle.</param>
        public TriangleF(PointF tip, PointF bottomleft, int bottomrightdist)
        {
            this.tip = tip;
            this.bottomleft = bottomleft;
            bottomright = new(bottomleft.X + bottomrightdist, bottomleft.Y);
        }

        /// <summary>
        /// Creates an orthogonal triangle, specified by it's tip and bottom-right corners.
        /// </summary>
        /// <param name="tip">The tip corner of the triangle.</param>
        /// <param name="bottomright">The bottom-right corner of the triangle.</param>
        /// <returns>The orthogonal triangle.</returns>
        public static TriangleF CreateOrthogonal(PointF tip, PointF bottomright) => new(tip, new PointF(tip.X, bottomright.Y), bottomright);

        /// <summary>Creates an irregular triangle.</summary>
        /// <param name="tip">The tip corner of the triangle.</param>
        /// <param name="bottomleft">The bottom-left corner of the triangle.</param>
        /// <param name="bottomright">The bottom-right corner of the triangle.</param>
        /// <returns>The irregular triangle.</returns>
        public static TriangleF CreateIrregular(PointF tip, PointF bottomleft, PointF bottomright) => new(tip, bottomleft, bottomright);

        /// <summary>
        /// Gets a value whether this triangle is orthogonal.
        /// </summary>
        public readonly System.Boolean IsOrthogonal => bottomleft.X == tip.X && bottomright.Y == bottomleft.Y;

        /// <summary>
        /// Gets the tip corner of the triangle.
        /// </summary>
        public readonly PointF Tip => tip;

        /// <summary>
        /// Gets the bottom-left corner of the triangle.
        /// </summary>
        public readonly PointF BottomLeft => bottomleft;

        /// <summary>
        /// Gets the bottom-right corner of the rectangle.
        /// </summary>
        public readonly PointF BottomRight => bottomright;

        readonly System.Object ICloneable.Clone() => Clone();

        /// <summary>Clones the current triangle.</summary>
        /// <returns>The cloned triangle.</returns>
        public readonly TriangleF Clone() => new(tip, bottomleft, bottomright);

        /// <summary>
        /// Gets a value whether this <see cref="TriangleF"/> instance has equal values with the <see cref="TriangleF"/> instance provided in <paramref name="other"/> parameter.
        /// </summary>
        /// <param name="other">The other triangle to test.</param>
        /// <returns>A value whether both instances represent the same triangle.</returns>
        public System.Boolean Equals(TriangleF other) => tip == other.tip && bottomleft == other.bottomleft && bottomright == other.bottomright;

        /// <summary>
        /// Gets a value whether this <see cref="TriangleF"/> instance has equal values with the <see cref="Triangle"/> instance provided in <paramref name="other"/> parameter.
        /// </summary>
        /// <param name="other">The other triangle to test.</param>
        /// <returns>A value whether both instances represent the same triangle.</returns>
        public System.Boolean Equals(Triangle other) => tip == other.Tip && bottomleft == other.BottomLeft && bottomright == other.BottomRight;

        /// <summary>
        /// Returns the bounds of this triangle in a formatted string. <br />
        /// For debugging purposes only.
        /// </summary>
        /// <returns>A formatted string containing the selected bounds of the triangle.</returns>
        public readonly override System.String ToString() => String.Format(
            "TriangleF(2D) {{ Tip: {0} BottomLeft: {1} BottomRight: {2} }}",
            tip,
            bottomleft,
            bottomright
        );

        /// <summary>
        /// Gets a value whether this <see cref="TriangleF"/> instance and an object are equal.
        /// </summary>
        /// <param name="obj">The other object instance for this <see cref="TriangleF"/> to be compared with.</param>
        /// <returns>A value whether both instances do represent the same triangle.</returns>
        public readonly override bool Equals(object obj) => obj switch
        {
            Triangle t => Equals(t),
            TriangleF tr => Equals(tr),
            _ => false
        };

        /// <summary>Gets a hash code for this <see cref="TriangleF"/>.</summary>
        /// <returns>A hash code for this <see cref="TriangleF"/> instance.</returns>
        public readonly override int GetHashCode() => tip.GetHashCode() + bottomleft.GetHashCode() + bottomright.GetHashCode();

        /// <inheritdoc />
        public Triangle Truncate() => Triangle.CreateIrregular(tip.Truncate(), bottomleft.Truncate(), bottomright.Truncate());

        /// <summary>Gets a value whether the two triangles are equal.</summary>
        /// <param name="left">The first triangle to test.</param>
        /// <param name="right">The second triangle to test.</param>
        /// <returns>The equality result of <paramref name="left"/> and <paramref name="right"/>.</returns>
        public static bool operator ==(TriangleF left, TriangleF right) => left.Equals(right);

        /// <summary>Gets a value whether the two triangles are inequal.</summary>
        /// <param name="left">The first triangle to test.</param>
        /// <param name="right">The second triangle to test.</param>
        /// <returns>The inequality result of <paramref name="left"/> and <paramref name="right"/>.</returns>
        public static bool operator !=(TriangleF left, TriangleF right) => !left.Equals(right);

        /// <summary>Gets a value whether the two triangles are equal.</summary>
        /// <param name="left">The first triangle to test.</param>
        /// <param name="right">The second triangle to test.</param>
        /// <returns>The equality result of <paramref name="left"/> and <paramref name="right"/>.</returns>
        public static bool operator ==(TriangleF left, Triangle right) => left.Equals(right);

        /// <summary>Gets a value whether the two triangles are inequal.</summary>
        /// <param name="left">The first triangle to test.</param>
        /// <param name="right">The second triangle to test.</param>
        /// <returns>The inequality result of <paramref name="left"/> and <paramref name="right"/>.</returns>
        public static bool operator !=(TriangleF left, Triangle right) => !left.Equals(right);
    }
}