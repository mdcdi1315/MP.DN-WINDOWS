using System;
using MP.TagReading;
using System.Windows;
using MP.AudioLibrary;
using static MP.Settings;
using DotNetResourcesExtensions;
using MP.AudioLibrary.MMDevice;

namespace MP
{
    /// <summary>
    /// A minimal window for playbacking any files given by the cmd-line.
    /// </summary>
    partial class SingleFilePlayerWindow : Window
    {
        private MusicPlayerStream stream;
        private MMDevice devicedefault;
        private PlayerInstance instance;
        private RotatingThreadCancellationToken token;

        public SingleFilePlayerWindow() { }

        public void InitSFPW(MusicPlayerStream stream)
        {
            if (stream is null) { return; }
            if (Global.ResourceDictionary.Exists == false)
            {
                MusicPlayerHelper.ShowErrorMessage("Cannot locate the Resources dictionary. The Music Player cannot load without them.\nPress 'OK' to exit.");
                System.Environment.Exit(4);
            }
            try
            {
                Global.Resources = new DotNetResourceLoader(Global.ResourceDictionary.FullName);
            }
            catch
            {
                MusicPlayerHelper.ShowErrorMessage("Music player failed to open: Resources are unavailable to the reader.\nPress \'OK\' to exit.");
                System.Environment.Exit(4);
            }
            this.stream = stream;
            token = null;
            instance = null;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            TenSecsAheadImage.Source = Global.Resources.LoadXamlImage("Next10Secs");
            TenSecsBackImage.Source = Global.Resources.LoadXamlImage("Prev10Secs");
            PlayImage.Source = Global.Resources.LoadXamlImage("PauseImg");
            StopImage.Source = Global.Resources.LoadXamlImage("StopImg");
            LeftVolume.Value = Global.LeftChannelVolume;
            RightVolume.Value = Global.RightChannelVolume;
            LeftVolume.ValueChanged += LeftVolume_ValueChanged;
            RightVolume.ValueChanged += RightVolume_ValueChanged;
            Title = Global.Resources.GetStringResource("AppTitle");
            Icon = Global.Resources.LoadXamlImage("ApplicationIcon");
            RepeatImage.Source = Global.Resources.LoadXamlImage("Repeat");
            TimeBar.Minimum = 0;
            TrackTitle.Content = "Loading...";
            devicedefault = MusicPlayerHelper.GetSuitableDeviceNoInteraction();
            if (devicedefault is null) { Close(); return; }
            Cursor = System.Windows.Input.Cursors.Wait;
            ITagReader rdr = DetermineAudioTag();
            stream.Position = 0;
            instance = new(new InstanceData(devicedefault) { Stream = stream , Latency = Global.Latency , VolumeLeft = Global.LeftChannelVolume , VolumeRight = Global.RightChannelVolume } , false);
            instance.Create();
            OtherData.Dispatcher.Invoke(() => {
                OtherData.Content = $"Bitrate: {instance.BitsPerSample} bit\nChannels: {instance.Channels}\n" +
                $"Sample Rate: {instance.SamplesPerSecond} kHz";
            });
            TrackTitle.Dispatcher.Invoke(() => {
                if (rdr is not null && System.String.IsNullOrWhiteSpace(rdr.Title2) == false) {
                    TrackTitle.Content = rdr.Title2;
                } else {
                    TrackTitle.Content = Microsoft.IO.Path.GetFileNameWithoutExtension(stream.GetStringAttribute("FileName"));
                }
                CreateRotatingTitleIfNeeded(TrackTitle.Content);
            });
            Cursor = null;
            LoadTagDataFromReader(rdr);
            rdr?.Dispose();
            rdr = null;
            instance.RepeatRequestSuccessfull += Host_RepeatRequestSuccessfull;
            instance.PlaybackStopped += Host_PlaybackStopped;
            instance.UpdateMusicDisplay += Host_UpdateMusicDisplay;
            instance.Play();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            LeftVolume.ValueChanged -= LeftVolume_ValueChanged;
            RightVolume.ValueChanged -= RightVolume_ValueChanged;
            if (instance is not null) {
                instance.UpdateMusicDisplay -= Host_UpdateMusicDisplay;
                instance.PlaybackStopped -= Host_PlaybackStopped;
                instance.RepeatRequestSuccessfull -= Host_RepeatRequestSuccessfull;
                instance.Stop(); instance.Dispose(); instance = null; 
            }
            if (token is not null) { token.Invalidate(); token = null; }
            devicedefault?.Dispose();
            devicedefault = null;
        }

        private void PlayButton_Click(object sender, RoutedEventArgs e)
        {
            switch (instance.CurrentState)
            {
                case PlaybackState.Playing:
                    PlayImage.Dispatcher.Invoke(() => { PlayImage.Source = Global.Resources.LoadXamlImage("PlayImg"); });
                    instance.Pause();
                    break;
                case PlaybackState.Stopped:
                case PlaybackState.Paused:
                    PlayImage.Dispatcher.Invoke(() => { PlayImage.Source = Global.Resources.LoadXamlImage("PauseImg"); });
                    instance.Play();
                    break;
            }
        }

