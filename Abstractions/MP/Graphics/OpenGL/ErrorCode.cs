


namespace MP.Graphics.OpenGL
{
    /// <summary>
    /// Defines different OpenGL error codes.
    /// </summary>
    public enum ErrorCode : System.UInt32
    {
        /// <summary>
        /// No error has been recorded. <br />
        /// The value of this symbolic constant is guaranteed to be 0.
        /// </summary>
        GL_NO_ERROR = 0,
        /// <summary>
        /// An unacceptable value is specified for an enumerated argument. <br />
        /// The offending command is ignored and has no other side effect than to set the error flag.
        /// </summary>
        GL_INVALID_ENUM = 0x0500,
        /// <summary>
        /// A numeric argument is out of range. <br />
        /// The offending command is ignored and has no other side effect than to set the error flag.
        /// </summary>
        GL_INVALID_VALUE = 0x0501,
        /// <summary>
        /// The specified operation is not allowed in the current state. <br />
        /// The offending command is ignored and has no other side effect than to set the error flag.
        /// </summary>
        GL_INVALID_OPERATION = 0x0502,
        /// <summary>
        /// There is not enough memory left to execute the command. <br />
        /// The state of the GL is undefined, except for the state of the error flags, after this error is recorded.
        /// </summary>
        GL_OUT_OF_MEMORY = 0x0505,
        /// <summary>
        /// The framebuffer object is not complete. <br />
        /// The offending command is ignored and has no other side effect than to set the error flag.
        /// </summary>
        GL_INVALID_FRAMEBUFFER_OPERATION = 0x0506,
    }
}