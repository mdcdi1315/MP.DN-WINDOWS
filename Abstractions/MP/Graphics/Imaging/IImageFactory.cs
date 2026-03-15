
using System;
using MP.Random;
using System.Runtime.CompilerServices;

namespace MP.Graphics.Imaging
{
    /// <summary>
    /// Defines static methods for creating fast in-memory <see cref="IImage"/>-derived objects.
    /// </summary>
    [Annotations.RequiresNativeLayer]
    public static unsafe class IImageFactory
    {
        /// <summary>
        /// Creates a new <see cref="IImage"/> object from the specified raw RGB data and the desired image size.
        /// </summary>
        /// <param name="rgb">The raw RGB data to initialize the <see cref="IImage"/> object from.</param>
        /// <param name="desiredsize">The desired size of the newly created image.</param>
        /// <returns>The image object, having it's RGB data copied from the specified array.</returns>
        /// <exception cref="OutOfMemoryException">Not enough memory to create the new image object.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="rgb"/> was <see langword="null"/>.</exception>
        /// <exception cref="ArgumentException">The size of the array was not equal to the desired dimensions * 3.</exception>
        public static IImage FromRGB(System.Byte[] rgb, Size desiredsize)
        {
            ArgumentNullException.ThrowIfNull(rgb);
            if ((((long)desiredsize.Width * desiredsize.Height) * 4L) != rgb.LongLength)
            {
                throw new ArgumentException($"The array does not contain enough or valid data to be an RGB image with size {desiredsize} .");
            }
            DefaultImage dfi = new(desiredsize);
            dfi.PixelFormat = ImagePixelFormat.RGB;
            dfi.InitializeMemoryWithSize(dfi.GetMemoryByteLength());
            Unsafe.CopyBlockUnaligned(ref dfi.NativePointer[0], ref rgb[0], rgb.LongLength.ToUInt32());
            return dfi;
        }

        /// <summary>
        /// Creates a new <see cref="IImage"/> object from the specified raw ARGB data and the desired image size.
        /// </summary>
        /// <param name="argb">The raw ARGB data to initialize the <see cref="IImage"/> object from.</param>
        /// <param name="desiredsize">The desired size of the newly created image.</param>
        /// <returns>The image object, having it's ARGB data copied from the specified array.</returns>
        /// <exception cref="OutOfMemoryException">Not enough memory to create the new image object.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="argb"/> was <see langword="null"/>.</exception>
        /// <exception cref="ArgumentException">The size of the array was not equal to the desired dimensions * 4.</exception>
        public static IImage FromARGB(System.Byte[] argb, Size desiredsize)
        {
            ArgumentNullException.ThrowIfNull(argb);
            if ((((long)desiredsize.Width * desiredsize.Height) * 4L) != argb.LongLength)
            {
                throw new ArgumentException($"The array does not contain enough or valid data to be an ARGB image with size {desiredsize} .");
            }
            DefaultImage dfi = new(desiredsize);
            dfi.PixelFormat = ImagePixelFormat.ARGB;
            dfi.InitializeMemoryWithSize(dfi.GetMemoryByteLength());
            Unsafe.CopyBlockUnaligned(ref dfi.NativePointer[0], ref argb[0], argb.LongLength.ToUInt32());
            return dfi;
        }

        /// <summary>
        /// Creates a new <see cref="IImage"/> object from the specified raw RGBA data and the desired image size.
        /// </summary>
        /// <param name="rgba">The raw RGBA data to initialize the <see cref="IImage"/> object from.</param>
        /// <param name="desiredsize">The desired size of the newly created image.</param>
        /// <returns>The image object, having it's RGBA data copied from the specified array.</returns>
        /// <exception cref="OutOfMemoryException">Not enough memory to create the new image object.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="rgba"/> was <see langword="null"/>.</exception>
        /// <exception cref="ArgumentException">The size of the array was not equal to the desired dimensions * 4.</exception>
        public static IImage FromRGBA(System.Byte[] rgba, Size desiredsize)
        {
            ArgumentNullException.ThrowIfNull(rgba);
            if ((((long)desiredsize.Width * desiredsize.Height) * 4L) != rgba.LongLength)
            {
                throw new ArgumentException($"The array does not contain enough or valid data to be an RGBA image with size {desiredsize} .");
            }
            DefaultImage dfi = new(desiredsize);
            dfi.PixelFormat = ImagePixelFormat.RGBA;
            dfi.InitializeMemoryWithSize(dfi.GetMemoryByteLength());
            Unsafe.CopyBlockUnaligned(ref dfi.NativePointer[0], ref rgba[0], rgba.LongLength.ToUInt32());
            return dfi;
        }

