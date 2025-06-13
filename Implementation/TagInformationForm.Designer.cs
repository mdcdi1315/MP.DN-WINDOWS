namespace MP
{
    partial class TagInformationForm
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
            TextInfo = new System.Windows.Forms.Label();
            CoverImageBox = new System.Windows.Forms.PictureBox();
            L2 = new System.Windows.Forms.Label();
            ExitButton = new System.Windows.Forms.Button();
            PURLLinkButton = new System.Windows.Forms.Button();
            EURLLinkButton = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)CoverImageBox).BeginInit();
            SuspendLayout();
            // 
            // TextInfo
            // 
            TextInfo.AutoSize = true;
            TextInfo.Location = new System.Drawing.Point(8, 18);
            TextInfo.Name = "TextInfo";
            TextInfo.Size = new System.Drawing.Size(506, 15);
            TextInfo.TabIndex = 0;
            TextInfo.Text = "This window shows some of the information of the data tag contained in the current audio file.";
            // 
            // CoverImageBox
            // 
            CoverImageBox.Location = new System.Drawing.Point(115, 284);
            CoverImageBox.Name = "CoverImageBox";
            CoverImageBox.Size = new System.Drawing.Size(164, 121);
            CoverImageBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            CoverImageBox.TabIndex = 1;
            CoverImageBox.TabStop = false;
            CoverImageBox.Click += CoverImageBox_Export;
            // 
            // L2
            // 
            L2.AutoSize = true;
            L2.Location = new System.Drawing.Point(23, 292);
            L2.Name = "L2";
            L2.Size = new System.Drawing.Size(77, 15);
            L2.TabIndex = 2;
            L2.Text = "Cover Image:";
            // 
            // ExitButton
            // 
            ExitButton.BackColor = System.Drawing.SystemColors.ActiveCaptionText;
            ExitButton.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(0, 0, 192);
            ExitButton.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(0, 64, 0);
            ExitButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            ExitButton.ForeColor = System.Drawing.SystemColors.Control;
            ExitButton.Location = new System.Drawing.Point(342, 284);
            ExitButton.Name = "ExitButton";
            ExitButton.Size = new System.Drawing.Size(138, 23);
            ExitButton.TabIndex = 3;
            ExitButton.Text = "Exit...";
            ExitButton.UseVisualStyleBackColor = false;
            ExitButton.Click += EXB_CLICK;
            // 
            // PURLLinkButton
            // 
            PURLLinkButton.BackColor = System.Drawing.SystemColors.ActiveCaptionText;
            PURLLinkButton.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(0, 0, 192);
            PURLLinkButton.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(0, 64, 0);
            PURLLinkButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            PURLLinkButton.Location = new System.Drawing.Point(342, 368);
            PURLLinkButton.Name = "PURLLinkButton";
            PURLLinkButton.Size = new System.Drawing.Size(138, 23);
            PURLLinkButton.TabIndex = 4;
            PURLLinkButton.Text = "Follow Publisher link...";
            PURLLinkButton.UseVisualStyleBackColor = false;
            PURLLinkButton.Click += PURLLinkButton_Click;
            // 
            // EURLLinkButton
            // 
            EURLLinkButton.BackColor = System.Drawing.SystemColors.ActiveCaptionText;
            EURLLinkButton.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(0, 0, 192);
            EURLLinkButton.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(0, 64, 0);
            EURLLinkButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            EURLLinkButton.ForeColor = System.Drawing.SystemColors.Control;
            EURLLinkButton.Location = new System.Drawing.Point(342, 325);
            EURLLinkButton.Name = "EURLLinkButton";
            EURLLinkButton.Size = new System.Drawing.Size(137, 23);
            EURLLinkButton.TabIndex = 5;
            EURLLinkButton.Text = "Follow Encoder link...";
            EURLLinkButton.UseVisualStyleBackColor = false;
            EURLLinkButton.Click += EURLLinkButton_Click;
            // 
            // TagInformationForm
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            BackColor = System.Drawing.SystemColors.ActiveCaptionText;
            ClientSize = new System.Drawing.Size(531, 413);
            Controls.Add(EURLLinkButton);
            Controls.Add(PURLLinkButton);
            Controls.Add(ExitButton);
            Controls.Add(L2);
            Controls.Add(CoverImageBox);
            Controls.Add(TextInfo);
            ForeColor = System.Drawing.SystemColors.Control;
            FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "TagInformationForm";
            ShowIcon = false;
            ShowInTaskbar = false;
            StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            Text = "Tag Information";
            Load += F_LOAD;
            ((System.ComponentModel.ISupportInitialize)CoverImageBox).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private System.Windows.Forms.Label TextInfo;
        private System.Windows.Forms.PictureBox CoverImageBox;
        private System.Windows.Forms.Label L2;
        private System.Windows.Forms.Button ExitButton;
        private System.Windows.Forms.Button PURLLinkButton;
        private System.Windows.Forms.Button EURLLinkButton;
    }
}