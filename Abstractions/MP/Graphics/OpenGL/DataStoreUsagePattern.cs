

namespace MP.Graphics.OpenGL
{
    /// <summary>
    /// Defines different storage patterns when specifying buffer data to OpenGL. <br/>
    /// Note: Doc comments of the fields will change as I discover these, so please do not advise these!!
    /// </summary>
    public enum DataStoreUsagePattern : System.UInt32
    {
        /// <summary>
        /// The buffer data can be re-used for continuous drawing.
        /// </summary>
        GL_STREAM_DRAW = 0x88E0,
        /// <summary>
        /// The buffer data can be re-used, but only for reading them.
        /// </summary>
        GL_STREAM_READ = 0x88E1,
        /// <summary>
        /// The buffer data can be re-used, but only for copying them to another buffer?? 
        /// </summary>
        GL_STREAM_COPY = 0x88E2,
        /// <summary>
        /// The buffer data can be used only once for drawing.
        /// </summary>
        GL_STATIC_DRAW = 0x88E4,
        /// <summary>
        /// The buffer data can be used only once for reading them.
        /// </summary>
        GL_STATIC_READ = 0x88E5,
        /// <summary>
        /// The buffer data can be used only once for copying them to another buffer?? 
        /// </summary>
        GL_STATIC_COPY = 0x88E6,
        /// <summary>
        /// The buffer data can be overwritten and re-used for continuous drawing.
        /// </summary>
        GL_DYNAMIC_DRAW = 0x88E8,
        /// <summary>
        /// The buffer data can be overwritten and re-used for reading them.
        /// </summary>
        GL_DYNAMIC_READ = 0x88E9,
        /// <summary>
        /// The buffer data can be overwritten and re-used for copying them to another buffer?? 
        /// </summary>
        GL_DYNAMIC_COPY = 0x88EA
    }
}