        private void TenSecsBack_Click(object sender, RoutedEventArgs e)
        {
            instance.TenSecondsBehind();
        }

        private void TenSecsAhead_Click(object sender, RoutedEventArgs e)
        {
            instance.TenSecondsAhead();
        }

        private void StopButton_Click(object sender, RoutedEventArgs e)
        {
            instance.Stop();
            DeletePlayerScreen();
        }

        private void RepeatButton_Click(object sender, RoutedEventArgs e)
        {
            if (instance.Repeat())
            {
                RepeatImage.Dispatcher.Invoke(() => { RepeatImage.Source = Global.Resources.LoadXamlImage("RepeatEnabled"); });
            } else
            {
                RepeatImage.Dispatcher.Invoke(() => { RepeatImage.Source = Global.Resources.LoadXamlImage("Repeat"); });
            }
        }

        private void RightVolume_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            e.Handled = true;
            RightVolumeLabel.Dispatcher.Invoke(new System.Action(() => RightVolumeLabel.Content = $"Right Speaker: ({Global.RightChannelVolume} %)"));
        }

        private void LeftVolume_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            e.Handled = true;
            LeftVolumeLabel.Dispatcher.Invoke(new System.Action(() => LeftVolumeLabel.Content = $"Left Speaker: ({Global.LeftChannelVolume} %)"));
        }

        private void RightVolume_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            e.Handled = true;
            RightVolumeLabel.Dispatcher.Invoke(new System.Action(() => RightVolumeLabel.Content = "Right Speaker:"));
        }

        private void LeftVolume_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            e.Handled = true;
            LeftVolumeLabel.Dispatcher.Invoke(new System.Action(() => LeftVolumeLabel.Content = "Left Speaker:"));
        }

        private void RightVolume_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            Global.RightChannelVolume = (System.Byte)e.NewValue;
            RightVolumeLabel.Dispatcher.Invoke(new System.Action(() => RightVolumeLabel.Content = $"Right Speaker: ({Global.RightChannelVolume} %)"));
            instance.RightSpeakerVolume = Global.RightChannelVolume;
        }

        private void LeftVolume_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            Global.LeftChannelVolume = (System.Byte)e.NewValue;
            LeftVolumeLabel.Dispatcher.Invoke(new System.Action(() => LeftVolumeLabel.Content = $"Left Speaker: ({Global.LeftChannelVolume} %)"));
            instance.LeftSpeakerVolume = Global.LeftChannelVolume;
        }

        #region Private Implementation Details
        private void DeletePlayerScreen()
        {
            System.Threading.Thread.Sleep(20);
            TimeBar.Dispatcher.Invoke(new System.Action(() => TimeBar.Value = 0));
            TimeElapsed.Dispatcher.Invoke(new System.Action(() => TimeElapsed.Content = "00:00:00.00"));
            TimeRemaining.Dispatcher.Invoke(new System.Action(() => TimeRemaining.Content = "--:--:--.--"));
            if (Global.Resources is not null)
            {
                PlayImage.Dispatcher.Invoke(new System.Action(() => PlayImage.Source = Global.Resources.LoadXamlImage("PlayImg")));
            }
        }

        private void Host_UpdateMusicDisplay(object sender, UpdateMusicDisplayEventArgs e)
        {
            TimeBar.Dispatcher.Invoke(() => {
                TimeBar.Maximum = e.TotalTrackTime.Ticks;
                TimeBar.Value = e.CurrentReachedTime.Ticks;
            });
            TimeElapsed.Dispatcher.Invoke(new System.Action(() => TimeElapsed.Content = e.CurrentReachedTime.ToString(@"hh\:mm\:ss\.ff")));
            TimeRemaining.Dispatcher.Invoke(new System.Action(() => TimeRemaining.Content = e.TotalTrackTime.Subtract(e.CurrentReachedTime).ToString(@"hh\:mm\:ss\.ff")));
        }

        private void Host_PlaybackStopped(MP.AudioLibrary.PlaybackStoppedEventInfo ei)
        {
            if (IsLoaded)
            {
                TimeBar.Dispatcher.Invoke(() => {
                    TimeBar.Maximum = 1;
                    TimeBar.Value = 0;
                });
                TimeElapsed.Dispatcher.Invoke(() => TimeElapsed.Content = "00:00:00.00");
                if (instance is not null) { TimeRemaining.Dispatcher.Invoke(() => TimeRemaining.Content = instance.RemainingTime.ToString(@"hh\:mm\:ss\.ff")); }
                else { TimeRemaining.Dispatcher.Invoke(() => TimeRemaining.Content = "--:--:--.--"); }
                if (Global.Resources is not null)
                {
                    PlayImage.Dispatcher.Invoke(() => PlayImage.Source = Global.Resources.LoadXamlImage("PlayImg"));
                }
            }
        }

        private void Host_RepeatRequestSuccessfull(object sender, EventArgs e)
        {
            PlayImage.Dispatcher.Invoke(() => { PlayImage.Source = Global.Resources.LoadXamlImage("PauseImg"); });
        }

        private void CreateRotatingTitleIfNeeded(System.Object data)
        {
            // Creates a 'rotating' title in the case that the title is not fully filled 
            // into the title field. A new thread takes this burden so as to work.
            // Currently width 410 with ClearType settings corresponds to 39 fully interpretable characters.
            if (data is not System.String dt) { return; }
            if (dt.Length < 40) { return; }
            token = new(token);
            var rotating = new System.Threading.Thread((System.Object obj) => {
                // Code perf: casting happens only once.
                System.String title = (System.String)obj;
                // Note that the current code is optimized to break immediately when the token requests to 
                // terminate the thread.
                System.Int32 index = 0, tid = token.InstanceId;
                void HaltThread(System.Int32 t)
                {
                    System.Int32 time = t;
                    while (token.IsValid(tid) && time > 0) { System.Threading.Thread.Sleep(10); time -= 10; }
                }
            rotate:
                index = 0;
                while (token.IsValid(tid) && index + 39 < title.Length)
                {
                    // Rotate until all chars are processed. (Or break if the thread is requested to terminate)
                    TrackTitle.Dispatcher.Invoke(() => { TrackTitle.Content = title.Substring(index, 40); });
                    HaltThread(650);
                    index++;
                }
                if (token.IsValid(tid))
                {
                    // Hold the title still for 1.66 seconds.
                    TrackTitle.Dispatcher.Invoke(() => { TrackTitle.Content = title.Substring(0, 40); });
                    HaltThread(1670);
                    goto rotate;
                }
            });
            rotating.Name = Global.Resources.GetStringResource("RotatingTitleThread_Name");
            rotating.Start(data);
        }

        private void LoadTagDataFromReader(ITagReader rdr)
        {
            void LoadPicture(System.Byte[] dt)
            {
                System.Threading.Thread TG = new(() => {
                    System.Threading.Thread.Sleep(400);
                    Microsoft.IO.MemoryStream MS = new(dt);
                    try
                    {
                        MS.Position = 0;
                        System.Windows.Media.Imaging.BitmapImage IMG = new();
                        IMG.BeginInit();
                        IMG.StreamSource = MS;
                        IMG.CacheOption = System.Windows.Media.Imaging.BitmapCacheOption.OnLoad;
                        IMG.CreateOptions = System.Windows.Media.Imaging.BitmapCreateOptions.DelayCreation;
                        IMG.EndInit();
                        IMG.Freeze();
                        CoverImage.Dispatcher.Invoke(() => {
                            CoverImage.Visibility = Visibility.Visible;
                            CoverImage.BeginInit();
                            CoverImage.Source = IMG;
                            CoverImage.EndInit();
                            CoverImage.UpdateLayout();
                        });
                    }
                    catch { }
                    finally { MS.Dispose(); }
                    MS = null;
                });
                TG.TrySetApartmentState(System.Threading.ApartmentState.STA);
                TG.Start();
            }
            if (rdr is null) { return; }
            OtherData.Dispatcher.Invoke(() => OtherData.Content =
            $"Artists: {rdr.ContributingArtists} Album: {rdr.AlbumName}\n" +
            $"Genre: {rdr.Genre} Album Artist: {rdr.AlbumArtist}\n" +
            $"Bitrate: {instance.BitsPerSample} bit Sample Rate: {instance.SamplesPerSecond} kHz");
            if (rdr.Image is not null && rdr.Image.LongLength > 0) { LoadPicture(rdr.Image); }
        }

        private ITagReader DetermineAudioTag()
        {
            ITagReader reader = null;
            try
            {
                System.Boolean flag = false;
                System.Threading.Thread TD = new(() => {
                    stream.Position = 0;
                    try { reader = new ID3V2DataReader(stream); } catch { }
                    stream.Position = 0;
                    if (reader is not null) { goto G_completed; }
                    try { reader = new MP4AudioTagReader(stream); } catch { }
                    if (reader is not null) { goto G_completed; }
                    stream.Position = 0;
                    try { reader = new FlacAudioTagReader(stream); } catch { }
                    if (reader is null) { goto G_completed; }
                    try { reader = new OggVorbisAudioTagReader(stream); } catch { }
                G_completed:
                    flag = true;
                    return;
                });
                TD.TrySetApartmentState(System.Threading.ApartmentState.STA);
                TD.Start();
                while (flag == false) { System.Threading.Thread.Sleep(10); }
                return reader;
            } catch { return null; }
        }

        #endregion
    }
}
