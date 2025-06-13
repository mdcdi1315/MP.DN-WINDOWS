using System;
using MP.AudioLibrary;
using MP.ExtSystemApi;
using MP.ExtensibilitySystem;
using MP.AudioLibrary.MMDevice;
using System.Runtime.CompilerServices;

namespace MP
{
    /// <summary>
    /// Wrapper class for constructing and using a player instance for the specified track. <br />
    /// It does almost everything; Just use it's methods to start the playback...
    /// </summary>
    public sealed class PlayerInstance : AbstractPlayerInstance
    {
        [Flags]
        private enum PlayerInstanceFlags : System.Byte
        {
            None = 0,
            RepeatRoutineRegistered = 1,
            Created = 2,
            Disposed = 4,
            RunMDUThread = 8,
            LatencyLow = 16,
            OGGInternalActive = 32
        }
        
        private InstanceData temp;
        private ExtensionEngine eng;
        private WASAPIRenderer outdev;
        private PlayerInstanceFlags flags;
        private CodecProbeRequestResult codec;
        private System.Threading.Thread backthread;

        public PlayerInstance(InstanceData Data, System.Boolean internaloggvactive , ExtensionEngine eng = null) : base()
        { 
            temp = Data;
            this.eng = eng;
            System.String ext = temp.Stream.GetStringAttribute("FileName");
            if (temp.Stream is not MusicPlayerStream)
            {
                throw new InvalidOperationException("The only accepted property streams are those that are objects from the MusicPlayerStream class.");
            }
            backthread = null;
            UpdateMusicDisplay = new(UpdateMusicDisplayDummyFunction);
            flags = PlayerInstanceFlags.None;
            if (internaloggvactive)
            {
                flags |= PlayerInstanceFlags.OGGInternalActive;
            }
        }

        private static void RepeatRoutineInternal(System.Object obj)
        {
            PlayerInstance pi = obj as PlayerInstance;
            while (pi.outdev.State != PlaybackState.Stopped) { System.Threading.Thread.Sleep(100); }
            pi.Play();
            pi.OnRepeatRequestSuccessfull();
        }

        private void RepeatRoutine(PlaybackStoppedEventInfo e)
        {
            switch (e.Reason)
            {
                case PlaybackStoppedReason.Exception:
                    switch (e.Exception)
                    {
                        case SpecialStopButtonAssertionException:
                            return;
                        case not null:
                            MusicPlayerHelper.ShowErrorMessage($"Music player failed and stopped due to an exception: \n{e.Exception}");
                            return;
                    }
                    break;
                case PlaybackStoppedReason.EndOfStream:
                    codec.AudioStream.CurrentTime = new TimeSpan(0, 0, 0);
                    backthread?.Join();
                    System.Threading.Thread td = new(new System.Threading.ParameterizedThreadStart(RepeatRoutineInternal));
                    td.Name = "[MP] Repeat playback thread";
                    td.IsBackground = true;
                    td.Start(this);
                    break;
            }
        }

        private static void UpdateMusicDisplayDummyFunction(System.Object obj , UpdateMusicDisplayEventArgs e) { }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private System.Boolean HasFlagFast(PlayerInstanceFlags flags) => (this.flags & flags) == flags;

        private void MDU_RemoveFlagAndVerifyClosed()
        {
            // This forces it to unset the flag actually.
            // We do not have to check if the flag is set;
            // if the flag is unset just it will keep the flags state same.
            flags &= ~PlayerInstanceFlags.RunMDUThread;
            // Possibly the dispatcher is involved with this so we cannot enable it.
            // while (backthread is not null && backthread.IsAlive) { System.Threading.Thread.Sleep(10); }
        }

