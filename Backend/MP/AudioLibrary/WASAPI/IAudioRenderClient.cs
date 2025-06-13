
using MP.ComInterop;
using System.Runtime.InteropServices;

namespace MP.AudioLibrary.WASAPI
{
    [ComImport]
    [Guid(WASAPIInterfaceIds.IID_IAudioRenderClient)]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public unsafe interface IAudioRenderClient
    {
        // NOTE: ppaudiobuffer size is: numframesrequested * WAVEFORMATEX.BlockAlign 
        [PreserveSig]
        public HRESULT GetBuffer(System.UInt32 numframesrequested, System.Byte** ppaudiobuffer);

        // Sends the buffer to the underlying audio device.
        [PreserveSig]
        public HRESULT ReleaseBuffer(System.UInt32 numframeswritten, AUDCLNT_BUFFERFLAGS flags);
    }
}