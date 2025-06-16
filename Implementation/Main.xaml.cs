using System;
using MP.Utilities;
using System.Windows;
using MP.AudioLibrary;
using static MP.Settings;
using System.Windows.Forms;

namespace MP
{
    /// <summary>
    /// Interaction logic for Main.xaml
    /// </summary>
    public partial class PlayerWindow : Window
    {
        private ListView lvw;
        private ListViewMode lastmode;
        private Dialogs.WaitDialog waitdlg;
        private ContextMenuStrip lvwcms;
        private PlayerWindowUIState state;
        private WindowsRCUEngine backend;
        private RotatingThreadCancellationToken token;

        public PlayerWindow()
        {
            lvw = new();
            lvwcms = new();
            state = new();
            backend = new();
            token = null;
            waitdlg = null;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            TenSecsAheadImage.Source = Global.Resources.LoadXamlImage("Next10Secs");
            TenSecsBackImage.Source = Global.Resources.LoadXamlImage("Prev10Secs");
            PlayImage.Source = Global.Resources.LoadXamlImage("PlayImg");
            NextImage.Source = Global.Resources.LoadXamlImage("NextTrack");
            StopImage.Source = Global.Resources.LoadXamlImage("StopImg");
            PrevImage.Source = Global.Resources.LoadXamlImage("PrevTrack");
            LeftVolume.Value = Global.LeftChannelVolume;
            RightVolume.Value = Global.RightChannelVolume;
            LeftVolume.ValueChanged += LeftVolume_ValueChanged;
            RightVolume.ValueChanged += RightVolume_ValueChanged;
            Title = AppInfo.FormatWindowTitle(Global.Resources.GetStringResource("AppTitle"));
#if DEBUG
            Title += $" - Windows {SystemInfo.OperatingSystemVersion} - LogonServer {SystemInfo.ComputerName} - Profile ID {SystemInfo.HardwareProfileID}";
#endif
            Icon = Global.Resources.LoadXamlImage("ApplicationIcon");
            TimeBar.Minimum = 0;
            WFH_LISTVIEW_1.Child = lvw;
            // Prepare the ListView
            lvw.CreateControl();
            lvw.BeginUpdate();
            lvw.BackColor = Global.ExplorationViewBackColor;
            lvw.ForeColor = Global.ExplorationViewForeColor;
            lvw.View = System.Windows.Forms.View.Details;
            lvw.TileSize = new(15, 15);
            lvw.FullRowSelect = true;
            lvw.MultiSelect = true;
            lvw.EndUpdate();
            lvw.MouseDown += LVW_MouseClick;
            lvw.KeyPress += LVW_EnterOrSpaceBarPress;
            lvw.SmallImageList = new();
            lvw.SmallImageList.ImageSize = new(32 , 32);
            // Prepare the context menu strip for ListView
            lvwcms.CreateGraphics().Dispose();
            lvwcms.Size = new(385, 50);
            lvwcms.BackColor = Global.ExplorationViewBackColor;
            lvwcms.ForeColor = Global.ExplorationViewForeColor;
            lvwcms.Opening += LVWCMS_Opening;
            lvwcms.ItemClicked += LVWCMS_ItemClicked;
            lvw.ContextMenuStrip = lvwcms;
            backend.SendCommand += RecieveCmd;
            Topmost = true;
            Topmost = false;
            Activated += Window_Activated;
            Deactivated += Window_Deactivated;
            System.Boolean result = backend.Initialize();
            MusicPlayer.Entry.WaitFormInstance?.Close();
            if (result == false) { 
                backend = null; 
                Close(); 
                return; 
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            ExitDialog dlg = null;
            if (state.HardFailureOccured == false)
            {
                try {
                    state.Enabled = false;
                    dlg = new(backend.ListViewMode == ListViewMode.LoadedPlaylist && backend.Playlist is TrackPlayList, backend.Gamepad);
                    // If bringing the window into viewable area has failed , the exit dialog will not be shown and the app will be terminated.
                    if (Activate() && dlg.ShowDialog() == System.Windows.Forms.DialogResult.Cancel)
                    {
                        state.AssertClosing = false;
                        e.Cancel = true;
                        state.Enabled = true;
                        return;
                    }
                } catch { } finally {
                    dlg?.Dispose();
                    dlg = null;
                }
            }
            // Remove activation/deactivation events
            Activated -= Window_Activated;
            Deactivated -= Window_Deactivated;
            // Remove all connected events for the custom ListView
            lvw.MouseDown -= LVW_MouseClick;
            lvw.KeyPress -= LVW_EnterOrSpaceBarPress;
            lvwcms.Opening -= LVWCMS_Opening;
            lvwcms.ItemClicked -= LVWCMS_ItemClicked;
            // Remove non-auto-disposed events
            LeftVolume.ValueChanged -= LeftVolume_ValueChanged;
            RightVolume.ValueChanged -= RightVolume_ValueChanged;
            state.UIShutdown = true;
            WFH_LISTVIEW_1.Dispose();
            lvwcms?.Dispose();
            lvw?.Dispose();
            token?.Invalidate();
            lvw = null;
            lvwcms = null;
            // Dispose and preserve data for the last playlist
            // If the backend is null or it has a hard failure do not do anything!
            if (state.HardFailureOccured) { goto g_26; }
            if (backend is null) { goto g_26; }
            backend.PrepareShutdown();
            backend.Dispose();
            backend = null;
            // Exit
            g_26: // Note that resource disposal must be done after all calls to resources have been finished!
                waitdlg?.Dispose();
                waitdlg = null;
                Global.Resources.Dispose();
                Global.Resources = null;
        }

        private void Window_Activated(System.Object sender, EventArgs e)
        {
            var td = new System.Threading.Thread(() => {
                DebugProvider.WriteLine("UIManager: Transitioning into active state...");
                System.Threading.Thread.Sleep(140);
                if (backend is null) { return; }
                if (backend.Gamepad is not null) { backend.Gamepad.Listen = true; }
                if (state is null) { return; }
                state.Enabled = true;
                try { lvw.Invoke(lvw.Focus); } catch { }
            });
            td.Name = Global.Resources.GetStringResource("ActThread_MainWindowThreadName");
            td.IsBackground = true;
            td.Start();
        }

        private void Window_Deactivated(System.Object sender, EventArgs e)
        {
            DebugProvider.WriteLine("UIManager: Transitioning into stand-by state...");
            if (backend.Gamepad is not null) { backend.Gamepad.Listen = false; }
            state.Enabled = false;
        }

        private void ClearView()
        {
            try {
                if (lastmode == ListViewMode.LoadedPlaylist)
                {
                    Global.WD_TrackName = lvw.Columns[0].Width;
                    Global.WD_TrackCreationTime = lvw.Columns[1].Width;
                    Global.WD_TrackSize = lvw.Columns[2].Width;
                    Global.WD_TrackContributingArtists = lvw.Columns[3].Width;
                    Global.WD_TrackNumber = lvw.Columns[4].Width;
                    Global.WD_EncodedBy = lvw.Columns[5].Width;
                    Global.WD_Album = lvw.Columns[6].Width;
                } else if (lastmode == ListViewMode.LoadedPlaylist) 
                {
                    Global.WD_PlaylistName = lvw.Columns[0].Width.ToInt16();
                    Global.WD_CreationTime = lvw.Columns[1].Width.ToInt16();
                    Global.WD_LastPlaylistTrack = lvw.Columns[3].Width.ToInt16();
                } else if (lastmode == ListViewMode.Files)
                {
                    Global.WD_FileName = lvw.Columns[0].Width.ToInt16();
                }
            } catch { }
            try {
                lvw.Invoke(lvw.BeginUpdate);
                // Clear everything
                lvw.Invoke(lvw.Columns.Clear);
                lvw.Invoke(lvw.Items.Clear);
                if (lvw.SmallImageList is not null)
                {
                    foreach (System.Drawing.Image img in lvw.SmallImageList.Images)
                    {
                        img.Dispose();
                    }
                    lvw.SmallImageList.Images.Clear();
                    lvw.Invoke(lvw.SmallImageList.Dispose);
                }
                lvw.Invoke(() => {
                    lvw.SmallImageList = null;
                    lvw.SmallImageList = new();
                    lvw.SmallImageList.ImageSize = new(32, 32);
                });
                state.ListViewIndex = 0;
            } finally {
                if (lvw is not null) { lvw.Invoke(lvw.EndUpdate); }
            }
            // Get focus so that we can actually perform clear.
            lvw.Invoke(lvw.Focus);
            lvw.Invoke(lvw.Update);
        }

        private System.Boolean UpdateView_ImageLoader(CommandMetadata data)
        {
            System.IO.Stream stream = null;
            foreach (var img in data.GetItemsMatchingCommonType(CommandMetadataItemType.Image))
            {
                try {
                    stream = new Microsoft.IO.MemoryStream(img.Value as System.Byte[]);
                    lvw.SmallImageList.Images.Add(new System.Drawing.Bitmap(stream, true));
                } catch (System.Exception e) {
                    DebugProvider.WriteLine($"RCU: Error reported while loading an image from the interchargeable data:\n{e}");
                    lvw.SmallImageList.Images.Clear();
                    return false;
                } finally {
                    stream?.Dispose();
                    stream = null;
                }
            }
            return true;
        }

        private void UpdateView(CommandMetadata data)
        {
            lastmode = backend.ListViewMode;
            try {
                lvw.Invoke(lvw.BeginUpdate);
                DebugProvider.WriteLine("UIManager: Updating exploration view.");
                foreach (var col in data.GetItemsMatchingCommonType(CommandMetadataItemType.Column))
                {
                    lvw.Invoke((WindowsListViewColumn cl) => {
                        lvw.Columns.Add(new ColumnHeader() {
                            Text = cl.Name,
                            Width = cl.Width,
                            TextAlign = cl.CenterText ? System.Windows.Forms.HorizontalAlignment.Center : System.Windows.Forms.HorizontalAlignment.Left
                        });
                    } , col as WindowsListViewColumn);
                }
                DebugProvider.WriteLine($"UIManager: Added {lvw.Columns.Count} columns.");
                System.Boolean applyimg = (System.Boolean)lvw.Invoke(UpdateView_ImageLoader , data);
                ListViewItem item;
                WindowsListViewElement elem;
                foreach (var el in data.GetItemsMatchingCommonType(CommandMetadataItemType.Element))
                {
                    elem = el as WindowsListViewElement;
                    item = new ListViewItem() { 
                        BackColor = elem.Background, 
                        ForeColor = elem.Foreground, 
                        ImageIndex = applyimg ? elem.ImageIndex : -1, 
                        Text = elem.Value.ToString() ,
                        ToolTipText = elem.Value.ToString(),
                        UseItemStyleForSubItems = true,
                        Tag = elem.AdditionalData 
                    };
                    item.SubItems.AddRange(elem.SecondaryData);
                    lvw.Invoke((ListViewItem it) => lvw.Items.Add(it) , item);
                }
                item = null;
                DebugProvider.WriteLine($"UIManager: Added {lvw.Items.Count} items.");
            } finally {
                if (lvw is not null)
                {
                    lvw.Invoke(lvw.EndUpdate);
                }
                data?.Clean();
                data = null;
            }
            lvw.Invoke(() => {
                lvw.BackColor = Global.ExplorationViewBackColor;
                lvw.ForeColor = Global.ExplorationViewForeColor;
            });
            if (lvwcms.InvokeRequired) {
                lvwcms.Invoke(() => {
                    lvwcms.BackColor = Global.ExplorationViewBackColor;
                    lvwcms.ForeColor = Global.ExplorationViewForeColor;
                });
            } else {
                lvwcms.BackColor = Global.ExplorationViewBackColor;
                lvwcms.ForeColor = Global.ExplorationViewForeColor;
            }
            DebugProvider.WriteLine("UIManager: Done updating exploration view.");
            // Get focus so that we can actually perform update.
            lvw.Invoke(lvw.Focus);
            lvw.Invoke(lvw.Update);
        }

        private void LVWCMS_Opening(object sender, System.ComponentModel.CancelEventArgs e)
        {
            // Now check whether we have actually buttons to display on ContextMenuStrip instead
            e.Cancel = lvwcms.Items.Count < 1 || lvw.SelectedIndices.Count != 1;
        }

        private void LVWCMS_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            ListViewItem item = lvw.SelectedItems[0];
            switch (e.ClickedItem.Name)
            {
                case "EDT_1": // Delete the currently selected track
                    backend.RecieveCommand(new(WindowsRCURecieveCommandTypes.DeleteTrack) { Data = new TrackCommandData() { Index = item.Index, Name = item.Text }});
                    break;
                case "EDT_2": // Play this track
                    backend.RecieveCommand(new(CommonRecieveCommandTypes.LoadTrackOrdinal) { Data = item.Index });
                    break;
                case "EDT_3": // Show tag information for this track.
                    backend.RecieveCommand(new(WindowsRCURecieveCommandTypes.ShowTrackTag) { Data = new TrackCommandData() { Index = item.Index, Name = item.Text } });
                    break;
                case "EDT_4": // Change this track with another track.
                    if (backend.Playlist is ArchivedTrackPlaylist)
                    {
                        MusicPlayerHelper.ShowErrorResourceMessage("Error_OperationOnlyOnPhysicalPlaylists", backend.Gamepad);
                        break;
                    }
                    System.Int32 sidx = item.Index , I=0; // The track to be changed
                    System.String[] titles = new System.String[lvw.Items.Count];
                    foreach (ListViewItem titem in lvw.Items) { titles[I++] = titem.Text; }
                    ChangeTrackIndexDialog ctid = null;
                    try {
                        ctid = new(titles, sidx , backend.Gamepad);
                        titles = null;
                        if (ctid.ShowDialog() == System.Windows.Forms.DialogResult.OK) 
                        {
                            IPlaylistFile original = backend.Playlist.TracksContained[sidx];
                            backend.Playlist.TracksContained[sidx] = backend.Playlist.TracksContained[ctid.SelectedTrack];
                            backend.Playlist.TracksContained[ctid.SelectedTrack] = original;
                            original = null;
                            if (backend.Player is not null) { backend.RecieveCommand(new(CommonRecieveCommandTypes.DestroyPlayer)); }
                            Dispatcher.Invoke(new Action(() => Cursor = System.Windows.Input.Cursors.Wait));
                            backend.SyncSavePlaylist();
                            backend.RecieveCommand(new(CommonRecieveCommandTypes.ReloadOpenedPlaylist));
                        }
                    } catch { } finally {
                        ctid?.Dispose();
                        ctid = null;
                        titles = null;
                        Dispatcher.Invoke(new Action(() => Cursor = null));
                    }
                    break;
                case "PLS_1": // Delete the selected playlist
                    backend.RecieveCommand(new(CommonRecieveCommandTypes.DeletePlaylist) { Data = item.Tag as System.String });
                    break;
                case "PLS_2": // Play the last track from the selected playlist.
                    backend.RecieveCommand(new(CommonRecieveCommandTypes.OpenPlaylistUseLastTrack) { Data = item.Tag });
                    break;
            }
        }

