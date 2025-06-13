
using MP.CGISettings;
using MP.SettingsTree;

namespace MP
{
    [CGISettingsLoaderClass]
    [SettingsTreeLayoutClass]
    [SettingsTreeNode(null , "audio" , "Audio Playback")]
    [SettingsTreeNode(null , "controller" , "Controller Settings")]
    [SettingsTreeNode(null , "expview" , "Exploration Panel")]
    [SettingsTreeNode(null , "misc" , "Miscellaneous Functions")]
    [SettingsTreeNode("expview" , "expviewg1" , "Exploration Panel: Column Definitions")]
    [SettingsTreeNode("expview" , "expviewg2" , "Exploration Panel: Color Definitions")]
    public class Settings
    {
        [SettingsTreeIgnore]
        public Microsoft.IO.DirectoryInfo BaseDataDirectory;

        [SettingsTreeIgnore]
        public Microsoft.IO.DirectoryInfo PlaylistsDirectory;

        [SettingsTreeIgnore]
        public Microsoft.IO.DirectoryInfo TempDirectory;

        [SettingsTreeIgnore]
        public Microsoft.IO.FileInfo ResourceDictionary;

        [SettingsTreeIgnore]
        public System.String LastPlaylistName;

        [SettingsTreeIgnore]
        public System.Byte RightChannelVolume;

        [SettingsTreeIgnore]
        public System.Byte LeftChannelVolume;

        [SettingsTreeParent("audio")]
        [SettingFriendlyName("Device Latency")]
        [SettingType(SettingType.HasSpecificRange)]
        [SettingsTreeValidValues<System.Byte>(55 , 255)]
        [SettingDescriptionResource("SettingTree_AudioDeviceLatency_Desc")]
        public System.Byte Latency;

        [SettingsTreeParent("audio")]
        [SettingType(SettingType.ValueList)]
        [SettingFriendlyName("Audio Device Selection")]
        [SettingDescriptionResource("SettingTree_AudioDeviceSelection_Desc")]
        public System.String AudioDeviceId;

        [SettingsTreeIgnore]
        [CGISettingsLoaderIgnore]
        public DotNetResourcesExtensions.IResourceLoader Resources;

        [SettingsTreeIgnore]
        public System.Int32 Version;

        [SettingsTreeParent("expviewg1")]
        [SettingType(SettingType.HasSpecificRange)]
        [SettingFriendlyName("Track Name Column Width")]
        [SettingsTreeValidValues<System.Int32>(250 , 100000)]
        [SettingDescriptionResource("SettingTree_ColWidth_TrackName_Desc")]
        public System.Int32 WD_TrackName;

        [SettingsTreeParent("expviewg1")]
        [SettingType(SettingType.HasSpecificRange)]
        [SettingsTreeValidValues<System.Int32>(150, 100000)]
        [SettingFriendlyName("Track Creation Time Column Width")]
        [SettingDescriptionResource("SettingTree_ColWidth_TrackCreationTime_Desc")]
        public System.Int32 WD_TrackCreationTime;

        [SettingsTreeParent("expviewg1")]
        [SettingFriendlyName("Track Size Column Width")]
        [SettingType(SettingType.HasSpecificRange)]
        [SettingsTreeValidValues<System.Int32>(82, 100000)]
        [SettingDescriptionResource("SettingTree_ColWidth_TrackSize_Desc")]
        public System.Int32 WD_TrackSize;

        [SettingsTreeParent("expviewg1")]
        [SettingType(SettingType.HasSpecificRange)]
        [SettingsTreeValidValues<System.Int32>(180, 100000)]
        [SettingFriendlyName("Track Contributing Artists Column Width")]
        [SettingDescriptionResource("SettingTree_ColWidth_TrackContribArtists_Desc")]
        public System.Int32 WD_TrackContributingArtists;

        [SettingsTreeParent("expviewg1")]
        [SettingType(SettingType.HasSpecificRange)]
        [SettingFriendlyName("Track Number Column Width")]
        [SettingsTreeValidValues<System.Int32>(90, 100000)]
        [SettingDescriptionResource("SettingTree_ColWidth_TrackNumber_Desc")]
        public System.Int32 WD_TrackNumber;

        [SettingsTreeParent("expviewg1")]
        [SettingType(SettingType.HasSpecificRange)]
        [SettingFriendlyName("Encoded By Column Width")]
        [SettingsTreeValidValues<System.Int32>(150, 100000)]
        [SettingDescriptionResource("SettingTree_ColWidth_EncodedBy_Desc")]
        public System.Int32 WD_EncodedBy;

