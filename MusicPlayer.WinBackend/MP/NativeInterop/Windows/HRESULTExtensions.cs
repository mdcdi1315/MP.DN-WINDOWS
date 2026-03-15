
using System;

namespace MP.NativeInterop.Windows
{
    /// <summary>
    /// Extension methods for the <see cref="HRESULT"/> type.
    /// </summary>
    public static class HRESULTExtensions
    {
        /// <summary>
        /// Throws an appropriate exception for the current <see cref="HRESULT"/> error code , if this <see cref="HRESULT"/> does represent an error code anyway.
        /// </summary>
        /// <exception cref="ExceptionSystem.NativeWindowsCOMException">The exception that is thrown if <see cref="IS_ERROR"/> returns <see langword="true"/>.</exception>
        [System.Diagnostics.StackTraceHidden]
        public static void ThrowOnFailure(this HRESULT hr)
        {
            Exception e = CreateException(hr);
            if (e is not null) { throw e; }
        }

        /// <summary>
        /// Creates an <see cref="Exception"/> object for this <see cref="HRESULT"/>, if this does represent an error.
        /// </summary>
        /// <returns>An <see cref="Exception"/> object that can be thrown.</returns>
        [Annotations.CodeAnalysis.MustNotReportException]
        public static Exception CreateException(this HRESULT hr)
        {
            if (hr.IS_ERROR)
            {
                return hr.Code switch {
                    CommonHResults.E_OUTOFMEMORY => new OutOfMemoryException("There was not sufficient memory in order to complete the COM operation."),
                    CommonHResults.E_INVALIDARG => new ArgumentException("Argument was invalid."),
                    CommonHResults.E_NOINTERFACE => new NotSupportedException("The requested interface is not supported by this COM object."),
                    CommonHResults.E_NOTIMPL => new NotImplementedException("The specified call is not yet implemented."),
                    CommonHResults.E_UNEXPECTED => new AggregateException("An unexpected error occured."),
                    CommonHResults.E_POINTER => new ArgumentNullException("The provided pointer was invalid.", innerException: null),
                    CommonHResults.E_HANDLE => new ArgumentException("The specified handle is invalid."),
                    CommonHResults.E_BOUNDS => new InvalidOperationException("The operation attempted to access data out of the range of valid values."),
                    CommonHResults.E_ACCESSDENIED => new UnauthorizedAccessException("Access denied."),
                    CommonHResults.E_ABORT => new OperationCanceledException("The operation was aborted."),
                    CommonHResults.E_FAIL => new AggregateException("The operation failed unexpectedly."),
                    _ => new ExceptionSystem.NativeWindowsCOMException(hr)
                };
            }
            return null;
        }
    }
}