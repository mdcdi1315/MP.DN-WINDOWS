


namespace MP.Graphics.OpenGL
{
    /// <summary>
    /// Defines the different factor types to use with the <see cref="GL.glBlendFunc"/> function. 
    /// </summary>
    public enum BlendFunctionFactor : System.UInt32
    {
        /// <summary>No color values are received or outputted.</summary>
        GL_ZERO = 0,
        /// <summary>The color values are received or outputted as is.</summary>
        GL_ONE = 1,
        /// <summary>The source color values are outputted divided by the source color values.</summary>
        GL_SRC_COLOR = 0x0300,
        /// <summary>Works like <see cref="GL_SRC_COLOR"/> but 1 is subtracted from the color values.</summary>
        GL_ONE_MINUS_SRC_COLOR = 0x0301,
        /// <summary>The source color values are outputted divided by the destination alpha channel value.</summary>
        GL_SRC_ALPHA = 0x0302,
        /// <summary>Works like <see cref="GL_SRC_ALPHA"/> but 1 is subtracted from the color values.</summary>
        GL_ONE_MINUS_SRC_ALPHA = 0x0303,
        /// <summary>The destination color values are outputted divided by the source alpha channel value.</summary>
        GL_DST_ALPHA = 0x0304,
        /// <summary>Works like <see cref="GL_DST_ALPHA"/> but 1 is subtracted from the color values.</summary>
        GL_ONE_MINUS_DST_ALPHA = 0x0305,
        /// <summary>The destination color values are outputted divided by the source color values.</summary>
        GL_DST_COLOR = 0x0306,
        /// <summary>Works like <see cref="GL_DST_COLOR"/> but 1 is subtracted from the color values.</summary>
        GL_ONE_MINUS_DST_COLOR = 0x0307,
        /// <summary>The source or destination alpha channel does always become 1.</summary>
        GL_SRC_ALPHA_SATURATE = 0x0308,
        /// <summary>The destination color values are the same as the source color values.</summary>
        GL_CONSTANT_COLOR = 0x8001,
        /// <summary>Works like <see cref="GL_CONSTANT_COLOR"/> but 1 is subtracted from the color values.</summary>
        GL_ONE_MINUS_CONSTANT_COLOR = 0x8002,
        /// <summary>The destination color values are the same as the source's alpha channel value.</summary>
        GL_CONSTANT_ALPHA = 0x8003,
        /// <summary>Works like <see cref="GL_CONSTANT_ALPHA"/> but 1 is subtracted from the color values.</summary>
        GL_ONE_MINUS_CONSTANT_ALPHA = 0x8004,
        /// <summary>The second source color values are outputted divided by the destination alpha channel value.</summary>
        GL_SRC1_ALPHA = 0x8589,
        /// <summary>The second source color values are outputted divided by the destination alpha channel value.</summary>
        GL_SRC1_COLOR = 0x88F9,
        /// <summary>Works like <see cref="GL_SRC1_COLOR"/> but 1 is subtracted from the color values.</summary>
        GL_ONE_MINUS_SRC1_COLOR = 0x88FA,
        /// <summary>Works like <see cref="GL_SRC1_ALPHA"/> but 1 is subtracted from the color values.</summary>
        GL_ONE_MINUS_SRC1_ALPHA = 0x88FB,
    }
}