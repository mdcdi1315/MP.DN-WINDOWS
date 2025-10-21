
using System;
using System.Runtime.CompilerServices;

namespace MP.Graphics.Imaging
{
    /// <summary>
    /// Defines extension methods for the <see cref="IImage"/> interface.
    /// </summary>
    public static unsafe class IImageExtensions
    {
        /// <summary>
        /// Gets the size of a single pixel , when the image has the format specified by the <paramref name="format"/> parameter.
        /// </summary>
        /// <param name="format">The image pixel format to query.</param>
        /// <returns>The size, in bytes, of a single pixel.</returns>
        /// <exception cref="ArgumentException">The specified format is out of range of valid values.</exception>
        public static System.Byte GetByteSize(this ImagePixelFormat format) => format switch {
            ImagePixelFormat.R => 1,
            ImagePixelFormat.RG => 2,
            ImagePixelFormat.RGB => 3,
            ImagePixelFormat.RGBA or
            ImagePixelFormat.ARGB => 4,
            _ => throw new ArgumentException($"The value given is invalid: {format}"),
        };

        /// <summary>
        /// Gets the size in bytes of the <see cref="IImage.NativePointer"/> property.
        /// </summary>
        /// <param name="image">The image interface to test.</param>
        /// <returns>The size, in bytes , of the <see cref="IImage.NativePointer"/> property pointer.</returns>
        public static System.Int32 GetMemoryByteLength(this IImage image)
                => image.Size.Width * image.Size.Height * GetByteSize(image.PixelFormat);

        /// <summary>Gets the size, in pixels, of the entire image.</summary>
        /// <param name="image">The image to inspect.</param>
        /// <returns>The total pixels contained into this image object.</returns>
        public static System.Int32 GetTotalPixels(this IImage image) => image.Size.Width * image.Size.Height;

        /// <summary>
        /// From the given image , it takes the image pixels and flips them vertically. <br />
        /// It can be also called on an already flipped vertically image to get the original representation of it.
        /// </summary>
        /// <param name="source">The image to flip it's bytes vertically.</param>
        /// <returns>A new instance of the <see cref="IImage"/> interface that has it's pixels flipped vertically.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="source"/> was null.</exception>
        public static IImage FlipVertically(this IImage source)
        {
            if (source is null) { throw new ArgumentNullException(nameof(source)); }
            DefaultImage ret = DefaultImage.CreateCopy(source);
			/*
			    The original code for this is located at stb_image.h file in https://github.com/nothings/stb
			
			    Copyright (c) 2017 Sean Barrett

			    Permission is hereby granted, free of charge, to any person obtaining a copy of
			    this software and associated documentation files (the "Software"), to deal in
			    the Software without restriction, including without limitation the rights to
			    use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies
			    of the Software, and to permit persons to whom the Software is furnished to do
			    so, subject to the following conditions:

			    The above copyright notice and this permission notice shall be included in all
			    copies or substantial portions of the Software.

			    THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
			    IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
			    FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
			    AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
			    LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
			    OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
			    SOFTWARE.
			*/
            System.Int32 bytes_per_row = source.Size.Width * source.PixelFormat.GetByteSize() , row;
            // mdcdi1315: Negate the flipping in the destination image. 
            // If not flipped before, it will be flipped; otherwise it is unflipped.
            ret.IsFlippedVertically = ! source.IsFlippedVertically;
            System.Byte* bytes = ret.NativePointer;
            IMemoryHandle temp = null;
            try {
                temp = SystemInfo.CreateNativeMemory(2048UL);
                for (row = 0; row < (source.Size.Height >> 1); row++)
                {
                    System.Byte* row0 = bytes + row * bytes_per_row;
                    System.Byte* row1 = bytes + (source.Size.Height - row - 1) * bytes_per_row;
                    System.Int32 bytes_left = bytes_per_row;
                    while (bytes_left > 0)
                    {
                        System.UInt32 bytes_copy = ((bytes_left < temp.MemoryLength) ? bytes_left : temp.MemoryLength).ToUInt32();
                        Unsafe.CopyBlockUnaligned(temp.MemoryPointer, row0, bytes_copy);
                        Unsafe.CopyBlockUnaligned(row0, row1, bytes_copy);
                        Unsafe.CopyBlockUnaligned(row1, temp.MemoryPointer, bytes_copy);
                        row0 += bytes_copy;
                        row1 += bytes_copy;
                        bytes_left -= bytes_copy.ToInt32();
                    }
                }
            } finally {
                temp?.Dispose();
            }
            return ret;
        }

