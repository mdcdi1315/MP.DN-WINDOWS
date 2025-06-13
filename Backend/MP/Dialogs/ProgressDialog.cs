
using System;
using MP.ComInterop;
using System.Threading;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using MP.WindowsInterop;

namespace MP.Dialogs
{
    public abstract class ProgressDialogInstance
    {
        private System.IntPtr parent;
        private System.UInt64 current, max;

        protected internal ProgressDialogInstance(System.IntPtr parent) => this.parent = parent;

        public System.UInt64 ProgressCurrent
        {
            get => current;
            set => UpdateProgress(current = value, max);
        }

        public System.UInt64 Max
        {
            get => max;
            set => UpdateProgress(current, max = value);
        }

        public System.IntPtr Parent => parent;

        protected abstract void UpdateProgress(System.UInt64 current , System.UInt64 max);

        public abstract void ChangeText(System.Int32 line, System.String newtext, System.Boolean compactpaths = false);

        public abstract void ResetTimer();

        public abstract System.Boolean HasBeenCanceled();
    }

    /// <summary>
    /// Defines the progress dialog class, a specialized class for operations to be done.
    /// </summary>
    public abstract class ProgressDialog : IDisposable
    {
        private sealed class PDI : ProgressDialogInstance
        {
            private ProgressDialog reference;

            public PDI(ProgressDialog ppd , System.IntPtr pt) : base(pt) {
                reference = ppd;
            }

            protected override void UpdateProgress(ulong current, ulong max) => reference.pgddialog.SetProgress64(current, max);

            public unsafe override void ResetTimer() => reference.pgddialog.Timer(ProgressDialogTimerAction.PDTIMER_RESET, null);

            public override bool HasBeenCanceled() => reference.pgddialog.HasUserCancelled() == BOOL.TRUE || reference.cancelledinternal;

            public unsafe override void ChangeText(int line, string newtext , System.Boolean compactpaths = false)
            {
                if (line < 1 || line > 3) {
                    throw new ArgumentException("line parameter must be in range from 1 to 3.");
                }
                if (newtext is null) { newtext = System.String.Empty; }
                fixed (System.Char* ptext = newtext)
                {
                    reference.pgddialog.SetLine(line.ToUInt32(), ptext , compactpaths ? BOOL.TRUE : BOOL.FALSE , null);
                }
            }
        }

        private System.Boolean cancelledinternal;
        private System.String title, cancelmsg;
        private ProgressDialogFlags flags;
        private IProgressDialog pgddialog;
        private Thread dialogthread;
        
        public ProgressDialog()
        {
            flags = 0;
            cancelledinternal = false;
            title = System.String.Empty;
        }

        public System.Boolean CreateModal
        {
            get => flags.HasFlag(ProgressDialogFlags.PROGDLG_MODAL);
            set {
                if (value) {
                    flags |= ProgressDialogFlags.PROGDLG_MODAL;
                } else {
                    flags &= ~ProgressDialogFlags.PROGDLG_MODAL;
                }
            }
        }

        public System.Boolean AutoTimer
        {
            get => flags.HasFlag(ProgressDialogFlags.PROGDLG_AUTOTIME);
            set {
                if (value) {
                    flags |= ProgressDialogFlags.PROGDLG_AUTOTIME;
                } else {
                    flags &= ~ProgressDialogFlags.PROGDLG_AUTOTIME;
                }
            }
        }

        public System.Boolean ShowMinimizeButton
        {
            get => flags.HasFlag(ProgressDialogFlags.PROGDLG_NOMINIMIZE) == false;
            set {
                if (value) {
                    flags &= ~ProgressDialogFlags.PROGDLG_NOMINIMIZE;
                } else {
                    flags |= ProgressDialogFlags.PROGDLG_NOMINIMIZE;
                }
            }
        }

        public System.Boolean ShowCancelButton
        {
            get => flags.HasFlag(ProgressDialogFlags.PROGDLG_NOCANCEL) == false;
            set {
                if (value) {
                    flags &= ~ProgressDialogFlags.PROGDLG_NOCANCEL;
                } else {
                    flags |= ProgressDialogFlags.PROGDLG_NOCANCEL;
                }
            }
        }

