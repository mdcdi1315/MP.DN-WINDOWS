

using System;
using System.Runtime.InteropServices;

namespace MP.Graphics
{
    /// <summary>
    /// Represents a color that is instead reproduced by Hue, Luminance and Saturation. <br />
    /// Both conversions to RGB are provided.
    /// </summary>
    [StructLayout(LayoutKind.Explicit, Pack = 1, Size = 4)]
    public readonly struct HSLColor
    {
        [FieldOffset(0)]
        private readonly short hue;

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
            float lum = ((max + min) / 2) * 100f;
            luminance = (System.Byte)Math.Round(lum, 0);
            if ((saturation = (System.Byte)((min == max) ? 0d : Math.Round((lum > 50f) ? (max - min) / (2.0f - max - min) : (max - min) / (max + min), 0))) == 0)
            {
                hue = 0;
                return;
            }
            float h1;
            if (rgbratio[0] == max) // Maximum is the red channel
            {
                h1 = ((rgbratio[1] - rgbratio[2]) / (max - min));
            }
            else if (rgbratio[1] == max) // Maximum is the green channel
            {
                h1 = (2.0f + (rgbratio[2] - rgbratio[0]) / (max - min));
            }
            else // if (rgbratio[2] == max) // Maximum is the blue channel
            {
                h1 = (4.0f + (rgbratio[0] - rgbratio[1]) / (max - min));
            }
            h1 *= 60f;
            if (h1 < 0f)
            {
                h1 += 360f;
            }
            hue = (System.Byte)Math.Round(h1, 0);
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
        public HSLColor(short hue, byte saturation, byte luminance)
        {
            if (hue < 0 || hue > 360)
            {
                throw new ArgumentOutOfRangeException(nameof(hue), "Hue is a value in degrees and thus it cannot be less than 0 and more than 360.");
            }
            if (saturation < 0 || saturation > 100)
            {
                throw new ArgumentOutOfRangeException(nameof(saturation), "Saturation must be a value between 0 and 100.");
            }
            if (luminance < 0 || luminance > 100)
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
            if (saturation == 0)
            {
                // No saturation, color is in grayscale, thus map luminance directly
                byte color = (System.Byte)((luminance * 0.01f) * 255f);
                return new RGBColor(
                    color,
                    color,
                    color
                );
            }
            else
            {
                float t1 = (luminance < 50) ? (luminance * (1.0f + saturation)) : luminance + saturation - (luminance * saturation);
                float t2 = 2 * luminance - t1;
                float hue = this.hue / 360f;
                float[] colorst = new float[] { hue + 0.333f, hue, hue - 0.333f };
                for (int I = 0; I < colorst.Length; I++)
                {
                    if (colorst[I] > 1f)
                    {
                        colorst[I] -= 1;
                    }
                    else if (colorst[I] < 0f)
                    {
                        colorst[I] += 1;
                    }
                    if (6f * colorst[I] < 1)
                    {
                        colorst[I] = t2 + (t1 - t2) * 6f * colorst[I];
                    }
                    else if (2f * colorst[I] < 1)
                    {
                        colorst[I] = t1;
                    }
                    else if (3f * colorst[I] < 2)
                    {
                        colorst[I] = t2 + (t1 - t2) * (0.666f - colorst[I]) * 6f;
                    }
                    else
                    {
                        colorst[I] = t2;
                    }
                }

                return new RGBColor(
                    (System.Byte)Math.Round(colorst[0], 0),
                    (System.Byte)Math.Round(colorst[1], 0),
                    (System.Byte)Math.Round(colorst[2], 0)
                );
            }
        }

        /// <summary>Hue</summary>
        public readonly short Hue => hue;

        /// <summary>Saturation of the color</summary>
        public readonly byte Saturation => saturation;

        /// <summary>Luminance of the color</summary>
        public readonly byte Luminance => luminance;

        /// <summary>Gets a string describing this color.</summary>
        /// <returns>The color description.</returns>
        public override System.String ToString() => $"Color<HSL> {{ Hue: {hue} Degrees , Saturation: {saturation} % , Luminance: {luminance} % }}";
    }
}