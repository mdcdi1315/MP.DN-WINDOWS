
using System;
using MP.Utilities;
using Microsoft.IO;
using MP.Threading;
using System.Windows.Forms;

namespace MP.Dialogs
{
    /// <summary>
    /// Defines a simple dialog for making file operations while displaying what is happening on a window.
    /// </summary>
    public sealed class FileOperationDialog : IDisposable , IWin32Window
    {
        private sealed class FileOperationThreadToken : ICancellationToken<System.Byte>
        {
            private System.Byte iid;

            public FileOperationThreadToken() => iid = 100;

            public byte InstanceId => iid;

            public void Invalidate() => iid = 0;

            public bool IsValid(byte unique) => iid == unique && iid == 100;
        }

        private enum FileOperationState : System.Byte
        {
            Unstarted,
            Working,
            Finished,
            Cancelled,
            Fault
        }

        private Button CButton;
        private Form dialogform;
        private Exception except;
        private FileOperationType type;
        private FileOperationState state;
        private Label OperationDetailsLabel;
        private Label GenericOperationLabel;
        private FileOperationThreadToken token;
        private System.String source, destination;
        private ProgressBar OperationGeneralBar;
        private ProgressBar CurrentFileProgressBar;
        private System.Threading.Thread operationthread;

        public FileOperationDialog(FileOperationType type , System.String src , System.String dest)
        {
            dialogform = new();
            token = new();
            this.type = type;
            state = FileOperationState.Unstarted;
            operationthread = null;
            source = src;
            destination = dest;
            InitializeComponent();
        }

        public System.Drawing.Color ForegroundColor
        {
            get => dialogform.ForeColor;
            set {
                dialogform.ForeColor = value;
                CButton.ForeColor = value;
                OperationGeneralBar.ForeColor = value;
                OperationDetailsLabel.ForeColor = value;
                GenericOperationLabel.ForeColor = value;
                CurrentFileProgressBar.ForeColor = value;
            }
        }

        public System.Drawing.Color BackgroundColor
        {
            get => dialogform.BackColor;
            set {
                dialogform.BackColor = value;
                CButton.BackColor = value;
                OperationGeneralBar.BackColor = value;
                OperationDetailsLabel.BackColor = value;
                GenericOperationLabel.BackColor = value;
                CurrentFileProgressBar.BackColor = value;
            }
        }

        public FileOperationType SelectedType => type;

        public System.IntPtr Handle => dialogform is null ? System.IntPtr.Zero : dialogform.Handle;

        public System.Boolean HasSuccessfullyCompleted => state == FileOperationState.Finished;

