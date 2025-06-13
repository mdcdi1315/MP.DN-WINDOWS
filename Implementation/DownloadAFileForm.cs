
using System;
using Microsoft.IO;
using MP.Threading;
using static MP.Settings;
using MP.ProcessLaunching;
using System.Windows.Forms;

namespace MP
{
    public partial class DownloadAFileForm : Form
    {
        private sealed class DownloadToken : ICancellationToken<System.Byte>
        {
            private System.Byte tv;

            public DownloadToken() => tv = 255;

            public byte InstanceId => tv;

            public void Invalidate() => tv = 0;

            public bool IsValid(byte unique) => tv == unique && unique == 255;
        }

        private DirectoryInfo downloaddir;
        private DownloadedFileInfo dfi;
        private DownloadToken dtk;

        public DownloadAFileForm(DirectoryInfo downloaddir)
        {
            if (downloaddir is null) { throw new ArgumentNullException(nameof(downloaddir)); }
            InitializeComponent();
            ModeSelectionBox.Items.Add(Global.Resources.GetStringResource("DownloadAFileForm_ModePrompt_O1"));
            ModeSelectionBox.Items.Add(Global.Resources.GetStringResource("DownloadAFileForm_ModePrompt_O2"));
            ModeSelectionBox.Items.Add(Global.Resources.GetStringResource("DownloadAFileForm_ModePrompt_O3"));
            this.downloaddir = downloaddir;
            this.downloaddir.Create();
            dfi = null;
            dtk = new();
            L1.Text = Global.Resources.GetStringResource("DownloadAFileForm_ModePrompt");
            L2.Text = Global.Resources.GetStringResource("DownloadAFileForm_UrlBoxPrompt");
            StatusText.Text = Global.Resources.GetStringResource("DownloadAFileForm_StatusText_DownloadNotStarted");
        }

        // DragDrop applies the given data to the form.
        private void DDF_DragDrop(object sender, DragEventArgs e)
        {
            URLBox.Text = e.Data?.GetData(typeof(System.String)) as System.String;
        }

        // DragOver checks for a Drag-n-Drop request , and if it is valid , it routes it.
        private void DDF_DragOver(object sender, DragEventArgs e)
        {
            System.String dt = e.Data?.GetData(typeof(System.String)) as System.String;
            dt ??= System.String.Empty;
            switch (ModeSelectionBox.SelectedIndex)
            {
                case 0:
                case 2:
                    if (dt.StartsWith("http://") || dt.StartsWith("https://"))
                    {
                        e.Effect = DragDropEffects.Copy;
                    }
                    else
                    {
                        e.Effect = DragDropEffects.None;
                    }
                    break;
                case 1:
                    e.Effect = DragDropEffects.Copy;
                    break;
            }
        }

        private void DDF_SelectionModeChanged(object sender, System.EventArgs e)
        {
            URLBox.Text = System.String.Empty;
        }

        private void EnsureInternetConnectionHandler()
        {
            try
            {
                MP.Networking.NetworkProvider.VerifyConnection();
            }
            catch (MP.ExceptionSystem.NativeWindowsException ex)
            {
                ShowErrorMessageWithRes("DownloadAFileForm_InitConnFailed" , ex.ErrorCode.ToString("x2") , ex.NativeMessage);
                Close();
                return;
            }
            FormClosing += DDF_F_CLOSING;
        }

        private void DDF_F_CLOSING(System.Object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (DialogResult == DialogResult.OK) { return; }
            if (DialogResult == DialogResult.Cancel && ShowQuestionMsgWithRes("DownloadAFileForm_CancelActionQuestion"))
            {
                if (dtk.IsValid(dtk.InstanceId)) { dtk.Invalidate(); }
                return;
            }
            e.Cancel = true;
        }

        private void DDF_F_LOAD(object sender, System.EventArgs e)
        {
            System.Threading.Thread tdm = new(EnsureInternetConnectionHandler);
            tdm.Priority = System.Threading.ThreadPriority.BelowNormal;
            tdm.Start();
        }