        [SettingsTreeParent("expviewg1")]
        [SettingFriendlyName("Album Column Width")]
        [SettingType(SettingType.HasSpecificRange)]
        [SettingsTreeValidValues<System.Int32>(150, 100000)]
        [SettingDescriptionResource("SettingTree_ColWidth_Album_Desc")]
        public System.Int32 WD_Album;

        [SettingsTreeIgnore]
        public System.Boolean InitializeLastTrackFromLastPlaylist;

        [SettingsTreeParent("expviewg1")]
        [SettingType(SettingType.HasSpecificRange)]
        [SettingFriendlyName("Playlist Name Column Width")]
        [SettingsTreeValidValues<System.Int16>(160, 32767)]
        [SettingDescriptionResource("SettingTree_ColWidth_PlaylistName_Desc")]
        public System.Int16 WD_PlaylistName;

        [SettingsTreeParent("expviewg1")]
        [SettingType(SettingType.HasSpecificRange)]
        [SettingsTreeValidValues<System.Int16>(150, 32767)]
        [SettingFriendlyName("File Creation Time Column Width")]
        [SettingDescriptionResource("SettingTree_ColWidth_CreationTime_Desc")]
        public System.Int16 WD_CreationTime;

        [SettingsTreeParent("expviewg1")]
        [SettingType(SettingType.HasSpecificRange)]
        [SettingsTreeValidValues<System.Int16>(220, 32767)]
        [SettingFriendlyName("Last Playlist Track Column Width")]
        [SettingDescriptionResource("SettingTree_ColWidth_LastPlaylistTrack_Desc")]
        public System.Int16 WD_LastPlaylistTrack;

        [SettingsTreeParent("expviewg1")]
        [SettingType(SettingType.HasSpecificRange)]
        [SettingFriendlyName("File Name Column Width")]
        [SettingsTreeValidValues<System.Int16>(220, 32767)]
        [SettingDescriptionResource("SettingTree_ColWidth_FileName_Desc")]
        public System.Int16 WD_FileName;

        [SettingsTreeParent("controller")]
        [SettingType(SettingType.HasSpecificRange)]
        [SettingsTreeValidValues<System.Int16>(140, 32767)]
        [SettingFriendlyName("Controller Timeout Thread Delay")]
        [SettingDescriptionResource("SettingTree_ControllerTimeoutDelay_Desc")]
        public System.Int16 Controller_Delay;

        [SettingsTreeParent("controller")]
        [SettingType(SettingType.HasSpecificRange)]
        [SettingsTreeValidValues<System.Int16>(0, 32767)]
        [SettingFriendlyName("Controller Thumbstick Accuracy")]
        [SettingDescriptionResource("SettingTree_ControllerThumbPrecision_Desc")]
        public System.Int16 Controller_ThumbstickPrecision;

        [SettingsTreeParent("controller")]
        [SettingType(SettingType.HasSpecificRange)]
        [SettingsTreeValidValues<System.Byte>(0, 255)]
        [SettingFriendlyName("Controller Trigger Accuracy")]
        [SettingDescriptionResource("SettingTree_ControllerTrigPrecision_Desc")]
        public System.Byte Controller_TriggerPrecision;

        [SettingType(SettingType.Color)]
        [SettingsTreeParent("expviewg2")]
        [SettingFriendlyName("Exploration View Background Color")]
        [SettingDescriptionResource("SettingTree_ExpBackColorSelection_Desc")]
        public System.Drawing.Color ExplorationViewBackColor;

        [SettingType(SettingType.Color)]
        [SettingsTreeParent("expviewg2")]
        [SettingFriendlyName("Exploration View Foreground Color")]
        [SettingDescriptionResource("SettingTree_ExpForeColorSelection_Desc")]
        public System.Drawing.Color ExplorationViewForeColor;

        [SettingsTreeIgnore]
        public Microsoft.IO.DirectoryInfo IconCachesDirectory;

        [SettingsTreeIgnore]
        public Microsoft.IO.FileInfo IconCachesMapFile;

        [SettingsTreeIgnore]
        public Microsoft.IO.DirectoryInfo OptionalFeaturesDirectory;

