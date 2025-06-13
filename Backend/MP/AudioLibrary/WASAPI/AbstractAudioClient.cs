

using MP.ComInterop;
using System;

namespace MP.AudioLibrary.WASAPI
{
    /// <summary>
    /// Defines the base class for both audio clients: The capture and the render audio client.
    /// </summary>
    public unsafe abstract class AbstractAudioClient : IDisposable
    {
        private IAudioClient audioclient;
        private ChannelAudioVolume cav;
        private AudioClockClient clockclient;

        protected AbstractAudioClient(IAudioClient audioclient)
        {
            ArgumentNullException.ThrowIfNull(audioclient);
            this.audioclient = audioclient;
            cav = null;
        }

        /// <summary>
        /// Gets the native COM object. <br />
        /// You may want to use it for doing <strong>QueryInterface</strong> on it.
        /// </summary>
        protected IAudioClient AudioClientNative => audioclient;

        /// <summary>Queries whether the specified audio format is supported by the WAS.</summary>
        /// <remarks>
        /// To find out what to do in each case, follow the following truth table: <br />
        /// <list type="table">
        ///     <listheader>
        ///         <term>Return Value</term>
        ///         <term><paramref name="closestformat"/> is <see langword="null"/>?</term>
        ///         <description>Action</description>
        ///     </listheader>
        ///     <item>
        ///         <term><see langword="true"/></term>
        ///         <term>Yes</term>
        ///         <description>Format passed by <paramref name="format"/> is fully supported, just hook up the source to the engine.</description>
        ///     </item>
        ///     <item>
        ///         <term><see langword="true"/></term>
        ///         <term>No</term>
        ///         <description>
        ///         Format passed by <paramref name="format"/> is partially supported. 
        ///         A resampler between the source and the engine may be needed. 
        ///         The resampler must produce results based on <paramref name="closestformat"/>.
        ///         </description>
        ///     </item>
        ///     <item>
        ///         <term><see langword="false"/></term>
        ///         <term>Does not matter</term>
        ///         <description>The format specified in <paramref name="format"/> is NOT supported.</description>
        ///     </item>
        /// </list> <br />
        /// Note that for the second case of the table that you must be prepared for hooking up the resampler , if finally the <see cref="Initialize"/> call fails. <br />
        /// In all other cases, you will possibly not need the resampler.
        /// </remarks>
        /// <param name="sharemode">The share mode that the session desired to be created will be into.</param>
        /// <param name="format">The audio format of the session.</param>
        /// <param name="closestformat">The closest audio format that the engine exactly needs, if the format is pseudo-supported.</param>
        /// <returns>A value whether the specified audio format is supported or not.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="format"/> was <see langword="null"/>.</exception>
        /// <exception cref="NotSupportedException">Invalid <see cref="AUDCLNT_SHAREMODE"/> was selected.</exception>
        /// <exception cref="ExceptionSystem.NativeWindowsCOMException">An error occured.</exception>
        public System.Boolean IsFormatSupported(AUDCLNT_SHAREMODE sharemode , AudioFormat format , out AudioFormat closestformat)
        {
            ArgumentNullException.ThrowIfNull(format);
            WAVEFORMATEX* pfmt = null;
            WAVEFORMATEXTENSIBLE finput = format.ConvertTo<WAVEFORMATEXTENSIBLE>();
            HRESULT hr;
            switch (sharemode)
            {
                case AUDCLNT_SHAREMODE.AUDCLNT_SHAREMODE_SHARED:
                    hr = audioclient.IsFormatSupported(sharemode, (WAVEFORMATEX*)&finput, &pfmt);
                    break;
                case AUDCLNT_SHAREMODE.AUDCLNT_SHAREMODE_EXCLUSIVE:
                    hr = audioclient.IsFormatSupported(sharemode, (WAVEFORMATEX*)&finput, null);
                    break;
                default:
                    throw new NotSupportedException($"Option is not yet supported: {sharemode}");
            }
            switch (hr)
            {
                case CommonHResults.S_OK:
                    closestformat = null;
                    return true;
                case CommonHResults.S_FALSE:
                    if (pfmt->Tag == WAVEFORMATTAG.Extensible) {
                        closestformat = AudioFormatConverter.ConvertFrom(*(WAVEFORMATEXTENSIBLE*)pfmt);
                    } else {
                        closestformat = AudioFormatConverter.ConvertFrom(*pfmt);
                    }
                    Interop.Ole32.CoTaskMemFree(pfmt);
                    pfmt = null;
                    return true;
                case WASAPIErrorCodes.AUDCLNT_E_UNSUPPORTED_FORMAT:
                    // On Shared mode, it is supported though.
                    if (sharemode == AUDCLNT_SHAREMODE.AUDCLNT_SHAREMODE_SHARED && pfmt is not null) {
                        goto case CommonHResults.S_FALSE;
                    }
                    // Otherwise fail.
                    Interop.Ole32.CoTaskMemFree(pfmt);
                    pfmt = null;
                    closestformat = null;
                    return false;
                default:
                    throw new ExceptionSystem.NativeWindowsCOMException(hr);
            }
        }

