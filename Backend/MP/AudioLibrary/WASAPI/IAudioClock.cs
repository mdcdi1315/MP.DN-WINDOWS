
using MP.ComInterop;
using System.Runtime.InteropServices;

namespace MP.AudioLibrary.WASAPI
{
    [ComImport]
    [Guid(WASAPIInterfaceIds.IID_IAudioClock)]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public unsafe interface IAudioClock
    {
        [PreserveSig]
        public HRESULT GetFrequency(System.UInt64* pfreq);

        [PreserveSig]
        public HRESULT GetPosition(System.UInt64* pposition , System.UInt64* pqpcposition);

        [PreserveSig]
        public HRESULT GetCharacteristics(AUDIOCLOCK_CHARACTERISTIC* characteristics);
    }
}