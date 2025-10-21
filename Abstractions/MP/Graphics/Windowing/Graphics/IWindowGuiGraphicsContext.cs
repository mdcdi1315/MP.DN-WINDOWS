
using System;
using MP.Graphics.Imaging;

namespace MP.Graphics.Windowing.Graphics
{
    /// <summary>
    /// Defines the graphics context, which does render primitives on a graphics surface. <br />
    /// Each created shape is attached with a <see cref="GraphicsObjectID"/> instance, until that instance is destroyed by using the <see cref="DestroyObject(GraphicsObjectID)"/> method. <br /> <br />
    /// How the graphics context works <br /> <br />
    /// 
    /// Each graphics context is an independent object controlling how graphic objects will be created, requested data and destroyed. <br />
    /// Additionally, each graphics context is not a real window drawing surface. <br />
    /// The object itself assumes a virtual window area, in virtual window coordinates. <br />
    /// That virtual area is expressed as the (0,0) coordinate point to be the top-left side of the window, 
    /// and has a maximum size as denoted by the <see cref="Size"/> property.
    /// </summary>
    public interface IWindowGuiGraphicsContext : IDisposable
    {
        /// <summary>Creates a rectangle to be drawn into the current context.</summary>
        /// <param name="rectangle">The rectangle bounds.</param>
        /// <returns>An object referencing the drawn rectangle.</returns>
        public GraphicsObjectID CreateRectangle(Rectangle rectangle);

        /// <summary>Creates a triangle to be drawn into the current context.</summary>
        /// <param name="triangle">The triangle bounds.</param>
        /// <returns>An object referencing the drawn triangle.</returns>
        public GraphicsObjectID CreateTriangle(Triangle triangle);

        /// <summary>Creates a line to be drawn into the current context.</summary>
        /// <param name="line">The line's bounds.</param>
        /// <returns>An object referencing the drawn line.</returns>
        public GraphicsObjectID CreateLine(Line line);

        /// <summary>Creates a graphics texture to be drawn into the current context.</summary>
        /// <param name="image">The image to draw.</param>
        /// <param name="texture_area">The drawing area, as a rectangle, that the texture will cover.</param>
        /// <returns>An object referencing the drawn texture.</returns>
        public GraphicsObjectID CreateTexture(IImage image , Rectangle texture_area);

        /// <summary>
        /// Updates the rectangle bounds, if the current graphics object is a rectangle.
        /// </summary>
        /// <param name="o">The rectangle graphics object to update.</param>
        /// <param name="newrectangle">The new bounds of this rectangle graphics object.</param>
        /// <returns>A value whether the operation succeeded or not. Operation also fails if <paramref name="o"/> is not a rectangle graphics object.</returns>
        public bool UpdateRectangle(GraphicsObjectID o, Rectangle newrectangle);

        /// <summary>
        /// Updates the triangle bounds, if the current graphics object is a triangle.
        /// </summary>
        /// <param name="o">The triangle graphics object to update.</param>
        /// <param name="newtriangle">The new bounds of this triangle graphics object.</param>
        /// <returns>A value whether the operation succeeded or not. Operation also fails if <paramref name="o"/> is not a triangle graphics object.</returns>
        public bool UpdateTriangle(GraphicsObjectID o, Triangle newtriangle);

        /// <summary>
        /// Updates the line bounds, if the current graphics object is a line.
        /// </summary>
        /// <param name="o">The line graphics object to update.</param>
        /// <param name="line">The new bounds of this line graphics object.</param>
        /// <returns>A value whether the operation succeeded or not. Operation also fails if <paramref name="o"/> is not a line graphics object.</returns>
        public bool UpdateLine(GraphicsObjectID o, Line line);

        /// <summary>
        /// Updates the texture bounds, if the current graphics object is a texture.
        /// </summary>
        /// <param name="o">The texture graphics object to update.</param>
        /// <param name="texture_area">The new bounds of the texture graphics object.</param>
        /// <returns>A value whether the operation succeeded or not. Operation also fails if <paramref name="o"/> is not a texture graphics object.</returns>
        public bool UpdateTextureBounds(GraphicsObjectID o, Rectangle texture_area);

        /// <summary>
        /// Destroys a previously created graphics object. <br />
        /// After the object is destroyed, it must be considered invalid and you cannot retrieve information about it. <br />
        /// The returned ID might be also re-used later by the context for other graphics objects that might be requested later.
        /// </summary>
        /// <param name="o">The created graphics object.</param>
        /// <returns><see langword="true"/> when the specified object was found and deleted; otherwise, it returns <see langword="false"/>.</returns>
        public bool DestroyObject(GraphicsObjectID o);

        /// <summary>
        /// Gets the virtual size where this graphics context will work with.
        /// </summary>
        public Size Size { get; }

        /// <summary>
        /// If possible, all the queued rendering commands are rendered immediately.
        /// </summary>
        public void Flush();

        /// <summary>Clears the window screen with the specified color.</summary>
        /// <param name="color">The <see cref="IColor"/> instance to clear the window screen with.</param>
        public void Clear(IColor color);

        /// <summary>
        /// Draws a single frame of the current graphics data.
        /// </summary>
        public void DrawFrame();
    }

}