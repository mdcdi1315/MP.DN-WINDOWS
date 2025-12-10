
using System;
using MP.Utilities;
using System.Linq;
using Microsoft.IO;
using MP.Threading;
using MP.AudioLibrary;
using static MP.Settings;
using MP.GamepadBackend;
using MP.ExtensibilitySystem;
using MP.Caches.StatisticsCache;
using System.Collections.Generic;
using MP.Caches.PlaylistIconCache;
using DotNetResourcesExtensions;

namespace MP
{
    /// <summary>
    /// The main Music Player engine implementation for Windows assembly. <br />
    /// NOTE: For the abstraction of the engine and it's core parts , see the <see cref="RCUEngine"/> class.
    /// </summary>
    internal sealed class WindowsRCUEngine : RCUEngine
    {
        public const System.String DirectoryIdentifierString = "Directory";
        private IPlaylist current;
        private System.Int64 cmds;
        private System.Int32 trkidx;
        private ListViewMode lvmode;
        private RepeatMode repmode;
        private DirectoryInfo basedir;
        private GamepadReader gamepad;
        private OperationsTasker tasker;
        private DiscordRichPresense drp;
        private PlaylistIconCacheInstance pici;
        private PlayerInstance currentplayer;
        private StatisticsCacheInstance statinst;
        private volatile RCUEngineStateFlags state;
        private MPWindowsExtEngine extensionengine;
        private AudioLibrary.MMDevice.MMDevice device;
        // Ensures that all the access to the player instance is only valid access
        private System.Threading.SemaphoreSlim playerlock;

        public WindowsRCUEngine() : base() { }

        protected override void Create()
        {
            DebugProvider.WriteLine("Creating RCU engine...");
            current = null;
            currentplayer = null;
            lvmode = new();
            trkidx = 0;
            cmds = 0;
            repmode = RepeatMode.No;
            device = null;
            tasker = new();
            statinst = new();
            extensionengine = new(Global.BaseDataDirectory , Global.BaseDataDirectory.GetSubDirectory("Plugins"));
            state = RCUEngineStateFlags.None; // No states are defined during this phase
            playerlock = new System.Threading.SemaphoreSlim(1);
            DebugProvider.WriteLine($"RCU engine created at thread {System.Threading.Thread.CurrentThread.ManagedThreadId} .");
        }

        /// <summary>
        /// When the UI is ready to start getting requests from this instance , this must be called so as to initialize the backend. <br />
        /// If creation fails , then the caller must immediately exit the app. <br />
        /// This creation MUST HAPPEN ON THE UI THREAD!!!!
        /// </summary>
        /// <returns>A value whether initialization succeeded or not.</returns>
        public override System.Boolean Initialize()
        {
            DebugProvider.WriteLine("Initializing created RCU engine...");
            MusicPlayerHelper.InitializeMainWindowHandle();
            // No playlist selected? then initialize back to files mode.
            if (Global.LastPlaylistName.Length == 0) { lvmode = ListViewMode.Files; } else { lvmode = ListViewMode.LoadedPlaylist; }
            DebugProvider.WriteLine($"RCU engine will load in {lvmode} mode.");
            DebugProvider.WriteLine("RCU: Attempting to retrieve valid audio device.");
            if ((device = MusicPlayerHelper.GetSuitableDevice()) is null)
            {
                MusicPlayerHelper.ShowErrorResourceMessage("Error_NoAudioDeviceFound");
                return false;
            }
            if (device.State != AudioLibrary.MMDevice.DEVICE_STATE.ACTIVE || device.DataFlow == AudioLibrary.MMDevice.EDataFlow.Capture)
            {
                MusicPlayerHelper.ShowErrorResourceMessage("Error_NoValidAudioDeviceFound");
                return false;
            }
            // Keep the audio device ID, we will elsewise create it later on the Tasker thread.
            Global.AudioDeviceId = device.ID;
            DebugProvider.WriteLine("RCU: Destroying audio device, will be re-created again.");
            device.Dispose();
            device = null;
            DebugProvider.WriteLine("Checking correct RCU instance creation...");
            if (MusicPlayer.Entry.Window.Dispatcher.Thread.ManagedThreadId != System.Threading.Thread.CurrentThread.ManagedThreadId)
            {
                MusicPlayerHelper.ShowErrorResourceMessage("Error_InvalidThreadSelected");
                return false;
            }
            try {
                DebugProvider.WriteLine("RCU: Starting up the Extension Engine...");
                extensionengine.Load();
            } catch (Exception ex) {
                MusicPlayer.Entry.WaitFormInstance?.Close();
                MusicPlayerHelper.ShowErrorResourceMessage("Error_CannotLoadExtEngine" , ex);
                return false;
            }
            DebugProvider.WriteLine("RCU: Extension Engine is now loaded and attached to the RCU Engine!");
            DebugProvider.WriteLine("GamepadManager: Requesting 1 gamepads.");
            GamepadManager.RequestGamepads(1, new() { Navigation = true });
            if (GamepadManager.Gamepads.Count() == 1)
            {
                GamepadFound(null, GamepadManager.Gamepads.First());
            }
            GamepadManager.GamepadAdded += GamepadFound;
            GamepadManager.GamepadRemoved += GamepadRemoved;
            basedir = new(SystemInfo.GetKnownFolder(ShellKnownFolder.Music));
            DebugProvider.WriteLine($"RCU: Base landing directory for Files mode was resolved as {basedir.FullName}");
            state |= RCUEngineStateFlags.IsFirstFileExploration; // Since a custom first file exploration is specified , we must set this flag.
            DebugProvider.WriteLine("RCU: Attempting to create a Discord Client connection.");
            drp = new(AppInfo.DiscordAPIToken);
#if DEBUG
            DiscordTraceMonitor dti = new();
            drp.RegisterLogsListener(dti);
            DebugProvider.AddSource(dti);
            dti = null;
#endif
            drp.TryRun();
            DebugProvider.WriteLine("RCU: Loading cache files...");
            DebugProvider.WriteLine("RCU: Loading Playlist Icon Cache...");
            pici = new(Global.IconCachesDirectory);
            using (var sm = Global.IconCachesMapFile.OpenRead())
            {
                pici.LoadCacheFromStream(sm);
            }
            DebugProvider.WriteLine("RCU: Loading Statistics Cache...");
            statinst.LoadFromFile(Global.StatisticsCacheFile);
            DebugProvider.WriteLine("RCU: Done loading caches!!");
            DebugProvider.WriteLine("Loading RCU engine preferred mode...");
            RCUENGINE_INIT_MODE();
            DebugProvider.WriteLine("RCU: Attaching to and syncronizing the UI...");
            RaiseSendCommand(CommonSendCommandTypes.Refresh);
            RaiseSendCommand(WindowsRCUSendCommandTypes.ChangeRepeatModeImage);
            DebugProvider.WriteLine($"RCU engine initialization finished in {SystemInfo.Now}.");
            DebugProvider.WriteLine("RCU: Loading Main Dispatch Tasker...");
            tasker.Run();
            DebugProvider.WriteLine("RCU: Main Dispatch Tasker is now active.");
            return true;
        }

        private void GamepadFound(System.Object obj, GamepadReader reader)
        {
            if (gamepad is not null && gamepad.Running) { return; }
            DebugProvider.WriteLine("GamepadManager: XInput gamepad connected , requesting verification.");
            if (MusicPlayerHelper.ShowResourceQuestionMessage("Question_ConnectNewGamepad"))
            {
                DebugProvider.WriteLine("GamepadManager: Attaching gamepad to UI events...");
                gamepad = reader;
                gamepad.GamepadAction += GamepadHandler;
                gamepad.UpdateDelay = 165;
                gamepad.Listen = true;
                var s = statinst.Get("NumberOfControllerConnectedTimes");
                s.IncrementNumericValue();
                statinst.Update(s);
                tasker.Add(() => {
                    AudioLibrary.MMDevice.MMDevice dev = gamepad.GetHeadsetOutputDevice();
                    if (dev is not null && MusicPlayerHelper.ShowResourceQuestionMessage("UseConnectedHeadsetDevice", dev.ID, dev.FriendlyName))
                    {
                        state |= RCUEngineStateFlags.IsGamepadHeadsetUsed;
                        device?.Dispose();
                        device = dev;
                        dev = null;
                    }
                });
                DebugProvider.WriteLine("GamepadManager: Gamepad attach complete.");
            }
        }

        private void GamepadRemoved(System.Object obj, System.UInt32 indx)
        {
            if (gamepad is not null && indx == gamepad.UserIndex)
            {
                gamepad.Listen = false;
                if (state.HasFlag(RCUEngineStateFlags.IsGamepadHeadsetUsed))
                {
                    tasker.Add(() => {
                        DestroyPlayerInstance();
                        device?.Dispose();
                        device = MusicPlayerHelper.GetSuitableDevice();
                        state &= ~RCUEngineStateFlags.IsGamepadHeadsetUsed;
                    });
                }
                DebugProvider.WriteLine("GamepadManager: Detaching gamepad from UI events.");
                gamepad.GamepadAction -= GamepadHandler;
                gamepad.Dispose();
                gamepad = null;
                DebugProvider.WriteLine("GamepadManager: Successfully detached the gamepad from the UI events.");
            }
        }

        private void GamepadHandler(System.Object obj, GamepadEventEventArgs e)
        {
            System.String cmd = System.String.Empty;
            if (e.Type == GamepadMode.Button)
            {
                switch (e.Button)
                {
                    case GamepadButton.A:
                        cmd = "PLAY";
                        break;
                    case GamepadButton.B:
                        cmd = "STOP";
                        break;
                    case GamepadButton.Y:
                        cmd = "SEL_SPK";
                        break;
                    case GamepadButton.X:
                        cmd = "RIGHT_CLICK";
                        break;
                    case GamepadButton.Menu:
                        cmd = "CHANGE_MODE";
                        break;
                    case GamepadButton.View:
                        tasker.Add(ChangeRepeatMode);
                        return;
                    case GamepadButton.DPadLeft:
                        currentplayer?.TenSecondsBehind();
                        return;
                    case GamepadButton.DPadRight:
                        currentplayer?.TenSecondsAhead();
                        return;
                    case GamepadButton.DPadUp:
                        cmd = "PREV";
                        break;
                    case GamepadButton.DPadDown:
                        cmd = "NEXT";
                        break;
                    case GamepadButton.LeftShoulder:
                        cmd = "SPK_DOWN";
                        break;
                    case GamepadButton.RightShoulder:
                        cmd = "SPK_UP";
                        break;
                    case GamepadButton.LeftThumbstick:
                        cmd = "CLOSE";
                        break;
                    case GamepadButton.RightThumbstick:
                        cmd = "OPTION";
                        break;
                }
            }
            else if (e.Type == GamepadMode.RightThumbstick)
            {
                if (e.Y > 0) { cmd = "UP"; }
                if (e.Y < 0) { cmd = "DOWN"; }
            }
            if (System.String.IsNullOrEmpty(cmd)) { return; }
            DebugProvider.WriteLine($"GamepadManager: Dispatching message {cmd} to the UI.");
            RaiseMessage(cmd, CommonSendCommandTypes.ControllerCommand);
        }

        private void RaiseMessage(System.String message, CommonSendCommandTypes type = CommonSendCommandTypes.ThrowMessage)
        {
            // Prefer this invokation to happen in a new thread so that the call is not blocking.
            var th = new System.Threading.Thread((System.Object obj) => {
                RaiseSendMessageCommand(type , message.ToString());
            });
            th.Name = "[MP] Message processing event thread";
            th.IsBackground = true;
            DebugProvider.WriteLine($"CoreMessageDispatcher: Dispatching command {type} to the UI.");
            th.Start(message);
            th = null; // To not have it stale
        }

