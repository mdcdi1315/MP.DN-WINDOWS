
using System.Runtime.CompilerServices;

namespace MP.Graphics.Imaging
{
    /// <summary>
    /// Typed <see cref="IImage"/> implementation shared by <see cref="IImageExtensions"/> and <see cref="IImageFactory"/> classes.
    /// </summary>
    internal sealed class DefaultImage : IImage
    {
        private Size size;
        private IMemoryHandle mem;
        private System.Boolean flipped;
        private ImagePixelFormat pixfmt;

        private DefaultImage() 
        {
            mem = null;
            size = default;
            flipped = false;
            pixfmt = default;
        }

        public DefaultImage(Size size) : this() => this.size = size;

        public static DefaultImage CreateUninitialized(Size desired, bool flipped, ImagePixelFormat pixelformat)
        {
            DefaultImage img = new();
            img.pixfmt = pixelformat;
            img.size = desired;
            img.flipped = flipped;
            img.InitializeMemoryWithSize(img.GetMemoryByteLength());
            return img;
        }

        public static unsafe DefaultImage CreateCopy(IImage source)
        {
            DefaultImage img = new();
            img.size = source.Size;
            img.pixfmt = source.PixelFormat;
            img.flipped = source.IsFlippedVertically;
            uint size = source.GetMemoryByteLength().ToUInt32();
            img.mem = SystemInfo.CreateNativeMemory(size);
            Unsafe.CopyBlockUnaligned(img.mem.MemoryPointer, source.NativePointer, size);
            return img;
        }

        public void InitializeMemoryWithSize(System.Int32 size)
        {
            mem?.Dispose();
            mem = SystemInfo.CreateNativeMemory(size.ToUInt64());
            mem.ZeroMemory();
        }

        public unsafe System.Byte* NativePointer => mem.MemoryPointer;

        public ImagePixelFormat PixelFormat
        {
            get => pixfmt;
            set => pixfmt = value;
        }

        public System.Boolean IsFlippedVertically
        {
            get => flipped;
            set => flipped = value;
        }

        public Size Size => size;

        public void Dispose()
        {
            mem?.Dispose();
            mem = null;
        }
    }
}