        /// <summary>
        /// Creates a new <see cref="IImage"/> object from the specified raw BGRA data and the desired image size.
        /// </summary>
        /// <param name="bgra">The raw BGRA data to initialize the <see cref="IImage"/> object from.</param>
        /// <param name="desiredsize">The desired size of the newly created image.</param>
        /// <returns>The image object, having it's BGRA data copied from the specified array.</returns>
        /// <exception cref="OutOfMemoryException">Not enough memory to create the new image object.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="bgra"/> was <see langword="null"/>.</exception>
        /// <exception cref="ArgumentException">The size of the array was not equal to the desired dimensions * 4.</exception>
        public static IImage FromBGRA(System.Byte[] bgra, Size desiredsize)
        {
            ArgumentNullException.ThrowIfNull(bgra);
            long length = bgra.LongLength;
            if ((((long)desiredsize.Width * desiredsize.Height) * 4L) != length)
            {
                throw new ArgumentException($"The array does not contain enough or valid data to be an RGBA image with size {desiredsize} .");
            }
            DefaultImage dfi = DefaultImage.CreateUninitialized(desiredsize, false, ImagePixelFormat.RGBA);
            Unsafe.CopyBlockUnaligned(ref dfi.NativePointer[0], ref bgra[0], length.ToUInt32());
            return dfi;
        }

        /// <summary>
        /// Creates a new <see cref="IImage"/> object from the specified 2-dimensional <see cref="IColor"/> array. <br />
        /// The image's dimensions are auto-determined from the array itself.
        /// </summary>
        /// <param name="cls">The color array to define the image data as well as it's dimensions.</param>
        /// <returns>The newly created <see cref="IImage"/> object describing the image.</returns>
        /// <exception cref="OutOfMemoryException">Not enough memory to create the new image object.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="cls"/> was <see langword="null"/>.</exception>
        /// <exception cref="ArgumentException">The size of the array was not equal to the desired dimensions * 4.</exception>
        public static IImage FromColors(IColor[,] cls)
        {
            ArgumentNullException.ThrowIfNull(cls);
            Size se = new(cls.GetLength(0), cls.GetLength(1));
            DefaultImage img = DefaultImage.CreateUninitialized(se, false, ImagePixelFormat.ARGB);
            IColor cl;
            System.Byte* temp;
            for (System.Int32 Y = 0; Y < se.Height; Y++)
            {
                for (System.Int32 X = 0; X < se.Width; X++)
                {
                    cl = cls[X, Y];
                    temp = img.NativePointer + (Y * se.Width + X) * 4;
                    temp[0] = cl.A;
                    temp[1] = cl.R;
                    temp[2] = cl.G;
                    temp[3] = cl.B;
                }
            }
            return img;
        }

        /// <summary>
        /// Creates an empty <see cref="IImage"/> object whose colors are to be specified by the caller. <br />
        /// Note: The data containing by the returned image object are UNDEFINED. That means, the bytes can be whatever values, and not necessarily zeroes.
        /// </summary>
        /// <param name="size">The desired dimensions of the new image object.</param>
        /// <param name="format">The pixel format to use for completeing the image data.</param>
        /// <returns>The newly created <see cref="IImage"/> object describing the empty image whose colors must be filled in by the caller.</returns>
        /// <exception cref="OutOfMemoryException">Not enough memory to create the new image object.</exception>
        /// <exception cref="ArgumentException">One of the image dimensions was less than 1 pixel.</exception>
        public static IImage CreateEmpty(Size size, ImagePixelFormat format)
        {
            if (size.Height < 1 || size.Width < 1) {
                throw new ArgumentException("The image dimensions must not be negative or zero!!");
            }
            return DefaultImage.CreateUninitialized(size, false, format);
        }

        private static void CreateRandom_R(IRandomSource source, System.Byte* data, System.Int64 len)
        {
            float f;
            for (System.Int64 I = 0L; I < len; I++, data++)
            {
                f = source.NextSingle();
                *data = (byte)(f * 255f);
            }
        }

        private static void CreateRandom_RG(IRandomSource source, System.Byte* data, System.Int64 len)
        {
            float r, g;
            for (System.Int64 I = 0L; I < len; I++, data += 2)
            {
                r = source.NextSingle();
                g = source.NextSingle();
                *data = (byte)(r * 255f);
                data[1] = (byte)(g * 255f);
            }
        }

