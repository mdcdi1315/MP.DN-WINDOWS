
using System;
using MP.Utilities;
using MP.Annotations.CodeAnalysis;

namespace MP.Graphics
{
    /// <summary>
    /// Defines a color in normalized ([0..1]) floating point format. <br />
    /// The data are packed as RGBA, if needed for native data depiction.
    /// </summary>
    public readonly struct FloatColor
    {
        private readonly float red;
        private readonly float green;
        private readonly float blue;
        private readonly float alpha;

        /// <summary>
        /// Creates a new <see cref="FloatColor"/> structure instance, ensuring that all parameters are into the range [0..1]. If not, they are appropriately clamped.
        /// </summary>
        /// <param name="red">The red channel.</param>
        /// <param name="green">The green channel.</param>
        /// <param name="blue">The blue channel.</param>
        /// <param name="alpha">The alpha channel.</param>
        public FloatColor(float red , float green, float blue, float alpha)
        {
            this.red = MathHelpers.Clamp(red , 0,  1);
            this.green = MathHelpers.Clamp(green, 0, 1);
            this.blue = MathHelpers.Clamp(blue, 0, 1);
            this.alpha = MathHelpers.Clamp(alpha, 0, 1);
        }

        /// <summary>
        /// Creates a new <see cref="FloatColor"/> structure instance from byte values. <br />
        /// The byte values are appropriately transformed to floating-point values during construction time.
        /// </summary>
        /// <param name="red">The red channel.</param>
        /// <param name="green">The green channel.</param>
        /// <param name="blue">The blue channel.</param>
        /// <param name="alpha">The alpha channel.</param>
        public FloatColor(byte red , byte green , byte blue , byte alpha)
        {
            this.red = red / 255f;
            this.green = green / 255f;
            this.blue = blue / 255f;
            this.alpha = alpha / 255f;
        }

        /// <summary>
        /// Creates a new <see cref="FloatColor"/> structure from an existing <see cref="IColor"/> instance. <br />
        /// The byte values contained by the <see cref="IColor"/> instance are appropriately transformed to floating-point values during construction time.
        /// </summary>
        /// <param name="color">The <see cref="IColor"/> instance to create a new floating-point color from.</param>
        /// <exception cref="ArgumentNullException"><paramref name="color"/> was <see langword="null"/>.</exception>
        [Throws(typeof(ArgumentNullException))]
        public FloatColor(IColor color)
        {
            ArgumentNullException.ThrowIfNull(color);
            red = color.R / 255f;
            green = color.G / 255f;
            blue = color.B / 255f;
            alpha = color.A / 255f;
        }

        /// <summary>
        /// Gets the red channel/component of the color.
        /// </summary>
        public readonly float Red => red;

        /// <summary>
        /// Gets the green channel/component of the color.
        /// </summary>
        public readonly float Green => green;

        /// <summary>
        /// Gets the blue channel/component of the color.
        /// </summary>
        public readonly float Blue => blue;

        /// <summary>
        /// Gets the alpha channel/component of the color.
        /// </summary>
        public readonly float Alpha => alpha;

    }
}
