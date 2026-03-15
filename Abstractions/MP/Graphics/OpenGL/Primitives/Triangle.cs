


namespace MP.Graphics.OpenGL.Primitives
{
    /// <summary>
    /// Defines a triangle shape.
    /// </summary>
    public unsafe sealed class Triangle : BaseOpenGLPrimitiveShape
    {
        private PointF second , third;
        private VertexArrayObject vertexarray;
        private BufferObject vertexbufferobject;

        /// <summary>
        /// Creates a new triangle. <br />
        /// The vertex shader must accept a vec2 describing a 2D triangle coordinate,
        /// and the fragment shader must accept a vec4 uniform named OutColor.
        /// </summary>
        /// <param name="vertex">The vertex shader to define.</param>
        /// <param name="fragment">The fragment shader to define.</param>
        public Triangle(OpenGLShader vertex, OpenGLShader fragment) 
            : base(vertex, fragment , "OutColor")
        {
            // Configure required objects to work with

            vertexarray = GL.GLGenSingleVertexArray();
            vertexbufferobject = GL.GLGenSingleBufferObject();

            GL.glBindVertexArray(vertexarray);

            GLUtils.GLBufferData(vertexbufferobject, BufferObjectType.GL_ARRAY_BUFFER, DataStoreUsagePattern.GL_DYNAMIC_DRAW, new PointF[] {
                default,
                default, // Specify empty data (The OnUpdate call will perpetually happen to update our data before sending them to OpenGL)
                default
            });
            GL.GLVertexAttribPointer(0, 3, VertexAttributePointerDataType.GL_FLOAT, false, 3 * sizeof(PointF), 0);
            GL.glEnableVertexAttribArray(0);


            // Clean the environment
            GL.glBindVertexArray(VertexArrayObject.Empty);
            GL.glBindBuffer(BufferObjectType.GL_ARRAY_BUFFER, BufferObject.Empty);
        }

        /// <summary>
        /// Gets or sets the second point of the triangle. <br />
        /// The first one can be got and set by using the <see cref="BaseOpenGLPrimitiveShape.Location"/> property.
        /// </summary>
        public PointF Second
        {
            get => second; 
            set {
                second = value;
                Update();
            }
        }

        /// <summary>
        /// Gets or sets the third point of the triangle. <br />
        /// The first one can be got and set by using the <see cref="BaseOpenGLPrimitiveShape.Location"/> property.
        /// </summary>
        public PointF Third
        {
            get => third;
            set { 
                third = value;
                Update();
            }
        }

        /// <summary>
        /// Defines the code that draws the triangle.
        /// </summary>
        protected override void DrawingCode()
        {
            GL.glBindVertexArray(vertexarray);
            GL.glDrawArrays(DrawArraysPrimitiveType.GL_TRIANGLES, 0, 3);
            GL.glBindVertexArray(VertexArrayObject.Empty);
        }

        /// <summary>
        /// Updates the triangle shape, as well as it's color.
        /// </summary>
        protected override void OnUpdate()
        {
            GL.glBindVertexArray(vertexarray);

            GLUtils.GLBufferSubData(vertexbufferobject, BufferObjectType.GL_ARRAY_BUFFER, new PointF[] {
                Location,
                second,
                third
            });

            // Unbind buffers
            GL.glBindVertexArray(VertexArrayObject.Empty);
            GL.glBindBuffer(BufferObjectType.GL_ARRAY_BUFFER, BufferObject.Empty);

            // Set the new color
            FloatColor fc = Color.ToFloat();
            Program.SetUniform("OutColor" , fc.Red , fc.Green , fc.Blue , fc.Alpha);

            // Done!
        }

        /// <summary>
        /// Disposes of the current triangle.
        /// </summary>
        /// <param name="disposing"></param>
        protected override void Dispose(bool disposing)
        {
            GL.GLDeleteBufferObject(vertexbufferobject);
            GL.GLDeleteVertexArray(vertexarray);
        }
    }
}