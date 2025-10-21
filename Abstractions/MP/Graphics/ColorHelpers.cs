
using System;
using System.Runtime.CompilerServices;

namespace MP.Graphics
{ 
    /// <summary>Defines utilities around colors.</summary>
    public static class ColorHelpers
    {
        /// <summary>
        /// Returns a 'common', non-collisible, hash code for this 8-bit color instance. <br />
        /// This hash code matches integer ARGB values.
        /// </summary>
        /// <param name="color">The instance to get the hash code for.</param>
        /// <returns>The computed hash code.</returns>
        public static int GetCommonHashCode(this IColor color)
            // Pack them into an integer. Non-collisible since these are 4 bytes placed into an integer.
            => (color.A << 24) | (color.R << 16) | (color.G << 8) | color.B;

        /// <summary>Returns a common description string for this color.</summary>
        /// <param name="color">The instance to get the description for.</param>
        /// <returns>The description string for <paramref name="color"/>.</returns>
        public static String GetCommonStringDescription(this IColor color)
            => String.Format("Color: {{ Alpha: {0} Red: {1} Green: {2} Blue: {3} }}" , color.A , color.R , color.G , color.B);

        /// <summary>
        /// Gets the ARGB value for this color, packed into a 32-bit signed integer.
        /// </summary>
        /// <param name="color">The instance to get the ARGB value for.</param>
        /// <returns>The computed ARGB value.</returns>
        public static int GetARGBValue(this IColor color) => (color.A << 24) | (color.R << 16) | (color.G << 8) | color.B;

        /// <summary>
        /// Gets the ARGB value for this color, packed into a 32-bit signed integer.
        /// </summary>
        /// <param name="color">The instance to get the ARGB value for.</param>
        /// <returns>The computed ARGB value.</returns>
        public static int GetARGBValue(this FloatColor color) 
            => ((int)(color.Alpha * 255f) << 24) | ((int)(color.Red * 255f) << 16) | ((int)(color.Green * 255f) << 8) | (int)(color.Blue * 255f);

        /// <summary>
        /// Gets the BGRA value for this color, packed into a 32-bit signed integer.
        /// </summary>
        /// <param name="color">The instance to get the BGRA value for.</param>
        /// <returns>The computed BGRA value.</returns>
        public static int GetBGRAValue(this IColor color) => (color.B << 24) | (color.G << 16) | (color.R << 8) | color.A;

        /// <summary>
        /// Gets the BGRA value for this color, packed into a 32-bit signed integer.
        /// </summary>
        /// <param name="color">The instance to get the BGRA value for.</param>
        /// <returns>The computed BGRA value.</returns>
        public static int GetBGRAValue(this FloatColor color) 
            => ((int)(color.Blue * 255f) << 24) | ((int)(color.Green * 255f) << 16) | ((int)(color.Red * 255f) << 8) | (int)(color.Alpha * 255f);

        /// <summary>
        /// Gets the RGBA value for this color, packed into a 32-bit signed integer.
        /// </summary>
        /// <param name="color">The instance to get the RGBA value for.</param>
        /// <returns>The computed RGBA value.</returns>
        public static int GetRGBAValue(this IColor color) => (color.R << 24) | (color.G << 16) | (color.B << 8) | color.A;

        /// <summary>
        /// Gets the RGBA value for this color, packed into a 32-bit signed integer.
        /// </summary>
        /// <param name="color">The instance to get the RGBA value for.</param>
        /// <returns>The computed RGBA value.</returns>
        public static int GetRGBAValue(this FloatColor color)
           => ((int)(color.Red * 255f) << 24) | ((int)(color.Green * 255f) << 16) | ((int)(color.Blue * 255f) << 8) | (int) (color.Alpha * 255f);

        /// <summary>
        /// Converts a 32-bit integer to an <see cref="IColor"/> instance.
        /// </summary>
        /// <param name="argb">The ARGB value to convert.</param>
        /// <returns>The converted ARGB value.</returns>
        public static ARGBColor FromArgb(this int argb) => Unsafe.As<int, ARGBColor>(ref Unsafe.AsRef(ReverseIfNeeded(argb)));

