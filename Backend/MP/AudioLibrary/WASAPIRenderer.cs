
/*
 
Portions of code are from the NAudio project.

Copyright 2020 Mark Heath

Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
 
 */

using System;
using MP.Collections;
using MP.ComInterop;
using System.Threading;
using MP.AudioLibrary.WASAPI;
using System.Collections.Generic;

namespace MP.AudioLibrary
{
    /// <summary>
    /// This is the next-gen evolution of NAudio's WasapiOut. <br />
    /// All it's features do work correctly.
    /// </summary>
    public sealed class WASAPIRenderer : IAudioPlayer
    {
        [Flags]
        private enum STATEFLAGS : System.Byte
        {
            EventSync = 1,
            MMDeviceWasCreated = 2,
            DisposeCodeRuns = 4,
            DisposeCodeCompleted = 8,
            // Do Restart playback thread CTL code.
            // By default when the user dispatches Stop, it does just wait around itself until one of two actions do happen:
            // -> Dispose was called, which in this case the Audio Client must be released and destroyed
            // -> Play was called, which in such case the thread will resurrect the audio stream, in a possibly very small amount of time.
            // This because all the internal states are still held, provided that the playback provider is still valid of course.
            ThreadCC_DoRestart = 16,
            ThreadCC_ShutDownCmdRealized = 32,
            ThreadCC_FirstTime = 64,
        }

        private enum AudioBufferFillState : System.Byte
        {
            Completed,
            EndOfStream,
            Exception
        }

        private static void PlaybackStoppedDummyMethod(PlaybackStoppedEventInfo inf) { }

        private System.Int32 latms; // latency milliseconds
        private Thread playbackthread;
        private RenderingAudioClient ac;
        private IAudioProvider provider;
        private AudioFormat engineformat;
        private volatile STATEFLAGS state;
        private MMDevice.MMDevice mmdevice;
        private volatile PlaybackState current;
        private AUDCLNT_SHAREMODE sharemode;
        private AudioRenderingBuffer tempbuffer;
        private System.UInt32 bufferframesize, bufferblkalign;
        private EventWaitHandle eventsynchandle, threadcontrolhandle;

        public WASAPIRenderer(System.Boolean iseventsync , System.Int32 latency) : this(null , WASAPIMode.Shared , iseventsync , latency) { }

        public WASAPIRenderer(WASAPIMode mode , System.Boolean iseventsync , System.Int32 latency) : this(null , mode , iseventsync , latency) { }

        /// <summary>
        /// Creates a new WASAPI audio player with the specified audio device , the audio stream mode to use , a value 
        /// whether to handle playback using an event handle and a value in milliseconds that specifies the buffered data
        /// to store before a new I/O request is performed.
        /// </summary>
        /// <param name="mmdevice">The audio device to connect the current WASAPI renderer. Must be an MM Endpoint. If passed null , the renderer provides the default MM Device endpoint.</param>
        /// <param name="mode">The audio stream mode to initialize the current WASAPI renderer into.</param>
        /// <param name="iseventsync">A value whether playback buffering should be handled by a <see cref="EventWaitHandle"/>.</param>
        /// <param name="latency"></param>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="latency"/> was negative or zero, or very small.</exception>
        /// <exception cref="NotSupportedException">The specified audio rendering mode is not supported by WASAPI.</exception>
        public WASAPIRenderer(MMDevice.MMDevice mmdevice , WASAPIMode mode , System.Boolean iseventsync , System.Int32 latency)
        {
            state = 0;
            if (latency < 0) {
                throw new ArgumentOutOfRangeException(nameof(latency), "Latency cannot be negative.");
            }
            if (mmdevice is null) {
                this.mmdevice = GetDefaultAudioEndpoint();
                state |= STATEFLAGS.MMDeviceWasCreated;
            } else {
                this.mmdevice = mmdevice;
            }

            if (iseventsync) {
                state |= STATEFLAGS.EventSync;
            }
            sharemode = mode switch {
                WASAPIMode.Shared => AUDCLNT_SHAREMODE.AUDCLNT_SHAREMODE_SHARED,
                WASAPIMode.Exclusive => AUDCLNT_SHAREMODE.AUDCLNT_SHAREMODE_EXCLUSIVE,
                _ => throw new NotSupportedException($"Value of {mode} is not supported by the WASAPI renderer.")
            };
            latms = latency;
            eventsynchandle = null;
            current = PlaybackState.Stopped;
            threadcontrolhandle = new(false , EventResetMode.AutoReset);
            playbackthread = null;
            // Create the audio client. This is and one of the most critical stages.
            ac = RenderingAudioClient.FromMMDevice(this.mmdevice);
            // Create the event.
            PlaybackStopped = new(PlaybackStoppedDummyMethod);
            // Assign the output mix format as reported by WASAPI
            engineformat = ac.MixFormat;
        }

