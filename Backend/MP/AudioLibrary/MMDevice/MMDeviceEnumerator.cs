

using System;
using MP.ComInterop;
using MP.WindowsInterop;
using System.Runtime.Versioning;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace MP.AudioLibrary.MMDevice
{
    public unsafe sealed class MMDeviceEnumerator : IDisposable
    {
        private IMMDeviceEnumerator enumerator;
        private AbstractMMNotificationClient notifclient;

        [SupportedOSPlatform(WindowsVersions.NTDDI_VISTA)]
        public MMDeviceEnumerator()
        {
            if (SystemInfo.OperatingSystemVersion.Major < 6) {
                throw new PlatformNotSupportedException("This functionality is only supported on Windows Vista or newer.");
            }
            notifclient = null;
            enumerator = ComMarshalling.GetClassInstanceAsInterface<IMMDeviceEnumerator>(CLSCTX.CLSCTX_ALL);
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0028:Simplify collection initialization", Justification = "Needs to return a read-only collection")]
        public IList<MMDevice> EnumAudioEndpoints(EDataFlow dataflow , DEVICE_STATE statemask = DEVICE_STATE.ACTIVE)
        {
            ObjectDisposedException.ThrowIf(enumerator is null, this);
            void* tp;
            HRESULT hr = enumerator.EnumAudioEndpoints(dataflow, statemask, &tp);
            if (hr.FAILED) { throw hr.MappingException; }
            IMMDeviceCollection collection = ComMarshalling.CreateInteropObject(tp) as IMMDeviceCollection;
            try
            {
                System.UInt32 devices;
                hr = collection.GetCount(&devices);
                if (hr.FAILED) { throw hr.MappingException; }
                tp = null;
                List<MMDevice> devs = new(devices.ToInt32());
                for (System.UInt32 I = 0; I < devices; I++)
                {
                    hr = collection.Item(I, &tp);
                    if (hr.FAILED) { continue; }
                    devs.Add(new(ComMarshalling.CreateInteropObject(tp, -1) as IMMDevice));
                }
                return new Collections.ReadOnlyList<MMDevice>(devs);
            } finally {
                ComMarshalling.ReleaseInteropObject(collection);
            }
        }

        public System.Boolean TryGetDevice(System.String devid , out MMDevice device)
        {
            ObjectDisposedException.ThrowIf(enumerator is null, this);
            ArgumentException.ThrowIfNullOrWhiteSpace(devid);
            HRESULT hr;
            void* pdevobj;
            device = null;
            fixed (System.Char* pdev = devid)
            {
                hr = enumerator.GetDevice(pdev, &pdevobj);
            }
            if (hr.FAILED) { return false; }
            device = new(ComMarshalling.CreateInteropObject(pdevobj) as IMMDevice);
            return true;
        }

        public MMDevice GetDevice(System.String devid)
        {
            ObjectDisposedException.ThrowIf(enumerator is null, this);
            ArgumentException.ThrowIfNullOrWhiteSpace(devid);
            HRESULT hr;
            void* pdevobj;
            fixed (System.Char* pdev = devid)
            {
                hr = enumerator.GetDevice(pdev, &pdevobj);
            }
            if (hr.FAILED) { throw hr.MappingException; }
            return new(ComMarshalling.CreateInteropObject(pdevobj) as IMMDevice);
        }

        public MMDevice GetDefaultEndpoint(EDataFlow desiredflow , ERole desiredrole)
        {
            ObjectDisposedException.ThrowIf(enumerator is null, this);
            void* pendpoint;
            HRESULT hr = enumerator.GetDefaultAudioEndpoint(desiredflow, desiredrole, &pendpoint);
            if (hr.FAILED) { throw hr.MappingException; }
            return new(ComMarshalling.CreateInteropObject(pendpoint) as IMMDevice);
        }

        public AbstractMMNotificationClient NotificationClient
        {
            get {
                ObjectDisposedException.ThrowIf(enumerator is null, this);
                return notifclient;
            }
            set {
                ObjectDisposedException.ThrowIf(enumerator is null, this);
                ArgumentNullException.ThrowIfNull(value);
                HRESULT hr;
                System.IntPtr ptr;
                if (notifclient is not null) {
                    ptr = Marshal.GetIUnknownForObject(notifclient);
                    hr = enumerator.UnregisterEndpointNotificationCallback(ptr.ToPointer());
                    if (hr.FAILED) { throw hr.MappingException; }
                    notifclient = null;
                }
                ptr = Marshal.GetIUnknownForObject(notifclient = value);
                hr = enumerator.RegisterEndpointNotificationCallback(ptr.ToPointer());
                if (hr.FAILED) { throw hr.MappingException; }
            }
        }

        /// <summary>
        /// Disposes this <see cref="MMDeviceEnumerator"/> instance.
        /// </summary>
        public void Dispose() 
        {
            if (enumerator is not null)
            {
                if (notifclient is not null)
                {
                    System.IntPtr ptr;
                    ptr = Marshal.GetIUnknownForObject(notifclient);
                    enumerator.UnregisterEndpointNotificationCallback(ptr.ToPointer());
                    notifclient = null;
                }
                ComMarshalling.ReleaseInteropObject(enumerator);
                enumerator = null;
            }
        }
    }
}