        private void MDUThreadCode()
        {
            // Now the total time is cached because is standard, 
            // and the MDU is using now a predicting model to predict the next time interval shown.
            // This is useful when the latency is high, so that the values returned are approximate values.
            DebugProvider.WriteLine("RCU: Starting MDU thread ...");
            System.TimeSpan total = TotalTrackTime , readertime , 
                predictingnextvalue = HasFlagFast(PlayerInstanceFlags.LatencyLow) ? TimeSpan.MaxValue : TimeSpan.Zero;
            TimeSpan predicttime = new(558700); // Around 55.87 ms
            try {
                flags |= PlayerInstanceFlags.RunMDUThread;
                UpdateMusicDisplayEventArgs evsinst = new();
                DebugProvider.WriteLine($"RCU: MDU Thread: Music Data Updater Thread with id {System.Threading.Thread.CurrentThread.ManagedThreadId} was started.");
                while (HasFlagFast(PlayerInstanceFlags.RunMDUThread) && 
                    HasFlagFast(PlayerInstanceFlags.Disposed) == false) // The thread should exit when the dispose flag has been detected.
                {
                    readertime = CurrentReachedTime;
                    if (predictingnextvalue == TimeSpan.MaxValue) {
                        evsinst.CurrentReachedTime = readertime;
                    } else {
                        evsinst.CurrentReachedTime = predictingnextvalue > readertime ? predictingnextvalue : readertime;
                    }
                    evsinst.State = CurrentState;
                    evsinst.TotalTrackTime = total;
                    UpdateMusicDisplay.Invoke(this, evsinst);
                    System.Threading.Thread.Sleep(55);
                    predictingnextvalue = readertime + predicttime;
                }
                evsinst = null;
            } catch (System.Exception e) {
                DebugProvider.WriteLine($"RCU: MDU Thread: Thread failed execution with: \n{e}");
            }
            DebugProvider.WriteLine("RCU: MDU thread was stopped.");
        }

        private void g_method31(PlaybackStoppedEventInfo e)
        {
            DebugProvider.WriteLine($"RCU: Player was stopped due to: {e.Reason}: {e.Exception}");
            MDU_RemoveFlagAndVerifyClosed();
            OnPlaybackStopped(e);
        }

        public event EventHandler<UpdateMusicDisplayEventArgs> UpdateMusicDisplay;

        public override IPlayerInstanceData InitialCreationInfo => temp;

        public override PlaybackState CurrentState => outdev is null ? PlaybackState.Stopped : outdev.State;

        /// <summary>
        /// Gets or sets the current time point that the player has reached. <br />
        /// When setting this value , it means that you skip the track to the specified time.
        /// </summary>
        public override TimeSpan CurrentReachedTime 
        { 
            get => HasFlagFast(PlayerInstanceFlags.Disposed) ? new(0) : codec.AudioStream.CurrentTime; 
            set { 
                if (flags.HasFlag(PlayerInstanceFlags.Disposed) == false) { 
                    codec.AudioStream.CurrentTime = value; 
                }
            }
        }

        /// <summary>
        /// Gets the total time of this track.
        /// </summary>
        public override TimeSpan TotalTrackTime => HasFlagFast(PlayerInstanceFlags.Disposed) ? new(0) : codec.AudioStream.TotalTime;

        /// <summary>
        /// Gets the remaining life time of this track , whether it is playing or not.
        /// </summary>
        public override TimeSpan RemainingTime => HasFlagFast(PlayerInstanceFlags.Disposed) ? new(0) : codec.AudioStream.TotalTime.Subtract(codec.AudioStream.CurrentTime);

        /// <summary>
        /// Gets the count of channels contained in this track.
        /// </summary>
        public override System.Int32 Channels => flags.HasFlag(PlayerInstanceFlags.Disposed) ? 0 : codec.AudioStream.Format.ChannelLayout.Length;

        /// <summary>
        /// Gets the count of samples that are 'played' in a second.
        /// </summary>
        public override System.Int32 SamplesPerSecond => flags.HasFlag(PlayerInstanceFlags.Disposed) ? 0 : codec.AudioStream.Format.SampleRate;

        /// <summary>
        /// Gets the track bitrate. The value is counted in bits on a track sample , 
        /// which if it is multiplied with <see cref="SamplesPerSecond"/> property , 
        /// will give you the total bits in a second.
        /// </summary>
        public override System.Int32 BitsPerSample => flags.HasFlag(PlayerInstanceFlags.Disposed) ? 0 : codec.AudioStream.Format.BitRate;