        public override void RecieveCommand(RecieveCommand cmd)
        {
            if (tasker is null) {
                DebugProvider.WriteLine("RCU: Command submission manager is not yet ready. This submission will be destroyed.");
                return;
            }
            cmds++;
            if (cmd is null) { DebugProvider.WriteLine($"CommandSubmissionManager: Command submission {cmds} was rejected because it was null."); }
            
            switch (cmd.Type) 
            {
                case CommonRecieveCommandTypes.None:
                    DebugProvider.WriteLine($"CommandSubmissionManager: Command submission {cmds} was rejected because it did not contain any event data.");
                    break;
                case CommonRecieveCommandTypes.HardFailAppMustClose:
                    DebugProvider.WriteLine("RCU: The engine state has become corrupted and cannot continue due to a hard failure. Dispatching hard failure message.");
                    CommandMetadata mdta = new();
                    mdta.AddItem(new("Exception" , cmd.Data));
                    // Intercept already registered commands, remove those, and register engine destroy events.
                    tasker.ClearCurrentWorkItemQueue();
                    tasker.Add((Action<CommonSendCommandTypes , CommandMetadata>)RaiseSendCommand, CommonSendCommandTypes.HardFailGracefulExitRequested , mdta);
                    tasker.Add((Action<CommonSendCommandTypes>)RaiseSendCommand, CommonSendCommandTypes.IgnoreFutureRequests);
                    tasker.Add(PrepareShutdown);
                    break;
                case WindowsRCURecieveCommandTypes.GetFilesAndDirs:
                    tasker.Add(GatherFilesAndDirs , cmd.Data);
                    break;
                case WindowsRCURecieveCommandTypes.GetDrives:
                    tasker.Add(GatherDrives);
                    break;
                case CommonRecieveCommandTypes.GetPlaylists:
                    tasker.Add(GatherPlaylists);
                    break;
                case CommonRecieveCommandTypes.DestroyPlayer:
                    tasker.Add(DestroyPlayerInstance);
                    break;
                case CommonRecieveCommandTypes.LoadTrackOrdinal:
                    tasker.Add(SafeLoadPlayer , cmd.Data);
                    break;
                case CommonRecieveCommandTypes.CloseActivePlaylist:
                    if (cmd.Data is System.Boolean dt && dt) {
                        tasker.Add<System.Func<System.Boolean>>(SavePlaylist);
                    }
                    tasker.Add(DestroyActivePlaylist);
                    break;
                case CommonRecieveCommandTypes.OpenPlaylist:
                    tasker.Add(LoadPlaylist, cmd.Data);
                    break;
                case CommonRecieveCommandTypes.ReloadOpenedPlaylist:
                    if (current is null) { 
                        return; 
                    } else if (current is DownloadedFilesPlaylist) {
                        tasker.Add<System.Func<System.Boolean>>(SavePlaylist);
                        tasker.Add(LoadPlaylist, $"special-{Global.DownloadedTracksPlaylist.Name}");
                    } else if (current is ArchivedTrackPlaylist) {
                        tasker.Add(LoadEx_PlaylistArchives);
                    } else {
                        tasker.Add(LoadPlaylist, this.GetStringAttribute("PlaylistNameField"));
                    }
                    break;
                case WindowsRCURecieveCommandTypes.SelectRepeatMode:
                    tasker.Add(ChangeRepeatMode);
                    break;
                case CommonRecieveCommandTypes.OpenPlaylistUseLastTrack:
                    tasker.Add(LoadPlaylistAndLastTrack , cmd.Data);
                    break;
                case CommonRecieveCommandTypes.DeletePlaylist:
                    tasker.Add(DeletePlaylist, cmd.Data);
                    break;
                case WindowsRCURecieveCommandTypes.GetPlaylistPreferences:
                    tasker.Add(ModPlaylistPreferences);
                    break;
                case WindowsRCURecieveCommandTypes.DeleteTrack:
                    tasker.Add(DeleteTrack, cmd.Data);
                    break;
                case WindowsRCURecieveCommandTypes.ShowTrackTag:
                    tasker.Add(ShowTrackTag, cmd.Data);
                    break;
                case WindowsRCURecieveCommandTypes.AddTrackToPlaylist:
                    tasker.Add(AddTrackToPlaylist);
                    break;
                case WindowsRCURecieveCommandTypes.OpenArchivedPlaylist:
                    tasker.Add(LoadArchivedPlaylist , cmd.Data);
                    break;
                case WindowsRCURecieveCommandTypes.CleanStatisticsCacheData:
                    tasker.Add(statinst.ClearCacheEntries);
                    tasker.Add(CreateStatistics);
                    DebugProvider.WriteLine($"CommandSubmissionManager: Special handling required for code {cmds}. The command itself, however, is registered as expected.");
                    return;
                default:
                    DebugProvider.WriteLine($"CommandSubmissionManager: Implementation Limitation: Command {cmd.Type} not implemented yet.");
                    break;
            }
            var s = statinst.Get("NumberOfDispatchedCommands");
            if (s is not null)
            {
                s.IncrementNumericValue();
                statinst.Update(s);
            }
            DebugProvider.WriteLine($"CommandSubmissionManager: Command {cmds} with code {cmd.Type} registered for execution.");
        }

        private void GatherDrives()
        {
            // Invoke the wait cursor first
            RaiseSendCommand(WindowsRCUSendCommandTypes.InvokeWaitCursor);
            DebugProvider.WriteLine("FileMinimalExplorer: Gathering drive list...");
            // Gather any drives found.
            // Get the main OS Drive. (Although that in 99% of cases this will be the C drive)
            System.String main = SystemInfo.GetEnvironmentVariable("SYSTEMDRIVE") + "\\";
            DebugProvider.WriteLine($"FileMinimalExplorer: System drive determined as {main} (as defined by 'SYSTEMDRIVE' environment variable).");
            // Create output data
            CommandMetadata data = new();
            try {
                System.Int32 imgidx = 1;
                DebugProvider.WriteLine("FileMinimalExplorer: Getting drives...");
                foreach (var drive in DriveInfo.GetDrives())
                {
                    try {
                        DebugProvider.WriteLine($"FileMinimalExplorer: Got drive {drive.Name} with label {drive.VolumeLabel} and filesystem {drive.DriveFormat}. Root directory is {drive.RootDirectory.FullName}.");
                        if (drive.RootDirectory.FullName.Equals(main)) { imgidx = 0; DebugProvider.WriteLine($"Drive {main} seems to match as being the OS drive (as reported by the OS environment variable 'SYSTEMDRIVE')."); }
                        data.AddItem(new WindowsListViewElement() {
                            PrimaryData = drive.RootDirectory.FullName,
                            ImageIndex = imgidx,
                            SecondaryData = [drive.VolumeLabel , MusicPlayerHelper.GetSize(drive.AvailableFreeSpace) , MusicPlayerHelper.GetSize(drive.TotalSize)]
                        });
                        imgidx = 1;
                    } catch (System.IO.IOException ioex) {
                        DebugProvider.WriteLine($"FileMinimalExplorer: I/O Exception occured on the native call: {ioex}\n\nThis drive entry will not be registered to the drive list.");
                    }
                }
                // Add our desired columns.
                data.AddItem(new WindowsListViewColumn("Path", 60, false));
                data.AddItem(new WindowsListViewColumn("Name", 180));
                data.AddItem(new WindowsListViewColumn("Remaining Size", 110, false));
                data.AddItem(new WindowsListViewColumn("Size", 80, false));
                data.AddItem(new("dd_img" , Global.Resources.GetByteArrayResource("OSDrive")) { Type = CommandMetadataItemType.Image });
                data.AddItem(new("dd_img", Global.Resources.GetByteArrayResource("Drive")) { Type = CommandMetadataItemType.Image });
                // Create a new event to send the drive list and boom , ready.
                DebugProvider.WriteLine("FileMinimalExplorer: Drive list gather finished successfully.");
                // Ensure to update the data in UI
                lvmode = ListViewMode.SelectDrive;
                RaiseSendCommand(CommonSendCommandTypes.ClearExplorationScreen);
                RaiseSendCommand(WindowsRCUSendCommandTypes.UpdateSelectedMode);
                RaiseSendCommand(CommonSendCommandTypes.LoadExplorationScreen, data);
            } catch (Exception e) {
                switch (e)
                {
                    case UnauthorizedAccessException:
                        DebugProvider.WriteLine("FileMinimalExplorer: Access Error occured while gathering drives!!!");
                        RaiseMessage(Global.Resources.GetStringResource("Error_DriveGather_AccessDenied"));
                        break;
                    default:
                        throw;
                }
                return;
            } finally {
                data = null;
                RaiseSendCommand(WindowsRCUSendCommandTypes.RemoveWaitCursor);
                GC.Collect(1);
            }
        }

        private void GatherFilesAndDirs(DirectoryInfo searchdatabase)
        {
            if (state.HasFlag(RCUEngineStateFlags.IsFirstFileExploration))
            {
                searchdatabase = basedir;
                state ^= RCUEngineStateFlags.IsFirstFileExploration;
            }
            if (searchdatabase is null) { 
                GatherDrives();
                return;
            }
            // Find all directories and the below files. The file list is retrieved from the SupportedFormats resource.
            List<FileSystemInfo> infos = null;
            DebugProvider.WriteLine($"FileMinimalExplorer: Getting all files and directories under {searchdatabase.FullName}...");
            RaiseSendCommand(WindowsRCUSendCommandTypes.InvokeWaitCursor);
            try {
                infos = new(searchdatabase.GetDirectories());
                // Continue loading any files that are found.
                DebugProvider.WriteLine("FileMinimalExplorer: Adding valid audio files to exploration list.");
                foreach (System.String fmt in Global.Resources.GetStringResource("SupportedExplorationViewFiles").Split(';'))
                {
                    infos.AddRange(searchdatabase.GetFiles(fmt));
                }
            } catch (Exception e) {
                switch (e)
                {
                    case UnauthorizedAccessException:
                        DebugProvider.WriteLine($"FileMinimalExplorer: Access failed to {searchdatabase.FullName}...");
                        RaiseMessage(System.String.Format(Global.Resources.GetStringResource("FileExpView_AccessNotAllowed"), searchdatabase.Name));
                        break;
                    case System.IO.DirectoryNotFoundException:
                        DebugProvider.WriteLine($"FileMinimalExplorer: Directory {searchdatabase.FullName} was not found.");
                        RaiseMessage(System.String.Format(Global.Resources.GetStringResource("FileExpView_DirectoryNotFound"), searchdatabase.Name));
                        break;
                    case System.IO.IOException:
                        DebugProvider.WriteLine($"FileMinimalExplorer: Generic I/O error occured while getting file list for {searchdatabase.FullName}.");
                        RaiseMessage(System.String.Format(Global.Resources.GetStringResource("FileExpView_IOError"), searchdatabase.Name, e.Message));
                        break;
                    default:
                        // On cases where an unexpected exception was occured, prefer to throw it back.
                        throw;
                }
                RaiseSendCommand(WindowsRCUSendCommandTypes.RemoveWaitCursor);
                return;
            }
            // After successfull info retrieval , we can save this back to basedir field.
            basedir = searchdatabase;
            CommandMetadata data = new();
            // Initialize columns.
            data.AddItem(new WindowsListViewColumn("Name", Global.WD_FileName, true));
            data.AddItem(new WindowsListViewColumn("Date Created", 140));
            data.AddItem(new WindowsListViewColumn("Date Modified", 140));
            data.AddItem(new WindowsListViewColumn("Size", 80));
            // Add the images to load along.
            data.AddItem(new("dd_img", Global.Resources.GetByteArrayResource("Folder")) { Type = CommandMetadataItemType.Image });
            data.AddItem(new("dd_img", Global.Resources.GetByteArrayResource("Audiofile")) { Type = CommandMetadataItemType.Image });
            data.AddItem(new("dd_img", Global.Resources.GetByteArrayResource("M3UFile")) { Type = CommandMetadataItemType.Image });
            // Create the corresponding elements.
            DebugProvider.WriteLine("FileMinimalExplorer: Creating list view entries.");
            foreach (var info in infos)
            {
                if (info is DirectoryInfo di)
                {
                    data.AddItem(new WindowsListViewElement() { ImageIndex = 0, PrimaryData = di.Name, SecondaryData = [di.CreationTimeUtc.ToString(), di.LastAccessTimeUtc.ToString(), DirectoryIdentifierString] });
                } else if (info is FileInfo fi)
                {
                    switch (fi.Extension.ToLower())
                    {
                        case ".m3u":
                        case ".m3u8":
                            data.AddItem(new WindowsListViewElement() { ImageIndex = 2, PrimaryData = fi.Name, SecondaryData = [fi.CreationTimeUtc.ToString(), fi.LastAccessTimeUtc.ToString(), MusicPlayerHelper.GetSize(fi.Length)] });
                            break;
                        default:
                            data.AddItem(new WindowsListViewElement() { ImageIndex = 1, PrimaryData = fi.Name, SecondaryData = [fi.CreationTimeUtc.ToString(), fi.LastAccessTimeUtc.ToString(), MusicPlayerHelper.GetSize(fi.Length)] });
                            break;
                    }
                }
            }
            // Ensure that our list view has been cleared first.
            RaiseSendCommand(CommonSendCommandTypes.ClearExplorationScreen);
            // Create a new event to send them and boom , ready.
            DebugProvider.WriteLine("FileMinimalExplorer: File/dir gather finished successfully.");
            // Ensure to update the data in UI
            lvmode = ListViewMode.Files;
            RaiseSendCommand(WindowsRCUSendCommandTypes.UpdateSelectedMode);
            RaiseSendCommand(CommonSendCommandTypes.LoadExplorationScreen, data);
            data = null;
            RaiseSendCommand(WindowsRCUSendCommandTypes.RemoveWaitCursor);
            GC.Collect(1);
        }

