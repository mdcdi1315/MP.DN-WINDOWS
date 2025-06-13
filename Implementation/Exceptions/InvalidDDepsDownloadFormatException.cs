

namespace MP.ExceptionSystem
{
    public sealed class InvalidDDepsDownloadFormatException : BaseException
    {

        public InvalidDDepsDownloadFormatException(System.String str) : base(str) { }

        public InvalidDDepsDownloadFormatException(System.String message , System.Exception inner) : base(message, inner) { }
    }
}