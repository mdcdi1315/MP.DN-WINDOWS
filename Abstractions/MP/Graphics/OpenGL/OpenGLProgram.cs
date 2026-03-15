
using System;
using System.Collections.Generic;
using MP.Annotations.CodeAnalysis;

namespace MP.Graphics.OpenGL
{
    /// <summary>
    /// Defines an OpenGL program.
    /// </summary>
    public unsafe sealed class OpenGLProgram : IDisposable
    {
        private readonly ProgramObject program;
        private Dictionary<String, int> uniformmappings;

        /// <summary>
        /// Creates a new and empty OpenGL program.
        /// </summary>
        public OpenGLProgram() {
            program = GL.glCreateProgram();
            GL.AssertError();
            uniformmappings = null;
        }

        /// <summary>
        /// Creates a new and empty OpenGL program. <br />
        /// This constructor overload allows to specify the names of the uniforms you wish to use in your program. 
        /// </summary>
        /// <param name="uniformnames">The uniform names that you are going to use. A mapping will be created for these once the program is successfully linked.</param>
        public OpenGLProgram(params System.String[] uniformnames) : this()
        {
            uniformmappings = new();
            foreach (var name in uniformnames) {
                uniformmappings.Add(name, 0);
            }
        }

        /// <summary>Attaches a shader to this program.</summary>
        /// <param name="shader">The shader to attach.</param>
        /// <remarks>
        /// Even if the <see cref="OpenGLShader.Dispose"/> is called after attaching happens, 
        /// the shader will not be disposed of until a call to <see cref="DetachShader(OpenGLShader)"/> 
        /// happens or the current program object is destroyed through <see cref="Dispose"/>.
        /// </remarks>
        [Throws(typeof(ArgumentNullException))]
        public void AttachShader(OpenGLShader shader)
        {
            ArgumentNullException.ThrowIfNull(shader);
            GL.glAttachShader(program , shader.Handle);
        }

        /// <summary>Attaches a shader to this program.</summary>
        /// <param name="shader">The shader to attach.</param>
        public void AttachShader(ShaderObject shader) => GL.glAttachShader(program, shader);

        /// <summary>Detaches a shader from this program.</summary>
        /// <param name="shader">The shader to detach.</param>
        [Throws(typeof(ArgumentNullException))]
        public void DetachShader(OpenGLShader shader)
        {
            ArgumentNullException.ThrowIfNull(shader);
            GL.glDetachShader(program , shader.Handle);
        }

        /// <summary>Detaches a shader from this program.</summary>
        /// <param name="shader">The shader to detach.</param>
        public void DetachShader(ShaderObject shader) => GL.glDetachShader(program, shader);

        /// <summary>
        /// Links the current program object with it's attached shaders.
        /// </summary>
        /// <exception cref="OpenGLProgramLinkageException">(Debug builds only). A program linkage error has been occured.</exception>
        [ThrowsOnlyWhen("DEBUG" , typeof(OpenGLProgramLinkageException))]
        public void Link()
        {
            GL.glLinkProgram(program);
            if (GL.GLGetProgramIV(program, ProgramParameter.GL_LINK_STATUS) == GLConstants.GL_TRUE)
            {
                if (uniformmappings is not null && uniformmappings.Count > 0)
                {
                    // Linkage successfull, we can retrieve the mappings
                    foreach (var uniform in uniformmappings.Keys)
                    {
                        uniformmappings[uniform] = GLUtils.GLGetUniformLocation(program, uniform);
                    }
                }
            }
#if DEBUG
            else {
                throw new OpenGLProgramLinkageException(GLUtils.GLGetProgramInfoLog(program));
            }
#endif
        }

        /// <summary>
        /// Ensures that the program can run in the current OpenGL context. <br />
        /// This method should only be used during debugging.
        /// </summary>
        /// <returns>A value whether the program is valid can be used in subsequent <see cref="Use"/> calls.</returns>
        public System.Boolean Validate()
        {
            GL.glValidateProgram(program);
            return GL.GLGetProgramIV(program, ProgramParameter.GL_VALIDATE_STATUS) == GLConstants.GL_TRUE;
        }

        /// <summary>
        /// Uses the current program. <br />
        /// The program must have been linked successfully. <br />
        /// (That is, instructs OpenGL that every operation will be applied to this program).
        /// </summary>
        public void Use() => GL.glUseProgram(program);

        #region SetUniform

        /// <summary>
        /// Updates a uniform variable to the specified value.
        /// </summary>
        /// <param name="name">The name of the uniform value to update.</param>
        /// <param name="value">The desired value.</param>
        public void SetUniform(String name, float value) => GL.glUniform1f(uniformmappings[name], value);

