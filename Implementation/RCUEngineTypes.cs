using System;
using System.Collections.Generic;

namespace MP
{
    internal enum ListViewMode : System.Byte
    {
        Files,
        SelectDrive,
        PlaylistSelect,
        LoadedPlaylist
    }

    internal enum RepeatMode : System.Byte
    {
        No,
        One,
        All
    }

    internal static class WindowsRCURecieveCommandTypes
    {
        public const CommonRecieveCommandTypes GetFilesAndDirs = (CommonRecieveCommandTypes)20;
        public const CommonRecieveCommandTypes GetDrives = (CommonRecieveCommandTypes)21;
        public const CommonRecieveCommandTypes SelectRepeatMode = (CommonRecieveCommandTypes)22;
        public const CommonRecieveCommandTypes GetPlaylistPreferences = (CommonRecieveCommandTypes)23;
        public const CommonRecieveCommandTypes DeleteTrack = (CommonRecieveCommandTypes)24;
        public const CommonRecieveCommandTypes ShowTrackTag = (CommonRecieveCommandTypes)25;
        public const CommonRecieveCommandTypes AddTrackToPlaylist = (CommonRecieveCommandTypes)26;
        public const CommonRecieveCommandTypes OpenArchivedPlaylist = (CommonRecieveCommandTypes)27;
        public const CommonRecieveCommandTypes ReloadFilesAndDirs = (CommonRecieveCommandTypes)28;
        public const CommonRecieveCommandTypes CleanStatisticsCacheData = (CommonRecieveCommandTypes)29;
    }

    internal static class WindowsRCUSendCommandTypes
    {
        public const CommonSendCommandTypes InvokeWaitCursor = (CommonSendCommandTypes)20;
        public const CommonSendCommandTypes RemoveWaitCursor = (CommonSendCommandTypes)21;
        public const CommonSendCommandTypes UpdateSelectedMode = (CommonSendCommandTypes)22;
        public const CommonSendCommandTypes ChangeRepeatModeImage = (CommonSendCommandTypes)23;
    }

    internal sealed class WindowsListViewElement : CommandMetadataItem
    {
        public WindowsListViewElement()
        {
            Name = "dd_lvel";
            Background = System.Drawing.Color.Black;
            Foreground = System.Drawing.Color.White;
            Type = CommandMetadataItemType.Element;
        }

        // This must be always set to the first field in the element.
        public System.String PrimaryData { get => Value.ToString(); set => Value = value; }

        // These data are passed to minor sub-columns to the list view.
        // Note that this array must have the same length as the number of sub-columns.
        public System.String[] SecondaryData;

        // Specifies an image to use inside the image array. If not specified , it is set to -1 (Do not use any image at all).
        public System.Int32 ImageIndex;

        // Sets the foreground and background colors of this element.
        // The defaults are white and black , respectively.
        public System.Drawing.Color Foreground;

        public System.Drawing.Color Background;

        // Sets additional data to associate with the list view item.
        public System.String AdditionalData;
    }

    internal sealed class WindowsListViewColumn : CommandMetadataItem
    {
        public WindowsListViewColumn() {
            Flags = ColumnUsageFlags.None;
            Type = CommandMetadataItemType.Column;
        }

        public WindowsListViewColumn(System.String name) : this()
        {
            Width = 60;
            Name = name;
        }

        public WindowsListViewColumn(System.String name, System.Int32 width) : this()
        {
            Name = name;
            Width = width;
        }

        public WindowsListViewColumn(System.String name, System.Int32 width, System.Boolean adj) : this()
        {
            Name = name;
            Width = width;
            if (adj) { Flags |= ColumnUsageFlags.Adjustable; }
        }

        public WindowsListViewColumn(System.String name, System.Int32 width, System.Boolean adj, System.Boolean center) : this()
        {
            Name = name;
            Width = width;
            if (center) { Flags |= ColumnUsageFlags.CenterText; }
            if (adj) { Flags |= ColumnUsageFlags.Adjustable; }
        }

        // The initial length of this particular column.
        public System.Int32 Width;

        // Column behavior modification flags.
        public ColumnUsageFlags Flags;

        public System.Boolean Adjustable => Flags.HasFlag(ColumnUsageFlags.Adjustable);

        public System.Boolean CenterText => Flags.HasFlag(ColumnUsageFlags.CenterText);
    }

    internal struct TrackCommandData
    {
        public System.String Name;
        public System.Int32 Index;
    }

    /// <summary>
    /// Windows RCU engine state flags enumeration - it defines the state of the engine at a given time. <br />
    /// NOTE: FOR INTERNAL USE ONLY.
    /// </summary>
    [Flags]
    internal enum RCUEngineStateFlags : System.Byte
    {
        None = 0,
        IsGamepadHeadsetUsed = 1,
        IsShuttingDown = 2,
        PlayerIsBusy = 4,
        IsFirstFileExploration = 8,
    }

    [Flags]
    internal enum ColumnUsageFlags : System.Byte
    {
        None = 0,
        Adjustable = 0x01,
        CenterText = 0x02
    }
}
