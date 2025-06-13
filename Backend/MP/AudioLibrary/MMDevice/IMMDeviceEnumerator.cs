
using MP.ComInterop;
using MP.Annotations;
using MP.WindowsInterop;
using System.Runtime.Versioning;
using System.Runtime.InteropServices;

namespace MP.AudioLibrary.MMDevice
{
    [ComImport]
    [SupportedOSPlatform(WindowsVersions.NTDDI_VISTA)]
    [Guid(MMDeviceInterfaceIds.IID_IMMDeviceEnumerator)]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    [DefaultCOMInterfaceObjectGuid(MMDeviceInterfaceIds.CLSID_MMDeviceEnumerator)]
    public unsafe interface IMMDeviceEnumerator
    {
        /// <summary>Enumerates Endpoint devices</summary>
        /// <param name="dataFlow">[in] The dataflow direction of Endpoint devices to enumerate</param>
        /// <param name="dwStateMask">[in] The allowed states to filter by.  Typically, this is <see cref="DEVICE_STATE.ACTIVE"/></param>
        /// <param name="ppDevices">[out] Address of a pointer that will receive the device collection</param>
        /// <returns><see cref="CommonHResults.S_OK"/> is successfull</returns>
        /// <remarks>The caller is responsible for releasing *ppDevices using IUnknown::Release()</remarks>
        [PreserveSig]
        public HRESULT EnumAudioEndpoints(EDataFlow dataFlow, DEVICE_STATE dwStateMask, [IsPointerToCOMInterfaceType(typeof(IMMDeviceCollection))] void** ppDevices);

        /// <summary>Returns the default Endpoint device for the specified role</summary>
        /// <param name="dataFlow">[in] The dataflow direction of Endpoint devices to enumerate</param>
        /// <param name="role">[in] The role</param>
        /// <param name="ppEndpoint">[out] Address of a pointer that will receive the default Endpoint device</param>
        /// <returns><see cref="CommonHResults.S_OK"/> is successfull</returns>
        /// <remarks>The caller is responsible for releasing *ppEndpoint using IUnknown::Release()</remarks>
        [PreserveSig]
        public HRESULT GetDefaultAudioEndpoint(EDataFlow dataFlow, ERole role, [IsPointerToCOMInterfaceType(typeof(IMMDevice))] void** ppEndpoint);

        /// <summary>
        /// Gets the device with the specified ID. <br />
        /// Use QueryInterface to determine if it is an Endpoint, Pnp Devnode or Pnp Interface.
        /// </summary>
        /// <param name="pwstrId">[in] The ID of the device to retrieve</param>
        /// <param name="ppDevice">[out] Address of a pointer that will receive the device</param>
        /// <returns><see cref="CommonHResults.S_OK"/> is successfull</returns>
        /// <remarks>The caller is responsible for releasing *ppDevices using IUnknown::Release()</remarks>
        [PreserveSig]
        public HRESULT GetDevice(System.Char* pwstrId, [IsPointerToCOMInterfaceType(typeof(IMMDevice))] void** ppDevice);

        /// <summary>Registers the specified client to receive Endpoint device notifications</summary>
        /// <param name="pClient">[in] Pointer to an <see cref="IMMNotificationClient"/> interface on an object implemented by the client</param>
        /// <remarks>The client is responsible for ensuring that the specified object is valid until calling <see cref="UnregisterEndpointNotificationCallback"/></remarks>
        /// <returns><see cref="CommonHResults.S_OK"/> is successfull</returns>
        [PreserveSig]
        public HRESULT RegisterEndpointNotificationCallback([IsPointerToCOMInterfaceType(typeof(IMMNotificationClient))] void* pClient);

        /// <summary>
        /// Unregisters a client that was registered in a previous call to <see cref="RegisterEndpointNotificationCallback"/>
        /// </summary>
        /// <param name="pClient">[in] The client to unregister</param>
        /// <returns><see cref="CommonHResults.S_OK"/> is successfull</returns>
        [PreserveSig]
        public HRESULT UnregisterEndpointNotificationCallback([IsPointerToCOMInterfaceType(typeof(IMMNotificationClient))] void* pClient);
    }
}