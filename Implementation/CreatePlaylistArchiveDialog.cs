using System;
using static MP.Settings;
using System.Windows.Forms;
using System.Collections.Generic;
using DotNetResourcesExtensions;


namespace MP
{
    internal partial class CreatePlaylistArchiveDialog : Form
    {
        private System.Int32 label_count;
        private TrackPlayList playlist;
        private System.String resultfile, iconcachefileifany;
        private ArchivedPlaylistAttributeCollection attributes;

        public CreatePlaylistArchiveDialog(TrackPlayList playlist , MP.Caches.PlaylistIconCache.PlaylistIconCacheInstance ici)
        {
            InitializeComponent();
            StatusLabel.Text = System.String.Format(Global.Resources.GetStringResource("CreatePA_Message1"), playlist.PlaylistName);
            if (CreatePlaylistArchiveInfrastracture.IsValidCancellationToken == false)
            {
                CreatePlaylistArchiveInfrastracture.GenerateThreadID();
            }
            this.playlist = playlist;
            label_count = 0;
            attributes = new();
            if (ici.PlaylistRegistered(playlist.PlaylistNameField)) {
                iconcachefileifany = ici.GetLargeIconFileFullPath(playlist.PlaylistNameField);
            } else {
                iconcachefileifany = null;
            }
            ArchiveProgressBar.Value = 0;
            ArchiveProgressBar.Minimum = 0;
            // Add common attributes...
            attributes.Add(new("Creator" , SystemInfo.UserName));
            attributes.Add(new("CreationTime" , SystemInfo.Now.ToString("G")));
            attributes.Add(new("Comment" , null));
            attributes.Add(new("Name" , playlist.PlaylistName));
            attributes.Add(new("CompressionMethod" , "Store"));
        }

        private void ThreadStartInfra()
        {
            Invoke(() => { UseWaitCursor = true; });
            CreatePlaylistArchiveInfrastracture.CurrentOperation += UpdateStatus;
            if (CreatePlaylistArchiveInfrastracture.IsValidCancellationToken == false) { goto g_exit; }
            List<IPlaylistFile> _files = new(CreatePlaylistArchiveInfrastracture.FindAvailableFiles(playlist));
            if (CreatePlaylistArchiveInfrastracture.IsValidCancellationToken == false) { goto g_exit; }
            CreatePlaylistArchiveInfrastracture.DetermineTags(playlist, _files);
            if (CreatePlaylistArchiveInfrastracture.IsValidCancellationToken == false) { goto g_exit; }
            CreatePlaylistArchiveInfrastracture.ArchiveFiles(playlist, _files, resultfile , attributes , iconcachefileifany);
        g_exit:
            CreatePlaylistArchiveInfrastracture.CurrentOperation -= UpdateStatus;
            try { Invoke(() => { UseWaitCursor = false; }); } catch { }
            UpdateStatusLabel("Terminating Transaction. Successfull operation performed.");
            System.Threading.Thread.Sleep(1000);
            Close();
        }

        private void UpdateStatus(System.Object send, CreatePlaylistArchiveInfrastracture.OperationEventArgs args)
        {
            switch (args.Operation)
            {
                case CreatePlaylistArchiveInfrastracture.Operation.ReportMaxProgress:
                    ArchiveProgressBar.Maximum = (args.CurrentProgress + 10).ToInt32();
                    ArchiveProgressBar.Value = 0;
                    ArchiveProgressBar.Minimum = 0;
                    break;
                case CreatePlaylistArchiveInfrastracture.Operation.Fail:
                    MusicPlayerHelper.ShowErrorMessage(args.OperationData);
                    Close();
                    break;
                default:
                    try { ArchiveProgressBar.Value = args.CurrentProgress.ToInt32(); } catch (ArgumentException) { }
                    UpdateStatusLabel(args.OperationData);
                    break;
            }
        }

        private void UpdateStatusLabel(System.String data)
        {
            const int LineCount = 10;
            try
            {
                StatusLabel.BeginInvoke(() =>
                {
                    if (label_count < LineCount)
                    {
                        System.Threading.Interlocked.Increment(ref label_count);
                    }
                    else
                    {
                        System.String temp = StatusLabel.Text;
                        System.Int32 idx = temp.IndexOf("\r\n");
                        StatusLabel.Text = temp.Substring(idx + 2);
                        temp = null;
                    }
                    StatusLabel.Text += data + "\r\n";
                });
            }
            catch { }
        }

        private void StartOpButton_Click(object sender, EventArgs e)
        {
            if (PathBox.Text.Length > 0) { resultfile = PathBox.Text; }
            if (System.String.IsNullOrWhiteSpace(resultfile))
            {
                MusicPlayerHelper.ShowErrorMessage($"You must supply a target path in order to save the {playlist.PlaylistName} playlist.");
                return;
            }
            if (resultfile.EndsWith(".aplaylist") == false)
            {
                resultfile += ".aplaylist";
            }
            if (System.IO.File.Exists(resultfile) && (MusicPlayerHelper.ShowQuestionMessage($"The file {resultfile} does already exist. Really overwrite the file?") == false))
            {
                return;
            }
            var th = new System.Threading.Thread(ThreadStartInfra)
            {
                Name = "[MP] Background worker thread archive creator"
            };
            th.TrySetApartmentState(System.Threading.ApartmentState.STA);
            th.Start();
            AdditonalAttributesButton.Visible = false;
            StartOpButton.Visible = false;
            GenericButton_1.Text = "Cancel...";
            PathBox.Visible = false;
            ArchiveProgressBar.Visible = true;
            StatusLabel.Text = null;
        }

        private void GenericButton_1_Click(object sender, EventArgs e)
        {
            switch (GenericButton_1.Text)
            {
                case "Cancel...":
                    CreatePlaylistArchiveInfrastracture.CancellationToken.Invalidate();
                    StatusLabel.Text = "Terminating...";
                    Text = "Cancelling...";
                    System.Threading.Thread.Sleep(500);
                    Close();
                    break;
                case "Browse...":
                    while (System.String.IsNullOrEmpty(resultfile))
                    {
                        Dialogs.SaveFileDialog SFD = new();
                        SFD.DefaultFilterExtension = ".aplaylist";
                        SFD.CheckPath = true;
                        SFD.AddFilter(new("*.aplaylist", "Archived playlist files"));
                        SFD.Title = Global.Resources.GetStringResource("ArchivedPlaylist_PathSelectTitle");
                        if (SFD.SpawnDialog(Handle))
                        {
                            resultfile = SFD.FilePaths[0];
                            PathBox.Text = resultfile;
                        }
                        else
                        {
                            MusicPlayerHelper.ShowErrorMessage("A result save path was not provided. Press 'OK' to retry.");
                        }
                        SFD = null;
                    }
                    break;
            }
        }

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (components is not null) { components.Dispose(); }
                attributes?.Clear();
                attributes = null;
                playlist = null; // Do not call Dispose, this will dispose the playlist in the RCU engine.
            }
            base.Dispose(disposing);
        }

        private void OPENADVFUNCBOX(object sender, EventArgs e)
        {
            PlaylistArchiveOptionsForm f = null;
            try {
                f = new(attributes);
                if (f.ShowDialog() == DialogResult.OK) 
                {
                    attributes.Clear();
                    foreach (var nd in f.NewAttributes)
                    {
                        attributes.Add(nd);
                    }
                }
            } catch { } finally {
                f?.Dispose();
                f = null;
            }
        }
    }
}