        private void InitializeComponent()
        {
            CButton = new Button();
            OperationGeneralBar = new ProgressBar();
            CurrentFileProgressBar = new ProgressBar();
            GenericOperationLabel = new Label();
            OperationDetailsLabel = new Label();
            dialogform.SuspendLayout();
            // 
            // CButton
            // 
            CButton.FlatAppearance.BorderColor = System.Drawing.Color.Black;
            CButton.FlatAppearance.BorderSize = 2;
            CButton.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Lavender;
            CButton.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Crimson;
            CButton.FlatStyle = FlatStyle.Flat;
            CButton.Location = new System.Drawing.Point(567, 210);
            CButton.Name = InternalResources.MP_DIALOGS_FOP_CBUTTON_NAME;
            CButton.Size = new System.Drawing.Size(75, 32);
            CButton.TabIndex = 0;
            CButton.Text = InternalResources.MP_DIALOGS_FOP_CBUTTON_TEXT;
            CButton.UseVisualStyleBackColor = true;
            CButton.Click += CancelActionButton_Click;
            // 
            // OperationGeneralBar
            // 
            OperationGeneralBar.Location = new System.Drawing.Point(12, 210);
            OperationGeneralBar.Name = InternalResources.MP_DIALOGS_FOP_OPERATIONGENERALBAR_NAME;
            OperationGeneralBar.Size = new System.Drawing.Size(529, 32);
            OperationGeneralBar.Step = 1;
            OperationGeneralBar.Style = ProgressBarStyle.Continuous;
            OperationGeneralBar.TabIndex = 1;
            // 
            // CurrentFileProgressBar
            // 
            CurrentFileProgressBar.Location = new System.Drawing.Point(12, 138);
            CurrentFileProgressBar.Name = InternalResources.MP_DIALOGS_FOP_CURRENTFILEPROGRESSBAR_NAME;
            CurrentFileProgressBar.Size = new System.Drawing.Size(617, 28);
            CurrentFileProgressBar.Step = 1;
            CurrentFileProgressBar.Style = ProgressBarStyle.Continuous;
            CurrentFileProgressBar.TabIndex = 2;
            // 
            // GenericOperationLabel
            // 
            GenericOperationLabel.AutoSize = true;
            GenericOperationLabel.Location = new System.Drawing.Point(12, 172);
            GenericOperationLabel.Name = InternalResources.MP_DIALOGS_FOP_GENERICOPERATIONLABEL_NAME;
            GenericOperationLabel.Size = new System.Drawing.Size(0, 15);
            GenericOperationLabel.TabIndex = 3;
            // 
            // OperationDetailsLabel
            // 
            OperationDetailsLabel.AutoSize = true;
            OperationDetailsLabel.Location = new System.Drawing.Point(12, 9);
            OperationDetailsLabel.Name = InternalResources.MP_DIALOGS_FOP_OPERATIONDETAILSLABEL_NAME;
            OperationDetailsLabel.Size = new System.Drawing.Size(0, 15);
            OperationDetailsLabel.TabIndex = 4;
            // 
            // FileOperationDialog
            // 
            dialogform.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            dialogform.AutoScaleMode = AutoScaleMode.Font;
            dialogform.ClientSize = new System.Drawing.Size(654, 256);
            dialogform.Controls.Add(OperationDetailsLabel);
            dialogform.Controls.Add(GenericOperationLabel);
            dialogform.Controls.Add(CurrentFileProgressBar);
            dialogform.Controls.Add(OperationGeneralBar);
            dialogform.Controls.Add(CButton);
            dialogform.FormBorderStyle = FormBorderStyle.FixedDialog;
            dialogform.MaximizeBox = false;
            dialogform.MinimizeBox = false;
            dialogform.Name = InternalResources.MP_DIALOGS_FOPFORMNAME;
            dialogform.ShowIcon = false;
            dialogform.StartPosition = FormStartPosition.CenterParent;
            dialogform.Text = InternalResources.MP_DIALOGS_FOPFORMTITLE;
            dialogform.FormClosing += F_DIALOG_CLOSING;
            dialogform.ResumeLayout(false);
            dialogform.PerformLayout();
        }

        private void F_DIALOG_CLOSING(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.WindowsShutDown) {
                state = FileOperationState.Cancelled;
                token.Invalidate();
                return; 
            }
            if (state == FileOperationState.Working)
            {
                using (NewGenMessageBox msg = new())
                {
                    msg.Title = InternalResources.MP_DIALOGS_FOP_CANCEL_ACTION_TITLE;
                    msg.Text = InternalResources.MP_DIALOGS_FOP_CANCEL_ACTION_TEXT;
                    msg.SelectedIcon = IconSelection.Question;
                    msg.Buttons = ButtonSelection.YesNo;
                    msg.ShowDialog(dialogform.Handle);
                    if (msg.ReturnedButton == ButtonReturned.Yes) { 
                        token.Invalidate();
                        ChangeDetailStatusText("Cancelling...");
                        state = FileOperationState.Cancelled;
                        return; 
                    } else {
                        e.Cancel = true;
                        return;
                    }
                }
            } else if (state == FileOperationState.Fault) {
                token.Invalidate();
                NewGenMessageBox msg = null;
                try {
                    msg = new();
                    msg.Title = "Error";
                    msg.Text = $"Operation failed due to an error. Error details: \n{except}";
                    msg.SelectedIcon = IconSelection.Error;
                    msg.ShowDialog(this);
                } catch {

                } finally {
                    msg?.Dispose();
                    msg = null;
                }
                return;
            } else if (state == FileOperationState.Finished) {
                return;
            }
            token.Invalidate();
            state = FileOperationState.Cancelled;
            operationthread?.Join();
            operationthread = null;
        }

        private void CancelActionButton_Click(object sender, EventArgs e)
        {
            dialogform.Close(); // Stuck here to wait for the dialog form to close.
        }

        private void ChangeGenericStatusText(System.String text)
        {
            if (state == FileOperationState.Working)
            {
                GenericOperationLabel.Invoke((System.String txt) => GenericOperationLabel.Text = txt , text);
            }
        }