        private void GatherPlaylists()
        {
            // Ensure that our list view has been cleared first.
            DebugProvider.WriteLine("PlaylistGatherer: Preparing for playlist gather.");
            RaiseSendCommand(WindowsRCUSendCommandTypes.InvokeWaitCursor);
            List<FileInfo> playlists = null;
            try {
                RaiseMessage(Global.Resources.GetStringResource("WaitMsg_GatheringPlaylists"), CommonSendCommandTypes.ThrowWaitMessage);
                playlists = new(10);
                DebugProvider.WriteLine("PlaylistGatherer: Gathering playlists from FS...");
                foreach (var extstring in Global.Resources.GetStringResource("AllPlaylistExtensions").Split(';'))
                {
                    playlists.AddRange(Global.PlaylistsDirectory.GetFiles($"*{extstring}"));
                }
            } catch (System.Exception e) {
                switch (e) {
                    case UnauthorizedAccessException:
                        DebugProvider.WriteLine("PlaylistGatherer: FATAL: No access to the playlists directory. Fatal error occured.");
                        throw;
                    case System.IO.IOException:
                        DebugProvider.WriteLine($"PlaylistGatherer: I/O Error occured while gathering playlists: {e}");
                        RaiseMessage(Global.Resources.GetStringResource("Error_GatherPlaylists_IO"));
                        break;
                    default:
                        throw;
                }
                RaiseSendCommand(CommonSendCommandTypes.ClearWaitMessage);
                RaiseSendCommand(WindowsRCUSendCommandTypes.RemoveWaitCursor);
                return;
            }
            DebugProvider.WriteLine($"PlaylistGatherer: Retrieved {playlists.Count} playlists on search dir {Global.PlaylistsDirectory.FullName}");
            TrackPlayList temp = null;
            FileStream tmpstr = null;
            CommandMetadata data = new();
            // Add the default playlist image now.
            data.AddItem(new("dd_img", Global.Resources.GetByteArrayResource("PlaylistIconDefault")) { Type = CommandMetadataItemType.Image });
            System.Int32 imgct = 1;
            foreach (FileInfo Item in playlists)
            {
                try {
                    DebugProvider.WriteLine($"PlaylistGatherer: Attempting to open playlist {Item.Name}...");
                    RaiseMessage($"Gathering information for: {Item.Name}", CommonSendCommandTypes.ThrowWaitMessage);
                    tmpstr = Item.OpenRead();
                    temp = new(tmpstr, Item.Name);
                    // Load the image , if found and include it into the list.
                    System.Byte[] BM = null;
                    System.Int32 imgidx = 0; // Now all the playlists will get a default internal image
                    DebugProvider.WriteLine($"PlaylistGatherer: Playlist {Item.Name} retrieved successfully.");
                    try {
                        System.Boolean reg = pici.PlaylistRegistered(temp.PlaylistNameField);
                        DebugProvider.WriteLine($"PlaylistGatherer: This playlist {(reg ? "does have" : "does not have")} a cached cover image.");
                        if (reg) {
                            DebugProvider.WriteLine($"PlaylistGatherer: Loading cached playlist cover image file.");
                            BM = File.ReadAllBytes(pici.GetSmallIconFileFullPath(temp.PlaylistNameField));
                            data.AddItem(new("dd_img", BM) { Type = CommandMetadataItemType.Image });
                            imgct++;
                            imgidx = imgct - 1;
                        } else {
                            DebugProvider.WriteLine("PlaylistGatherer: Attempting to open the playlist cover file specified in preferences. " +
                                $"Path: {temp.PlaylistCoverPath}");
                            BM = File.ReadAllBytes(temp.PlaylistCoverPath);
                            data.AddItem(new("dd_img", BM) { Type = CommandMetadataItemType.Image });
                            imgct++;
                            imgidx = imgct - 1;
                            DebugProvider.WriteLine("PlaylistGatherer: Cover file opened successfully.");
                        }
                        // If we have reached here, the image is loadable so we can add it to the image caches.
                        // Note that images above 256 pixels cannot be cached.
                        if (reg == false) {
                            // ArgumentException that this method throws regarding image dims is already covered by the catch clause.
                            pici.AddPlaylistIcon(temp.PlaylistCoverPath, temp.PlaylistNameField);
                        }
                    } // FFE in this case , do not do anything.
                    catch (System.IO.FileNotFoundException e) {
                        DebugProvider.WriteLine($"PlaylistGatherer: Cannot open cover file due to: {e}"); 
                    } catch (ArgumentException) { }
                    DebugProvider.WriteLine($"PlaylistGatherer: Playlist {Item.Name} is valid , adding it to the playlist list");
                    System.String n = System.String.Empty;
                    if (temp.Metadata is not null && temp.Metadata.TryGetValue("CurrentTrackFileName" , out System.Object obj))
                    {
                        n = obj as System.String;
                    } else if (temp.CurrentTrack is not null) {
                        n = temp.CurrentTrack.Name;
                    }
                    data.AddItem(new WindowsListViewElement() {
                            PrimaryData = temp.PlaylistName,
                            SecondaryData = [Item.CreationTimeUtc.ToString(), temp.TracksContained.Count.ToString(), n],
                            ImageIndex = imgidx,
                            AdditionalData = Item.Name,
                    });
                } catch (Exception e) {
                    DebugProvider.WriteLine($"PlaylistGatherer: Playlist {Item.Name} could not be loaded. Error Data:\n{e}");
                    data.AddItem(new WindowsListViewElement() {
                        PrimaryData = $"{Item.GetNameOnly()} (Cannot load)",
                        ImageIndex = -1,
                        SecondaryData = [Item.CreationTimeUtc.ToString(), "", ""],
                        AdditionalData = Item.Name
                    });
                } finally {
                    tmpstr?.Dispose();
                    temp?.Dispose();
                    temp = null;
                    GC.Collect(1);
                }
            }
            // Detect whether the Downloaded playlist does exist for this gather
            if (Global.DownloadedTracksPlaylist.Exists)
            {
                // The playlist does exist , add it to the available playlist list
                data.AddItem(new("dd_img", Global.Resources.GetByteArrayResource("SpecialDownloadsPlaylistIcon")) { Type = CommandMetadataItemType.Image });
                DebugProvider.WriteLine("PlaylistGatherer: Adding special downloads playlist.");
                DownloadedFilesPlaylist ppl = null;
                try
                {
                    ppl = new(Global.DownloadedTracksDirectory, Global.DownloadedTracksPlaylist);
                    DebugProvider.WriteLine($"PlaylistGatherer: Playlist {Global.DownloadedTracksPlaylist.Name} is valid , adding it to the playlist list");
                    // Note here that we do not load any image from the cache because it is a special playlist and has elsewise it's own unique icon
                    imgct++;
                    System.String n = System.String.Empty;
                    if (ppl.Metadata is not null && ppl.Metadata.TryGetValue("CurrentTrackFileName", out System.Object obj)) {
                        n = obj as System.String;
                    } else if (ppl.CurrentTrack is not null) {
                        n = ppl.CurrentTrack.Name;
                    }
                    data.AddItem(new WindowsListViewElement() {
                        PrimaryData = ppl.PlaylistName,
                        SecondaryData = [Global.DownloadedTracksPlaylist.CreationTimeUtc.ToString(), ppl.TracksContained.Count.ToString(), n],
                        ImageIndex = imgct - 1,
                        AdditionalData = $"special-{Global.DownloadedTracksPlaylist.Name}"
                    });
                } catch (Exception e) {
                    // On exception , do the classic handling
                    DebugProvider.WriteLine($"PlaylistGatherer: Cannot load special downloads playlist due to: {e}");
                    data.AddItem(new WindowsListViewElement() {
                        PrimaryData = $"{Global.DownloadedTracksPlaylist.GetNameOnly()} (Cannot load)",
                        SecondaryData = [Global.DownloadedTracksPlaylist.CreationTimeUtc.ToString(), "" , ""],
                        ImageIndex = imgct - 1,
                        AdditionalData = $"special-{Global.DownloadedTracksPlaylist.Name}"
                    });
                } finally {
                    ppl?.Dispose();
                    GC.Collect(1);
                }
            }
            // Create the columns
            data.AddItem(new WindowsListViewColumn("Name", Global.WD_PlaylistName, false));
            data.AddItem(new WindowsListViewColumn("Date Created", Global.WD_CreationTime, false));
            data.AddItem(new WindowsListViewColumn("Tracks", 48, false));
            data.AddItem(new WindowsListViewColumn("Last Track", Global.WD_LastPlaylistTrack, false));
            DebugProvider.WriteLine($"PlaylistGatherer: Playlist gather finished cleanly.");
            if (lvmode == ListViewMode.Files)
            {
                // If we are coming from such mode , re-set the First-Time-Exploration flag.
                state |= RCUEngineStateFlags.IsFirstFileExploration;
            }
            lvmode = ListViewMode.PlaylistSelect;
            RaiseSendCommand(CommonSendCommandTypes.ClearExplorationScreen);
            RaiseSendCommand(WindowsRCUSendCommandTypes.UpdateSelectedMode);
            RaiseSendCommand(CommonSendCommandTypes.LoadExplorationScreen, data);
            RaiseSendCommand(CommonSendCommandTypes.ClearWaitMessage);
            var s = statinst.Get("NumberOfPlaylists");
            s.Value = playlists.Count.ToUInt64();
            statinst.Update(s);
            playlists.Clear();
            playlists = null;
            RaiseSendCommand(WindowsRCUSendCommandTypes.RemoveWaitCursor);
            drp.UpdateInfo(CreateInfoData(
                Global.Resources.GetStringResource("DRP_DETAILS_ON_PLAYLISTS_SELECTION"),
                Global.Resources.GetStringResource("DRP_STATE_PICKING_PLAYLIST")));
            GC.Collect(GC.MaxGeneration, GCCollectionMode.Aggressive, true);
        }

