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
        /// <summary>
        /// The <see cref="BufferObject"/> is a pixel read target buffer.
        /// </summary>
        GL_PIXEL_PACK_BUFFER = 0x88EB,
        /// <summary>
        /// The <see cref="BufferObject"/> is a buffer used for uniform variable block storage.
        /// </summary>
        GL_UNIFORM_BUFFER = 0x8A11,
        /// <summary>
        /// The <see cref="BufferObject"/> is a buffer used to store texture data.
        /// </summary>
        GL_TEXTURE_BUFFER = 0x8C2A,
        /// <summary>
        /// The <see cref="BufferObject"/> is a generic buffer used for buffer copy read operations.
        /// </summary>
        GL_COPY_READ_BUFFER = 0x8F36,
        /// <summary>
        /// The <see cref="BufferObject"/> is a generic buffer used for buffer copy write operations.
        /// </summary>
        GL_COPY_WRITE_BUFFER = 0x8F37,
    }
}
