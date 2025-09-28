


namespace MP.Graphics.OpenGL
{
    /// <summary>
    /// Defines different parameter names for use with glTexParameter** functions
    /// </summary>
    public enum TextureParameterName : System.UInt32
    {
        /// <summary></summary>
        GL_TEXTURE_MAG_FILTER = 0x2800,
        /// <summary></summary>
        GL_TEXTURE_MIN_FILTER = 0x2801,
        /// <summary></summary>
        GL_TEXTURE_MIN_LOD = 0x813A,
        /// <summary></summary>
        GL_TEXTURE_MAX_LOD = 0x813B,
        /// <summary></summary>
        GL_TEXTURE_BASE_LEVEL = 0x813C,
        /// <summary></summary>
        GL_TEXTURE_MAX_LEVEL = 0x813D,
        /// <summary></summary>
        GL_TEXTURE_LOD_BIAS = 0x8501,
        /// <summary></summary>
        GL_TEXTURE_COMPARE_MODE = 0x884C,
        /// <summary></summary>
        GL_TEXTURE_COMPARE_FUNC = 0x884D,
    }
}