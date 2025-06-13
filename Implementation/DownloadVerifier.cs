
using System;
using MP.Dialogs;
using Microsoft.IO;
using MP.Threading;
using MP.DownloadableDeps;

namespace MP
{
    public sealed class DownloadVerifier : ProgressDialog
    {
        private sealed class CancellationToken : ICancellationToken<System.Byte>
        {
            private Dialogs.ProgressDialogInstance inst;

            public CancellationToken(Dialogs.ProgressDialogInstance inst) { this.inst = inst; }

            public byte InstanceId => 0;

            public void Invalidate() { }

            public bool IsValid(byte unique) => inst.HasBeenCanceled() == false;
        }

        private DirectoryInfo dibase;
        private FileInfo verifierfile;

        public DownloadVerifier(DirectoryInfo basedir , FileInfo verifierfile)
        {
            ArgumentNullException.ThrowIfNull(basedir);
            ArgumentNullException.ThrowIfNull(verifierfile);
            dibase = basedir;
            this.verifierfile = verifierfile;
            Title = "Downloaded sources verification in progress...";
            CancellationMessage = "Please wait...";
        }

        protected override void DialogCode(ProgressDialogInstance inst)
        {
            void UpdateState(InstallationVerifierStatus status , System.Object obj)
            {
                switch (status)
                {
                    case InstallationVerifierStatus.NumberOfTotalItemsToCheck:
                        inst.ResetTimer();
                        inst.Max = ((System.Int64)obj).ToUInt64();
                        break;
                    case InstallationVerifierStatus.VerifyingFile:
                        inst.ChangeText(3 , $"Verifying file: {obj}" , true);
                        System.Threading.Thread.Sleep(300);
                        break;
                    case InstallationVerifierStatus.VerifyingDependency:
                        inst.ChangeText(2, $"Verifying dependency: {obj}");
                        System.Threading.Thread.Sleep(1000);
                        break;
                    case InstallationVerifierStatus.VerificationFailedOn:
                        inst.ChangeText(1, "Verification failed.");
                        inst.ChangeText(2, $"Failed on file: {obj}");
                        inst.ChangeText(3, System.String.Empty);
                        break;
                    case InstallationVerifierStatus.UpdateVerifiedItems:
                        inst.ProgressCurrent = ((System.Int64)obj).ToUInt64();
                        break;
                }
            }

            inst.ChangeText(1, "Verifying downloaded sources.");
            inst.ChangeText(2, $"Opening verifier file: {verifierfile.FullName}" , true);
            FileStream fsm = null;
            DownloadInstallationVerifier vrf = null;
            try {
                fsm = verifierfile.OpenRead();
                System.Threading.Thread.Sleep(100);
                vrf = new(dibase, fsm);
                System.Threading.Thread.Sleep(100);
                vrf.OnStatusChanged += UpdateState;
                if (vrf.VerifyInstallation(new CancellationToken(inst)) && inst.HasBeenCanceled() == false)
                {
                    NativeMessageBox.Show("All sources have been successfully verified. The installation is usuable.", "Information", ButtonSelection.OK, IconSelection.Info);
                }
            } catch (Exception ex) {
                inst.ChangeText(1, "Verification failed.");
                inst.ChangeText(2 , "Operation failed:");
                inst.ChangeText(3 , ex.Message);
                DebugProvider.WriteLine($"VERIFIER: Logging error: {ex}");
                System.Threading.Thread.Sleep(1000);
            } finally {
                vrf?.Dispose();
                vrf = null;
                fsm?.Dispose();
                fsm = null;
            }
        }
    }
}