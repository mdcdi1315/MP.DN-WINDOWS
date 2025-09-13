using System;
using Microsoft.IO;
using static MP.Settings;
using System.Windows.Forms;

namespace MP
{
    internal partial class ExportPlaylistFromArchiveForm : Form
    {
        private System.Boolean cannotclose, canceled;
        private ArchivedTrackPlaylist playlist;

        public ExportPlaylistFromArchiveForm(ArchivedTrackPlaylist plt)
        {
            playlist = plt;
            canceled = false;
            cannotclose = false;
            InitializeComponent();
        }

        private void ChangeMessageText(System.String msg) => Invoke(() => ProgressInformationLabel.Text = msg);

        private void ExporterThreadCode()
        {
            ChangeMessageText("Starting operation...");
            cannotclose = true;
            StartCancelOpButton.Text = "Cancel...";
            System.Threading.Thread.Sleep(600);
            if (canceled) { Close(); return; }
            TrackPlayList tplt = null;
            MusicPlayerStreamV2 ts2 = null;
            FileInfo targetfile;
            FileStream tempstream = null;
            DirectoryInfo targetdir;
            try {
                ChangeMessageText("Extracting audio files...");
                System.Threading.Thread.Sleep(600);
                targetdir = new(Path.Join(Global.ExportedPlaylistTracksDirectory.FullName , playlist.PlaylistName));
                if (targetdir.Exists) {
                    targetdir = new(Path.Join(Global.ExportedPlaylistTracksDirectory.FullName, $"{playlist.PlaylistName}-{SystemInfo.Now.Ticks:x2}"));
                }
                targetdir.Create();
                targetfile = new(Path.Join(Global.PlaylistsDirectory.FullName, $"{playlist.PlaylistName}.mpbpl"));
                if (targetfile.Exists) {
                    targetfile = new(Path.Join(Global.PlaylistsDirectory.FullName, $"{playlist.PlaylistName}-{SystemInfo.Now.Ticks:x2}.mpbpl"));
                }
                Invoke(() => OperationProgressBar.Maximum = playlist.TracksContained.Count);
                if (playlist.Attributes is null)
                {
                    foreach (var file in playlist.TracksContained)
                    {
                        ChangeMessageText($"Extracting {file.Name}...\nExtracted {OperationProgressBar.Value} files out of {playlist.TracksContained.Count} files.");
                        tempstream = targetdir.CreateFileStream(file.Name);
                        ts2 = playlist.GetStream(file);
                        ts2.DirectCopyToStream(tempstream);
                        tempstream.Dispose();
                        tempstream = null;
                        Invoke(OperationProgressBar.PerformStep);
                        if (canceled) { goto g_canceled; }
                    }
                } else {
                    foreach (var file in playlist.ActualArchiveStreams) 
                    {
                        System.String fpname = Path.GetFileName(file.Entry.EntryPath);
                        try {
                            ChangeMessageText($"Extracting {fpname}...\nExtracted {OperationProgressBar.Value} files out of {playlist.TracksContained.Count} files.");
                            tempstream = targetdir.CreateFileStream(fpname);
                            file.EntryStream.DirectCopyToStream(tempstream);
                            tempstream.Dispose();
                            tempstream = null;
                        } finally {
                            file?.Dispose();
                        }
                        Invoke(OperationProgressBar.PerformStep);
                        if (canceled) { goto g_canceled; }
                    }
                }
                ChangeMessageText("Creating playlist...");
                System.Threading.Thread.Sleep(600);
                Invoke(OperationProgressBar.Hide);
                tplt = new(targetdir);
                tplt.Preferences.Clear();
                if (canceled) { goto g_canceled; }
                for (System.Int32 I = 0; I < playlist.Preferences.Count; I++) 
                {
                    tplt.Preferences.Add(playlist.Preferences[I]);
                }
                tplt.PlaylistName = targetfile.GetNameOnly();
                if (playlist.PlaylistImageData is not null)
                {
                    ChangeMessageText("Saving playlist cover image...");
                    FileStream fsm = null;
                    try {
                        playlist.PlaylistImageData.Position = 0;
                        System.String sp = Path.Join(targetdir.FullName, "PlaylistCover.img");
                        fsm = new(sp, FileMode.Create);
                        playlist.PlaylistImageData.DirectCopyToStream(fsm);
                        tplt.Preferences.Update("PlaylistCoverPath" , sp);
                        fsm.Flush();
                    } catch {
                        ChangeMessageText("Cannot save cover image , proceeding with the save...");
                    } finally {
                        fsm?.Dispose();
                    }
                }
                ChangeMessageText("Saving playlist...");
                if (canceled) { goto g_canceled; }
                tempstream = targetfile.Create();
                if (canceled) { goto g_canceled; }
                tplt.SaveAsPlaylist(tempstream);
                tempstream.Dispose();
                tempstream = null;
                ChangeMessageText("Updating States...");
                UpdateCacheEntries(tplt.PlaylistName , targetdir.Name);
                ChangeMessageText("Done!!!");
                Invoke(StartCancelOpButton.Hide);
                System.Threading.Thread.Sleep(1200);
                cannotclose = false;
                Invoke(Close);
            } catch (Exception ex) {
                Dialogs.NativeMessageBox.Show($"Operation failed: \n{ex}" , "Error" , Dialogs.ButtonSelection.OK , Dialogs.IconSelection.Error);
                cannotclose = false;
                Invoke(Close);
                return;
            } finally {
                ts2?.Dispose();
                ts2 = null;
                tempstream?.Dispose();
                tempstream = null;
                tplt?.Dispose();
                tplt = null;
            }
            return;
        g_canceled:
            try {
                targetdir.Delete(true);
                targetfile.Delete();
            } catch { }
            Invoke(Close);
        }

        private static void UpdateCacheEntries(System.String pln , System.String foldername)
        {
            Caches.ExportedPlaylistsCache.ExportedPlaylistCacheInstance inst = new();
            inst.LoadCacheFromExportedDirectory(Global.ExportedPlaylistTracksDirectory);
            inst.AddPlaylistNameAndFolder(pln , foldername);
            inst.SaveCacheToExportedDirectory(Global.ExportedPlaylistTracksDirectory);
            inst.ClearCacheEntries();
            inst = null;
        }

        private void STARTCANCELOP(object sender, EventArgs e)
        {
            if (StartCancelOpButton.Text == "Start...")
            {
                var tdm = new System.Threading.Thread(ExporterThreadCode);
                tdm.Name = "[MP] Export Worker";
                tdm.IsBackground = true;
                tdm.Start();
            } else {
                ProgressInformationLabel.Text = "Cancelling...";
                canceled = true;
            }
        }

        private void F_CLOSING(object sender, FormClosingEventArgs e)
        {
            e.Cancel = cannotclose;
        }

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (components is not null)
                {
                    components.Dispose();
                    components = null;
                }
                playlist = null;
            }
            base.Dispose(disposing);
        }
    }
}
