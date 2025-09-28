
using System;
using MP.Annotations;
using System.Reflection;
using System.Diagnostics;
using MP.Annotations.CodeAnalysis;

namespace MP.Graphics.OpenGL
{
    /// <summary>
    /// Defines the currently used OpenGL core profile functions,
    /// and a convenient handling for initializing all the functions and destroying them.
    /// </summary>
    [Preliminary]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "CA2211:Non-constant fields should not be visible", Justification = "OpenGL Layer API")]
    public static unsafe class GL
    {
        /// <summary>
        /// Loads all the currently defined OpenGL functions, by using the specified function loader.
        /// </summary>
        /// <remarks>
        /// While it is not required by the interface itself, this function tests after every function is loaded or failure whether the loader also implements the <see cref="IDisposable"/> interface.
        /// If it does so, it calls that <see cref="IDisposable.Dispose"/> method implementation.
        /// </remarks>
        /// <param name="loader">The function loader to use.</param>
        [Throws(typeof(ArgumentNullException))]
        public static void LoadFunctions(IOpenGLFunctionLoader loader)
        {
            ArgumentNullException.ThrowIfNull(loader);
            try {
                foreach (var dgf in typeof(GL).GetFields(BindingFlags.Static | BindingFlags.Public))
                {
                    dgf.SetValue(null, Pointer.Box(loader.GetFunction(dgf.Name) , typeof(void*)));
                }
            } finally {
                if (loader is IDisposable d) { d.Dispose(); }
            }
        }

        /// <summary>
        /// Unloads all the functions previously loaded with <see cref="LoadFunctions(IOpenGLFunctionLoader)"/> method.
        /// </summary>
        public static void UnloadFunctions()
        {
            foreach (var f in typeof(GL).GetFields(BindingFlags.Public | BindingFlags.Static))
            {
                f.SetValue(null, null);
            }
        }

        #region OpenGL Declarations

        /// <summary>
        /// Gets the <c>glAttachShader</c> delegate.
        /// </summary>
        /// <remarks><c>void glAttachShader(ProgramObject program , ShaderObject shader)</c></remarks>
        public static delegate* unmanaged[Cdecl]<ProgramObject , ShaderObject , void> glAttachShader;

        /// <summary>
        /// Gets the <c>glBindBuffer</c> delegate.
        /// </summary>
        /// <remarks><c>void glBindBuffer(BufferObjectType type , BufferObject object , void)</c></remarks>
        public static delegate* unmanaged[Cdecl]<BufferObjectType, BufferObject , void> glBindBuffer;

        /// <summary>
        /// Gets the <c>glViewport</c> delegate.
        /// </summary>
        /// <remarks><c>void glViewport(int x , int y , int width , int height)</c></remarks>
        public static delegate* unmanaged[Cdecl]<int , int , int , int , void> glViewport;

        /// <summary>
        /// Gets the <c>glBufferData</c> delegate.
        /// </summary>
        /// <remarks><c>void glBufferData(GLenum target, GLsizeiptr datasize, const void* data, GLenum usage)</c></remarks>
        public static delegate* unmanaged[Cdecl]<BufferObjectType , int, void*, DataStoreUsagePattern , void> glBufferData;

        /// <summary>
        /// Gets the <c>glClear</c> delegate.
        /// </summary>
        /// <remarks><c>void glClear(GLbitfield mask)</c></remarks>
        public static delegate* unmanaged[Cdecl]<BufferBits , void> glClear;

        /// <summary>
        /// Gets the <c>glClearColor</c> delegate.
        /// </summary>
        /// <remarks><c>void glClearColor(GLfloat red, GLfloat green, GLfloat blue, GLfloat alpha)</c></remarks>
        public static delegate* unmanaged[Cdecl]<float , float , float , float , void> glClearColor;

        /// <summary>
        /// Gets the <c>glCreateProgram</c> delegate.
        /// </summary>
        /// <remarks><c>GLuint glCreateProgram(void)</c></remarks>
        public static delegate* unmanaged[Cdecl]<ProgramObject> glCreateProgram;

        /// <summary>
        /// Gets the <c>glGetShaderInfoLog</c> delegate.
        /// </summary>
        /// <remarks><c>void glGetShaderInfoLog(GLuint shader, GLsizei maxLength, GLsizei* length, GLchar* infoLog)</c></remarks>
        public static delegate* unmanaged[Cdecl]<ShaderObject , int , int* , byte* , void> glGetShaderInfoLog;

        /// <summary>
        /// Gets the <c>glCreateShader</c> delegate.
        /// </summary>
        /// <remarks><c>GLuint glCreateShader(GLenum shaderType)</c></remarks>
        public static delegate* unmanaged[Cdecl]<ShaderType, ShaderObject> glCreateShader;

        /// <summary>
        /// Gets the <c>glGenBuffers</c> delegate.
        /// </summary>
        /// <remarks><c>void glGenBuffers(GLsizei n, GLuint* buffers)</c></remarks>
        public static delegate* unmanaged[Cdecl]<int , BufferObject* , void> glGenBuffers;

        /// <summary>
        /// Gets the <c>glShaderCompile</c> delegate.
        /// </summary>
        /// <remarks><c>void glShaderCompile(GLuint shader)</c></remarks>
        public static delegate* unmanaged[Cdecl]<ShaderObject , void> glShaderCompile;

        /// <summary>
        /// Gets the <c>glShaderSource</c> delegate.
        /// </summary>
        /// <remarks><c>void glShaderSource(GLuint shader, GLsizei count, const GLchar**string, const GLint* length)</c></remarks>
        public static delegate* unmanaged[Cdecl]<ShaderObject , int , byte**, int* , void> glShaderSource;

        /// <summary>
        /// Gets the <c>glLinkProgram</c> delegate.
        /// </summary>
        /// <remarks><c>void glLinkProgram(GLuint program)</c></remarks>
        public static delegate* unmanaged[Cdecl]<ProgramObject , void> glLinkProgram;

        /// <summary>
        /// Gets the <c>glUseProgram</c> delegate.
        /// </summary>
        /// <remarks><c>void glUseProgram(GLuint program)</c></remarks>
        public static delegate* unmanaged[Cdecl]<ProgramObject, void> glUseProgram;

        /// <summary>
        /// Gets the <c>glDeleteProgram</c> delegate.
        /// </summary>
        /// <remarks><c>void glDeleteProgram(GLuint program)</c></remarks>
        public static delegate* unmanaged[Cdecl]<ProgramObject, void> glDeleteProgram;

        /// <summary>
        /// Gets the <c>glGetProgramiv</c> delegate.
        /// </summary>
        /// <remarks><c>void glGetProgramiv(GLuint program, GLenum pname, GLint*params)</c></remarks>
        public static delegate* unmanaged[Cdecl]<ProgramObject, uint, int*, void> glGetProgramiv;

        /// <summary>
        /// Gets the <c>glDetachShader</c> delegate.
        /// </summary>
        /// <remarks><c>void glDetachShader(GLuint program, GLuint shader)</c></remarks>
        public static delegate* unmanaged[Cdecl]<ProgramObject, ShaderObject, void> glDetachShader;

        /// <summary>
        /// Gets the <c>glDeleteShader</c> delegate.
        /// </summary>
        /// <remarks><c>void glDeleteShader(GLuint shader)</c></remarks>
        public static delegate* unmanaged[Cdecl]<ShaderObject, void> glDeleteShader;

        /// <summary>
        /// Gets the <c>glGetShaderiv</c> delegate.
        /// </summary>
        /// <remarks><c>void glGetShaderiv(GLuint shader, GLenum pname, GLint* params)</c></remarks>
        public static delegate* unmanaged[Cdecl]<ShaderObject , uint , int*, void> glGetShaderiv;

        /// <summary>
        /// Gets the <c>glGenTextures</c> delegate.
        /// </summary>
        /// <remarks><c>void glGenTextures(GLsizei n, GLuint* textures)</c></remarks>
        public static delegate* unmanaged[Cdecl]<int, TextureObject*, void> glGenTextures;

        /// <summary>
        /// Gets the <c>glDeleteTextures</c> delegate.
        /// </summary>
        /// <remarks><c>void glDeleteTextures(GLsizei n, const GLuint* textures)</c></remarks>
        public static delegate* unmanaged[Cdecl]<int, TextureObject*, void> glDeleteTextures;

        /// <summary>
        /// Gets the <c>glBindTexture</c> delegate.
        /// </summary>
        /// <remarks><c>void glBindTexture(GLenum target, GLuint texture)</c></remarks>
        public static delegate* unmanaged[Cdecl]<TextureObjectType, TextureObject, void> glBindTexture;

        /// <summary>
        /// Gets the <c>glTexImage2D</c> delegate.
        /// </summary>
        /// <remarks><c>void glTexImage2D(GLenum target, GLint level, GLint internalformat, GLsizei width, GLsizei height, GLint border, GLenum format, GLenum type, const void * data)</c></remarks>
        public static delegate* unmanaged[Cdecl]<TextureObjectType, int, BaseTextureFormat, int, int, int, BaseTextureFormat, PixelDataType, void*, void> glTexImage2D;

        /// <summary>
        /// Gets the <c>glPixelStorei</c> delegate.
        /// </summary>
        /// <remarks><c>void glPixelStorei(GLenum pname, GLint param)</c></remarks>
        public static delegate* unmanaged[Cdecl]<PixelStoreCommandType, int, void> glPixelStorei;

        /// <summary>
        /// Gets the <c>glPixelStoref</c> delegate.
        /// </summary>
        /// <remarks><c>void glPixelStoref(GLenum pname, GLfloat param)</c></remarks>
        public static delegate* unmanaged[Cdecl]<PixelStoreCommandType, float, void> glPixelStoref;

        /// <summary>
        /// Gets the <c>glReadPixels</c> delegate.
        /// </summary>
        /// <remarks><c>void glReadPixels(GLint x,GLint y, GLsizei width, GLsizei height, GLenum format, GLenum type,void* data)</c></remarks>
        public static delegate* unmanaged[Cdecl]<int, int, int, int, BaseTextureFormat, PixelDataType, void*, void> glReadPixels;

        /// <summary>
        /// Gets the <c>glActiveTexture</c> delegate.
        /// </summary>
        /// <remarks><c>void glActiveTexture(GLenum texture)</c></remarks>
        public static delegate* unmanaged[Cdecl]<ActiveTextureOrdinal, void> glActiveTexture;

        /// <summary>
        /// Gets the <c>glCopyTexImage2D</c> delegate.
        /// </summary>
        /// <remarks><c>void glCopyTexImage2D(GLenum target, GLint level, GLenum internalformat, GLint x, GLint y, GLsizei width, GLsizei height, GLint border)</c></remarks>
        public static delegate* unmanaged[Cdecl]<TextureObjectType, int, BaseTextureFormat, int, int, int, int, int, void> glCopyTexImage2D;

        /// <summary>
        /// Gets the <c>glTexParameterf</c> delegate.
        /// </summary>
        /// <remarks><c>void glTexParameterf(GLenum target, GLenum pname, GLfloat param)</c></remarks>
        public static delegate* unmanaged[Cdecl]<TextureObjectType, TextureParameterName, float, void> glTexParameterf;

        /// <summary>
        /// Gets the <c>glTexParameteri</c> delegate.
        /// </summary>
        /// <remarks><c>void glTexParameteri(GLenum target, GLenum pname, GLint param)</c></remarks>
        public static delegate* unmanaged[Cdecl]<TextureObjectType, TextureParameterName, int, void> glTexParameteri;

        /// <summary>
        /// Gets the <c>glTexParameterfv</c> delegate.
        /// </summary>
        /// <remarks><c>void glTexParameterfv(GLenum target, GLenum pname, const GLfloat * params)</c></remarks>
        public static delegate* unmanaged[Cdecl]<TextureObjectType, TextureParameterName, float*, void> glTexParameterfv;

        /// <summary>
        /// Gets the <c>glTexParameteriv</c> delegate.
        /// </summary>
        /// <remarks><c>void glTexParameteri(GLenum target, GLenum pname, const GLint * param)</c></remarks>
        public static delegate* unmanaged[Cdecl]<TextureObjectType, TextureParameterName, int*, void> glTexParameteriv;

        /// <summary>
        /// Gets the <c>glGetError</c> delegate.
        /// </summary>
        /// <remarks><c>GLenum glGetError(void)</c></remarks>
        public static delegate* unmanaged[Cdecl]<ErrorCode> glGetError;

        /// <summary>
        /// Gets the <c>glGetString</c> delegate.
        /// </summary>
        /// <remarks><c>const GLubyte* glGetString(GLenum name)</c></remarks>
        public static delegate* unmanaged[Cdecl]<GlGetStringType, byte*> glGetString;

        /// <summary>
        /// Gets the <c>glGetStringi</c> delegate.
        /// </summary>
        /// <remarks><c>const GLubyte* glGetStringi(GLenum name, GLuint index)</c></remarks>
        public static delegate* unmanaged[Cdecl]<GlGetStringType, int, byte*> glGetStringi;

        #endregion

        #region Utility functions

        /// <summary>
        /// Asserts the error after calling an OpenGL function and you wish to check the result during application debugging. <br />
        /// On Release builds, this function is removed for performance.
        /// </summary>
        [Conditional("DEBUG")]
        public static void AssertError()
        {
            switch (glGetError())
            {
                case ErrorCode.GL_NO_ERROR:
                    return;
                case ErrorCode.GL_INVALID_ENUM:
                    throw new ArgumentException("An unacceptable value is specified for an enumerated argument.");
                case ErrorCode.GL_INVALID_OPERATION:
                    throw new InvalidOperationException("The specified operation is not allowed in the current state.");
                case ErrorCode.GL_INVALID_VALUE:
                    throw new ArgumentOutOfRangeException("A numeric argument is out of range." , innerException: null);
                case ErrorCode.GL_INVALID_FRAMEBUFFER_OPERATION:
                    throw new InvalidOperationException("The framebuffer object is not complete.");
                case ErrorCode.GL_OUT_OF_MEMORY:
                    throw new OutOfMemoryException("There is not enough memory left to execute the command.");
            }
        }

        /// <summary>
        /// Identical to <see cref="glGetString"/> but provides a wrapper for it in .NET .
        /// </summary>
        /// <param name="type">The type of string to retrieve.</param>
        /// <returns>The string contents as a .NET string.</returns>
        public static System.String GLGetString(GlGetStringType type)
        {
            byte* p = glGetString(type);
            if (p is null) {
                AssertError(); // But this will be removed during a release build, so it is OK to do this
                return null; 
            }
            return new((System.SByte*)p);
        }

        /// <summary>
        /// Identical to <see cref="glGetStringi"/> but provides a wrapper for it in .NET .
        /// </summary>
        /// <param name="type">The type of string to retrieve.</param>
        /// <param name="index">Request-specific value.</param>
        /// <returns>The string contents as a .NET string.</returns>
        public static System.String GLGetStringI(GlGetStringType type , int index)
        {
            byte* p = glGetStringi(type , index);
            if (p is null) {
                AssertError(); // But this will be removed during a release build, so it is OK to do this
                return null;
            }
            return new((System.SByte*)p);
        }

        #endregion

    }
}