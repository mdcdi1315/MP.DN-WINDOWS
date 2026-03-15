
using System;
using System.Collections.Generic;
using MP.NativeInterop.Windows.GDI;

namespace MP.Graphics.Imaging.WindowsBitmap
{
    // Hard-coded enum so that all the values are correctly filled into ARGB data organization.
    internal enum DecoderColorReference : System.Byte
    {
        Alpha,
        Red,
        Green,
        Blue
    }

    internal struct ColorDataDepiction
    {
        public DecoderColorReference Color;
        public System.Int32 Index;
        public System.Byte MaskToApply;
    }

    // A color decoder for 24/32-bit images.
    // The color data organization is depicted by bit fields,
    // which the decoder decodes out and converts it into ARGB sequences
    internal sealed class BitFieldDecoder
    {
        private System.Boolean hasalpha;
        private ColorDataDepiction[] mappings;

        // Gets a ColorDataDepiction where the color is into the data as the bit mask mandates to be.
        private static ColorDataDepiction GetColorDepiction(System.UInt32 bitmask , DecoderColorReference cref)
        {
            System.Byte[] decomposed = bitmask.GetBytes();
            System.Int32 indx = -1;
            System.Byte mask = 0;
            for (System.Int32 I = 0; I < decomposed.Length; I++)
            {
                if (decomposed[I] > 0)
                {
                    if (indx > -1)
                    {
                        throw new InvalidOperationException("Attempted to define more than 1 colors in the bitmask. This is invalid.");
                    }
                    // Keep the index and the mask,
                    // however continue searching to catch mistaken usage of bitfields.
                    mask = decomposed[I];
                    indx = I;
                }
            }
            return new() { Color = cref , Index = indx , MaskToApply = mask };
        }

        public BitFieldDecoder(BITFIELDS fields , System.UInt32 alpha)
        {
            List<ColorDataDepiction> deplist = new(4);
            ColorDataDepiction cdep = GetColorDepiction(alpha , DecoderColorReference.Alpha);
            if (hasalpha = (cdep.Index > -1)) {
                deplist.Add(cdep);
            }
            cdep = GetColorDepiction(fields.RedMask , DecoderColorReference.Red);
            if (cdep.Index > -1) { 
                deplist.Add(cdep);
            }
            cdep = GetColorDepiction(fields.GreenMask, DecoderColorReference.Green);
            if (cdep.Index > -1) {
                deplist.Add(cdep);
            }
            cdep = GetColorDepiction(fields.BlueMask , DecoderColorReference.Blue);
            if (cdep.Index > -1) {
                deplist.Add(cdep);
            }
            mappings = deplist.ToArray();
            deplist = null;
        }

        public unsafe void ToARGB(System.Byte* pdata, System.Byte[] rawbytes, System.Int32 startindex)
        {
            // The below loop decodes the color data from rawbytes as ARGB and storing these into pdata.
            // Note how the decode is happening:
            // The mappings define where to read exactly from rawbytes array and the index to save these to.
            foreach (var map in mappings)
            {
                pdata[(System.Int32)map.Color] = (rawbytes[startindex + map.Index] & map.MaskToApply).ToByte();
            }
            if (hasalpha == false) {
                // Possibly the format does not have an alpha channel,
                // but because we are decoding into ARGB we must set
                // 255 to the alpha channel so that the image is finally visible.
                pdata[(System.Int32)DecoderColorReference.Alpha] = 255;
            }
        }
    }
}