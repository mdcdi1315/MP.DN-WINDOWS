

using MP.ComInterop;
using MP.WindowsInterop;
using System.Runtime.InteropServices;

namespace MP.AudioLibrary.WASAPI
{
    [ComImport]
    [Guid(WASAPIInterfaceIds.IID_IAudioClient)]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public unsafe interface IAudioClient
    {
        [PreserveSig]
        public HRESULT Initialize(
            AUDCLNT_SHAREMODE sharemode,
            AUDCLNT_STREAMFLAGS flags,
            REFERENCE_TIME bufferduration,
            REFERENCE_TIME periodicity,
            WAVEFORMATEX* pfmt,
            GUID* sessionguid = null);

        [PreserveSig]
        public HRESULT GetBufferSize(System.UInt32* pbfframesize);

        [PreserveSig]
        public HRESULT GetStreamLatency(REFERENCE_TIME* pstreamlatency);

        [PreserveSig]
        public HRESULT GetCurrentPadding(System.UInt32* ppadframes);

        [PreserveSig]
        public HRESULT IsFormatSupported(AUDCLNT_SHAREMODE sharemode, WAVEFORMATEX* pfmt, WAVEFORMATEX** closestmatch);

        [PreserveSig]
        public HRESULT GetMixFormat(WAVEFORMATEX** ppfmt);

        [PreserveSig]
        public HRESULT GetDevicePeriod(REFERENCE_TIME* defaultdevperiod, REFERENCE_TIME* minimumdeviceperiod);

        [PreserveSig]
        public HRESULT Start();

        [PreserveSig]
        public HRESULT Stop();

        [PreserveSig]
        public HRESULT Reset();

        [PreserveSig]
        public HRESULT SetEventHandle(System.IntPtr eventhandle);

        [PreserveSig]
        public HRESULT GetService(GUID* piid, void** ppv);
    }
}