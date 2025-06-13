
using MP.ComInterop;
using MP.WindowsInterop;
using System.Runtime.InteropServices;

namespace MP.AudioLibrary.WASAPI
{
    [ComImport]
    [Guid(WASAPIInterfaceIds.IID_IAudioClient2)]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public unsafe interface IAudioClient2 : IAudioClient
    {
        [PreserveSig]
        public new HRESULT Initialize(
            AUDCLNT_SHAREMODE sharemode,
            AUDCLNT_STREAMFLAGS flags,
            REFERENCE_TIME bufferduration,
            REFERENCE_TIME periodicity,
            WAVEFORMATEX* pfmt,
            GUID* sessionguid = null);

        [PreserveSig]
        public new HRESULT GetBufferSize(System.UInt32* pbfframesize);

        [PreserveSig]
        public new HRESULT GetStreamLatency(REFERENCE_TIME* pstreamlatency);

        [PreserveSig]
        public new HRESULT GetCurrentPadding(System.UInt32* ppadframes);

        [PreserveSig]
        public new HRESULT IsFormatSupported(AUDCLNT_SHAREMODE sharemode, WAVEFORMATEX* pfmt, WAVEFORMATEX** closestmatch);

        [PreserveSig]
        public new HRESULT GetMixFormat(WAVEFORMATEX** ppfmt);

        [PreserveSig]
        public new HRESULT GetDevicePeriod(REFERENCE_TIME* defaultdevperiod, REFERENCE_TIME* minimumdeviceperiod);

        [PreserveSig]
        public new HRESULT Start();

        [PreserveSig]
        public new HRESULT Stop();

        [PreserveSig]
        public new HRESULT Reset();

        [PreserveSig]
        public new HRESULT SetEventHandle(System.IntPtr eventhandle);

        [PreserveSig]
        public new HRESULT GetService(GUID* piid, void** ppv);

        [PreserveSig]
        public HRESULT IsOffloadCapable(AUDIO_STREAM_CATEGORY category, BOOL* offloadcapable);

        [PreserveSig]
        public HRESULT SetClientProperties(AudioClientProperties* pprops);

        [PreserveSig]
        public HRESULT GetBufferSizeLimits(
            WAVEFORMATEX* pfmt , 
            BOOL iseventdriven , 
            REFERENCE_TIME* phnsminbufferduration , 
            REFERENCE_TIME* phnsmaxbufferduration
        );
    }
}