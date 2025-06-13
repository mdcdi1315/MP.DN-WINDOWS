namespace MP
{
    partial class CreditsPane
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
            L1 = new System.Windows.Forms.Label();
            CreditsTextPanel = new System.Windows.Forms.Panel();
            CreditsTextScrollBar = new System.Windows.Forms.VScrollBar();
            SuspendLayout();
            // 
            // L1
            // 
            L1.AutoSize = true;
            L1.Location = new System.Drawing.Point(98, 9);
            L1.Name = "L1";
            L1.Size = new System.Drawing.Size(0, 15);
            L1.TabIndex = 0;
            // 
            // CreditsTextPanel
            // 
            CreditsTextPanel.Location = new System.Drawing.Point(33, 81);
            CreditsTextPanel.Name = "CreditsTextPanel";
            CreditsTextPanel.Size = new System.Drawing.Size(600, 289);
            CreditsTextPanel.TabIndex = 1;
            // 
            // CreditsTextScrollBar
            // 
            CreditsTextScrollBar.Location = new System.Drawing.Point(636, 81);
            CreditsTextScrollBar.Name = "CreditsTextScrollBar";
            CreditsTextScrollBar.Size = new System.Drawing.Size(22, 289);
            CreditsTextScrollBar.TabIndex = 0;
            CreditsTextScrollBar.ValueChanged += ScrollBar_ValueChanged;
            // 
            // CreditsPane
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            BackColor = System.Drawing.SystemColors.WindowText;
            ClientSize = new System.Drawing.Size(690, 405);
            Controls.Add(CreditsTextScrollBar);
            Controls.Add(CreditsTextPanel);
            Controls.Add(L1);
            ForeColor = System.Drawing.SystemColors.Window;
            FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "CreditsPane";
            StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            Text = "Credits/Acknowledgements";
            FormClosing += F_CLOSING;
            Load += F_LOAD;
            Shown += F_SHOWN;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private System.Windows.Forms.Label L1;
        private System.Windows.Forms.Panel CreditsTextPanel;
        private System.Windows.Forms.VScrollBar CreditsTextScrollBar;
    }
}