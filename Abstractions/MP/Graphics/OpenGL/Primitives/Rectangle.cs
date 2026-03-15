


namespace MP.Graphics.OpenGL.Primitives
{
    /// <summary>
    /// Defines the rectangle primitive.
    /// </summary>
    public unsafe sealed class Rectangle : BaseOpenGLPrimitiveShape
    {
        private VertexArrayObject vertexarray;
        private BufferObject vertexbufferobject , elementbufferobject;

        /// <summary>
        /// Creates a new instance of the <see cref="Rectangle"/> class. <br />
        /// The vertex shader must accept a vec2 describing a 2D rectangle coordinate,
        /// and the fragment shader must accept a vec4 uniform named OutColor.
        /// </summary>
        /// <param name="vertex">The vertex shader to define.</param>
        /// <param name="fragment">The fragment shader to define.</param>
        public Rectangle(OpenGLShader vertex, OpenGLShader fragment) : base(vertex, fragment , "OutColor")
        {
            vertexarray = GL.GLGenSingleVertexArray();
            vertexbufferobject = GL.GLGenSingleBufferObject();
            elementbufferobject = GL.GLGenSingleBufferObject();

            GL.glBindVertexArray(vertexarray);

            GLUtils.GLBufferData(vertexbufferobject, BufferObjectType.GL_ARRAY_BUFFER, DataStoreUsagePattern.GL_DYNAMIC_DRAW, new PointF[4]);

            System.UInt32[] indices = new System.UInt32[] {
                0 , 1 , 3,
                1,  2 , 3
            };

            GLUtils.GLBufferData(elementbufferobject, BufferObjectType.GL_ELEMENT_ARRAY_BUFFER, DataStoreUsagePattern.GL_STATIC_DRAW, indices);

            GL.GLVertexAttribPointer(0, 4, VertexAttributePointerDataType.GL_FLOAT, false, 4 * sizeof(PointF), 0);
            GL.glEnableVertexAttribArray(0);

            // Unbind everything
            GL.glBindVertexArray(VertexArrayObject.Empty);
            GL.glBindBuffer(BufferObjectType.GL_ARRAY_BUFFER , BufferObject.Empty);
            GL.glBindBuffer(BufferObjectType.GL_ELEMENT_ARRAY_BUFFER, BufferObject.Empty);
        }

        /// <summary>
        /// Disposes this <see cref="Rectangle"/> instance.
        /// </summary>
        /// <param name="disposing"></param>
        protected override void Dispose(bool disposing)
        {
            GL.GLDeleteBufferObjects(vertexbufferobject, elementbufferobject);
            GL.GLDeleteVertexArray(vertexarray);
        }

        /// <summary>
        /// Specifies the code to draw the rectangle.
        /// </summary>
        protected override void DrawingCode()
        {
            GL.glBindVertexArray(vertexarray);
            // The vertex arrays store the selected EBO's so this binding is not needed.
            // GL.glBindBuffer(BufferObjectType.GL_ELEMENT_ARRAY_BUFFER, elementbufferobject);
            GL.glDrawElements(DrawArraysPrimitiveType.GL_TRIANGLES, 6, DrawElementsDataType.GL_UNSIGNED_INT, null);
            GL.glBindVertexArray(VertexArrayObject.Empty);
        }

        /// <summary>
        /// Defines the code to run for updating the details of the rectangle.
        /// </summary>
        protected override void OnUpdate()
        {
            GL.glBindVertexArray(vertexarray);

            RectangleF rf = new(Location, Size);

            GLUtils.GLBufferSubData(vertexbufferobject, BufferObjectType.GL_ARRAY_BUFFER, new PointF[] {
                rf.TopRight,
                rf.BottomRight,
                rf.BottomLeft,
                rf.TopLeft,
            });

            // TODO: Check whether GLVertexAttribPointer is unecessary
            // and just updating the point data buffer is enough for our use case.

            GL.GLVertexAttribPointer(0, 4, VertexAttributePointerDataType.GL_FLOAT, false, 4 * sizeof(PointF), 0);
            GL.glEnableVertexAttribArray(0);

            // Unbind everything
            GL.glBindVertexArray(VertexArrayObject.Empty);
            GL.glBindBuffer(BufferObjectType.GL_ARRAY_BUFFER, BufferObject.Empty);

            FloatColor fc = Color.ToFloat();
            Program.SetUniform("OutColor" , fc.Red , fc.Green , fc.Blue , fc.Alpha);
        }
    }
}