        private void ChangeDetailStatusText(System.String text)
        {
            if (state == FileOperationState.Working)
            {
                OperationDetailsLabel.Invoke((System.String txt) => OperationDetailsLabel.Text = txt, text);
            }
        }

        private void UpdateFileProgressTo(System.Byte percentage)
        {
            if (state == FileOperationState.Working)
            {
                CurrentFileProgressBar.Invoke((System.Byte cp) => { CurrentFileProgressBar.Value = cp; } , percentage);
            }
        }

        private void UpdateGeneralProgressTo(System.Byte percentage)
        {
            if (state == FileOperationState.Working)
            {
                OperationGeneralBar.Invoke((System.Byte cp) => { OperationGeneralBar.Value = cp; }, percentage);
            }
        }

        private void OnOperationFailed()
        {
            ChangeDetailStatusText("Cleaning environment. Please be patient.");
            switch (type)
            {
                case FileOperationType.MoveFile:
                case FileOperationType.CopyFile:
                    File.Delete(destination);
                    break;
                case FileOperationType.CopyDirectory:
                case FileOperationType.MoveDirectory:
                    Directory.Delete(destination, true);
                    break;
            }
            System.Threading.Thread.Sleep(2000);
        }

        private void OPT_CopyMoveFile()
        {
            if (token.IsValid(token.InstanceId) == false) { return; }
            FileStream fss = null , fst = null;
            OperationGeneralBar.Invoke(OperationGeneralBar.Hide);
            try {
                fss = new(source, FileMode.Open, FileAccess.Read, FileShare.Read);
                fst = new(destination, FileMode.Create, FileAccess.Write, FileShare.None);
                System.Byte[] bufferscratch = new System.Byte[4096];
                System.Int32 rb;
                System.Byte p;
                while (token.IsValid(token.InstanceId) && (rb = fss.Read(bufferscratch, 0, bufferscratch.Length)) > 0)
                {
                    fst.Write(bufferscratch, 0, rb);
                    p = PercentageHelpers.GetPercentage(fss.Position, fss.Length);
                    switch (type)
                    {
                        case FileOperationType.MoveFile:
                            ChangeDetailStatusText($"Moving file...\n{p}% of the file data were moved.");
                            break;
                        case FileOperationType.CopyFile:
                            ChangeDetailStatusText($"Copying file...\n{p}% of the file data were copied.");
                            break;
                    }
                    UpdateFileProgressTo(p);
                }
                ChangeDetailStatusText("Finalizing file...");
            } finally {
                fss?.Dispose();
                fss = null;
                fst?.Dispose();
                fst = null;
            }
        }