        /// <summary>
        /// Gets the default audio endpoint that WASAPI will prefer to use, when a specific device is not specified. <br />
        /// You must free the object after you successfully obtain it by using the <see cref="MMDevice.MMDevice.Dispose"/> method.
        /// </summary>
        public static MMDevice.MMDevice DefaultAudioEndpoint => GetDefaultAudioEndpoint();

        private static MMDevice.MMDevice GetDefaultAudioEndpoint()
        {
            var enumerator = new MMDevice.MMDeviceEnumerator();
            try {
                return enumerator.GetDefaultEndpoint(MMDevice.EDataFlow.Render, MMDevice.ERole.Console);
            } finally {
                enumerator?.Dispose();
                enumerator = null;
            }
        }

        // Adapted and used from NAudio's WasapiOut.
        private AudioFormat GetFallbackFormat()
        {
            var deviceSampleRate = ac.MixFormat.SampleRate;
            var deviceChannels = ac.MixFormat.ChannelLayout.Length; // almost certain to be stereo

            // we are in exclusive mode
            // First priority is to try the sample rate you provided.
            var sampleRatesToTry = new List<int>() { engineformat.SampleRate };
            // Second priority is to use the sample rate the device wants
            if (!sampleRatesToTry.Contains(deviceSampleRate)) sampleRatesToTry.Add(deviceSampleRate);
            // And if we've not already got 44.1 and 48kHz in the list, let's try them too
            if (!sampleRatesToTry.Contains(44100)) sampleRatesToTry.Add(44100);
            if (!sampleRatesToTry.Contains(48000)) sampleRatesToTry.Add(48000);

            var channelCountsToTry = new List<int>() { engineformat.ChannelLayout.Length };
            if (!channelCountsToTry.Contains(deviceChannels)) channelCountsToTry.Add(deviceChannels);
            if (!channelCountsToTry.Contains(2)) channelCountsToTry.Add(2);

            var bitDepthsToTry = new List<int>() { engineformat.BitRate };
            if (!bitDepthsToTry.Contains(32)) bitDepthsToTry.Add(32);
            if (!bitDepthsToTry.Contains(24)) bitDepthsToTry.Add(24);
            if (!bitDepthsToTry.Contains(16)) bitDepthsToTry.Add(16);

            AudioFormat fmt;

            foreach (var sampleRate in sampleRatesToTry)
            {
                foreach (var channelCount in channelCountsToTry)
                {
                    foreach (var bitDepth in bitDepthsToTry)
                    {
                        switch (bitDepth)
                        {
                            case 32:
                                fmt = AudioFormat.CreateIEEEFloat(sampleRate, channelCount);
                                break;
                            default:
                                fmt = AudioFormat.CreatePCM(sampleRate, bitDepth, channelCount);
                                break;
                        }
                        if (ac.IsFormatSupported(sharemode, fmt , out _))
                            return fmt;
                    }
                }
            }
            throw new NotSupportedException("Can't find a supported format to use");
        }

