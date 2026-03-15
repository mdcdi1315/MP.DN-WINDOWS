

namespace MP.Graphics.OpenGL
{
    /// <summary>
    /// Defines different storage patterns when specifying buffer data to OpenGL. <br />
    /// They are hints to the OpenGL indicating how the app will manage the buffer.
    /// </summary>
    public enum DataStoreUsagePattern : System.UInt32
    {
        /// <summary>
        /// The buffer contents will be modified once and used at most a few times. <br />
        /// The app can modify the data from such buffers after they are defined. <br />
        /// For drawing purposes.
        /// </summary>
        GL_STREAM_DRAW = 0x88E0,
        /// <summary>
        /// The buffer contents will be modified once and used at most a few times. <br />
        /// For reading purposes.
        /// </summary>
        GL_STREAM_READ = 0x88E1,
        /// <summary>
        /// The buffer contents will be modified once and used at most a few times. <br />
        /// For drawing purposes.
        /// </summary>
        GL_STREAM_COPY = 0x88E2,
        /// <summary>
        /// The data store contents will be modified once and used many times. <br />
        /// The app can modify the data from such buffers after they are defined. <br />
        /// For drawing purposes.
        /// </summary>
        GL_STATIC_DRAW = 0x88E4,
        /// <summary>
        /// The data store contents will be modified once and used many times. <br />
        /// For reading purposes.
        /// </summary>
        GL_STATIC_READ = 0x88E5,
        /// <summary>
        /// The data store contents will be modified once and used many times. <br />
        /// For drawing purposes.
        /// </summary>
        GL_STATIC_COPY = 0x88E6,
        /// <summary>
        /// The data store contents will be modified repeatedly and used many times. <br />
        /// The app can modify the data from such buffers after they are defined. <br />
        /// For drawing purposes.
        /// </summary>
        GL_DYNAMIC_DRAW = 0x88E8,
        /// <summary>
        /// The data store contents will be modified repeatedly and used many times. <br />
        /// For reading purposes.
        /// </summary>
        GL_DYNAMIC_READ = 0x88E9,
        /// <summary>
        /// The data store contents will be modified repeatedly and used many times. <br />
        /// For drawing purposes.
        /// </summary>
        GL_DYNAMIC_COPY = 0x88EA
    }
}