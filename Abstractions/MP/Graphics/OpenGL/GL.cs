
using System;
using System.Reflection;
using MP.Annotations;
using MP.Annotations.CodeAnalysis;

namespace MP.Graphics.OpenGL
{
    /// <summary>
    /// Defines the currently used OpenGL core profile functions,
    /// and a convenient handling for initializing all the functions and re-destroying them.
    /// </summary>
    [Preliminary]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "CA2211:Non-constant fields should not be visible", Justification = "OpenGL Layer API")]
    public static class GL
    {
        /// <summary>
        /// Loads all the currently defined OpenGL functions, by using the specified function loader.
        /// </summary>
        /// <remarks>
        /// While it is not required by the interface itself, this function tests after every function is loaded or failure whether the loader also implements the <see cref="IDisposable"/> interface.
        /// If it does so, it calls that <see cref="IDisposable.Dispose"/> method implementation.
        /// </remarks>
        /// <param name="loader">The function loader to use.</param>
        [Throws(typeof(ArgumentNullException))]
        public static void LoadFunctions(IOpenGLFunctionLoader loader)
        {
            ArgumentNullException.ThrowIfNull(loader);
            try {
                Type delegatetype;
                OpenGLFunctionNameAttribute a;
                foreach (var dgf in typeof(GL).GetFields(BindingFlags.Static | BindingFlags.Public))
                {
                    delegatetype = dgf.FieldType;
                    System.String name;
                    a = delegatetype.GetCustomAttribute<OpenGLFunctionNameAttribute>();
                    if (a is null) {
                        name = dgf.Name;
                    } else {
                        name = a.GetName();
                    }
                    dgf.SetValue(null, loader.GetFunction(name , delegatetype));
                }
            } finally {
                if (loader is IDisposable d) { d.Dispose(); }
            }
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

        /// <summary>
        /// Gets the <c>glAttachShader</c> delegate.
        /// </summary>
        public static GLATTACHSHADER glAttachShader;

        /// <summary>
        /// Gets the <c>glBindBuffer</c> delegate.
        /// </summary>
        public static GLBINDBUFFER glBindBuffer;

        /// <summary>
        /// Gets the <c>glViewport</c> delegate.
        /// </summary>
        public static GLVIEWPORT glViewport;

        /// <summary>
        /// Gets the <c>glBufferData</c> delegate.
        /// </summary>
        public static GLBUFFERDATA glBufferData;

        /// <summary>
        /// Gets the <c>glClear</c> delegate.
        /// </summary>
        public static GLCLEAR glClear;

        /// <summary>
        /// Gets the <c>glClearColor</c> delegate.
        /// </summary>
        public static GLCLEARCOLOR glClearColor;

        /// <summary>
        /// Gets the <c>glCreateProgram</c> delegate.
        /// </summary>
        public static GLCREATEPROGRAM glCreateProgram;

        /// <summary>
        /// Gets the <c>glGetShaderInfoLog</c> delegate.
        /// </summary>
        public static GLGETSHADERINFOLOG glGetShaderInfoLog;

        /// <summary>
        /// Gets the <c>glCreateShader</c> delegate.
        /// </summary>
        public static GLCREATESHADER glCreateShader;

        /// <summary>
        /// Gets the <c>glGenBuffers</c> delegate.
        /// </summary>
        public static GLGENBUFFERS glGenBuffers;

        /// <summary>
        /// Gets the <c>glShaderCompile</c> delegate.
        /// </summary>
        public static GLSHADERCOMPILE glShaderCompile;

        /// <summary>
        /// Gets the <c>glShaderSource</c> delegate.
        /// </summary>
        public static GLSHADERSOURCE glShaderSource;
    }
}