

using MP.NativeInterop;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace MP.Graphics.OpenGL
{
    /// <summary>
    /// Thrown when shader compilation errors are found.
    /// </summary>
    public sealed class OpenGLShaderCompilationException : OpenGLException
    {
        /// <summary>
        /// Creates a new instance of the <see cref="OpenGLShaderCompilationException"/> 
        /// class with the specified detailed error message that is the cause of this exception.
        /// </summary>
        /// <param name="message">The error message to show.</param>
        public OpenGLShaderCompilationException(System.String message) : base(message) { }

        /// <summary>
        /// Throws an <see cref="OpenGLShaderCompilationException"/> from a specified shader, if an error is found. <br />
        /// Note: This method call is removed on release builds for performance.
        /// </summary>
        /// <param name="shader">The shader object to test.</param>
        [Conditional("DEBUG")]
        public static void ThrowIfFailed(ShaderObject shader)
        {
            if (GL.GLGetShaderIV(shader, ShaderParameter.GL_COMPILE_STATUS) == GLConstants.GL_FALSE)
            {
                ThrowCode(shader);
            }
        }

        // We cannot optimize the method. The best is to avoid optimizing it because it is too long.
        // Only performance headaches will this cause.
        [StackTraceHidden]
        [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
        private unsafe static void ThrowCode(ShaderObject shader)
        {
            int length = GL.GLGetShaderIV(shader, ShaderParameter.GL_INFO_LOG_LENGTH);
            if (length == 0) {
                throw new OpenGLShaderCompilationException("Shader compilation failed for unknown reasons");
            } else {
                IMemoryHandle mem = SystemInfo.GetDefaultMemoryManager().AllocateMemoryHandle(length.ToUInt64());
                System.String temp;
                try {
                    int leninternal;
                    GL.glGetShaderInfoLog(shader, length, &leninternal , mem.MemoryPointer);
                    temp = new((System.SByte*)mem.MemoryPointer, 0 ,leninternal);
                } finally {
                    mem.Dispose();
                }
                throw new OpenGLShaderCompilationException(temp);
            }
        }
    }
}