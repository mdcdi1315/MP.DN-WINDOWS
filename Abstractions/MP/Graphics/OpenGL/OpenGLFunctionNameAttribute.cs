

using System;

namespace MP.Graphics.OpenGL
{
    /// <summary>
    /// Internal attribute class that describes the actual function name of an OpenGL function pointer delegate.
    /// </summary>
    [AttributeUsage(AttributeTargets.Delegate , AllowMultiple = false , Inherited = false)]
    internal sealed class OpenGLFunctionNameAttribute : Attribute
    {
        private System.String fname;

        public OpenGLFunctionNameAttribute(System.String name) => fname = name;

        public System.String GetName() => fname;
    }
}