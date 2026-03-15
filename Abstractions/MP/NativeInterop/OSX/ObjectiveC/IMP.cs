
using System;
using System.Runtime.InteropServices;

namespace MP.NativeInterop.OSX.ObjectiveC
{
    /// <summary>
    /// Defines a structure for a method's function pointer.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public readonly unsafe struct IMP
    {
        /// <summary>
        /// The function pointer to be invoked <br />
        /// Cast to an appropriate function pointer before calling.
        /// </summary>
        public readonly void* Implementation;

        /// <summary>
        /// Initializes a new instance of the <see cref="IMP"/> structure from the specified method implementation.
        /// </summary>
        /// <param name="p_implementation">The method implementation to use.</param>
        public IMP(void* p_implementation)
        {
            ArgumentNullException.ThrowIfNull(p_implementation);
            Implementation = p_implementation;
        }
    }
}