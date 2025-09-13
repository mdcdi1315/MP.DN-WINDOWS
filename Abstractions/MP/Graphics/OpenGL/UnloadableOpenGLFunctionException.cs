namespace MP.Graphics.OpenGL
{
    /// <summary>
    /// Thrown when a function requested through the <see cref="IOpenGLFunctionLoader.GetFunction"/> method was not found.
    /// </summary>
    public sealed class UnloadableOpenGLFunctionException : OpenGLException
    {
        private System.String name;

        /// <summary>
        /// Creates a new instance of the <see cref="UnloadableOpenGLFunctionException"/> class, specifying the name of the function that was not found.
        /// </summary>
        /// <param name="funcname">The name of the function that was not found.</param>
        public UnloadableOpenGLFunctionException(System.String funcname) => name = funcname;

        /// <summary>
        /// Gets the name of the function that was not found.
        /// </summary>
        public System.String Name => name;

        /// <inheritdoc />
        public override System.String Message => $"The requested OpenGL function was not found.\nFunction name: {name}";
    }
}
