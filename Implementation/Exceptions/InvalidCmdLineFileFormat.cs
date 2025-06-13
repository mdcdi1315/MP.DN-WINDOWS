

namespace MP.ExceptionSystem
{
    public sealed class InvalidCmdLineFormatException : BaseException
    {
        public InvalidCmdLineFormatException() { }

        public InvalidCmdLineFormatException(string message) : base(message) { }

        public InvalidCmdLineFormatException(System.String message, System.Exception innerException) : base(message, innerException) { }
    }
}