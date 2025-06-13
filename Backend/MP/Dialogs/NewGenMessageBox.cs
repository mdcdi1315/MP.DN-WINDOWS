using System;
using System.Windows.Forms;

namespace MP.Dialogs
{
    public sealed class NewGenMessageBox : IDisposable
    {
        private Button b1 , b2 , b3;
        private System.String text , title;
        private IconSelection ici;
        private ButtonSelection bs;
        private PictureBox image;
        private Label textdt;
        private ButtonReturned sel;
        private System.Boolean done;
        private ControllerCompatibleForm fm;
        private System.Threading.Thread fmcode;
        private GamepadBackend.GamepadReader rdtemp;

        public NewGenMessageBox()
        {
            rdtemp = null;
            this.text = System.String.Empty;
            fmcode = null;
            done = false;
        }

        public NewGenMessageBox(GamepadBackend.GamepadReader rdr) : this()
        {
            rdtemp = rdr;
        }

        public NewGenMessageBox(GamepadBackend.GamepadReader rdr , System.String text) : this()
        {
            rdtemp = rdr;
            this.text = text ?? System.String.Empty;
            fmcode = null;
        }

        public IconSelection SelectedIcon
        {
            get => ici;
            set => ici = value;
        }

        public System.String Text
        {
            get => text;
            set {
                if (value is null) { return; }
                text = value;
                textdt?.Invoke(() => { textdt.Text = text; });
            }
        }

        public System.String Title
        {
            get => title;
            set {
                if (value is null) { return; }
                title = value;
                if (fm is not null && fm.IsDisposed == false) { fm.Text = title; }
            }
        }

        public ButtonSelection Buttons
        {
            get => bs;
            set => bs = value;
        }

        private void CreateButtonArrangements(System.Drawing.Point lbdrawpoint)
        {
            switch (bs)
            {
                case ButtonSelection.OK:
                    b1 = new() { Text = "OK" };
                    break;
                case ButtonSelection.YesNo:
                    b1 = new() { Text = "Yes" };
                    b2 = new() { Text = "No" };
                    break;
                case ButtonSelection.OKCancel:
                    b1 = new() { Text = "OK" };
                    b2 = new() { Text = "Cancel" };
                    break;
                case ButtonSelection.YesNoCancel:
                    b1 = new() { Text = "Yes" };
                    b2 = new() { Text = "No" };
                    b3 = new() { Text = "Cancel" };
                    break;
                case ButtonSelection.YesNoRetry:
                    b1 = new() { Text = "Yes" };
                    b2 = new() { Text = "No" };
                    b3 = new() { Text = "Retry" };
                    break;
                default:
                    throw new NotSupportedException($"Value {bs} not supported.");
            }
            // Standard size for all buttons
            System.Drawing.Size buttonsize = new(72 , 26);
            // The first button will always be present so do this:
            b1.Size = buttonsize;
            b1.ForeColor = System.Drawing.Color.White;
            b1.BackColor = System.Drawing.Color.Black;
            b1.Location = new(lbdrawpoint.X - buttonsize.Width, lbdrawpoint.Y - buttonsize.Height);
            b1.DialogResult = DialogResult.OK;
            fm.Controls.Add(b1);
            if (b2 is not null)
            {
                b2.Size = buttonsize;
                b2.Location = new(b1.Location.X - 10 - buttonsize.Width , b1.Location.Y);
                b2.DialogResult = DialogResult.Cancel;
                b2.ForeColor = System.Drawing.Color.White;
                b2.BackColor = System.Drawing.Color.Black;
                fm.Controls.Add(b2);
            }
            if (b3 is not null) 
            {
                b3.Size = buttonsize;
                b3.Location = new(b2.Location.X - 10 - buttonsize.Width, b1.Location.Y);
                b3.DialogResult = DialogResult.Abort;
                b3.ForeColor = System.Drawing.Color.White;
                b3.BackColor = System.Drawing.Color.Black;
                fm.Controls.Add(b3);
            }
        }

        private System.Drawing.Point DrawTextAndGetButtonLoc()
        {
            System.Drawing.Point ret;
            System.Drawing.SizeF size;
            using (var g = System.Drawing.Graphics.FromHwnd(fm.Handle))
            {
                size = g.MeasureString(text, fm.Font);
                fm.Size = new((System.Int32)(size.Width + 38 + image.Location.X + image.Width), (System.Int32)(size.Height + 120));
                fm.Update();
                g.Clear(fm.BackColor);
                System.Drawing.Point drawpoint = new System.Drawing.Point(image.Location.X + image.Width + 10, 14);
                textdt = new();
                textdt.Location = drawpoint;
                textdt.Font = fm.Font;
                textdt.Size = new((System.Int32)size.Width + 1 , (System.Int32)size.Height + 1);
                textdt.Text = text;
                ret = new(drawpoint.X + (System.Int32)size.Width - 8 , drawpoint.Y + (System.Int32)size.Height + 43);
            }
            return ret;
        }