        /// <summary>Starts the playback , or if the player has been paused it continues from the <see cref="CurrentReachedTime"/>.</summary>
        /// <exception cref="InvalidOperationException">The player instance has not been created using the <see cref="Create"/> method.</exception>
        public override void Play()
        {
            if (flags.HasFlag(PlayerInstanceFlags.Disposed)) { return; }
            if (flags.HasFlag(PlayerInstanceFlags.Created)) { 
                outdev.Play();
                if (backthread is not null && backthread.IsAlive) { return; }
                backthread = new(MDUThreadCode);
                backthread.Name = "[MP] Music Data Updater thread";
                backthread.TrySetApartmentState(System.Threading.ApartmentState.STA);
                backthread.IsBackground = true;
                backthread.Start();
                return; 
            }
            throw new InvalidOperationException("The instance must be created first.");
        }

        /// <summary>
        /// Stops playback and sets the elapsed playback time to zero. <br />
        /// If in any case any event is attached and this method is called with the <paramref name="ffb"/>
        /// parameter to <see langword="true"/> , then it will generate a special playback stop event, even overriding the repeat mode.
        /// </summary>
        /// <exception cref="InvalidOperationException">The player instance has not been created using the <see cref="Create"/> method.</exception>
        public override void Stop(System.Boolean ffb = false)
        {
            if (flags.HasFlag(PlayerInstanceFlags.Disposed)) { return; }
            if (flags.HasFlag(PlayerInstanceFlags.Created)) { 
                if (flags.HasFlag(PlayerInstanceFlags.RepeatRoutineRegistered)) { outdev.PlaybackStopped -= RepeatRoutine; }
                if (ffb) { outdev.PlaybackStopped -= g_method31; }
                outdev.Stop();
                if (codec.AudioStream is not null) { codec.AudioStream.CurrentTime = new System.TimeSpan(0); }
                if (ffb) { outdev.PlaybackStopped += g_method31; g_method31(new(PlaybackStoppedReason.UserRequest , new SpecialStopButtonAssertionException())); }
                if (flags.HasFlag(PlayerInstanceFlags.RepeatRoutineRegistered)) { outdev.PlaybackStopped += RepeatRoutine; }
                return; 
            }
            throw new InvalidOperationException("The instance must be created first.");
        }

        /// <summary>Pauses playback.</summary>
        /// <exception cref="InvalidOperationException">The player instance has not been created using the <see cref="Create"/> method.</exception>
        public override void Pause()
        {
            if (flags.HasFlag(PlayerInstanceFlags.Disposed)) { return; }
            if (flags.HasFlag(PlayerInstanceFlags.Created)) {
                flags &= ~PlayerInstanceFlags.RunMDUThread;
                outdev.Pause(); 
                return; 
            }
            throw new InvalidOperationException("The instance must be created first.");
        }

        /// <summary>
        /// Sets the elapsed time ten seconds ahead.
        /// </summary>
        public override void TenSecondsAhead()
        {
            if (flags.HasFlag(PlayerInstanceFlags.Disposed)) { return; }
            TimeSpan current = codec.AudioStream.CurrentTime , desired = current.Add(new(0 , 0, 10));
            if (desired > codec.AudioStream.TotalTime) { desired = codec.AudioStream.TotalTime; }
            PlaybackState last = outdev.State;
            if (outdev.State == PlaybackState.Playing)
            {
                outdev.Pause();
                System.Threading.Thread.Sleep(60);
            }
            codec.AudioStream.CurrentTime = desired;
            System.Threading.Thread.Sleep(10);
            if (last == PlaybackState.Playing) { outdev.Play(); }
            else { UpdateMusicDisplay.Invoke(this, new() { CurrentReachedTime = CurrentReachedTime, State = (PlaybackState)CurrentState, TotalTrackTime = TotalTrackTime }); }
        }

        /// <summary>
        /// Sets the elapsed time ten seconds back.
        /// </summary>
        public override void TenSecondsBehind()
        {
            if (flags.HasFlag(PlayerInstanceFlags.Disposed)) { return; }
            TimeSpan current = codec.AudioStream.CurrentTime, desired = current.Subtract(new(0, 0, 10));
            if (desired < new TimeSpan(0,0,0)) { desired = new TimeSpan(0,0,0); }
            PlaybackState last = outdev.State;
            if (outdev.State == PlaybackState.Playing)
            {
                outdev.Pause();
                System.Threading.Thread.Sleep(60);
            }
            codec.AudioStream.CurrentTime = desired;
            System.Threading.Thread.Sleep(10);
            if (last == PlaybackState.Playing) { outdev.Play(); }
            else { UpdateMusicDisplay.Invoke(this, new() { CurrentReachedTime = CurrentReachedTime, State = (PlaybackState)CurrentState, TotalTrackTime = TotalTrackTime }); }
        }