        private void OPT_MoveCopyDirectories()
        {
            DirectoryInfo disource = new(source), ditarget = new(destination);
            ChangeDetailStatusText("Creating target directory.");
            ditarget.Create();
            System.Int32 commonsrcnamelen = disource.FullName.Length;
            switch (type)
            {
                case FileOperationType.MoveDirectory:
                    ChangeDetailStatusText($"Moving directory {disource.Name} to {ditarget.Name}...\nGathering Information. This might take some time.");
                    break;
                case FileOperationType.CopyDirectory:
                    ChangeDetailStatusText($"Copying directory {disource.Name} to {ditarget.Name}...\nGathering Information. This might take some time.");
                    break;
            }
            FileSystemInfo[] fsts = disource.GetFileSystemInfos("*" , SearchOption.AllDirectories);
            System.Int64 I = 0;
            void ChangeDetailData_AdditionalInfo(System.String str) 
                => ChangeDetailStatusText($"Copying directory {disource.Name} to {ditarget.Name}...\n" +
                $"{I} items were {(type == FileOperationType.MoveDirectory ? "moved" : "copied")} out of {fsts.LongLength} items.\n{str}");
            for (; I < fsts.Length; I++)
            {
                if (fsts[I] is DirectoryInfo dd) {
                    System.String cd = Path.Join(ditarget.FullName, dd.FullName.Substring(commonsrcnamelen + 1));
                    ChangeDetailData_AdditionalInfo($"Creating directory named {dd.Name} ...");
                    DirectoryInfo ddn = new(cd);
                    ddn.Create();
                    ddn.Attributes = dd.Attributes;
                    ddn = null;
                } else if (fsts[I] is FileInfo fi) {
                    FileStream fss = null, fst = null;
                    FileInfo target = new(Path.Join(ditarget.FullName, fi.FullName.Substring(commonsrcnamelen + 1)));
                    try
                    {
                        ChangeDetailData_AdditionalInfo($"Preparing for creating file named as {fi.Name} ...");
                        fss = fi.OpenRead();
                        fst = target.Open(FileMode.Create, FileAccess.Write, FileShare.Read);
                        System.Threading.Thread.Sleep(100);
                        System.Byte[] bufferscratch = new System.Byte[4096];
                        switch (type)
                        {
                            case FileOperationType.MoveDirectory:
                                ChangeDetailData_AdditionalInfo($"Moving file {fi.Name} ...");
                                break;
                            case FileOperationType.CopyDirectory:
                                ChangeDetailData_AdditionalInfo($"Copying file {fi.Name} ...");
                                break;
                        }
                        System.Int32 rb;
                        System.Byte p;
                        System.Boolean isnotsmall = fss.Length > 1048576;
                        while (token.IsValid(token.InstanceId) && (rb = fss.Read(bufferscratch, 0, bufferscratch.Length)) > 0)
                        {
                            fst.Write(bufferscratch, 0, rb);
                            if (isnotsmall)
                            {
                                p = PercentageHelpers.GetPercentage(fss.Position, fss.Length);
                                switch (type)
                                {
                                    case FileOperationType.MoveDirectory:
                                        ChangeGenericStatusText($"Moving file...\n{p}% of the file data were moved.");
                                        break;
                                    case FileOperationType.CopyDirectory:
                                        ChangeGenericStatusText($"Copying file...\n{p}% of the file data were copied.");
                                        break;
                                }
                                UpdateFileProgressTo(p);
                            }
                        }
                        if (isnotsmall) { ChangeGenericStatusText("Finalizing file..."); }
                    } finally {
                        fss?.Dispose();
                        fss = null;
                        fst?.Dispose();
                        fst = null;
                    }
                    UpdateFileProgressTo(0);
                    ChangeGenericStatusText("");
                    // We do not care if the attributes will be set successfully or not.
                    try { target.Attributes = fi.Attributes; } catch { }
                    target = null;
                }
                System.Threading.Thread.Sleep(55);
                if (token.IsValid(token.InstanceId) == false) { break; }
                UpdateGeneralProgressTo(PercentageHelpers.GetPercentage(I + 1, fsts.LongLength));
                // Null the element to free memory as most as possible.
                // Because this loop may run for a long time for many files (# of files > 100000) , this
                // will free all the data that holds if a GC is run on the unreferenced objects.
                fsts[I] = null; 
            }
        }

        private void ExecuteRecursiveDelete(DirectoryInfo di)
        {
            System.Int64 del = 0;
            var data = di.GetFileSystemInfos("*", SearchOption.TopDirectoryOnly);
            foreach (var fim in data)
            {
                if (token.IsValid(token.InstanceId) == false) { break; }
                if (fim is DirectoryInfo dd) {
                    ChangeGenericStatusText($"Entering directory {dd.Name} ...");
                    ExecuteRecursiveDelete(dd);
                } else if (fim is FileInfo fi) {
                    ChangeGenericStatusText($"Deleting file {fi.Name} ...");
                    fi.Delete();
                    del++;
                    ChangeGenericStatusText("");
                }
                ChangeDetailStatusText($"Deleting Directory {di.Name}...\nDeleted {del} files.");
                UpdateGeneralProgressTo(PercentageHelpers.GetPercentage(del, data.LongLength));
            }
        }

        private void OPT_DeleteDirectories()
        {
            DirectoryInfo disource = new(source);
            System.Int64 deleted = 0;
            ChangeDetailStatusText($"Deleting Directory {disource.Name}...\nDeleted {deleted} files.");
            var data = disource.GetFileSystemInfos("*", SearchOption.TopDirectoryOnly);
            foreach (var fim in data)
            {
                if (token.IsValid(token.InstanceId) == false) { break; }
                if (fim is DirectoryInfo dd) {
                    ChangeGenericStatusText($"Entering directory {dd.Name} ...");
                    ExecuteRecursiveDelete(dd);
                } else if (fim is FileInfo fi) {
                    ChangeGenericStatusText($"Deleting file {fi.Name} ...");
                    fi.Delete();
                    deleted++;
                    ChangeGenericStatusText("");
                }
                ChangeDetailStatusText($"Deleting Directory {disource.Name}...\nDeleted {deleted} items.");
                UpdateGeneralProgressTo(PercentageHelpers.GetPercentage(deleted, data.LongLength));
            }
        }