        private void G_Repeat(PlaybackStoppedEventInfo e)
        {
            switch (e.Reason)
            {
                case PlaybackStoppedReason.UserRequest:
                    return;
                case PlaybackStoppedReason.Exception:
                    if (e.Exception is SpecialStopButtonAssertionException) { return; }
                    MusicPlayerHelper.ShowErrorMessage($"Cannot repeat the audio file: {e}");
                    return;
            }
            trkidx++;
            if (trkidx >= current.TracksContained.Count) {
                trkidx = 0;
            }
            tasker.StopAndProcessAll();
            tasker.Add(DestroyPlayerInstance);
            tasker.Add(LoadPlayer_Unsafe, trkidx, -1);
            tasker.Run();
        }

        private void DestroyPlayerInstance()
        {
            if (lvmode != ListViewMode.LoadedPlaylist) {
                DebugProvider.WriteLine($"RCU: Invalid attempt to dispose a player outside it's load context. Command will not be executed, but this can leave the player in a corrupted state.");
                return;
            }
            playerlock.Wait();
            try
            {
                if (currentplayer is not null)
                {
                    DebugProvider.WriteLine($"RCU: Destroying active player instance.");
                    if (state.HasFlag(RCUEngineStateFlags.IsShuttingDown) == false)
                    {
                        if (repmode == RepeatMode.All)
                        {
                            currentplayer.PlaybackStopped -= G_Repeat;
                        }
                    }
                    DebugProvider.WriteLine("RCU: Player instance destroy phase 1 begun...");
                    RaiseSendCommand(CommonSendCommandTypes.DetachPlayerEvents);
                    DebugProvider.WriteLine("RCU: Player instance destroy phase 2 begun...");
                    currentplayer.Dispose();
                    DebugProvider.WriteLine("RCU: Player instance destroy phase 3 begun...");
                    currentplayer = null;
                    DebugProvider.WriteLine("RCU: Player instance was successfully destroyed.");
                    drp.UpdateInfo(CreateInfoData(
                        System.String.Format(
                        Global.Resources.GetStringResource("DRP_DETAILS_PLAYBACK_STOPPED"),
                        this.GetStringAttribute("PlaylistNameField")),
                        Global.Resources.GetStringResource("DRP_STATE_STOPPED")));
                    GC.Collect(1, GCCollectionMode.Forced, true);
                }
                if (state.HasFlag(RCUEngineStateFlags.IsShuttingDown) == false) { RaiseSendCommand(CommonSendCommandTypes.ClearPlayerScreen); }
            } finally {
                playerlock.Release();
            }
        }

        private void DestroyActivePlaylist()
        {
            if (current is not null)
            {
                DebugProvider.WriteLine($"RCU: Disposing playlist {this.GetStringAttribute("PlaylistNameField")}...");
                DestroyPlayerInstance();
                SetAttribute("PlaylistNameField", null);
                SetAttribute("OpenedPlaylistFullPath", null);
                current?.Dispose();
                current = null;
            }
        }

        private void LoadPlayer_Unsafe(System.Int32 idx , System.Int64 proctimeints)
        {
            playerlock.Wait();
            if (currentplayer is not null) {
                DebugProvider.WriteLine("RCU: Attempted to instantiate the same player instance twice. This could later cause stale object issues. Effectively bailing out this command.");
                playerlock.Release();
                return;
            }
            try {
                RaiseSendCommand(CommonSendCommandTypes.ClearPlayerScreen);
                current.CurrentTrack = current.TracksContained[idx];
                if (current.CurrentTrack is null) { RaiseMessage(Global.Resources.GetStringResource("Error_CurrentTrackNotLoaded_Null")); }
                RaiseSendCommand(WindowsRCUSendCommandTypes.InvokeWaitCursor);
                System.Threading.Thread.Sleep(300);
                DebugProvider.WriteLine($"RCU: Loading player instance for physical location RCU/playlist-{this.GetStringAttribute("PlaylistNameField")}/{idx}");
                DebugProvider.WriteLine($"RCU: The player engine will use the {device.FriendlyName} audio device with native id {device.ID}.");
                InstanceData DT = new(current.CurrentTrack, device) {
                    Latency = Global.Latency,
                    VolumeLeft = Global.LeftChannelVolume,
                    VolumeRight = Global.RightChannelVolume,
                };
                DebugProvider.WriteLine($"RCU: Injecting audio session with parameters lat={DT.Latency} vl={DT.VolumeLeft} vr={DT.VolumeRight}");
                currentplayer = new(DT , extensionengine.IsPackageLoaded("mp-default-package") , extensionengine);
                if (repmode == RepeatMode.All)
                {
                    currentplayer.PlaybackStopped += G_Repeat;
                }
                DebugProvider.WriteLine($"RCU: Creating audio session...");
                currentplayer.Create();
                if (repmode == RepeatMode.One) {
                    currentplayer.Repeat();
                }
                RaiseSendCommand(CommonSendCommandTypes.LoadPlayerScreen);
                DebugProvider.WriteLine($"RCU: Audio session on native id {device.ID} succeeded.");
                RaiseSendCommand(CommonSendCommandTypes.AttachPlayerEvents);
                DebugProvider.WriteLine($"RCU: Audio session loaded on {device.ID} with parameters: ch={currentplayer.Channels} brt={currentplayer.BitsPerSample} crt={currentplayer.CurrentReachedTime.Ticks} lrt={currentplayer.RemainingTime.Ticks}");
                RaiseSendCommand(WindowsRCUSendCommandTypes.RemoveWaitCursor);
                trkidx = idx;
                RaiseSendCommand(CommonSendCommandTypes.UpdateSelectedIndex);
                currentplayer.Play();
                if (proctimeints > -1) {
                    TimeSpan ts = new(proctimeints);
                    if (ts < currentplayer.TotalTrackTime) {
                        DebugProvider.WriteLine("RCU: Applying current track time as provided by the request.");
                        currentplayer.CurrentReachedTime = ts;
                    }
                }
            } catch (System.Exception e) {
                switch (e)
                {
                    case OutOfMemoryException:
                        if (current is ArchivedTrackPlaylist)
                        {
                            // It is sure that this OOM was occured from the playlists' Memory Stream manager.
                            // If such is the case , it means that the Music Player cannot give other memory to load the track , so say a more specialized message instead.
                            RaiseMessage(System.String.Format(Global.Resources.GetStringResource("TrackCannotBeLoadedInMemory_ArchivedMode"), current?.CurrentTrack?.Name));
                            RaiseSendCommand(WindowsRCUSendCommandTypes.RemoveWaitCursor);
                            return;
                        }
                        break;
                    case ExceptionSystem.OptionalFeatureAbsentException ex:
                        // It is an optional capability that is missing from user's side.
                        RaiseMessage(System.String.Format(Global.Resources.GetStringResource("PlayerInstance_OptionalFeatureRequired"), ex.MissingFeature));
                        RaiseSendCommand(WindowsRCUSendCommandTypes.RemoveWaitCursor);
                        return;
                }
                RaiseSendCommand(WindowsRCUSendCommandTypes.RemoveWaitCursor);
                RaiseSendCommand(CommonSendCommandTypes.DetachPlayerEvents);
                RaiseSendCommand(CommonSendCommandTypes.ClearPlayerScreen);
                if (repmode == RepeatMode.All)
                {
                    trkidx++;
                    tasker.Add(LoadPlayer_Unsafe, trkidx, -1); // LoadPlayer is always called through the tasker so it will not cause contention on the semaphore
                    return;
                }
                RaiseMessage(System.String.Format(Global.Resources.GetStringResource("ErrCannotLoadTrack"), current?.CurrentTrack?.Name)
#if DEBUG
                    + $"\nException Data:\n{e}"
#endif
                    );
                return;
            } finally {
                playerlock.Release();
            }
            // These must be executed only and only if the player creation succeeded.
            drp.UpdateInfo(CreateInfoData($"On Playlist {current?.PlaylistName} Track ordinal {current.GetCurrentTrackIndex()} is loaded.", "Listening" , true));
            if (current.Metadata is not null)
            {
                current.Metadata["NumberOfTracksPlayed"] = unchecked(((System.UInt64)current.Metadata["NumberOfTracksPlayed"]) + 1);
                System.String tn = $"TimesPlayed_{current.CurrentTrack.Name}";
                if (current.Metadata.TryGetValue(tn , out System.Object obj)) {
                    current.Metadata[tn] = unchecked(((System.UInt32)obj) + 1); // unchecked so that this never fails.
                } else {
                    current.Metadata.Add(new(tn, 1U));
                }
            }
            var s = statinst.Get("NumberOfTracksPlayed");
            s.IncrementNumericValue();
            statinst.Update(s);
            s = statinst.Get("LastPlayedTrackFileName");
            s.Value = current.CurrentTrack.Name;
            statinst.Update(s);
        }

        private void SafeLoadPlayer(System.Int32 idx)
        {
            if (current is null)
            {
                RaiseMessage(Global.Resources.GetStringResource("ErrPlaylistNotLoaded"));
                return;
            }
            if (currentplayer is not null)
            {
                DestroyPlayerInstance();
                currentplayer = null;
            }
            tasker.Add(LoadPlayer_Unsafe, idx , -1);
        }

        private void LoadPlaylistAndLastTrack(System.Object name)
        {
            LoadPlaylist(name);
            if (current is null) { return; }
            System.Int32 ti;
            try {
                ti = current.GetCurrentTrackIndex();
            } catch (ArgumentNullException) {
                return;
            }
            DebugProvider.WriteLine("PlaylistGatherer: Loading last track...");
            tasker.Add(LoadPlayer_Unsafe, ti, current.CurrentTrackProcessedTime.Ticks);
            RaiseSendCommand(CommonSendCommandTypes.ClearWaitMessage);
        }

