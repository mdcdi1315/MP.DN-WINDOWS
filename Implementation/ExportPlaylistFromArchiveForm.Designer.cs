namespace MP
{
    partial class ExportPlaylistFromArchiveForm
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
            ProgressInformationLabel = new System.Windows.Forms.Label();
            StartCancelOpButton = new System.Windows.Forms.Button();
            OperationProgressBar = new System.Windows.Forms.ProgressBar();
            SuspendLayout();
            // 
            // ProgressInformationLabel
            // 
            ProgressInformationLabel.AutoSize = true;
            ProgressInformationLabel.Location = new System.Drawing.Point(11, 9);
            ProgressInformationLabel.Name = "ProgressInformationLabel";
            ProgressInformationLabel.Size = new System.Drawing.Size(456, 30);
            ProgressInformationLabel.TabIndex = 0;
            ProgressInformationLabel.Text = "This dialog assists you exporting a playlist to a physical one without additional hassle.\r\nPress 'Start...' to begin the operation.";
            // 
            // StartCancelOpButton
            // 
            StartCancelOpButton.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(192, 192, 0);
            StartCancelOpButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            StartCancelOpButton.Location = new System.Drawing.Point(490, 178);
            StartCancelOpButton.Name = "StartCancelOpButton";
            StartCancelOpButton.Size = new System.Drawing.Size(75, 23);
            StartCancelOpButton.TabIndex = 1;
            StartCancelOpButton.Text = "Start...";
            StartCancelOpButton.UseVisualStyleBackColor = true;
            StartCancelOpButton.Click += STARTCANCELOP;
            // 
            // OperationProgressBar
            // 
            OperationProgressBar.ForeColor = System.Drawing.SystemColors.Control;
            OperationProgressBar.Location = new System.Drawing.Point(12, 178);
            OperationProgressBar.Name = "OperationProgressBar";
            OperationProgressBar.Size = new System.Drawing.Size(455, 23);
            OperationProgressBar.Step = 1;
            OperationProgressBar.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
            OperationProgressBar.TabIndex = 2;
            // 
            // ExportPlaylistFromArchiveForm
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            BackColor = System.Drawing.SystemColors.ControlText;
            ClientSize = new System.Drawing.Size(577, 213);
            Controls.Add(OperationProgressBar);
            Controls.Add(StartCancelOpButton);
            Controls.Add(ProgressInformationLabel);
            ForeColor = System.Drawing.SystemColors.Control;
            FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "ExportPlaylistFromArchiveForm";
            StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            Text = "Export To a Physical Playlist...";
            FormClosing += F_CLOSING;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private System.Windows.Forms.Label ProgressInformationLabel;
        private System.Windows.Forms.Button StartCancelOpButton;
        private System.Windows.Forms.ProgressBar OperationProgressBar;
    }
}