        private unsafe void ThreadCode()
        {
            // For a first start we need the AudioRenderClient. Create it, so.
            AudioRenderClient arc;
            try {
                arc = ac.GetRenderClient();
            } catch (Exception ex) {
                // Report the exception, and exit.
                provider.Dispose();
                PlaybackStopped.Invoke(new(PlaybackStoppedReason.Exception , ex));
                return;
            }

            // HRESULT instance used by the thread code
            HRESULT hr; 

            // Load the first buffer from the stream.
            // The buffer size must be around GetBufferSize() * BlockAlign

            System.UInt32 psize;
            hr = ac.GetBufferSize(out psize);
            if (hr.FAILED) {
                // Call failed, free AudioRenderClient, report the exception and return.
                arc.Dispose();
                provider.Dispose();
                PlaybackStopped.Invoke(new(PlaybackStoppedReason.Exception , hr.CreateException()));
                return;
            }

            bufferframesize = psize;
            bufferblkalign = provider.Format.BlockAlignment.ToUInt32();

            // Use AudioRenderingBuffer to manage the translation buffer
            tempbuffer = new(bufferframesize * bufferblkalign);

            AudioBufferFillState s1;

            // Add event sync handle and thread instance control handle to be checked.
            WaitHandle[] waithandles = new[] { eventsynchandle ?? new EventWaitHandle(false, EventResetMode.AutoReset), threadcontrolhandle };

            // Compute desired latency value
            System.Int32 enforcedlatency = ((state & STATEFLAGS.EventSync) == STATEFLAGS.EventSync) ? 3 * latms : latms / 2;

            // The first time that the thread enters this loop it should just get to the stream end state and must not fire any events, except if playback has finished on an exception was occured.
            state |= STATEFLAGS.ThreadCC_FirstTime;

        G_RESTART:
            // Read the first buffer from the stream as-is.
            // This will also surface and IAudioProvider programming mistakes.
            s1 = FillAudioBuffer(provider, arc);
            switch (s1)
            {
                case AudioBufferFillState.EndOfStream:
                    // Propagated end of stream, we should do nothing else but free resources and exit.
                    // Stop and destroy stream if possible
                    PlaybackStopped.Invoke(new(PlaybackStoppedReason.EndOfStream));
                    goto G_STREAMEND;
                case AudioBufferFillState.Exception:
                    // An exception was already occured, the exception was injected to the playback stopped event
                    goto G_DISPOSE_FAIL_NO_HR;
            }
            // Start audio device
            hr = ac.Start();
            if (hr.FAILED) { goto G_DISPOSE_FAIL_HR; }
            while (current != PlaybackState.Stopped && s1 == AudioBufferFillState.Completed)
            {
                // Playback is still running.
                // Wait half latency on non event-sync or 3 * latency with the event sync handle if applicable
                // If case 0 is hit, which is the event-sync handle , we will just continue execution, as it is elsewise expected...
                if (WaitHandle.WaitAny(waithandles, enforcedlatency, false) == 1
                    && state.HasFlag(STATEFLAGS.DisposeCodeRuns))
                {
                    // The user requests to dispose the object, destroy immediately.
                    ac.Stop();
                    ac.Reset();
                    arc.Dispose();
                    waithandles.DisposeAll();
                    provider.Dispose();
                    // Clearly this is a user request.
                    current = PlaybackState.Stopped;
                    PlaybackStopped.Invoke(new(PlaybackStoppedReason.UserRequest));
                    return;
                }
                if (current == PlaybackState.Playing) // Still playback is valid
                {
                    // Fetch next buffer from the audio stream.
                    s1 = FillAudioBuffer(provider, arc);
                }
            }
            System.Boolean eos = false;
            // We have broken the loop for a reason , check why.
            if (current == PlaybackState.Playing && s1 == AudioBufferFillState.EndOfStream)
            {
                // OK. we have reached the end of stream.
                // Theoritically, the player still plays, so give a window to process all the data.
                WaitHandle.WaitAny(waithandles, enforcedlatency, false);
                eos = true;
            }
            if (s1 == AudioBufferFillState.Exception)
            {
                // An exception was occured, we have no other choice than destroying the playback thread.
                ac.Stop(); // If it can happen
                ac.Reset();
                goto G_DISPOSE_FAIL_NO_HR;
            }
            // If applicable , flush the buffers as requested by the user.
            // Additionally, fire the playback stopped event now.
            hr = ac.Stop(); // Stop WASAPI
            if (hr.FAILED) { goto G_DISPOSE_FAIL_HR; }
            hr = ac.Reset(); // Reset WASAPI device position
            if (hr.FAILED) { goto G_DISPOSE_FAIL_HR; }
            if (eos) {
                PlaybackStopped.Invoke(new(PlaybackStoppedReason.EndOfStream));
                current = PlaybackState.Stopped;
            } else if ((state & STATEFLAGS.ThreadCC_FirstTime) == 0) {
                PlaybackStopped.Invoke(new(PlaybackStoppedReason.UserRequest));
            } else {
                state &= ~STATEFLAGS.ThreadCC_FirstTime;
            }
            G_STREAMEND:
            // We have entered the state where the 'thread must halt'.
            // Halting the thread allows us to re-use the thread and all the initialized data.
            // The only overhead existing will be that of the wait op itself.

            // Wait for handle 1.
            // If the event sync handle accidentally fires, just gracefully eat it and re-enter the loop
            if (WaitHandle.WaitAny(waithandles) != 1) { goto G_STREAMEND; } 

            // Check for Restart flag.
            if ((state & STATEFLAGS.ThreadCC_DoRestart) == STATEFLAGS.ThreadCC_DoRestart)
            {
                // This means that Stop was dispatched so just re-start playback.
                // Clear flag before resurrecting.
                state &= ~STATEFLAGS.ThreadCC_DoRestart;
                goto G_RESTART;
            }

            state |= STATEFLAGS.ThreadCC_ShutDownCmdRealized;

            // Otherwise, just safely dispose and exit.
            // No need to fire the PlaybackStopped event since that had been done at the proper time before.
            arc.Dispose();
            waithandles.DisposeAll();
            provider.Dispose();
            return;
        G_DISPOSE_FAIL_NO_HR:
            arc.Dispose();
            waithandles.DisposeAll();
            provider.Dispose();
            current = PlaybackState.Stopped;
            return;
        G_DISPOSE_FAIL_HR:
            arc.Dispose();
            waithandles.DisposeAll();
            provider.Dispose();
            current = PlaybackState.Stopped;
            PlaybackStopped.Invoke(new(PlaybackStoppedReason.Exception, hr.CreateException()));
            return;
        }

