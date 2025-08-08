

namespace MP.Graphics
{
    /// <summary>
    /// The <see cref="IColor"/> interface defines a way to access 8-bit-depth colors in a unified way, regardless of the color format. <br />
    /// Implementers of the interface just specify the data depiction and how a color should be read out and written to.
    /// </summary>
    public interface IColor
    {
        /// <summary>
        /// Gets the alpha channel of the color.
        /// </summary>
        public System.Byte A { get; }

        /// <summary>
        /// Gets the red channel of the color.
        /// </summary>
        public System.Byte R { get; }

        /// <summary>
        /// Gets the green channel of the color.
        /// </summary>
        public System.Byte G { get; }

        /// <summary>
        /// Gets the blue channel of the color.
        /// </summary>
        public System.Byte B { get; }
    }
}