        private void LoadDownloadsPlaylist()
        {
            if (currentplayer is not null) { DestroyPlayerInstance(); }
            RaiseSendCommand(CommonSendCommandTypes.ClearExplorationScreen);
            FileStream FS = null;
            try
            {
                RaiseMessage(Global.Resources.GetStringResource("WaitMsg_DownloadPlaylistLoad") , CommonSendCommandTypes.ThrowWaitMessage);
                FS = Global.DownloadedTracksPlaylist.OpenRead();
                current?.Dispose();
                current = new DownloadedFilesPlaylist(Global.DownloadedTracksDirectory, Global.DownloadedTracksPlaylist);
            } catch (System.Exception e) {
                DebugProvider.WriteLine($"PlaylistGatherer: Could not load physical playlist with name {Global.DownloadedTracksPlaylist.Name} because of \n{e}");
                RaiseSendCommand(WindowsRCUSendCommandTypes.RemoveWaitCursor);
                RaiseSendCommand(CommonSendCommandTypes.ClearWaitMessage);
                DebugProvider.WriteLine($"PlaylistGatherer: Exiting with error of type {e.GetType().AssemblyQualifiedName}.");
                RaiseMessage($"Could not load the playlist {Global.DownloadedTracksPlaylist.Name}. An unexpected error occured while loading this playlist.");
                lvmode = ListViewMode.PlaylistSelect;
                RaiseSendCommand(WindowsRCUSendCommandTypes.UpdateSelectedMode);
                return;
            } finally {
                GC.Collect(1);
                FS?.Dispose();
                FS = null;
            }
            RaiseMessage(System.String.Format(Global.Resources.GetStringResource("WaitMsg_LoadingPlaylist"), this.GetStringAttribute("PlaylistNameField")), CommonSendCommandTypes.ThrowWaitMessage);
            // OK. Now load the exploration screen.
            CommandMetadata data = new();
            // Prepare columns.
            data.AddItem(new WindowsListViewColumn("Track Name", Global.WD_TrackName));
            data.AddItem(new WindowsListViewColumn("Track Creation Time", Global.WD_TrackCreationTime));
            data.AddItem(new WindowsListViewColumn("Track Size", Global.WD_TrackSize, false, true));
            data.AddItem(new WindowsListViewColumn("Track Contributing Artists", Global.WD_TrackContributingArtists));
            data.AddItem(new WindowsListViewColumn("Track Number", Global.WD_TrackNumber, false, true));
            data.AddItem(new WindowsListViewColumn("Encoded By", Global.WD_EncodedBy));
            data.AddItem(new WindowsListViewColumn("Album", Global.WD_Album));
            data.AddItem(new("dd_img", Global.Resources.GetByteArrayResource("Audiofile")) { Type = CommandMetadataItemType.Image });
            // Now add all the files.
            SavedDataTag rdr = null;
            System.Int32 loaded = 0, imgidxloaded = 1;
            IPlaylistFile file = null;
            for (System.Int32 I = 0; I < current.TracksContained.Count; I++)
            {
                file = current.TracksContained[I];
                if (file.Exists == false) { continue; }
                rdr = current.GetTagFromFile(file);
                if (rdr is null || rdr.DataExist == false)
                {
                    data.AddItem(new WindowsListViewElement() {
                        PrimaryData = file.GetNameOnly(),
                        SecondaryData = [file.CreationTimeUtc.ToString() ,
                            file.GetFileSizeAsFriendlyString() ,
                            "" , (I+1).ToString() , "" , ""],
                        ImageIndex = 0
                    });
                }
                else
                {
                    System.Boolean exec;
                    if (exec = rdr.Image is not null)
                    {
                        data.AddItem(new("dd_img", rdr.Image) { Type = CommandMetadataItemType.Image });
                    }
                    data.AddItem(new WindowsListViewElement() {
                        PrimaryData = System.String.IsNullOrWhiteSpace(rdr.Title2) ? file.GetNameOnly() : rdr.Title2,
                        SecondaryData = [file.CreationTimeUtc.ToString() ,
                            file.GetFileSizeAsFriendlyString() ,
                            rdr.ContributingArtists, System.String.IsNullOrWhiteSpace(rdr.TrackNumber) ? (I+1).ToString() : rdr.TrackNumber ,
                            rdr.EncodedBy, rdr.AlbumName],
                        ImageIndex = exec ? imgidxloaded : 0
                    });
                    if (exec) { imgidxloaded++; }
                }
                loaded++;
            }
            if (loaded < current.TracksContained.Count)
            {
                RaiseMessage(System.String.Format(Global.Resources.GetStringResource("CouldNotLoadTracks"), current.TracksContained.Count - loaded));
                DebugProvider.WriteLine($"PlaylistGatherer: Note that the playlist {Global.DownloadedTracksPlaylist.Name} is loaded partially because not all files are resolved.");
                DebugProvider.WriteLine($"PlaylistGatherer: {loaded}/{current.TracksContained.Count} files were actually loaded.");
            }
            if (loaded == 0)
            {
                RaiseMessage(Global.Resources.GetStringResource("CouldNotLoadAnyTracks"));
                DebugProvider.WriteLine($"PlaylistGatherer: The playlist {Global.DownloadedTracksPlaylist.Name} failed to load any tracks.");
                DebugProvider.WriteLine($"PlaylistGatherer: This could mean that the playlist based on the available resources , is invalid.");
            }
            lvmode = ListViewMode.LoadedPlaylist;
            SetAttribute("PlaylistNameField", current.PlaylistName);
            RaiseSendCommand(CommonSendCommandTypes.ClearExplorationScreen);
            RaiseSendCommand(WindowsRCUSendCommandTypes.UpdateSelectedMode);
            RaiseSendCommand(CommonSendCommandTypes.LoadExplorationScreen, data);
            RaiseSendCommand(WindowsRCUSendCommandTypes.RemoveWaitCursor);
            RaiseSendCommand(CommonSendCommandTypes.ClearWaitMessage);
            GC.Collect(1);
            drp.UpdateInfo(CreateInfoData(
                System.String.Format(
                    Global.Resources.GetStringResource("DRP_DETAILS_ONPLAYLIST"),
                    this.GetStringAttribute("PlaylistNameField")),
                Global.Resources.GetStringResource("DRP_STATE_STOPPED")));
            DebugProvider.WriteLine($"PlaylistGatherer: Playlist with name {Global.DownloadedTracksPlaylist.Name} loaded into context cleanly.");
        }

        private void LoadPlaylist(System.Object nam)
        {
            System.String name = nam is null ? System.String.Empty : nam.ToString();
            RaiseSendCommand(WindowsRCUSendCommandTypes.InvokeWaitCursor);
            if (System.String.IsNullOrEmpty(name)) { name = Global.LastPlaylistName; }
            DebugProvider.WriteLine($"PlaylistGatherer: Attempting to load playlist with name {name}");
            Microsoft.IO.FileInfo fi = null;
            System.Int32 idx = name.IndexOf("special-");
            if (idx == 0) {
                System.String str = name.Substring(idx + 8);
                if (str == Global.DownloadedTracksPlaylist.Name)
                {
                    DebugProvider.WriteLine($"PlaylistGatherer: Found downloads special playlist because the strings match: {str}");
                    LoadDownloadsPlaylist();
                    return;
                }
            }
            if (Path.HasExtension(name))
            {
                fi = new(Path.Join(Global.PlaylistsDirectory.FullName, name));
                DebugProvider.WriteLine($"PlaylistGatherer: Found file {name} loaded from playlists directory.");
                goto G_Load;
            }
            foreach (var stringext in Global.Resources.GetStringResource("AllPlaylistExtensions").Split(';'))
            {
                fi = new(Path.Join(Global.PlaylistsDirectory.FullName, $"{name}{stringext}"));
                if (fi.Exists) { break; }
            }
        G_Load:
            System.Threading.Thread.Sleep(40);
            if (fi is null || fi.Exists == false)
            {
                DebugProvider.WriteLine($"PlaylistGatherer: Could not load physical playlist with name {name}.");
                RaiseMessage(System.String.Format(Global.Resources.GetStringResource("CannotLocatePlaylist"), name));
                tasker.Add(GatherPlaylists);
                RaiseSendCommand(WindowsRCUSendCommandTypes.RemoveWaitCursor);
                return;
            }
            fi.Refresh();
            if (currentplayer is not null) { DestroyPlayerInstance(); }
            RaiseSendCommand(CommonSendCommandTypes.ClearExplorationScreen);
            FileStream FS = null;
            try
            {
                RaiseMessage(System.String.Format(Global.Resources.GetStringResource("WaitMsg_LoadingPlaylist"), fi.GetNameOnly()) , CommonSendCommandTypes.ThrowWaitMessage);
                FS = fi.OpenRead();
                current?.Dispose();
                current = new TrackPlayList(FS, name);
                System.String ne;
                System.Int32 nidx = name.LastIndexOf('.');
                if (nidx == -1) { ne = name; } else { ne = name.Remove(nidx); }
                SetAttribute("PlaylistNameField" , ne);
                ne = null;
                if (current.AudioTagsDetermined == false)
                {
                    RaiseMessage(Global.Resources.GetStringResource("DeterminingAudioTags"), CommonSendCommandTypes.ThrowWaitMessage);
                    if (current.CurrentTrack is null && current.TracksContained.Count > 0) { current.CurrentTrack = current.TracksContained[0]; }
                    current.DetermineAudioTags();
                }
            } catch (System.Exception e) {
                DebugProvider.WriteLine($"PlaylistGatherer: Could not load physical playlist with name {name} because of \n{e}");
                RaiseSendCommand(WindowsRCUSendCommandTypes.RemoveWaitCursor);
                RaiseSendCommand(CommonSendCommandTypes.ClearWaitMessage);
                DebugProvider.WriteLine($"PlaylistGatherer: Exiting with error of type {e.GetType().AssemblyQualifiedName}.");
                RaiseMessage($"Could not load the playlist {name}. An unexpected error occured while loading this playlist.");
                lvmode = ListViewMode.PlaylistSelect;
                RaiseSendCommand(WindowsRCUSendCommandTypes.UpdateSelectedMode);
                return;
            } finally {
                GC.Collect(1);
                FS?.Dispose();
                FS = null;
            }
            RaiseMessage(System.String.Format(Global.Resources.GetStringResource("WaitMsg_LoadingPlaylist"), fi.GetNameOnly()) , CommonSendCommandTypes.ThrowWaitMessage);
            // OK. Now load the exploration screen.
            CommandMetadata data = new();
            // Prepare columns.
            data.AddItem(new WindowsListViewColumn("Track Name", Global.WD_TrackName));
            data.AddItem(new WindowsListViewColumn("Track Creation Time", Global.WD_TrackCreationTime));
            data.AddItem(new WindowsListViewColumn("Track Size", Global.WD_TrackSize, false, true));
            data.AddItem(new WindowsListViewColumn("Track Contributing Artists", Global.WD_TrackContributingArtists));
            data.AddItem(new WindowsListViewColumn("Track Number", Global.WD_TrackNumber, false, true));
            data.AddItem(new WindowsListViewColumn("Encoded By", Global.WD_EncodedBy));
            data.AddItem(new WindowsListViewColumn("Album", Global.WD_Album));
            data.AddItem(new("dd_img", Global.Resources.GetByteArrayResource("Audiofile")) { Type = CommandMetadataItemType.Image });
            // Now add all the files.
            SavedDataTag rdr = null;
            System.Int32 loaded = 0, imgidxloaded = 1;
            IPlaylistFile file = null;
            for (System.Int32 I = 0; I < current.TracksContained.Count; I++)
            {
                file = current.TracksContained[I];
                if (file.Exists == false) { continue; }
                rdr = current.GetTagFromFile(file);
                if (rdr is null || rdr.DataExist == false)
                {
                    data.AddItem(new WindowsListViewElement() {
                        PrimaryData = file.GetNameOnly(),
                        SecondaryData = [file.CreationTimeUtc.ToString() ,
                            file.GetFileSizeAsFriendlyString() ,
                            "" , (I+1).ToString() , "" , ""],
                        ImageIndex = 0
                    });
                } else {
                    System.Boolean exec;
                    if (exec = rdr.Image is not null)
                    {
                        data.AddItem(new("dd_img", rdr.Image) { Type = CommandMetadataItemType.Image });
                    }
                    data.AddItem(new WindowsListViewElement() {
                        PrimaryData = System.String.IsNullOrWhiteSpace(rdr.Title2) ? file.GetNameOnly() : rdr.Title2,
                        SecondaryData = [file.CreationTimeUtc.ToString() ,
                            file.GetFileSizeAsFriendlyString() ,
                            rdr.ContributingArtists, System.String.IsNullOrWhiteSpace(rdr.TrackNumber) ? (I+1).ToString() : rdr.TrackNumber ,
                            rdr.EncodedBy, rdr.AlbumName],
                        ImageIndex = exec ? imgidxloaded : 0
                    });
                    if (exec) { imgidxloaded++; }
                }
                loaded++;
            }
            if (loaded < current.TracksContained.Count)
            {
                RaiseMessage(System.String.Format(Global.Resources.GetStringResource("CouldNotLoadTracks"), current.TracksContained.Count - loaded));
                DebugProvider.WriteLine($"PlaylistGatherer: Note that the playlist {name} is loaded partially because not all files are resolved.");
                DebugProvider.WriteLine($"PlaylistGatherer: {loaded}/{current.TracksContained.Count} files were actually loaded.");
            }
            if (loaded == 0)
            {
                RaiseMessage(Global.Resources.GetStringResource("CouldNotLoadAnyTracks"));
                DebugProvider.WriteLine($"PlaylistGatherer: The playlist {name} failed to load any tracks.");
                DebugProvider.WriteLine($"PlaylistGatherer: This could mean that the playlist based on the available resources , is invalid.");
            }
            lvmode = ListViewMode.LoadedPlaylist;
            SetAttribute("PlaylistNameField", current.PlaylistName);
            SetAttribute("OpenedPlaylistFullPath" , fi.FullName);
            fi = null;
            RaiseSendCommand(CommonSendCommandTypes.ClearExplorationScreen);
            RaiseSendCommand(WindowsRCUSendCommandTypes.UpdateSelectedMode);
            RaiseSendCommand(CommonSendCommandTypes.LoadExplorationScreen, data);
            RaiseSendCommand(WindowsRCUSendCommandTypes.RemoveWaitCursor);
            RaiseSendCommand(CommonSendCommandTypes.ClearWaitMessage);
            GC.Collect(1);
            DebugProvider.WriteLine($"PlaylistGatherer: Playlist with name {name} loaded into context cleanly.");
            DebugProvider.WriteLine($"PlaylistGatherer: Running statistic tasks for {name} before exiting.");
            var s = statinst.Get("LastOpenedPlaylist");
            s.Value = current.PlaylistName;
            statinst.Update(s);
            statinst.Update(new($"NumOfLoadedTracksIn_[{current.PlaylistName}]", loaded.ToUInt64()));
            drp.UpdateInfo(CreateInfoData(
                System.String.Format(
                    Global.Resources.GetStringResource("DRP_DETAILS_ONPLAYLIST"),
                    this.GetStringAttribute("PlaylistNameField")),
                Global.Resources.GetStringResource("DRP_STATE_STOPPED")));
        }

