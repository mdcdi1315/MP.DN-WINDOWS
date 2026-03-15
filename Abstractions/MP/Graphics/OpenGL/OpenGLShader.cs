
using System;
using MP.Annotations;
using MP.NativeInterop;
using MP.Annotations.CodeAnalysis;

namespace MP.Graphics.OpenGL
{
    /// <summary>
    /// Defines an OpenGL shader.
    /// </summary>
    public unsafe sealed class OpenGLShader : IDisposable
    {
        private readonly ShaderObject shader;

        /// <summary>
        /// Creates a new and empty OpenGL shader of the specified type.
        /// </summary>
        /// <param name="type">The type of the shader to create.</param>
        /// <exception cref="OpenGLException">Debug builds only: The shader could not be created.</exception>
        [ThrowsOnlyWhen("DEBUG", typeof(OpenGLException))]
        public OpenGLShader(ShaderType type)
        {
            shader = GL.glCreateShader(type);
            GL.AssertAsOpenGLException("Cannot create the specified shader object.");
        }

        /// <summary>
        /// Provides shader source from the specified stream. <br />
        /// The stream is read to completion.
        /// </summary>
        /// <param name="stream">The stream to read shader data from.</param>
        /// <exception cref="ArgumentNullException"><paramref name="stream"/> was <see langword="null"/>.</exception>
        /// <exception cref="ArgumentException"><paramref name="stream"/> was unreadable.</exception>
        [RequiresNativeLayer]
        [Throws(typeof(ArgumentNullException), typeof(ArgumentException))]
        public void ProvideSourceFromStream(IO.IDataStreamAccess stream)
        {
            ArgumentNullException.ThrowIfNull(stream);
            IO.InvalidDataStreamException.ThrowIfUnreadable(stream);

            INativeMemoryManager mgr = SystemInfo.GetDefaultMemoryManager();

            int size = 1024;

            byte* p = (byte*)mgr.Allocate(size);
            try {
                while (stream.Read(new Span<byte>(p, 1024)) > 0)
                {
                    size += 1024;
                    p = (byte*)mgr.ReAllocate(p, size);
                }

                GL.glShaderSource(shader, 1, &p, &size);
            } finally {
                mgr.Free(p);
            }
        }

        /// <summary>
        /// Provides shader source from the specified strings.
        /// </summary>
        /// <param name="lines">The strings that make up the shader's source.</param>
        [RequiresNativeLayer]
        public void ProvideSourceFromLines(params System.String[] lines) => GLUtils.GLShaderSource(shader, lines);

        /// <summary>
        /// Compiles the current shader. <br />
        /// If on a debug build and the shader has failed compilation a proper <see cref="OpenGLShaderCompilationException"/> is thrown.
        /// </summary>
        /// <exception cref="OpenGLShaderCompilationException">Shader compilation failed (Debug builds only).</exception>
        [ThrowsOnlyWhen("DEBUG" , typeof(OpenGLShaderCompilationException))]
        public void Compile()
        {
            GL.glCompileShader(shader);
            OpenGLShaderCompilationException.ThrowIfFailed(shader);
        }

        /// <summary>
        /// Gets the actual handle to the shader. <br />
        /// Use it to pass it directly to OpenGL API's.
        /// </summary>
        public ShaderObject Handle => shader;

        /// <summary>
        /// Disposes the current <see cref="OpenGLShader"/> instance.
        /// </summary>
        public void Dispose()
        {
            GL.glDeleteShader(shader);
            GL.AssertError();
        }
    }
}