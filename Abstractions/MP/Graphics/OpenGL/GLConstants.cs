

namespace MP.Graphics.OpenGL
{
    /// <summary>
    /// Constant values that are too generic to be used by specific enumerations , instead they are specified here.
    /// </summary>
    public static class GLConstants
    {
        /// <summary>
        /// Defines the OpenGL <see langword="false"/> boolean value.
        /// </summary>
        public const System.Byte GL_FALSE = 0;
        /// <summary>
        /// Defines the OpenGL <see langword="true"/> boolean value.
        /// </summary>
        public const System.Byte GL_TRUE = 1;

        /// <summary></summary>
        public const System.UInt32 GL_TIMESTAMP = 0x8E28;

        /// <summary></summary>
        public const System.UInt32 GL_TIME_ELAPSED = 0x88BF;

        /// <summary>
        /// A value for querying whether an OpenGL object is flagged for deletion. <br />
        /// This is passed to any glGet* function that can obtain information for a given OpenGL object. <br />
        /// It returns GLboolean, which it does mean that it returns <see cref="GL_TRUE"/> when the object is flagged for deletion, and <see cref="GL_FALSE"/> when the particular object is still alive.
        /// </summary>
        public const System.UInt32 GL_DELETE_STATUS = 0x8B80;

        /// <summary>
        /// A value for querying whether an OpenGL object has been compiled successfully. <br />
        /// This is passed to any glGet* function that can obtain information for a given OpenGL object. <br />
        /// It returns GLboolean, which it does mean that it returns <see cref="GL_TRUE"/> when the object has been succesfully compiled; otherwise it returns <see cref="GL_FALSE"/>.
        /// </summary>
        public const System.UInt32 GL_COMPILE_STATUS = 0x8B81;

        /// <summary>
        /// A value that does return the number of characters of an OpenGL object information log. <br />
        /// This is passed to any glGet* function that can obtain information for a given OpenGL object. <br />
        /// If not otherwise noted, it returns a <see cref="System.Int32"/> containing the number of characters of the information log.
        /// </summary>
        public const System.UInt32 GL_INFO_LOG_LENGTH = 0x8B84;
    }
}