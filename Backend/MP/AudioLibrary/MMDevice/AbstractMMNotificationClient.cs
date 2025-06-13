

using MP.ComInterop;

namespace MP.AudioLibrary.MMDevice
{
    /// <summary>
    /// A managed layer around the <see cref="IMMNotificationClient"/> interface so that it can be accessed safely from managed code. <br />
    /// The user who needs the <see cref="IMMNotificationClient"/> creates a class extending this one and by properly overriding 
    /// the methods he wants to be notified for (such as, <see cref="OnDeviceAdded(string)"/>), he just creates an override for it.
    /// </summary>
    public abstract class AbstractMMNotificationClient : IMMNotificationClient
    {
        protected virtual void OnDeviceStateChanged(System.String DeviceID , DEVICE_STATE newstate) { }

        protected virtual void OnDeviceAdded(System.String DeviceID) { }

        protected virtual void OnDeviceRemoved(System.String DeviceID) { }

        protected virtual void OnPropertyValueChanged(System.String DeviceID , PROPERTYKEY key) { }

        unsafe HRESULT IMMNotificationClient.OnDeviceStateChanged(char* pwstrDeviceId, DEVICE_STATE dwNewState)
        {
            OnDeviceStateChanged(new(pwstrDeviceId), dwNewState);
            return CommonHResults.S_OK;
        }

        unsafe HRESULT IMMNotificationClient.OnDeviceAdded(char* pwstrDeviceId)
        {
            OnDeviceAdded(new(pwstrDeviceId));
            return CommonHResults.S_OK;
        }

        unsafe HRESULT IMMNotificationClient.OnDeviceRemoved(char* pwstrDeviceId)
        {
            OnDeviceRemoved(new(pwstrDeviceId));
            return CommonHResults.S_OK;
        }

        unsafe HRESULT IMMNotificationClient.OnPropertyValueChanged(char* pwstrDeviceId, PROPERTYKEY key)
        {
            OnPropertyValueChanged(new(pwstrDeviceId), key);
            return CommonHResults.S_OK;
        }
    }
}