        // Fills the audio buffer 'tempbuffer' with audio data from the provided audio provider,
        // and passes those to the actual rendering device.
        // Then, it does return a code back indicating three distinct states:
        // Completed: Operation completed successfully, more data may exist in next calls.
        // Exception: An exception was occured. Operation was cancelled and it's error was returned to the event.
        // EndOfStream: The stream was ended. The caller must start shutting down.
        private unsafe AudioBufferFillState FillAudioBuffer(IAudioProvider provactual , AudioRenderClient arc)
        {
            // Get any additional padding required - this is dynamic so it must be called multiple times.
            System.UInt32 padding;
            HRESULT hr = ac.GetCurrentPadding(out padding);
            if (hr.FAILED)
            {
                PlaybackStopped.Invoke(new(PlaybackStoppedReason.Exception, hr.CreateException()));
                return AudioBufferFillState.Exception;
            }

            System.UInt32 availframes = bufferframesize - padding;
            if (availframes < 11)
            {
                // Expect at least 10 frames to have been processed by the audio engine.
                // If these are not processed yet, just return with success, waiting to fetch to the next buffering cycle.
                // If we allowed that to happen, we could max out CPU usage without any particular reason.
                // Additionally, this implements the Pause logic.
                return AudioBufferFillState.Completed;
            }
            System.Int32 totalreadlength = (availframes * bufferblkalign).ToInt32();

            // Attempt to read from the audio provider.
            System.Int32 br;
            // Create try layer on the fly, only for the provider call.
            try {
                br = provactual.Read(tempbuffer.Buffer, 0, totalreadlength);
            } catch (Exception ex) {
                PlaybackStopped.Invoke(new(PlaybackStoppedReason.Exception, ex));
                return AudioBufferFillState.Exception;
            }

            if (br < 1) {
                // Audio stream read to completion - stop.
                return AudioBufferFillState.EndOfStream;
            }

            // From AudioRenderClient now, get the buffer pointer to write in the first block.
            hr = arc.GetBuffer(availframes, out System.Byte* pbuf);
            if (hr.FAILED) {
                PlaybackStopped.Invoke(new(PlaybackStoppedReason.Exception, hr.CreateException()));
                return AudioBufferFillState.Exception;
            }

            tempbuffer.CopyTo(pbuf, br.ToUInt32());

            // First check for exclusiveness, then for the EventSync flag.
            if (sharemode == AUDCLNT_SHAREMODE.AUDCLNT_SHAREMODE_EXCLUSIVE && 
                (state & STATEFLAGS.EventSync) == STATEFLAGS.EventSync) {
                // We have requested availframes, but we do not have that many.
                // Because we are in exclusive mode, we are ought to at least fill these with zeroes.
                if (br < totalreadlength) { tempbuffer.Zeroize(br, (totalreadlength - br).ToUInt32()); }
                hr = arc.ReleaseBuffer(availframes, 0);
            } else {
                // We do not need to zeroize the rest of the buffer, if it is not already zeroized.
                hr = arc.ReleaseBuffer((br / bufferblkalign).ToUInt32() , 0);
            }
            // Now check for HRESULT too
            if (hr.FAILED)
            {
                // The call failed, we need to retrieve exception and return it
                PlaybackStopped.Invoke(new(PlaybackStoppedReason.Exception, hr.CreateException()));
                return AudioBufferFillState.Exception;
            }
            // If succeeded , just return success.
            return AudioBufferFillState.Completed;
        }