        /// <summary>
        /// Sets the elapsed time one second ahead.
        /// </summary>
        public void OneSecondAhead()
        {
            if (flags.HasFlag(PlayerInstanceFlags.Disposed)) { return; }
            TimeSpan current = codec.AudioStream.CurrentTime, desired = current.Add(new(0, 0, 1));
            if (desired > codec.AudioStream.TotalTime) { desired = codec.AudioStream.TotalTime; }
            PlaybackState last = outdev.State;
            if (outdev.State == PlaybackState.Playing)
            {
                outdev.Pause();
                System.Threading.Thread.Sleep(60);
            }
            codec.AudioStream.CurrentTime = desired;
            System.Threading.Thread.Sleep(10);
            if (last == PlaybackState.Playing) { outdev.Play(); }
            else { UpdateMusicDisplay.Invoke(this, new() { CurrentReachedTime = CurrentReachedTime, State = (PlaybackState)CurrentState, TotalTrackTime = TotalTrackTime }); }
        }

        /// <summary>
        /// Sets the elapsed time one second back.
        /// </summary>
        public void OneSecondBehind()
        {
            if (flags.HasFlag(PlayerInstanceFlags.Disposed)) { return; }
            TimeSpan current = codec.AudioStream.CurrentTime, desired = current.Subtract(new(0, 0, 1));
            if (desired < new TimeSpan(0, 0, 0)) { desired = new TimeSpan(0, 0, 0); }
            PlaybackState last = outdev.State;
            if (outdev.State == PlaybackState.Playing)
            {
                outdev.Pause();
                System.Threading.Thread.Sleep(60);
            }
            codec.AudioStream.CurrentTime = desired;
            System.Threading.Thread.Sleep(10);
            if (last == PlaybackState.Playing) { outdev.Play(); }
            else { UpdateMusicDisplay.Invoke(this, new() { CurrentReachedTime = CurrentReachedTime, State = (PlaybackState)CurrentState, TotalTrackTime = TotalTrackTime }); }
        }

        /// <summary>
        /// Adjusts the volume on the left speaker. <br />
        /// The acceptable range is from 0 (No Sound) to 100 (Max volume).
        /// </summary>
        public override System.Byte LeftSpeakerVolume
        {
            get => temp.VolumeLeft;
            set => outdev.ChannelAudioVolume.SetChannelVolume(0, (temp.VolumeLeft = value) * 0.01f);
        }

        /// <summary>
        /// Adjusts the volume on the right speaker. <br />
        /// The acceptable range is from 0 (No Sound) to 100 (Max volume).
        /// </summary>
        public override System.Byte RightSpeakerVolume
        {
            get => temp.VolumeRight;
            set => outdev.ChannelAudioVolume.SetChannelVolume(1, (temp.VolumeRight = value) * 0.01f);
        }

        /// <summary>
        /// Gets a value whether Repeat Mode is enabled.
        /// </summary>
        public override System.Boolean RepeatEnabled => HasFlagFast(PlayerInstanceFlags.RepeatRoutineRegistered);

        /// <summary>
        /// Enables or disables Repeat Mode. The action performed is based on these returned values:
        /// <list type="bullet">
        /// <item>
        /// When the return value is <see langword="true"/>: <br />
        /// It means that the method enabled Repeat Mode.
        /// </item>
        /// <item>
        /// When the return value is <see langword="false"/>: <br />
        /// It means that the method disabled Repeat Mode.
        /// </item>
        /// </list> <br />
        /// You may also query the Repeat Mode state by reading the <see cref="RepeatEnabled"/> property.
        /// </summary>
        /// <returns>A value whether Repeat Mode is enabled or not.</returns>
        public override System.Boolean Repeat()
        {
            if (flags.HasFlag(PlayerInstanceFlags.Disposed)) { return false; }
            System.Boolean repregistered = flags.HasFlag(PlayerInstanceFlags.RepeatRoutineRegistered) == false;
            if (repregistered) {
                flags |= PlayerInstanceFlags.RepeatRoutineRegistered;
                outdev.PlaybackStopped += RepeatRoutine;
            } else {
                flags &= ~PlayerInstanceFlags.RepeatRoutineRegistered;
                outdev.PlaybackStopped -= RepeatRoutine;
            }
            return repregistered;
        }

