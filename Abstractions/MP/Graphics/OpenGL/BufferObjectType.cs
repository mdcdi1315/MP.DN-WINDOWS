namespace MP.Graphics.OpenGL
{
    /// <summary>
    /// Defines the different <see cref="BufferObject"/> types that can exist in an OpenGL environment.
    /// </summary>
    public enum BufferObjectType : System.UInt32
    {
        /// <summary>
        /// The <see cref="BufferObject"/> is an array buffer.
        /// </summary>
        GL_ARRAY_BUFFER = 0x8892,
        /// <summary>
        /// The <see cref="BufferObject"/> is an element array buffer.
        /// </summary>
        GL_ELEMENT_ARRAY_BUFFER = 0x8893,
    }
}
