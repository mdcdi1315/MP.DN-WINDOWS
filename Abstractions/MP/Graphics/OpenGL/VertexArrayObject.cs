


using System.Runtime.InteropServices;

namespace MP.Graphics.OpenGL
{
    /// <summary>
    /// Defines OpenGL vertex array object handles.
    /// </summary>
    [StructLayout(LayoutKind.Explicit , Size = 4 , Pack = 1)]
    public readonly struct VertexArrayObject : IOpenGLObject
    {
        /// <summary>
        /// Gets the empty vertex array object. 
        /// You use this to unbind your vertex array objects.
        /// </summary>
        public static readonly VertexArrayObject Empty = new(0);

        [FieldOffset(0)]
        private readonly System.UInt32 handle;

        /// <summary>
        /// Creates a vertex array object from an existing vertex array handle.
        /// </summary>
        /// <param name="handle">The existing OpenGL vertex array handle</param>
        public VertexArrayObject(System.UInt32 handle) => this.handle = handle;

        /// <summary>
        /// Gets a value whether this vertex array object represents the 'null' object.
        /// </summary>
        public readonly System.Boolean IsNull => handle == 0;
    }
}