        /// <summary>
        /// Creates the player instance. <br />
        /// The instance must be created before it is able to play! <br />
        /// For safety and robustness , this method can be called as many times as long as the first creation succeeds.
        /// </summary>
        public override void Create()
        {
            if (flags.HasFlag(PlayerInstanceFlags.Created | PlayerInstanceFlags.Disposed)) { return; }
            System.String ext = temp.Stream.GetStringAttribute("FileName");
            DebugProvider.WriteLine("RCU: Creating player pipeline.");
            // First step: Codec probe.
            // If it can be proven that it is a .ogg file, and we have Ogg Vorbis libraries , well , play it.
            DebugProvider.WriteLine($"RCU: Detecting codec to use for file {ext}.");
            if (ext is not null && ext.EndsWith(".ogg")) {
                if (HasFlagFast(PlayerInstanceFlags.OGGInternalActive) == false)
                {
                    // Throw on feature's absense.
                    throw new ExceptionSystem.OptionalFeatureAbsentException("OggVorbis");
                }
                DebugProvider.WriteLine("RCU: Successfully detected that the file needs the internal Ogg Vorbis codec. Using it.");
                var vp = new VorbisWrapper.VorbisProvider(temp.Stream);
                codec = new() {
                    AudioStream = vp,
                    StableCodecFormat = vp.NomimalAudioFormat
                };
            } else {
                // Try IMFSourceReader if possible. If not, the player will kick in the managed extensions
                try {
                    DebugProvider.WriteLine("RCU: Attempting to use IMFSourceReader.");
                    codec = new() {
                        AudioStream = new MediaFoundationReaderFromStream(temp.Stream),
                        StableCodecFormat = null
                    };
                } catch {
                    DebugProvider.WriteLine("RCU: Cannot load with IMFSourceReader , dispatching extension request.");
                    // Try to load a codec from the managed extensions
                    if (eng is not null)
                    {
                        foreach (var reqdata in eng.DispatchRequest(DefaultExtSystemRequests.GetAudioStream , temp.Stream))
                        {
                            if (codec is not null) {
                                // Dispose all the rest if any 
                                codec.AudioStream.Dispose();
                            } else {
                                // Nice! register the codec.
                                DebugProvider.WriteLine("RCU: An extension registrered a valid codec, using it.");
                                codec = reqdata as CodecProbeRequestResult;
                            }
                        }
                    }
                    // If no codecs through extensions were found, throw the Media Foundation's exception
                    if (codec is null) {
                        DebugProvider.WriteLine("RCU: Cannot use any codecs provided by the extensions. Throwing the original IMFSourceReader exception.");
                        throw; 
                    }
                }
            }
            if (codec is null || codec.AudioStream is null) { throw new InvalidOperationException("OPERATION FAILED , NO READER OBJECT IS AVAILABLE"); }
            // Next step - create the audio device
            DebugProvider.WriteLine("RCU: Creating WASAPI audio renderer.");
            if (temp.Device is null) {
                outdev = new(WASAPIMode.Shared, false, temp.Latency);
            } else {
                outdev = new(temp.Device, WASAPIMode.Shared, false, temp.Latency);
            }
            // Now check through WASAPI's IsFormatSupported whether the passed audio format is supported.
            // If a variable bit-rate codec, use it's recommended audio format.
            // But if WASAPI needs a closest format , the WASAPI becomes the priority instead of the codec's recommended value.
            IAudioProvider providerfinal = codec.AudioStream;
            AudioFormat fmt = codec.StableCodecFormat ?? codec.AudioStream.Format;
            // Next step - query WASAPI's capabilities.
            try {
                System.Boolean supp = outdev.IsFormatSupported(fmt, out var closest);
                DebugProvider.WriteLine("RCU: Querying stream capabilities...");
                if (supp)
                {
                    if (closest is not null) {
                        // OK we have to provide the resampler to resample to the closest format, as it seems.
                        providerfinal = new MediaFoundationMFTResampler(providerfinal, closest, temp.Latency);
                        DebugProvider.WriteLine("RCU: WASAPI does not exactly support the codec format, an resampler was inserted.");
                    } else if (codec.StableCodecFormat is not null) {
                        // OK we have to provide the resampler to resample to the recommended format of the codec, as it seems.
                        providerfinal = new MediaFoundationMFTResampler(providerfinal, codec.StableCodecFormat, temp.Latency);
                        DebugProvider.WriteLine("RCU: The provided codec outputs variable bit-rate data, an resampler was inserted.");
                        // If we still have monaural audio (somewhat impossible tho) add a mono -> stereo converter.
                        if (providerfinal.Format.ChannelLayout.IsEqualTo(CommonChannelTypes.Mono)) {
                            DebugProvider.WriteLine("RCU: Monophonic audio detected, attaching a converter.");
                            providerfinal = new MonoToStereoAudioProvider(providerfinal);
                        }
                    }
                    // Possibly the format is supported as is
                } else {
                    // This audio stream is not supported!!!
                    throw new NotSupportedException("This audio stream is not supported by WASAPI in any means.");
                }
            } catch {
                try { outdev.Dispose(); } catch { }
                try { providerfinal.Dispose(); } catch { }
                throw;
            }
            // Done, we can now safely initialize WASAPI.
            if (temp.Latency <= 100) {
                flags |= PlayerInstanceFlags.LatencyLow;
            }
            DebugProvider.WriteLine("RCU: Injecting pipeline and initializing audio session...");
            outdev.Initialize(providerfinal);
            LeftSpeakerVolume = temp.VolumeLeft;
            RightSpeakerVolume = temp.VolumeRight;
            outdev.PlaybackStopped += g_method31;
            flags |= PlayerInstanceFlags.Created;
        }

