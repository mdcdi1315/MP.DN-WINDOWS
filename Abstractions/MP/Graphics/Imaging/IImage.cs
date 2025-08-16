
using System;

namespace MP.Graphics.Imaging
{
    /// <summary>
    /// Defines an abstraction for loadable images that can be loaded into any window/UI contexts. <br />
    /// Note that most and important imaging manipulations are provided through extension methods. <br />
    /// Purpose of this interface is to find how imaging can be done more easier, while focusing on flexibility. <br />
    /// In most cases, you should not implement this interface for any reason; <br />
    /// the only implementers of this interface should be image decoders.
    /// </summary>
    public unsafe interface IImage : IDisposable
    {
        /// <summary>
        /// Gets the raw bitmap image bytes. <br />
        /// It is the object's responsibility to free this pointer through <see cref="IDisposable.Dispose"/>. <br />
        /// If you need the memory size of this pointer , use the <see cref="IImageExtensions.GetMemoryByteLength(IImage)"/> extension method.
        /// </summary>
        public System.Byte* NativePointer { get; }

        /// <summary>
        /// Depicts the organization of data inside the <see cref="NativePointer"/> property. <br />
        /// Note that the data organization may not be standard for each image object, even if you use the same decoder to do the job.
        /// </summary>
        public ImagePixelFormat PixelFormat { get; }

        /// <summary>
        /// Specifies whether the specified image is already flipped vertically; <br />
        /// According to each implementation , the bitmap bytes must be processed so that
        /// the target UI can interpret the sequence correctly. <br />
        /// For example, OpenGL requires the image bytes to be flipped in order to be previewed correctly.
        /// </summary>
        public System.Boolean IsFlippedVertically { get; }

        /// <summary>
        /// Gets the image dimensions , in pixels.
        /// </summary>
        public Size Size { get; }
    }
}
