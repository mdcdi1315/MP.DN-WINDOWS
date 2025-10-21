
using System;
using MP.Graphics.Imaging;
using System.Collections.Generic;
using MP.Graphics.Windowing.Graphics;


#pragma warning disable 1591 // Still in very preliminary stage

namespace MP.Graphics.OpenGL.GraphicsContext
{
    public unsafe sealed class OpenGLGraphicsContext : IWindowGuiGraphicsContext
    {
        private FloatColor colorcurrent;
        private GraphicsObjectID lastgeneratedid;

        private Dictionary<GraphicsObjectID, Line> lines;
        private Dictionary<GraphicsObjectID, Rectangle> rects;
        private Dictionary<GraphicsObjectID, Triangle> triangles;
        private Dictionary<GraphicsObjectID, TextureData> textures;
        
        private GraphicsObjectID GenerateIDAndCreateObject<T>(IDictionary<GraphicsObjectID, T> dict , T data)
        {
            GraphicsObjectID idnew = new(lastgeneratedid.ID + 1);
            while (dict.TryGetValue(idnew , out _)) { idnew = new(lastgeneratedid.ID + 1); }
            dict.Add(idnew, data);
            return idnew;
        }

        private struct TextureData
        {
            public IImage Texture;
            public Size Texture_Area;
        }

        public OpenGLGraphicsContext()
        {
            colorcurrent = Colors.Black.ToFloat();
            lastgeneratedid = new GraphicsObjectID(0);
            rects = new Dictionary<GraphicsObjectID, Rectangle>();
            lines = new Dictionary<GraphicsObjectID, Line>();
            triangles = new Dictionary<GraphicsObjectID, Triangle>();
            textures = new Dictionary<GraphicsObjectID, TextureData>();
        }

        public Size Size => new(600 , 1200);

        public void Clear(IColor color)
        {
            ArgumentNullException.ThrowIfNull(color);
            colorcurrent = color.ToFloat();
        }

        public GraphicsObjectID CreateLine(Line line)
        {
            throw new System.NotImplementedException();
        }

        public GraphicsObjectID CreateRectangle(Rectangle rectangle)
        {
            throw new System.NotImplementedException();
        }

        public GraphicsObjectID CreateTexture(IImage image, Rectangle texture_area)
        {
            throw new System.NotImplementedException();
        }

        public GraphicsObjectID CreateTriangle(Triangle triangle)
        {
            throw new System.NotImplementedException();
        }

        public bool DestroyObject(GraphicsObjectID o)
        {
            throw new System.NotImplementedException();
        }

        

        public void DrawFrame()
        {
            GL.glClear(BufferBits.GL_COLOR_BUFFER_BIT);
            GL.glClearColor(colorcurrent.Red , colorcurrent.Green , colorcurrent.Blue , colorcurrent.Alpha);


        }

        public void Flush() { }

        public bool UpdateLine(GraphicsObjectID o, Line line)
        {
            throw new System.NotImplementedException();
        }

        public bool UpdateRectangle(GraphicsObjectID o, Rectangle newrectangle)
        {
            throw new System.NotImplementedException();
        }

        public bool UpdateTextureBounds(GraphicsObjectID o, Rectangle texture_area)
        {
            throw new System.NotImplementedException();
        }

        public bool UpdateTriangle(GraphicsObjectID o, Triangle newtriangle)
        {
            throw new System.NotImplementedException();
        }

        public void Dispose()
        {
            throw new System.NotImplementedException();
        }
    }
}