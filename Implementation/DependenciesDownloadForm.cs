
using Microsoft.IO;
using MP.Threading;
using static MP.Settings;
using MP.DownloadableDeps;
using System.Windows.Forms;

namespace MP
{
    public sealed partial class DependenciesDownloadForm : Form
    {
        private Downloader dinstance;
        private DownloaderToken token;
        private System.Int32 depboxidx;
        private System.String ijf, resultdir;
        private System.Threading.Thread worker;

        private sealed class DownloaderToken : ICancellationToken<System.Byte>
        {
            private System.Byte iid;

            public DownloaderToken() => iid = 100;

            public byte InstanceId => iid;

            public void Invalidate() => iid = 0;

            public bool IsValid(byte unique) => unique == 100 && iid == unique;
        }

        public DependenciesDownloadForm(System.String infojsonfile, System.String resultsdir)
        {
            if (File.Exists(infojsonfile) == false)
            {
                ShowErrorMessageWithRes("DependenciesDownloadForm_DownloadInfoJsonNotFoundText" , infojsonfile);
                DialogResult = DialogResult.Cancel;
                return;
            }
            InitializeComponent();
            ijf = infojsonfile;
            worker = null;
            resultdir = resultsdir;
        }

        private void F_LOAD(object sender, System.EventArgs e)
        {
            DownloadableDepConfigReader rdr = null;
            System.IO.FileStream fsm = null;
            DebugProvider.WriteLine($"DependencyManager: Initiating dependency installer. Installer file: {ijf}.");
            try {
                fsm = new(ijf, System.IO.FileMode.Open);
                rdr = new(fsm);
                dinstance = new(rdr.MainDependency, rdr.SubDependencies);
                dinstance.ResultDirectory = new(resultdir);
            } catch (System.Exception ex) {
                DebugProvider.WriteLine("DependencyManager: Cannot create dependency tree due to an underlying exception. Exiting.");
                ShowErrorMessageWithRes("DependenciesDownloadForm_UnderlyingErrorText" , ex);
                Close();
                return;
            } finally {
                rdr?.Dispose();
                fsm?.Dispose();
            }
            DebugProvider.WriteLine("DependencyManager: Creating instance...");
            token = new();
            dinstance.RequestInitiated += Downloader_RequestInitiated;
            dinstance.Succeeded += Downloader_ItemSucceeded;
            dinstance.Progress += Downloader_ItemProgress;
            dinstance.Failed += Downloader_ItemFailed;
            dinstance.ExtractingFiles += Downloader_ExtractingFiles;

            foreach (var dep in dinstance.DependenciesRequired)
            {
                DepBox.Items.Add($"{dep.Name} - {dep.Description}");
            }
        }

        private void F_CLOSING(object sender, FormClosingEventArgs e)
        {
            if (DialogResult == DialogResult.OK) { return; }
            System.Boolean abort = ShowQuestionMsgWithRes("DependenciesDownloadForm_AbortQuestionText");
            e.Cancel = abort == false;
            if (abort) {
                token?.Invalidate();
                worker?.Join();
                worker = null;
                DialogResult = DialogResult.Cancel;
            }
        }

        private void CancelButton_Click(object sender, System.EventArgs e)
        {
            if (CButton.Text == "Exit...") {
                DialogResult = DialogResult.OK;
                return; 
            }
            Close();
        }

        private void ThreadWorkerCode()
        {
            Networking.WinInetApplication app = null;
            try {
                DebugProvider.WriteLine("DependencyManager: Opening network connection...");
                app = new(Global.Resources.GetStringResource("DependenciesDownloadForm_UserAgentString"));
                depboxidx = 0;
                DialogResult = DialogResult.None;
                DebugProvider.WriteLine("DependencyManager: Yielding execution to download engine.");
                dinstance.ProcessAll(app, Global.TempDirectory, token);
                DebugProvider.WriteLine("DependencyManager: Download successfull.");
                WriteVerificationData();
                DebugProvider.WriteLine("DependencyManager: Cleaning up...");
                if (token.IsValid(token.InstanceId))
                {
                    StatusText.Text = Global.Resources.GetStringResource("DependenciesDownloadForm_StatusFinishedText");
                    CButton.Text = "Exit...";
                    token.Invalidate();
                    DialogResult = DialogResult.OK;
                    DebugProvider.WriteLine("DependencyManager: Exiting cleanly.");
                }
            } catch (System.Exception e) {
                DebugProvider.WriteLine("DependencyManager: Operation failed.");
                ShowErrorMessageWithRes("DependenciesDownloadForm_UnderlyingErrorText", e);
                Close();
                return;
            } finally {
                app.Dispose();
            }
        }