        private void SavePlaylist(FileInfo fi)
        {
            if (this.current is not TrackPlayList current) { return; }
            // Save the playlist directly , if an error occurs dispose the useless data and do not perform any special update on the playlist.
            // This is a safe update of the physical playlist file.
            FileStream FS = null;
            try {
                DebugProvider.WriteLine($"PlaylistSaver: Saving playlist {fi.Name} ...");
                if (trkidx >= 0 && trkidx < current.TracksContained.Count)
                {
                    current.CurrentTrack = current.TracksContained[trkidx];
                    if (currentplayer is not null)
                    {
                        TimeSpan crt = currentplayer.CurrentReachedTime;
                        if (currentplayer.CurrentState != PlaybackState.Stopped && crt < currentplayer.TotalTrackTime) 
                        {
                            current.CurrentTrackProcessedTime = crt;
                        }
                    }
                }
                RaiseMessage(Global.Resources.GetStringResource("WaitMsg_SavingPlaylistWait"), CommonSendCommandTypes.ThrowWaitMessage);
                RaiseSendCommand(WindowsRCUSendCommandTypes.InvokeWaitCursor);
                DestroyPlayerInstance();
                DebugProvider.WriteLine($"PlaylistSaver: Initial path string is {fi.FullName}...");
                FS = fi.Create();
                FS.Position = 0;
                DebugProvider.WriteLine("PlaylistSaver: Saving data...");
                current.SaveAsPlaylist(FS);
                if (FS.Length == 0 || FS.Position == 0) { throw new FormatException(Global.Resources.GetStringResource("ResultingDatLengthCannotBeZero")); }
            } catch (Exception e) {
                RaiseSendCommand(CommonSendCommandTypes.ClearWaitMessage);
                DebugProvider.WriteLine($"PlaylistSaver: Exiting with exception: {e}");
                RaiseMessage(System.String.Format(Global.Resources.GetStringResource("PlaylistNotSavedErrorMsg"), e));
            } finally {
                try {
                    FS?.Dispose();
                    FS = null;
                } finally {
                    RaiseSendCommand(CommonSendCommandTypes.ClearWaitMessage);
                    RaiseSendCommand(WindowsRCUSendCommandTypes.RemoveWaitCursor);
                }
            }
            fi = null;
            DebugProvider.WriteLine("PlaylistSaver: Exiting cleanly.");
        }

        private void SaveDownloadedPlaylist()
        {
            if (current is not DownloadedFilesPlaylist dfp) { return; }
            // Save the playlist directly , if an error occurs dispose the useless data and do not perform any special update on the playlist.
            // This is a safe update of the file intended only for playlist data refresh.
            try
            {
                RaiseSendCommand(WindowsRCUSendCommandTypes.InvokeWaitCursor);
                if (trkidx >= 0 && trkidx < current.TracksContained.Count)
                {
                    current.CurrentTrack = current.TracksContained[trkidx];
                }
                DebugProvider.WriteLine("PlaylistSaver: Saving special Downloaded Playlist...");
                dfp.SaveCurrentPlaylistState();
                DebugProvider.WriteLine("PlaylistSaver: Done!");
            } catch (System.Exception e) {
                DebugProvider.WriteLine($"PlaylistSaver: Exiting with exception: {e}");
                RaiseMessage(System.String.Format(Global.Resources.GetStringResource("PlaylistNotSavedErrorMsg"), e));
            } finally {
                RaiseSendCommand(WindowsRCUSendCommandTypes.RemoveWaitCursor);
            }
            DebugProvider.WriteLine("PlaylistSaver: Exiting cleanly.");
        }

        private System.Boolean SavePlaylist()
        {
            if (current is null) { return false; }
            if (current is ArchivedTrackPlaylist) { return true; }
            if (current is DownloadedFilesPlaylist)
            {
                SaveDownloadedPlaylist();
                return true;
            }
            System.String fp = this.GetStringAttribute("OpenedPlaylistFullPath");
            if (fp is null) { return false; }
            FileInfo file = new(fp);
            if (file.Exists == false)
            {
                RaiseMessage(System.String.Format(Global.Resources.GetStringResource("CannotLocatePlaylist"), file.GetNameOnly()));
                return false;
            }
            if (currentplayer is not null && currentplayer.CurrentState >= PlaybackState.Playing) { DestroyPlayerInstance(); }
            SavePlaylist(file);
            return true;
        }

        private void ChangeRepeatMode()
        {
            repmode++;
            if (repmode > RepeatMode.All)
            {
                repmode = RepeatMode.No;
            }
            ChangeRepeatMode(repmode);
        }

        private void ChangeRepeatMode(RepeatMode mode)
        {
            repmode = mode;
            RaiseSendCommand(WindowsRCUSendCommandTypes.ChangeRepeatModeImage);
            switch (repmode)
            {
                case RepeatMode.No:
                    if (currentplayer is not null)
                    {
                        if (currentplayer.RepeatEnabled) { currentplayer.Repeat(); }
                        currentplayer.PlaybackStopped -= G_Repeat;
                    }
                    break;
                case RepeatMode.One:
                    if (currentplayer is not null && currentplayer.RepeatEnabled == false)
                    {
                        currentplayer.Repeat();
                    }
                    break;
                case RepeatMode.All:
                    // The All repeat mode will be enabled through the player event.
                    // If the player has not registered this event , it is done now.
                    if (currentplayer is not null)
                    {
                        currentplayer.PlaybackStopped += G_Repeat;
                        if (currentplayer.RepeatEnabled) { currentplayer.Repeat(); }
                    }
                    break;
            }
        }

        private void CreateStatistics()
        {
            statinst.AddRange([
                new("StatisticsVersion" , 1L),
                new("NumberOfTracksPlayed" , 0UL),
                new("NumberOfTracksRemoved", 0UL),
                new("LastPlayedTrackFileName" , System.String.Empty),
                new("LastOpenedPlaylist" , System.String.Empty),
                new("NumberOfDispatchedCommands" , 0UL),
                new("NumberOfLifeTimeTracksPlayed" , 0UL),
                new("NumberOfPlaylists" , 0UL),
                new("NumberOfControllerConnectedTimes" , 0UL)
            ]);
        }

        private void PrepareAudioLibrary()
        {
            AudioLibrary.MMDevice.MMDeviceEnumerator mmde = null;
            try {
                // Initialize the Windows Audio Library.
                WindowsAudioLibrary.Initialize();
                mmde = new();
                // Create the MMDevice object in the OperationsTasker thread instead.
                device = mmde.GetDevice(Global.AudioDeviceId);
                DebugProvider.WriteLine($"RCU: MMDevice object with ID {device.ID} was created on the Tasker thread!!");
            } catch (Exception ex) {
                RecieveCommand(new(CommonRecieveCommandTypes.HardFailAppMustClose) { Data = ex });
                return;
            } finally {
                mmde?.Dispose();
                mmde = null;
            }
        }

        private void RCUENGINE_INIT_MODE()
        {
            // Create statistics, if the cache file was deleted or on any other failure
            if (statinst.Count == 0) { tasker.Add(CreateStatistics); }
            // Prepare audio backend.
            tasker.Add(PrepareAudioLibrary);
            if (Global.PlaylistsDirectory.GetFiles().Length == 0) 
            {
                lvmode = ListViewMode.Files;
                tasker.Add(GatherFilesAndDirs , basedir);
            } else {
                if (System.String.IsNullOrEmpty(Global.LastPlaylistName)) {
                    lvmode = ListViewMode.PlaylistSelect;
                    tasker.Add(GatherPlaylists);
                } else {
                    if (Global.InitializeLastTrackFromLastPlaylist) {
                        tasker.Add(LoadPlaylistAndLastTrack, Global.LastPlaylistName);
                    } else {
                        tasker.Add(LoadPlaylist, Global.LastPlaylistName);
                    }
                }
            }
        }

        private void ModPlaylistPreferences()
        {
            if (current is TrackPlayList temp)
            {
                PlaylistPreferencesEditor editor = null;
                try {
                    editor = new(ref temp) { ForeColor = System.Drawing.Color.White, BackColor = System.Drawing.Color.Black };
                    editor.ShowDialog(MusicPlayerHelper.WinFormsHandle);
                } finally {
                    editor?.Dispose();
                    editor = null;
                }
            } else if (current is not null) {
                MusicPlayerHelper.ShowErrorResourceMessage("Error_PrefEditingOnlyOnPhysical", gamepad);
            }
        }