        /// <summary>
        /// Updates a uniform variable to the specified value.
        /// </summary>
        /// <param name="name">The name of the uniform value to update.</param>
        /// <param name="value">The desired value.</param>
        public void SetUniform(String name, int value) => GL.glUniform1i(uniformmappings[name], value);

        /// <summary>
        /// Updates a uniform variable to the specified value.
        /// </summary>
        /// <param name="name">The name of the uniform value to update.</param>
        /// <param name="value">The desired value.</param>
        public void SetUniform(String name, uint value) => GL.glUniform1ui(uniformmappings[name], value);

        /// <summary>
        /// Updates a uniform variable to the specified values.
        /// </summary>
        /// <param name="name">The name of the uniform value to update.</param>
        /// <param name="v1">The desired first value.</param>
        /// <param name="v2">The desired second value.</param>
        public void SetUniform(String name, float v1 , float v2) => GL.glUniform2f(uniformmappings[name], v1, v2);

        /// <summary>
        /// Updates a uniform variable to the specified values.
        /// </summary>
        /// <param name="name">The name of the uniform value to update.</param>
        /// <param name="v1">The desired first value.</param>
        /// <param name="v2">The desired second value.</param>
        public void SetUniform(String name, int v1 , int v2) => GL.glUniform2i(uniformmappings[name], v1, v2);

        /// <summary>
        /// Updates a uniform variable to the specified values.
        /// </summary>
        /// <param name="name">The name of the uniform value to update.</param>
        /// <param name="v1">The desired first value.</param>
        /// <param name="v2">The desired second value.</param>
        public void SetUniform(String name, uint v1 , uint v2) => GL.glUniform2ui(uniformmappings[name], v1, v2);

        /// <summary>
        /// Updates a uniform variable to the specified values.
        /// </summary>
        /// <param name="name">The name of the uniform value to update.</param>
        /// <param name="v1">The desired first value.</param>
        /// <param name="v2">The desired second value.</param>
        /// <param name="v3">The desired third value.</param>
        public void SetUniform(String name, float v1, float v2 , float v3) => GL.glUniform3f(uniformmappings[name], v1, v2, v3);

        /// <summary>
        /// Updates a uniform variable to the specified values.
        /// </summary>
        /// <param name="name">The name of the uniform value to update.</param>
        /// <param name="v1">The desired first value.</param>
        /// <param name="v2">The desired second value.</param>
        /// <param name="v3">The desired third value.</param>
        public void SetUniform(String name, int v1, int v2, int v3) => GL.glUniform3i(uniformmappings[name], v1, v2, v3);

        /// <summary>
        /// Updates a uniform variable to the specified values.
        /// </summary>
        /// <param name="name">The name of the uniform value to update.</param>
        /// <param name="v1">The desired first value.</param>
        /// <param name="v2">The desired second value.</param>
        /// <param name="v3">The desired third value.</param>
        public void SetUniform(String name, uint v1, uint v2, uint v3) => GL.glUniform3ui(uniformmappings[name], v1, v2 , v3);

        /// <summary>
        /// Updates a uniform variable to the specified values.
        /// </summary>
        /// <param name="name">The name of the uniform value to update.</param>
        /// <param name="v1">The desired first value.</param>
        /// <param name="v2">The desired second value.</param>
        /// <param name="v3">The desired third value.</param>
        /// <param name="v4">The desired fourth value.</param>
        public void SetUniform(String name, float v1, float v2, float v3, float v4) => GL.glUniform4f(uniformmappings[name] , v1, v2, v3, v4);

        /// <summary>
        /// Updates a uniform variable to the specified values.
        /// </summary>
        /// <param name="name">The name of the uniform value to update.</param>
        /// <param name="v1">The desired first value.</param>
        /// <param name="v2">The desired second value.</param>
        /// <param name="v3">The desired third value.</param>
        /// <param name="v4">The desired fourth value.</param>
        public void SetUniform(String name, int v1, int v2, int v3, int v4) => GL.glUniform4i(uniformmappings[name], v1, v2, v3, v4);

        /// <summary>
        /// Updates a uniform variable to the specified values.
        /// </summary>
        /// <param name="name">The name of the uniform value to update.</param>
        /// <param name="v1">The desired first value.</param>
        /// <param name="v2">The desired second value.</param>
        /// <param name="v3">The desired third value.</param>
        /// <param name="v4">The desired fourth value.</param>
        public void SetUniform(String name, uint v1, uint v2, uint v3, uint v4) => GL.glUniform4ui(uniformmappings[name], v1, v2, v3, v4);

        #endregion

        #region GetUniform

        /// <summary>
        /// Gets a uniform variable that has a type of <see cref="float"/>.
        /// </summary>
        /// <param name="name">The name of the uniform variable to retrieve it's value.</param>
        /// <returns>The uniform variable value.</returns>
        public float GetUniform(String name)
        {
            float fl;
            GL.glGetUniformfv(program, uniformmappings[name], &fl);
            return fl;
        }