        [SettingsTreeParent("misc")]
        [SettingType(SettingType.IsFolderPath)]
        [SettingFriendlyName("Downloaded Tracks Placement Directory")]
        [SettingDescriptionResource("SettingTree_DownloadedPLTracksDirSelection_Desc")]
        public Microsoft.IO.DirectoryInfo DownloadedTracksDirectory;

        [SettingsTreeIgnore]
        public Microsoft.IO.FileInfo DownloadedTracksPlaylist;

        [SettingsTreeIgnore]
        public Microsoft.IO.DirectoryInfo YtDlpInstallationDirectory;

        // NOTE: The folder that this directory info has is usually created lazily.
        [SettingsTreeParent("misc")]
        [SettingType(SettingType.IsFolderPath)]
        [SettingFriendlyName("Exported Tracks Placement Directory")]
        [SettingDescriptionResource("SettingTree_ExportedPLTracksDirSelection_Desc")]
        public Microsoft.IO.DirectoryInfo ExportedPlaylistTracksDirectory;

        // This points to a path like Data/StatisticsCacheFile.mpc
        // Unchangeable by the Settings Editor
        [SettingsTreeIgnore]
        public Microsoft.IO.FileInfo StatisticsCacheFile;

        // Do not forget to add the new file or directory entry 
        // to the recommended path of GetDefault() to PatchBrokenPaths() function!
        public static Settings GetDefault()
        {
            Settings sets = new();
            // Versioning information
            // TODO: Consider if this is needed anymore (due to the fact that CGISettingsLoader
            // conveniently covers us versioning changes through the event and we do not update the field correctly.)
            sets.Version = 10;
            // Hidden - internally used settings.
            sets.BaseDataDirectory = new(Microsoft.IO.Path.Join(BaseConsultingDirectory , "Data"));
            sets.PlaylistsDirectory = new(Microsoft.IO.Path.Join(sets.BaseDataDirectory.FullName , "Playlists"));
            sets.ResourceDictionary = new(Microsoft.IO.Path.Join(sets.BaseDataDirectory.FullName , "Resources.rsrc"));
            sets.TempDirectory = new(Microsoft.IO.Path.Join(sets.BaseDataDirectory.FullName, "Temp"));
            sets.IconCachesDirectory = new(Microsoft.IO.Path.Join(sets.BaseDataDirectory.FullName, "PlaylistIcons"));
            sets.IconCachesMapFile = new(Microsoft.IO.Path.Join(sets.BaseDataDirectory.FullName , "PlaylistIconMap.mpc"));
            sets.OptionalFeaturesDirectory = new(Microsoft.IO.Path.Join(sets.BaseDataDirectory.FullName, "Plugins"));
            sets.DownloadedTracksDirectory = new(Microsoft.IO.Path.Join(sets.BaseDataDirectory.FullName, "DownloadedTracks"));
            sets.DownloadedTracksPlaylist = new(Microsoft.IO.Path.Join(sets.BaseDataDirectory.FullName, "DownloadedTracks.mpbpl"));
            sets.YtDlpInstallationDirectory = new(Microsoft.IO.Path.Join(sets.BaseDataDirectory.FullName, "YtDlp"));
            sets.ExportedPlaylistTracksDirectory = new(Microsoft.IO.Path.Join(SystemInfo.GetKnownFolder(ShellKnownFolder.Music) , "mp-dotnet8"));
            sets.StatisticsCacheFile = new(Microsoft.IO.Path.Join(sets.BaseDataDirectory.FullName , "StatisticsCacheFile.mpc"));
            // Playback Settings
            sets.Latency = 70;
            sets.AudioDeviceId = "";
            sets.RightChannelVolume = sets.LeftChannelVolume = 50;
            // Playlist Settings
            sets.InitializeLastTrackFromLastPlaylist = false;
            sets.LastPlaylistName = "";
            // Exploration Pane Settings
            sets.ExplorationViewForeColor = System.Drawing.Color.White;
            sets.ExplorationViewBackColor = System.Drawing.Color.Black;
            // Column Width Settings
            sets.WD_Album = 150;
            sets.WD_EncodedBy = 150;
            sets.WD_TrackNumber = 90;
            sets.WD_TrackContributingArtists = 180;
            sets.WD_TrackSize = 82;
            sets.WD_TrackCreationTime = 150;
            sets.WD_TrackName = 250;
            sets.WD_FileName = 220;
            sets.WD_PlaylistName = 160;
            sets.WD_CreationTime = 140;
            sets.WD_LastPlaylistTrack = 220;
            // Controller Settings
            sets.Controller_Delay = 152;
            sets.Controller_TriggerPrecision = 10;
            sets.Controller_ThumbstickPrecision = 2500;
            return sets;
        }

