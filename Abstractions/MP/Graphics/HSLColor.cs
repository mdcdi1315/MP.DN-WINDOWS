
using System;
using MP.Annotations;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;

namespace MP.Graphics
{
    // Information and insight about HSL and it's color conversions are acquired from https://www.niwa.nu/2013/05/math-behind-colorspace-conversions-rgb-hsl/.

    /// <summary>
    /// Represents a color that is instead reproduced by Hue, Luminance and Saturation. <br />
    /// Both conversions to RGB are provided.
    /// </summary>
    [Preliminary]
    [StructLayout(LayoutKind.Explicit, Pack = 4, Size = 4)]
    public readonly struct HSLColor :
        ITruncatable<RGBColor>
    {
        [FieldOffset(0)]
        private readonly ushort hue;

        [FieldOffset(2)]
        private readonly byte saturation;

        [FieldOffset(3)]
        private readonly byte luminance;

        /// <summary>
        /// Initializes a new <see cref="HSLColor"/> instance from the specified RGB color.
        /// </summary>
        /// <param name="rgb">The RGB color to process.</param>
        public HSLColor(IColor rgb)
        {
            float[] rgbratio = new float[] {
                rgb.R / 255F,
                rgb.G / 255F,
                rgb.B / 255F
            };
            float min = 2, max = -1;
            foreach (var c in rgbratio)
            {
                if (c > max) { max = c; }
                if (c < min) { min = c; }
            }
            float lum = ((max + min) / 2) * 100f , mn = max - min;
            luminance = (System.Byte)Round(lum);
            if ((saturation = (System.Byte)((min == max) ? 0f : Round(((lum > 50f) ? mn / (2.0f - mn) : mn / (max + min)) * 100f))) == 0) {
                hue = 0;
            } else {
                float ht1, ht2;
                if (rgbratio[0] == max) { // Maximum is the red channel
                    ht1 = 0f;
                    ht2 = rgbratio[1] - rgbratio[2];
                } else if (rgbratio[1] == max) { // Maximum is the red channel
                    ht1 = 2.0f;
                    ht2 = rgbratio[2] - rgbratio[0];
                } else /* if (rgbratio[2] == max) */ { // Maximum is the blue channel
                    ht1 = 4.0f;
                    ht2 = rgbratio[0] - rgbratio[1];
                }
                float h1 = (ht1 + (ht2 / (max - min))) * 60f;
                hue = (System.UInt16)Round(h1 < 0f ? h1 + 360f : h1);
            }
        }

        /// <summary>
        /// Initializes a new <see cref="HSLColor"/> instance from the specified raw hue, saturation and luminance values.
        /// </summary>
        /// <param name="hue">The hue of the HSL color.</param>
        /// <param name="saturation">The saturation of the HSL color.</param>
        /// <param name="luminance">The luminance of the HSL color.</param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="hue"/> was not in the range 0..360 <br />
        /// -or- <br />
        /// <paramref name="luminance"/> or <paramref name="saturation"/> were not in the range 0..100.
        /// </exception>
        public HSLColor(ushort hue, byte saturation, byte luminance)
        {
            if (hue > 360)
            {
                throw new ArgumentOutOfRangeException(nameof(hue), "Hue is a value in degrees and thus it cannot be less than 0 and more than 360.");
            }
            if (saturation > 100)
            {
                throw new ArgumentOutOfRangeException(nameof(saturation), "Saturation must be a value between 0 and 100.");
            }
            if (luminance > 100)
            {
                throw new ArgumentOutOfRangeException(nameof(luminance), "Luminance must be a value between 0 and 100.");
            }
            this.hue = hue;
            this.saturation = saturation;
            this.luminance = luminance;
        }

        /// <summary>
        /// Converts the color back to the RGB scale.
        /// </summary>
        /// <returns>The converted RGB color.</returns>
        public readonly RGBColor ToRGB()
        {
            float t_lum = luminance * 0.01f;
            float t_sat = saturation * 0.01f;
            if (saturation == 0)
            {
                // No saturation, color is in grayscale, thus map luminance directly
                byte color = (System.Byte)(t_lum * 255f);
                return new RGBColor(
                    color,
                    color,
                    color
                );
            }
            else
            {
                float t1 = (luminance < 50) ? (t_lum * (1.0f + t_sat)) : (t_lum + t_sat) - (t_lum * t_sat);
                float t2 = (2 * t_lum) - t1;
                float hue = this.hue / 360f;
                float[] colorst = new float[] { hue + 0.333f, hue, hue - 0.333f };
                for (int I = 0; I < colorst.Length; I++)
                {
                    if (colorst[I] > 1f) {
                        colorst[I] -= 1;
                    } else if (colorst[I] < 0f) {
                        colorst[I] += 1;
                    }
                    if ((6f * colorst[I]) < 1f) {
                        colorst[I] = t2 + ((t1 - t2) * 6f * colorst[I]);
                    } else if ((2f * colorst[I]) < 1f) {
                        colorst[I] = t1;
                    } else if ((3f * colorst[I]) < 2f) {
                        colorst[I] = t2 + ((t1 - t2) * ((0.666f - colorst[I]) * 6f));
                    } else {
                        colorst[I] = t2;
                    }
                }

                return new RGBColor(
                    (System.Byte)Round(colorst[0] * 255f),
                    (System.Byte)Round(colorst[1] * 255f),
                    (System.Byte)Round(colorst[2] * 255f)
                );
            }
        }

        RGBColor ITruncatable<RGBColor>.Truncate() => ToRGB();

        /// <summary>Hue</summary>
        public readonly ushort Hue => hue;

        /// <summary>Saturation of the color</summary>
        public readonly byte Saturation => saturation;

        /// <summary>Luminance of the color</summary>
        public readonly byte Luminance => luminance;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static float Round(float value)
        {
            int v = (int)value;
            return (value - v > 0.4999999f) ? v + 1f : v;
        }

        /// <summary>Gets a string describing this color.</summary>
        /// <returns>The color description.</returns>
        public readonly override System.String ToString() => $"Color<HSL> {{ Hue: {hue} Degrees , Saturation: {saturation} % , Luminance: {luminance} % }}";
    }
}