        private void WriteVerificationData()
        {
            if (token.IsValid(token.InstanceId) == false) { return; }
            System.IO.Stream fsm = null;
            FileStream rt = null;
            DownloadInstallationVerifierFileWriter fw = null;
            StatusText.Text = Global.Resources.GetStringResource("DependenciesDownloadForm_WritingVerDataText");
            try {
                DebugProvider.WriteLine("DependencyManager: Writing installation verification data...");
                fsm = new MemoryStream();
                fw = new(fsm);
                fw.WriteFromDirectoryAndDeps(dinstance.ResultDirectory, dinstance.DependenciesRequired);
                fw.Dispose();
                rt = dinstance.ResultDirectory.CreateFileStream("VerifierData.json");
                fsm.Position = 0;
                rt.Position = 0;
                DebugProvider.WriteLine("DependencyManager: Finalizing verification data...");
                fsm.CopyTo(rt);
            } catch (System.Exception ex) {
                ShowErrorMessageWithRes("DependenciesDownloadForm_WritingVerDataFailedText" , ex);
                StatusText.Text = Global.Resources.GetStringResource("DependenciesDownloadForm_WritingVerDataFailedText_st");
                token.Invalidate();
            } finally {
                rt?.Dispose();
                rt = null;
                fw?.Dispose();
                fw = null;
                fsm?.Dispose();
                fsm = null;
            }
        }

        private void Downloader_ExtractingFiles(int newprogress)
        {
            DebugProvider.WriteLine($"DependencyManager: Download engine extracts files from a downloaded .zip file. It has extracted {newprogress} files.");
            StatusText.Text = $"Extracting files... {newprogress} files extracted.";
        }

        private void Downloader_ItemFailed(System.Exception reason)
        {
            DebugProvider.WriteLine("DependencyManager: Download engine reported a failure.");
            ShowErrorMessageWithRes("DependenciesDownloadForm_ItemDownloadFailed" , reason);
            Invoke(Close);
            return;
        }

        private void Downloader_ItemProgress(byte newprogress)
        {
            DebugProvider.WriteLine($"DependencyManager: Download engine has downloaded {newprogress}% of the data.");
            StatusText.Text = $"Downloaded {newprogress}% ...";
            System.Threading.Thread.Sleep(12);
        }

        private void Downloader_ItemSucceeded(string component)
        {
            DebugProvider.WriteLine($"DependencyManager: Download engine reported that the component named as {component} succeeded.");
            StatusText.Text = System.String.Format(Global.Resources.GetStringResource("DependenciesDownloadForm_ItemDownloadSucceeded") , component);
            DepBox.Items[depboxidx] = $"[Completed] {DepBox.Items[depboxidx]}";
            depboxidx++;
            if (depboxidx < DepBox.Items.Count) { DepBox.SelectedIndex = depboxidx; }
            System.Threading.Thread.Sleep(130);
        }

        private void Downloader_RequestInitiated(string component)
        {
            L1.Text = Global.Resources.GetStringResource("DependenciesDownloadForm_DownloadingDepsText");
            DebugProvider.WriteLine($"DependencyManager: Download engine reported that is downloading a component named as: {component}");
            StatusText.Text = System.String.Format(Global.Resources.GetStringResource("DependenciesDownloadForm_RequestSubmittedText") , component);
            if (depboxidx < DepBox.Items.Count) { DepBox.SelectedIndex = depboxidx; }
        }

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            if (dinstance is not null)
            {
                dinstance.RequestInitiated -= Downloader_RequestInitiated;
                dinstance.Succeeded -= Downloader_ItemSucceeded;
                dinstance.Progress -= Downloader_ItemProgress;
                dinstance.Failed -= Downloader_ItemFailed;
                dinstance.ExtractingFiles -= Downloader_ExtractingFiles;
                dinstance = null;
            }
            worker?.Join();
            worker = null;
            token = null;
            base.Dispose(disposing);
        }

        private void ShowErrorMessageWithRes(System.String resource, params System.Object[] fmt)
        {
            System.String resfmt = System.String.Format(Global.Resources.GetStringResource(resource), args: fmt);
            using (MP.Dialogs.NewGenMessageBox mb = new())
            {
                mb.Text = resfmt;
                mb.Title = "Error";
                mb.Buttons = Dialogs.ButtonSelection.OK;
                mb.SelectedIcon = Dialogs.IconSelection.Error;
                mb.ShowDialog(this);
            }
        }

        private System.Boolean ShowQuestionMsgWithRes(System.String resource, params System.Object[] fmt)
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

        private void StartButton_Click(object sender, System.EventArgs e)
        {
            if (worker is not null) { return; }
            StartButton.Hide();
            worker = new(ThreadWorkerCode);
            worker.TrySetApartmentState(System.Threading.ApartmentState.STA);
            worker.Priority = System.Threading.ThreadPriority.BelowNormal;
            worker.IsBackground = true;
            worker.Start();
        }
    }
}