        /// <summary>
        /// Gets a uniform variable that has a type of vector of <see cref="float"/>.
        /// </summary>
        /// <param name="name">The name of the uniform variable to retrieve it's value.</param>
        /// <param name="v2">On return, it retrieves the second value of the variable.</param>
        /// <returns>The first value of the uniform variable.</returns>
        public float GetUniform(String name , out float v2)
        {
            float* pfloats = stackalloc float[2];
            GL.glGetUniformfv(program, uniformmappings[name], pfloats);
            v2 = pfloats[1];
            return pfloats[0];
        }

        /// <summary>
        /// Gets a uniform variable that has a type of vector of <see cref="float"/>.
        /// </summary>
        /// <param name="name">The name of the uniform variable to retrieve it's value.</param>
        /// <param name="v2">On return, it retrieves the second value of the variable.</param>
        /// <param name="v3">On return, it retrieves the third value of the variable.</param>
        /// <returns>The first value of the uniform variable.</returns>
        public float GetUniform(String name, out float v2 , out float v3)
        {
            float* pfloats = stackalloc float[3];
            GL.glGetUniformfv(program, uniformmappings[name], pfloats);
            v2 = pfloats[1];
            v3 = pfloats[2];
            return pfloats[0];
        }

        /// <summary>
        /// Gets a uniform variable that has a type of vector of <see cref="float"/>.
        /// </summary>
        /// <param name="name">The name of the uniform variable to retrieve it's value.</param>
        /// <param name="v2">On return, it retrieves the second value of the variable.</param>
        /// <param name="v3">On return, it retrieves the third value of the variable.</param>
        /// <param name="v4">On return, it retrieves the fourth value of the variable.</param>
        /// <returns>The first value of the uniform variable.</returns>
        public float GetUniform(String name, out float v2, out float v3, out float v4)
        {
            float* pfloats = stackalloc float[4];
            GL.glGetUniformfv(program, uniformmappings[name], pfloats);
            v2 = pfloats[1];
            v3 = pfloats[2];
            v4 = pfloats[3];
            return pfloats[0];
        }

        /// <summary>
        /// Gets a uniform variable that has a type of <see cref="int"/>.
        /// </summary>
        /// <param name="name">The name of the uniform variable to retrieve it's value.</param>
        /// <returns>The uniform variable value.</returns>
        public int GetUniformI(String name)
        {
            int v;
            GL.glGetUniformiv(program, uniformmappings[name], &v);
            return v;
        }

        /// <summary>
        /// Gets a uniform variable that has a type of vector of <see cref="int"/>.
        /// </summary>
        /// <param name="name">The name of the uniform variable to retrieve it's value.</param>
        /// <param name="v2">On return, it retrieves the second value of the variable.</param>
        /// <returns>The first value of the uniform variable.</returns>
        public int GetUniformI(String name, out int v2)
        {
            int* pints = stackalloc int[2];
            GL.glGetUniformiv(program, uniformmappings[name], pints);
            v2 = pints[1];
            return pints[0];
        }

        /// <summary>
        /// Gets a uniform variable that has a type of vector of <see cref="int"/>.
        /// </summary>
        /// <param name="name">The name of the uniform variable to retrieve it's value.</param>
        /// <param name="v2">On return, it retrieves the second value of the variable.</param>
        /// <param name="v3">On return, it retrieves the third value of the variable.</param>
        /// <returns>The first value of the uniform variable.</returns>
        public int GetUniformI(String name, out int v2, out int v3)
        {
            int* pints = stackalloc int[3];
            GL.glGetUniformiv(program, uniformmappings[name], pints);
            v2 = pints[1];
            v3 = pints[2];
            return pints[0];
        }

        /// <summary>
        /// Gets a uniform variable that has a type of vector of <see cref="int"/>.
        /// </summary>
        /// <param name="name">The name of the uniform variable to retrieve it's value.</param>
        /// <param name="v2">On return, it retrieves the second value of the variable.</param>
        /// <param name="v3">On return, it retrieves the third value of the variable.</param>
        /// <param name="v4">On return, it retrieves the fourth value of the variable.</param>
        /// <returns>The first value of the uniform variable.</returns>
        public int GetUniformI(String name, out int v2, out int v3, out int v4)
        {
            int* pints = stackalloc int[4];
            GL.glGetUniformiv(program, uniformmappings[name], pints);
            v2 = pints[1];
            v3 = pints[2];
            v4 = pints[3];
            return pints[0];
        }

