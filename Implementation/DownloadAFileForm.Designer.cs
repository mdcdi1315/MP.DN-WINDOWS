namespace MP
{
    partial class DownloadAFileForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DownloadAFileForm));
            ModeSelectionBox = new System.Windows.Forms.ComboBox();
            L1 = new System.Windows.Forms.Label();
            URLBox = new System.Windows.Forms.TextBox();
            L2 = new System.Windows.Forms.Label();
            CButton = new System.Windows.Forms.Button();
            ConfirmButton = new System.Windows.Forms.Button();
            DownloadProgressBar = new System.Windows.Forms.ProgressBar();
            StatusText = new System.Windows.Forms.Label();
            SuspendLayout();
            // 
            // ModeSelectionBox
            // 
            ModeSelectionBox.BackColor = System.Drawing.SystemColors.WindowText;
            ModeSelectionBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            ModeSelectionBox.ForeColor = System.Drawing.SystemColors.Window;
            ModeSelectionBox.FormattingEnabled = true;
            resources.ApplyResources(ModeSelectionBox, "ModeSelectionBox");
            ModeSelectionBox.Name = "ModeSelectionBox";
            ModeSelectionBox.SelectedIndexChanged += DDF_SelectionModeChanged;
            // 
            // L1
            // 
            resources.ApplyResources(L1, "L1");
            L1.Name = "L1";
            // 
            // URLBox
            // 
            URLBox.BackColor = System.Drawing.SystemColors.WindowText;
            URLBox.ForeColor = System.Drawing.SystemColors.Window;
            resources.ApplyResources(URLBox, "URLBox");
            URLBox.Name = "URLBox";
            // 
            // L2
            // 
            resources.ApplyResources(L2, "L2");
            L2.Name = "L2";
            // 
            // CButton
            // 
            CButton.BackColor = System.Drawing.SystemColors.ControlText;
            CButton.ForeColor = System.Drawing.SystemColors.Control;
            resources.ApplyResources(CButton, "CButton");
            CButton.Name = "CButton";
            CButton.UseVisualStyleBackColor = false;
            CButton.Click += CancelButton_Click;
            // 
            // ConfirmButton
            // 
            ConfirmButton.BackColor = System.Drawing.SystemColors.ControlText;
            resources.ApplyResources(ConfirmButton, "ConfirmButton");
            ConfirmButton.Name = "ConfirmButton";
            ConfirmButton.UseVisualStyleBackColor = false;
            ConfirmButton.Click += ConfirmButton_Click;
            // 
            // DownloadProgressBar
            // 
            DownloadProgressBar.BackColor = System.Drawing.SystemColors.ControlText;
            DownloadProgressBar.ForeColor = System.Drawing.SystemColors.Control;
            resources.ApplyResources(DownloadProgressBar, "DownloadProgressBar");
            DownloadProgressBar.MarqueeAnimationSpeed = 10;
            DownloadProgressBar.Name = "DownloadProgressBar";
            DownloadProgressBar.Step = 1;
            DownloadProgressBar.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
            // 
            // StatusText
            // 
            resources.ApplyResources(StatusText, "StatusText");
            StatusText.Name = "StatusText";
            // 
            // DownloadAFileForm
            // 
            AllowDrop = true;
            resources.ApplyResources(this, "$this");
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            BackColor = System.Drawing.SystemColors.ControlText;
            Controls.Add(StatusText);
            Controls.Add(DownloadProgressBar);
            Controls.Add(ConfirmButton);
            Controls.Add(CButton);
            Controls.Add(L2);
            Controls.Add(URLBox);
            Controls.Add(L1);
            Controls.Add(ModeSelectionBox);
            ForeColor = System.Drawing.SystemColors.Control;
            FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            Name = "DownloadAFileForm";
            Load += DDF_F_LOAD;
            DragDrop += DDF_DragDrop;
            DragOver += DDF_DragOver;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private System.Windows.Forms.ComboBox ModeSelectionBox;
        private System.Windows.Forms.Label L1;
        private System.Windows.Forms.TextBox URLBox;
        private System.Windows.Forms.Label L2;
        private System.Windows.Forms.Button CButton;
        private System.Windows.Forms.Button ConfirmButton;
        private System.Windows.Forms.ProgressBar DownloadProgressBar;
        private System.Windows.Forms.Label StatusText;
    }
}