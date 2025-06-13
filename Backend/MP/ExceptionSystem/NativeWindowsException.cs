namespace MP.ExceptionSystem
{
    public class NativeWindowsException : BaseException , INativeException
    {
        private System.String message;
        private System.Int32 errorcode;

        public NativeWindowsException() : base() 
        {
            errorcode = Interop.Kernel32.GetLastError();
            if (errorcode == 0) {
                message = "Operation Successfull";
            } else {
                message = Interop.Kernel32.GetMessage(errorcode);
            }
        }

        public NativeWindowsException(System.Int32 errorcode) : base()
        {
            this.errorcode = errorcode;
            message = Interop.Kernel32.GetMessage(errorcode);
        }

        public override string Message => $"Windows error 0x{errorcode:x6} occured. \nDetails: {message}";

        public System.String NativeMessage => message;

        public System.Int32 ErrorCode => errorcode;
    }
}