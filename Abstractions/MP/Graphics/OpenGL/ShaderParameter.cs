


namespace MP.Graphics.OpenGL
{
    /// <summary>
    /// Defines the parameter values that can be used with <see cref="GL.glGetShaderiv"/> method.
    /// </summary>
    public enum ShaderParameter : System.UInt32
    {
        /// <summary>
        /// Queries for the shader type. <br />
        /// <see cref="ShaderType.GL_VERTEX_SHADER"/> if the shader is a vertex shader object, 
        /// <see cref="ShaderType.GL_GEOMETRY_SHADER"/> if shader is a geometry shader object, 
        /// and <see cref="ShaderType.GL_FRAGMENT_SHADER"/> if shader is a fragment shader object.
        /// </summary>
        GL_SHADER_TYPE = 0x8B4F,
        /// <summary>
        /// Queries whether the shader is flagged by OpenGL for deletion. <br />
        /// Returns <see cref="GLConstants.GL_TRUE"/> if the shader is currently flagged 
        /// for deletion, and <see cref="GLConstants.GL_FALSE"/> otherwise.
        /// </summary>
        GL_DELETE_STATUS = GLConstants.GL_DELETE_STATUS,
        /// <summary>
        /// Queries whether the shader has been succesfully compiled. <br />
        /// Returns <see cref="GLConstants.GL_TRUE"/> if the shader has been 
        /// successfully compiled; otherwise, it returns <see cref="GLConstants.GL_FALSE"/>.
        /// </summary>
        GL_COMPILE_STATUS = GLConstants.GL_COMPILE_STATUS,
        /// <summary>
        /// Queries the length, in characters, of the information log of the given shader. <br />
        /// It returns the number of characters in the information log for the shader including the null termination character (i.e., the size of the character buffer required to store the information log). <br />
        /// If the given shader has no information log, a value of 0 is returned.
        /// </summary>
        GL_INFO_LOG_LENGTH = GLConstants.GL_INFO_LOG_LENGTH,
        /// <summary>
        /// Returns the length of the concatenation of the source strings that make up the shader source for the shader, including the null termination character. (i.e., the size of the character buffer required to store the shader source). <br />
        /// If no source code exists, 0 is returned.
        /// </summary>
        GL_SHADER_SOURCE_LENGTH = 0x8B88,
    }
}