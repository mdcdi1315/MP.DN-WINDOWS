
using System;
using MP.Annotations.CodeAnalysis;

namespace MP.Graphics.OpenGL.Primitives
{
    /// <summary>
    /// Defines a convenience abstract class for programming OpenGL primitive shapes.
    /// </summary>
    public abstract class BaseOpenGLPrimitiveShape : IOpenGLPrimitiveShape
    {
        private SizeF size;
        private PointF point;
        private IColor color;
        private volatile bool update;
        private OpenGLProgram program;

        private BaseOpenGLPrimitiveShape()
        {
            color = Colors.White;
            update = true;
            program = null;
        }

        /// <summary>
        /// Creates a new instance of the <see cref="BaseOpenGLPrimitiveShape"/> by attaching the specified compiled shaders. <br />
        /// Before the constructor exits, the program will be linked. <br />
        /// This must be called from the render thread.
        /// </summary>
        /// <param name="vertex">The vertex shader to specify.</param>
        /// <param name="fragment">The fragment shader to specify.</param>
        protected BaseOpenGLPrimitiveShape(OpenGLShader vertex , OpenGLShader fragment) : this()
        {
            program = new();
            program.AttachShader(vertex);
            program.AttachShader(fragment);
            program.Link();
        }

        /// <summary>
        /// Creates a new instance of the <see cref="BaseOpenGLPrimitiveShape"/> by attaching the specified compiled shaders. <br />
        /// Before the constructor exits, the program will be linked. <br />
        /// This must be called from the render thread.
        /// </summary>
        /// <param name="vertex">The vertex shader to specify.</param>
        /// <param name="fragment">The fragment shader to specify.</param>
        /// <param name="uniforms">An array of uniform variable names that your primitive is expected to use.</param>
        protected BaseOpenGLPrimitiveShape(OpenGLShader vertex, OpenGLShader fragment, params System.String[] uniforms) : this()
        {
            program = new(uniformnames: uniforms);
            program.AttachShader(vertex);
            program.AttachShader(fragment);
            program.Link();
        }

        /// <summary>
        /// Gains access to the actual OpenGL program for getting/setting uniforms or updating the program itself.
        /// </summary>
        protected OpenGLProgram Program => program;

        /// <summary>
        /// Gets or sets the size of the primitive.
        /// </summary>
        public virtual SizeF Size 
        { 
            get => size; 
            set {
                size = value;
                update = true;
            }
        }
        
        /// <summary>
        /// Gets or sets the location of the primitive where it will be drawn.
        /// </summary>
        public virtual PointF Location 
        { 
            get => point;
            set {
                point = value;
                update = true;
            } 
        }
        
        /// <summary>
        /// Gets or sets the drawing color of the primitive, if applicable.
        /// </summary>
        public virtual IColor Color 
        { 
            get => color; 
            set {
                color = value;
                update = true;
            }
        }

        /// <summary>
        /// Updates any data that are invalidated and they do need new drawing. <br />
        /// Thread-safe.
        /// </summary>
        public void Update() => update = true;

        /// <summary>
        /// Uses this primitive in this rendering pass.
        /// </summary>
        [ThrowsOnlyWhen("DEBUG" , typeof(ObjectDisposedException))]
        public void Use()
        {
#if DEBUG
            ObjectDisposedException.ThrowIf(program is null, this);
#endif
            program.Use();
            if (update) { 
                // Call the updater method
                OnUpdate(); 
                // Clear the flag
                update = false; 
            }
            // Make necessary draw calls
            DrawingCode();
        }

        /// <summary>
        /// Defines the code that actually draws the primitive.
        /// </summary>
        protected abstract void DrawingCode();

        /// <summary>
        /// This method is called on whenever one of the common properties 
        /// (<see cref="Size"/>, <see cref="Location"/> and <see cref="Color"/> have been changed) or 
        /// the <see cref="Update"/> method was explicitly called. 
        /// </summary>
        protected virtual void OnUpdate() { }

        /// <summary>
        /// Disposes the resources used by the current OpenGL primitive. 
        /// </summary>
        /// <param name="disposing">Do not use.</param>
        protected virtual void Dispose(bool disposing) { }

        /// <summary>
        /// Disposes this <see cref="BaseOpenGLPrimitiveShape"/> class instance. <br />
        /// This call must be performed on the OpenGL thread.
        /// </summary>
        public void Dispose()
        {
            try {
                program?.Dispose();
                program = null;
                Dispose(disposing: true);
            } finally {
                System.GC.SuppressFinalize(this);
            }
        }
    }
}