        public System.Boolean Initialize(IAudioProvider provider)
        {
            ArgumentNullException.ThrowIfNull(provider);
            if (this.provider is not null) { return false; }
            this.provider = provider;
            // Provide as the engine format the IAudioProvider's one.
            engineformat = this.provider.Format;

            AUDCLNT_STREAMFLAGS flags = 0;

            if (sharemode == AUDCLNT_SHAREMODE.AUDCLNT_SHAREMODE_EXCLUSIVE)
            {
                // Exclusive mode initialization.
                // The resampler here must be explicitly created, and we also need to check for IsFormatSupported here.
                if (ac.IsFormatSupported(sharemode, engineformat, out AudioFormat af)) 
                {
                    // If the audio format returns a non-null value, the source format is not supported exactly , and we need the resampler

                    if (af is not null) {
                        engineformat = af;
                        this.provider = new MediaFoundationMFTResampler(provider, engineformat, latms);
                    }
                } else {
                    return false;
                }
            }

            if (state.HasFlag(STATEFLAGS.EventSync))
            {
                // The user requires EventSync.
                // Additional initailization code must run.

                if (sharemode == AUDCLNT_SHAREMODE.AUDCLNT_SHAREMODE_SHARED)
                {
                    // On Shared mode , just set the flag, and correct the latency.

                    // With EventCallBack and Shared, both latencies must be set to 0 (update - not sure this is true anymore)
                    // 
                    ac.Initialize(sharemode, AUDCLNT_STREAMFLAGS.EVENTCALLBACK | flags, latms, 0, engineformat, Guid.Empty);

                    // Windows 10 returns 0 from stream latency, resulting in maxing out CPU usage later
                    REFERENCE_TIME latency;
                    ac.GetStreamLatency(out latency).ThrowOnFailure();
                    if (latency != 0)
                    {
                        // Get back the effective latency from AudioClient
                        latms = (System.Int32)latency.ToMilliseconds();
                    }
                } else {
                    try {
                        // With EventCallBack and Exclusive, both latencies must be equal.
                        ac.Initialize(sharemode, AUDCLNT_STREAMFLAGS.EVENTCALLBACK | flags, latms, latms, engineformat, Guid.Empty);
                    } catch (ExceptionSystem.NativeWindowsCOMException ex) {
                        // Starting with Windows 7, Initialize can return AUDCLNT_E_BUFFER_SIZE_NOT_ALIGNED for a render device.
                        // We should initialize again.
                        if (ex.ErrorCode != WASAPIErrorCodes.AUDCLNT_E_BUFFER_SIZE_NOT_ALIGNED)
                            throw;

                        System.UInt32 bfs;
                        ac.GetBufferSize(out bfs).ThrowOnFailure();

                        // Calculate the new latency.
                        System.Int32 newLatencyRefTimes = (System.Int32)(new REFERENCE_TIME((long)(10000000.0 / engineformat.SampleRate * bfs + 0.5)).ToMilliseconds());

                        ac.Dispose();
                        ac = RenderingAudioClient.FromMMDevice(mmdevice);
                        ac.Initialize(sharemode, AUDCLNT_STREAMFLAGS.EVENTCALLBACK | flags, newLatencyRefTimes, newLatencyRefTimes, engineformat, Guid.Empty);
                    }
                }

                eventsynchandle = new(false, EventResetMode.AutoReset);
                ac.SetEventHandle(eventsynchandle);
            }
            else
            {
                // Setup code for both Shared and Exclusive modes.
                ac.Initialize(sharemode, flags, latms, 0, engineformat, Guid.Empty);
            }

            playbackthread = new(ThreadCode);
            playbackthread.IsBackground = true;
            playbackthread.Name = "[MP] WASAPI Renderer Thread";
            playbackthread.TrySetApartmentState(ApartmentState.MTA);
            playbackthread.Start();

            return true;
        }