        private void DeletePlaylist(System.String Name)
        {
            if (MusicPlayerHelper.ShowResourceQuestionMessage("Question_DeletePlaylist", Name))
            {
                try {
                    DestroyActivePlaylist();
                    DebugProvider.WriteLine($"PlaylistDeleter: Deleting playlist named as {Name}...");
                    System.String pln = Name;
                    System.Int32 idx = pln.LastIndexOf('.');
                    if (idx > -1) { pln = pln.Remove(idx); }
                    DebugProvider.WriteLine($"PlaylistDeleter: Clean playlist name appears to be {pln} ...");
                    Caches.ExportedPlaylistsCache.ExportedPlaylistCacheInstance inst = new();
                    inst.LoadCacheFromExportedDirectory(Global.ExportedPlaylistTracksDirectory);
                    if (inst.IsExportedPlaylist(pln))
                    {
                        DebugProvider.WriteLine("PlaylistDeleter: This playlist is an exported one , deleting associated folder from the cache.");
                        Dialogs.FileOperationDialog fop = null;
                        try {
                            fop = new(Dialogs.FileOperationType.DeleteDirectory, Global.ExportedPlaylistTracksDirectory.GetSubDirectory(inst.GetPlaylistFolder(pln)).FullName, null) {
                                BackgroundColor = Global.ExplorationViewBackColor,
                                ForegroundColor = Global.ExplorationViewForeColor
                            };
                            fop.ShowDialog(MusicPlayerHelper.WinFormsHandle);
                            if (fop.HasSuccessfullyCompleted == false) { return; }
                        } catch (System.Exception e) {
                            MusicPlayerHelper.ShowErrorResourceMessage("Error_ExportedPlaylistDeletion_Failed_WithEx", gamepad, e);
                            return;
                        } finally {
                            fop?.Dispose();
                        }
                        inst.RemovePlaylistName(pln);
                        // Update the cache only if we successfully deleted the exported items.
                        DebugProvider.WriteLine("PlaylistDeleter: Updating Exported Playlist Cache state...");
                        inst.SaveCacheToExportedDirectory(Global.ExportedPlaylistTracksDirectory);
                    }
                    inst = null;
                    DebugProvider.WriteLine($"PlaylistDeleter: Deleting playlist named as: {Name}");
                    File.Delete(Path.Join(Global.PlaylistsDirectory.FullName, Name));
                    pici.RemovePlaylist(pln);
                    DebugProvider.WriteLine("PlaylistDeleter: The requested playlist was successfully deleted.");
                    GatherPlaylists();
                } catch (System.Exception ex) {
                    DebugProvider.WriteLine($"PlaylistDeleter: Recorded exception while trying to delete the playlist with name {Name}:\n{ex}");
                    return;
                }
                DebugProvider.WriteLine("PlaylistDeleter: Exiting cleanly.");
            }
        }

        private void DeleteTrack(TrackCommandData dt)
        {
            if (current is TrackPlayList playlist)
            {
                // N/A when this playlist is read-only.
                if (playlist.IsReadOnly)
                {
                    MusicPlayerHelper.ShowErrorResourceMessage("Error_ReadOnlyPlaylist", gamepad);
                    return;
                }
                if (MusicPlayerHelper.ShowResourceQuestionMessage("Question_DeleteTrack", dt.Name))
                {
                    if (currentplayer is not null && currentplayer.CurrentState > PlaybackState.Stopped)
                    {
                        DestroyPlayerInstance();
                    }
                    RaiseSendCommand(CommonSendCommandTypes.ClearPlayerScreen);
                    if (RemoveTrack(dt.Index))
                    {
                        MusicPlayerHelper.ShowResourceMessage("Info_TrackDeleted", gamepad, dt.Name);
                        current.CurrentTrack = null;
                        SyncSavePlaylist();
                        RecieveCommand(new(CommonRecieveCommandTypes.ReloadOpenedPlaylist));
                    }
                    else
                    {
                        MusicPlayerHelper.ShowErrorResourceMessage("Error_TrackNotDeleted", gamepad, dt.Name);
                    }
                }
            } else if (current is DownloadedFilesPlaylist) {
                if (MusicPlayerHelper.ShowResourceQuestionMessage("Question_DeleteTrack", dt.Name))
                {
                    if (currentplayer is not null && currentplayer.CurrentState > PlaybackState.Stopped)
                    {
                        DestroyPlayerInstance();
                    }
                    RaiseSendCommand(CommonSendCommandTypes.ClearPlayerScreen);
                    if (RemoveTrack(dt.Index))
                    {
                        MusicPlayerHelper.ShowResourceMessage("Info_TrackDeleted", gamepad, dt.Name);
                        current.CurrentTrack = null;
                        SyncSavePlaylist();
                        RecieveCommand(new(CommonRecieveCommandTypes.ReloadOpenedPlaylist));
                    } else {
                        MusicPlayerHelper.ShowErrorResourceMessage("Error_TrackNotDeleted", gamepad, dt.Name);
                    }
                }
            } else {
                MusicPlayerHelper.ShowErrorResourceMessage("Error_OperationOnlyOnPhysicalPlaylists", gamepad);
            }
        }

        private void ShowTrackTag(TrackCommandData dt)
        {
            TagInformationForm TIF = null;
            try {
                SavedDataTag rdr = current.GetTagFromFile(current.TracksContained[dt.Index]);
                if (rdr is null || rdr.DataExist == false) {
                    MusicPlayerHelper.ShowErrorResourceMessage("Error_TrackDoesNotContainTagInfo", gamepad, dt.Name);
                    return;
                }
                TIF = new(rdr, gamepad);
                TIF.ShowDialog(MusicPlayerHelper.WinFormsHandle);
            } finally {
                TIF?.Dispose();
                TIF = null;
            }
        }

        private void AddTrackToPlaylist()
        {
            if (current is ArchivedTrackPlaylist)
            {
                MusicPlayerHelper.ShowErrorResourceMessage("Error_AddingTracksInArchivedDisallowed", gamepad);
                return;
            }
            Dialogs.OpenFileDialog OFD = new();
            OFD.StartupFolder = basedir.FullName;
            OFD.DefaultFilterExtension = ".mp3";
            OFD.MultiSelect = false;
            OFD.Title = Global.Resources.GetStringResource("Message_AddAudioTrackDialog_Title");
            OFD.CheckFilePath = true;
            OFD.CheckPath = true;
            OFD.AddFilter(new(Global.Resources.GetStringResource("SupportedFormats"), "All Audio Files"));
            if (OFD.SpawnDialog())
            {
                var worker = new System.Threading.Thread(() => {
                    current.TracksContained.Add(new TypedFileInfo(OFD.FilePaths[0]));
                    DestroyPlayerInstance();
                    current.DetermineAudioTagAndAddToTagList(current.TracksContained[current.TracksContained.Count - 1]);
                    if (SyncSavePlaylist())
                    {
                        MusicPlayerHelper.ShowResourceMessage("FileAddedToPlaylist", gamepad, OFD.FilePaths[0], current.PlaylistName);
                        RecieveCommand(new(CommonRecieveCommandTypes.ReloadOpenedPlaylist));
                    } else {
                        MusicPlayerHelper.ShowResourceMessage("FileNotAddedToPlaylist", gamepad);
                    }
                });
                worker.Name = Global.Resources.GetStringResource("AddFileWorkThreadName");
                worker.Start();
            }
        }

        private void LoadEx_PlaylistArchives()
        {
            if (currentplayer is not null) { DestroyPlayerInstance(); }
            RaiseSendCommand(CommonSendCommandTypes.ClearExplorationScreen);
            RaiseSendCommand(WindowsRCUSendCommandTypes.InvokeWaitCursor);
            DebugProvider.WriteLine("PlaylistGatherer: Gathering information for loaded archived playlist...");
            if (current.CurrentTrack is null && current.TracksContained.Count > 0) { current.CurrentTrack = current.TracksContained[0]; }
            // OK. Now load the exploration screen.
            CommandMetadata data = new();
            // Prepare layout of the list view.
            data.AddItem(new WindowsListViewColumn("Track Name", Global.WD_TrackName));
            data.AddItem(new WindowsListViewColumn("Track Creation Time", Global.WD_TrackCreationTime));
            data.AddItem(new WindowsListViewColumn("Track Size", Global.WD_TrackSize, false, true));
            data.AddItem(new WindowsListViewColumn("Track Contributing Artists", Global.WD_TrackContributingArtists));
            data.AddItem(new WindowsListViewColumn("Track Number", Global.WD_TrackNumber, false, true));
            data.AddItem(new WindowsListViewColumn("Encoded By", Global.WD_EncodedBy));
            data.AddItem(new WindowsListViewColumn("Album", Global.WD_Album));
            data.AddItem(new("dd_img", Global.Resources.GetByteArrayResource("Audiofile")) { Type = CommandMetadataItemType.Image });
            // Now add all the files.
            SavedDataTag rdr;
            System.Int32 loaded = 0, imgidxloaded = 1;
            IPlaylistFile file = null;
            for (System.Int32 I = 0; I < current.TracksContained.Count; I++)
            {
                file = current.TracksContained[I];
                if (file.Exists == false) { continue; }
                rdr = current.GetTagFromFile(file);
                if (rdr is null || rdr.DataExist == false)
                {
                    data.AddItem(new WindowsListViewElement() {
                        PrimaryData = file.GetNameOnly(),
                        SecondaryData = [file.CreationTimeUtc.ToString() ,
                            file.GetFileSizeAsFriendlyString() ,
                             "" , (I+1).ToString() , "" , ""],
                        ImageIndex = 0
                    });
                } else {
                    System.Boolean exec;
                    if (exec = rdr.Image is not null)
                    {
                        data.AddItem(new("dd_img", rdr.Image) { Type = CommandMetadataItemType.Image });
                    }
                    data.AddItem(new WindowsListViewElement() {
                        PrimaryData = System.String.IsNullOrWhiteSpace(rdr.Title2) ? file.GetNameOnly() : rdr.Title2,
                        SecondaryData = [file.CreationTimeUtc.ToString() ,
                            file.GetFileSizeAsFriendlyString() ,
                            rdr.ContributingArtists, System.String.IsNullOrWhiteSpace(rdr.TrackNumber) ? (I+1).ToString() : rdr.TrackNumber ,
                            rdr.EncodedBy, rdr.AlbumName],
                        ImageIndex = exec ? imgidxloaded : 0
                    });
                    if (exec) { imgidxloaded++; }
                }
                loaded++;
            }
            RaiseSendCommand(WindowsRCUSendCommandTypes.RemoveWaitCursor);
            if (loaded < current.TracksContained.Count)
            {
                RaiseMessage(System.String.Format(Global.Resources.GetStringResource("CouldNotLoadTracks"), current.TracksContained.Count - loaded));
                DebugProvider.WriteLine($"PlaylistGatherer: Note that the archived playlist is loaded partially because not all files are resolved.");
                DebugProvider.WriteLine($"PlaylistGatherer: {loaded}/{current.TracksContained.Count} files were actually loaded.");
            }
            if (loaded == 0)
            {
                RaiseMessage(Global.Resources.GetStringResource("CouldNotLoadAnyTracks"));
                DebugProvider.WriteLine($"PlaylistGatherer: The archived playlist failed to load any tracks.");
                DebugProvider.WriteLine($"PlaylistGatherer: This could mean that the playlist based on the available resources , is invalid.");
            }
            SetAttribute("PlaylistNameField" , current.PlaylistName);
            RaiseSendCommand(CommonSendCommandTypes.LoadExplorationScreen, data);
            // Ensure to update the mode in UI
            RaiseSendCommand(WindowsRCUSendCommandTypes.UpdateSelectedMode);
            RaiseSendCommand(CommonSendCommandTypes.ClearWaitMessage);
            DebugProvider.WriteLine("PlaylistGatherer: The archived playlist loaded into context cleanly.");
        }

