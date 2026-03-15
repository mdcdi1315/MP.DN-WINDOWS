namespace MP.Graphics.Windowing
{
    /// <summary>
    /// Creation parameters for a GLFW window.
    /// </summary>
    public sealed class GLFWWindowCreationParameters
    {
        public int MajorContextVersion;
        public int MinorContextVersion;

        public GLFWBackendAPI API;

        public bool EnableContextDebugging;

        public bool EnableForwardCompatibility;

        public GLFWOpenGLProfile OpenGLProfile;

        public Size Size; // The size of the window.
    }

    public enum GLFWBackendAPI : System.Byte
    {
        None,
        OpenGL,
        OpenGL_ES
    }

    public enum GLFWOpenGLProfile : System.Byte
    {
        None,
        OpenGLCore,
        OpenGLCompatibility
    }
}