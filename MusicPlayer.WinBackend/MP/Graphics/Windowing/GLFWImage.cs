
using System;
using MP.Annotations;
using MP.Graphics.Imaging;
using MP.Graphics.Windowing.Input;
using System.Runtime.CompilerServices;

namespace MP.Graphics.Windowing
{
    internal unsafe sealed class GLFWImage : IImage
    {
        private readonly bool fp;
        private System.Byte* data;
        private readonly Size size;

        private static void RToRGBA(System.Byte* source, System.Byte* destination, System.Int64 len)
        {
            for (System.Int64 I = 0; I < len; I++, source++, destination += 4)
            {
                destination[0] = *source;
                destination[1] = 0;
                destination[2] = 0;
                destination[3] = 255;
            }
        }

        private static void RGToRGBA(System.Byte* source, System.Byte* destination, System.Int64 len)
        {
            for (System.Int64 I = 0; I < len; I++, source += 2, destination += 4)
            {
                destination[0] = *source;
                destination[1] = source[1];
                destination[2] = 0;
                destination[3] = 255;
            }
        }

        private static void RGBToRGBA(System.Byte* source, System.Byte* destination, System.Int64 len)
        {
            for (System.Int64 I = 0; I < len; I++, source += 3, destination += 4)
            {
                destination[0] = *source;
                destination[1] = source[1];
                destination[2] = source[2];
                destination[3] = 255;
            }
        }

        private static void ARGBToRGBA(System.Byte* source, System.Byte* destination, System.Int64 len)
        {
            for (System.Int64 I = 0; I < len; I++, source += 4, destination += 4)
            {
                destination[0] = source[1];
                destination[1] = source[2];
                destination[2] = source[3];
                destination[3] = *source;
            }
        }

        private static void BGRAToRGBA(System.Byte* source, System.Byte* destination, System.Int64 len)
        {
            for (System.Int64 I = 0; I < len; I++, source += 4, destination += 4)
            {
                destination[0] = source[2];
                destination[1] = source[1];
                destination[2] = *source;
                destination[3] = source[3];
            }
        }

        [RequiresNativeLayer]
        public GLFWImage(IImage other)
        {
            ArgumentNullException.ThrowIfNull(other);

            size = other.Size;
            fp = other.IsFlippedVertically;
            long runlength = other.GetTotalPixels();
            data = (System.Byte*)SystemInfo.GetDefaultMemoryManager().Allocate(4UL * runlength.ToUInt64());
            
            switch (other.PixelFormat)
            {
                case ImagePixelFormat.R:
                    RToRGBA(other.NativePointer, data, runlength);
                    break;
                case ImagePixelFormat.RG:
                    RGToRGBA(other.NativePointer, data, runlength);
                    break;
                case ImagePixelFormat.RGB:
                    RGBToRGBA(other.NativePointer, data, runlength);
                    break;
                case ImagePixelFormat.ARGB:
                    ARGBToRGBA(other.NativePointer, data, runlength);
                    break;
                case ImagePixelFormat.BGRA:
                    BGRAToRGBA(other.NativePointer, data, runlength);
                    break;
                case ImagePixelFormat.RGBA:
                    // We can just directly do CopyBlockUnaligned, which it will be much faster than the other alternatives
                    Unsafe.CopyBlockUnaligned(data, other.NativePointer, (runlength * 4L).ToUInt32());
                    break;
            }
        }

        public unsafe byte* NativePointer => data;

        public ImagePixelFormat PixelFormat => ImagePixelFormat.RGBA;

        public bool IsFlippedVertically => fp;

        public Size Size => size;

        [RequiresNativeLayer]
        public void Dispose()
        {
            if (data is not null) {
                SystemInfo.GetDefaultMemoryManager().Free(data);
                data = null;
            }
        }
    }

    internal unsafe sealed class GLFWCursor : Cursor
    {
        private readonly Point hotspot;
        private readonly GLFWImage image;

        public GLFWCursor(IImage image, Point hotspot)
        {
            this.image = new(image);
            this.hotspot = hotspot;
        }

        public GLFWCursor(Cursor other)
        {
            ArgumentNullException.ThrowIfNull(other);
            image = new(other);
            hotspot = other.Hotspot;
        }

        public override unsafe byte* NativePointer => image.NativePointer;

        public override ImagePixelFormat PixelFormat => image.PixelFormat;

        public override bool IsFlippedVertically => image.IsFlippedVertically;

        public override Size Size => image.Size;

        public override Point Hotspot => hotspot;

        [RequiresNativeLayer]
        protected override void Dispose(bool disposing) { if (disposing) { image.Dispose(); } }
    }
}