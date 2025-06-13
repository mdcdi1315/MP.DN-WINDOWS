using System;
using MP.ComInterop;

namespace MP.AudioLibrary.WASAPI
{
    /// <summary>
    /// Audio Render Client
    /// </summary>
    public sealed unsafe class AudioRenderClient : IDisposable
    {
        private IAudioRenderClient render;

        internal AudioRenderClient(IAudioRenderClient audiorenderclient)
        {
            ArgumentNullException.ThrowIfNull(audiorenderclient);
            render = audiorenderclient;
        }

        /// <summary>Gets a pointer to the rendering buffer.</summary>
        /// <remarks>Do not free somehow the pointer returned , it is owned by the audio device.</remarks>
        /// <param name="numFramesRequested">Number of frames requested</param>
        /// <returns>
        /// The HRESULT code whether the operation succeeded or not. <br />
        /// Use it to handle different situations that may occur during rendering.
        /// </returns>
        public HRESULT GetBuffer(System.UInt32 numFramesRequested , out System.Byte* buffer)
        {
            System.Byte* pbuf;
            var hr = render.GetBuffer(numFramesRequested, &pbuf);
            buffer = pbuf;
            return hr;
        }

        /// <summary>Releases the buffer, effectively sending the buffer to the underlying audio device.</summary>
        /// <param name="numframeswritten">Number of frames written</param>
        /// <param name="bufferFlags">Buffer flags</param>
        /// <returns>
        /// The HRESULT code whether the operation succeeded or not. <br />
        /// Use it to handle different situations that may occur during rendering.
        /// </returns>
        public HRESULT ReleaseBuffer(System.UInt32 numframeswritten, AUDCLNT_BUFFERFLAGS bufferFlags) 
            => render.ReleaseBuffer(numframeswritten, bufferFlags);

        /// <summary>Destroy the COM object</summary>
        public void Dispose()
        {
            if (render is not null)
            {
                ComMarshalling.ReleaseInteropObject(render);
                render = null;
            }
            GC.SuppressFinalize(this);
        }
    }
}
