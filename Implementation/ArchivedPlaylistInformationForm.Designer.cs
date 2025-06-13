namespace MP
{
    partial class ArchivedPlaylistInformationForm
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
            PlaylistCoverBox = new System.Windows.Forms.PictureBox();
            BasicInformationLabel = new System.Windows.Forms.Label();
            ExitButton = new System.Windows.Forms.Button();
            AdditionalAttributesBox = new System.Windows.Forms.ListBox();
            AdditionalItemsLabel = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)PlaylistCoverBox).BeginInit();
            SuspendLayout();
            // 
            // PlaylistCoverBox
            // 
            PlaylistCoverBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            PlaylistCoverBox.Location = new System.Drawing.Point(12, 12);
            PlaylistCoverBox.Name = "PlaylistCoverBox";
            PlaylistCoverBox.Size = new System.Drawing.Size(128, 128);
            PlaylistCoverBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            PlaylistCoverBox.TabIndex = 0;
            PlaylistCoverBox.TabStop = false;
            // 
            // BasicInformationLabel
            // 
            BasicInformationLabel.AutoSize = true;
            BasicInformationLabel.Location = new System.Drawing.Point(163, 12);
            BasicInformationLabel.Name = "BasicInformationLabel";
            BasicInformationLabel.Size = new System.Drawing.Size(0, 15);
            BasicInformationLabel.TabIndex = 1;
            // 
            // ExitButton
            // 
            ExitButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            ExitButton.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Teal;
            ExitButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            ExitButton.Location = new System.Drawing.Point(601, 261);
            ExitButton.Name = "ExitButton";
            ExitButton.Size = new System.Drawing.Size(75, 23);
            ExitButton.TabIndex = 2;
            ExitButton.Text = "Exit...";
            ExitButton.UseVisualStyleBackColor = true;
            // 
            // AdditionalAttributesBox
            // 
            AdditionalAttributesBox.BackColor = System.Drawing.SystemColors.WindowText;
            AdditionalAttributesBox.ForeColor = System.Drawing.SystemColors.Window;
            AdditionalAttributesBox.FormattingEnabled = true;
            AdditionalAttributesBox.ItemHeight = 15;
            AdditionalAttributesBox.Location = new System.Drawing.Point(15, 179);
            AdditionalAttributesBox.Name = "AdditionalAttributesBox";
            AdditionalAttributesBox.Size = new System.Drawing.Size(532, 94);
            AdditionalAttributesBox.TabIndex = 3;
            // 
            // AdditionalItemsLabel
            // 
            AdditionalItemsLabel.AutoSize = true;
            AdditionalItemsLabel.Location = new System.Drawing.Point(18, 149);
            AdditionalItemsLabel.Name = "AdditionalItemsLabel";
            AdditionalItemsLabel.Size = new System.Drawing.Size(0, 15);
            AdditionalItemsLabel.TabIndex = 4;
            // 
            // ArchivedPlaylistInformationForm
            // 
            AcceptButton = ExitButton;
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            BackColor = System.Drawing.SystemColors.ControlText;
            ClientSize = new System.Drawing.Size(688, 296);
            Controls.Add(AdditionalItemsLabel);
            Controls.Add(AdditionalAttributesBox);
            Controls.Add(ExitButton);
            Controls.Add(BasicInformationLabel);
            Controls.Add(PlaylistCoverBox);
            ForeColor = System.Drawing.SystemColors.Control;
            FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "ArchivedPlaylistInformationForm";
            ShowIcon = false;
            ShowInTaskbar = false;
            StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            Text = "Playlist Information";
            Load += F_LOAD;
            ((System.ComponentModel.ISupportInitialize)PlaylistCoverBox).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private System.Windows.Forms.PictureBox PlaylistCoverBox;
        private System.Windows.Forms.Label BasicInformationLabel;
        private System.Windows.Forms.Button ExitButton;
        private System.Windows.Forms.ListBox AdditionalAttributesBox;
        private System.Windows.Forms.Label AdditionalItemsLabel;
    }
}