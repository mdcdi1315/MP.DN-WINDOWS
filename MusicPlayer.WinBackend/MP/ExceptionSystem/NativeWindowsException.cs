using MP.NativeInterop.Windows;

namespace MP.ExceptionSystem
{
    public class NativeWindowsException : BaseException , INativeException
    {
        private readonly System.String message;
        private readonly System.UInt32 errorcode;

        public static void ThrowIfError(NTSTATUS status)
        {
            if (status != NTSTATUS.STATUS_SUCCESS) {
                throw new NativeWindowsException(status);
            }
        }

        public static void ThrowIfError(System.UInt32 error_code)
        {
            if (error_code != Interop.Errors.ERROR_SUCCESS) {
                throw new NativeWindowsException(error_code);
            }
        }

        public static void ThrowFromLastError() => ThrowIfError(Interop.Kernel32.GetLastError());

        public NativeWindowsException() : base() 
        {
            errorcode = Interop.Kernel32.GetLastError();
            if (errorcode == 0) {
                message = "Operation Successfull";
            } else {
                message = Interop.Kernel32.GetMessage(errorcode);
            }
        }

        public NativeWindowsException(System.UInt32 errorcode) : base() => message = Interop.Kernel32.GetMessage(this.errorcode = errorcode);

        public NativeWindowsException(NTSTATUS ntstatus) : this(Interop.NtDll.RtlNtStatusToDosError(ntstatus)) { }

        public override string Message => $"Windows error 0x{errorcode:x6} occured.\nDetails: {message}";

        public System.String NativeMessage => message;

        public System.UInt32 ErrorCode => errorcode;
    }
}