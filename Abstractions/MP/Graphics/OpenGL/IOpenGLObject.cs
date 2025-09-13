

namespace MP.Graphics.OpenGL
{
    /// <summary>
    /// Defines common functionality that OpenGL objects should have.
    /// </summary>
    public interface IOpenGLObject
    {
        /// <summary>
        /// Gets a value whether this instance represents the 'null' object.
        /// </summary>
        public System.Boolean IsNull { get; }
    }
}