        /// <summary>
        /// Converts a 32-bit unsigned integer to an <see cref="IColor"/> instance.
        /// </summary>
        /// <param name="argb">The ARGB value to convert.</param>
        /// <returns>The converted ARGB value.</returns>
        public static ARGBColor FromArgb(this uint argb) => Unsafe.As<uint, ARGBColor>(ref Unsafe.AsRef(ReverseIfNeeded(argb)));

        /// <summary>
        /// Converts a 32-bit integer to an <see cref="IColor"/> instance.
        /// </summary>
        /// <param name="rgba">The RGBA value to convert.</param>
        /// <returns>The converted RGBA value.</returns>
        public static RGBAColor FromRgba(this int rgba) => Unsafe.As<int, RGBAColor>(ref Unsafe.AsRef(ReverseIfNeeded(rgba)));

        /// <summary>
        /// Converts a 32-bit unsigned integer to an <see cref="IColor"/> instance.
        /// </summary>
        /// <param name="rgba">The RGBA value to convert.</param>
        /// <returns>The converted RGBA value.</returns>
        public static RGBAColor FromRgba(this uint rgba) => Unsafe.As<uint, RGBAColor>(ref Unsafe.AsRef(ReverseIfNeeded(rgba)));

        /// <summary>
        /// Converts a 32-bit integer to an <see cref="HSLColor"/> instance.
        /// </summary>
        /// <param name="hsl">The color to convert.</param>
        /// <returns>The translated HSL color from <paramref name="hsl"/>.</returns>
        public static HSLColor FromHsl(this int hsl) => Unsafe.As<int , HSLColor>(ref Unsafe.AsRef(ReverseIfNeeded(hsl)));

        /// <summary>
        /// Blends the alpha channel into the color channels, and that is returned instead. <br />
        /// Thus, no alpha channel exists after the blending, but this conversion is inaccurate due 
        /// to the fact that the color is not done more opaque but it tends to be more 'black' instead. <br />
        /// Passing a color with an alpha channel value of zero causes this effect to be obvious.
        /// </summary>
        /// <param name="color">The color to return as a color with it's channels blended with the alpha channel.</param>
        /// <returns>A <see cref="RGBColor">RGB</see> color describing the alpha-blended channels.</returns>
        public static RGBColor AlphaBlend(this IColor color)
        {
            int alpha = color.A;
            return new(
               (byte)(color.R * alpha / 255),
               (byte)(color.G * alpha / 255),
               (byte)(color.B * alpha / 255)
            );
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static int ReverseIfNeeded(int value) => BitConverter.IsLittleEndian ? value.ReverseEndianess() : value;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static uint ReverseIfNeeded(uint value) => BitConverter.IsLittleEndian ? value.ReverseEndianess() : value;

        /// <summary>
        /// Converts any <see cref="IColor"/> instance to a <see cref="FloatColor"/> instance.
        /// </summary>
        /// <param name="color">The color to convert.</param>
        /// <returns>The converted color in floating point format.</returns>
        public static FloatColor ToFloat(this IColor color) => new(color);

        /// <summary>
        /// Converts any <see cref="IColor16"/> instance to a 8-bit-depth <see cref="IColor"/> instance.
        /// </summary>
        /// <param name="c16">The color to convert.</param>
        /// <returns>The converted color in floating point format.</returns>
        public static IColor To8BitColor(this IColor16 c16) => new ARGBColor(
            // It seems that doing channel / 257f is working the same as (channel / 65535f) * 255f and it perfectly divides 65535 and returns value 255 when the color channel is such!
            (byte)(c16.A / 257f),
            (byte)(c16.R / 257f),
            (byte)(c16.G / 257f),
            (byte)(c16.B / 257f)
        );
    }
}