        private void OperationThreadCode()
        {
            while (dialogform is not null && dialogform.IsHandleCreated == false) { System.Threading.Thread.Sleep(100); }
            state = FileOperationState.Working;
            ChangeDetailStatusText("Starting operation...");
            System.Threading.Thread.Sleep(2000);
            if (token.IsValid(token.InstanceId) == false) { return; }
            try {
                switch (type)
                {
                    case FileOperationType.MoveFile:
                        ChangeDetailStatusText("Moving file...");
                        OPT_CopyMoveFile();
                        File.Delete(source);
                        break;
                    case FileOperationType.CopyFile:
                        ChangeDetailStatusText("Copying file...");
                        OPT_CopyMoveFile();
                        break;
                    case FileOperationType.MoveDirectory:
                        ChangeDetailStatusText($"Moving directory {Path.GetFileName(source)} to {destination}...");
                        OPT_MoveCopyDirectories();
                        ChangeDetailStatusText("Deleting source directory. Please be patient.");
                        Directory.Delete(source, true);
                        System.Threading.Thread.Sleep(2000);
                        break;
                    case FileOperationType.CopyDirectory:
                        ChangeDetailStatusText($"Copying directory {Path.GetFileName(source)} to {destination}...");
                        OPT_MoveCopyDirectories();
                        break;
                    case FileOperationType.DeleteDirectory:
                        OPT_DeleteDirectories();
                        ChangeDetailStatusText($"Deleting empty anymore directory {Path.GetFileName(source)} ...");
                        Directory.Delete(source, false);
                        break;
                    case FileOperationType.DeleteFile:
                        ChangeDetailStatusText($"Deleting file named as {Path.GetFileName(source)} ...");
                        File.Delete(source);
                        break;
                }
                ChangeDetailStatusText("Operation finished, please wait...");
                if (state == FileOperationState.Cancelled)
                {
                    switch (type)
                    {
                        case FileOperationType.MoveFile:
                        case FileOperationType.CopyFile:
                            File.Delete(destination);
                            break;
                        case FileOperationType.CopyDirectory:
                        case FileOperationType.MoveDirectory:
                            Directory.Delete(destination , true);
                            break;
                    }
                    System.Threading.Thread.Sleep(2000);
                }
            } catch (Exception e) {
                ChangeDetailStatusText("Operation failed.");
                state = FileOperationState.Fault;
                try { OnOperationFailed(); } catch { }
                except = e;
                dialogform.Invoke(dialogform.Close);
                return;
            }
            if (state == FileOperationState.Working) { 
                state = FileOperationState.Finished;
                dialogform.Invoke(dialogform.Close);
            }
        }

        public void ShowDialog(IWin32Window parent = null)
        {
            if (dialogform is not null && dialogform.IsHandleCreated) { return; }
            operationthread = new(OperationThreadCode);
            operationthread.Name = InternalResources.MP_DIALOGS_FOPFORMTHREADWORKERNAME;
            operationthread.Priority = System.Threading.ThreadPriority.BelowNormal;
            operationthread.IsBackground = true;
            operationthread.Start();
            dialogform.ShowDialog(parent);
        }

        /// <summary>
        /// Disposes this file operation dialog, after ensuring that the worker thread has been destroyed.
        /// </summary>
        public void Dispose()
        {
            if (CButton is not null)
            {
                CButton.Click -= CancelActionButton_Click;
                CButton.Dispose();
                CButton = null;
            }
            if (OperationDetailsLabel is not null) 
            {
                OperationDetailsLabel.Dispose();
                OperationDetailsLabel = null;
            }
            if (OperationGeneralBar is not null)
            {
                OperationGeneralBar.Dispose();
                OperationGeneralBar = null;
            }
            if (GenericOperationLabel is not null)
            {
                GenericOperationLabel.Dispose();
                GenericOperationLabel = null;
            }
            if (CurrentFileProgressBar is not null)
            {
                CurrentFileProgressBar.Dispose();
                CurrentFileProgressBar = null;
            }
            if (dialogform is not null)
            {
                dialogform.FormClosing -= F_DIALOG_CLOSING;
                dialogform.Dispose();
                dialogform = null;
            }
        }
    }
}
