


namespace MP.Graphics.OpenGL
{
    /// <summary>
    /// Defines the parameter values that can be used with <see cref="GL.glGetProgramiv"/> method.
    /// </summary>
    public enum ProgramParameter : System.UInt32
    {
        /// <summary>
        /// Returns the maximum number of vertices that the geometry shader contained in the program will output.
        /// </summary>
        GL_GEOMETRY_VERTICES_OUT = 0x8916,
        /// <summary>
        /// Returns a symbolic constant indicating the primitive type accepted as input to the geometry shader contained in the program.
        /// </summary>
        GL_GEOMETRY_INPUT_TYPE = 0x8917,
        /// <summary>
        /// Returns a symbolic constant indicating the primitive type that will be output by the geometry shader contained in the program.
        /// </summary>
        GL_GEOMETRY_OUTPUT_TYPE = 0x8918,
        /// <summary>
        /// Returns <see cref="GLConstants.GL_TRUE"/> if the program is currently flagged for deletion, and <see cref="GLConstants.GL_FALSE"/> otherwise.
        /// </summary>
        GL_DELETE_STATUS = GLConstants.GL_DELETE_STATUS,
        /// <summary>
        /// Returns <see cref="GLConstants.GL_TRUE"/> if the last link operation on the program was successful, and <see cref="GLConstants.GL_FALSE"/> otherwise.
        /// </summary>
        GL_LINK_STATUS = 0x8B82,
        /// <summary>
        /// Returns <see cref="GLConstants.GL_TRUE"/> or if the last validation operation on program was successful, and <see cref="GLConstants.GL_FALSE"/> otherwise.
        /// </summary>
        GL_VALIDATE_STATUS = 0x8B83,
        /// <summary>
        /// Returns the number of characters in the information log for the program including the null termination character (i.e., the size of the character buffer required to store the information log). <br />
        /// If the program has no information log, a value of 0 is returned.
        /// </summary>
        GL_INFO_LOG_LENGTH = GLConstants.GL_INFO_LOG_LENGTH,
        /// <summary>
        /// Returns the number of shader objects attached to the program.
        /// </summary>
        GL_ATTACHED_SHADERS = 0x8B85,
        /// <summary>
        /// Returns the number of active uniform variables for the program.
        /// </summary>
        GL_ACTIVE_UNIFORMS = 0x8B86,
        /// <summary>
        /// Returns the length of the longest active uniform variable name for the program, including the null termination character (i.e., the size of the character buffer required to store the longest uniform variable name). <br />
        /// If no active uniform variables exist, 0 is returned.
        /// </summary>
        GL_ACTIVE_UNIFORM_MAX_LENGTH = 0x8B87,
        /// <summary>
        /// Returns the number of active attribute variables for the program.
        /// </summary>
        GL_ACTIVE_ATTRIBUTES = 0x8B89,
        /// <summary>
        /// Returns the length of the longest active attribute name for the program, including the null termination character (i.e., the size of the character buffer required to store the longest attribute name). <br />
        /// If no active attributes exist, 0 is returned.
        /// </summary>
        GL_ACTIVE_ATTRIBUTE_MAX_LENGTH = 0x8B8A,
        /// <summary>
        /// Returns a symbolic constant indicating the buffer mode used when transform feedback is active. <br />
        /// This may be <c>GL_SEPARATE_ATTRIBS</c> or <c>GL_INTERLEAVED_ATTRIBS</c>.
        /// </summary>
        GL_TRANSFORM_FEEDBACK_BUFFER_MODE = 0x8C7F,
        /// <summary>
        /// Returns the length of the longest variable name to be used for transform feedback, including the null-terminator.
        /// </summary>
        GL_TRANSFORM_FEEDBACK_VARYING_MAX_LENGTH = 0x8C76,
        /// <summary>
        /// Returns the number of varying variables to capture in transform feedback mode for the program.
        /// </summary>
        GL_TRANSFORM_FEEDBACK_VARYINGS = 0x8C83,
    }
}