        public static Settings Global = new();

        public static void WriteAsCGI(System.IO.Stream stream)
        {
            CGISettingsWriter writer = null;
            CGISettingsWriterLoader<SettingEntry> ldr = null;
            try
            {
                writer = new(stream, System.Text.Encoding.UTF8);
                writer.RegisterExtension(new ColorExtension());
                writer.RegisterExtension(new FileInfoExtension());
                writer.RegisterExtension(new DirectoryInfoExtension());
                writer.ApplicationName = "MP_DOTNETEIGHT";
                writer.IsStreamOwner = false;
                ldr = new(writer);
                ldr.WriteFromClassInstance(Global);
                writer.Generate();
            } finally {
                ldr?.Dispose();
                writer?.Dispose();
            }
        }

        private static void CGISettingNotLoaded_ProvideDefault(System.String name)
        {
            DebugProvider.WriteLine($"APP: WARN: Setting with name {name} seems to not have been provided. This might mean that the loaded settings may be corrupt.");
            // If such a case it happens, we need to update the version field at any way.
            Global.Version = 10;
            switch (name)
            {
                case nameof(Version):
                    break;
                case nameof(TempDirectory):
                    Global.TempDirectory ??= new(Microsoft.IO.Path.Join(Global.BaseDataDirectory.FullName, "Temp"));
                    break;
                case nameof(WD_Album):
                    Global.WD_Album = 150;
                    break;
                case nameof(WD_CreationTime):
                    Global.WD_CreationTime = 140;
                    break;
                case nameof(WD_TrackNumber):
                    Global.WD_TrackNumber = 90;
                    break;
                case nameof(WD_TrackContributingArtists):
                    Global.WD_TrackContributingArtists = 180;
                    break;
                case nameof(WD_TrackName):
                    Global.WD_TrackName = 250;
                    break;
                case nameof(WD_TrackSize):
                    Global.WD_TrackSize = 82;
                    break;
                case nameof(WD_TrackCreationTime):
                    Global.WD_TrackCreationTime = 150;
                    break;
                case nameof(WD_EncodedBy):
                    Global.WD_EncodedBy = 150;
                    break;
                case nameof(WD_FileName):
                    Global.WD_FileName = 220;
                    break;
                case nameof(WD_PlaylistName):
                    Global.WD_PlaylistName = 160;
                    break;
                case nameof(WD_LastPlaylistTrack):
                    Global.WD_LastPlaylistTrack = 220;
                    break;
                case nameof(InitializeLastTrackFromLastPlaylist):
                    Global.InitializeLastTrackFromLastPlaylist = false;
                    break;
                case nameof(Controller_Delay):
                    Global.Controller_Delay = 152;
                    break;
                case nameof(Controller_ThumbstickPrecision):
                    Global.Controller_ThumbstickPrecision = 2500;
                    break;
                case nameof(Controller_TriggerPrecision):
                    Global.Controller_TriggerPrecision = 10;
                    break;
                case nameof(ExplorationViewBackColor):
                    Global.ExplorationViewBackColor = System.Drawing.Color.Black;
                    break;
                case nameof(ExplorationViewForeColor):
                    Global.ExplorationViewForeColor = System.Drawing.Color.White;
                    break;
                case nameof(IconCachesDirectory):
                    Global.IconCachesDirectory = new(Microsoft.IO.Path.Join(Global.BaseDataDirectory.FullName, "PlaylistIcons"));
                    break;
                case nameof(IconCachesMapFile):
                    Global.IconCachesMapFile = new(Microsoft.IO.Path.Join(Global.BaseDataDirectory.FullName, "PlaylistIconMap.mpc"));
                    break;
                case nameof(OptionalFeaturesDirectory):
                    Global.OptionalFeaturesDirectory = new(Microsoft.IO.Path.Join(Global.BaseDataDirectory.FullName, "Plugins"));
                    break;
                case nameof(DownloadedTracksDirectory):
                    Global.DownloadedTracksDirectory = new(Microsoft.IO.Path.Join(Global.BaseDataDirectory.FullName, "DownloadedTracks"));
                    break;
                case nameof(DownloadedTracksPlaylist):
                    Global.DownloadedTracksPlaylist = new(Microsoft.IO.Path.Join(Global.BaseDataDirectory.FullName, "DownloadedTracks.mpbpl"));
                    break;
                case nameof(YtDlpInstallationDirectory):
                    Global.YtDlpInstallationDirectory = new(Microsoft.IO.Path.Join(Global.BaseDataDirectory.FullName, "YtDlp"));
                    break;
                case nameof(ExportedPlaylistTracksDirectory):
                    Global.ExportedPlaylistTracksDirectory = new(Microsoft.IO.Path.Join(SystemInfo.GetKnownFolder(ShellKnownFolder.Music), "mp-dotnet8"));
                    break;
                case nameof(StatisticsCacheFile):
                    Global.StatisticsCacheFile = new(Microsoft.IO.Path.Join(Global.BaseDataDirectory.FullName, "StatisticsCacheFile.mpc"));
                    break;
                default:
                    DebugProvider.WriteLine($"APP: WARN: Cannot recognize setting name {name}. Maybe it has been removed from the app it's related feature?");
                    break;
            }
        }

