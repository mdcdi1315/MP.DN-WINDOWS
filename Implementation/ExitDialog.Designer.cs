namespace MP
{
    partial class ExitDialog
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
            MainInfo = new System.Windows.Forms.Label();
            ContinuePlaybackOpt = new System.Windows.Forms.CheckBox();
            Cbutton = new System.Windows.Forms.Button();
            ExitButton = new System.Windows.Forms.Button();
            SuspendLayout();
            // 
            // MainInfo
            // 
            MainInfo.AutoSize = true;
            MainInfo.Location = new System.Drawing.Point(185, 31);
            MainInfo.Name = "MainInfo";
            MainInfo.Size = new System.Drawing.Size(101, 15);
            MainInfo.TabIndex = 0;
            MainInfo.Text = "Exit Music Player?";
            // 
            // ContinuePlaybackOpt
            // 
            ContinuePlaybackOpt.AutoSize = true;
            ContinuePlaybackOpt.FlatAppearance.BorderSize = 0;
            ContinuePlaybackOpt.FlatAppearance.CheckedBackColor = System.Drawing.Color.FromArgb(0, 192, 192);
            ContinuePlaybackOpt.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Gray;
            ContinuePlaybackOpt.Location = new System.Drawing.Point(12, 68);
            ContinuePlaybackOpt.Name = "ContinuePlaybackOpt";
            ContinuePlaybackOpt.Size = new System.Drawing.Size(336, 19);
            ContinuePlaybackOpt.TabIndex = 1;
            ContinuePlaybackOpt.Text = "Also continue playback from the current point at next time";
            ContinuePlaybackOpt.UseVisualStyleBackColor = true;
            ContinuePlaybackOpt.CheckedChanged += Update_Value_OPT1;
            // 
            // Cbutton
            // 
            Cbutton.BackColor = System.Drawing.SystemColors.ControlText;
            Cbutton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            Cbutton.FlatAppearance.BorderColor = System.Drawing.Color.White;
            Cbutton.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(0, 0, 192);
            Cbutton.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(0, 64, 0);
            Cbutton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            Cbutton.ForeColor = System.Drawing.SystemColors.Control;
            Cbutton.Location = new System.Drawing.Point(294, 137);
            Cbutton.Name = "Cbutton";
            Cbutton.Size = new System.Drawing.Size(75, 23);
            Cbutton.TabIndex = 2;
            Cbutton.Text = "Cancel";
            Cbutton.UseVisualStyleBackColor = false;
            // 
            // ExitButton
            // 
            ExitButton.BackColor = System.Drawing.SystemColors.ControlText;
            ExitButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            ExitButton.FlatAppearance.BorderColor = System.Drawing.Color.White;
            ExitButton.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(0, 0, 192);
            ExitButton.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(0, 64, 0);
            ExitButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            ExitButton.ForeColor = System.Drawing.SystemColors.Control;
            ExitButton.Location = new System.Drawing.Point(399, 137);
            ExitButton.Name = "ExitButton";
            ExitButton.Size = new System.Drawing.Size(75, 23);
            ExitButton.TabIndex = 3;
            ExitButton.Text = "Exit...";
            ExitButton.UseVisualStyleBackColor = false;
            // 
            // ExitDialog
            // 
            AcceptButton = ExitButton;
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            BackColor = System.Drawing.SystemColors.ControlText;
            CancelButton = Cbutton;
            ClientSize = new System.Drawing.Size(486, 172);
            ControlBox = false;
            Controls.Add(ExitButton);
            Controls.Add(Cbutton);
            Controls.Add(ContinuePlaybackOpt);
            Controls.Add(MainInfo);
            ForeColor = System.Drawing.SystemColors.Control;
            FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            Name = "ExitDialog";
            ShowInTaskbar = false;
            StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            FormClosing += F_CLOSING;
            Load += F_LOAD;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private System.Windows.Forms.Label MainInfo;
        private System.Windows.Forms.CheckBox ContinuePlaybackOpt;
        private System.Windows.Forms.Button Cbutton;
        private System.Windows.Forms.Button ExitButton;
    }
}