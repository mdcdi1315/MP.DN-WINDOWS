namespace MP
{
    partial class ChangeTrackIndexDialog
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

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
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            C_Button = new System.Windows.Forms.Button();
            OK_Button = new System.Windows.Forms.Button();
            L1 = new System.Windows.Forms.Label();
            TracksView = new System.Windows.Forms.ListBox();
            SuspendLayout();
            // 
            // C_Button
            // 
            C_Button.BackColor = System.Drawing.SystemColors.ControlText;
            C_Button.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            C_Button.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(0, 0, 192);
            C_Button.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(0, 64, 0);
            C_Button.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            C_Button.Location = new System.Drawing.Point(397, 154);
            C_Button.Name = "C_Button";
            C_Button.Size = new System.Drawing.Size(75, 23);
            C_Button.TabIndex = 0;
            C_Button.Text = "Cancel...";
            C_Button.UseVisualStyleBackColor = false;
            // 
            // OK_Button
            // 
            OK_Button.BackColor = System.Drawing.SystemColors.ControlText;
            OK_Button.DialogResult = System.Windows.Forms.DialogResult.OK;
            OK_Button.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(0, 0, 192);
            OK_Button.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(0, 64, 0);
            OK_Button.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            OK_Button.Location = new System.Drawing.Point(489, 154);
            OK_Button.Name = "OK_Button";
            OK_Button.Size = new System.Drawing.Size(75, 23);
            OK_Button.TabIndex = 1;
            OK_Button.Text = "&OK...";
            OK_Button.UseVisualStyleBackColor = false;
            // 
            // L1
            // 
            L1.AutoSize = true;
            L1.Location = new System.Drawing.Point(14, 12);
            L1.Name = "L1";
            L1.Size = new System.Drawing.Size(123, 15);
            L1.TabIndex = 2;
            L1.Text = "Change track {0} with:";
            // 
            // TracksView
            // 
            TracksView.BackColor = System.Drawing.SystemColors.ControlText;
            TracksView.Cursor = System.Windows.Forms.Cursors.Hand;
            TracksView.ForeColor = System.Drawing.SystemColors.Control;
            TracksView.FormattingEnabled = true;
            TracksView.ItemHeight = 15;
            TracksView.Location = new System.Drawing.Point(19, 36);
            TracksView.Name = "TracksView";
            TracksView.Size = new System.Drawing.Size(521, 94);
            TracksView.TabIndex = 3;
            TracksView.SelectedValueChanged += Selected_Changed;
            // 
            // ChangeTrackIndexDialog
            // 
            AcceptButton = OK_Button;
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            BackColor = System.Drawing.SystemColors.ControlText;
            CancelButton = C_Button;
            ClientSize = new System.Drawing.Size(576, 189);
            Controls.Add(TracksView);
            Controls.Add(L1);
            Controls.Add(OK_Button);
            Controls.Add(C_Button);
            Cursor = System.Windows.Forms.Cursors.Default;
            ForeColor = System.Drawing.SystemColors.Control;
            FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "ChangeTrackIndexDialog";
            ShowIcon = false;
            Text = "Change Track Position...";
            FormClosing += F_CLOSING;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private System.Windows.Forms.Button C_Button;
        private System.Windows.Forms.Button OK_Button;
        private System.Windows.Forms.Label L1;
        private System.Windows.Forms.ListBox TracksView;
    }
}