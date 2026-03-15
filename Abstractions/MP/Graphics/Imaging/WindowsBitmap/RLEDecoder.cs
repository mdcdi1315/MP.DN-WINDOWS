
using System;
using MP.NativeInterop.Windows.GDI;

namespace MP.Graphics.Imaging.WindowsBitmap
{
    internal unsafe static class RLEDecoder
    {
        // Portions of the code used are adapted from https://github.com/Extender/BMPDecoder/blob/master/bmp.cpp
        // CPP code was adapted to C# by mdcdi1315 at 2025.

        private enum Escape : System.Byte
        {
            EndOfLine,
            EndOfBitmap,
            Delta
        }

        public static void ReadRLE8(System.Byte* p_target_argb, MP.IO.DataStream stream, BITMAPINFOHEADER info, RGBQUAD[] quads)
        {
            System.Int32 X = 0, Y = info.Height > 0 ? info.Height - 1 : 0;
            System.Byte* p_data = p_target_argb;
            System.Byte first_byte, second_byte;
            while (true)
            {
                first_byte = stream.ReadLiteralByte();
                second_byte = stream.ReadLiteralByte();
                if (first_byte == 0) {
                    // Process escape value, or go absolute mode.
                    switch ((Escape)second_byte)
                    {
                        case Escape.EndOfLine:
                            // End of scan line, update coordinates.
                            X = 0;
                            if (info.Height > 0) { Y--; } else { Y++; }
                            p_data = p_target_argb + (Y * info.Width * 4);
                            break;
                        case Escape.Delta:
                            X += stream.ReadLiteralByte();
                            Y += stream.ReadLiteralByte();
                            p_data = p_target_argb + (((Y * info.Width) + X) * 4);
                            break;
                        case Escape.EndOfBitmap:
                            return;
                        default:
                            // OK, absolute mode.
                            RGBQUAD q;
                            foreach (System.Byte b in stream.ReadBytes(second_byte))
                            {
                                q = quads[b];
                                *p_data = 255;
                                p_data[1] = q.R;
                                p_data[2] = q.G;
                                p_data[3] = q.B;
                                p_data += 4;
                                X++;
                            }
                            if (stream.DiscardBytes(UnsafeMethods.PadAlignment(2, second_byte)) == 0) { return; }
                            break;
                    }
                } else {
                    // Encoded mode.
                    RGBQUAD q = quads[second_byte];
                    for (byte times = 0; times < first_byte; times++, p_data += 4, X++)
                    {
                        *p_data = 255;
                        p_data[1] = q.R;
                        p_data[2] = q.G;
                        p_data[3] = q.B;
                    }
                }
            }
        }

        public static void ReadRLE4(System.Byte* p_target_argb, MP.IO.DataStream stream, BITMAPINFOHEADER info, RGBQUAD[] quads)
        {
            RGBQUAD q;
            System.Int32 X = 0, Y = info.Height > 0 ? info.Height - 1 : 0;
            System.Byte* p_data = p_target_argb;
            System.Byte first_byte, second_byte;
            while (true)
            {
                first_byte = stream.ReadLiteralByte();
                second_byte = stream.ReadLiteralByte();
                if (first_byte == 0) {
                    // Process escape value, or go absolute mode.
                    switch ((Escape)second_byte)
                    {
                        case Escape.EndOfLine:
                            // End of scan line, update coordinates.
                            X = 0;
                            if (info.Height > 0) { Y--; } else { Y++; }
                            p_data = p_target_argb + (Y * info.Width * 4);
                            break;
                        case Escape.Delta:
                            X += stream.ReadLiteralByte();
                            Y += stream.ReadLiteralByte();
                            p_data = p_target_argb + (((Y * info.Width) + X) * 4);
                            break;
                        case Escape.EndOfBitmap:
                            return;
                        default:
                            // OK, absolute mode.
                            // Note - color indexes in RLE 4 mode are coded as follows:
                            // FFFFSSSS
                            // Where F: The bits of the first pixel.
                            // Where S: The bits of the second pixel.

                            System.Byte[] data = stream.ReadBytes(second_byte / 2); // Note - the second_byte value is the number of pixels to draw!
                            int data_index = 0 , rem = 0;
                            for (byte I = 0; I < second_byte; I++, p_data += 4, X++, data_index = Math.DivRem(I , 2 , out rem))
                            {
                                q = quads[data[data_index] & ((rem == 0) ? 0b11110000 : 0b00001111)];
                                *p_data = 255;
                                p_data[1] = q.R;
                                p_data[2] = q.G;
                                p_data[3] = q.B;
                            }

                            if (stream.DiscardBytes(UnsafeMethods.PadAlignment(2, second_byte)) == 0) { return; }
                            break;
                    }
                } else {
                    // Encoded mode.
                    q = quads[second_byte & 0b11110000];
                    RGBQUAD q2 = quads[second_byte & 0b00001111];
                    for (byte times = 0; times < first_byte; times++, p_data += 4, X++)
                    {
                        *p_data = 255;
                        if (times % 2 == 0) {
                            p_data[1] = q.R;
                            p_data[2] = q.G;
                            p_data[3] = q.B;
                        } else {
                            p_data[1] = q2.R;
                            p_data[2] = q2.G;
                            p_data[3] = q2.B;
                        }
                    }
                }
            }
        }
    }
}