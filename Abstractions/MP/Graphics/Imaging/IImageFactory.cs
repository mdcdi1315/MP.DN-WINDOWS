
using System;
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
    }
}