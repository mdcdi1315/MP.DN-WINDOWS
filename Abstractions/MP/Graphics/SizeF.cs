
using System;
using System.Numerics;
using System.Runtime.InteropServices;

namespace MP.Graphics
{
    /// <summary>
    /// Defines a two-dimensional vector offsetting from point (0 , 0), starting at the upper-left corner of a drawing rectangle.
    /// </summary>
    [StructLayout(LayoutKind.Explicit , Pack = sizeof(float) , Size = sizeof(float) * 2)]
    public readonly struct SizeF :
        ICloneable,
        IEquatable<Size>,
        IEquatable<SizeF>,
        ITruncatable<Size>,
        IVectorDistance<SizeF, System.Single>,
        IUnaryNegationOperators<SizeF, SizeF>,
        IDivisionOperators<SizeF, SizeF, SizeF>,
        IAdditionOperators<SizeF, SizeF, SizeF>,
        IMultiplyOperators<SizeF, SizeF, SizeF>,
        ISubtractionOperators<SizeF, SizeF, SizeF>,
        IEqualityOperators<SizeF, SizeF, System.Boolean>
    {
        /// <summary>
        /// The width or the displacement starting from X = 0 to the current value.
        /// </summary>
        [FieldOffset(0)]
        public readonly float Width;

        /// <summary>
        /// The height or the displacement starting from Y = 0 to the current value.
        /// </summary>
        [FieldOffset(sizeof(float))]
        public readonly float Height;

        /// <summary>
        /// Constructs a new <see cref="SizeF"/> instance by explicitly specifying the X and Y displacements from point (0 , 0).
        /// </summary>
        /// <param name="width">The desired X displacement.</param>
        /// <param name="height">The desired Y displacement.</param>
        public SizeF(float width, float height)
        {
            Width = width;
            Height = height;
        }

        /// <summary>
        /// Constructs a new <see cref="SizeF"/> instance by explicitly specifying the X and Y displacements from point (0 , 0). <br />
        /// This constructor variant allows to create a <see cref="SizeF"/> instance from two integers instead.
        /// </summary>
        /// <param name="width">The desired X displacement.</param>
        /// <param name="height">The desired Y displacement.</param>
        public SizeF(int width , int height)
        {
            Width = width;
            Height = height;
        }

        /// <summary>
        /// Constructs a new <see cref="SizeF"/> instance, from another <see cref="Size"/> instance. This cast is always safe.
        /// </summary>
        /// <param name="size">The <see cref="Size"/> instance to get the coordinates from.</param>
        public SizeF(Size size)
        {
            Width = size.Width;
            Height = size.Height;
        }

        /// <summary>
        /// Gets a value whether the two <see cref="SizeF"/> instances are equal. <br />
        /// Two <see cref="Size"/> instances are considered equal if and only if both coordinates are equal.
        /// </summary>
        /// <param name="other">The other <see cref="SizeF"/> instance to compare this structure against.</param>
        /// <returns>A value representing the equality of both structures.</returns>
        public readonly bool Equals(SizeF other) => Width == other.Width && Height == other.Height;

        /// <summary>
        /// Gets a value whether the current <see cref="SizeF"/> and a provided <see cref="Size"/> instance are equal.
        /// </summary>
        /// <param name="other">The <see cref="Size"/> instance to compare this one against.</param>
        /// <returns>A value representing the equality of both structures.</returns>
        public readonly bool Equals(Size other) => Width == other.Width && Height == other.Height;

        readonly object ICloneable.Clone() => Clone();

        /// <summary>Clones the current <see cref="SizeF"/> instance.</summary>
        /// <returns>The cloned <see cref="SizeF"/> instance.</returns>
        public readonly SizeF Clone() => new(Width, Height);

        /// <summary>
        /// For debugging purposes only. <br />
        /// Returns the contents of the current structure.
        /// </summary>
        /// <returns>The width and height into a nicely formatted string.</returns>
        public readonly override System.String ToString() => $"SizeF(2D) {{ Width = {Width} , Height = {Height} }}";

        /// <inheritdoc />
        public readonly override bool Equals(System.Object obj) => obj switch
        {
            Size p => Equals(p),
            SizeF pf => Equals(pf),
            _ => false,
        };

        /// <summary>
        /// Gets a hash code for the current size. <br />
        /// The hash code is effectively computed by the sum of both <see cref="Width"/> and <see cref="Height"/> fields.
        /// </summary>
        /// <returns>The computed hash code for this instance.</returns>
        public readonly override int GetHashCode() => (System.Int32)(Width + Height);

        /// <inheritdoc />
        public readonly Size Truncate() => new((System.Int32)Width , (System.Int32)Height);

        /// <summary>Computes the absolute distance of the specified <see cref="SizeF"/>.</summary>
        /// <param name="self">The <see cref="SizeF"/> structure to compute it's distance.</param>
        /// <returns>The absolute distance (displacement) of <paramref name="self"/> from point (0, 0).</returns>
        public static float GetDistance(SizeF self) => unchecked(self.Width * self.Height);

        /// <summary>
        /// Performs unary negation on the specified <see cref="SizeF"/>.
        /// </summary>
        /// <param name="size">The <see cref="SizeF"/> instance to perform unary negation on.</param>
        /// <returns>The unary negation result of <paramref name="size"/>.</returns>
        public static SizeF operator -(SizeF size) => new(-size.Width, -size.Height);

        /// <summary>
        /// Performs the addition of two <see cref="SizeF"/> structures.
        /// </summary>
        /// <param name="left">The first <see cref="SizeF"/> of the sum operation.</param>
        /// <param name="right">The second <see cref="SizeF"/> of the sum operation.</param>
        /// <returns>The addition result of adding <paramref name="left"/> and <paramref name="right"/>.</returns>
        public static SizeF operator +(SizeF left, SizeF right) => new(left.Width + right.Width, left.Height + right.Height);

        /// <summary>
        /// Performs the subtraction of two <see cref="SizeF"/> structures.
        /// </summary>
        /// <param name="left">The first <see cref="SizeF"/> of the sub operation.</param>
        /// <param name="right">The second <see cref="SizeF"/> of the sub operation.</param>
        /// <returns>The subtraction result of removing <paramref name="left"/> by <paramref name="right"/>.</returns>
        public static SizeF operator -(SizeF left, SizeF right) => new(left.Width - right.Width, left.Height - right.Height);

        /// <summary>
        /// Determines whether two <see cref="SizeF"/>s are equal.
        /// </summary>
        /// <param name="left">The first <see cref="SizeF"/> to compare.</param>
        /// <param name="right">The second <see cref="SizeF"/> to compare.</param>
        /// <returns>A value determining equality of <paramref name="left"/> and <paramref name="right"/>.</returns>
        public static bool operator ==(SizeF left, SizeF right) => left.Equals(right);

        /// <summary>
        /// Determines whether two <see cref="SizeF"/>s are inequal.
        /// </summary>
        /// <param name="left">The first <see cref="SizeF"/> to compare.</param>
        /// <param name="right">The second <see cref="SizeF"/> to compare.</param>
        /// <returns>A value determining inequality of <paramref name="left"/> and <paramref name="right"/>.</returns>
        public static bool operator !=(SizeF left, SizeF right) => !left.Equals(right);

        /// <summary>
        /// Implcitly converts a <see cref="Size"/> to a <see cref="SizeF"/> instance. See <see cref="SizeF(Size)"/> constructor for more information.
        /// </summary>
        /// <param name="size">The <see cref="Size"/> to convert as a <see cref="SizeF"/> instance.</param>
        public static implicit operator SizeF(Size size) => new(size);

        /// <summary>
        /// Performs the division of two <see cref="SizeF"/> structures. <br />
        /// Both components of <paramref name="left"/> are mapped to the respective components of <paramref name="right"/>. <br />
        /// Division is performed as follows: <c>left.Width / right.Width, left.Height / right.Height</c>
        /// </summary>
        /// <param name="left">The dividend <see cref="SizeF"/> structure.</param>
        /// <param name="right">The divisor <see cref="SizeF"/> structure.</param>
        /// <returns>The division result of dividing <paramref name="left"/> by <paramref name="right"/>.</returns>
        public static SizeF operator /(SizeF left, SizeF right) => new(left.Width / right.Width, left.Height / right.Height);

        /// <summary>
        /// Performs the multiplication of two <see cref="SizeF"/> structures. <br />
        /// Both components of <paramref name="left"/> are mapped to the respective components of <paramref name="right"/>. <br />
        /// Multiplication is performed as follows: <c>left.Width * right.Width, left.Height * right.Height</c>
        /// </summary>
        /// <param name="left">The first <see cref="SizeF"/> structure to multiply.</param>
        /// <param name="right">The second <see cref="SizeF"/> structure to multiply.</param>
        /// <returns>The multiplication result of multiplying <paramref name="left"/> by <paramref name="right"/>.</returns>
        public static SizeF operator *(SizeF left, SizeF right) => new(left.Width * right.Width, left.Height * right.Height);
    }
}