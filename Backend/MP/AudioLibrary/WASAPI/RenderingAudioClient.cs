

using System;
using MP.ComInterop;

namespace MP.AudioLibrary.WASAPI
{
    /// <summary>
    /// Defines the audio client that can render data into the provided audio device. <br />
    /// Extends the <see cref="AbstractAudioClient"/> class.
    /// </summary>
    public sealed class RenderingAudioClient : AbstractAudioClient
    {
        private RenderingAudioClient(IAudioClient ac) : base(ac) { }

        public static RenderingAudioClient FromMMDevice(MMDevice.MMDevice device)
        {
            ArgumentNullException.ThrowIfNull(device);
            if (device.DataFlow == MMDevice.EDataFlow.Capture) {
                throw new ArgumentException("The target MMDevice must be a rendering device.");
            }
            System.Object cobj = null;
            try {
                // Attempt first to get IAudioClient2.
                cobj = device.Activate(new Guid(WASAPIInterfaceIds.IID_IAudioClient2));
            } catch (NotSupportedException) {
                // If that query fails, use IAudioClient.
                // If this fails too, just throw the exception as is
                cobj = device.Activate(new Guid(WASAPIInterfaceIds.IID_IAudioClient));
            }
            // Cast as IAudioClient. If needed by an application, it can cast to IAudioClient2 at run-time.
            return new(cobj as IAudioClient);
        }

        /// <summary>
        /// Gets an object that can access the underlying audio device for providing data. <br />
        /// This object is not tracked by <see cref="RenderingAudioClient"/>; You should dispose it when you finish with it.
        /// </summary>
        /// <returns>A new <see cref="AudioRenderClient"/> instance.</returns>
        /// <exception cref="AudioDeviceDisconnectedException">The audio device was disconnected.</exception>
        public AudioRenderClient GetRenderClient()
        {
            HRESULT hr = GetService(new(WASAPIInterfaceIds.IID_IAudioRenderClient), out var o);
            switch (hr)
            {
                case WASAPIErrorCodes.AUDCLNT_E_NOT_INITIALIZED:
                    throw new AudioSessionNotInitializedException();
                case WASAPIErrorCodes.AUDCLNT_E_DEVICE_INVALIDATED:
                    throw new AudioDeviceDisconnectedException();
                default:
                    hr.ThrowOnFailure();
                    break;
            }
            return new(o as IAudioRenderClient);
        }
    }
}