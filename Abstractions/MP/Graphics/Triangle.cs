
using System;

namespace MP.Graphics
{
    /// <summary>
    /// Defines a triangle in a 2-dimensional space.
    /// </summary>
    public struct Triangle
    {
        private Point tip;
        private Point bottomleft, bottomright;

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
        public static Triangle CreateOrthogonal(Point tip, Point bottomright) => new()
        {
            tip = tip,
            bottomright = bottomright,
            bottomleft = new(tip.X, bottomright.Y)
        };

        /// <summary>
        /// Creates an irregular triangle.
        /// </summary>
        /// <param name="tip">The tip corner of the triangle.</param>
        /// <param name="bottomleft">The bottom-left corner of the triangle.</param>
        /// <param name="bottomright">The bottom-right corner of the triangle.</param>
        /// <returns>The irregular triangle.</returns>
        public static Triangle CreateIrregular(Point tip, Point bottomleft, Point bottomright) => new()
        {
            tip = tip,
            bottomleft = bottomleft,
            bottomright = bottomright
        };

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
    }
}