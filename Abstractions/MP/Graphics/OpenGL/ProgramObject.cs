

using System.Runtime.InteropServices;

namespace MP.Graphics.OpenGL
{
    /// <summary>
    /// Defines an OpenGL program object handle.
    /// </summary>
    [StructLayout(LayoutKind.Explicit , Size = 4 , Pack = 4)]
    public readonly struct ProgramObject : IOpenGLObject
    {
        [FieldOffset(0)]
        private readonly System.UInt32 handle;

        /// <summary>
        /// Creates a buffer object from an existing program object handle.
        /// </summary>
        /// <param name="handle">The existing OpenGL program object handle</param>
        public ProgramObject(System.UInt32 handle) => this.handle = handle;
 
        /// <summary>
        /// Gets a value whether this program object represents the 'null' object.
        /// </summary>
        public readonly bool IsNull => handle == 0;
    }
}