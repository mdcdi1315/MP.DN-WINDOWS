
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace MP.Graphics.OpenGL
{
    /// <summary>
    /// Defines the exception that is thrown when an OpenGL linkage error occurs.
    /// </summary>
    public sealed class OpenGLProgramLinkageException : OpenGLException
    {
        /// <summary>
        /// Creates a new instance of the <see cref="OpenGLProgramLinkageException"/> 
        /// class with the specified detailed error message that is the cause of this exception.
        /// </summary>
        /// <param name="message">The error message to show.</param>
        public OpenGLProgramLinkageException(string message) : base(message) { }

        /// <summary>
        /// Throws an <see cref="OpenGLProgramLinkageException"/> from a specified program, if an error is found. <br />
        /// Note: This method call is removed on release builds for performance.
        /// </summary>
        /// <param name="program">The program object to test.</param>
        [Conditional("DEBUG")]
        public static void ThrowIfFailed(ProgramObject program)
        {
            if (GL.GLGetProgramIV(program, ProgramParameter.GL_LINK_STATUS) == GLConstants.GL_FALSE)
            {
                ThrowCode(program);
            }
        }

        // We cannot optimize the method. The best is to avoid optimizing it because it is too long.
        // Only performance headaches will this cause.
        [StackTraceHidden]
        [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
        private unsafe static void ThrowCode(ProgramObject shader)
        {
            int length = GL.GLGetProgramIV(shader, ProgramParameter.GL_INFO_LOG_LENGTH);
            if (length == 0) {
                throw new OpenGLProgramLinkageException("Program linkage failed for unknown reasons");
            } else {
                IMemoryHandle mem = SystemInfo.GetMemoryHandleFactory().CreateMemoryHandle(length);
                System.String temp;
                try {
                    int leninternal;
                    GL.glGetProgramInfoLog(shader, length, &leninternal, mem.MemoryPointer);
                    temp = new((System.SByte*)mem.MemoryPointer, 0, leninternal);
                } finally {
                    mem.Dispose();
                }
                throw new OpenGLProgramLinkageException(temp);
            }
        }

    }
}