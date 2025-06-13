
using MP.ComInterop;
using MP.Annotations;
using MP.WindowsInterop;
using System.Runtime.InteropServices;

namespace MP.AudioLibrary.MMDevice
{
    /// <summary>
    /// Base interface for Device objects supported by MMDeviceAPI.
    /// </summary>
    [ComImport]
    [Guid(MMDeviceInterfaceIds.IID_IMMDevice)]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public unsafe interface IMMDevice
    {
        /// <summary>
        /// Creates an object with the specified interface appropriate for this device.
        /// </summary>
        /// <param name="iid">[in] The requested interface</param>
        /// <param name="dwClsCtx">[in] The requested CLSCTX if the object is a COM object</param>
        /// <param name="pActivationParams">[in] Object specific context (usually <see langword="null"/>).</param>
        /// <param name="ppInterface">[out] Address of a pointer to receive the new object interface</param>
        /// <returns>
        /// <list type="bullet">
        ///     <item>
        ///         <see cref="CommonHResults.S_OK"/> if successfull
        ///     </item>
        ///     <item>
        ///         <see cref="CommonHResults.E_NOINTERFACE"/> if the specified interface is not supported for this device type
        ///     </item>
        ///     <item>
        ///         (Other)  If activation is supported for the requested interface, any initialization <br />
        ///         errors returned by the component being activated will be propagated. <br />
        ///         See documentation for the activated interface for more information.
        ///     </item>
        /// </list>
        /// </returns>
        /// <remarks>The caller is responsible for releasing *ppInterface using IUnknown::Release()</remarks>
        [PreserveSig]
        public HRESULT Activate(GUID* iid, CLSCTX dwClsCtx, PROPVARIANT* pActivationParams, void** ppInterface);

        /// <summary>
        /// Opens a <see cref="IPropertyStore"/> for this object
        /// </summary>
        /// <param name="stgmAccess">[in] Access flags (e.g. <see cref="STORAGE_ACCESS_MODE.STGM_READ"/>, <see cref="STORAGE_ACCESS_MODE.STGM_WRITE"/>, or <see cref="STORAGE_ACCESS_MODE.STGM_READWRITE"/>)</param>
        /// <param name="ppProperties">[out] Address of a pointer to receive the PropertyStore interface</param>
        /// <returns><see cref="CommonHResults.S_OK"/> if successfull</returns>
        /// <remarks>The caller is responsible for releasing *ppProperties using IUnknown::Release()</remarks>
        [PreserveSig]
        public HRESULT OpenPropertyStore(STORAGE_ACCESS_MODE stgmAccess, [IsPointerToCOMInterfaceType(typeof(IPropertyStore))] void** ppProperties);

        /// <summary>
        /// Returns the ID of the device as an allocated string
        /// </summary>
        /// <param name="ppstrId">[out] The ID of this Device</param>
        /// <returns><see cref="CommonHResults.S_OK"/> if successfull</returns>
        /// <remarks>The caller is responsible for freeing *pstrId using <see cref="Interop.Ole32.CoTaskMemFree"/>.</remarks>
        [PreserveSig]
        public HRESULT GetId(System.Char** ppstrId);

        /// <summary>
        /// Retrieves the current state of the device
        /// </summary>
        /// <param name="pdwState">[out] Address of a <see cref="DEVICE_STATE"/> that is set to the current state of the device</param>
        /// <returns><see cref="CommonHResults.S_OK"/> if successfull</returns>
        /// <remarks>On return *pdwState will be one of the <see cref="DEVICE_STATE"/> flags</remarks>
        [PreserveSig]
        public HRESULT GetState(DEVICE_STATE* pdwState);
    }
}