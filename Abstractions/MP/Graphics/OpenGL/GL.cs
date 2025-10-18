
using System;
using MP.Annotations;
using System.Reflection;
using System.Diagnostics;
using MP.Annotations.CodeAnalysis;

namespace MP.Graphics.OpenGL
{
    /*
      /// <summary>Gets the <c></c> delegate.</summary>
      /// <remarks><c></c></remarks>
    */

    /// <summary>
    /// Defines the currently used OpenGL 3.3 core profile functions,
    /// and a convenient handling for initializing all the functions and destroying them.
    /// </summary>
    [Preliminary]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "CA2211:Non-constant fields should not be visible", Justification = "OpenGL Layer API")]
    public static unsafe class GL
    {
        /// <summary>
        /// Loads all the currently defined OpenGL functions, by using the specified function loader.
        /// </summary>
        /// <param name="loader">The function loader to use.</param>
        /// <exception cref="ArgumentNullException"><paramref name="loader"/> was <see langword="null"/>.</exception>
        [Throws(typeof(ArgumentNullException))]
        public static void LoadFunctions(IOpenGLFunctionLoader loader)
        {
            ArgumentNullException.ThrowIfNull(loader);
            var voidpointertype = typeof(void*);
#if DEBUG
            var fields = typeof(GL).GetFields(BindingFlags.Public | BindingFlags.Static);
            DebugProvider.WriteLine($"OPENGLFUNCLOADER: Attempting to load {fields.LongLength} function pointers");
            foreach (var dgf in fields)
            {
                try {
                    dgf.SetValue(null, Pointer.Box(loader.GetFunction(dgf.Name), voidpointertype));
                } catch (Exception) {
                    DebugProvider.WriteLine($"OPENGLFUNCLOADER: Preparing LOADEXCEPTION report. Function {dgf.Name} failed to be loaded.");
                    throw;
                }
            }
            DebugProvider.WriteLine("OPENGLFUNCLOADER: Loading complete!");
#else
            foreach (var dgf in typeof(GL).GetFields(BindingFlags.Static | BindingFlags.Public))
            {
                dgf.SetValue(null, Pointer.Box(loader.GetFunction(dgf.Name), voidpointertype));
            }
#endif
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

        /// <summary>Gets the <c>glAttachShader</c> delegate.</summary>
        /// <remarks><c>void glAttachShader(ProgramObject program , ShaderObject shader)</c></remarks>
        public static delegate* unmanaged<ProgramObject , ShaderObject , void> glAttachShader;

        /// <summary>Gets the <c>glBindBuffer</c> delegate.</summary>
        /// <remarks><c>void glBindBuffer(BufferObjectType type , BufferObject object)</c></remarks>
        public static delegate* unmanaged<BufferObjectType, BufferObject , void> glBindBuffer;

        /// <summary>Gets the <c>glViewport</c> delegate.</summary>
        /// <remarks><c>void glViewport(int x , int y , int width , int height)</c></remarks>
        public static delegate* unmanaged<int , int , int , int , void> glViewport;

        /// <summary>Gets the <c>glBufferData</c> delegate.</summary>
        /// <remarks><c>void glBufferData(GLenum target, GLsizeiptr datasize, const void* data, GLenum usage)</c></remarks>
        public static delegate* unmanaged<BufferObjectType , int, void*, DataStoreUsagePattern , void> glBufferData;

        /// <summary>Gets the <c>glClear</c> delegate.</summary>
        /// <remarks><c>void glClear(GLbitfield mask)</c></remarks>
        public static delegate* unmanaged<BufferBits , void> glClear;

        /// <summary>Gets the <c>glClearColor</c> delegate.</summary>
        /// <remarks><c>void glClearColor(GLfloat red, GLfloat green, GLfloat blue, GLfloat alpha)</c></remarks>
        public static delegate* unmanaged<float , float , float , float , void> glClearColor;

        /// <summary>Gets the <c>glCreateProgram</c> delegate.</summary>
        /// <remarks><c>GLuint glCreateProgram(void)</c></remarks>
        public static delegate* unmanaged<ProgramObject> glCreateProgram;

        /// <summary>Gets the <c>glValidateProgram</c> delegate.</summary>
        /// <remarks><c>void glValidateProgram(GLuint program)</c></remarks>
        public static delegate* unmanaged<ProgramObject, void> glValidateProgram;

        /// <summary>Gets the <c>glGetProgramInfoLog</c> delegate.</summary>
        /// <remarks><c>void glGetProgramInfoLog(GLuint program, GLsizei maxLength, GLsizei* length, GLchar* infoLog)</c></remarks>
        public static delegate* unmanaged<ProgramObject, int, int*, byte*, void> glGetProgramInfoLog;

        /// <summary>Gets the <c>glGetAttachedShaders</c> delegate.</summary>
        /// <remarks><c>void glGetAttachedShaders(	GLuint program, GLsizei maxCount, GLsizei *count, GLuint* shaders)</c></remarks>
        public static delegate* unmanaged<ProgramObject, int, int*, ShaderObject*, void> glGetAttachedShaders;

        /// <summary>Gets the <c>glGetShaderInfoLog</c> delegate.</summary>
        /// <remarks><c>void glGetShaderInfoLog(GLuint shader, GLsizei maxLength, GLsizei* length, GLchar* infoLog)</c></remarks>
        public static delegate* unmanaged<ShaderObject , int , int* , byte* , void> glGetShaderInfoLog;

        /// <summary>Gets the <c>glCreateShader</c> delegate.</summary>
        /// <remarks><c>GLuint glCreateShader(GLenum shaderType)</c></remarks>
        public static delegate* unmanaged<ShaderType, ShaderObject> glCreateShader;

        /// <summary>Gets the <c>glGenBuffers</c> delegate.</summary>
        /// <remarks><c>void glGenBuffers(GLsizei n, GLuint* buffers)</c></remarks>
        public static delegate* unmanaged<int , BufferObject* , void> glGenBuffers;

        /// <summary>Gets the <c>glCompileShader</c> delegate.</summary>
        /// <remarks><c>void glCompileShader(GLuint shader)</c></remarks>
        public static delegate* unmanaged<ShaderObject , void> glCompileShader;

        /// <summary>Gets the <c>glShaderSource</c> delegate.</summary>
        /// <remarks><c>void glShaderSource(GLuint shader, GLsizei count, const GLchar**string, const GLint* length)</c></remarks>
        public static delegate* unmanaged<ShaderObject , int , byte**, int* , void> glShaderSource;

        /// <summary>Gets the <c>glLinkProgram</c> delegate.</summary>
        /// <remarks><c>void glLinkProgram(GLuint program)</c></remarks>
        public static delegate* unmanaged<ProgramObject , void> glLinkProgram;

        /// <summary>Gets the <c>glUseProgram</c> delegate.</summary>
        /// <remarks><c>void glUseProgram(GLuint program)</c></remarks>
        public static delegate* unmanaged<ProgramObject, void> glUseProgram;

        /// <summary>Gets the <c>glDeleteProgram</c> delegate.</summary>
        /// <remarks><c>void glDeleteProgram(GLuint program)</c></remarks>
        public static delegate* unmanaged<ProgramObject, void> glDeleteProgram;

        /// <summary>Gets the <c>glGetProgramiv</c> delegate.</summary>
        /// <remarks><c>void glGetProgramiv(GLuint program, GLenum pname, GLint*params)</c></remarks>
        public static delegate* unmanaged<ProgramObject, ProgramParameter, int*, void> glGetProgramiv;

        /// <summary>Gets the <c>glDetachShader</c> delegate.</summary>
        /// <remarks><c>void glDetachShader(GLuint program, GLuint shader)</c></remarks>
        public static delegate* unmanaged<ProgramObject, ShaderObject, void> glDetachShader;

        /// <summary>Gets the <c>glDeleteShader</c> delegate.</summary>
        /// <remarks><c>void glDeleteShader(GLuint shader)</c></remarks>
        public static delegate* unmanaged<ShaderObject, void> glDeleteShader;

        /// <summary>Gets the <c>glGetShaderiv</c> delegate.</summary>
        /// <remarks><c>void glGetShaderiv(GLuint shader, GLenum pname, GLint* params)</c></remarks>
        public static delegate* unmanaged<ShaderObject , ShaderParameter , int*, void> glGetShaderiv;

        /// <summary>Gets the <c>glGenTextures</c> delegate.</summary>
        /// <remarks><c>void glGenTextures(GLsizei n, GLuint* textures)</c></remarks>
        public static delegate* unmanaged<int, TextureObject*, void> glGenTextures;

        /// <summary>Gets the <c>glBufferSubData</c> delegate.</summary>
        /// <remarks><c>void glBufferSubData(GLenum target​, GLintptr offset​, GLsizeiptr size​, const GLvoid *data​)</c></remarks>
        public static delegate* unmanaged<BufferObjectType, int , int , void* , void> glBufferSubData;

        /// <summary>Gets the <c>glDeleteTextures</c> delegate.</summary>
        /// <remarks><c>void glDeleteTextures(GLsizei n, const GLuint* textures)</c></remarks>
        public static delegate* unmanaged<int, TextureObject*, void> glDeleteTextures;

        /// <summary>Gets the <c>glBindTexture</c> delegate.</summary>
        /// <remarks><c>void glBindTexture(GLenum target, GLuint texture)</c></remarks>
        public static delegate* unmanaged<TextureObjectType, TextureObject, void> glBindTexture;

        /// <summary>Gets the <c>glTexImage2D</c> delegate.</summary>
        /// <remarks><c>void glTexImage2D(GLenum target, GLint level, GLint internalformat, GLsizei width, GLsizei height, GLint border, GLenum format, GLenum type, const void * data)</c></remarks>
        public static delegate* unmanaged<TextureObjectType, int, BaseTextureFormat, int, int, int, BaseTextureFormat, PixelDataType, void*, void> glTexImage2D;

        /// <summary>Gets the <c>glPixelStorei</c> delegate.</summary>
        /// <remarks><c>void glPixelStorei(GLenum pname, GLint param)</c></remarks>
        public static delegate* unmanaged<PixelStoreCommandType, int, void> glPixelStorei;

        /// <summary>Gets the <c>glPixelStoref</c> delegate.</summary>
        /// <remarks><c>void glPixelStoref(GLenum pname, GLfloat param)</c></remarks>
        public static delegate* unmanaged<PixelStoreCommandType, float, void> glPixelStoref;

        /// <summary>Gets the <c>glReadPixels</c> delegate.</summary>
        /// <remarks><c>void glReadPixels(GLint x,GLint y, GLsizei width, GLsizei height, GLenum format, GLenum type,void* data)</c></remarks>
        public static delegate* unmanaged<int, int, int, int, BaseTextureFormat, PixelDataType, void*, void> glReadPixels;

        /// <summary>Gets the <c>glActiveTexture</c> delegate.</summary>
        /// <remarks><c>void glActiveTexture(GLenum texture)</c></remarks>
        public static delegate* unmanaged<ActiveTextureOrdinal, void> glActiveTexture;

        /// <summary>Gets the <c>glCopyTexImage2D</c> delegate.</summary>
        /// <remarks><c>void glCopyTexImage2D(GLenum target, GLint level, GLenum internalformat, GLint x, GLint y, GLsizei width, GLsizei height, GLint border)</c></remarks>
        public static delegate* unmanaged<TextureObjectType, int, BaseTextureFormat, int, int, int, int, int, void> glCopyTexImage2D;

        /// <summary>Gets the <c>glTexParameterf</c> delegate.</summary>
        /// <remarks><c>void glTexParameterf(GLenum target, GLenum pname, GLfloat param)</c></remarks>
        public static delegate* unmanaged<TextureObjectType, TextureParameterName, float, void> glTexParameterf;

        /// <summary>Gets the <c>glTexParameteri</c> delegate.</summary>
        /// <remarks><c>void glTexParameteri(GLenum target, GLenum pname, GLint param)</c></remarks>
        public static delegate* unmanaged<TextureObjectType, TextureParameterName, int, void> glTexParameteri;

        /// <summary>Gets the <c>glTexParameterfv</c> delegate.</summary>
        /// <remarks><c>void glTexParameterfv(GLenum target, GLenum pname, const GLfloat * params)</c></remarks>
        public static delegate* unmanaged<TextureObjectType, TextureParameterName, float*, void> glTexParameterfv;

        /// <summary>Gets the <c>glTexParameteriv</c> delegate.</summary>
        /// <remarks><c>void glTexParameteri(GLenum target, GLenum pname, const GLint * param)</c></remarks>
        public static delegate* unmanaged<TextureObjectType, TextureParameterName, int*, void> glTexParameteriv;

        /// <summary>Gets the <c>glVertexAttrib1f</c> delegate.</summary>
        /// <remarks><c>void glVertexAttrib1f(GLuint index, GLfloat v0)</c></remarks>
        public static delegate* unmanaged<uint, float, void> glVertexAttrib1f;

        /// <summary>Gets the <c>glVertexAttrib1s</c> delegate.</summary>
        /// <remarks><c>void glVertexAttrib1s(GLuint index, GLshort v0)</c></remarks>
        public static delegate* unmanaged<uint, short, void> glVertexAttrib1s;

        /// <summary>Gets the <c>glVertexAttrib1d</c> delegate.</summary>
        /// <remarks><c>void glVertexAttrib1d(GLuint index, GLdouble v0)</c></remarks>
        public static delegate* unmanaged<uint, double, void> glVertexAttrib1d;

        /// <summary>Gets the <c>glVertexAttribI1i</c> delegate.</summary>
        /// <remarks><c>void glVertexAttribI1i(GLuint index, GLint v0)</c></remarks>
        public static delegate* unmanaged<uint, int, void> glVertexAttribI1i;

        /// <summary>Gets the <c>glVertexAttribI1ui</c> delegate.</summary>
        /// <remarks><c>void glVertexAttribI1ui(GLuint index, GLuint v0)</c></remarks>
        public static delegate* unmanaged<uint, uint, void> glVertexAttribI1ui;

        /// <summary>Gets the <c>glVertexAttrib2f</c> delegate.</summary>
        /// <remarks><c>void glVertexAttrib2f(GLuint index, GLfloat v0, GLfloat v1)</c></remarks>
        public static delegate* unmanaged<uint, float, float, void> glVertexAttrib2f;

        /// <summary>Gets the <c>glVertexAttrib2s</c> delegate.</summary>
        /// <remarks><c>void glVertexAttrib2s(GLuint index, GLshort v0, GLshort v1)</c></remarks>
        public static delegate* unmanaged<uint, short, short, void> glVertexAttrib2s;

        /// <summary>Gets the <c>glVertexAttrib2d</c> delegate.</summary>
        /// <remarks><c>void glVertexAttrib2d(GLuint index, GLdouble v0, GLdouble v1)</c></remarks>
        public static delegate* unmanaged<uint, double, double, void> glVertexAttrib2d;

        /// <summary>Gets the <c>glVertexAttribI2i</c> delegate.</summary>
        /// <remarks><c>void glVertexAttribI2i(GLuint index, GLint v0, GLint v1)</c></remarks>
        public static delegate* unmanaged<uint, int, int, void> glVertexAttribI2i;

        /// <summary>Gets the <c>glVertexAttribI2ui</c> delegate.</summary>
        /// <remarks><c>void glVertexAttribI2ui(GLuint index, GLuint v0, GLuint v1)</c></remarks>
        public static delegate* unmanaged<uint, uint, uint, void> glVertexAttribI2ui;

        /// <summary>Gets the <c>glVertexAttrib3f</c> delegate.</summary>
        /// <remarks><c>void glVertexAttrib3f(GLuint index, GLfloat v0, GLfloat v1, GLfloat v2)</c></remarks>
        public static delegate* unmanaged<uint, float, float, float, void> glVertexAttrib3f;

        /// <summary>Gets the <c>glVertexAttrib3s</c> delegate.</summary>
        /// <remarks><c>void glVertexAttrib3s(GLuint index, GLshort v0, GLshort v1, GLshort v2)</c></remarks>
        public static delegate* unmanaged<uint, short, short, short, void> glVertexAttrib3s;

        /// <summary>Gets the <c>glVertexAttrib3d</c> delegate.</summary>
        /// <remarks><c>void glVertexAttrib3d(GLuint index, GLdouble v0, GLdouble v1, GLdouble v2)</c></remarks>
        public static delegate* unmanaged<uint, double, double, double, void> glVertexAttrib3d;

        /// <summary>Gets the <c>glVertexAttribI3i</c> delegate.</summary>
        /// <remarks><c>void glVertexAttribI3i(GLuint index, GLint v0, GLint v1, GLint v2)</c></remarks>
        public static delegate* unmanaged<uint, int, int, int, void> glVertexAttribI3i;

        /// <summary>Gets the <c>glVertexAttribI3ui</c> delegate.</summary>
        /// <remarks><c>void glVertexAttribI3ui(GLuint index, GLuint v0, GLuint v1, GLuint v2)</c></remarks>
        public static delegate* unmanaged<uint, uint, uint, uint, void> glVertexAttribI3ui;

        /// <summary>Gets the <c></c> delegate.</summary>
        /// <remarks><c>void glVertexAttrib4f(GLuint index, GLfloat v0, GLfloat v1, GLfloat v2, GLfloat v3)</c></remarks>
        public static delegate* unmanaged<uint, float, float, float, float, void> glVertexAttrib4f;

        /// <summary>Gets the <c>glVertexAttrib4s</c> delegate.</summary>
        /// <remarks><c>void glVertexAttrib4s(GLuint index, GLshort v0, GLshort v1, GLshort v2, GLshort v3)</c></remarks>
        public static delegate* unmanaged<uint, short, short, short, short, void> glVertexAttrib4s;

        /// <summary>Gets the <c>glVertexAttrib4d</c> delegate.</summary>
        /// <remarks><c>void glVertexAttrib4d(GLuint index, GLdouble v0, GLdouble v1, GLdouble v2, GLdouble v3)</c></remarks>
        public static delegate* unmanaged<uint, double, double, double, double, void> glVertexAttrib4d;

        /// <summary>Gets the <c>glVertexAttrib4Nub</c> delegate.</summary>
        /// <remarks><c>void glVertexAttrib4Nub(GLuint index, GLubyte v0, GLubyte v1, GLubyte v2, GLubyte v3)</c></remarks>
        public static delegate* unmanaged<uint, byte, byte, byte, byte, void> glVertexAttrib4Nub;

        /// <summary>Gets the <c>glVertexAttribI4i</c> delegate.</summary>
        /// <remarks><c>void glVertexAttribI4i(GLuint index, GLint v0, GLint v1, GLint v2, GLint v3)</c></remarks>
        public static delegate* unmanaged<uint, int, int, int, int, void> glVertexAttribI4i;

        /// <summary>Gets the <c>glVertexAttribI4ui</c> delegate.</summary>
        /// <remarks><c>void glVertexAttribI4ui(GLuint index, GLuint v0, GLuint v1, GLuint v2, GLuint v3)</c></remarks>
        public static delegate* unmanaged<uint, uint, uint, uint, uint, void> glVertexAttribI4ui;

        /// <summary>Gets the <c>glVertexAttribPointer</c> delegate.</summary>
        /// <remarks><c>void glVertexAttribPointer(GLuint index, GLint size, GLenum type, GLboolean normalized, GLsizei stride, const void* pointer)</c></remarks>
        public static delegate* unmanaged<uint, int, VertexAttributePointerDataType, byte, int, void*, void> glVertexAttribPointer;

        /// <summary>Gets the <c>glVertexAttribIPointer</c> delegate.</summary>
        /// <remarks><c>void glVertexAttribIPointer(GLuint index, GLint size, GLenum type, GLsizei stride, const void* pointer)</c></remarks>
        public static delegate* unmanaged<uint, int, VertexAttributePointerDataType, int, void*, void> glVertexAttribIPointer;

        /// <summary>Gets the <c>glEnableVertexAttribArray</c> delegate.</summary>
        /// <remarks><c>void glEnableVertexAttribArray(GLuint index)</c></remarks>
        public static delegate* unmanaged<uint, void> glEnableVertexAttribArray;

        /// <summary>Gets the <c>glDisableVertexAttribArray</c> delegate.</summary>
        /// <remarks><c>void glDisableVertexAttribArray(GLuint index)</c></remarks>
        public static delegate* unmanaged<uint, void> glDisableVertexAttribArray;

        /// <summary>Gets the <c>glUniform1f</c> delegate.</summary>
        /// <remarks><c>void glUniform1f(GLint location, GLfloat v0)</c></remarks>
        public static delegate* unmanaged<int, float, void> glUniform1f;

        /// <summary>Gets the <c>glUniform2f</c> delegate.</summary>
        /// <remarks><c>void glUniform2f(GLint location, GLfloat v0, GLfloat v1)</c></remarks>
        public static delegate* unmanaged<int, float, float, void> glUniform2f;

        /// <summary>Gets the <c>glUniform3f</c> delegate.</summary>
        /// <remarks><c>void glUniform3f(GLint location, GLfloat v0, GLfloat v1, GLfloat v2)</c></remarks>
        public static delegate* unmanaged<int, float, float, float, void> glUniform3f;

        /// <summary>Gets the <c>glUniform4f</c> delegate.</summary>
        /// <remarks><c>void glUniform4f(GLint location, GLfloat v0, GLfloat v1, GLfloat v2, GLfloat v3)</c></remarks>
        public static delegate* unmanaged<int, float, float, float, float, void> glUniform4f;

        /// <summary>Gets the <c>glUniform1i</c> delegate.</summary>
        /// <remarks><c>void glUniform1i(GLint location, GLint v0)</c></remarks>
        public static delegate* unmanaged<int, int, void> glUniform1i;

        /// <summary>Gets the <c>glUniform2i</c> delegate.</summary>
        /// <remarks><c>void glUniform1i(GLint location, GLint v0, GLint v1)</c></remarks>
        public static delegate* unmanaged<int, int, int, void> glUniform2i;

        /// <summary>Gets the <c>glUniform3i</c> delegate.</summary>
        /// <remarks><c>void glUniform1i(GLint location, GLint v0, GLint v1, GLint v2)</c></remarks>
        public static delegate* unmanaged<int, int, int, int, void> glUniform3i;

        /// <summary>Gets the <c>glUniform4i</c> delegate.</summary>
        /// <remarks><c>void glUniform1i(GLint location, GLint v0, GLint v1, GLint v2, GLint v3)</c></remarks>
        public static delegate* unmanaged<int, int, int, int, int, void> glUniform4i;

        /// <summary>Gets the <c>glUniform1ui</c> delegate.</summary>
        /// <remarks><c>void glUniform1ui(GLint location, GLuint v0)</c></remarks>
        public static delegate* unmanaged<int, uint, void> glUniform1ui;

        /// <summary>Gets the <c>glUniform2ui</c> delegate.</summary>
        /// <remarks><c>void glUniform2ui(GLint location, GLuint v0, GLuint v1)</c></remarks>
        public static delegate* unmanaged<int, uint, uint, void> glUniform2ui;

        /// <summary>Gets the <c>glUniform3ui</c> delegate.</summary>
        /// <remarks><c>void glUniform3ui(GLint location, GLuint v0, GLuint v1, GLuint v2)</c></remarks>
        public static delegate* unmanaged<int, uint, uint, uint, void> glUniform3ui;

        /// <summary>Gets the <c>glUniform4ui</c> delegate.</summary>
        /// <remarks><c>void glUniform4ui(GLint location, GLuint v0, GLuint v1, GLuint v2, GLuint v3)</c></remarks>
        public static delegate* unmanaged<int, uint, uint, uint, uint, void> glUniform4ui;

        /// <summary>Gets the <c>glUniform1fv</c> delegate.</summary>
        /// <remarks><c>void glUniform1fv(GLint location, GLsizei count, const GLfloat* value)</c></remarks>
        public static delegate* unmanaged<int, int, float*, void> glUniform1fv;

        /// <summary>Gets the <c>glUniform2fv</c> delegate.</summary>
        /// <remarks><c>void glUniform2fv(GLint location, GLsizei count, const GLfloat* value)</c></remarks>
        public static delegate* unmanaged<int, int, float*, void> glUniform2fv;

        /// <summary>Gets the <c>glUniform3fv</c> delegate.</summary>
        /// <remarks><c>void glUniform3fv(GLint location, GLsizei count, const GLfloat* value)</c></remarks>
        public static delegate* unmanaged<int, int, float*, void> glUniform3fv;

        /// <summary>Gets the <c>glUniform4fv</c> delegate.</summary>
        /// <remarks><c>void glUniform4fv(GLint location, GLsizei count, const GLfloat* value)</c></remarks>
        public static delegate* unmanaged<int, int, float*, void> glUniform4fv;

        /// <summary>Gets the <c>glUniform1iv</c> delegate.</summary>
        /// <remarks><c>void glUniform1iv(GLint location, GLsizei count, const GLint* value)</c></remarks>
        public static delegate* unmanaged<int, int, int*, void> glUniform1iv;

        /// <summary>Gets the <c>glUniform2iv</c> delegate.</summary>
        /// <remarks><c>void glUniform2iv(GLint location, GLsizei count, const GLint* value)</c></remarks>
        public static delegate* unmanaged<int, int, int*, void> glUniform2iv;

        /// <summary>Gets the <c>glUniform3iv</c> delegate.</summary>
        /// <remarks><c>void glUniform3iv(GLint location, GLsizei count, const GLint* value)</c></remarks>
        public static delegate* unmanaged<int, int, int*, void> glUniform3iv;

        /// <summary>Gets the <c>glUniform4iv</c> delegate.</summary>
        /// <remarks><c>void glUniform4iv(GLint location, GLsizei count, const GLint* value)</c></remarks>
        public static delegate* unmanaged<int, int, int*, void> glUniform4iv;

        /// <summary>Gets the <c>glUniform1uiv</c> delegate.</summary>
        /// <remarks><c>void glUniform1uiv(GLint location, GLsizei count, const GLuint* value)</c></remarks>
        public static delegate* unmanaged<int, int, uint*, void> glUniform1uiv;

        /// <summary>Gets the <c>glUniform2uiv</c> delegate.</summary>
        /// <remarks><c>void glUniform2uiv(GLint location, GLsizei count, const GLuint* value)</c></remarks>
        public static delegate* unmanaged<int, int, uint*, void> glUniform2uiv;

        /// <summary>Gets the <c>glUniform3uiv</c> delegate.</summary>
        /// <remarks><c>void glUniform3uiv(GLint location, GLsizei count, const GLuint* value)</c></remarks>
        public static delegate* unmanaged<int, int, uint*, void> glUniform3uiv;

        /// <summary>Gets the <c>glUniform4uiv</c> delegate.</summary>
        /// <remarks><c>void glUniform4uiv(GLint location, GLsizei count, const GLuint* value)</c></remarks>
        public static delegate* unmanaged<int, int, uint*, void> glUniform4uiv;

        /// <summary>Gets the <c>glUniformMatrix2fv</c> delegate.</summary>
        /// <remarks><c>void glUniformMatrix2fv(GLint location, GLsizei count, GLboolean transpose, const GLfloat* value)</c></remarks>
        public static delegate* unmanaged<int, int, byte, float*, void> glUniformMatrix2fv;

        /// <summary>Gets the <c>glUniformMatrix3fv</c> delegate.</summary>
        /// <remarks><c>void glUniformMatrix3fv(GLint location, GLsizei count, GLboolean transpose, const GLfloat* value)</c></remarks>
        public static delegate* unmanaged<int, int, byte, float*, void> glUniformMatrix3fv;

        /// <summary>Gets the <c>glUniformMatrix4fv</c> delegate.</summary>
        /// <remarks><c>void glUniformMatrix4fv(GLint location, GLsizei count, GLboolean transpose, const GLfloat* value)</c></remarks>
        public static delegate* unmanaged<int, int, byte, float*, void> glUniformMatrix4fv;

        /// <summary>Gets the <c>glUniformMatrix2x3fv</c> delegate.</summary>
        /// <remarks><c>void glUniformMatrix2x3fv(GLint location, GLsizei count, GLboolean transpose, const GLfloat* value)</c></remarks>
        public static delegate* unmanaged<int, int, byte, float*, void> glUniformMatrix2x3fv;

        /// <summary>Gets the <c>glUniformMatrix3x2fv</c> delegate.</summary>
        /// <remarks><c>void glUniformMatrix3x2fv(GLint location, GLsizei count, GLboolean transpose, const GLfloat* value)</c></remarks>
        public static delegate* unmanaged<int, int, byte, float*, void> glUniformMatrix3x2fv;

        /// <summary>Gets the <c>glUniformMatrix2x4fv</c> delegate.</summary>
        /// <remarks><c>void glUniformMatrix2x4fv(GLint location, GLsizei count, GLboolean transpose, const GLfloat* value)</c></remarks>
        public static delegate* unmanaged<int, int, byte, float*, void> glUniformMatrix2x4fv;

        /// <summary>Gets the <c>glUniformMatrix4x2fv</c> delegate.</summary>
        /// <remarks><c>void glUniformMatrix4x2fv(GLint location, GLsizei count, GLboolean transpose, const GLfloat* value)</c></remarks>
        public static delegate* unmanaged<int, int, byte, float*, void> glUniformMatrix4x2fv;

        /// <summary>Gets the <c>glUniformMatrix3x4fv</c> delegate.</summary>
        /// <remarks><c>void glUniformMatrix3x4fv(GLint location, GLsizei count, GLboolean transpose, const GLfloat* value)</c></remarks>
        public static delegate* unmanaged<int, int, byte, float*, void> glUniformMatrix3x4fv;

        /// <summary>Gets the <c>glUniformMatrix4x3fv</c> delegate.</summary>
        /// <remarks><c>void glUniformMatrix4x3fv(GLint location, GLsizei count, GLboolean transpose, const GLfloat* value)</c></remarks>
        public static delegate* unmanaged<int, int, byte, float*, void> glUniformMatrix4x3fv;

        /// <summary>Gets the <c>glGetUniformLocation</c> delegate.</summary>
        /// <remarks><c>GLint glGetUniformLocation(GLuint program, const GLchar* name)</c></remarks>
        public static delegate* unmanaged<ProgramObject, byte*, int> glGetUniformLocation;

        /// <summary>Gets the <c>glGetUniformfv</c> delegate.</summary>
        /// <remarks><c>void glGetUniformfv(GLuint program, GLint location, GLfloat* params)</c></remarks>
        public static delegate* unmanaged<ProgramObject, int, float*, void> glGetUniformfv;

        /// <summary>Gets the <c>glGetUniformiv</c> delegate.</summary>
        /// <remarks><c>void glGetUniformiv(GLuint program, GLint location, GLint* params)</c></remarks>
        public static delegate* unmanaged<ProgramObject, int, int*, void> glGetUniformiv;

        /// <summary>Gets the <c>glGetUniformuiv</c> delegate.</summary>
        /// <remarks><c>void glGetUniformiv(GLuint program, GLint location, GLuint* params)</c></remarks>
        public static delegate* unmanaged<ProgramObject, int, uint*, void> glGetUniformuiv;

        /// <summary>Gets the <c>glDrawArrays</c> delegate.</summary>
        /// <remarks><c>void glDrawArrays(GLenum mode, GLint first, GLsizei count)</c></remarks>
        public static delegate* unmanaged<DrawArraysPrimitiveType, int, int, void> glDrawArrays;

        /// <summary>Gets the <c>glDrawArraysInstanced</c> delegate.</summary>
        /// <remarks><c>void glDrawArraysInstanced(GLenum mode, GLint first, GLsizei count, GLsizei instancecount)</c></remarks>
        public static delegate* unmanaged<DrawArraysPrimitiveType, int, int, int, void> glDrawArraysInstanced;

        /// <summary>Gets the <c>glDrawElements</c> delegate.</summary>
        /// <remarks><c>void glDrawElements(GLenum mode, GLsizei count, GLenum type, const void* indices)</c></remarks>
        public static delegate* unmanaged<DrawArraysPrimitiveType, int, DrawElementsDataType, void*, void> glDrawElements;

        /// <summary>Gets the <c>glDrawElementsInstanced</c> delegate.</summary>
        /// <remarks><c>void glDrawElementsInstanced(GLenum mode, GLsizei count, GLenum type, const void* indices, GLsizei instancecount)</c></remarks>
        public static delegate* unmanaged<DrawArraysPrimitiveType, int, DrawElementsDataType, void*, int, void> glDrawElementsInstanced;

        /// <summary>Gets the <c>glDrawElementsBaseVertex</c> delegate.</summary>
        /// <remarks><c>void glDrawElementsBaseVertex(GLenum mode, GLsizei count, GLenum type, void* indices, GLint basevertex)</c></remarks>
        public static delegate* unmanaged<DrawArraysPrimitiveType, int, DrawElementsDataType, void*, int, void> glDrawElementsBaseVertex;

        /// <summary>Gets the <c>glDrawElementsInstancedBaseVertex</c> delegate.</summary>
        /// <remarks><c>void glDrawElementsInstancedBaseVertex(GLenum mode, GLsizei count, GLenum type, void* indices, GLsizei instancecount, GLint basevertex)</c></remarks>
        public static delegate* unmanaged<DrawArraysPrimitiveType, int, DrawElementsDataType, void*, int, int, void> glDrawElementsInstancedBaseVertex;

        /// <summary>Gets the <c>glDrawRangeElements</c> delegate.</summary>
        /// <remarks><c>void glDrawRangeElements(GLenum mode, GLuint start, GLuint end, GLsizei count, GLenum type, const void* indices)</c></remarks>
        public static delegate* unmanaged<DrawArraysPrimitiveType, int, int, int, DrawElementsDataType, void*, void> glDrawRangeElements;

        /// <summary>Gets the <c>glDrawRangeElementsBaseVertex</c> delegate.</summary>
        /// <remarks><c>void glDrawRangeElementsBaseVertex(GLenum mode, GLuint start, GLuint end, GLsizei count, GLenum type, void* indices, GLint basevertex)</c></remarks>
        public static delegate* unmanaged<DrawArraysPrimitiveType, int, int, int, DrawElementsDataType, void*, int, void> glDrawRangeElementsBaseVertex;

        /// <summary>Gets the <c>glLineWidth</c> delegate.</summary>
        /// <remarks><c>void glLineWidth(GLfloat width)</c></remarks>
        public static delegate* unmanaged<float, void> glLineWidth;

        /// <summary>Gets the <c>glSampleCoverage</c> delegate.</summary>
        /// <remarks><c>void glSampleCoverage(GLfloat value, GLboolean invert)</c></remarks>
        public static delegate* unmanaged<float, byte, void> glSampleCoverage;

        /// <summary>Gets the <c>glPrimitiveRestartIndex</c> delegate.</summary>
        /// <remarks><c>void glPrimitiveRestartIndex(GLuint index)</c></remarks>
        public static delegate* unmanaged<uint, void> glPrimitiveRestartIndex;

        /// <summary>Gets the <c>glScissor</c> delegate.</summary>
        /// <remarks><c>void glScissor(GLint x, GLint y, GLsizei width, GLsizei height)</c></remarks>
        public static delegate* unmanaged<int, int, int, int, void> glScissor;

        /// <summary>Gets the <c>glEnable</c> delegate.</summary>
        /// <remarks><c>void glEnable(GLenum cap)</c></remarks>
        public static delegate* unmanaged<GLCapability, void> glEnable;

        /// <summary>Gets the <c>glEnablei</c> delegate.</summary>
        /// <remarks><c>void glEnablei(GLenum cap, GLuint index)</c></remarks>
        public static delegate* unmanaged<GLCapability, uint, void> glEnablei;

        /// <summary>Gets the <c>glDisable</c> delegate.</summary>
        /// <remarks><c>void glDisable(GLenum cap)</c></remarks>
        public static delegate* unmanaged<GLCapability, void> glDisable;

        /// <summary>Gets the <c>glDisablei</c> delegate.</summary>
        /// <remarks><c>void glDisablei(GLenum cap, GLuint index)</c></remarks>
        public static delegate* unmanaged<GLCapability, uint, void> glDisablei;

        /// <summary>Gets the <c>glGenVertexArrays</c> delegate.</summary>
        /// <remarks><c>void glGenVertexArrays(GLsizei n, GLuint* arrays)</c></remarks>
        public static delegate* unmanaged<int, VertexArrayObject*, void> glGenVertexArrays;

        /// <summary>Gets the <c>glDeleteVertexArrays</c> delegate.</summary>
        /// <remarks><c>void glDeleteVertexArrays(GLsizei n, const GLuint* arrays)</c></remarks>
        public static delegate* unmanaged<int, VertexArrayObject*, void> glDeleteVertexArrays;

        /// <summary>Gets the <c>glBindVertexArray</c> delegate.</summary>
        /// <remarks><c>void glBindVertexArray(GLuint array)</c></remarks>
        public static delegate* unmanaged<VertexArrayObject, void> glBindVertexArray;

        /// <summary>Gets the <c>glDeleteBuffers</c> delegate.</summary>
        /// <remarks><c>void glDeleteBuffers(GLsizei n, const GLuint* buffers)</c></remarks>
        public static delegate* unmanaged<int, BufferObject*, void> glDeleteBuffers;

        /// <summary>Gets the <c>glBlendFunc</c> delegate.</summary>
        /// <remarks><c>void glBlendFunc(GLenum sfactor, GLenum dfactor)</c></remarks>
        public static delegate* unmanaged<BlendFunctionFactor, BlendFunctionFactor, void> glBlendFunc;

        /// <summary>Gets the <c>glBlendColor</c> delegate.</summary>
        /// <remarks><c>void glBlendColor(GLfloat red, GLfloat green, GLfloat blue, GLfloat alpha)</c></remarks>
        public static delegate* unmanaged<float, float, float, float, void> glBlendColor;

        /// <summary>Gets the <c>glGetError</c> delegate.</summary>
        /// <remarks><c>GLenum glGetError(void)</c></remarks>
        public static delegate* unmanaged<ErrorCode> glGetError;

        /// <summary>Gets the <c>glGetString</c> delegate.</summary>
        /// <remarks><c>const GLubyte* glGetString(GLenum name)</c></remarks>
        public static delegate* unmanaged<GlGetStringType, byte*> glGetString;

        /// <summary>Gets the <c>glGetStringi</c> delegate.</summary>
        /// <remarks><c>const GLubyte* glGetStringi(GLenum name, GLuint index)</c></remarks>
        public static delegate* unmanaged<GlGetStringType, int, byte*> glGetStringi;

        #endregion

        #region Utility functions

        /// <summary>
        /// Asserts the error after calling an OpenGL function and you wish to check the result during application debugging. <br />
        /// On Release builds, this function is removed for performance.
        /// </summary>
        [Conditional("DEBUG")]
        public static void AssertError()
        {
            Exception ex = GetError();
            if (ex is not null) { throw ex; }
        }

        /// <summary>
        /// Asserts the error after calling an OpenGL function and you wish to check the result during application debugging. <br />
        /// On Release builds, this function is removed for performance. <br />
        /// Unlike <see cref="AssertError"/> that does throw directly, this function does instead wrap the exception into an <see cref="OpenGLException"/> instance.
        /// </summary>
        /// <param name="details">Additional details for the exception. If not provided, the function passes a generic message instead.</param>
        /// <exception cref="OpenGLException">The error that was occured and was wrapped as an exception of this type.</exception>
        [Conditional("DEBUG")]
        public static void AssertAsOpenGLException(String details = null)
        {
            Exception ex = GetError();
            if (ex is not null) { throw new OpenGLException(details ?? "An unexpected error has been occured.", ex); }
        }

        /// <summary>
        /// Gets an <see cref="Exception"/> for the current OpenGL error after calling an OpenGL function and you wish to check the result during application debugging.
        /// </summary>
        /// <returns>An <see cref="Exception"/> describing the current error or <see langword="null"/> if no error does exist.</returns>
        public static Exception GetError() => glGetError() switch
        {
            ErrorCode.GL_INVALID_ENUM => new ArgumentException("An unacceptable value is specified for an enumerated argument."),
            ErrorCode.GL_INVALID_OPERATION => new InvalidOperationException("The specified operation is not allowed in the current state."),
            ErrorCode.GL_INVALID_VALUE => new ArgumentOutOfRangeException("A numeric argument is out of range.", innerException: null),
            ErrorCode.GL_INVALID_FRAMEBUFFER_OPERATION => new InvalidOperationException("The framebuffer object is not complete."),
            ErrorCode.GL_OUT_OF_MEMORY => new OutOfMemoryException("There is not enough memory left to execute the command."),
            _ => null,
        };

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

        /// <summary>
        /// Uses the <see cref="glGenVertexArrays"/> to generate a single vertex array object instead.
        /// </summary>
        /// <returns>The created vertex array object.</returns>
        public static VertexArrayObject GLGenSingleVertexArray()
        {
            VertexArrayObject ret;
            glGenVertexArrays(1 , &ret);
            return ret;
        }

        /// <summary>
        /// Uses the <see cref="glDeleteVertexArrays"/> to delete a single vertex array object instead.
        /// </summary>
        /// <param name="obj">The previously created array object to be deleted.</param>
        public static void GLDeleteVertexArray(VertexArrayObject obj) => glDeleteVertexArrays(1, &obj);

        /// <summary>
        /// Defines a .NET type-safe equivalent of <see cref="glGenVertexArrays"/> function.
        /// </summary>
        /// <param name="count">The number of vertex array objects to generate.</param>
        /// <returns>The generated array objects.</returns>
        public static VertexArrayObject[] GLGenVertexArrays(int count)
        {
            var objects = new VertexArrayObject[count];
            fixed (VertexArrayObject* ptr = objects) { 
                glGenVertexArrays(count, ptr);
            }
            return objects;
        }

        /// <summary>
        /// Defines a .NET type-safe equivalent of <see cref="glDeleteVertexArrays"/> function.
        /// </summary>
        /// <param name="objects">The vertex array objects to delete.</param>
        public static void GLDeleteVertexArrays(params VertexArrayObject[] objects)
        {
            fixed (VertexArrayObject* ptr = objects) {
                glDeleteVertexArrays(objects.Length, ptr);
            }
        }

        /// <summary>
        /// Uses the <see cref="glGenBuffers"/> to generate a single buffer object instead.
        /// </summary>
        /// <returns>The created buffer object.</returns>
        public static BufferObject GLGenSingleBufferObject()
        {
            BufferObject ret;
            glGenBuffers(1 , &ret);
            return ret;
        }

        /// <summary>
        /// Uses the <see cref="glDeleteBuffers"/> to delete a single buffer object instead.
        /// </summary>
        /// <param name="obj">The previously created buffer object to delete.</param>
        public static void GLDeleteBufferObject(BufferObject obj) => glDeleteBuffers(1, &obj);

        /// <summary>
        /// Defines a .NET type-safe equivalent of <see cref="glGenBuffers"/> function.
        /// </summary>
        /// <param name="count">The number of buffer objects to generate.</param>
        /// <returns>The generated buffer objects.</returns>
        public static BufferObject[] GLGenBufferObjects(int count)
        {
            BufferObject[] objects = new BufferObject[count];
            fixed (BufferObject* ptr = objects) { 
                glGenBuffers(count, ptr); 
            }
            return objects;
        }

        /// <summary>
        /// Defines a .NET type-safe equivalent of <see cref="glDeleteBuffers"/> function.
        /// </summary>
        /// <param name="objects">The buffer objects to delete.</param>
        public static void GLDeleteBufferObjects(params BufferObject[] objects)
        {
            fixed (BufferObject* ptr = objects) {
                glDeleteBuffers(objects.Length, ptr);
            }
        }

        /// <summary>
        /// Equivalent to the <see cref="glGetShaderiv"/> function, but it returns the value instead of getting it through pointers.
        /// </summary>
        /// <param name="shader">The shader object to query.</param>
        /// <param name="p">Specifies the object parameter. See the <see cref="ShaderParameter"/> enumeration for valid values.</param>
        /// <returns>The value of <paramref name="p"/>.</returns>
        public static int GLGetShaderIV(ShaderObject shader , ShaderParameter p)
        {
            int v;
            glGetShaderiv(shader, p, &v);
            return v;
        }

        /// <summary>
        /// Equivalent to the <see cref="glGetProgramiv"/> function, but it returns the value instead of getting it through pointers.
        /// </summary>
        /// <param name="program">The program object to query.</param>
        /// <param name="pname">Specifies the object parameter. See the <see cref="ProgramParameter"/> enumeration for valid values.</param>
        /// <returns>The value of <paramref name="pname"/>.</returns>
        public static int GLGetProgramIV(ProgramObject program, ProgramParameter pname) 
        {
            int v;
            glGetProgramiv(program, pname, &v);
            return v;
        }

        /// <summary>
        /// Equivalent to the <see cref="glVertexAttribPointer"/> function, but it is defined in safe code.
        /// </summary>
        /// <param name="index">The index of the attribute to be defined.</param>
        /// <param name="size">The number of elements this attribute describes.</param>
        /// <param name="datatype">The underlying data type of the elements.</param>
        /// <param name="normalized">A value whether floating-point data should be normalized.</param>
        /// <param name="stride">The exact size, in bytes, of the attribute data payload.</param>
        /// <param name="offset">The additional byte offset to apply from the beginning.</param>
        public static void GLVertexAttribPointer(System.UInt32 index, System.Int32 size, VertexAttributePointerDataType datatype, System.Boolean normalized , int stride , int offset)
            => glVertexAttribPointer(index, size, datatype, normalized ? GLConstants.GL_TRUE : GLConstants.GL_FALSE, offset, (void*)stride);

        #endregion

    }
}