        /// <summary>
        /// The <see cref="MixFormat"/> property retrieves the stream format that the audio engine uses for its internal processing of shared-mode streams. <br />
        /// Can be called before actual initialization.
        /// </summary>
        public AudioFormat MixFormat
        {
            get {
                WAVEFORMATEX* ppwf;
                var hr = audioclient.GetMixFormat(&ppwf);
                hr.ThrowOnFailure();
                AudioFormat af;
                try {
                    if (ppwf->Tag == WAVEFORMATTAG.Extensible) {
                        af = AudioFormatConverter.ConvertFrom(*(WAVEFORMATEXTENSIBLE*)ppwf);
                    } else {
                        af = AudioFormatConverter.ConvertFrom(*ppwf);
                    }
                } finally {
                    // Reliably free the ppwf pointer, even on failure.
                    Interop.Ole32.CoTaskMemFree(ppwf);
                }
                return af;
            }
        }

        public void Initialize(AUDCLNT_SHAREMODE sharemode , AUDCLNT_STREAMFLAGS sf , System.Int32 bufferdurationinms , System.Int32 periodicityinms , AudioFormat fmt , Guid sessionclassguid = default)
        {
            REFERENCE_TIME bufduration = REFERENCE_TIME.FromMilliseconds(bufferdurationinms);
            REFERENCE_TIME periodicity = REFERENCE_TIME.FromMilliseconds(periodicityinms);
            WAVEFORMATEXTENSIBLE ext = fmt.ConvertTo<WAVEFORMATEXTENSIBLE>();
            WindowsInterop.GUID clguid = WindowsInterop.GUID.FromGUID(sessionclassguid);
            HRESULT hr = audioclient.Initialize(sharemode, sf, bufduration, periodicity, (WAVEFORMATEX*)&ext, &clguid);
            switch (hr)
            {
                case WASAPIErrorCodes.AUDCLNT_E_DEVICE_INVALIDATED:
                    throw new AudioDeviceDisconnectedException();
                case WASAPIErrorCodes.AUDCLNT_E_ALREADY_INITIALIZED:
                    throw new AudioSessionAlreadyInitializedException();
                default:
                    hr.ThrowOnFailure();
                    break;
            }
        }

        public HRESULT GetDevicePeriod(out REFERENCE_TIME defaultperiodicity , out REFERENCE_TIME minimalperiodicity)
        {
            REFERENCE_TIME rft1, rft2;
            HRESULT hr = audioclient.GetDevicePeriod(&rft1 , &rft2);
            defaultperiodicity = rft1;
            minimalperiodicity = rft2;
            return hr;
        }

        public HRESULT GetStreamLatency(out REFERENCE_TIME latency)
        {
            REFERENCE_TIME rft;
            HRESULT hr = audioclient.GetStreamLatency(&rft);
            latency = rft;
            return hr;
        }

        public HRESULT GetBufferSize(out System.UInt32 frames)
        {
            System.UInt32 fs;
            HRESULT hr = audioclient.GetBufferSize(&fs);
            frames = fs;
            return hr;
        }

