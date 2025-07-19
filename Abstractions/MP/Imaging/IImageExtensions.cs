
using System;
using System.Drawing;
using System.Runtime.CompilerServices;

namespace MP.Imaging
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
        public static System.Byte GetByteSize(this ImagePixelFormat format)
        {
            switch (format)
            {
                case ImagePixelFormat.R:
                    return 1;
                case ImagePixelFormat.RG:
                    return 2;
                case ImagePixelFormat.RGB:
                    return 3;
                case ImagePixelFormat.RGBA:
                case ImagePixelFormat.ARGB:
                    return 4;
                default:
                    throw new ArgumentException($"The value given is invalid: {format}");
            }
        }

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
        /// From the given image , it takes the image pixels and flips them vertically.
        /// </summary>
        /// <param name="source">The image to flip it's bytes vertically.</param>
        /// <returns>A new instance of the <see cref="IImage"/> interface that has it's pixels flipped vertically.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="source"/> was null.</exception>
        public static IImage FlipVertically(this IImage source)
        {
            if (source is null) { throw new ArgumentNullException(nameof(source)); }
            DefaultImage ret = new(source);
            System.Int32 bytes_per_row = (source.Size.Width * source.PixelFormat.GetByteSize());
            System.Int32 row;
            System.Byte* bytes = ret.NativePointer;
            Unsafe.CopyBlockUnaligned(bytes, source.NativePointer, GetMemoryByteLength(source).ToUInt32());
            IMemoryHandle temp = SystemInfo.CreateNativeMemory(2048UL);
            for (row = 0; row < (source.Size.Height >> 1); row++)
            {
                System.Byte* row0 = bytes + row * bytes_per_row;
                System.Byte* row1 = bytes + (source.Size.Height - row - 1) * bytes_per_row;
                System.Int32 bytes_left = bytes_per_row;
                while (bytes_left > 0)
                {
                    System.UInt32 bytes_copy = (bytes_left < temp.MemoryLength) ? bytes_left.ToUInt32() : temp.MemoryLength.ToUInt32();
                    Unsafe.CopyBlockUnaligned(temp.MemoryPointer, row0, bytes_copy);
                    Unsafe.CopyBlockUnaligned(row0, row1, bytes_copy);
                    Unsafe.CopyBlockUnaligned(row1, temp.MemoryPointer, bytes_copy);
                    row0 += bytes_copy;
                    row1 += bytes_copy;
                    bytes_left -= bytes_copy.ToInt32();
                }
            }
            return ret;
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
            DefaultImage ret = new(source);
            ret.PixelFormat = ImagePixelFormat.RGBA;
            ret.InitializeMemoryWithSize(source.Size.Width * source.Size.Height * 4);
            System.Byte* cpy = source.NativePointer;
            System.Byte* outimg = ret.NativePointer;
            for (System.Int32 Y = 0; Y < source.Size.Height; Y++)
            {
                for (System.Int32 X = 0; X < source.Size.Width; X++)
                {
                    switch (source.PixelFormat)
                    {
                        case ImagePixelFormat.R:
                            outimg[0] = *cpy;
                            outimg[1] = 0;
                            outimg[2] = 0;
                            outimg[3] = 255;
                            cpy++;
                            break;
                        case ImagePixelFormat.RG:
                            outimg[0] = cpy[0];
                            outimg[1] = cpy[1];
                            outimg[2] = 0;
                            outimg[3] = 255;
                            cpy += 2;
                            break;
                        case ImagePixelFormat.RGB:
                            outimg[0] = cpy[0];
                            outimg[1] = cpy[1];
                            outimg[2] = cpy[2];
                            outimg[3] = 255;
                            cpy += 3;
                            break;
                        case ImagePixelFormat.RGBA:
                            outimg[0] = cpy[0];
                            outimg[1] = cpy[1];
                            outimg[2] = cpy[2];
                            outimg[3] = cpy[3];
                            cpy += 4;
                            break;
                        case ImagePixelFormat.ARGB:
                            outimg[0] = cpy[1];
                            outimg[1] = cpy[2];
                            outimg[2] = cpy[3];
                            outimg[3] = cpy[0];
                            cpy += 4;
                            break;
                    }
                    outimg += 4;
                }
            }
            return ret;
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
            DefaultImage ret = new(source);
            ret.PixelFormat = ImagePixelFormat.ARGB;
            ret.InitializeMemoryWithSize(source.Size.Width * source.Size.Height * 4);
            System.Byte* cpy = source.NativePointer;
            System.Byte* outimg = ret.NativePointer;
            for (System.Int32 Y = 0; Y < source.Size.Height; Y++)
            {
                for (System.Int32 X = 0; X < source.Size.Width; X++)
                {
                    switch (source.PixelFormat)
                    {
                        case ImagePixelFormat.R:
                            outimg[0] = 255;
                            outimg[1] = *cpy;
                            outimg[2] = 0;
                            outimg[3] = 0;
                            cpy++;
                            break;
                        case ImagePixelFormat.RG:
                            outimg[0] = 255;
                            outimg[1] = cpy[0];
                            outimg[2] = cpy[1];
                            outimg[3] = 0;
                            cpy += 2;
                            break;
                        case ImagePixelFormat.RGB:
                            outimg[0] = 255;
                            outimg[1] = cpy[0];
                            outimg[2] = cpy[1];
                            outimg[3] = cpy[2];
                            cpy += 3;
                            break;
                        case ImagePixelFormat.RGBA:
                            outimg[0] = cpy[3];
                            outimg[1] = cpy[0];
                            outimg[2] = cpy[1];
                            outimg[3] = cpy[2];
                            cpy += 4;
                            break;
                        case ImagePixelFormat.ARGB:
                            outimg[0] = cpy[0];
                            outimg[1] = cpy[1];
                            outimg[2] = cpy[2];
                            outimg[3] = cpy[3];
                            cpy += 4;
                            break;
                    }
                    outimg += 4;
                }
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
            DefaultImage ret = new(source);
            Unsafe.CopyBlockUnaligned(source.NativePointer, ret.NativePointer, source.GetMemoryByteLength().ToUInt32());
            return ret;
        }

        /// <summary>Gets the pixel specified at the current image object.</summary>
        /// <param name="source">The source image to set the result to.</param>
        /// <param name="x">The x coordinate inside the image to get the pixel.</param>
        /// <param name="y">The y coordinate inside the image to get the pixel.</param>
        /// <returns>The retrieved pixel , casted to a <see cref="Color"/> instance.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="source"/> was null.</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="x"/> and/or <paramref name="y"/> had invalid ranges.</exception>
        /// <exception cref="ArgumentException">The <paramref name="source"/>'s <see cref="IImage.PixelFormat"/> property had an invalid value.</exception>
        public static Color GetPixel(this IImage source, System.Int32 x, System.Int32 y)
        {
            if (source is null) { throw new ArgumentNullException(nameof(source)); }
            if (x < 0 || y < 0) { throw new ArgumentOutOfRangeException(nameof(source), "Both X and Y parameters must not be negative."); }
            if (x >= source.Size.Width || y >= source.Size.Height) { throw new ArgumentOutOfRangeException(nameof(source), "Both X and Y parameters must be inside the image bounds."); }
            System.Byte* pbase = source.NativePointer + ((y * source.Size.Width + x) * source.PixelFormat.GetByteSize());
            return source.PixelFormat switch {
                ImagePixelFormat.R => Color.FromArgb(255, pbase[0], 0, 0),
                ImagePixelFormat.RG => Color.FromArgb(255, pbase[0], pbase[1], 0),
                ImagePixelFormat.RGB => Color.FromArgb(255, pbase[0], pbase[1], pbase[2]),
                ImagePixelFormat.RGBA => Color.FromArgb(pbase[3], pbase[0], pbase[1], pbase[2]),
                ImagePixelFormat.ARGB => Color.FromArgb(pbase[0], pbase[1], pbase[2], pbase[3]),
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
        public static void SetPixel(this IImage source, System.Int32 x, System.Int32 y, Color pixel)
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
        /// Gets the entire pixel array as a <see cref="Color"/> 2-dimensional array defining the literal image coordinates.
        /// </summary>
        /// <param name="source">The source image to retrieve the image data.</param>
        /// <returns>The created image data.</returns>
        public static Color[,] GetPixels2DPlane(this IImage source)
        {
            if (source is null) { throw new ArgumentNullException(nameof(source)); }
            Color[,] colors = new Color[source.Size.Width, source.Size.Height];
            System.Byte* ptemp;
            System.Int32 bs = source.PixelFormat.GetByteSize();
            for (System.Int32 Y = 0; Y < source.Size.Height; Y++)
            {
                for (System.Int32 X = 0; X < source.Size.Width; X++)
                {
                    ptemp = source.NativePointer + (Y * source.Size.Width + X) * bs;
                    colors[X, Y] = source.PixelFormat switch
                    {
                        ImagePixelFormat.R => Color.FromArgb(255, ptemp[0], 0, 0),
                        ImagePixelFormat.RG => Color.FromArgb(255, ptemp[0], ptemp[1], 0),
                        ImagePixelFormat.RGB => Color.FromArgb(255, ptemp[0], ptemp[1], ptemp[2]),
                        ImagePixelFormat.RGBA => Color.FromArgb(ptemp[3], ptemp[0], ptemp[1], ptemp[2]),
                        ImagePixelFormat.ARGB => Color.FromArgb(ptemp[0], ptemp[1], ptemp[2], ptemp[3]),
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