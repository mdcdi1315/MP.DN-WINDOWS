namespace MP
{
    partial class CreatePlaylistArchiveDialog
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
            StatusLabel = new System.Windows.Forms.Label();
            GenericButton_1 = new System.Windows.Forms.Button();
            PathBox = new System.Windows.Forms.TextBox();
            StartOpButton = new System.Windows.Forms.Button();
            ArchiveProgressBar = new System.Windows.Forms.ProgressBar();
            AdditonalAttributesButton = new System.Windows.Forms.Button();
            SuspendLayout();
            // 
            // StatusLabel
            // 
            StatusLabel.AutoSize = true;
            StatusLabel.Location = new System.Drawing.Point(14, 10);
            StatusLabel.Name = "StatusLabel";
            StatusLabel.Size = new System.Drawing.Size(309, 45);
            StatusLabel.TabIndex = 0;
            StatusLabel.Text = "Create Playlist Archive dialog.\r\nYou have been reached here after selecting the {0} playlist\r\nto be archived.\r\n";
            // 
            // GenericButton_1
            // 
            GenericButton_1.BackColor = System.Drawing.SystemColors.ControlText;
            GenericButton_1.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(0, 0, 192);
            GenericButton_1.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(0, 64, 0);
            GenericButton_1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            GenericButton_1.ForeColor = System.Drawing.SystemColors.Control;
            GenericButton_1.Location = new System.Drawing.Point(472, 212);
            GenericButton_1.Name = "GenericButton_1";
            GenericButton_1.Size = new System.Drawing.Size(75, 23);
            GenericButton_1.TabIndex = 1;
            GenericButton_1.Text = "Browse...";
            GenericButton_1.UseVisualStyleBackColor = false;
            GenericButton_1.Click += GenericButton_1_Click;
            // 
            // PathBox
            // 
            PathBox.AllowDrop = true;
            PathBox.BackColor = System.Drawing.SystemColors.ControlText;
            PathBox.ForeColor = System.Drawing.SystemColors.Control;
            PathBox.Location = new System.Drawing.Point(12, 213);
            PathBox.Name = "PathBox";
            PathBox.Size = new System.Drawing.Size(454, 23);
            PathBox.TabIndex = 2;
            // 
            // StartOpButton
            // 
            StartOpButton.BackColor = System.Drawing.SystemColors.ControlText;
            StartOpButton.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(0, 0, 192);
            StartOpButton.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(0, 64, 0);
            StartOpButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            StartOpButton.ForeColor = System.Drawing.SystemColors.Control;
            StartOpButton.Location = new System.Drawing.Point(350, 134);
            StartOpButton.Name = "StartOpButton";
            StartOpButton.Size = new System.Drawing.Size(197, 30);
            StartOpButton.TabIndex = 3;
            StartOpButton.Text = "Start Archiving Operation...";
            StartOpButton.UseVisualStyleBackColor = false;
            StartOpButton.Click += StartOpButton_Click;
            // 
            // ArchiveProgressBar
            // 
            ArchiveProgressBar.BackColor = System.Drawing.SystemColors.ControlText;
            ArchiveProgressBar.ForeColor = System.Drawing.SystemColors.Control;
            ArchiveProgressBar.Location = new System.Drawing.Point(12, 213);
            ArchiveProgressBar.Name = "ArchiveProgressBar";
            ArchiveProgressBar.Size = new System.Drawing.Size(454, 23);
            ArchiveProgressBar.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
            ArchiveProgressBar.TabIndex = 4;
            ArchiveProgressBar.Visible = false;
            // 
            // AdditonalAttributesButton
            // 
            AdditonalAttributesButton.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(0, 0, 192);
            AdditonalAttributesButton.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(0, 64, 0);
            AdditonalAttributesButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            AdditonalAttributesButton.Location = new System.Drawing.Point(350, 95);
            AdditonalAttributesButton.Name = "AdditonalAttributesButton";
            AdditonalAttributesButton.Size = new System.Drawing.Size(197, 27);
            AdditonalAttributesButton.TabIndex = 5;
            AdditonalAttributesButton.Text = "Advanced Options...";
            AdditonalAttributesButton.UseVisualStyleBackColor = true;
            AdditonalAttributesButton.Click += OPENADVFUNCBOX;
            // 
            // CreatePlaylistArchiveDialog
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            BackColor = System.Drawing.SystemColors.ControlText;
            ClientSize = new System.Drawing.Size(559, 247);
            Controls.Add(AdditonalAttributesButton);
            Controls.Add(ArchiveProgressBar);
            Controls.Add(StartOpButton);
            Controls.Add(PathBox);
            Controls.Add(GenericButton_1);
            Controls.Add(StatusLabel);
            ForeColor = System.Drawing.SystemColors.Control;
            FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "CreatePlaylistArchiveDialog";
            StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            Text = "Create Playlist Archive";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private System.Windows.Forms.Label StatusLabel;
        private System.Windows.Forms.Button GenericButton_1;
        private System.Windows.Forms.TextBox PathBox;
        private System.Windows.Forms.Button StartOpButton;
        private System.Windows.Forms.ProgressBar ArchiveProgressBar;
        private System.Windows.Forms.Button AdditonalAttributesButton;
    }
}