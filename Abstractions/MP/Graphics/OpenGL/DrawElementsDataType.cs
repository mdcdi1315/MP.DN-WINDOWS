


namespace MP.Graphics.OpenGL
{
    /// <summary>
    /// Specifies the index type to use with <see cref="GL.glDrawElements"/> function.
    /// </summary>
    public enum DrawElementsDataType : System.UInt32
    {
        /// <summary>
        /// The indices data are of type <see cref="System.Byte"/>.
        /// </summary>
        GL_UNSIGNED_BYTE = 0x1401,
        /// <summary>
        /// The indices data are of type <see cref="System.UInt16"/>.
        /// </summary>
        GL_UNSIGNED_SHORT = 0x1403,
        /// <summary>
        /// The indices data are of type <see cref="System.UInt32"/>.
        /// </summary>
        GL_UNSIGNED_INT = 0x1405,
    }
}