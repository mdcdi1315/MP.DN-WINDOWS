namespace MP.ExceptionSystem
{
    public sealed class GLFWException : BaseException, INativeException
    {
        public GLFWException(string message) : base(message) { }
    }
}