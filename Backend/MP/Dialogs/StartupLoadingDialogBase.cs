
using System;
using System.Windows.Forms;
using System.Diagnostics.CodeAnalysis;

namespace MP.Dialogs
{
    /// <summary>
    /// A parameterless delegate for running animation code in the invoked dialog. <br />
    /// NOTE: This is called into a loop inside a created thread. <br />
    /// However, the thread guarantees that this code will be executed on the dialog thread.
    /// </summary>
    public delegate void AnimationThreadCode();

    public abstract class StartupLoadingDialogBase : IDisposable
    {
        private Form dialog;
        private System.Int32 nextanimdelaytimems;
        private AnimationThreadCode thc;
        private volatile System.Boolean completed , created;
        private System.Threading.Thread dtthread , animthread;

        public StartupLoadingDialogBase()
        {
            thc = null;
            completed = false;
            created = false;
            dtthread = null;
            animthread = null;
            nextanimdelaytimems = 100;
        }

        private void AnimThreadCodeAbstract()
        {
            while (completed == false)
            {
                dialog.Invoke(thc);
                System.Threading.Thread.Sleep(nextanimdelaytimems);
            }
        }

        /// <summary>
        /// Gets the form object that is the victim of recieveing messages.
        /// </summary>
        protected Form Victim => dialog;

        private void OnClosingInternal(System.Object obj, FormClosingEventArgs e) => OnClosing(e);

        private void OnLoadInternal(System.Object obj, System.EventArgs e)
        {
            OnLoad();
            if (thc is not null)
            {
                animthread = new(AnimThreadCodeAbstract);
                animthread.IsBackground = true;
                animthread.Name = "[MP] Loading Dialog Base animation thread";
                animthread.Start();
            }
        }

        /// <summary>
        /// Override this method to provide code to be executed while the form is loading.
        /// </summary>
        protected virtual void OnLoad() { }

        /// <summary>
        /// Override this method to provide code to be executed while the form is closing.
        /// </summary>
        /// <param name="e">The event arguments also passed thru the event.</param>
        protected virtual void OnClosing([DisallowNull] FormClosingEventArgs e) { }

        /// <summary>
        /// Override this method to provide code to be executed while the <see cref="Dispose"/> method is running. <br />
        /// Note that this method is called just before the loading dialog is destroyed, thus you can provide additional shutdown tasks as you expect through the <paramref name="formdispose"/> parameter.
        /// </summary>
        /// <param name="formdispose">The dialog form that will be disposed once this method completes.</param>
        protected virtual void OnDisposing([DisallowNull] [NotNull] Form formdispose) { }

        /// <summary>
        /// Must be overriden so that the actual form instance is provided.
        /// </summary>
        /// <returns>The provided form instance by the implementer.</returns>
        protected abstract Form GetFormInstance();

        private void LoadDialogF_II()
        {
            completed = false;
            dialog = GetFormInstance();
            if (dialog is null) {
                throw new InvalidOperationException("Form was not intialized properly.");
            }
            dialog.Load += OnLoadInternal;
            dialog.FormClosing += OnClosingInternal;
            created = true;
            dialog.ShowDialog(null);
            dialog.Load -= OnLoadInternal;
            dialog.FormClosing -= OnClosingInternal;
        }

        public void StartAsync()
        {
            if (dtthread is not null) { return; }
            dtthread = new(LoadDialogF_II);
            dtthread.Priority = System.Threading.ThreadPriority.BelowNormal;
            dtthread.Name = "[MP] Loading Dialog Base form thread";
            dtthread.Start();
        }

        public void Close()
        {
            if (completed || created == false) { return; }
            completed = true;
            animthread?.Join();
            animthread = null;
            if (dialog is not null) {
                dialog.Invoke(dialog.Close);
            }
            dtthread?.Join();
            dtthread = null;
        }
        
        public void Hide()
        {
            if (dialog is null || dialog.IsDisposed) { return; }
            if (dialog.InvokeRequired && dialog.IsHandleCreated) {
                dialog.Invoke(dialog.Hide);
            } else {
                dialog.Hide();
            }
        }

        public void Show()
        {
            if (dialog is null || dialog.IsDisposed) { return; }
            if (dialog.InvokeRequired && dialog.IsHandleCreated) {
                dialog.Invoke(dialog.Show);
            } else {
                dialog.Show();
            }
        }

        public AnimationThreadCode AnimationCode
        {
            get => thc;
            set {
                if (animthread is not null) {
                    throw new InvalidOperationException("Cannot change the animation method while the thread is executing.");
                }
                thc = value;
            }
        }

        public System.Int32 NextAnimationDelay
        {
            get => nextanimdelaytimems;
            set {
                if (value < 0) { throw new ArgumentOutOfRangeException(nameof(value) , "Animation Thread delay must not be negative."); }
                nextanimdelaytimems = value;
            }
        }

        public System.Boolean HasCompleted => completed;

        /// <summary>
        /// Disposes this <see cref="StartupLoadingDialogBase"/> class instance.
        /// </summary>
        public void Dispose()
        {
            Close();
            if (dialog is not null)
            {
                OnDisposing(dialog);
                dialog.Dispose();
            }
            dialog = null;
            thc = null;
            GC.SuppressFinalize(this);
        }
    }

}