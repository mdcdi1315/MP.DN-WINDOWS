
using MP;
using Microsoft.Win32.SafeHandles;

namespace System.Drawing
{
    /// <summary>
    /// Typed <see cref="IImage"/> implementation shared by <see cref="IImageExtensions"/> and <see cref="IImageFactory"/> classes.
    /// </summary>
    internal sealed class DefaultImage : IImage
    {
        private Size size;
        private System.Boolean flipped;
        private ImagePixelFormat pixfmt;
        private SafeLibcMemoryHandle mem;

        public DefaultImage(IImage other)
        {
            size = other.Size;
            pixfmt = other.PixelFormat;
            mem = new(other.GetMemoryByteLength());
            flipped = other.IsFlippedVertically;
        }

        public DefaultImage(Size size)
        {
            this.size = size;
        }

        public void InitializeMemoryWithSize(System.Int32 size)
        {
            mem?.Dispose();
            mem = new(size);
            mem.ZeroMemory();
        }

        public SafeBaseMemoryHandle Handle => mem;

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