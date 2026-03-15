
using System.Diagnostics.CodeAnalysis;

namespace MP.NativeInterop.Windows.COM
{
    /// <summary>
    /// Implemented by classes inside COM interfaces to be wrapped in COM usage scenarios. <br />
    /// Typically, an implementation of this interface is source-generated.
    /// </summary>
    public unsafe interface ICOMDispatchServicesProvider
    {
        /// <summary>
        /// Generates a virtual table for the current COM interface.
        /// </summary>
        /// <param name="builder">A virtual table builder providing the virtual table for this COM interface.</param>
        public static abstract void GenerateCCWVirtualTable([DisallowNull] VirtualTableBuilder builder);

        /// <summary>
        /// Creates an object that implements the COM interface and wraps it around. <br />
        /// Typically, the returned object extends from the <see cref="INativeCOMObject"/> interface and implements the COM interface to use.
        /// </summary>
        /// <param name="p_native">The native pointer to wrap.</param>
        /// <returns>The constructed wrapper object that delegates to the actual COM object.</returns>
        public static abstract INativeCOMObject CreateNativeObject([DisallowNull] void* p_native);
    }
}