        private void LVW_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Clicks == 2 && e.Button == MouseButtons.Left && lvw.SelectedIndices.Count == 1)
            {
                ListViewItem item = lvw.SelectedItems[0];
                switch (backend.ListViewMode)
                {
                    case ListViewMode.Files:
                        if (item.ImageIndex == 0)
                        {
                            backend.RecieveCommand(new(WindowsRCURecieveCommandTypes.GetFilesAndDirs) { Data = new Microsoft.IO.DirectoryInfo(Microsoft.IO.Path.Combine(backend.CurrentDirectory.FullName, item.Text)) });
                        }
                        break;
                    case ListViewMode.PlaylistSelect:
                        backend.RecieveCommand(new(CommonRecieveCommandTypes.OpenPlaylist) { Data = item.Tag });
                        break;
                    case ListViewMode.LoadedPlaylist:
                        // A new file will be played.
                        backend.RecieveCommand(new(CommonRecieveCommandTypes.LoadTrackOrdinal) { Data = item.Index });
                        break;
                    case ListViewMode.SelectDrive:
                        backend.RecieveCommand(new(WindowsRCURecieveCommandTypes.GetFilesAndDirs) { Data = new Microsoft.IO.DirectoryInfo(item.Text) });
                        break;
                }
            } else if (e.Clicks == 1 && e.Button == MouseButtons.Right && backend.ListViewMode == ListViewMode.Files)
            {
                // Then the user requested to create a playlist.
                CreatePlaylistDialog CPD = null;
                if (lvw.SelectedIndices.Count == 0) { return; }
                else if (lvw.SelectedIndices.Count == 1) {
                    switch (lvw.SelectedItems[0].ImageIndex)
                    {
                        case 0:
                            CPD = new(Microsoft.IO.Path.Join(backend.CurrentDirectory.FullName, lvw.SelectedItems[0].Text));
                            break;
                        case 1:
                            return;
                        case 2:
                            CPD = CreatePlaylistDialog.FromM3UPlaylistPath(Microsoft.IO.Path.Join(backend.CurrentDirectory.FullName, lvw.SelectedItems[0].Text));
                            break;
                    }
                } else {
                    ListViewItem[] items = new ListViewItem[lvw.SelectedItems.Count];
                    for (System.Int32 I = 0; I < items.Length; I++)
                    {
                        items[I] = lvw.SelectedItems[I];
                        if (items[I].ImageIndex == 2)
                        {
                            MusicPlayerHelper.ShowErrorResourceMessage("Error_InvalidAudioFilesSelection");
                            return;
                        }
                    }
                    CPD = new(backend.CurrentDirectory.FullName, items);
                }
                CPD.ShowDialog();
                if (CPD.DialogResult == CreatePlaylistDialogResult.OK && CPD.OpenAfterCreated)
                {
                    backend.RecieveCommand(new(CommonRecieveCommandTypes.OpenPlaylist) { Data = CPD.NameSavedUnder });
                }
            }
        }

        private void LVW_EnterOrSpaceBarPress(object sender, KeyPressEventArgs e)
        {
            Keys prt = (Keys)e.KeyChar;
            if ((prt == Keys.Space || prt == Keys.Enter) && lvw.SelectedIndices.Count == 1)
            {
                ListViewItem item = lvw.SelectedItems[0];
                switch (backend.ListViewMode)
                {
                    case ListViewMode.Files:
                        if (item.ImageIndex == 0)
                        {
                            backend.RecieveCommand(new(WindowsRCURecieveCommandTypes.GetFilesAndDirs) { Data = new Microsoft.IO.DirectoryInfo(Microsoft.IO.Path.Combine(backend.CurrentDirectory.FullName, item.Text)) });
                        }
                        break;
                    case ListViewMode.PlaylistSelect:
                        backend.RecieveCommand(new(CommonRecieveCommandTypes.OpenPlaylist) { Data = item.Tag });
                        break;
                    case ListViewMode.LoadedPlaylist:
                        // A new file will be played.
                        backend.RecieveCommand(new(CommonRecieveCommandTypes.LoadTrackOrdinal) { Data = item.Index });
                        break;
                    case ListViewMode.SelectDrive:
                        // This was not existed before but now this does also work.
                        backend.RecieveCommand(new(WindowsRCURecieveCommandTypes.GetFilesAndDirs) { Data = new Microsoft.IO.DirectoryInfo(item.Text) });
                        break;
                }
            } else if (prt == Keys.Back)
            {
                if (backend.ListViewMode == ListViewMode.Files)
                {
                    // In this case , it is Go To Top Directory.
                    backend.RecieveCommand(new(WindowsRCURecieveCommandTypes.GetFilesAndDirs) { Data = backend.CurrentDirectory.Parent });
                }
            }
        }

        private void GenericButton1_Click(object sender, RoutedEventArgs e)
        {
            if (backend.ListViewMode == ListViewMode.PlaylistSelect)
            {
                backend.RecieveCommand(new(CommonRecieveCommandTypes.CloseActivePlaylist));
                backend.RecieveCommand(new(WindowsRCURecieveCommandTypes.GetFilesAndDirs));
                return;
            } else if (backend.ListViewMode == ListViewMode.Files)
            {
                // In this case , it is Go To Top Directory.
                backend.RecieveCommand(new(WindowsRCURecieveCommandTypes.GetFilesAndDirs) { Data = backend.CurrentDirectory.Parent });
            } else if (backend.ListViewMode == ListViewMode.LoadedPlaylist) {
                switch (backend.Playlist)
                {
                    case ArchivedTrackPlaylist plt:
                        ArchivedPlaylistInformationForm fm = null;
                        try {
                            fm = new(plt);
                            if (fm.DialogResult != System.Windows.Forms.DialogResult.Abort)
                            {
                                fm.ShowDialog(MusicPlayerHelper.WinFormsHandle);
                            }
                        } finally {
                            fm?.Dispose();
                            fm = null;
                        }
                        break;
                    default:
                        backend.RecieveCommand(new(WindowsRCURecieveCommandTypes.GetPlaylistPreferences));
                        break;
                }
            }
        }

        private void GenericButton2_Click(object sender, RoutedEventArgs e)
        {
            if (backend.ListViewMode == ListViewMode.Files || 
                backend.ListViewMode == ListViewMode.SelectDrive)
            {
                backend.RecieveCommand(new(CommonRecieveCommandTypes.CloseActivePlaylist));
                backend.RecieveCommand(new(CommonRecieveCommandTypes.GetPlaylists));
                return;
            } else if (backend.ListViewMode == ListViewMode.LoadedPlaylist) 
            {
                backend.RecieveCommand(new(CommonRecieveCommandTypes.ReloadOpenedPlaylist));
            } else if (backend.ListViewMode == ListViewMode.PlaylistSelect) {
                backend.RecieveCommand(new(CommonRecieveCommandTypes.CloseActivePlaylist));
                backend.RecieveCommand(new(CommonRecieveCommandTypes.GetPlaylists));
            }
        }

        private void GenericButton3_Click(object sender, RoutedEventArgs e)
        {
            if (backend.ListViewMode == ListViewMode.Files)
            {
                // In this case , it does a reload over the file list.
                backend.RecieveCommand(new(WindowsRCURecieveCommandTypes.ReloadFilesAndDirs));
            } else if (backend.ListViewMode == ListViewMode.LoadedPlaylist)
            {
                backend.RecieveCommand(new(CommonRecieveCommandTypes.CloseActivePlaylist) { Data = lvw.Items.Count > 0 });
                backend.RecieveCommand(new(CommonRecieveCommandTypes.GetPlaylists));
                return;
            } else if (backend.ListViewMode == ListViewMode.PlaylistSelect)
            {
                Dialogs.OpenFolderDialog OFD = new();
                OFD.AddFilter(Dialogs.FileDialogFilter.GetFromWin32Filter(Global.Resources.GetStringResource("ArchOpener_FileFilter")));
                OFD.DefaultFilterExtension = ".aplaylist";
                OFD.Title = Global.Resources.GetStringResource("Message_SaveArchivedPlaylistDialog_Title");
                OFD.MultiSelect = false;
                OFD.CheckPath = true;
                OFD.CheckFilePath = true;
                if (OFD.SpawnDialog(MusicPlayerHelper.WinFormsHandle))
                {
                    backend.RecieveCommand(new(WindowsRCURecieveCommandTypes.OpenArchivedPlaylist) { Data = OFD.FilePaths[0] });
                }
            }
        }

        private void GenericButton4_Click(object sender, RoutedEventArgs e)
        {
            switch (backend.ListViewMode) {
                case ListViewMode.LoadedPlaylist:
                    switch (backend.Playlist)
                    {
                        case DownloadedFilesPlaylist dfp:
                            if (state.DDFWindowIsActive)
                            {
                                MusicPlayerHelper.ShowErrorResourceMessage("Error_AnotherDownloadOnProgress");
                                return;
                            }
                            DownloadAFileForm fm = new(Global.DownloadedTracksDirectory);
                            if (fm.ShowDialog(MusicPlayerHelper.WinFormsHandle) == System.Windows.Forms.DialogResult.OK)
                            {
                                dfp.AddDownloadedFile(fm.DownloadedFile);
                                backend.RecieveCommand(new(CommonRecieveCommandTypes.ReloadOpenedPlaylist));
                            }
                            fm.Dispose();
                            break;
                        case ArchivedTrackPlaylist atpt:
                            ExportPlaylistFromArchiveForm fm2 = null;
                            try {
                                fm2 = new(atpt);
                                fm2.ShowDialog(MusicPlayerHelper.WinFormsHandle);
                            } catch (Exception ex) {
                                DebugProvider.WriteLine($"UIManager: Exception occured while running the Export As Physical Playlist Window! \nException: {ex}");
                                break;
                            } finally {
                                fm2?.Dispose();
                                fm2 = null;
                            }
                            break;
                        default:
                            backend.RecieveCommand(new(WindowsRCURecieveCommandTypes.AddTrackToPlaylist));
                            break;
                    }
                    break;
                case ListViewMode.Files:
                case ListViewMode.SelectDrive:
                    backend.RecieveCommand(new(WindowsRCURecieveCommandTypes.GetFilesAndDirs) { Data = new Microsoft.IO.DirectoryInfo(SystemInfo.GetKnownFolder(ShellKnownFolder.Downloads)) });
                    break;
                case ListViewMode.PlaylistSelect:
                    if (state.DDFWindowIsActive) {
                        MusicPlayerHelper.ShowErrorResourceMessage("Error_AnotherDownloadOnProgress");
                        return;
                    }
                    System.Threading.Thread dt = new(() => {
                        DownloadAFileForm fm_1 = new(Global.DownloadedTracksDirectory);
                        state.DDFWindowIsActive = true;
                        if (fm_1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                        {
                            // 
                            // Temporarily open the playlist , add the track and re-close it (If we can do that in fact)
                            DownloadedFilesPlaylist dfp = null;
                            if (backend.ListViewMode == ListViewMode.LoadedPlaylist &&
                                    backend.Playlist is DownloadedFilesPlaylist d) {
                                try {
                                    d.AddDownloadedFile(fm_1.DownloadedFile);
                                    backend.RecieveCommand(new(CommonRecieveCommandTypes.ReloadOpenedPlaylist));
                                } catch (Exception ex) {
                                    MusicPlayerHelper.ShowErrorMessage($"Cannot save the downloaded file {fm_1?.DownloadedFile?.Name} due to an error: {ex}");
                                    state.DDFWindowIsActive = false;
                                    fm_1?.Dispose();
                                    return;
                                } finally { state.DDFWindowIsActive = false; fm_1?.Dispose(); }
                                return;
                            }
                            try {
                                dfp = new(Global.DownloadedTracksDirectory, Global.DownloadedTracksPlaylist);
                                dfp.AddDownloadedFile(fm_1.DownloadedFile);
                                dfp.SaveCurrentPlaylistState();
                                // Do a refresh on the playlist object to ensure that Playlist Gatherer will find the file.
                                Global.DownloadedTracksPlaylist.Refresh();
                            } catch (Exception ex) {
                                MusicPlayerHelper.ShowErrorMessage($"Cannot save the downloaded file {fm_1?.DownloadedFile?.Name} due to an error: {ex}");
                                state.DDFWindowIsActive = false;
                                fm_1?.Dispose();
                                return;
                            } finally { dfp?.Dispose(); }
                            // Refresh playlist states, if we are in the playlist selection state
                            if (backend.ListViewMode == ListViewMode.PlaylistSelect) {
                                backend.RecieveCommand(new(CommonRecieveCommandTypes.GetPlaylists));
                            }
                        }
                        state.DDFWindowIsActive = false;
                        fm_1.Dispose();
                    });
                    dt.Priority = System.Threading.ThreadPriority.Highest;
                    dt.TrySetApartmentState(System.Threading.ApartmentState.STA);
                    dt.Start();
                    break;
            }
        }

        private void GenericButton5_Click(object sender, RoutedEventArgs e)
        {
            switch (backend.ListViewMode)
            {
                case ListViewMode.LoadedPlaylist:
                    if (backend.Playlist is TrackPlayList pl) {
                        System.Threading.Thread th = new((System.Object plt) => {
                            using (CreatePlaylistArchiveDialog dlg = new(plt as TrackPlayList , backend.IconCache))
                            {
                                dlg.ShowDialog(MusicPlayerHelper.WinFormsHandle);
                            }
                        });
                        th.TrySetApartmentState(System.Threading.ApartmentState.STA);
                        th.IsBackground = true;
                        th.Name = Global.Resources.GetStringResource("ArchCreator_MainWindowThreadName");
                        th.Start(pl);
                    } else {
                        MusicPlayerHelper.ShowErrorResourceMessage("Error_ArchivedPlaylistCannotBeArchived" , backend.Gamepad);
                    }
                    break;
                case ListViewMode.PlaylistSelect:
                    using (StatisticsViewer sv = new(backend.StatisticsCache))
                    {
                        sv.ShowDialog(MusicPlayerHelper.WinFormsHandle);
                    }
                    break;
            }
        }

        private void GenericButton6_Click(object sender, RoutedEventArgs e)
        {
            switch (backend.ListViewMode)
            {
                case ListViewMode.LoadedPlaylist:
                    using (StatisticsViewer sv = new(backend.StatisticsCache , backend.Playlist.Metadata))
                    {
                        sv.ShowDialog(MusicPlayerHelper.WinFormsHandle);
                    }
                    break;
            }
        }

        private void SettingsButton_Click(object sender, RoutedEventArgs e)
        {
            RecieveCmd(null, new(CommonSendCommandTypes.ThrowWaitMessage) { MessageData = "Loading settings tree..." });
            SettingsTree.SettingsTreeBuilder stb = new(Global);
            stb.Build();
            using MP.SettingsEditorNew editor = new(stb , backend.ExtensionEngineInstance);
            RecieveCmd(null, new(CommonSendCommandTypes.ClearWaitMessage));
            editor.ShowDialog();
            stb = null;
            if (editor.ShouldUpdateDevice)
            {
                backend.ChangeAudioDevice(Global.AudioDeviceId);
            }
            lvw.BackColor = Global.ExplorationViewBackColor;
            lvw.ForeColor = Global.ExplorationViewForeColor;
            lvwcms.BackColor = Global.ExplorationViewBackColor;
            lvwcms.ForeColor = Global.ExplorationViewForeColor;
        }

        private void PlayButton_Click(object sender, RoutedEventArgs e)
        {
            if (backend.Playlist is not null)
            {
                if (backend.Player is not null) {
                    switch (backend.Player.CurrentState)
                    {
                        case PlaybackState.Playing:
                            PlayImage.Dispatcher.Invoke(new System.Action(() => PlayImage.Source = Global.Resources.LoadXamlImage("PlayImg")));
                            backend.Player.Pause();
                            break;
                        case PlaybackState.Paused:
                            PlayImage.Dispatcher.Invoke(new System.Action(() => PlayImage.Source = Global.Resources.LoadXamlImage("PauseImg")));
                            backend.Player.Play();
                            break;
                        case PlaybackState.Stopped:
                            PlayImage.Dispatcher.Invoke(new System.Action(() => PlayImage.Source = Global.Resources.LoadXamlImage("PauseImg")));
                            backend.RichPresense.UpdateInfo(new()
                            {
                                Assets = new() { LargeImage = "appicon", LargeText = "", SmallImage = "appicon_small", SmallText = "" },
                                State = Global.Resources.GetStringResource("DRP_STATE_LISTENING"),
                                Details = System.String.Format(
                                    Global.Resources.GetStringResource("DRP_DETAILS_NOWPLAYING"),
                                    backend.GetStringAttribute("PlaylistNameField"),
                                    backend.Playlist.GetCurrentTrackIndex()
                                ),
                                IsInstancedSession = true,
                                Timestamps = default
                            });
                            backend.Player.Play();
                            break;
                    }
                } else {
                    System.Int32 tid = 0;
                    if (backend.Playlist.CurrentTrack is not null) {
                        tid = backend.CurrentTrackIndex;
                    } else if (backend.Playlist.TracksContained.Count > 0) {
                        tid = 0;
                    } else {
                        return;
                    }
                    backend.RecieveCommand(new(CommonRecieveCommandTypes.LoadTrackOrdinal) { Data = tid });
                }
            }
        }

        private void RunLastTrack(System.Int32 idx)
        {
            if (lvw.SelectedItems.Count <= 0) {
                MusicPlayerHelper.ShowErrorResourceMessage("Error_NoPlaylistProvided_PlayLastTrack" , backend.Gamepad);
                return;
            }
            if (idx >= 0) { backend.RecieveCommand(new(CommonRecieveCommandTypes.OpenPlaylistUseLastTrack) { Data = lvw.Items[idx].Tag }); }
        }

        private void LDTFR_LoadPictureThread(System.Object obj)
        {
            System.Threading.Thread.Sleep(100);
            Microsoft.IO.MemoryStream MS = null;
            try
            {
                MS = new(obj as System.Byte[]);
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
            } catch {

            } finally {
                MS?.Dispose();
            }
            MS = null;
        }

        private void LoadTagDataFromReader(SavedDataTag rdr)
        {
            if (rdr is null || rdr.DataExist == false) { return; }
            OtherData.Dispatcher.Invoke(() => OtherData.Content =
            $"Artists: {rdr.ContributingArtists} Album: {rdr.AlbumName}\n" +
            $"Genre: {rdr.Genre} Album Artist: {rdr.AlbumArtist}\n" +
            $"Bitrate: {backend.Player.BitsPerSample} bit Sample Rate: {backend.Player.SamplesPerSecond} kHz");
            if (rdr.Image is not null && rdr.Image.LongLength > 0) {
                System.Threading.Thread TG = new(LDTFR_LoadPictureThread);
                TG.TrySetApartmentState(System.Threading.ApartmentState.STA);
                TG.Start(rdr.Image);
            }
        }

        private void Host_PlaybackStopped(PlaybackStoppedEventInfo ei)
        {
            DebugProvider.WriteLine("RCU: Stopping player due to dispatched message.");
            if (ei.Reason == PlaybackStoppedReason.Exception && ei.Exception is not SpecialStopButtonAssertionException)
            {
                backend.RecieveCommand(new(CommonRecieveCommandTypes.DestroyPlayer));
                MusicPlayerHelper.ShowErrorResourceMessage("Error_NativePlayerException", ei.Exception);
                DebugProvider.WriteLine($"RCU: Player stopped due to error: {ei.Exception}.");
                return;
            }
            if (ei.Reason == PlaybackStoppedReason.EndOfStream)
            {
                var s = backend.StatisticsCache.Get("NumberOfLifeTimeTracksPlayed");
                s.IncrementNumericValue();
                backend.StatisticsCache.Update(s);
            }
            backend.RichPresense.UpdateInfo(new() { Assets = new() { LargeImage = "appicon", LargeText = "Listening", SmallImage = "appicon_small", SmallText = "" }, State = "Listening", Details = $"On Playlist {backend.Playlist?.PlaylistName}\nPlayback Stopped." });
            PlayerInstance host = backend.Player;
            if (host is not null) {
                if (ei.Reason == PlaybackStoppedReason.EndOfStream)
                {
                    host.CurrentReachedTime = new(0, 0, 0);
                    TimeRemaining.Dispatcher.Invoke(void (PlayerInstance pi) => TimeRemaining.Content = pi.TotalTrackTime.ToString(@"hh\:mm\:ss\.ff"), host);
                }
                else
                {
                    TimeRemaining.Dispatcher.Invoke(void (PlayerInstance pi) => TimeRemaining.Content = pi.RemainingTime.ToString(@"hh\:mm\:ss\.ff"), host);
                }
            } else {
                TimeRemaining.Dispatcher.Invoke(void () => TimeRemaining.Content = "--:--:--.--");
            }
            TimeBar.Dispatcher.Invoke(() => {
                //TimeBar.Maximum = 1;
                TimeBar.Value = 0;
            });
            TimeElapsed.Dispatcher.Invoke(() => TimeElapsed.Content = "00:00:00.00");
            if (backend.Playlist.Metadata is not null && ei.Reason == PlaybackStoppedReason.EndOfStream)
            {
                backend.Playlist.Metadata["LifetimeTracksPlayed"] = unchecked(((System.UInt64)backend.Playlist.Metadata["LifetimeTracksPlayed"]) + 1);
            }
            if (Global.Resources is not null)
            {
                PlayImage.Dispatcher.Invoke(void () => PlayImage.Source = Global.Resources.LoadXamlImage("PlayImg"));
            }
        }

        private void Host_UpdateMusicDisplay(object sender, UpdateMusicDisplayEventArgs e)
        {
            TimeBar.Dispatcher.InvokeAsync(() => { 
                TimeBar.Maximum = e.TotalTrackTime.Ticks;
                TimeBar.Value = e.CurrentReachedTime.Ticks;
            });
            TimeElapsed.Dispatcher.InvokeAsync(new Action(() => TimeElapsed.Content = e.CurrentReachedTime.ToString(@"hh\:mm\:ss\.ff")));
            TimeRemaining.Dispatcher.InvokeAsync(new Action(() => TimeRemaining.Content = e.TotalTrackTime.Subtract(e.CurrentReachedTime).ToString(@"hh\:mm\:ss\.ff")));
        }

        private void Host_RepeatRequestSuccessfull(object sender, EventArgs e)
        {
            DebugProvider.WriteLine("RCU: Repeating audio file for current audio device.");
            PlayImage.Dispatcher.Invoke(() => { PlayImage.Source = Global.Resources.LoadXamlImage("PauseImg"); });
        }

        private void StopButton_Click(object sender, RoutedEventArgs e)
        {
            if (backend.Player is null) { return; }
            if (backend.Player.CurrentState == PlaybackState.Stopped) {
                backend.RecieveCommand(new(CommonRecieveCommandTypes.DestroyPlayer));
                return;
            }
            backend.Player.Stop(true);
        }

        private void DeletePlayerScreen()
        {
            if (state.UIShutdown) { return; }
            TimeBar.Dispatcher.Invoke(new System.Action(() => TimeBar.Value = 0));
            TimeElapsed.Dispatcher.Invoke(new System.Action(() => TimeElapsed.Content = "00:00:00.00"));
            TimeRemaining.Dispatcher.Invoke(new System.Action(() => TimeRemaining.Content = "--:--:--.--"));
            if (Global.Resources is not null) {
                PlayImage.Dispatcher.Invoke(new System.Action(() => PlayImage.Source = Global.Resources.LoadXamlImage("PlayImg")));
            }
            CoverImage.Dispatcher.Invoke(() => {
                CoverImage.Source = null;
                CoverImage.Visibility = Visibility.Hidden;
            });
            TrackTitle.Dispatcher.Invoke(new System.Action(() => TrackTitle.Content = ""));
            OtherData.Dispatcher.Invoke(new System.Action(() => OtherData.Content = ""));
        }

        private void RightVolume_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            Global.RightChannelVolume = (System.Byte)e.NewValue;
            RightVolumeLabel.Dispatcher.Invoke(new System.Action(() => RightVolumeLabel.Content = $"Right Speaker: ({Global.RightChannelVolume} %)"));
            if (backend.Player is null) { return; }
            backend.Player.RightSpeakerVolume = Global.RightChannelVolume;
        }

        private void LeftVolume_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            Global.LeftChannelVolume = (System.Byte)e.NewValue;
            LeftVolumeLabel.Dispatcher.Invoke(new System.Action(() => LeftVolumeLabel.Content = $"Left Speaker: ({Global.LeftChannelVolume} %)"));
            if (backend.Player is null) { return; }
            backend.Player.LeftSpeakerVolume = Global.LeftChannelVolume;
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

        private void TenSecsBack_Click(object sender, RoutedEventArgs e) => backend.Player?.TenSecondsBehind();

        private void TenSecsAhead_Click(object sender, RoutedEventArgs e) => backend.Player?.TenSecondsAhead();

        private void TimeBar_QueryCursor(object sender, System.Windows.Input.QueryCursorEventArgs e)
        {
            if (backend.Player is not null && e.LeftButton == System.Windows.Input.MouseButtonState.Pressed)
            {
                Size se = TimeBar.RenderSize;
                Point pp = e.GetPosition(TimeBar);
                if (pp.X >= (se.Width / 2)) {
                    backend.Player.OneSecondAhead();
                } else {
                    backend.Player.OneSecondBehind();
                }
            }
        }

        private void PrevTrack_Click(object sender, RoutedEventArgs e)
        {
            if (backend.Playlist is not null)
            {
                System.Int32 playidx = backend.CurrentTrackIndex;
                if (playidx > 0) { playidx--; }
                backend.RecieveCommand(new(CommonRecieveCommandTypes.LoadTrackOrdinal) { Data = playidx });
            }
        }

        private void NextTrack_Click(object sender, RoutedEventArgs e)
        {
            if (backend.Playlist is not null)
            {
                System.Int32 playidx = backend.CurrentTrackIndex;
                if (playidx > backend.Playlist.TracksContained.Count - 2) { playidx = 0; } else { playidx++; }
                backend.RecieveCommand(new(CommonRecieveCommandTypes.LoadTrackOrdinal) { Data = playidx });
            }
        }

        private void RepeatButton_Click(object sender, RoutedEventArgs e) => backend.RecieveCommand(new(WindowsRCURecieveCommandTypes.SelectRepeatMode));

        private void CreateRotatingTitleIfNeeded(System.Object data)
        {
            // Creates a 'rotating' title in the case that the title is not fully filled 
            // into the title field. A new thread takes this burden so as to work.
            // Currently width 410 with ClearType settings corresponds to 39 fully interpretable characters.
            if (data is null) { return; }
            System.String dt = (System.String)data;
            if (dt.Length < 40) { return; }
            token = new(token);
            var rotating = new System.Threading.Thread((System.Object obj) => { 
                // Code perf: casting happens only once.
                System.String title = (System.String)obj; 
                // Note that the current code is optimized to break immediately when the token requests to 
                // terminate the thread.
                System.Int32 index = 0 , tid = token.InstanceId;
                void HaltThread(System.Int32 t) {
                    System.Int32 time = t;
                    while (token.IsValid(tid) && time > 0) { System.Threading.Thread.Sleep(10); time -= 10; }
                }
            rotate:
                index = 0;
                while (backend is not null && backend.IsShuttingDown == false && token.IsValid(tid) && index + 39 < title.Length) {
                    // Rotate until all chars are processed. (Or break if the thread is requested to terminate)
                    TrackTitle.Dispatcher.Invoke(() => { TrackTitle.Content = title.Substring(index, 40); });
                    HaltThread(650);
                    index++;
                }
                if (backend is not null && backend.IsShuttingDown == false && token.IsValid(tid)) {
                    // Hold the title still for 1.66 seconds.
                    TrackTitle.Dispatcher.Invoke(() => { TrackTitle.Content = title.Substring(0 , 40); });
                    HaltThread(1670);
                    goto rotate; 
                }
            });
            rotating.Name = Global.Resources.GetStringResource("RotatingTitleThread_Name");
            rotating.Start(data);
        }

        private void HandleControllerCmd(System.String command)
        {
            switch (command) 
            {
                case "CLOSE":
                    if (state.AssertClosing == false && IsActive) { state.AssertClosing = true; Close(); }
                    break;
                case "OPTION":
                    // Avoid calling options panel when no items selected , and when the panel has already been invoked.
                    if (lvw.SelectedItems.Count == 0 || state.OptionsPanelMode) { return; }
                    state.OptionsPanelMode = true;
                    lvwcms.Show(lvw, lvw.SelectedItems[0].Position);
                    break;
                case "PLAY":
                    if (state.OptionsPanelMode) {
                        LVWCMS_ItemClicked(null, new(lvwcms.Items[state.OptionsPanelIndex]));
                        // After clicking an item , the options panel must be closed , so update the value and close the panel if needed.
                        state.OptionsPanelMode = false;
                        lvwcms.Close();
                        return;
                    }
                    if (backend is null) { return; }
                    if (backend.ListViewMode == ListViewMode.LoadedPlaylist) {
                        if (backend.Player is null) {
                            if (state.ListViewIndex >= lvw.Items.Count) { state.ListViewIndex = 0; }
                            if (state.ListViewIndex <= -1) { state.ListViewIndex = 0; }
                            backend.RecieveCommand(new(CommonRecieveCommandTypes.LoadTrackOrdinal) { Data = state.ListViewIndex });
                        } else {
                            PlayButton_Click(null, new());
                        }
                    } else if (backend.ListViewMode == ListViewMode.PlaylistSelect) {
                        if (state.ListViewIndex <= -1) { return; }
                        backend.RecieveCommand(new(CommonRecieveCommandTypes.OpenPlaylist) { Data = lvw.Items[state.ListViewIndex].Tag });
                    }
                    break;
                case "STOP":
                    if (state.OptionsPanelMode) { lvwcms.Close(); state.OptionsPanelMode = false; return; }
                    if (backend.ListViewMode == ListViewMode.LoadedPlaylist)
                    {
                        StopButton_Click(null, new());
                    } else if (backend.ListViewMode == ListViewMode.PlaylistSelect)
                    {
                        if (state.ListViewIndex >= lvw.Items.Count) { state.ListViewIndex = 0; }
                        if (state.ListViewIndex <= -1) { state.ListViewIndex = 0; }
                        RunLastTrack(state.ListViewIndex);
                    }
                    break;
                case "PREV":
                    // Avoid accidental calls to the d-pad.
                    if (state.OptionsPanelMode) { return; }
                    PrevTrack_Click(null, new());
                    break;
                case "NEXT":
                    // Avoid accidental calls to the d-pad.
                    if (state.OptionsPanelMode) { return; }
                    NextTrack_Click(null, new());
                    break;
                case "DOWN":
                    if (state.OptionsPanelMode)
                    {
                        state.OptionsPanelIndex++;
                        if (state.OptionsPanelIndex >= lvwcms.Items.Count) { state.OptionsPanelIndex = 0; }
                        lvwcms.Items[state.OptionsPanelIndex].Select();
                        return;
                    }
                    if (lvw.Items.Count == 0) { return; }
                    state.ListViewIndex++;
                    if (state.ListViewIndex >= lvw.Items.Count) { state.ListViewIndex = 0; }
                    lvw.SelectedIndices.Clear();
                    lvw.SelectedIndices.Add(state.ListViewIndex);
                    lvw.SelectedItems[0].EnsureVisible();
                    break;
                case "UP":
                    if (state.OptionsPanelMode)
                    {
                        state.OptionsPanelIndex--;
                        if (state.OptionsPanelIndex < 0) { state.OptionsPanelIndex = 0; }
                        lvwcms.Items[state.OptionsPanelIndex].Select();
                        return;
                    }
                    if (lvw.Items.Count == 0) { return; }
                    state.ListViewIndex--;
                    if (state.ListViewIndex <= -1)
                    {
                        state.ListViewIndex = 0;
                    }
                    if (state.ListViewIndex >= lvw.Items.Count) { state.ListViewIndex = 0; }
                    lvw.SelectedIndices.Clear();
                    lvw.SelectedIndices.Add(state.ListViewIndex);
                    lvw.SelectedItems[0].EnsureVisible();
                    break;
                case "SEL_SPK":
                    state.ChangeSelectedChannel();
                    if (state.SelectedChannel == 0)
                    {
                        // Left Channel
                        System.Threading.Thread TD = new(() => {
                            LeftVolumeLabel.Dispatcher.Invoke(new System.Action(() => LeftVolumeLabel.Content = "Left Speaker: (Selected)"));
                            System.Threading.Thread.Sleep(1500);
                            LeftVolumeLabel.Dispatcher.Invoke(new System.Action(() => LeftVolumeLabel.Content = "Left Speaker:"));
                        });
                        TD.Start();
                    } else {
                        // Right Channel
                        System.Threading.Thread TD = new(() => {
                            RightVolumeLabel.Dispatcher.Invoke(new System.Action(() => RightVolumeLabel.Content = "Right Speaker: (Selected)"));
                            System.Threading.Thread.Sleep(1500);
                            RightVolumeLabel.Dispatcher.Invoke(new System.Action(() => RightVolumeLabel.Content = "Right Speaker:"));
                        });
                        TD.Start();
                    }
                    break;
                case "SPK_UP":
                    if (state.SelectedChannel == 0)
                    {
                        // Left Channel
                        System.Threading.Thread TD = new(() => {
                            Global.LeftChannelVolume += 5;
                            if (Global.LeftChannelVolume > 100) { Global.LeftChannelVolume = 100; }
                            LeftVolume.Dispatcher.Invoke(() => { LeftVolume.Value = Global.LeftChannelVolume; });
                            System.Threading.Thread.Sleep(1500);
                            LeftVolumeLabel.Dispatcher.Invoke(new System.Action(() => LeftVolumeLabel.Content = "Left Speaker:"));
                        });
                        TD.Start();
                    } else {
                        // Right Channel
                        System.Threading.Thread TD = new(() => {
                            Global.RightChannelVolume += 5;
                            if (Global.RightChannelVolume > 100) { Global.RightChannelVolume = 100; }
                            RightVolume.Dispatcher.Invoke(() => { RightVolume.Value = Global.RightChannelVolume; });
                            System.Threading.Thread.Sleep(1500);
                            RightVolumeLabel.Dispatcher.Invoke(new System.Action(() => RightVolumeLabel.Content = "Right Speaker:"));
                        });
                        TD.Start();
                    }
                    break;
                case "SPK_DOWN":
                    if (state.SelectedChannel == 0)
                    {
                        // Left Channel
                        System.Threading.Thread TD = new(() => {
                            Global.LeftChannelVolume -= 5;
                            if (Global.LeftChannelVolume > 100) { Global.LeftChannelVolume = 0; }
                            LeftVolume.Dispatcher.Invoke(() => { LeftVolume.Value = Global.LeftChannelVolume; });
                            System.Threading.Thread.Sleep(1500);
                            LeftVolumeLabel.Dispatcher.Invoke(new System.Action(() => LeftVolumeLabel.Content = "Left Speaker:"));
                        });
                        TD.Start();
                    } else {
                        // Right Channel
                        System.Threading.Thread TD = new(() => {
                            Global.RightChannelVolume -= 5;
                            if (Global.RightChannelVolume > 100) { Global.RightChannelVolume = 0; }
                            RightVolume.Dispatcher.Invoke(() => { RightVolume.Value = Global.RightChannelVolume; });
                            System.Threading.Thread.Sleep(1500);
                            RightVolumeLabel.Dispatcher.Invoke(new System.Action(() => RightVolumeLabel.Content = "Right Speaker:"));
                        });
                        TD.Start();
                    }
                    break;
                case "CHANGE_MODE":
                    if (backend.ListViewMode == ListViewMode.LoadedPlaylist) {
                        backend.RecieveCommand(new(CommonRecieveCommandTypes.CloseActivePlaylist) { Data = lvw.Items.Count > 0 });
                        backend.RecieveCommand(new(CommonRecieveCommandTypes.GetPlaylists));
                    }
                    break;
            }
        }

        private void ModifyContextMenuStrip_LVWCMS()
        {
            lvwcms.Invoke(lvwcms.Items.Clear);
            switch (backend.ListViewMode)
            {
                case ListViewMode.LoadedPlaylist:
                    lvwcms.Invoke(() => {
                        lvwcms.Items.AddRange(new ToolStripItem[] {
                            new ToolStripMenuItem(Global.Resources.GetStringResource("LVW_LOADEDPL_TSI_EDT_1")) { Name = "EDT_1" },
                            new ToolStripMenuItem(Global.Resources.GetStringResource("LVW_LOADEDPL_TSI_EDT_2")) { Name = "EDT_2" },
                            new ToolStripMenuItem(Global.Resources.GetStringResource("LVW_LOADEDPL_TSI_EDT_3")) { Name = "EDT_3" },
                            new ToolStripMenuItem(Global.Resources.GetStringResource("LVW_LOADEDPL_TSI_EDT_4")) { Name = "EDT_4" },
                        });
                        foreach (var dt in state.AdditionalPlaylistModeRightClickButtons)
                        {
                            lvwcms.Items.Add(new ToolStripMenuItem(dt.ButtonText) { Name = dt.ButtonID });
                        }
                    });
                    break;
                case ListViewMode.PlaylistSelect:
                    lvwcms.Invoke(() => {
                        lvwcms.Items.AddRange([
                            new ToolStripMenuItem(Global.Resources.GetStringResource("LVWCMS_PLSELECT_PLS_1")) { Name = "PLS_1" },
                            new ToolStripMenuItem(Global.Resources.GetStringResource("LVWCMS_PLSELECT_PLS_2")) { Name = "PLS_2" }
                        ]);
                    });
                    break;
            }
        }

        private void RecieveCmd(System.Object send , SendCommandDataEventArgs eventargs)
        {
            // Note that the send represents the backend that caused the event to trigger.
            var dispatch = Dispatcher; // Keep the main dispatcher.
            switch (eventargs.CommandType) // Switch through the commands.
            {
                case CommonSendCommandTypes.IgnoreFutureRequests:
                    // This will detach the UI from the backend. Any future requests are ignored by the UI.
                    backend.SendCommand -= RecieveCmd; 
                    break;
                case CommonSendCommandTypes.HardFailGracefulExitRequested:
                    state.HardFailureOccured = true;
                    DebugProvider.WriteLine("UIManager: Closing due to hard error. Something went wrong in the RCU instance.");
                    MusicPlayerHelper.ShowErrorResourceMessage("Error_RCUEngineCannotStartHardError", eventargs.Metadata.GetItem("Exception").Value);
                    dispatch.Invoke(Close);
                    break;
                case CommonSendCommandTypes.Refresh:
                    dispatch.Invoke(UpdateLayout);
                    break;
                case CommonSendCommandTypes.ClearPlayerScreen:
                    token?.Invalidate();
                    DeletePlayerScreen();
                    break;
                case CommonSendCommandTypes.LoadPlayerScreen:
                    // OK. We must load the player information.
                    OtherData.Dispatcher.Invoke(() => {
                        OtherData.Content = $"Bitrate: {backend.Player.BitsPerSample} bit\nChannels: {backend.Player.Channels}\n" +
                        $"Sample Rate: {backend.Player.SamplesPerSecond} kHz";
                    });
                    SavedDataTag tag = backend.GetTag();
                    TrackTitle.Dispatcher.Invoke(() => {
                        if ((tag is not null && tag.DataExist) && System.String.IsNullOrWhiteSpace(tag.Title2) == false)
                        {
                            TrackTitle.Content = tag.Title2;
                        } else if (backend.Playlist is not null) {
                            TrackTitle.Content = backend.Playlist.CurrentTrack.GetNameOnly();
                        } else {
                            TrackTitle.Content = Global.Resources.GetStringResource("NoTitleExists_PlayerScreen");
                        }
                        CreateRotatingTitleIfNeeded(TrackTitle.Content);
                    });
                    PlayImage.Dispatcher.Invoke(() => { PlayImage.Source = Global.Resources.LoadXamlImage("PauseImg"); });
                    LoadTagDataFromReader(tag);
                    break;
                case CommonSendCommandTypes.ClearExplorationScreen:
                    ClearView();
                    break;
                case CommonSendCommandTypes.LoadExplorationScreen:
                    UpdateView(eventargs.Metadata);
                    break;
                case CommonSendCommandTypes.AttachPlayerEvents:
                    if (backend.Player is null) { break; }
                    backend.Player.RepeatRequestSuccessfull += Host_RepeatRequestSuccessfull;
                    backend.Player.PlaybackStopped += Host_PlaybackStopped;
                    backend.Player.UpdateMusicDisplay += Host_UpdateMusicDisplay;
                    break;
                case CommonSendCommandTypes.DetachPlayerEvents:
                    if (backend.Player is null) { break; }
                    DebugProvider.WriteLine("RCU: Attempting to detach the player events...");
                    backend.Player.RepeatRequestSuccessfull -= Host_RepeatRequestSuccessfull;
                    backend.Player.PlaybackStopped -= Host_PlaybackStopped;
                    backend.Player.UpdateMusicDisplay -= Host_UpdateMusicDisplay;
                    DebugProvider.WriteLine("RCU: Player events are detached from the UI.");
                    break;
                case CommonSendCommandTypes.ThrowMessage:
#if DEBUG
                    System.Windows.Forms.MessageBox.Show(eventargs.MessageData, "Error" , MessageBoxButtons.OK , MessageBoxIcon.Error);
#else
                    MusicPlayerHelper.ShowErrorMessage(eventargs.MessageData);
#endif
                    break;
                case CommonSendCommandTypes.ThrowTitleMessage:
                    TrackTitle.Dispatcher.Invoke((System.String dat) => { TrackTitle.Content = dat; } , eventargs.MessageData);
                    break;
                case CommonSendCommandTypes.ClearTitleMessage:
                    TrackTitle.Dispatcher.Invoke(() => { TrackTitle.Content = System.String.Empty; });
                    break;
                case WindowsRCUSendCommandTypes.InvokeWaitCursor:
                    dispatch.Invoke((Action)(() => Cursor = System.Windows.Input.Cursors.Wait));
                    WFH_LISTVIEW_1.Dispatcher.Invoke(new(() => {
                        if (lvw is not null) {
                            lvw.Cursor = Cursors.WaitCursor;
                        }
                    }));
                    break;
                case WindowsRCUSendCommandTypes.RemoveWaitCursor:
                    dispatch.Invoke((Action)(() => Cursor = null));
                    WFH_LISTVIEW_1.Dispatcher.Invoke(new(() => { 
                        if (lvw is not null) {
                            lvw.Cursor = Cursors.Default;
                        }
                    }));
                    break;
                case CommonSendCommandTypes.UpdateSelectedIndex:
                    lvw.Invoke(() => {
                        try {
                            lvw.BeginUpdate();
                            lvw.SelectedIndices.Clear();
                            lvw.SelectedIndices.Add(backend.CurrentTrackIndex);
                            lvw.SelectedItems[0].EnsureVisible();
                        } finally { lvw.EndUpdate(); }
                    });
                    break;
                case CommonSendCommandTypes.ControllerCommand:
                    // both of the below properties must be checked before firing a controller command!
                    if (state.Enabled == false || state.AssertClosing) { return; }
                    dispatch.Invoke(HandleControllerCmd , eventargs.MessageData);
                    break;
                case WindowsRCUSendCommandTypes.ChangeRepeatModeImage:
                    switch (backend.RepeatMode)
                    {
                        case RepeatMode.No:
                            RepeatImage.Dispatcher.Invoke(() => { RepeatImage.Source = Global.Resources.LoadXamlImage("Repeat"); });
                            break;
                        case RepeatMode.One:
                            RepeatImage.Dispatcher.Invoke(() => { RepeatImage.Source = Global.Resources.LoadXamlImage("RepeatEnabled"); });
                            break;
                        case RepeatMode.All:
                            RepeatImage.Dispatcher.Invoke(() => { RepeatImage.Source = Global.Resources.LoadXamlImage("RepeatAll"); });
                            break;
                    }
                    break;
                case WindowsRCUSendCommandTypes.UpdateSelectedMode:
                    switch (backend.ListViewMode)
                    {
                        case ListViewMode.LoadedPlaylist:
                            switch (backend.Playlist)
                            {
                                case ArchivedTrackPlaylist:
                                    GenericButton1.Dispatcher.Invoke(() => { GenericButton1.Content = Global.Resources.GetStringResource("GButton1_LP_ShowArchivedPlaylistData_String"); });
                                    GenericButton2.Dispatcher.Invoke(() => { GenericButton2.Content = Global.Resources.GetStringResource("GButton2_LoadedPlaylist_String"); });
                                    GenericButton3.Dispatcher.Invoke(() => { GenericButton3.Content = Global.Resources.GetStringResource("GButton3_LoadedPlaylist_String"); });
                                    GenericButton4.Dispatcher.Invoke(() => { GenericButton4.Content = Global.Resources.GetStringResource("GButton4_LP_ExportArchivedPlaylist_String"); });
                                    GenericButton5.Dispatcher.Invoke(() => { GenericButton5.Visibility = Visibility.Hidden; });
                                    GenericButton6.Dispatcher.Invoke(() => { GenericButton6.Visibility = Visibility.Hidden; });
                                    break;
                                default:
                                    GenericButton1.Dispatcher.Invoke(() => { GenericButton1.Content = Global.Resources.GetStringResource("GButton1_LoadedPlaylist_String"); });
                                    GenericButton2.Dispatcher.Invoke(() => { GenericButton2.Content = Global.Resources.GetStringResource("GButton2_LoadedPlaylist_String"); });
                                    GenericButton3.Dispatcher.Invoke(() => { GenericButton3.Content = Global.Resources.GetStringResource("GButton3_LoadedPlaylist_String"); });
                                    GenericButton4.Dispatcher.Invoke(() => { GenericButton4.Content = Global.Resources.GetStringResource("GButton4_LoadedPlaylist_String"); });
                                    GenericButton5.Dispatcher.Invoke(() => { GenericButton5.Visibility = Visibility.Visible; GenericButton5.Content = Global.Resources.GetStringResource("GButton5_LoadedPlaylist_String"); });
                                    GenericButton6.Dispatcher.Invoke(() => { GenericButton6.Visibility = Visibility.Visible; GenericButton6.Content = Global.Resources.GetStringResource("GButton6_LoadedPlaylist_String"); });
                                    break;
                            }
                            break;
                        case ListViewMode.Files:
                        case ListViewMode.SelectDrive:
                            GenericButton1.Dispatcher.Invoke(() => { GenericButton1.Content = Global.Resources.GetStringResource("GButton1_Files_String"); });
                            GenericButton2.Dispatcher.Invoke(() => { GenericButton2.Content = Global.Resources.GetStringResource("GButton2_Files_String"); });
                            GenericButton3.Dispatcher.Invoke(() => { GenericButton3.Content = Global.Resources.GetStringResource("GButton3_Files_String"); });
                            GenericButton4.Dispatcher.Invoke(() => { GenericButton4.Content = Global.Resources.GetStringResource("GButton4_Files_String"); });
                            GenericButton5.Dispatcher.Invoke(() => { GenericButton5.Visibility = Visibility.Hidden; });
                            GenericButton6.Dispatcher.Invoke(() => { GenericButton6.Visibility = Visibility.Hidden; });
                            break;
                        case ListViewMode.PlaylistSelect:
                            GenericButton1.Dispatcher.Invoke(() => { GenericButton1.Content = Global.Resources.GetStringResource("GButton1_PlaylistSelect_String"); });
                            GenericButton2.Dispatcher.Invoke(() => { GenericButton2.Content = Global.Resources.GetStringResource("GButton2_PlaylistSelect_String"); });
                            GenericButton3.Dispatcher.Invoke(() => { GenericButton3.Content = Global.Resources.GetStringResource("GButton3_PlaylistSelect_String"); });
                            GenericButton4.Dispatcher.Invoke(() => { GenericButton4.Content = Global.Resources.GetStringResource("GButton4_PlaylistSelect_String"); });
                            GenericButton5.Dispatcher.Invoke(() => { GenericButton5.Visibility = Visibility.Visible; GenericButton5.Content = Global.Resources.GetStringResource("GButton5_PlaylistSelect_String"); });
                            GenericButton6.Dispatcher.Invoke(() => { GenericButton6.Visibility = Visibility.Hidden; });
                            break;
                    }
                    ModifyContextMenuStrip_LVWCMS();
                    break;
                case CommonSendCommandTypes.ThrowWaitMessage:
                    if (waitdlg is null) {
                        waitdlg = new(eventargs.MessageData, MusicPlayerHelper.WinFormsHandle);
                    } else {
                        waitdlg.Text = eventargs.MessageData;
                    }
                    waitdlg.Show();
                    break;
                case CommonSendCommandTypes.ClearWaitMessage:
                    waitdlg?.Dispose();
                    break;
            }
        }
    }
}
