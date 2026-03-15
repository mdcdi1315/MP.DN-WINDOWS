using System;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;

namespace MP.NativeInterop.Windows
{
    /// <summary>
    /// Provides the .NET equivalent of the native HRESULT type , with extended capabilities that most of them are provided
    /// in C/C++ as macros. <br />
    /// Can be also passed in unmanaged functions as a value directly.
    /// </summary>
    [StructLayout(LayoutKind.Explicit, Pack = 4, Size = 4)]
    public readonly struct HRESULT
    {
        private const System.UInt32 FACILITY_NT_BIT = 0x10000000;

        /// <summary>Gets the actual raw HRESULT code as the native interop returned it.</summary>
        [FieldOffset(0)]
        public readonly System.UInt32 Code;

        /// <summary>
        /// Creates a new <see cref="HRESULT"/> instance from a raw value.
        /// </summary>
        /// <param name="code">The raw value to initialize this structure from.</param>
        [Annotations.CodeAnalysis.MustNotReportException]
        public HRESULT(System.UInt32 code) => Code = code;

        /// <summary>
        /// Creates a new <see cref="HRESULT"/> instance from a specified code severity , the facility where this error comes from, 
        /// and the raw code pinpointing the exact error. This constructor is an exact translation of <strong>MAKE_HRESULT</strong> C/C++ macro.
        /// </summary>
        /// <param name="severity">The error code severity</param>
        /// <param name="facility">The facility where this error comes from</param>
        /// <param name="code">The raw error code</param>
        [Annotations.CodeAnalysis.MustNotReportException]
        public HRESULT(HRESULT_SEVERITY severity, FACILITY facility, System.UInt32 code) => Code = (((System.UInt32)severity) << 31) | (((System.UInt32)facility) << 16) | code;

        /// <summary>
        /// Translates a Win32 error code to a HRESULT code using the <see cref="FACILITY.WIN32"/> facility code.
        /// </summary>
        /// <param name="x">The Win32 error code to translate</param>
        /// <returns>The translated <see cref="HRESULT"/> code.</returns>
        [Annotations.CodeAnalysis.MustNotReportException]
        public static HRESULT FromWin32(System.UInt32 x)
            => new(x <= 0U ? x : ((x & 0x0000FFFFU) | ((System.UInt32)FACILITY.WIN32 << 16) | 0x80000000U));

        /// <summary>
        /// Translates a <see cref="NTSTATUS"/> code to a HRESULT code using the <see cref="FACILITY_NT_BIT"/> value.
        /// </summary>
        /// <param name="status">The <see cref="NTSTATUS"/> value to convert.</param>
        [Annotations.CodeAnalysis.MustNotReportException]
        public HRESULT(NTSTATUS status) => Code = ((System.UInt32)status) | FACILITY_NT_BIT;

        /// <summary>Gets a value whether this code represents a call that was failed.</summary>
        public readonly System.Boolean FAILED => Code < 0U;

        /// <summary>Gets a value whether this code represents any error.</summary>
        public readonly System.Boolean IS_ERROR => (Code >> 31) == 1U;

        /// <summary>Gets a value whether the COM call succeeded.</summary>
        public readonly System.Boolean SUCCEEDED => Code >= 0U;

        /// <summary>Extracts the actual code from this HRESULT.</summary>
        public readonly System.UInt16 HRESULT_CODE => (Code & 0xFFFFU).ToUInt16();

        /// <summary>Gets the facility or the code which this HRESULT comes from.</summary>
        public readonly FACILITY HRESULT_FACILITY => (FACILITY)((Code >> 16) & 0x1FFFU);

        /// <summary>Gets the severity of this code.</summary>
        public readonly HRESULT_SEVERITY HRESULT_SEVERITY => (HRESULT_SEVERITY)((Code >> 31) & 0x1U);

        /// <summary>
        /// For the interop infrastructure. <br />
        /// It allows to directly get the raw code to test it for specific errors that you want to catch.
        /// </summary>
        /// <param name="hr">The <see cref="HRESULT"/> to translate.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [Annotations.CodeAnalysis.MustNotReportException]
        public static implicit operator System.UInt32(HRESULT hr) => hr.Code;

        /// <summary>
        /// For the interop infrastracture. <br />
        /// Converts the given code to a new <see cref="HRESULT"/> structure.
        /// </summary>
        /// <param name="code">The <see cref="HRESULT"/> code that you wish to be reinterpreted as a <see cref="HRESULT"/> structure.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator HRESULT(System.UInt32 code) => new(code);

        /// <summary>
        /// Returns the current error code that this <see cref="HRESULT"/> structure holds , 
        /// described as a hexadecimal number.
        /// </summary>
        /// <returns>A string representing the current code as a hexadecimal number.</returns>
        public readonly override System.String ToString() => String.Concat("0x", Code.ToString("x2"));
    }
}
