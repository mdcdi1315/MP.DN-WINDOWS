
using System;
using System.Numerics;

namespace MP.Graphics
{
    /// <summary>
    /// Defines a triangle in a 2-dimensional space.
    /// </summary>
    public readonly struct Triangle :
        IEqualityOperators<Triangle , Triangle , System.Boolean>,
        IEquatable<Triangle>,
        ICloneable
    {
        private readonly Point tip, bottomleft, bottomright;

        private Triangle(Point tip, Point bottomleft, Point bottomright)
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
        public Triangle(Point tip, Point bottomleft, int bottomrightdist)
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
        public static Triangle CreateOrthogonal(Point tip, Point bottomright) => new(tip, new Point(tip.X, bottomright.Y), bottomright);

        /// <summary>Creates an irregular triangle.</summary>
        /// <param name="tip">The tip corner of the triangle.</param>
        /// <param name="bottomleft">The bottom-left corner of the triangle.</param>
        /// <param name="bottomright">The bottom-right corner of the triangle.</param>
        /// <returns>The irregular triangle.</returns>
        public static Triangle CreateIrregular(Point tip, Point bottomleft, Point bottomright) => new(tip , bottomleft , bottomright);

        /// <summary>
        /// Gets a value whether this triangle is orthogonal.
        /// </summary>
        public readonly System.Boolean IsOrthogonal => bottomleft.X == tip.X && bottomright.Y == bottomleft.Y;

        /// <summary>
        /// Gets the tip corner of the triangle.
        /// </summary>
        public readonly Point Tip => tip;

        /// <summary>
        /// Gets the bottom-left corner of the triangle.
        /// </summary>
        public readonly Point BottomLeft => bottomleft;

        /// <summary>
        /// Gets the bottom-right corner of the rectangle.
        /// </summary>
        public readonly Point BottomRight => bottomright;

        readonly System.Object ICloneable.Clone() => Clone();

        /// <summary>Clones the current triangle.</summary>
        /// <returns>The cloned triangle.</returns>
        public readonly Triangle Clone() => new(tip , bottomleft , bottomright);

        /// <summary>
        /// Gets a value whether this <see cref="Triangle"/> instance has equal values with the <see cref="Triangle"/> instance provided in <paramref name="other"/> parameter.
        /// </summary>
        /// <param name="other">The other triangle to test.</param>
        /// <returns>A value whether both instances represent the same triangle.</returns>
        public System.Boolean Equals(Triangle other) => tip == other.tip && bottomleft == other.bottomleft && bottomright == other.bottomright;

        /// <summary>
        /// Returns the bounds of this triangle in a formatted string. <br />
        /// For debugging purposes only.
        /// </summary>
        /// <returns>A formatted string containing the selected bounds of the triangle.</returns>
        public readonly override System.String ToString() => String.Format(
            "Triangle(2D) {{ Tip: {0} BottomLeft: {1} BottomRight: {2} }}",
            tip,
            bottomleft,
            bottomright
        );

        /// <summary>
        /// Gets a value whether this <see cref="Triangle"/> instance and an object are equal.
        /// </summary>
        /// <param name="obj">The other object instance for this <see cref="Triangle"/> to be compared with.</param>
        /// <returns>A value whether both are <see cref="Triangle"/> instances and do represent the same triangle.</returns>
        public readonly override bool Equals(object obj) => obj switch
        {
            Triangle tr => Equals(tr),
            _ => false
        };

        /// <summary>Gets a hash code for this <see cref="Triangle"/>.</summary>
        /// <returns>A hash code for this <see cref="Triangle"/> instance.</returns>
        public readonly override int GetHashCode() => tip.GetHashCode() + bottomleft.GetHashCode() + bottomright.GetHashCode();

        /// <summary>Gets a value whether the two triangles are equal.</summary>
        /// <param name="left">The first triangle to test.</param>
        /// <param name="right">The second triangle to test.</param>
        /// <returns>The equality result of <paramref name="left"/> and <paramref name="right"/>.</returns>
        public static bool operator ==(Triangle left, Triangle right) => left.Equals(right);

        /// <summary>Gets a value whether the two triangles are inequal.</summary>
        /// <param name="left">The first triangle to test.</param>
        /// <param name="right">The second triangle to test.</param>
        /// <returns>The inequality result of <paramref name="left"/> and <paramref name="right"/>.</returns>
        public static bool operator !=(Triangle left, Triangle right) => !left.Equals(right);
    }
}