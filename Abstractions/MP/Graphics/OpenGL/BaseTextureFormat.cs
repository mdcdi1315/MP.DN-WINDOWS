

namespace MP.Graphics.OpenGL
{
    /// <summary>
    /// Defines the base internal texture format of the image texture data.
    /// </summary>
    public enum BaseTextureFormat : System.UInt32
    {
        /// <summary>
        /// Image format has only the depth component.
        /// </summary>
        GL_DEPTH_COMPONENT = 0x1902,
        /// <summary>
        /// Image format has both the depth and stencil components.
        /// </summary>
        GL_DEPTH_STENCIL = 0x84F9,
        /// <summary>
        /// Image format has only the red channel data.
        /// </summary>
        GL_RED = 0x1903,
        /// <summary>
        /// Image format has both the red and green color channels.
        /// </summary>
        GL_RG = 0x8227,
        /// <summary>
        /// Image format has red , green and blue color channels.
        /// </summary>
        GL_RGB = 0x1907,
        /// <summary>
        /// Image format has red , green , blue and alpha color channels.
        /// </summary>
        GL_RGBA = 0x1908
    }
}