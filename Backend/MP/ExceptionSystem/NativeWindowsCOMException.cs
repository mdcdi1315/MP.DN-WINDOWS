

using MP.ComInterop;

namespace MP.ExceptionSystem
{
    /// <summary>
    /// Unlike the <see cref="NativeWindowsException"/> class, this one is responsible for throwing unhandled COM errors. <br />
    /// While the <see cref="NativeWindowsException"/> class can also work by just passing the <see cref="HRESULT.Code"/> field into the 
    /// exception, this one is specialized for COM interop, and this should be used instead for any COM-related operation.
    /// </summary>
    public sealed class NativeWindowsCOMException : BaseException , INativeException
    {
        private HRESULT error;
        private System.String message;

        public NativeWindowsCOMException(HRESULT hr) : base() => 
            message = Interop.Kernel32.GetMessage((error = hr).Code);

        public HRESULT ErrorCode => error;

        public System.String NativeMessage => message;

        public override System.String Message => $"Windows COM Exception {error} occured.\nDetails: {message}";
    }
}