        private static void CreateRandom_RGB(IRandomSource source, System.Byte* data, System.Int64 len)
        {
            float r, g, b;
            for (System.Int64 I = 0L; I < len; I++, data += 3)
            {
                r = source.NextSingle();
                g = source.NextSingle();
                b = source.NextSingle();
                *data = (byte)(r * 255f);
                data[1] = (byte)(g * 255f);
                data[2] = (byte)(b * 255f);
            }
        }

        private static void CreateRandom_ARGB(IRandomSource source, System.Byte* data, System.Int64 len)
        {
            float r, g, b, a;
            for (System.Int64 I = 0L; I < len; I++, data += 4)
            {
                r = source.NextSingle();
                g = source.NextSingle();
                b = source.NextSingle();
                a = source.NextSingle();
                *data = (byte)(a * 255f);
                data[1] = (byte)(r * 255f);
                data[2] = (byte)(g * 255f);
                data[3] = (byte)(b * 255f);
            }
        }

        private static void CreateRandom_BGRA(IRandomSource source, System.Byte* data, System.Int64 len)
        {
            float r, g, b, a;
            for (System.Int64 I = 0L; I < len; I++, data += 4)
            {
                r = source.NextSingle();
                g = source.NextSingle();
                b = source.NextSingle();
                a = source.NextSingle();
                *data = (byte)(b * 255f);
                data[1] = (byte)(g * 255f);
                data[2] = (byte)(r * 255f);
                data[3] = (byte)(a * 255f);
            }
        }

        private static void CreateRandom_RGBA(IRandomSource source, System.Byte* data, System.Int64 len)
        {
            float r, g, b, a;
            for (System.Int64 I = 0L; I < len; I++, data += 4)
            {
                r = source.NextSingle();
                g = source.NextSingle();
                b = source.NextSingle();
                a = source.NextSingle();
                *data = (byte)(r * 255f);
                data[1] = (byte)(g * 255f);
                data[2] = (byte)(b * 255f);
                data[3] = (byte)(a * 255f);
            }
        }

        /// <summary>Creates an image with random pixels within.</summary>
        /// <param name="size">The size of the random image.</param>
        /// <param name="format">The desired image format.</param>
        /// <returns>A new <see cref="IImage"/> object containing random pixels.</returns>
        /// <exception cref="ArgumentException">One of the fields of the <paramref name="size"/> parameter is less than 1.</exception>
        public static IImage CreateRandom(Size size, ImagePixelFormat format) => CreateRandom(size, format, new Xoroshiro256PlusPlus());

        /// <summary>Creates an image with random pixels within.</summary>
        /// <param name="size">The size of the random image.</param>
        /// <param name="format">The desired image format.</param>
        /// <param name="random_source">The <see cref="IRandomSource"/> providing the random generator to use.</param>
        /// <returns>A new <see cref="IImage"/> object containing random pixels.</returns>
        /// <exception cref="ArgumentException">One of the fields of the <paramref name="size"/> parameter is less than 1.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="random_source"/> is <see langword="null"/>.</exception>
        public static IImage CreateRandom(Size size, ImagePixelFormat format, IRandomSource random_source)
        {
            ArgumentNullException.ThrowIfNull(random_source);
            if (size.Height < 1 || size.Width < 1) {
                throw new ArgumentException("The image dimensions must not be negative or zero!!");
            } else {
                DefaultImage img = DefaultImage.CreateUninitialized(size, false, format);
                System.Int64 length = img.GetTotalPixels();

                switch (format)
                {
                    case ImagePixelFormat.R:
                        CreateRandom_R(random_source, img.NativePointer, length);
                        break;
                    case ImagePixelFormat.RG:
                        CreateRandom_RG(random_source, img.NativePointer, length);
                        break;
                    case ImagePixelFormat.RGB:
                        CreateRandom_RGB(random_source, img.NativePointer, length);
                        break;
                    case ImagePixelFormat.ARGB:
                        CreateRandom_ARGB(random_source, img.NativePointer, length);
                        break;
                    case ImagePixelFormat.RGBA:
                        CreateRandom_RGBA(random_source, img.NativePointer, length);
                        break;
                    case ImagePixelFormat.BGRA:
                        CreateRandom_BGRA(random_source, img.NativePointer, length);
                        break;
                }

                return img;
            }
        }
    }
}