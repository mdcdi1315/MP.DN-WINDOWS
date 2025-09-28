


namespace MP.Graphics.OpenGL
{
    /// <summary>
    /// Defines type constants for the <see cref="GL.glGetString"/> function.
    /// </summary>
    public enum GlGetStringType : System.UInt32
    {
        /// <summary>
        /// Returns the company responsible for this GL implementation. 
        /// This name does not change from release to release.
        /// </summary>
        GL_VENDOR = 0x1F00,
        /// <summary>
        /// Returns the name of the renderer. 
        /// This name is typically specific to a particular configuration of a hardware platform. 
        /// It does not change from release to release.
        /// </summary>
        GL_RENDERER = 0x1F01,
        /// <summary>
        /// Returns a version or release number.
        /// </summary>
        GL_VERSION = 0x1F02,
        /// <summary>
        /// For <see cref="GL.glGetStringi"/> only, returns the extension string supported by the implementation at index.
        /// </summary>
        GL_EXTENSIONS = 0x1F03,
        /// <summary>
        /// Returns a version or release number for the shading language.
        /// </summary>
        GL_SHADING_LANGUAGE_VERSION = 0x8B8C
    }
}