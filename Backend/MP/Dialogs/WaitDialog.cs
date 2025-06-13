using System;
using System.Windows.Forms;

namespace MP.Dialogs
{
    /// <summary>
    /// Wait messages dialog class.
    /// </summary>
    public sealed class WaitDialog : IDisposable
    {
        private Form dialogform;
        private System.String data;
        private IWin32Window parent;
        private System.Boolean close;
        private System.Threading.Thread waitthread;

        public WaitDialog() { dialogform = null; close = false; }

        public WaitDialog(System.String data) : this()
        {
            this.data = data;
        }

        public WaitDialog(System.String data, IWin32Window parent) : this() 
        {
            this.data = data;
            this.parent = parent;
        }

        public WaitDialog(System.String data , System.IntPtr hwnd)
        {
            this.data = data;
            parent = new ReadOnlyWindowHandle(hwnd);
        }

        public void Show()
        {
            if (waitthread is not null) { return; }
            waitthread = new(Init);
            waitthread.TrySetApartmentState(System.Threading.ApartmentState.STA);
            waitthread.Priority = System.Threading.ThreadPriority.Lowest;
            waitthread.Name = "[MP] Wait Dialog Thread";
            waitthread.Start();
        }

        private void Init()
        {
            dialogform = new();
            dialogform.BackColor = System.Drawing.Color.Black;
            dialogform.ForeColor = System.Drawing.Color.White;
            dialogform.ControlBox = false;
            dialogform.FormBorderStyle = FormBorderStyle.FixedDialog;
            dialogform.StartPosition = FormStartPosition.CenterParent;
            dialogform.ShowInTaskbar = false;
            dialogform.TopMost = true;
            dialogform.Shown += DialogForm_Shown;
            dialogform.FormClosing += DialogForm_FormClosing;
            dialogform.ShowDialog(parent);
        }

        private void DialogForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.WindowsShutDown) {
                return;
            }
            // Ensure that the wait window will not close 
            // upon external events (Such as using ALT + F4).
            e.Cancel = close == false;
        }

        private void DialogForm_Shown(object sender, EventArgs e) => UpdateText();

        private void UpdateText()
        {
            if (dialogform.InvokeRequired) {
                dialogform.Invoke(UpdateTextUnsafe);
            } else {
                UpdateTextUnsafe();
            }
        }

        private void UpdateTextUnsafe()
        {
            System.Drawing.SizeF size;
            using (var g = System.Drawing.Graphics.FromHwnd(dialogform.Handle))
            {
                size = g.MeasureString(data, dialogform.Font);
                dialogform.Size = new((System.Int32)(size.Width + 38), (System.Int32)(size.Height + 60));
                g.Clear(System.Drawing.Color.Black);
                using (var sb = new System.Drawing.SolidBrush(System.Drawing.Color.White))
                {
                    g.DrawString(data, dialogform.Font, sb, new System.Drawing.Point(10, dialogform.Size.Height / 3));
                }
            }
        }

        /// <summary>
        /// Gets or sets the text to be drawn.
        /// </summary>
        public System.String Text
        {
            get => data;
            set {
                data = value;
                if (data is null) { data = System.String.Empty; }
                if (dialogform is null) { return; }
                UpdateText();
                // Call it a second time to ensure that all the data are displayed properly.
                UpdateText();
            }
        }

        /// <summary>
        /// Gets the font that will be used to draw the <see cref="Text"/> property.
        /// </summary>
        public System.Drawing.Font DrawFont
        {
            get => dialogform?.Font;
            set {
                if (dialogform is null) { return; }
                dialogform.Font = value;
            }
        }

        /// <summary>
        /// Invalidates the form's contents.
        /// </summary>
        public void Update()
        {
            if (dialogform is null) { return; }
            dialogform.Update();
            UpdateText();
        }

        /// <summary>
        /// Disposes this <see cref="WaitDialog"/> class instance. 
        /// </summary>
        public void Dispose() 
        {
            if (dialogform is null) { return; }
            close = true; // Now the window can close.
            if (dialogform.IsHandleCreated) { dialogform.Invoke(dialogform.Close); }
            waitthread.Join();
            dialogform.Dispose();
            dialogform = null;
            waitthread = null;
        }
    }
}
