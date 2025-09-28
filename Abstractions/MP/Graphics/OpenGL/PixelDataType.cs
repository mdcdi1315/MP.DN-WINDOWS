

namespace MP.Graphics.OpenGL
{
    /// <summary>
    /// Defines the data type that each pixel of a texture is composed of.
    /// </summary>
    public enum PixelDataType : System.UInt32
    {
        /// <summary>
        /// The pixel data are of type <see cref="System.SByte"/>.
        /// </summary>
        GL_BYTE = 0x1400,
        /// <summary>
        /// The pixel data are of type <see cref="System.Byte"/>.
        /// </summary>
        GL_UNSIGNED_BYTE = 0x1401,
        /// <summary>
        /// The pixel data are of type <see cref="System.Int16"/>.
        /// </summary>
        GL_SHORT = 0x1402,
        /// <summary>
        /// The pixel data are of type <see cref="System.UInt16"/>.
        /// </summary>
        GL_UNSIGNED_SHORT = 0x1403,
        /// <summary>
        /// The pixel data are of type <see cref="System.Int32"/>.
        /// </summary>
        GL_INT = 0x1404,
        /// <summary>
        /// The pixel data are of type <see cref="System.UInt32"/>.
        /// </summary>
        GL_UNSIGNED_INT = 0x1405,
        /// <summary>
        /// The pixel data are of type <see cref="System.Single"/>.
        /// </summary>
        GL_FLOAT = 0x1406
    }
}