        private static void RToRGBA(System.Byte* source, System.Byte* destination, int len)
        {
            for (System.Int32 I = 0; I < len; I++, source++, destination += 4)
            {
                destination[0] = *source;
                destination[1] = 0;
                destination[2] = 0;
                destination[3] = 255;
            }
        }

        private static void RGToRGBA(System.Byte* source, System.Byte* destination, int len)
        {
            for (System.Int32 I = 0; I < len; I++, source += 2, destination += 4)
            {
                destination[0] = *source;
                destination[1] = source[1];
                destination[2] = 0;
                destination[3] = 255;
            }
        }

        private static void RGBToRGBA(System.Byte* source, System.Byte* destination, int len)
        {
            for (System.Int32 I = 0; I < len; I++, source += 3, destination += 4)
            {
                destination[0] = *source;
                destination[1] = source[1];
                destination[2] = source[2];
                destination[3] = 255;
            }
        }

        private static void ARGBToRGBA(System.Byte* source, System.Byte* destination, int len)
        {
            for (System.Int32 I = 0; I < len; I++, source += 4, destination += 4)
            {
                destination[0] = source[1];
                destination[1] = source[2];
                destination[2] = source[3];
                destination[3] = *source;
            }
        }

        /// <summary>
        /// From the given image , it returns the same image that returns it's pixels packed as the RGBA format.
        /// </summary>
        /// <param name="source">The image to translate.</param>
        /// <returns>A new independent image object that represents the translated image as RGBA.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="source"/> was null.</exception>
        public static IImage TranslateToRGBA(this IImage source)
        {
            if (source is null) { throw new ArgumentNullException(nameof(source)); }
            // Specify the transformed image, preparing it for accomondating the data.
            DefaultImage ret = DefaultImage.CreateUninitialized(source.Size , source.IsFlippedVertically , ImagePixelFormat.RGBA);
            System.Byte* cpy = source.NativePointer;
            System.Byte* outimg = ret.NativePointer;
            // Specify source image run length.
            // Will be used by the copy loops to determine how many times they should run.
            int runlength = source.Size.Width * source.Size.Height; 
            switch (source.PixelFormat)
            {
                case ImagePixelFormat.R:
                    RToRGBA(cpy, outimg, runlength);
                    break;
                case ImagePixelFormat.RG:
                    RGToRGBA(cpy, outimg, runlength);
                    break;
                case ImagePixelFormat.RGB:
                    RGBToRGBA(cpy, outimg, runlength);
                    break;
                case ImagePixelFormat.ARGB:
                    ARGBToRGBA(cpy, outimg, runlength);
                    break;
                case ImagePixelFormat.RGBA:
                    // We can just directly do CopyBlockUnaligned, which it will be much faster than the other alternatives
                    Unsafe.CopyBlockUnaligned(outimg, cpy, (runlength * 4L).ToUInt32());
                    break;
            }
            return ret;
        }

        private static void RToARGB(System.Byte* source, System.Byte* destination, int len)
        {
            for (System.Int32 I = 0; I < len; I++ , source++ , destination += 4)
            {
                destination[0] = 255;
                destination[1] = *source;
                destination[2] = 0;
                destination[3] = 0;
            }
        }

        private static void RGToARGB(System.Byte* source , System.Byte* destination, int len)
        {
            for (System.Int32 I = 0; I < len; I++, source += 2, destination += 4)
            {
                destination[0] = 255;
                destination[1] = *source;
                destination[2] = source[1];
                destination[3] = 0;
            }
        }

        private static void RGBToARGB(System.Byte* source, System.Byte* destination, int len)
        {
            for (System.Int32 I = 0; I < len; I++, source += 3, destination += 4)
            {
                destination[0] = 255;
                destination[1] = *source;
                destination[2] = source[1];
                destination[3] = source[2];
            }
        }

        private static void RGBAToARGB(System.Byte* source, System.Byte* destination, int len)
        {
            for (System.Int32 I = 0; I < len; I++, source += 4, destination += 4)
            {
                destination[0] = source[3];
                destination[1] = *source;
                destination[2] = source[1];
                destination[3] = source[2];
            }
        }

