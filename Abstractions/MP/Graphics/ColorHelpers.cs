
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

        /// <summary>
        /// Returns a 'common', non-collisible, hash code for this 16-bit color instance. <br />
        /// This hash code matches integer ARGB values.
        /// </summary>
        /// <param name="color">The instance to get the hash code for.</param>
        /// <returns>The computed hash code.</returns>
        public static long GetCommonHashCodeLong(this IColor16 color)
            => (color.A.ToInt64() << 48) | (color.R.ToInt64() << 32) | (color.G.ToInt64() << 16) | color.B;

        /// <summary>Returns a common description string for this color.</summary>
        /// <param name="color">The instance to get the description for.</param>
        /// <returns>The description string for <paramref name="color"/>.</returns>
        public static String GetCommonStringDescription(this IColor color)
            => String.Format("Color: {{ Alpha: {0} Red: {1} Green: {2} Blue: {3} }}" , color.A , color.R , color.G , color.B);

        /// <summary>Returns a common description string for this color.</summary>
        /// <param name="color">The instance to get the description for.</param>
        /// <returns>The description string for <paramref name="color"/>.</returns>
        public static String GetCommonStringDescription(this IColor16 color) 
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
        public static ARGBColor FromArgb(this int argb) => argb.ReinterpretAsStructure<int, ARGBColor>();

        /// <summary>
        /// Converts a 32-bit unsigned integer to an <see cref="IColor"/> instance.
        /// </summary>
        /// <param name="argb">The ARGB value to convert.</param>
        /// <returns>The converted ARGB value.</returns>
        public static ARGBColor FromArgb(this uint argb) => argb.ReinterpretAsStructure<uint, ARGBColor>();

        /// <summary>
        /// Converts a 32-bit integer to an <see cref="IColor"/> instance.
        /// </summary>
        /// <param name="rgba">The RGBA value to convert.</param>
        /// <returns>The converted RGBA value.</returns>
        public static RGBAColor FromRgba(this int rgba) => rgba.ReinterpretAsStructure<int, RGBAColor>();

        /// <summary>
        /// Converts a 32-bit unsigned integer to an <see cref="IColor"/> instance.
        /// </summary>
        /// <param name="rgba">The RGBA value to convert.</param>
        /// <returns>The converted RGBA value.</returns>
        public static RGBAColor FromRgba(this uint rgba) => rgba.ReinterpretAsStructure<uint, RGBAColor>();

        /// <summary>
        /// Converts a 32-bit integer to an <see cref="HSLColor"/> instance.
        /// </summary>
        /// <param name="hsl">The color to convert.</param>
        /// <returns>The translated HSL color from <paramref name="hsl"/>.</returns>
        public static HSLColor FromHsl(this int hsl) => hsl.ReinterpretAsStructure<int, HSLColor>();

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

        /// <summary>
        /// Blends the alpha channel into the color channels, and that is returned instead. <br />
        /// Thus, no alpha channel exists after the blending, but this conversion is inaccurate due 
        /// to the fact that the color is not done more opaque but it tends to be more 'black' instead. <br />
        /// Passing a color with an alpha channel value of zero causes this effect to be obvious.
        /// </summary>
        /// <param name="color">The color to return as a color with it's channels blended with the alpha channel.</param>
        /// <returns>A <see cref="RGB16Color">RGB</see> color describing the alpha-blended channels.</returns>
        public static RGB16Color AlphaBlend(this IColor16 color)
        {
            int alpha = color.A;
            return new(
               (ushort)(color.R * alpha / 65535),
               (ushort)(color.G * alpha / 65535),
               (ushort)(color.B * alpha / 65535)
            );
        }

        /// <summary>Inverts the specified color.</summary>
        /// <param name="color">The color to be inverted.</param>
        /// <returns>A <see cref="ARGBColor">ARGB</see> color describing the inverted color of the current color.</returns>
        public static ARGBColor Invert(this IColor color) => new(
            (byte)(255 - color.A),
            (byte)(255 - color.R),
            (byte)(255 - color.G),
            (byte)(255 - color.B)
        );

        /// <summary>Inverts the specified color.</summary>
        /// <param name="color">The color to be inverted.</param>
        /// <returns>A <see cref="ARGB16Color">ARGB</see> color describing the inverted color of the current color.</returns>
        public static ARGB16Color Invert(this IColor16 color) => new(
            (byte)(255 - color.A),
            (byte)(255 - color.R),
            (byte)(255 - color.G),
            (byte)(255 - color.B)
        );

        /// <summary>Maps this color to the gray-scale.</summary>
        /// <param name="color">The color to be mapped.</param>
        /// <returns>A <see cref="ARGBColor">ARGB</see> color describing the current color in the gray scale.</returns>
        public static ARGBColor Grayscale(this IColor color)
        {
            byte c = (byte)((color.R + color.G + color.B) / 765);
            return new(
                color.A,
                c,
                c,
                c
            );
        }

        /// <summary>Maps this color to the gray-scale.</summary>
        /// <param name="color">The color to be mapped.</param>
        /// <returns>A <see cref="ARGB16Color">ARGB</see> color describing the current color in the gray scale.</returns>
        public static ARGB16Color Grayscale(this IColor16 color)
        {
            ushort c = (ushort)((color.R + color.G + color.B) / 196605);
            return new(
                color.A,
                c,
                c,
                c
            );
        }

        /// <summary>
        /// Converts any <see cref="IColor"/> instance to a <see cref="FloatColor"/> instance.
        /// </summary>
        /// <param name="color">The color to convert.</param>
        /// <returns>The converted color in floating point format.</returns>
        public static FloatColor ToFloat(this IColor color) => new(color);

        /// <summary>
        /// Converts any <see cref="IColor16"/> instance to a <see cref="FloatColor"/> instance.
        /// </summary>
        /// <param name="color_16">The color to convert.</param>
        /// <returns>The converted color in floating point format.</returns>
        public static FloatColor ToFloat(this IColor16 color_16) => new(color_16);

        /// <summary>
        /// Converts any <see cref="IColor16"/> instance to a 8-bit-depth <see cref="IColor"/> instance.
        /// </summary>
        /// <param name="c16">The color to convert.</param>
        /// <returns>The converted color in 8-bit-depth format.</returns>
        public static IColor To8BitColor(this IColor16 c16) => new ARGBColor(
            // It seems that doing channel / 257f is working the same as (channel / 65535f) * 255f and it perfectly divides 65535 and returns value 255 when the color channel is such!
            (byte)(c16.A / 257f),
            (byte)(c16.R / 257f),
            (byte)(c16.G / 257f),
            (byte)(c16.B / 257f)
        );

        /// <summary>
        /// Converts any <see cref="IColor"/> instance to a 16-bit-depth <see cref="IColor16"/> instance.
        /// </summary>
        /// <param name="color">The color to convert.</param>
        /// <returns>The converted color as a 16-bit-depth format.</returns>
        public static IColor16 To16BitColor(this IColor color) => new ARGB16Color(
            (ushort)(color.A * 257),
            (ushort)(color.R * 257),
            (ushort)(color.G * 257),
            (ushort)(color.B * 257)
        );
    }
}
