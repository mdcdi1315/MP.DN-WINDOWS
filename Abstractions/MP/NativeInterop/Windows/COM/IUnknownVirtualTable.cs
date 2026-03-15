

using System.Runtime.InteropServices;

namespace MP.NativeInterop.Windows.COM
{
    /// <summary>
    /// Provides the structure for the IUnknown interface Virtual Table.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public unsafe struct IUnknownVirtualTable
    {
        /// <summary>
        /// Gets the function pointer that can query an interface on the current object.
        /// </summary>
        public delegate* unmanaged[Stdcall]<void*, GUID*, void**, HRESULT> QueryInterface;

        /// <summary>
        /// Gets the function pointer that can add a reference to the current object instance.
        /// </summary>
        public delegate* unmanaged[Stdcall]<void*, System.UInt32> AddRef;

        /// <summary>
        /// Gets the function pointer that can release a reference to the current object instance.
        /// </summary>
        public delegate* unmanaged[Stdcall]<void*, System.UInt32> Release;
    }
}