        /// <summary>
        /// Gets a value whether the specified audio format is directly, or not, supported by the current WASAPI renderer.
        /// </summary>
        /// <param name="format">The audio format to be queried.</param>
        /// <param name="closestformat">If the audio format in <paramref name="format"/> is not supported and this 
        /// parameter returns non-null value, then it can be used to resample between the source audio provider and the WASAPI renderer.</param>
        /// <returns>A value whether the format specified in <paramref name="format"/> is supported by WASAPI.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="format"/> was <see langword="null"/>.</exception>
        /// <exception cref="ObjectDisposedException">The underlying object has been disposed.</exception>
        public System.Boolean IsFormatSupported(AudioFormat format , out AudioFormat closestformat)
        {
            ObjectDisposedException.ThrowIf(ac is null, this);
            ArgumentNullException.ThrowIfNull(format);
            return ac.IsFormatSupported(sharemode, format , out closestformat);
        }

        /// <summary>
        /// Stops rendering. None of the buffers is flushed.
        /// </summary>
        public void Pause()
        {
            ObjectDisposedException.ThrowIf(ac is null, this);
            current = PlaybackState.Paused;
        }

        /// <summary>
        /// Re-starts playback from any of the two other cases.
        /// </summary>
        public void Play()
        {
            ObjectDisposedException.ThrowIf(ac is null, this);
            switch (current)
            {
                case PlaybackState.Paused:
                    current = PlaybackState.Playing;
                    break;
                case PlaybackState.Stopped:
                    current = PlaybackState.Playing;
                    // Request stream resurrection.
                    state |= STATEFLAGS.ThreadCC_DoRestart;
                    threadcontrolhandle.Set();
                    break;
            }
        }

        /// <summary>
        /// Stops rendering. All the buffers are flushed.
        /// </summary>
        public void Stop()
        {
            ObjectDisposedException.ThrowIf(ac is null, this);
            current = PlaybackState.Stopped;
        }