        private void CancelButton_Click(object sender, System.EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private static System.String GetExtensionFromMIMEType(System.String mimetype)
            => mimetype switch {
                "" or null => throw new System.ArgumentNullException(nameof(mimetype)),
                "audio/mpeg" => "mp3",
                "audio/mpeg4" => "m4a",
                "audio/flac" => "flac",
                "audio/wav" => "wav",
                "audio/aac" => "aac",
                "audio/ogg" => "ogg",
                _ => throw new System.NotSupportedException($"This MIME type is not supported: {mimetype}"),
            };

        private void SimpleFileDownload()
        {
            Networking.WinInetApplication app = null;
            if (dtk.IsValid(dtk.InstanceId) == false) { return; }
            DebugProvider.WriteLine("DownloadProvider: Initiating for simple file download.");
            Invoke(() => {
                StatusText.Text = Global.Resources.GetStringResource("DownloadAFileForm_Status_StartingText");
                DownloadProgressBar.Style = ProgressBarStyle.Marquee;
                DownloadProgressBar.MarqueeAnimationSpeed = 800;
                DownloadProgressBar.Show();
                ConfirmButton.Enabled = false;
            });
            Networking.GenericNetworkConnection conn = null;
            FileStream resultfile = null;
            System.IO.Stream networkstream = null;
            try {
                DebugProvider.WriteLine("DownloadProvider: Connecting to network...");
                app = new(Global.Resources.GetStringResource("DownloadAFileForm_UserAgentString"));
                if (dtk.IsValid(dtk.InstanceId) == false) { return; }
                conn = app.CreateNewGenericConnection(new() { URL = URLBox.Text });
                DebugProvider.WriteLine($"DownloadProvider: Final URL string determined as: {conn.Parameters.URL}");
                if (dtk.IsValid(dtk.InstanceId) == false) { return; }
                DebugProvider.WriteLine("DownloadProvider: Sending request...");
                conn.Submit();
                if (dtk.IsValid(dtk.InstanceId) == false) { return; }
                DebugProvider.WriteLine("DownloadProvider: Creating result file.");
                resultfile = downloaddir.CreateFileStream($"{SystemInfo.Now.Ticks:x2}.{GetExtensionFromMIMEType(conn.ContentType)}");
                DebugProvider.WriteLine("DownloadProvider: Creating request stream.");
                networkstream = conn.GetResultStream();
                if (dtk.IsValid(dtk.InstanceId) == false) { return; }
                Invoke(() => {
                    DownloadProgressBar.SuspendLayout();
                    DownloadProgressBar.Style = ProgressBarStyle.Continuous;
                    DownloadProgressBar.Step = 1;
                    DownloadProgressBar.Minimum = 0;
                    DownloadProgressBar.Value = 0;
                    DownloadProgressBar.Maximum = conn.ContentLength.ToInt32();
                    DownloadProgressBar.ResumeLayout(true);
                });
                if (dtk.IsValid(dtk.InstanceId) == false) { return; }
                System.Int32 rb, writtenbytes = 0;
                DebugProvider.WriteLine("DownloadProvider: Preparing for download...");
                System.Byte[] buffer = new System.Byte[4096];
                StatusText.Text = System.String.Format(Global.Resources.GetStringResource("DownloadAFileForm_Status_DownloadingText"), Path.GetFileName(resultfile.Name));
                DebugProvider.WriteLine("DownloadProvider: Reading from the network. 0 bytes are read so far.");
                while (dtk.IsValid(dtk.InstanceId) && (rb = networkstream.Read(buffer, 0, buffer.Length)) > 0)
                {
                    DebugProvider.WriteLine($"DownloadProvider: Reading from the network. {rb} bytes are read so far.");
                    resultfile.Write(buffer, 0, rb);
                    writtenbytes += rb;
                    Invoke(() => { DownloadProgressBar.Value = writtenbytes; });
                }
                DebugProvider.WriteLine("DownloadProvider: Download done!");
                dfi = new(new(resultfile.Name), URLBox.Text, DownloadedFileType.NormalDownload);
                DebugProvider.WriteLine("DownloadProvider: Cleaning up...");
            } catch (System.Exception ex) {
                DebugProvider.WriteLine("DownloadProvider: Operation failed.");
                Invoke(() => {
                    DownloadProgressBar.Hide();
                    StatusText.Text = Global.Resources.GetStringResource("DownloadAFileForm_Status_FailedText");
                    ConfirmButton.Visible = false;
                });
                switch (ex)
                {
                    case NotSupportedException:
                        ShowErrorMessageWithRes("DownloadAFileForm_DownloadFailed_NotSupported", ex.Message);
                        break;
                    case MP.ExceptionSystem.HttpFailureException ehttp:
                        ShowErrorMessageWithRes("DownloadAFileForm_HTTPReqFailure", ehttp.HttpErrorCode, ehttp.Message);
                        break;
                    case TimeoutException:
                        ShowErrorMessageWithRes("DownloadAFileForm_HTTPServerTimeoutFailure");
                        break;
                    default:
                        ShowErrorMessageWithRes("DownloadAFileForm_GenericError", ex);
                        break;
                }
                return;
            } finally {
                networkstream?.Dispose();
                resultfile?.Dispose();
                conn?.Dispose();
                app?.Dispose();
            }
            DebugProvider.WriteLine("DownloadProvider: Operation succeeded, exiting cleanly.");
            Invoke(() => {
                DownloadProgressBar.Hide();
                StatusText.Text = Global.Resources.GetStringResource("DownloadAFileForm_SuccessfullDownloadText");
                ConfirmButton.Enabled = true;
                ConfirmButton.Text = "Exit...";
            });
        }

        private void YouTubeDownload() 
        {
            DebugProvider.WriteLine("DownloadProvider: Initiating for Yt-Dlp download...");
            Global.YtDlpInstallationDirectory.Refresh();
            if (Global.YtDlpInstallationDirectory.Exists == false ||
                Global.YtDlpInstallationDirectory.GetFile("VerifierData.json") is null)
            {
                DebugProvider.WriteLine("DownloadProvider: The Yt-Dlp layout is invalid, requesting Yt-Dlp component to be downloaded.");
                if (Global.YtDlpInstallationDirectory.Exists)
                {
                    UseWaitCursor = true;
                    Global.YtDlpInstallationDirectory.Delete(true);
                    UseWaitCursor = false;
                }
                DependenciesDownloadForm ddf = null;
                try {
                    ddf = new(Global.BaseDataDirectory.GetFileFullPath("YtDlpSources.json"), Global.YtDlpInstallationDirectory.FullName);
                    DialogResult dg = ddf.ShowDialog(this);
                    if (dg != DialogResult.OK)
                    {
                        DebugProvider.WriteLine("DownloadProvider: Cannot download Yt-Dlp, failing due to missing feature...");
                        return;
                    }
                } finally {
                    ddf?.Dispose();
                    ddf = null;
                }
                DebugProvider.WriteLine("DownloadProvider: Yt-Dlp now exists, file download can now continue.");
            }
            Invoke(() => {
                StatusText.Text = Global.Resources.GetStringResource("DownloadAFileForm_Status_StartingText"); ;
                DownloadProgressBar.Style = ProgressBarStyle.Marquee;
                DownloadProgressBar.MarqueeAnimationSpeed = 800;
                DownloadProgressBar.Show();
                ConfirmButton.Enabled = false;
            });
            FileStream fsm = null;
            PreCompiledProcessLauncher ppl = null;
            System.String tfn = SystemInfo.Now.Ticks.ToString("x2");
            try {
                DebugProvider.WriteLine("DownloadProvider: Opening command-line definition file...");
                fsm = Global.BaseDataDirectory.OpenReadFileStream("YtDlpCommandLine.json");
                ppl = new(fsm);
                ppl.CreationFlags = ProcessCreationFlags.CreateNewProcessGroup;
#if !DEBUG
                ppl.CreationFlags |= ProcessCreationFlags.CreateWithNewConsole;
#endif
                System.String urlf = ModeSelectionBox.SelectedIndex == 1 ? $"https://youtube.com/watch?v={URLBox.Text}" : URLBox.Text;
                DebugProvider.WriteLine($"DownloadProvider: URL determined to be downloaded is: {urlf}");
                DebugProvider.WriteLine("DownloadProvider: Filling environment block apporpriately.");
                ppl.EnvironmentBlock.Add("URL", urlf);
                ppl.EnvironmentBlock.Add("BASEDIR", Global.YtDlpInstallationDirectory.FullName);
                ppl.EnvironmentBlock.Add("OUTFILENAME" , tfn);
                DebugProvider.WriteLine("DownloadProvider: Preparing for Yt-Dlp launch.");
                ppl.ApplyVariableTransformations();
                Invoke(() => { StatusText.Text = Global.Resources.GetStringResource("DownloadAFileForm_Status_YtDlpInvokedText"); });
                DebugProvider.WriteLine("DownloadProvider: Yt-Dlp was launched.");
                ppl.LaunchAndWaitToFinish();
                if (ppl.ExitCode != 0)
                {
                    Invoke(() => {
                        DownloadProgressBar.Hide();
                        StatusText.Text = Global.Resources.GetStringResource("DownloadAFileForm_Status_FailedText");
                        ConfirmButton.Visible = false;
                    });
                    ShowErrorMessageWithRes("DownloadAFileForm_Error_YtDlpExitCodeInvalid", ppl.ExitCode);
                    DebugProvider.WriteLine("DownloadProvider: Invalid exit code returned back from Yt-Dlp , exiting.");
                    return;
                }
                DebugProvider.WriteLine("DownloadProvider: Download complete!");
                Invoke(() => { StatusText.Text = Global.Resources.GetStringResource("DownloadAFileForm_Status_FinalizingYtDlpState"); });
                DebugProvider.WriteLine("DownloadProvider: Creating downloaded file and returning back...");
                System.String fp = Path.Join(Global.DownloadedTracksDirectory.FullName, $"{tfn}.m4a");
                File.Move(Path.Join(Global.YtDlpInstallationDirectory.FullName, $"{tfn}.m4a"), fp);
                dfi = new(new(fp), urlf, DownloadedFileType.Youtube);
                DebugProvider.WriteLine("DownloadProvider: Cleaning up..");
            } catch (Exception ex) {
                Invoke(() => {
                    DownloadProgressBar.Hide();
                    StatusText.Text = Global.Resources.GetStringResource("DownloadAFileForm_Status_FailedText");
                    ConfirmButton.Visible = false;
                });
                ShowErrorMessageWithRes("DownloadAFileForm_Error_YtDlpInitFailed", ex);
                try {
                    // Attempt to delete any pre-generated data. Yt-Dlp can produce working data that are now invalid.
                    foreach (FileInfo fi in Global.YtDlpInstallationDirectory.GetFiles($"{tfn}*"))
                    {
                        fi.Delete();
                    }
                } catch {}
                return;
            } finally {
                ppl?.Dispose();
                ppl = null;
                fsm?.Dispose();
                fsm = null;
            }
            Invoke(() => {
                DownloadProgressBar.Hide();
                StatusText.Text = Global.Resources.GetStringResource("DownloadAFileForm_SuccessfullDownloadText");
                ConfirmButton.Enabled = true;
                ConfirmButton.Text = "Exit...";
            });
            DebugProvider.WriteLine("DownloadProvider: Exiting cleanly.");
        }

        private void ConfirmButton_Click(object sender, System.EventArgs e)
        {
            if (ConfirmButton.Text == "Confirm...") {
                if (ModeSelectionBox.SelectedIndex == -1)
                {
                    ShowErrorMessageWithRes("DownloadAFileForm_ModeNotSelected");
                    return;
                }
                if (System.String.IsNullOrWhiteSpace(URLBox.Text))
                {
                    ShowErrorMessageWithRes("DownloadAFileForm_URLInvalid_1");
                    return;
                }
                if ((ModeSelectionBox.SelectedIndex == 0 || ModeSelectionBox.SelectedIndex == 2) && 
                    (URLBox.Text.StartsWith("https://") || URLBox.Text.StartsWith("http://")) == false)
                {
                    ShowErrorMessageWithRes("DownloadAFileForm_URLInvalid_0", URLBox.Text);
                    return;
                }
                System.Threading.Thread tdm = new(ModeSelectionBox.SelectedIndex switch {
                    0 => SimpleFileDownload,
                    1 or 2 => YouTubeDownload,
                    _ => null,
                });
                tdm.Priority = System.Threading.ThreadPriority.BelowNormal;
                tdm.Name = Global.Resources.GetStringResource("DownloadAFileForm_ThreadWorkerName");
                tdm.Start();
            } else if (ConfirmButton.Text == "Exit...") {
                DialogResult = DialogResult.OK;
                Close();
            }
        }

        private void ShowErrorMessageWithRes(System.String resource , params System.Object[] fmt)
        {
            System.String resfmt = System.String.Format(Global.Resources.GetStringResource(resource) , args: fmt);
            using (MP.Dialogs.NewGenMessageBox mb = new())
            {
                mb.Text = resfmt;
                mb.Title = "Error";
                mb.Buttons = Dialogs.ButtonSelection.OK;
                mb.SelectedIcon = Dialogs.IconSelection.Error;
                mb.ShowDialog(this);
            }
        }

        private System.Boolean ShowQuestionMsgWithRes(System.String resource , params System.Object[] fmt)
        {
            System.String resfmt = System.String.Format(Global.Resources.GetStringResource(resource), args: fmt);
            using (MP.Dialogs.NewGenMessageBox mb = new())
            {
                mb.Text = resfmt;
                mb.Title = "Question";
                mb.Buttons = Dialogs.ButtonSelection.YesNo;
                mb.SelectedIcon = Dialogs.IconSelection.Question;
                mb.ShowDialog(this);
                return mb.ReturnedButton == Dialogs.ButtonReturned.Yes;
            }
        }

        public DownloadedFileInfo DownloadedFile => dfi;
    }
}