        public static void ReadFromCGI(System.IO.Stream stream)
        {
            CGISettingNotFilledInDelegate dlg = new(CGISettingNotLoaded_ProvideDefault);
            CGISettingsReader reader = null;
            CGISettingsReaderLoader<SettingEntry> ldr = null;
            try {
                reader = new(stream);
                reader.RegisterExtension(new ColorExtension());
                reader.RegisterExtension(new FileInfoExtension());
                reader.RegisterExtension(new DirectoryInfoExtension());
                reader.IsStreamOwner = false;
                ldr = new(reader);
                ldr.CGISettingNotPresentInSource += dlg;
                ldr.LoadClassInstance(Global);
            } finally {
                if (ldr is not null)
                {
                    ldr.CGISettingNotPresentInSource -= dlg;
                    ldr.Dispose();
                }
                reader?.Dispose();
                dlg = null;
            }
        }

        public static void PatchBrokenPaths()
        {
            DebugProvider.WriteLine("Patching broken paths as the app believes it to be...");
            Global.BaseDataDirectory = new(Microsoft.IO.Path.Join(BaseConsultingDirectory, "Data"));
            Global.PlaylistsDirectory = new(Microsoft.IO.Path.Join(Global.BaseDataDirectory.FullName , "Playlists"));
            Global.ResourceDictionary = new(Microsoft.IO.Path.Join(Global.BaseDataDirectory.FullName, "Resources.rsrc"));
            Global.TempDirectory = new(Microsoft.IO.Path.Join(Global.BaseDataDirectory.FullName, "Temp"));
            Global.IconCachesDirectory = new(Microsoft.IO.Path.Join(Global.BaseDataDirectory.FullName, "PlaylistIcons"));
            Global.IconCachesMapFile = new(Microsoft.IO.Path.Join(Global.BaseDataDirectory.FullName, "PlaylistIconMap.mpc"));
            Global.OptionalFeaturesDirectory = new(Microsoft.IO.Path.Join(Global.BaseDataDirectory.FullName, "Plugins"));
            Global.DownloadedTracksDirectory = new(Microsoft.IO.Path.Join(Global.BaseDataDirectory.FullName, "DownloadedTracks"));
            Global.DownloadedTracksPlaylist = new(Microsoft.IO.Path.Join(Global.BaseDataDirectory.FullName, "DownloadedTracks.mpbpl"));
            Global.YtDlpInstallationDirectory = new(Microsoft.IO.Path.Join(Global.BaseDataDirectory.FullName, "YtDlp"));
            Global.ExportedPlaylistTracksDirectory = new(Microsoft.IO.Path.Join(SystemInfo.GetKnownFolder(ShellKnownFolder.Music), "mp-dotnet8"));
            Global.StatisticsCacheFile = new(Microsoft.IO.Path.Join(Global.BaseDataDirectory.FullName, "StatisticsCacheFile.mpc"));
            DebugProvider.WriteLine("Patching completed.");
        }

        // Resolves the base loading directory of the app.
        // In Debug give more flexibility for developers.
        private static System.String BaseConsultingDirectory
            =>
#if DEBUG
                System.AppDomain.CurrentDomain.BaseDirectory;
#else
                SystemInfo.CurrentProcessDirectory;
#endif
    }

}
