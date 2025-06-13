namespace MP
{
    partial class DependenciesDownloadForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            L1 = new System.Windows.Forms.Label();
            DepBox = new System.Windows.Forms.ListBox();
            StatusText = new System.Windows.Forms.Label();
            StartButton = new System.Windows.Forms.Button();
            CButton = new System.Windows.Forms.Button();
            SuspendLayout();
            // 
            // L1
            // 
            L1.AutoSize = true;
            L1.Location = new System.Drawing.Point(8, 7);
            L1.Name = "L1";
            L1.Size = new System.Drawing.Size(363, 15);
            L1.TabIndex = 0;
            L1.Text = "This feature requires optional dependencies - download them now?";
            // 
            // DepBox
            // 
            DepBox.BackColor = System.Drawing.SystemColors.WindowText;
            DepBox.ForeColor = System.Drawing.SystemColors.Window;
            DepBox.FormattingEnabled = true;
            DepBox.HorizontalScrollbar = true;
            DepBox.ItemHeight = 15;
            DepBox.Location = new System.Drawing.Point(27, 35);
            DepBox.Name = "DepBox";
            DepBox.Size = new System.Drawing.Size(407, 79);
            DepBox.TabIndex = 1;
            // 
            // StatusText
            // 
            StatusText.AutoSize = true;
            StatusText.Location = new System.Drawing.Point(23, 125);
            StatusText.Name = "StatusText";
            StatusText.Size = new System.Drawing.Size(184, 15);
            StatusText.TabIndex = 2;
            StatusText.Text = "The operation has not started yet.";
            // 
            // StartButton
            // 
            StartButton.BackColor = System.Drawing.SystemColors.ControlText;
            StartButton.ForeColor = System.Drawing.SystemColors.Control;
            StartButton.Location = new System.Drawing.Point(268, 134);
            StartButton.Name = "StartButton";
            StartButton.Size = new System.Drawing.Size(75, 23);
            StartButton.TabIndex = 3;
            StartButton.Text = "Start...";
            StartButton.UseVisualStyleBackColor = false;
            StartButton.Click += StartButton_Click;
            // 
            // CButton
            // 
            CButton.BackColor = System.Drawing.SystemColors.ControlText;
            CButton.Location = new System.Drawing.Point(359, 134);
            CButton.Name = "CButton";
            CButton.Size = new System.Drawing.Size(75, 23);
            CButton.TabIndex = 4;
            CButton.Text = "Cancel...";
            CButton.UseVisualStyleBackColor = false;
            CButton.Click += CancelButton_Click;
            // 
            // DependenciesDownloadForm
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            BackColor = System.Drawing.SystemColors.ControlText;
            ClientSize = new System.Drawing.Size(462, 171);
            ControlBox = false;
            Controls.Add(CButton);
            Controls.Add(StartButton);
            Controls.Add(StatusText);
            Controls.Add(DepBox);
            Controls.Add(L1);
            ForeColor = System.Drawing.SystemColors.Control;
            FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "DependenciesDownloadForm";
            StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            Text = "Additional Dependencies required";
            FormClosing += F_CLOSING;
            Load += F_LOAD;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private System.Windows.Forms.Label L1;
        private System.Windows.Forms.ListBox DepBox;
        private System.Windows.Forms.Label StatusText;
        private System.Windows.Forms.Button StartButton;
        private System.Windows.Forms.Button CButton;
    }
}