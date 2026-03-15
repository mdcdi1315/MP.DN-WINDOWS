
using System;
using MP.Annotations.CodeAnalysis;

namespace MP.Graphics.OpenGL
{
    /// <summary>
    /// Defines the object responsible for loading all the OpenGL functions.
    /// </summary>
    public unsafe interface IOpenGLFunctionLoader
    {
        /// <summary>
        /// Gets a function to be loaded from the current OpenGL context.
        /// </summary>
        /// <returns>The requested OpenGL function pointer. Must be cast later appropriately.</returns>
        /// <exception cref="UnloadableOpenGLFunctionException">The specified function could not be found.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="function"/> was <see langword="null"/>.</exception>
        [Throws(
            typeof(ArgumentNullException),
            typeof(UnloadableOpenGLFunctionException)
        )]
        public void* GetFunction(String function);
    }
}
