

using System.Runtime.InteropServices;

namespace MP.Graphics.OpenGL
{
    /// <summary>
    /// Defines the OpenGL object that is a buffer.
    /// </summary>
    [StructLayout(LayoutKind.Explicit , Size = 4 , Pack = 4)]
    public readonly struct BufferObject : IOpenGLObject
    {
        /// <summary>
        /// Defines the 'empty' buffer object. <br />
        /// Use this to unbind properly your GL buffers!!!
        /// </summary>
        public static readonly BufferObject Empty = new(0);

        [FieldOffset(0)]
        private readonly System.UInt32 handle;

        /// <summary>
        /// Creates a buffer object from an existing buffer object handle.
        /// </summary>
        /// <param name="handle">The existing OpenGL buffer object handle</param>
        public BufferObject(System.UInt32 handle) => this.handle = handle;

        /// <summary>
        /// Gets a value whether this buffer object represents the 'null' object.
        /// </summary>
        public readonly System.Boolean IsNull => handle == 0;
    }
}