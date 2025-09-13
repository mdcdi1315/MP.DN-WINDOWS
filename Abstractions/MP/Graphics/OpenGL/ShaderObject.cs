
using System.Runtime.InteropServices;

namespace MP.Graphics.OpenGL
{
    /// <summary>
    /// Defines the shader object handles in OpenGL.
    /// </summary>
    [StructLayout(LayoutKind.Explicit , Size = 4 , Pack = 4)]
    public readonly struct ShaderObject : IOpenGLObject
    {
        [FieldOffset(0)]
        private readonly System.UInt32 handle;

        /// <summary>
        /// Creates a new shader object from the preexisting OpenGL shader handle.
        /// </summary>
        /// <param name="handle">The OpenGL shader handle.</param>
        public ShaderObject(System.UInt32 handle) => this.handle = handle;

        /// <summary>
        /// Gets a value whether this shader object represents the 'null' object.
        /// </summary>
        public readonly bool IsNull => handle == 0;
    }
}