        private void LoadArchivedPlaylist(System.String fp)
        {
            FileInfo fi = null;
            try
            {
                fi = new(fp);
                RaiseSendCommand(WindowsRCUSendCommandTypes.InvokeWaitCursor);
                RaiseMessage(System.String.Format(Global.Resources.GetStringResource("WaitMsg_LoadingArchedPlaylist"), fi.Name), CommonSendCommandTypes.ThrowWaitMessage);
                current = new ArchivedTrackPlaylist(fi);
                lvmode = ListViewMode.LoadedPlaylist;
                tasker.Add(LoadEx_PlaylistArchives);
            } catch (System.IO.IOException ex) {
                RaiseSendCommand(CommonSendCommandTypes.ClearWaitMessage);
                RaiseMessage(System.String.Format(Global.Resources.GetStringResource("Error_IOException_Backend_ArchivedPlaylist"), (fi is null ? fp : fi.Name), ex));
            } catch (System.Exception e) {
                RaiseSendCommand(CommonSendCommandTypes.ClearWaitMessage);
                RaiseMessage(System.String.Format(Global.Resources.GetStringResource("Error_GenericException_Backend_ArchivedPlaylist"), e));
            } finally {
                RaiseSendCommand(WindowsRCUSendCommandTypes.RemoveWaitCursor);
            }
        }

        private static DiscordUpdateInfoData CreateInfoData(System.String details, System.String state , System.Boolean isinst = false)
            => new() { Assets = new() { LargeImage = "appicon", LargeText = "", SmallImage = "appicon_small", SmallText = "" }, Details = details , State = state, IsInstancedSession = isinst , Timestamps = default};

        public override System.Int32 CurrentTrackIndex
        {
            get {
                if (current is null) { return -1; }
                return current.GetCurrentTrackIndex();
            }
        }

        public System.Boolean RemoveTrack(System.Int32 trackindex)
        {
            if (current?.Remove(trackindex) == true)
            {
                var s = statinst.Get("NumberOfTracksRemoved");
                s.IncrementNumericValue();
                statinst.Update(s);
                return true;
            }
            return false;
        }

        /// <summary>
        /// Gets the number of the queued commands in the operation dispatch thread. 
        /// </summary>
        public System.Int32 QueuedCommands => tasker.QueuedWorkItems;

        public DiscordRichPresense RichPresense => drp;

        /// <summary>
        /// Gets the current exploration directory.
        /// </summary>
        public DirectoryInfo CurrentDirectory => basedir;

        /// <summary>
        /// Gets the current Player repeat mode.
        /// </summary>
        public RepeatMode RepeatMode => repmode;

        /// <summary>
        /// Gets the current exploration view mode.
        /// </summary>
        public ListViewMode ListViewMode => lvmode;

        /// <summary>
        /// Gets the current playing instance. <br />
        /// Will be null when no playback is performed
        /// </summary>
        public PlayerInstance Player => currentplayer;

        /// <summary>
        /// Gets the playlist that the player plays tracks from.
        /// </summary>
        public override IPlaylist Playlist => current;

        /// <summary>
        /// Provides the gamepad interface that is currently used. <br />
        /// Returns null if no connected interface is found.
        /// </summary>
        public GamepadReader Gamepad => gamepad;

        /// <summary>
        /// Gets the playlist icon cache instance for this RCU engine session.
        /// </summary>
        public PlaylistIconCacheInstance IconCache => pici;

        /// <summary>
        /// Gets the RCU's current extension engine instance that is used.
        /// </summary>
        public ExtensionEngine ExtensionEngineInstance => extensionengine;

        /// <summary>
        /// Gets the statistics cache instance for this RCU engine session.
        /// </summary>
        public StatisticsCacheInstance StatisticsCache => statinst;

        /// <summary>
        /// Gets a value whether this RCU engine instance is shutting down. <br />
        /// While in this state, no UI updates must be performed.
        /// </summary>
        public override System.Boolean IsShuttingDown => state.HasFlag(RCUEngineStateFlags.IsShuttingDown);

        public System.Boolean SyncSavePlaylist()
        {
            switch (current)
            {
                case null:
                    return false;
                case ArchivedTrackPlaylist:
                    DestroyPlayerInstance();
                    return true;
                case DownloadedFilesPlaylist dfp:
                    dfp.SaveCurrentPlaylistState();
                    DestroyPlayerInstance();
                    return true;
                case TrackPlayList:
                    System.String fp = this.GetStringAttribute("OpenedPlaylistFullPath");
                    if (fp is null) { return false; }
                    FileInfo file = new(fp);
                    if (file.Exists == false)
                    {
                        RaiseMessage(System.String.Format(Global.Resources.GetStringResource("CannotLocatePlaylist"), file.GetNameOnly()));
                        return false;
                    }
                    SavePlaylist(file);
                    return true;
                default:
                    return false;
            }
        }

        private void ChangeAudioDeviceInternal(System.String newdeviceid)
        {
            DebugProvider.WriteLine($"RCU: Changing target audio device with id {newdeviceid}");
            AudioLibrary.MMDevice.MMDevice newdevice = null;
            AudioLibrary.MMDevice.MMDeviceEnumerator mmdev = null;
            try {
                mmdev = new();
                newdevice = mmdev.GetDevice(newdeviceid);
            } catch (System.Exception ex) {
                DebugProvider.WriteLine($"RCU: Cannot change the audio device due to: {ex}");
                MusicPlayerHelper.ShowErrorResourceMessage("Error_CannotChangeAudioDevice", newdeviceid);
                return;
            } finally {
                mmdev?.Dispose();
                mmdev = null;
            }
            if (newdevice.State != AudioLibrary.MMDevice.DEVICE_STATE.ACTIVE || newdevice.DataFlow == AudioLibrary.MMDevice.EDataFlow.Capture)
            {
                MusicPlayerHelper.ShowErrorResourceMessage("Error_NoValidAudioDeviceFound");
                return;
            }
            DestroyPlayerInstance();
            device?.Dispose();
            device = null;
            device = newdevice;
            DebugProvider.WriteLine("RCU: Successfull audio device change.");
            return;
        }

        public void ChangeAudioDevice(System.String newdeviceid)
        {
            if (newdeviceid is null) { return; }
            tasker.Add(ChangeAudioDeviceInternal, newdeviceid);
        }

        /// <summary>
        /// Prepares the shutdown sequence. <br />
        /// Should be called as soon as the user has requested to exit.
        /// </summary>
        public override void PrepareShutdown()
        {
            DebugProvider.WriteLine($"RCU: Explicit request to shut down was requested at {SystemInfo.Now} .");
            DebugProvider.WriteLine("RCU: The engine from now on expects shut-down tasks to be running.");
            state |= RCUEngineStateFlags.IsShuttingDown;
            tasker.Add(PrepareShutdownInternal);
        }

        private void PrepareShutdownInternal()
        {
            // Instruct from now on the player UI to not listen to any new events.
            DebugProvider.WriteLine("RCU: Destroying CoreMessageDispatcher instance.");
            RaiseSendCommand(CommonSendCommandTypes.IgnoreFutureRequests);
            if (current is not null)
            {
                if (current is not ArchivedTrackPlaylist)
                {
                    System.Int32 cti = CurrentTrackIndex;
                    if (cti > -1)
                    {
                        current.CurrentTrack = current.TracksContained[cti];
                        // Save the current time on the playlist but it does not make sense to save the time 
                        // when the track has been finished playback or stopped.
                        if (currentplayer is not null && currentplayer.CurrentReachedTime.Ticks > 0)
                        {
                            if (currentplayer.CurrentState != PlaybackState.Stopped)
                            {
                                current.CurrentTrackProcessedTime = currentplayer.CurrentReachedTime;
                            }
                            else
                            {
                                current.CurrentTrackProcessedTime = new(0);
                            }
                        }
                    }
                    if (current is DownloadedFilesPlaylist dfp) {
                        dfp.SaveCurrentPlaylistState();
                        // If in case a request has been performed to initialize last tracks , on this special playlist this will never be allowed.
                        Global.InitializeLastTrackFromLastPlaylist = false;
                        Global.LastPlaylistName = System.String.Empty;
                    } else {
                        // Keep the playlist to open next time.
                        System.String fp = this.GetStringAttribute("OpenedPlaylistFullPath");
                        Global.LastPlaylistName = Path.GetFileName(fp);
                        if (fp is not null)
                        {
                            FileInfo file = new(fp);
                            if (file.Exists == false)
                            {
                                RaiseMessage(System.String.Format(Global.Resources.GetStringResource("CannotLocatePlaylist"), file.GetNameOnly()));
                                return;
                            }
                            SavePlaylist(file);
                        }
                    }
                }
                DestroyPlayerInstance();
                current.Dispose();
                current = null;
            }
            DebugProvider.WriteLine("RCU: Unloading Extension engine now...");
            extensionengine.Unload();
            DebugProvider.WriteLine("RCU: Unloading Windows Audio Library...");
            WindowsAudioLibrary.Uninitialize();
        }

        /// <summary>
        /// Unloads the player backend , when an explicit request for shutdown has been done before.
        /// </summary>
        protected override bool Uninitialize()
        {
            if (state.HasFlag(RCUEngineStateFlags.IsShuttingDown) == false)
            {
                DebugProvider.WriteLine($"RCU: An invalid request was dispatched for shutting down the engine while the engine is not informed.");
                DebugProvider.WriteLine("RCU: This suggests a malicious attempt to destroy the app state.");
                return false;
            }
            // Clear the flag to catch any additional double-disposals and malicious actions.
            state &= ~RCUEngineStateFlags.IsShuttingDown;
            if (tasker is not null)
            {
                tasker.Add(() => {
                    DebugProvider.WriteLine("RCU: Destroying Extension Engine instance.");
                    extensionengine?.Dispose();
                    extensionengine = null;
                    DebugProvider.WriteLine("RCU: Flushing and syncronising caches...");
                    using (var sm = Global.IconCachesMapFile.Open(FileMode.Create))
                    {
                        pici.SaveCacheToStream(sm);
                    }
                    statinst.SaveToFile(Global.StatisticsCacheFile);
                });
                // The Tasker holds the MMDevice COM object schedule it to be destroyed too.
                tasker.Add(() => {
                    if (device is not null)
                    {
                        _ = device.FriendlyName;
                        device.Dispose();
                        device = null;
                    }
                });
                tasker.StopAndProcessAll();
                // Generate a crash report if we have errors on the tasker thread
                if (tasker.FailedWorkItemsCount > 0)
                {
                    CrashReporter crt = null;
                    try {
                        crt = new(Global.BaseDataDirectory.Parent.FullName);
                        crt.WriteStartingText();
                        crt.WriteLine("NOTE: If you think that this report is generated due to a failed feature feel free to report it, along with this file.");
                        crt.WriteLine("If this was generated due to expected scenarios , you should not file a report.");
                        crt.WriteLineTerminator(); 
                        crt.WriteLineTerminator();
                        crt.LogReportedFailedDispatchData(tasker.FailedWorkItems);
                    } catch (Exception ex) {
                        DebugProvider.WriteLine($"Crash report failed to be generated! Exception: \n{ex}");
                    } finally {
                        try { crt.Dispose(); } catch { }
                    }
                }
                // Dispose now the tasker.
                tasker.Dispose();
                tasker = null;
            }
            if (gamepad is not null)
            {
                gamepad.Listen = false;
                gamepad.GamepadAction -= GamepadHandler;
                gamepad.Dispose();
                gamepad = null;
            }
            drp?.Dispose();
            drp = null;
            basedir = null;
            pici?.ClearCacheEntries();
            pici = null;
            statinst?.ClearCacheEntries();
            statinst = null;
            playerlock?.Dispose();
            playerlock = null;
            return true;
        }
    }
}
