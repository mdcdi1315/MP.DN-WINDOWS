

namespace MP.Graphics.OpenGL
{
    /// <summary>
    /// Defines different texture object types for use with <see cref="GL.glBindTexture"/>.
    /// </summary>
    public enum TextureObjectType : System.UInt32
    {
        /// <summary></summary>
        GL_TEXTURE_1D = 0x0DE0,
        /// <summary></summary>
        GL_TEXTURE_1D_ARRAY = 0x8C18,
        /// <summary></summary>
        GL_TEXTURE_2D = 0x0DE1,
        /// <summary></summary>
        GL_TEXTURE_2D_ARRAY = 0x8C1A,
        /// <summary></summary>
        GL_TEXTURE_2D_MULTISAMPLE = 0x9100,
        /// <summary></summary>
        GL_TEXTURE_2D_MULTISAMPLE_ARRAY = 0x9102,
            /// <summary></summary>
        GL_TEXTURE_3D = 0x806F,
        /// <summary></summary>
        GL_TEXTURE_RECTANGLE = 0x84F5,
        /// <summary></summary>
        GL_TEXTURE_CUBE_MAP = 0x8513,
        /// <summary></summary>
        GL_TEXTURE_BUFFER = 0x8C2A
    }
}