        /// <summary>
        /// From the given image , it returns the same image that returns it's pixels packed as the ARGB format.
        /// </summary>
        /// <param name="source">The image to translate.</param>
        /// <returns>A new independent image object that represents the translated image as ARGB.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="source"/> was null.</exception>
        public static IImage TranslateToARGB(this IImage source)
        {
            if (source is null) { throw new ArgumentNullException(nameof(source)); }
            // Specify the transformed image, preparing it for accomondating the data.
            DefaultImage ret = DefaultImage.CreateUninitialized(source.Size, source.IsFlippedVertically, ImagePixelFormat.ARGB);
            System.Byte* cpy = source.NativePointer;
            System.Byte* outimg = ret.NativePointer;
            // Specify source image run length.
            // Will be used by the copy loops to determine how many times they should run.
            int runlength = source.Size.Width * source.Size.Height;
            switch (source.PixelFormat)
            {
                case ImagePixelFormat.R:
                    RToARGB(cpy , outimg , runlength);
                    break;
                case ImagePixelFormat.RG:
                    RGToARGB(cpy , outimg , runlength);
                    break;
                case ImagePixelFormat.RGB:
                    RGBToARGB(cpy , outimg , runlength);
                    break;
                case ImagePixelFormat.RGBA:
                    RGBAToARGB(cpy , outimg , runlength);
                    break;
                case ImagePixelFormat.ARGB:
                    // We can just directly do CopyBlockUnaligned, which it will be much faster than the other alternatives
                    Unsafe.CopyBlockUnaligned(outimg, cpy, (runlength * 4L).ToUInt32());
                    break;
            }
            return ret;
        }

        /// <summary>
        /// Copies the current image representation to a new independent image object.
        /// </summary>
        /// <param name="source">The image to copy data from.</param>
        /// <returns>The cloned image.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="source"/> was null.</exception>
        public static IImage Copy(this IImage source)
        {
            if (source is null) { throw new ArgumentNullException(nameof(source)); }
            return DefaultImage.CreateCopy(source);
        }

        /// <summary>Gets the pixel specified at the current image object.</summary>
        /// <param name="source">The source image to set the result to.</param>
        /// <param name="x">The x coordinate inside the image to get the pixel.</param>
        /// <param name="y">The y coordinate inside the image to get the pixel.</param>
        /// <returns>The retrieved pixel , casted to a <see cref="IColor"/> instance.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="source"/> was null.</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="x"/> and/or <paramref name="y"/> had invalid ranges.</exception>
        /// <exception cref="ArgumentException">The <paramref name="source"/>'s <see cref="IImage.PixelFormat"/> property had an invalid value.</exception>
        public static IColor GetPixel(this IImage source, System.Int32 x, System.Int32 y)
        {
            if (source is null) { throw new ArgumentNullException(nameof(source)); }
            if (x < 0 || y < 0) { throw new ArgumentOutOfRangeException(nameof(source), "Both X and Y parameters must not be negative."); }
            if (x >= source.Size.Width || y >= source.Size.Height) { throw new ArgumentOutOfRangeException(nameof(source), "Both X and Y parameters must be inside the image bounds."); }
            System.Byte* pbase = source.NativePointer + ((y * source.Size.Width + x) * source.PixelFormat.GetByteSize());
            return source.PixelFormat switch {
                ImagePixelFormat.R => new RGBColor(*pbase, 0, 0),
                ImagePixelFormat.RG => new RGBColor(pbase[0], pbase[1], 0),
                ImagePixelFormat.RGB => new RGBColor(pbase[0], pbase[1], pbase[2]),
                ImagePixelFormat.RGBA => new RGBAColor(pbase[0], pbase[1], pbase[2], pbase[3]),
                ImagePixelFormat.ARGB => new ARGBColor(pbase[0], pbase[1], pbase[2], pbase[3]),
                _ => throw new ArgumentException($"Invalid pixel format {source.PixelFormat}.", nameof(source)),
            };
        }

