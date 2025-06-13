

namespace MP.ExceptionSystem
{
    public sealed class HttpFailureException : BaseException
    {
        private System.Int32 errorcode;

        public HttpFailureException() : base("Server returned an unsuccessfull HTTP code.") { }

        public HttpFailureException(string message) : base(message) { }

        public HttpFailureException(System.String message, System.Exception innerException) : base(message, innerException) { }

        public HttpFailureException(System.Int32 errorcode, string message) : base($"HTTP request failed with code {errorcode}: {message}") { this.errorcode = errorcode; }

        public System.Int32 HttpErrorCode => errorcode;

    }
}