
using MP.Graphics;
using System.Drawing;
using MP.Graphics.Imaging;
using System.Drawing.Imaging;
using System.Runtime.CompilerServices;

namespace MP.Imaging
{
    public static unsafe class AdditionalImagingExtensions
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static Color GetColor(IColor clr) => Color.FromArgb(clr.A, clr.R, clr.G, clr.B);

        /// <summary>
        /// Converts the current <see cref="IImage"/> interface object to a new GDI+ <see cref="Bitmap"/> class.
        /// </summary>
        /// <param name="source">The image to convert.</param>
        /// <returns>The created bitmap.</returns>
        public static Bitmap ToBitmap(this IImage source)
        {
            Bitmap bm = new(source.Size.Width, source.Size.Height, PixelFormat.Format32bppArgb);
            IImage img = source;
            if (source.IsFlippedVertically) { img = source.FlipVertically(); }
            IColor[,] data = img.GetPixels2DPlane();
            if (source.IsFlippedVertically) { img.Dispose(); }
            for (System.Int32 Y = 0; Y < source.Size.Height; Y++)
            {
                for (System.Int32 X = 0; X < source.Size.Width; X++)
                {
                    bm.SetPixel(X, Y, GetColor(data[X, Y]));
                }
            }
            data = null;
            return bm;
        }

        /// <summary>
        /// Converts the current GDI+ bitmap image to a new instance of the <see cref="IImage"/> interface.
        /// </summary>
        /// <param name="bitmap">The GDI+ bitmap image to convert.</param>
        /// <returns>The converted image.</returns>
        public static IImage ToImage(this Bitmap bitmap)
        {
            // Try first to lock the bitmap bits , if it fails it will not allocate anything else.
            var data = bitmap.LockBits(new System.Drawing.Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
            // Create the image object. The Bitmap bits are packed as RGBA, although specified to retrieve them as ARGB.
            IImage imgobj = IImageFactory.CreateEmpty(new(bitmap.Size.Width , bitmap.Size.Height), ImagePixelFormat.RGBA);
            Unsafe.CopyBlockUnaligned(imgobj.NativePointer, data.Scan0.ToPointer(), imgobj.GetMemoryByteLength().ToUInt32());
            bitmap.UnlockBits(data);
            return imgobj;
        }
    }
}