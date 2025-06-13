
using MP.ComInterop;
using System.Runtime.InteropServices;

namespace MP.AudioLibrary.MMDevice
{
    /// <summary>
    /// This interface is implemented by a client who wishes to be notified of changes to Endpoint devices on the system.
    /// </summary>
    [ComImport]
    [Guid(MMDeviceInterfaceIds.IID_IMMNotificationClient)]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public unsafe interface IMMNotificationClient
    {
        /// <summary>
        /// Called by the MMDeviceEnumerator object when the state of an Endpoint device changes
        /// </summary>
        /// <param name="pwstrDeviceId">[in] The ID of the Endpoint device whose state has changed</param>
        /// <param name="dwNewState">[in] The new state of the device</param>
        /// <returns>The return value is ignored - however prefer to return <see cref="CommonHResults.S_OK"/>.</returns>
        [PreserveSig]
        public HRESULT OnDeviceStateChanged(System.Char* pwstrDeviceId, DEVICE_STATE dwNewState);

        /// <summary>
        /// Called by the MMDeviceEnumerator object when a new Endpoint device is added to the system
        /// </summary>
        /// <param name="pwstrDeviceId">[in] The ID of the new Endpoint device</param>
        /// <returns>The return value is ignored - however prefer to return <see cref="CommonHResults.S_OK"/>.</returns>
        /// <remarks>Clients should check the state of the Endpoint before using it.  It is generally more useful
        ///  to monitor state changes than Endpoint additions and removals</remarks>
        [PreserveSig]
        public HRESULT OnDeviceAdded(System.Char* pwstrDeviceId);

        /// <summary>
        /// Called by the MMDeviceEnumerator object when an Endpoint device is removed from the system
        /// </summary>
        /// <param name="pwstrDeviceId">[in] The ID of the Endpoint device that was removed</param>
        /// <remarks>It is generally more useful to monitor state changes than Endpoint additions and removals</remarks>
        /// <returns>The return value is ignored - however prefer to return <see cref="CommonHResults.S_OK"/>.</returns>
        [PreserveSig]
        public HRESULT OnDeviceRemoved(System.Char* pwstrDeviceId);

        /// <summary>
        /// Called by the MMDeviceEnumerator object when a value in an Endpoint device property store changes
        /// </summary>
        /// <param name="pwstrDeviceId">[in] The ID of the Endpoint whose Property has changed</param>
        /// <param name="key">[in] The Property that was modified</param>
        /// <returns>The return value is ignored - however prefer to return <see cref="CommonHResults.S_OK"/>.</returns>
        [PreserveSig]
        public HRESULT OnPropertyValueChanged(System.Char* pwstrDeviceId, PROPERTYKEY key);
    }
}