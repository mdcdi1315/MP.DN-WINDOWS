using MP.Graphics.Imaging;
using System;

namespace MP.Graphics.Windowing
{
    /// <summary>
    /// A <see cref="Cursor"/> class is a special sub-class of the <see cref="IImage"/> interface that allows images to be passed as cursors in a window.
    /// </summary>
    public abstract class Cursor : IImage
    {
        private unsafe sealed class IImageWrappedCursor : Cursor
        {
            private IImage image;
            private Point hotspot;

            public IImageWrappedCursor(IImage image, Point hotspot)
            {
                this.image = image;
                this.hotspot = hotspot;
            }

            public override bool IsFlippedVertically => image.IsFlippedVertically;

            public override Size Size => image.Size;

            public override Point Hotspot => hotspot;

            public override byte* NativePointer => image.NativePointer;

            public override ImagePixelFormat PixelFormat => image.PixelFormat;

            public override void Dispose() => image.Dispose();
        }

        /// <summary>
        /// Creates a default <see cref="Cursor"/> instance by wrapping the specified <see cref="IImage"/> object.
        /// </summary>
        /// <param name="image">The image object to be wrapped.</param>
        /// <param name="hotspot">The hotspot to be manually specified.</param>
        /// <returns>A new <see cref="Cursor"/> instance.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="image"/> was <see langword="null"/>.</exception>
        public static Cursor FromExistingImage(IImage image , Point hotspot)
        {
            ArgumentNullException.ThrowIfNull(image);
            return new IImageWrappedCursor(image, hotspot);
        }

        /// <inheritdoc/>
        public abstract unsafe byte* NativePointer { get; }

        /// <inheritdoc/>
        public abstract ImagePixelFormat PixelFormat { get; }

        /// <inheritdoc/>
        public abstract bool IsFlippedVertically {  get; }

        /// <inheritdoc/>
        public abstract Size Size { get; }

        /// <summary>
        /// Gets the hotspot of this <see cref="Cursor"/> instance , specifying the pixel point where the cursor actually interacts with the window's UI elements.
        /// </summary>
        public abstract Point Hotspot { get; }

        /// <inheritdoc/>
        public abstract void Dispose();
    }
}
