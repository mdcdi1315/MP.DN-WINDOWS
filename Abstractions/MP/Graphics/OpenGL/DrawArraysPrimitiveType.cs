

namespace MP.Graphics.OpenGL
{
    /// <summary>
    /// Defines the primitives that can be drawn with <see cref="GL.glDrawArrays"/>.
    /// </summary>
    public enum DrawArraysPrimitiveType : System.UInt32
    {
        /// <summary>Draws points.</summary>
        GL_POINTS = 0x0000,
        /// <summary>Draws lines.</summary>
        GL_LINES = 0x0001,
        /// <summary>Draws a line loop.</summary>
        GL_LINE_LOOP = 0x0002,
        /// <summary>Draws a line strip.</summary>
        GL_LINE_STRIP = 0x0003,
        /// <summary>Draws triangles.</summary>
        GL_TRIANGLES = 0x0004,
        /// <summary></summary>
        GL_TRIANGLE_STRIP = 0x0005,
        /// <summary></summary>
        GL_TRIANGLE_FAN = 0x0006,
        /// <summary></summary>
        GL_LINES_ADJACENCY = 0x000A,
        /// <summary></summary>
        GL_LINE_STRIP_ADJACENCY = 0x000B,
        /// <summary></summary>
        GL_TRIANGLES_ADJACENCY = 0x000C,
        /// <summary></summary>
        GL_TRIANGLE_STRIP_ADJACENCY = 0x000D,
    }
}