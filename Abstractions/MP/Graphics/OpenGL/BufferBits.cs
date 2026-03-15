

using System;

namespace MP.Graphics.OpenGL
{
    /// <summary>
    /// Defines the buffer bits to clear with the <see cref="GL.glClear"/> function.
    /// </summary>
    [Flags]
    public enum BufferBits : System.Int32
    {
        /// <summary>
        /// Clear the color data.
        /// </summary>
        GL_COLOR_BUFFER_BIT = 0x00004000,
        /// <summary>
        /// Clear the depth data.
        /// </summary>
        GL_DEPTH_BUFFER_BIT = 0x00000100,
        /// <summary>
        /// Clear the stencil data.
        /// </summary>
        GL_STENCIL_BUFFER_BIT = 0x00000400
    }
}