using System;
using Microsoft.IO;
using System.Windows.Forms;
using System.Collections.Generic;
using MP.M3UPlaylist;

namespace MP
{
    public partial class CreatePlaylistDialog : Form
    {
        private enum PlaylistCreationMode : System.Byte
        {
            SpecificItems,
            Directory,
            M3UPlaylist
        }

        private System.String basedirectory;
        private ListViewItem[] items;
        private System.String savedunder;
        private PlaylistCreationMode mode;
        private CreatePlaylistDialogResult result;

        private CreatePlaylistDialog()
        {
            InitializeComponent();
            mode = PlaylistCreationMode.SpecificItems;
            result = CreatePlaylistDialogResult.OK;
        }

        public static CreatePlaylistDialog FromM3UPlaylistPath(System.String m3u)
        {
            var dialog = new CreatePlaylistDialog();
            dialog.basedirectory = m3u;
            dialog.mode = PlaylistCreationMode.M3UPlaylist;
            return dialog;
        }

        public CreatePlaylistDialog(string basedirectory, params ListViewItem[] items) : this()
        {
            mode = PlaylistCreationMode.SpecificItems;
            this.basedirectory = basedirectory;
            this.items = items;
        }

        public CreatePlaylistDialog(System.String basedirectory) : this()
        {
            mode = PlaylistCreationMode.Directory;
            this.basedirectory = basedirectory;
        }

        private void ConfirmButton_Click(object sender, EventArgs e)
        {
            if (System.String.IsNullOrWhiteSpace(NameBox.Text))
            {
                MusicPlayerHelper.ShowErrorResourceMessage("CreatePlaylistDialog_InvalidNameChars");
                return;
            }
            foreach (var ext in Settings.Global.Resources.GetStringResource("AllPlaylistExtensions").Split(';'))
            {
                System.String playlistname = $"{NameBox.Text}{ext}";
                foreach (var file in Settings.Global.PlaylistsDirectory.GetFiles($"*{ext}"))
                {
                    if (file.Name == playlistname)
                    {
                        MusicPlayerHelper.ShowErrorResourceMessage("CreatePlaylistDialog_NameOccupied", NameBox.Text);
                        return;
                    }
                }
            }
            L1.Text = Settings.Global.Resources.GetStringResource("CreatePlaylistDialog_SavingPLText");
            System.Threading.Thread td = new(mode switch { 
                PlaylistCreationMode.Directory => ThreadCode_2 , 
                PlaylistCreationMode.SpecificItems => ThreadCode_1,
                PlaylistCreationMode.M3UPlaylist => ThreadCode_3,
                _ => null
            });
            td.Name = Settings.Global.Resources.GetStringResource("CreatePlaylistDialog_PlaylistSaverThreadName");
            td.Start(); 
        }

        private void ThreadCode_3()
        {
            FileStream fsm = null;
            M3UPlaylistReader pr = null;
            List<FileInfo> files = null;
            try {
                L1.Text = Settings.Global.Resources.GetStringResource("CreatePlaylistDialog_Status_LoadingM3U");
                fsm = new(basedirectory , FileMode.Open , FileAccess.Read , FileShare.Read);
                pr = new(fsm);
                files = new(pr.FilePaths.Count);
                L1.Text = System.String.Format(Settings.Global.Resources.GetStringResource("CreatePlaylistDialog_Status_LoadingM3UWithName") , pr.PlaylistName);
                FileInfo tfi;
                foreach (var fs in pr.FilePaths)
                {
                    tfi = new(fs);
                    if (tfi.Exists == false) { continue; }
                    files.Add(tfi);
                }
                if (files.Count == 0) 
                {
                    L1.Text = Settings.Global.Resources.GetStringResource("CreatePlaylistDialog_Error_NoFilesFound");
                    MusicPlayerHelper.ShowErrorResourceMessage("CreatePlaylistDialog_Error_M3UNoFilesFound_ThrownMessage");
                    result = CreatePlaylistDialogResult.Error;
                    if (IsHandleCreated) { Invoke(Close); }
                    return;
                }
                L1.Text = Settings.Global.Resources.GetStringResource("CreatePlaylistDialog_SavingPLText");
            } catch {
                L1.Text = Settings.Global.Resources.GetStringResource("CreatePlaylistDialog_Status_ErrorFound");
                result = CreatePlaylistDialogResult.Error;
                files?.Clear();
                files = null;
                if (IsHandleCreated) { Invoke(Close); }
                return;
            } finally {
                pr?.Dispose();
                pr = null;
                fsm?.Dispose();
                fsm = null;
            }
            SavePLData(files.ToArray());
            files.Clear();
            files = null; 
        }
        