        /// <summary>
        /// Gets or sets the time that WASAPI must wait until the next I/O request is performed. <br />
        /// Cannot be modified after <see cref="Initialize(IAudioProvider)"/> has been called successfully.
        /// </summary>
        /// <returns>The time that WASAPI must wait until the next I/O request.</returns>
        /// <exception cref="InvalidOperationException">Attempted to modify the latency after <see cref="Initialize(IAudioProvider)"/> was called.</exception>
        /// <exception cref="ArgumentOutOfRangeException">New requested latency was negative, while this is not allowed.</exception>
        public System.Int32 Latency
        {
            get => latms;
            set {
                if (provider is not null) {
                    throw new InvalidOperationException("Cannot change the latency after an audio provider is registered!");
                }
                if (value < 0) {
                    throw new ArgumentOutOfRangeException(nameof(value), "Latency cannot be negative.");
                }
                latms = value;
            }
        }

        /// <summary>Gets the current playback state of the audio renderer.</summary>
        public PlaybackState State => current;

        /// <summary>
        /// Gets the audio format the audio device hardware is currently using. <br />
        /// Can be retrieved even after <see cref="Dispose"/> is called on the object.
        /// </summary>
        public AudioFormat OutputFormat => engineformat;

        /// <summary>
        /// Gets audio session volume controls for shared audio streams.
        /// </summary>
        /// <exception cref="InvalidOperationException">
        /// This is thrown when an exclusive audio stream is being used.
        /// </exception>
        public ChannelAudioVolume ChannelAudioVolume
        {
            get {
                ObjectDisposedException.ThrowIf(ac is null, this);
                if (sharemode == AUDCLNT_SHAREMODE.AUDCLNT_SHAREMODE_EXCLUSIVE)
                {
                    throw new InvalidOperationException("ChannelAudioVolume is only available for shared audio sessions.");
                }
                return ac.ChannelAudioVolume;
            }
        }

        /// <summary>
        /// The event that is fired when the playback is stopped. <br /> 
        /// It additionally provides to the user why playback
        /// was stopped, and if an error occured, the error message.
        /// </summary>
        public event PlaybackStoppedDelegate PlaybackStopped;

        /// <summary>
        /// Disposes this <see cref="WASAPIRenderer"/> class instance.
        /// </summary>
        public void Dispose()
        {
            if (state.HasFlag(STATEFLAGS.DisposeCodeRuns))
            {
                while (state.HasFlag(STATEFLAGS.DisposeCodeCompleted) == false)
                {
                    // Wait until the Dispose code completes, then exit.
                    Thread.Sleep(100);
                }
                return;
            }
            state |= STATEFLAGS.DisposeCodeRuns;
            try {
                DisposeCodeCommon();
            } finally {
                state |= STATEFLAGS.DisposeCodeCompleted;
            }
        }

        private void DisposeCodeCommon()
        {
            if (ac is not null)
            {
                // Signal for the thread to be destroyed, and wait for it to exit.
                state &= ~STATEFLAGS.ThreadCC_DoRestart; // Clear restart flag to avoid accidental repetitions.
                if (playbackthread is not null && playbackthread.IsAlive)
                {
                    // Destroy the playback thread.
                    try {
                        do {
                            threadcontrolhandle.Set();
                        } while (playbackthread.Join(70) == false && state.HasFlag(STATEFLAGS.ThreadCC_ShutDownCmdRealized) == false);
                    } catch (ObjectDisposedException) { }
                }
                playbackthread = null;
                // We can now dispose the AudioClient.
                ac?.Dispose();
                ac = null;
                // Destroy rendering buffer.
                tempbuffer = null;
                // If the MMDevice provided is created by the renderer, we must dispose that too
                if (state.HasFlag(STATEFLAGS.MMDeviceWasCreated) && mmdevice is not null)
                {
                    mmdevice.Dispose();
                    mmdevice = null;
                }
                // Done!
            }
        }
    }
}