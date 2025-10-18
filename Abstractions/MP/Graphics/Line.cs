
using System;
using System.Numerics;

namespace MP.Graphics
{
    /// <summary>
    /// Defines a new line in a 2-dimensional space.
    /// </summary>
    public readonly struct Line :
        IEqualityOperators<Line , Line , bool>,
        IEquatable<Line>,
        ICloneable
    {
        private readonly Point start, end;

        /// <summary>
        /// Creates a new instance of the <see cref="Line"/> structure, defining the starting and the ending point of the line.
        /// </summary>
        /// <param name="start">The point where the line starts.</param>
        /// <param name="end">The point where the line ends.</param>
        public Line(Point start, Point end)
        {
            this.start = start;
            this.end = end;
        }

        /// <summary>
        /// Gets the point where the line starts.
        /// </summary>
        public readonly Point Start => start;

        /// <summary>
        /// Gets the point where the line ends.
        /// </summary>
        public readonly Point End => end;

        /// <summary>
        /// Shifts the current <see cref="Line"/> instance by the specified X coordinate. <br />
        /// This means that the line is moving left or right in the graphical representation of the line.
        /// </summary>
        /// <param name="dx">The value to shift the line by.</param>
        /// <returns>A new <see cref="Line"/> instance representing the shifted line.</returns>
        public readonly Line ShiftX(int dx) => new(start.Offset(dx, 0), end.Offset(dx, 0));

        /// <summary>
        /// Shifts the current <see cref="Line"/> instance by the specified Y coordinate. <br />
        /// This means that the line is moving upwards or downwards in the graphical representation of the line.
        /// </summary>
        /// <param name="dy">The value to shift the line by.</param>
        /// <returns>A new <see cref="Line"/> instance representing the shifted line.</returns>
        public readonly Line ShiftY(int dy) => new(start.Offset(0 , dy) , end.Offset(0 , dy));

        /// <summary>
        /// Shifts the current <see cref="Line"/> instance by the specified point. <br />
        /// This method combines both the usage of <see cref="ShiftX(int)"/> and <see cref="ShiftY(int)"/> methods.
        /// </summary>
        /// <param name="p">The point to shift the line by.</param>
        /// <returns>A new <see cref="Line"/> instance representing the shifted line.</returns>
        public readonly Line ShiftBy(Point p) => new(start.Offset(p), end.Offset(p));

        /// <summary>
        /// Returns the bounds of this line in a formatted string. <br />
        /// For debugging purposes only.
        /// </summary>
        /// <returns>A formatted string containing the selected bounds of the line.</returns>
        public readonly override String ToString() => String.Format(
            "Line<2D> {{ Start: {0} End: {1} }}",
            start,
            end
        );

        /// <inheritdoc />
        public readonly override int GetHashCode() => start.GetHashCode() + end.GetHashCode();

        /// <summary>
        /// Gets a value whether this <see cref="Line"/> instance has the same coordinates as another <see cref="Line"/> instance.
        /// </summary>
        /// <param name="l">The other instance to test this instance against.</param>
        /// <returns>A value whether the current and the passed structure are considered equal.</returns>
        public readonly bool Equals(Line l) => l.start == start && l.end == end;

        /// <inheritdoc />
        public readonly override bool Equals(object obj) => obj switch {
            Line l => Equals(l),
            _ => false
        };

        /// <summary>Creates a copy this <see cref="Line"/> instance to another instance.</summary>
        /// <returns>A new <see cref="Line"/> instance, but having the same values as this <see cref="Line"/> instance.</returns>
        public readonly Line Clone() => new(start, end);

        readonly object ICloneable.Clone() => Clone();

        /// <summary>Determines whether two <see cref="Line"/>s are equal.</summary>
        /// <param name="left">The first <see cref="Line"/> to compare.</param>
        /// <param name="right">The second <see cref="Line"/> to compare.</param>
        /// <returns>A value determining equality of <paramref name="left"/> and <paramref name="right"/>.</returns>
        public static bool operator ==(Line left, Line right) => left.Equals(right);

        /// <summary>Determines whether two <see cref="Line"/>s are inequal.</summary>
        /// <param name="left">The first <see cref="Line"/> to compare.</param>
        /// <param name="right">The second <see cref="Line"/> to compare.</param>
        /// <returns>A value determining inequality of <paramref name="left"/> and <paramref name="right"/>.</returns>
        public static bool operator !=(Line left, Line right) => left.start != right.start || left.end != right.end;
    }
}