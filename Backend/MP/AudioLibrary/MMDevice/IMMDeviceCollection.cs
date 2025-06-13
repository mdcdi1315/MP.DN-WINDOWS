
using MP.Annotations;
using MP.ComInterop;
using System.Runtime.InteropServices;

namespace MP.AudioLibrary.MMDevice
{
    /// <summary>
    /// Provides methods for accessing and enumerating a collection of <see cref="IMMDevice"/> objects
    /// </summary>
    [ComImport]
    [Guid(MMDeviceInterfaceIds.IID_IMMDeviceCollection)]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public unsafe interface IMMDeviceCollection
    {
        /// <summary>
        /// Returns the number of devices in the collection.
        /// </summary>
        /// <param name="pcDevices">[out] Address of a <see cref="System.UInt32"/> that will receive the count</param>
        /// <returns><see cref="CommonHResults.S_OK"/> if successfull</returns>
        [PreserveSig]
        public HRESULT GetCount(System.UInt32* pcDevices);

        /// <summary>
        /// Gets the device at the specified index in the collection.
        /// </summary>
        /// <param name="nDevice">[in] The index</param>
        /// <param name="ppDevice">[out] Address of an <see cref="IMMDevice"/> pointer that will receive the device</param>
        /// <returns><see cref="CommonHResults.S_OK"/> if successfull</returns>
        /// <remarks>The caller is responsible for releasing *ppDevice using IUnknown::Release()</remarks>
        [PreserveSig]
        public HRESULT Item(System.UInt32 nDevice, [IsPointerToCOMInterfaceType(typeof(IMMDevice))] void** ppDevice);
    }
}