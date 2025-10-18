
using System;
using System.Numerics;

namespace MP.Graphics
{
    /// <summary>
    /// Defines a new line in a 2-dimensional space.
    /// </summary>
    public readonly struct LineF :
        IEqualityOperators<LineF, LineF, bool>,
        IEquatable<LineF>,
        IEquatable<Line>,
        ICloneable
    {
        private readonly PointF start, end;

        /// <summary>
        /// Creates a new instance of the <see cref="LineF"/> structure, defining the starting and the ending point of the line.
        /// </summary>
        /// <param name="start">The point where the line starts.</param>
        /// <param name="end">The point where the line ends.</param>
        public LineF(PointF start, PointF end)
        {
            this.start = start;
            this.end = end;
        }

        /// <summary>
        /// Creates a new instance of the <see cref="LineF"/> structure, defining the starting and the ending point of the line.
        /// </summary>
        /// <param name="start">The point where the line starts.</param>
        /// <param name="end">The point where the line ends.</param>
        public LineF(Point start, Point end)
        {
            this.start = new(start);
            this.end = new(end);
        }

        /// <summary>
        /// Gets the point where the line starts.
        /// </summary>
        public readonly PointF Start => start;

        /// <summary>
        /// Gets the point where the line ends.
        /// </summary>
        public readonly PointF End => end;

        /// <summary>
        /// Shifts the current <see cref="LineF"/> instance by the specified X coordinate. <br />
        /// This means that the line is moving left or right in the graphical representation of the line.
        /// </summary>
        /// <param name="dx">The value to shift the line by.</param>
        /// <returns>A new <see cref="LineF"/> instance representing the shifted line.</returns>
        public readonly LineF ShiftX(int dx) => new(start.Offset(dx, 0), end.Offset(dx, 0));

        /// <summary>
        /// Shifts the current <see cref="LineF"/> instance by the specified X coordinate. <br />
        /// This means that the line is moving left or right in the graphical representation of the line.
        /// </summary>
        /// <param name="dx">The value to shift the line by.</param>
        /// <returns>A new <see cref="LineF"/> instance representing the shifted line.</returns>
        public readonly LineF ShiftX(float dx) => new(start.Offset(dx, 0), end.Offset(dx, 0));

        /// <summary>
        /// Shifts the current <see cref="LineF"/> instance by the specified Y coordinate. <br />
        /// This means that the line is moving upwards or downwards in the graphical representation of the line.
        /// </summary>
        /// <param name="dy">The value to shift the line by.</param>
        /// <returns>A new <see cref="Line"/> instance representing the shifted line.</returns>
        public readonly LineF ShiftY(int dy) => new(start.Offset(0, dy), end.Offset(0, dy));

        /// <summary>
        /// Shifts the current <see cref="LineF"/> instance by the specified Y coordinate. <br />
        /// This means that the line is moving upwards or downwards in the graphical representation of the line.
        /// </summary>
        /// <param name="dy">The value to shift the line by.</param>
        /// <returns>A new <see cref="Line"/> instance representing the shifted line.</returns>
        public readonly LineF ShiftY(float dy) => new(start.Offset(0, dy), end.Offset(0, dy));

        /// <summary>
        /// Shifts the current <see cref="Line"/> instance by the specified point. <br />
        /// This method combines both the usage of <see cref="ShiftX(int)"/> and <see cref="ShiftY(int)"/> methods.
        /// </summary>
        /// <param name="p">The point to shift the line by.</param>
        /// <returns>A new <see cref="Line"/> instance representing the shifted line.</returns>
        public readonly LineF ShiftBy(Point p) => new(start.Offset(p), end.Offset(p));

        /// <summary>
        /// Shifts the current <see cref="Line"/> instance by the specified point. <br />
        /// This method combines both the usage of <see cref="ShiftX(int)"/> and <see cref="ShiftY(int)"/> methods.
        /// </summary>
        /// <param name="p">The point to shift the line by.</param>
        /// <returns>A new <see cref="Line"/> instance representing the shifted line.</returns>
        public readonly LineF ShiftBy(PointF p) => new(start.Offset(p), end.Offset(p));

        /// <summary>
        /// Returns the bounds of this line in a formatted string. <br />
        /// For debugging purposes only.
        /// </summary>
        /// <returns>A formatted string containing the selected bounds of the line.</returns>
        public readonly override String ToString() => String.Format(
            "LineF<2D> {{ Start: {0} End: {1} }}",
            start,
            end
        );

        /// <inheritdoc />
        public readonly override int GetHashCode() => start.GetHashCode() + end.GetHashCode();

        /// <summary>
        /// Gets a value whether this <see cref="LineF"/> instance has the same coordinates as another <see cref="LineF"/> instance.
        /// </summary>
        /// <param name="l">The other instance to test this instance against.</param>
        /// <returns>A value whether the current and the passed structure are considered equal.</returns>
        public readonly bool Equals(LineF l) => l.start == start && l.end == end;

        /// <summary>
        /// Gets a value whether this <see cref="LineF"/> instance has the same coordinates as another <see cref="Line"/> instance.
        /// </summary>
        /// <param name="l">The other instance to test this instance against.</param>
        /// <returns>A value whether the current and the passed structure are considered equal.</returns>
        public readonly bool Equals(Line l) => l.Start == start && l.End == end;

        /// <inheritdoc />
        public readonly override bool Equals(object obj) => obj switch
        {
            LineF l => Equals(l),
            Line l => Equals(l),
            _ => false
        };

        /// <summary>Creates a copy this <see cref="Line"/> instance to another instance.</summary>
        /// <returns>A new <see cref="Line"/> instance, but having the same values as this <see cref="Line"/> instance.</returns>
        public readonly LineF Clone() => new(start, end);

        readonly object ICloneable.Clone() => Clone();

        /// <summary>Determines whether two <see cref="Line"/>s are equal.</summary>
        /// <param name="left">The first <see cref="Line"/> to compare.</param>
        /// <param name="right">The second <see cref="Line"/> to compare.</param>
        /// <returns>A value determining equality of <paramref name="left"/> and <paramref name="right"/>.</returns>
        public static bool operator ==(LineF left, LineF right) => left.Equals(right);

        /// <summary>Determines whether two <see cref="Line"/>s are inequal.</summary>
        /// <param name="left">The first <see cref="Line"/> to compare.</param>
        /// <param name="right">The second <see cref="Line"/> to compare.</param>
        /// <returns>A value determining inequality of <paramref name="left"/> and <paramref name="right"/>.</returns>
        public static bool operator !=(LineF left, LineF right) => left.start != right.start || left.end != right.end;
    }
}