        /// <summary>
        /// Gets a uniform variable that has a type of <see cref="uint"/>.
        /// </summary>
        /// <param name="name">The name of the uniform variable to retrieve it's value.</param>
        /// <returns>The uniform variable value.</returns>
        public uint GetUniformUI(String name)
        {
            uint ret;
            GL.glGetUniformuiv(program, uniformmappings[name], &ret);
            return ret;
        }

        /// <summary>
        /// Gets a uniform variable that has a type of vector of <see cref="uint"/>.
        /// </summary>
        /// <param name="name">The name of the uniform variable to retrieve it's value.</param>
        /// <param name="v2">On return, it retrieves the second value of the variable.</param>
        /// <returns>The first value of the uniform variable.</returns>
        public uint GetUniformUI(String name, out uint v2)
        {
            uint* puints = stackalloc uint[2];
            GL.glGetUniformuiv(program, uniformmappings[name], puints);
            v2 = puints[1];
            return puints[0];
        }

        /// <summary>
        /// Gets a uniform variable that has a type of vector of <see cref="uint"/>.
        /// </summary>
        /// <param name="name">The name of the uniform variable to retrieve it's value.</param>
        /// <param name="v2">On return, it retrieves the second value of the variable.</param>
        /// <param name="v3">On return, it retrieves the third value of the variable.</param>
        /// <returns>The first value of the uniform variable.</returns>
        public uint GetUniformUI(String name, out uint v2, out uint v3)
        {
            uint* puints = stackalloc uint[3];
            GL.glGetUniformuiv(program, uniformmappings[name], puints);
            v2 = puints[1];
            v3 = puints[2];
            return puints[0];
        }

        /// <summary>
        /// Gets a uniform variable that has a type of vector of <see cref="uint"/>.
        /// </summary>
        /// <param name="name">The name of the uniform variable to retrieve it's value.</param>
        /// <param name="v2">On return, it retrieves the second value of the variable.</param>
        /// <param name="v3">On return, it retrieves the third value of the variable.</param>
        /// <param name="v4">On return, it retrieves the fourth value of the variable.</param>
        /// <returns>The first value of the uniform variable.</returns>
        public uint GetUniformUI(String name, out uint v2, out uint v3, out uint v4)
        {
            uint* puints = stackalloc uint[3];
            GL.glGetUniformuiv(program, uniformmappings[name], puints);
            v2 = puints[1];
            v3 = puints[2];
            v4 = puints[3];
            return puints[0];
        }

        /// <summary>
        /// Gets the value of the specified uniform as a .NET array with 4 elements. <br />
        /// Note: This WILL cause buffer overruns if your shader variable happens somehow to be larger than 4 floating-point values!
        /// </summary>
        /// <param name="name">The name of the uniform variable to retrieve it's value.</param>
        /// <returns>The value of the specified uniform value packed as an array of 4 elements.</returns>
        public float[] GetUniformArray(String name)
        {
            float[] ret = new float[4];
            fixed (float* p = ret) {
                GL.glGetUniformfv(program, uniformmappings[name], p);
            }
            return ret;
        }

        /// <summary>
        /// Gets the value of the specified uniform as a .NET array with 4 elements. <br />
        /// Note: This WILL cause buffer overruns if your shader variable happens somehow to be larger than 4 integer values!
        /// </summary>
        /// <param name="name">The name of the uniform variable to retrieve it's value.</param>
        /// <returns>The value of the specified uniform value packed as an array of 4 elements.</returns>
        public int[] GetUniformArrayI(String name)
        {
            int[] ret = new int[4];
            fixed (int* p = ret) {
                GL.glGetUniformiv(program, uniformmappings[name], p);
            }
            return ret;
        }

        /// <summary>
        /// Gets the value of the specified uniform as a .NET array with 4 elements. <br />
        /// Note: This WILL cause buffer overruns if your shader variable happens somehow to be larger than 4 unsigned integer values!
        /// </summary>
        /// <param name="name">The name of the uniform variable to retrieve it's value.</param>
        /// <returns>The value of the specified uniform value packed as an array of 4 elements.</returns>
        public uint[] GetUniformArrayUI(String name)
        {
            uint[] ret = new uint[4];
            fixed (uint* p = ret)
            {
                GL.glGetUniformuiv(program, uniformmappings[name], p);
            }
            return ret;
        }

        #endregion

        /// <summary>
        /// Gets the actual handle to the program. <br />
        /// Use it to pass it directly to OpenGL API's.
        /// </summary>
        public ProgramObject Handle => program;

        /// <summary>
        /// Disposes this program. <br />
        /// If the program is still in use, OpenGL will use it for the current frame and then it will delete it.
        /// </summary>
        public void Dispose()
        {
            GL.glDeleteProgram(program);
            GL.AssertError();
            uniformmappings = null;
        }
    }
}