        private void ThreadCode_2()
        {
            Microsoft.IO.DirectoryInfo DI = new(basedirectory);
            List<Microsoft.IO.FileInfo> files = new();
            try
            {
                foreach (System.String fmt in Settings.Global.Resources.GetStringResource("SupportedFormats").Split(';'))
                {
                    files.AddRange(DI.GetFiles(fmt));
                }
            } catch (System.Exception e) {
                switch (e)
                {
                    case UnauthorizedAccessException:
                        MusicPlayerHelper.ShowErrorResourceMessage("CreatePlaylistDialog_DirAccessError");
                        break;
                    case System.IO.IOException:
                        MusicPlayerHelper.ShowErrorResourceMessage("CreatePlaylistDialog_DirIOError");
                        break;
                }
                result = CreatePlaylistDialogResult.Error;
                if (IsHandleCreated) { Invoke(Close); }
                return;
            }
            if (files.Count == 0)
            {
                MusicPlayerHelper.ShowErrorResourceMessage("CreatePlaylistDialog_NoFilesFoundInDir");
                result = CreatePlaylistDialogResult.Error;
                if (IsHandleCreated) { Invoke(Close); }
                return;
            }
            SavePLData(files.ToArray());
            files.Clear();
            files = null;
        }

        private void ThreadCode_1()
        {
            Microsoft.IO.DirectoryInfo DI = new(basedirectory);
            List<Microsoft.IO.FileInfo> files = new();
            foreach (var file in DI.GetFiles())
            {
                foreach (var adding in items)
                {
                    if (file.Name == adding.Text && adding.ImageIndex == 1) { files.Add(file); }
                }
            }
            SavePLData(files.ToArray());
            files.Clear();
            files = null;
        }

        private void SavePLData(FileInfo[] files)
        {
            TrackPlayList TPL = new(files, NameBox.Text);
            files = null;
            FileStream FS = null;
            System.String pathc = Path.Join(Settings.Global.PlaylistsDirectory.FullName, NameBox.Text) + ".mpbpl";
            try {
                FS = new(pathc, FileMode.CreateNew);
                FS.Position = 0;
                TPL.SaveAsPlaylist(FS);
                savedunder = Path.GetFileName(pathc);
            } catch (Exception ex) {
                MusicPlayerHelper.ShowErrorMessage($"Failed to save playlist due to \n{ex}");
                result = CreatePlaylistDialogResult.Error;
                if (IsHandleCreated) { Invoke(Close); }
                return;
            } finally { 
                FS?.Dispose(); 
                TPL?.Dispose(); 
            }
            System.Threading.Thread.Sleep(1000);
            result = CreatePlaylistDialogResult.OK;
            if (IsHandleCreated) { Invoke(Close); }
        }

        private void CancelButton_Click(object sender, EventArgs e)
        {
            result = CreatePlaylistDialogResult.Cancelled;
            Close();
        }

        public System.String NameSavedUnder => savedunder;

        public System.Boolean OpenAfterCreated => OpenAfterCreation.CheckState == CheckState.Checked;

        /// <inheritdoc cref="Form.DialogResult"/>
        public new CreatePlaylistDialogResult DialogResult => result;
    }

    public enum CreatePlaylistDialogResult : System.Byte
    {
        OK = 0,
        Cancelled,
        Error
    }
}
