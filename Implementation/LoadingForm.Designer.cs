namespace MP
{
    partial class LoadingForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(LoadingForm));
            AppPictureBox = new System.Windows.Forms.PictureBox();
            L1 = new System.Windows.Forms.Label();
            L2 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)AppPictureBox).BeginInit();
            SuspendLayout();
            // 
            // AppPictureBox
            // 
            AppPictureBox.Dock = System.Windows.Forms.DockStyle.Left;
            AppPictureBox.Location = new System.Drawing.Point(0, 0);
            AppPictureBox.Name = "AppPictureBox";
            AppPictureBox.Size = new System.Drawing.Size(120, 113);
            AppPictureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            AppPictureBox.TabIndex = 0;
            AppPictureBox.TabStop = false;
            // 
            // L1
            // 
            L1.AutoSize = true;
            L1.Location = new System.Drawing.Point(216, 26);
            L1.Name = "L1";
            L1.Size = new System.Drawing.Size(74, 15);
            L1.TabIndex = 1;
            L1.Text = "Starting up...";
            // 
            // L2
            // 
            L2.AutoSize = true;
            L2.Location = new System.Drawing.Point(151, 73);
            L2.Name = "L2";
            L2.Size = new System.Drawing.Size(192, 15);
            L2.TabIndex = 2;
            L2.Text = "© MDCDI1315. All Rights Reserved.";
            // 
            // LoadingForm
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            BackColor = System.Drawing.SystemColors.ControlText;
            ClientSize = new System.Drawing.Size(397, 113);
            Controls.Add(L2);
            Controls.Add(L1);
            Controls.Add(AppPictureBox);
            ForeColor = System.Drawing.SystemColors.Control;
            FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            Name = "LoadingForm";
            ShowIcon = false;
            StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            Text = "Loading...";
            TopMost = true;
            Load += F_LOAD;
            ((System.ComponentModel.ISupportInitialize)AppPictureBox).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private System.Windows.Forms.PictureBox AppPictureBox;
        private System.Windows.Forms.Label L1;
        private System.Windows.Forms.Label L2;
    }
}