        private void WindowCreated(System.Object sender, System.EventArgs e)
        {
            UpdateData();
            fm.Controls.Add(textdt);
            textdt.Update();
        }

        private void UpdateData()
        {
            System.Drawing.Point firstbutton = DrawTextAndGetButtonLoc();
            CreateButtonArrangements(firstbutton);
            fm.Text = title;
            var ic = ici switch { 
                IconSelection.Error => System.Drawing.SystemIcons.Error,
                IconSelection.Info => System.Drawing.SystemIcons.Information,
                IconSelection.Warning => System.Drawing.SystemIcons.Warning,
                IconSelection.Notice => System.Drawing.SystemIcons.Information,
                IconSelection.Question => System.Drawing.SystemIcons.Question,
                _ => null
            };
            if (ic is not null)
            {
                image.Image = ic.ToBitmap();
                ic.Dispose();
            }
        }

        private void ThreadCode(System.Object obj)
        {
            fm = new(rdtemp);
            rdtemp = null;
            fm.BackColor = System.Drawing.Color.Black;
            fm.ForeColor = System.Drawing.Color.White;
            fm.ControlBox = true;
            fm.MinimizeBox = false;
            fm.MaximizeBox = false;
            fm.FormBorderStyle = FormBorderStyle.FixedDialog;
            fm.StartPosition = FormStartPosition.CenterParent;
            fm.ShowInTaskbar = false;
            fm.TopMost = true;
            image = new();
            image.Location = new(10, 19);
            image.Size = new(40, 40);
            b1 = b2 = b3 = null;
            fm.Controls.Add(image);
            fm.Shown += WindowCreated;
            done = true;  
        }

        public void ShowDialog(System.IntPtr hwnd = 0) => ShowDialog(new ReadOnlyWindowHandle(hwnd));

        public void ShowDialog(IWin32Window parent)
        {
            if (fmcode is not null) { return; }
            fmcode = new(ThreadCode);
            fmcode.TrySetApartmentState(System.Threading.ApartmentState.STA);
            fmcode.Priority = System.Threading.ThreadPriority.Lowest;
            fmcode.Start(parent);
            while (done == false) { System.Threading.Thread.Sleep(10); }
            sel = ButtonReturned.None;
            DialogResult dgr = fm.ShowDialog(parent);
            switch (bs)
            {
                case ButtonSelection.OK:
                    if (dgr == DialogResult.OK) { sel = ButtonReturned.OK; }
                    break;
                case ButtonSelection.YesNo:
                    switch (dgr)
                    {
                        case DialogResult.OK:
                            sel = ButtonReturned.Yes;
                            break;
                        case DialogResult.Cancel:
                            sel = ButtonReturned.No;
                            break;
                    }
                    break;
                case ButtonSelection.OKCancel:
                    switch (dgr)
                    {
                        case DialogResult.OK:
                            sel = ButtonReturned.OK;
                            break;
                        case DialogResult.Cancel:
                            sel = ButtonReturned.Cancel;
                            break;
                    }
                    break;
                case ButtonSelection.YesNoCancel:
                    switch (dgr)
                    {
                        case DialogResult.OK:
                            sel = ButtonReturned.Yes;
                            break;
                        case DialogResult.Cancel:
                            sel = ButtonReturned.No;
                            break;
                        case DialogResult.Abort:
                            sel = ButtonReturned.Cancel;
                            break;
                    }
                    break;
                case ButtonSelection.YesNoRetry:
                    switch (dgr)
                    {
                        case DialogResult.OK:
                            sel = ButtonReturned.Yes;
                            break;
                        case DialogResult.Cancel:
                            sel = ButtonReturned.No;
                            break;
                        case DialogResult.Abort:
                            sel = ButtonReturned.Retry;
                            break;
                    }
                    break;
            }
        }

        public ButtonReturned ReturnedButton => sel;

        /// <summary>
        /// Invalidates the form's contents.
        /// </summary>
        public void Update()
        {
            if (fm is null) { return; }
            fm.Update();
            fm.Invoke(UpdateData);
        }

        public void Dispose()
        {
            if (fm is not null)
            {
                fm.Shown -= WindowCreated;
                fm.Dispose();
                fm = null;
            }
            image?.Image?.Dispose();
            image?.Dispose();
            image = null;
            b1?.Dispose();
            b1 = null;
            b2?.Dispose();
            b2 = null;
            b3?.Dispose();
            b3 = null;
            textdt?.Dispose();
            textdt = null;
            text = null;
            fmcode = null;
        }
    }
}
