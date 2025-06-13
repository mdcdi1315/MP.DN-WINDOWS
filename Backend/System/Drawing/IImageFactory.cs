

using MP;
using System.Runtime.CompilerServices;

namespace System.Drawing
{
    /// <summary>
    /// Defines static methods for creating fast in-memory <see cref="IImage"/>-derived objects.
    /// </summary>
    public static unsafe class IImageFactory
    {
        public static IImage FromRGB(System.Byte[] rgb , Size desiredsize)
        {
            if (rgb is null) { throw new ArgumentNullException(nameof(rgb)); }
            if (desiredsize.Width * desiredsize.Height * 3 != rgb.LongLength) {
                throw new ArgumentException($"The array does not contain enough or valid data to be an RGB image with size {desiredsize} .");
            }
            DefaultImage dfi = new(desiredsize);
            dfi.PixelFormat = ImagePixelFormat.RGB;
            dfi.InitializeMemoryWithSize(dfi.GetMemoryByteLength());
            Unsafe.CopyBlockUnaligned(ref dfi.NativePointer[0], ref rgb[0], rgb.LongLength.ToUInt32());
            return dfi;
        }

        public static IImage FromARGB(System.Byte[] argb, Size desiredsize)
        {
            if (argb is null) { throw new ArgumentNullException(nameof(argb)); }
            if (desiredsize.Width * desiredsize.Height * 4 != argb.LongLength) {
                throw new ArgumentException($"The array does not contain enough or valid data to be an ARGB image with size {desiredsize} .");
            }
            DefaultImage dfi = new(desiredsize);
            dfi.PixelFormat = ImagePixelFormat.ARGB;
            dfi.InitializeMemoryWithSize(dfi.GetMemoryByteLength());
            Unsafe.CopyBlockUnaligned(ref dfi.NativePointer[0], ref argb[0], argb.LongLength.ToUInt32());
            return dfi;
        }

        public static IImage FromRGBA(System.Byte[] rgba, Size desiredsize)
        {
            if (rgba is null) { throw new ArgumentNullException(nameof(rgba)); }
            if (desiredsize.Width * desiredsize.Height * 4 != rgba.LongLength)
            {
                throw new ArgumentException($"The array does not contain enough or valid data to be an RGBA image with size {desiredsize} .");
            }
            DefaultImage dfi = new(desiredsize);
            dfi.PixelFormat = ImagePixelFormat.RGBA;
            dfi.InitializeMemoryWithSize(dfi.GetMemoryByteLength());
            Unsafe.CopyBlockUnaligned(ref dfi.NativePointer[0], ref rgba[0], rgba.LongLength.ToUInt32());
            return dfi;
        }
    
        public static IImage FromColors(Color[,] cls)
        {
            if (cls is null) { throw new ArgumentNullException(nameof(cls)); }
            Size se = new(cls.GetLength(0) , cls.GetLength(1));
            DefaultImage img = new(se);
            img.PixelFormat = ImagePixelFormat.ARGB;
            img.InitializeMemoryWithSize(img.GetMemoryByteLength());
            Color cl;
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
    }
}