        protected override void DestroyInstance()
        {
            if (flags.HasFlag(PlayerInstanceFlags.Disposed)) { return; }
            flags |= PlayerInstanceFlags.Disposed;
            if (outdev is not null)
            {
                if (flags.HasFlag(PlayerInstanceFlags.RepeatRoutineRegistered))
                {
                    outdev.PlaybackStopped -= RepeatRoutine;
                }
                outdev.PlaybackStopped -= g_method31;
                if (outdev.State >= PlaybackState.Playing) { outdev.Stop(); }
                // The player must ensure that the MDU thread will be disposed of
                flags &= ~PlayerInstanceFlags.RunMDUThread;
                outdev.Dispose();
                outdev = null;
            }
            // This if statement is managed by 'outdev' but you never know... 
            if (codec is not null)
            {
                codec.AudioStream?.Dispose();
                codec.AudioStream = null;
                codec = null;
            }
            temp.Stream.Dispose();
            temp.Stream = null;
        }
    }

    public class UpdateMusicDisplayEventArgs : System.EventArgs
    {
        public UpdateMusicDisplayEventArgs() { }

        public TimeSpan CurrentReachedTime { get; set; }

        public TimeSpan TotalTrackTime { get; set; }

        public PlaybackState State { get; set; }
    }

    /// <summary>
    /// Player instance initial information.
    /// </summary>
    public struct InstanceData : IPlayerInstanceData
    {
        private AbstractPropertyStream stream;
        private System.Byte vlt , vrt , latency;
        private MMDevice dev;

        public InstanceData(IPlaylistFile file, MMDevice dev) 
        {
            if (file is null) { throw new ArgumentNullException(nameof(file)); }
            stream = file.GetStream();
            // The FileName property is presumed to exist already if needs to be overriden.
            // If not , it is presumed that it is the current file name.
            if (stream.QueryAttribute("FileName") is null) {
                stream.SetStringAttribute("FileName", file.Name);
            }
            this.dev = dev;
        }

        public InstanceData(MMDevice device) {
            dev = device;
        }

        public byte VolumeLeft { get => vlt; set => vlt = value; }
        public byte VolumeRight { get => vrt; set => vrt = value; }
        public byte Latency { get => latency; set => latency = value; }

        public MMDevice Device => dev;

        public AbstractPropertyStream Stream { get => stream; set => stream = value; }
    }
}
