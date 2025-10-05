

using System;

namespace MP.Graphics.OpenGL.Primitives
{
    /// <summary>
    /// Defines the base interface for OpenGL primitive shapes. <br />
    /// The <see cref="IDisposable.Dispose"/> method provided by this interface unregisters the primitive from rendering,
    /// as well as disposes any associated information with it.
    /// </summary>
    public interface IOpenGLPrimitiveShape : IDisposable
    {
        /// <summary>
        /// Gets or sets the size of this primitive shape, expressed as NDC size.
        /// </summary>
        public SizeF Size { get; set; }

        /// <summary>
        /// Gets or sets the location of the primitive, in NDC coordinates.
        /// </summary>
        public PointF Location { get; set; }

        /// <summary>
        /// Gets or sets the color to use for drawing the primitive. <br />
        /// It's usage depends on what primitive you want to draw.
        /// </summary>
        public IColor Color { get; set; }

        /// <summary>
        /// Uses this primitive shape in this rendering loop pass.
        /// </summary>
        public void Use();
    }
}