

using MP.ComInterop;
using System.Runtime.InteropServices;

namespace MP.AudioLibrary.WASAPI
{
    [ComImport]
    [Guid(WASAPIInterfaceIds.IID_IAudioClock2)]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public unsafe interface IAudioClock2 : IAudioClock
    {
        [PreserveSig]
        public new HRESULT GetFrequency(System.UInt64* pfreq);

        [PreserveSig]
        public new HRESULT GetPosition(System.UInt64* pposition, System.UInt64* pqpcposition);

        [PreserveSig]
        public new HRESULT GetCharacteristics(AUDIOCLOCK_CHARACTERISTIC* characteristics);

        [PreserveSig]
        public HRESULT GetDevicePosition(System.UInt64* pvframepos , System.UInt64* qpcposition);
    }
}