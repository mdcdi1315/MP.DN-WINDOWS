namespace MP
{
    partial class WaitDialogForm
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
            TextDataLabel = new System.Windows.Forms.Label();
            SuspendLayout();
            // 
            // L1
            // 
            L1.AutoSize = true;
            L1.Location = new System.Drawing.Point(10, 10);
            L1.Name = "L1";
            L1.Size = new System.Drawing.Size(124, 15);
            L1.TabIndex = 0;
            L1.Text = "Loading Dialog events";
            // 
            // TextDataLabel
            // 
            TextDataLabel.AutoSize = true;
            TextDataLabel.Location = new System.Drawing.Point(10, 35);
            TextDataLabel.Name = "TextDataLabel";
            TextDataLabel.Size = new System.Drawing.Size(0, 15);
            TextDataLabel.TabIndex = 1;
            // 
            // WaitDialogForm
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            AutoSize = true;
            BackColor = System.Drawing.SystemColors.ControlText;
            ClientSize = new System.Drawing.Size(156, 68);
            ControlBox = false;
            Controls.Add(TextDataLabel);
            Controls.Add(L1);
            ForeColor = System.Drawing.SystemColors.Control;
            FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            Name = "WaitDialogForm";
            ShowIcon = false;
            ShowInTaskbar = false;
            StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private System.Windows.Forms.Label L1;
        private System.Windows.Forms.Label TextDataLabel;
    }
}