        /// <summary>Sets the specified pixel on the current image object.</summary>
        /// <param name="source">The image object where to set the new pixel to.</param>
        /// <param name="x">The x coordinate location inside the image to set the pixel.</param>
        /// <param name="y">The y coordinate location inside the image to set the pixel.</param>
        /// <param name="pixel">The new color of the specified pixel.</param>
        /// <exception cref="ArgumentNullException"><paramref name="source"/> was null.</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="x"/> and/or <paramref name="y"/> had invalid ranges.</exception>
        public static void SetPixel(this IImage source, System.Int32 x, System.Int32 y, IColor pixel)
        {
            if (source is null) { throw new ArgumentNullException(nameof(source)); }
            if (x < 0 || y < 0) { throw new ArgumentOutOfRangeException("", "Both X and Y parameters must not be negative."); }
            if (x >= source.Size.Width || y >= source.Size.Height) { throw new ArgumentOutOfRangeException("", "Both X and Y parameters must be inside the image bounds."); }
            System.Byte* pbase = source.NativePointer + ((y * source.Size.Width + x) * source.PixelFormat.GetByteSize());
            switch (source.PixelFormat)
            {
                case ImagePixelFormat.R:
                    *pbase = pixel.R;
                    break;
                case ImagePixelFormat.RG:
                    pbase[0] = pixel.R;
                    pbase[1] = pixel.G;
                    break;
                case ImagePixelFormat.RGB:
                    pbase[0] = pixel.R;
                    pbase[1] = pixel.G;
                    pbase[2] = pixel.B;
                    break;
                case ImagePixelFormat.RGBA:
                    pbase[0] = pixel.R;
                    pbase[1] = pixel.G;
                    pbase[2] = pixel.B;
                    pbase[3] = pixel.A;
                    break;
                case ImagePixelFormat.ARGB:
                    pbase[0] = pixel.A;
                    pbase[1] = pixel.R;
                    pbase[2] = pixel.G;
                    pbase[3] = pixel.B;
                    break;
            }
        }

        /// <summary>
        /// Gets the entire pixel array as a <see cref="IColor"/> 2-dimensional array defining the literal image coordinates.
        /// </summary>
        /// <param name="source">The source image to retrieve the image data.</param>
        /// <returns>The created image data.</returns>
        public static IColor[,] GetPixels2DPlane(this IImage source)
        {
            if (source is null) { throw new ArgumentNullException(nameof(source)); }
            IColor[,] colors = new IColor[source.Size.Width, source.Size.Height];
            System.Byte* ptemp;
            System.Int32 bs = source.PixelFormat.GetByteSize();
            for (System.Int32 Y = 0; Y < source.Size.Height; Y++)
            {
                for (System.Int32 X = 0; X < source.Size.Width; X++)
                {
                    ptemp = source.NativePointer + (Y * source.Size.Width + X) * bs;
                    colors[X, Y] = source.PixelFormat switch
                    {
                        ImagePixelFormat.R => new RGBColor(*ptemp, 0, 0),
                        ImagePixelFormat.RG => new RGBColor(ptemp[0], ptemp[1], 0),
                        ImagePixelFormat.RGB => new RGBColor(ptemp[0], ptemp[1], ptemp[2]),
                        ImagePixelFormat.RGBA => new RGBAColor(ptemp[0], ptemp[1], ptemp[2], ptemp[3]),
                        ImagePixelFormat.ARGB => new ARGBColor(ptemp[0], ptemp[1], ptemp[2], ptemp[3]),
                        _ => default
                    };
                }
            }
            return colors;
        }

        /// <summary>
        /// Saves the current raw pixel representation of the image to an existing stream. <br />
        /// Note that it does not write any other properties , such as the image size, just the image bytes themselves.
        /// </summary>
        /// <param name="source">The image to retrieve the pixels from.</param>
        /// <param name="stream">The stream to write the raw data to.</param>
        /// <exception cref="ArgumentNullException"><paramref name="stream"/> was null.</exception>
        /// <exception cref="ArgumentException"><paramref name="stream"/> was not writeable.</exception>
        public static void SaveRawToStream(this IImage source, System.IO.Stream stream)
        {
            const System.Int32 buffersize = 4096;
            if (stream is null) { throw new ArgumentNullException(nameof(stream)); }
            if (stream.CanWrite == false) { throw new ArgumentException("Stream must be writeable.", nameof(stream)); }
            System.Int32 length = source.GetMemoryByteLength();
            System.Int32 ch = length / buffersize, rem = length % buffersize;
            System.Byte[] temp = new System.Byte[buffersize];
            System.Byte* srcp = source.NativePointer;
            while (ch > 0)
            {
                fixed (System.Byte* dst = temp)
                {
                    Unsafe.CopyBlockUnaligned(dst, srcp, buffersize);
                }
                srcp += buffersize;
                stream.Write(temp, 0, temp.Length);
            }
            if (rem > 0)
            {
                fixed (System.Byte* dst = temp)
                {
                    Unsafe.CopyBlockUnaligned(dst, srcp, rem.ToUInt32());
                }
                stream.Write(temp, 0, rem);
            }
        }
    }
}