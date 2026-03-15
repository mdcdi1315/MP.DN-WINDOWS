

using System.Runtime.InteropServices;

namespace MP.Graphics.OpenGL
{
    /// <summary>
    /// Represents an OpenGL texture object which is an abstraction for how images should be loaded and displayed into an OpenGL context.
    /// </summary>
    [StructLayout(LayoutKind.Explicit , Pack = 1 , Size = 4)]
    public readonly struct TextureObject : IOpenGLObject
    {
        [FieldOffset(0)]
        private readonly System.UInt32 handle;

        /// <summary>
        /// Creates a texture object from an existing texture object handle.
        /// </summary>
        /// <param name="handle">The existing OpenGL texture object handle</param>
        public TextureObject(System.UInt32 handle) => this.handle = handle;

        /// <summary>
        /// Gets a value whether this buffer object represents the 'null' object.
        /// </summary>
        public readonly System.Boolean IsNull => handle == 0;
    }
}