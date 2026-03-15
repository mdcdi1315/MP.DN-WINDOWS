
using MP.NativeInterop.Windows;

namespace MP.ExceptionSystem
{
    /// <summary>
    /// Unlike the <see cref="NativeWindowsException"/> class, this one is responsible for throwing unhandled COM errors. <br />
    /// While the <see cref="NativeWindowsException"/> class can also work by just passing the <see cref="HRESULT.Code"/> field into the 
    /// exception, this one is specialized for COM interop, and this should be used instead for any COM-related operation.
    /// </summary>
    public sealed class NativeWindowsCOMException : BaseException , INativeException
    {
        private readonly HRESULT error;
        private readonly System.String message;

        /// <summary>
        /// Constructs a new instance of the <see cref="NativeWindowsCOMException"/> class.
        /// </summary>
        /// <param name="hr">The <see cref="HRESULT"/> code describing the error that was occurred.</param>
        public NativeWindowsCOMException(HRESULT hr) : base() => 
            message = Interop.Kernel32.GetMessage((error = hr).Code);

        /// <summary>
        /// Gets the raw <see cref="HRESULT"/> code as reported by the native call.
        /// </summary>
        public HRESULT ErrorCode => error;

        /// <summary>
        /// Gets the message best describing the given <see cref="HRESULT"/> code.
        /// </summary>
        public System.String NativeMessage => message;

        public override System.String Message => $"Windows COM Exception {error} occured.\nDetails: {message}";
    }
}