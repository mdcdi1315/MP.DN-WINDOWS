namespace MP.Graphics.OpenGL
{
    /// <summary>
    /// Defines different data types for use with <see cref="GL.glVertexAttribPointer"/> function.
    /// </summary>
    public enum VertexAttributePointerDataType : System.UInt32
    {
        /// <summary>
        /// The vertex attribute data are of type <see cref="System.SByte"/>.
        /// </summary>
        GL_BYTE = 0x1400,
        /// <summary>
        /// The vertex attribute data are of type <see cref="System.Byte"/>.
        /// </summary>
        GL_UNSIGNED_BYTE = 0x1401,
        /// <summary>
        /// The vertex attribute data are of type <see cref="System.Int16"/>.
        /// </summary>
        GL_SHORT = 0x1402,
        /// <summary>
        /// The vertex attribute data are of type <see cref="System.UInt16"/>.
        /// </summary>
        GL_UNSIGNED_SHORT = 0x1403,
        /// <summary>
        /// The vertex attribute data are of type <see cref="System.Int32"/>.
        /// </summary>
        GL_INT = 0x1404,
        /// <summary>
        /// The vertex attribute data are of type <see cref="System.UInt32"/>.
        /// </summary>
        GL_UNSIGNED_INT = 0x1405,
        /// <summary>
        /// The vertex attribute data are of type <see cref="System.Single"/>.
        /// </summary>
        GL_FLOAT = 0x1406,
        /// <summary>
        /// The vertex attribute data are of type <see cref="System.Double"/>.
        /// </summary>
        GL_DOUBLE = 0x140A,
        /// <summary>
        /// The vertex attribute data are of type <see cref="System.Half"/>.
        /// </summary>
        GL_HALF_FLOAT = 0x140B,
    }
}