        public HRESULT GetCurrentPadding(out System.UInt32 framespadding)
        {
            System.UInt32 fs;
            HRESULT hr = audioclient.GetCurrentPadding(&fs);
            framespadding = fs;
            return hr;
        }

        public HRESULT Start() => audioclient.Start();

        public HRESULT Stop() => audioclient.Stop();

        public HRESULT Reset() => audioclient.Reset();

        /// <summary>
        /// Required for using EventSync.
        /// </summary>
        /// <param name="ewh">The event handle to use to do EventSync.</param>
        /// <exception cref="ArgumentNullException"><paramref name="ewh"/> was <see langword="null"/>.</exception>
        public void SetEventHandle(System.Threading.EventWaitHandle ewh)
        {
            ArgumentNullException.ThrowIfNull(ewh);
            audioclient.SetEventHandle(ewh.SafeWaitHandle.DangerousGetHandle()).ThrowOnFailure();
        }

        /// <summary>
        /// Retrieves a COM object that acts as an additional feature on the current <see cref="AbstractAudioClient"/> class.
        /// </summary>
        /// <param name="interfaceid">The interface GUID of the interface you want to retrieve.</param>
        /// <param name="obj">The retrieved COM object. You are responsible of freeing it in the <see cref="Dispose(bool)"/> method.</param>
        /// <returns>An error code. Handle the code appropriately.</returns>
        protected HRESULT GetService(Guid interfaceid , out System.Object obj)
        {
            obj = null;
            void* pi;
            WindowsInterop.GUID g = WindowsInterop.GUID.FromGUID(interfaceid);
            HRESULT hr = audioclient.GetService(&g, &pi);
            if (hr.SUCCEEDED) {
                obj = ComMarshalling.CreateInteropObject(pi, -1);
            }
            return hr;
        }

        /// <summary>Gets the shared stream audio volume controls.</summary>
        public ChannelAudioVolume ChannelAudioVolume
        {
            get {
                if (cav is null)
                {
                    HRESULT hr = GetService(new(WASAPIInterfaceIds.IID_IChannelAudioVolume), out System.Object c);
                    if (hr == WASAPIErrorCodes.AUDCLNT_E_NOT_INITIALIZED) {
                        throw new AudioSessionNotInitializedException();
                    }
                    hr.ThrowOnFailure();
                    cav = new(c as IChannelAudioVolume);
                }
                return cav;
            }
        }

        /// <summary>
        /// Gets the audio clock client for this audio session. The audio session must have been created first.
        /// </summary>
        public AudioClockClient AudioClockClient
        {
            get {
                if (clockclient is null)
                {
                    System.Object o;
                    // If IAudioClock2 is available, use it.
                    HRESULT hr = GetService(new(WASAPIInterfaceIds.IID_IAudioClock2), out o);
                    switch (hr)
                    {
                        // E_NOT_INITIALIZED, throw the appropriate exception.
                        case WASAPIErrorCodes.AUDCLNT_E_NOT_INITIALIZED:
                            throw new AudioSessionNotInitializedException();
                        case WASAPIErrorCodes.AUDCLNT_E_DEVICE_INVALIDATED:
                            throw new AudioDeviceDisconnectedException();
                        case CommonHResults.E_NOINTERFACE:
                            // IAudioClock2 is not available, see if we can use the IAudioClock instead.
                            hr = GetService(new(WASAPIInterfaceIds.IID_IAudioClock), out o);
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
                            break;
                    }
                    clockclient = new(o as IAudioClock);
                }
                return clockclient;
            }
        }

        protected virtual void Dispose(System.Boolean disposing) 
        {
            if (disposing && audioclient is not null) 
            {
                clockclient?.Dispose();
                audioclient = null;
                cav?.Dispose();
                cav = null;
                ComMarshalling.ReleaseInteropObject(audioclient);
                audioclient = null;
            }
        }

        public void Dispose() 
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~AbstractAudioClient() => Dispose(false);
    }
}