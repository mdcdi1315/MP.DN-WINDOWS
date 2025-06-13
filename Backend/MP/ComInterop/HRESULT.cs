using System;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;

namespace MP.ComInterop
{
    /// <summary>
    /// Provides the .NET equivalent of the native HRESULT type , with extended capabilities that most of them are provided
    /// in C/C++ as macros. <br />
    /// Can be also passed in unmanaged functions as a value directly.
    /// </summary>
    [StructLayout(LayoutKind.Explicit , Size = 4)]
    public struct HRESULT
    {
        private const System.Int32 FACILITY_NT_BIT = 0x10000000;

        /// <summary>Gets the actual raw HRESULT code as the native interop returned it.</summary>
        [FieldOffset(0)]
        public System.Int32 Code;

        /// <summary>
        /// Creates a new <see cref="HRESULT"/> instance from a raw value.
        /// </summary>
        /// <param name="code">The raw value to initialize this structure from.</param>
        public HRESULT(System.Int32 code) => Code = code;

        /// <summary>
        /// Creates a new <see cref="HRESULT"/> instance from a specified code severity , the facility where this error comes from, 
        /// and the raw code pinpointing the exact error. This constructor is an exact translation of <strong>MAKE_HRESULT</strong> C/C++ macro.
        /// </summary>
        /// <param name="severity">The error code severity</param>
        /// <param name="facility">The facility where this error comes from</param>
        /// <param name="code">The raw error code</param>
        public HRESULT(HRESULT_SEVERITY severity, FACILITY facility, System.Int32 code)
            => Code = ((((System.UInt32)severity) << 31) | (((System.UInt32)facility) << 16) | ((System.UInt32)code)).ToInt32();

        public static HRESULT FromWin32(System.Int32 x)
        {
            HRESULT hr = new();
            hr.Code = (x <= 0 ? x : ((x & 0x0000FFFF) | ((System.Int32)FACILITY.WIN32 << 16) | 0x80000000)).ToInt32();
            return hr;
        }

        internal HRESULT(Interop.NTSTATUS status) => Code = ((System.Int32)status) | FACILITY_NT_BIT;

        /// <summary>Gets a value whether this code represents a call that was failed.</summary>
        public readonly System.Boolean FAILED => Code < 0;

        /// <summary>Gets a value whether this code represents any error.</summary>
        public readonly System.Boolean IS_ERROR => Code.ToUInt32() >> 31 == 1;

        /// <summary>Gets a value whether the COM call succeeded.</summary>
        public readonly System.Boolean SUCCEEDED => Code >= 0;

        /// <summary>Extracts the actual code from this HRESULT.</summary>
        public readonly System.Int32 HRESULT_CODE => Code & 0xFFFF;

        /// <summary>Gets the facility or the code which this HRESULT comes from.</summary>
        public readonly FACILITY HRESULT_FACILITY => (FACILITY)((Code >> 16) & 0x1fff);

        /// <summary>Gets the severity of this code.</summary>
        public readonly HRESULT_SEVERITY HRESULT_SEVERITY => (HRESULT_SEVERITY)((Code >> 31) & 0x1);

        /// <summary>Maps this code to a .NET exception if possible.</summary>
        /// <remarks>For newer designs, it is recommended to use the <see cref="ThrowOnFailure"/> method instead, when you want to throw on any failure.</remarks>
        [Annotations.DeprecatedMayBeRemoved]
        public readonly Exception MappingException => Marshal.GetExceptionForHR(Code);

        /// <summary>
        /// Throws an appropriate exception for the current <see cref="HRESULT"/> error code , if this <see cref="HRESULT"/> does represent an error code anyways.
        /// </summary>
        /// <exception cref="ExceptionSystem.NativeWindowsCOMException">The exception that is thrown if <see cref="IS_ERROR"/> returns <see langword="true"/>.</exception>
        [System.Diagnostics.StackTraceHidden]
        public readonly void ThrowOnFailure()
        {
            var e = CreateException();
            if (e is not null) { throw e; }
        }

        /// <summary>
        /// Creates an <see cref="Exception"/> object for this <see cref="HRESULT"/>, if this does represent an error. <br />
        /// This is the recommeneded to use for newer designs.
        /// </summary>
        /// <returns>An <see cref="Exception"/> object that can be thrown.</returns>
        public readonly Exception CreateException()
        {
            if (IS_ERROR)
            {
                switch (this)
                {
                    case CommonHResults.E_OUTOFMEMORY:
                        return new OutOfMemoryException("There was not sufficient memory in order to complete the COM operation.");
                    case CommonHResults.E_INVALIDARG:
                        return new ArgumentException("Argument was invalid.");
                    case CommonHResults.E_NOINTERFACE:
                        return new NotSupportedException("The requested interface is not supported by this COM object.");
                    case CommonHResults.E_NOTIMPL:
                        return new NotImplementedException("The specified call is not yet implemented.");
                    case CommonHResults.E_UNEXPECTED:
                        return new AggregateException("An unexpected error occured.");
                    case CommonHResults.E_POINTER:
                        return new ArgumentNullException("The provided pointer was invalid.", innerException: null);
                    case CommonHResults.E_HANDLE:
                        return new ArgumentException("The specified handle is invalid.");
                    default:
                        return new ExceptionSystem.NativeWindowsCOMException(this);
                }
            }
            return null;
        }

        /// <summary>
        /// For the interop infrastructure. <br />
        /// It allows to directly get the raw code to test it for specific errors that you want to catch.
        /// </summary>
        /// <param name="hr">The <see cref="HRESULT"/> to translate.</param>
        // We can aggressively inline this operator in JIT since it does only load the already known field.
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator System.Int32(HRESULT hr) => hr.Code;

        /// <summary>
        /// For the interop infrastracture. <br />
        /// Converts the given code to a new <see cref="HRESULT"/> structure.
        /// </summary>
        /// <param name="code">The <see cref="HRESULT"/> code that you wish to be reinterpreted as a <see cref="HRESULT"/> structure.</param>
        // However we cannot inline this operator since new() is a costly operation.
        public static implicit operator HRESULT(System.Int32 code) => new(code);

        /// <summary>
        /// Returns the current error code that this <see cref="HRESULT"/> structure holds , 
        /// described as a hexadecimal number.
        /// </summary>
        /// <returns>A string representing the current code as a hexadecimal number.</returns>
        public readonly override System.String ToString() => $"0x{Code:x2}";
    }
}