        public System.Boolean NoProgressBar
        {
            get => flags.HasFlag(ProgressDialogFlags.PROGDLG_NOPROGRESSBAR);
            set {
                if (value) {
                    flags |= ProgressDialogFlags.PROGDLG_NOPROGRESSBAR;
                } else {
                    flags &= ~ProgressDialogFlags.PROGDLG_NOPROGRESSBAR;
                }
            }
        }

        public System.String Title
        {
            get => title;
            set => title = value;
        }

        public System.String CancellationMessage
        {
            get => cancelmsg;
            set => cancelmsg = value;
        }

        protected abstract void DialogCode(ProgressDialogInstance inst);
        
        private unsafe void DialogThreadCode(System.Object obj)
        {
            pgddialog = ComMarshalling.GetClassInstanceAsInterface<IProgressDialog>(CLSCTX.CLSCTX_INPROC_SERVER);
            HRESULT hr = pgddialog.StartProgressDialog((IntPtr)obj , null , flags , null);
            if (hr.FAILED) {
                DebugProvider.WriteLine($"PROGDLG: Progress Dialog failed: {hr.MappingException}");
                return;
            }
            if (title is not null)
            {
                fixed (System.Char* pt = title) 
                {
                    pgddialog.SetTitle(pt);
                }
            }
            if (cancelmsg is not null)
            {
                fixed (System.Char* pc = cancelmsg)
                {
                    pgddialog.SetCancelMsg(pc, null);
                }
            }
            DebugProvider.WriteLine("PROGDLG: Executing user code.");
            try {
                cancelledinternal = false;
                DialogCode(new PDI(this , (IntPtr)obj));
            } catch (Exception ex) {
                DebugProvider.WriteLine($"PROGDLG: Exception thrown in user code: {ex}");
            }
            DebugProvider.WriteLine("PROGDLG: Execution completed.");
            pgddialog.StopProgressDialog();
        }

        /// <summary>
        /// Presents the dialog to the current parent window. <br />
        /// The parent window provided MUST BE a valid window handle object.
        /// </summary>
        /// <param name="window">The <see cref="IWin32Window"/> to act as the dialog's parent form.</param>
        /// <exception cref="ArgumentNullException"><paramref name="window"/> was <see langword="null"/>.</exception>
        /// <exception cref="ArgumentException">The <paramref name="window"/> provided has an invalid handle value.</exception>
        public void ShowDialog(IWin32Window window)
        {
            if (dialogthread is not null && dialogthread.IsAlive) { return; }
            ArgumentNullException.ThrowIfNull(window, nameof(window));
            if (window.Handle == IntPtr.Zero) {
                throw new ArgumentException(InternalResources.MP_DIALOGS_PDDIALOG_INVALID_HANDLE, nameof(window));
            }
            dialogthread = new(DialogThreadCode);
            dialogthread.TrySetApartmentState(ApartmentState.STA);
            dialogthread.Priority = ThreadPriority.BelowNormal;
            dialogthread.Name = InternalResources.MP_DIALOGS_PDDIALOG_THREADNAME;
            dialogthread.Start(window.Handle);
            dialogthread.Join();
        }

        /// <summary>
        /// Disposes this <see cref="ProgressDialog"/> instance, 
        /// blocking the thread that called this method if the dialog has not yet terminated.
        /// </summary>
        public void Dispose()
        {
            if (pgddialog is not null)
            {
                cancelledinternal = true;
                dialogthread?.Join();
                dialogthread = null;
                try { 
                    pgddialog.StopProgressDialog();
                    ComMarshalling.ReleaseInteropObject(pgddialog);
                } catch (InvalidComObjectException) {
                    DebugProvider.WriteLine($"PROGDLG: Native Dialog COM object with hash code {pgddialog.GetHashCode():x2} seems already to be freed, so just exiting.");
                }
                pgddialog = null;
            }
            GC.SuppressFinalize(this);
        }
    }
}