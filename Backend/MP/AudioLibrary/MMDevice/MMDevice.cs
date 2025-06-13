
using System;
using MP.ComInterop;

namespace MP.AudioLibrary.MMDevice
{
    public unsafe sealed class MMDevice : IDisposable
    {
        private IMMDevice native;
        private PropertyStore ps;

        internal MMDevice(IMMDevice native) 
        {
            ArgumentNullException.ThrowIfNull(native);
            this.native = native;
        }

        private System.Object ActivateInterfaceId(WindowsInterop.GUID guid , PROPVARIANT* pactparams = null)
        {
            void* pout;
            HRESULT hr = native.Activate(&guid , CLSCTX.CLSCTX_ALL , pactparams , &pout);
            hr.ThrowOnFailure();
            return ComMarshalling.CreateInteropObject(pout);
        }

        public void OpenPropertyStore(STORAGE_ACCESS_MODE access = STORAGE_ACCESS_MODE.STGM_READ)
        {
            ObjectDisposedException.ThrowIf(native is null, this);
            OpenPropertyStoreInternal(access);
        }

        public System.Object Activate(Guid intguid) => ActivateInterfaceId(WindowsInterop.GUID.FromGUID(intguid));

        public System.Object Activate(Guid intguid, PROPVARIANT activationparameters) => ActivateInterfaceId(WindowsInterop.GUID.FromGUID(intguid), &activationparameters);

        private void OpenPropertyStoreInternal(STORAGE_ACCESS_MODE access = STORAGE_ACCESS_MODE.STGM_READ)
        {
            if (ps is not null) { return; }
            void* propsnative;
            HRESULT hr = native.OpenPropertyStore(access, &propsnative);
            hr.ThrowOnFailure();
            ps = new(ComMarshalling.CreateInteropObject(propsnative) as IPropertyStore);
        }

        public System.String ID
        {
            get {
                ObjectDisposedException.ThrowIf(native is null, this);
                System.Char* pid;
                HRESULT hr = native.GetId(&pid);
                hr.ThrowOnFailure();
                System.String strret = new(pid);
                Interop.Ole32.CoTaskMemFree(pid);
                return strret;
            }
        }

        public DEVICE_STATE State
        {
            get {
                ObjectDisposedException.ThrowIf(native is null, this);
                DEVICE_STATE pstate;
                HRESULT hr = native.GetState(&pstate);
                hr.ThrowOnFailure();    
                return pstate;
            }
        }

        public System.String FriendlyName
        {
            get {
                ObjectDisposedException.ThrowIf(native is null, this);
                OpenPropertyStoreInternal();
                if (ps.TryGetProperty(MMDevicePropertyKeys.Device_FriendlyName, out var pp)) {
                    return pp.Value as System.String ?? "Unknown";
                } else {
                    return "Unknown";
                }
            }
        }

        public System.String DeviceFriendlyName
        {
            get {
                ObjectDisposedException.ThrowIf(native is null, this);
                OpenPropertyStoreInternal();
                if (ps.TryGetProperty(MMDevicePropertyKeys.DeviceInterface_FriendlyName, out var pp)) {
                    return pp.Value as System.String ?? "Unknown";
                } else {
                    return "Unknown";
                }
            }
        }

        public System.String InstanceID
        {
            get {
                ObjectDisposedException.ThrowIf(native is null, this);
                OpenPropertyStoreInternal();
                if (ps.TryGetProperty(MMDevicePropertyKeys.Device_InstanceId, out var pp))
                {
                    return pp.Value as System.String ?? "Unknown";
                }
                else
                {
                    return "Unknown";
                }
            }
        }

        public System.String Manufacturer
        {
            get {
                ObjectDisposedException.ThrowIf(native is null, this);
                OpenPropertyStoreInternal();
                if (ps.TryGetProperty(MMDevicePropertyKeys.Device_Manufacturer, out var pp))
                {
                    return pp.Value as System.String ?? "Unknown";
                }
                else
                {
                    return "Unknown";
                }
            }
        }

        public WAVEFORMATEXTENSIBLE DeviceAudioFormat
        {
            get {
                ObjectDisposedException.ThrowIf(native is null, this);
                OpenPropertyStoreInternal();
                if (ps.TryGetProperty(MMDevicePropertyKeys.DeviceFormat , out var pp)) {
                    System.Byte[] dt = pp.Value as System.Byte[];
                    if (dt.Length == sizeof(WAVEFORMATEX)) {
                        return new() {
                            BaseFormat = dt.ReadStructure<WAVEFORMATEX>(0)
                        };
                    } else {
                        return dt.ReadStructure<WAVEFORMATEXTENSIBLE>(0);
                    }
                } else {
                    return default;
                }
            }
        }

        public EDataFlow DataFlow
        {
            get {
                ObjectDisposedException.ThrowIf(native is null, this);
                try {
                    EDataFlow d;
                    (native as IMMEndpoint).GetDataFlow(&d);
                    return d;
                } catch (NullReferenceException) {
                    throw new NotSupportedException("This MMDevice is not a valid MM endpoint.");
                }
            }
        }

        /// <summary>
        /// Endpoint guid
        /// </summary>
        public System.String EndpointGuid
        {
            get
            {
                ObjectDisposedException.ThrowIf(native is null, this);
                OpenPropertyStoreInternal();
                if (ps.TryGetProperty(MMDevicePropertyKeys.GUID, out var pp)) {
                    return pp.Value as System.String ?? "Unknown";
                } else {
                    return "Unknown";
                }
            }
        }

        /// <summary>Gets a string representation of this MM Device object</summary>
        public override string ToString() => FriendlyName;

        private void PartialDispose()
        {
            ps?.Dispose();
            ps = null;
        }

        public void Dispose()
        {
            PartialDispose();
            if (native is not null)
            {
                ComMarshalling.ReleaseInteropObject(native);
                native = null;
            }
            GC.SuppressFinalize(this);
        }

        ~MMDevice() => PartialDispose();
    }
}