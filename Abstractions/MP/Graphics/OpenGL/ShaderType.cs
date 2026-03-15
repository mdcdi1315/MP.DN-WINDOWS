

namespace MP.Graphics.OpenGL
{
    /// <summary>
    /// Defines the different OpenGL shader types.
    /// </summary>
    public enum ShaderType : System.UInt32
    {
        /// <summary>
        /// Defines the fragment shader.
        /// </summary>
        GL_FRAGMENT_SHADER = 0x8B30,
        /// <summary>
        /// Defines the vertex shader.
        /// </summary>
        GL_VERTEX_SHADER = 0x8B31,
        /// <summary>
        /// Defines the geometry shader.
        /// </summary>
        GL_GEOMETRY_SHADER = 0x8DD9,
    }
}