namespace MP
{
    partial class SelectDeviceWindow
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
            DevSelectionBox = new System.Windows.Forms.ComboBox();
            CButton = new System.Windows.Forms.Button();
            OKSelButton = new System.Windows.Forms.Button();
            SuspendLayout();
            // 
            // L1
            // 
            L1.AutoSize = true;
            L1.Location = new System.Drawing.Point(16, 5);
            L1.Name = "L1";
            L1.Size = new System.Drawing.Size(14, 15);
            L1.TabIndex = 0;
            L1.Text = "d";
            // 
            // DevSelectionBox
            // 
            DevSelectionBox.BackColor = System.Drawing.SystemColors.ControlText;
            DevSelectionBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            DevSelectionBox.FlatStyle = System.Windows.Forms.FlatStyle.System;
            DevSelectionBox.ForeColor = System.Drawing.SystemColors.Window;
            DevSelectionBox.FormattingEnabled = true;
            DevSelectionBox.Location = new System.Drawing.Point(20, 71);
            DevSelectionBox.Name = "DevSelectionBox";
            DevSelectionBox.Size = new System.Drawing.Size(439, 23);
            DevSelectionBox.TabIndex = 1;
            // 
            // CButton
            // 
            CButton.BackColor = System.Drawing.SystemColors.ControlText;
            CButton.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(0, 192, 192);
            CButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            CButton.Location = new System.Drawing.Point(361, 143);
            CButton.Name = "CButton";
            CButton.Size = new System.Drawing.Size(69, 25);
            CButton.TabIndex = 2;
            CButton.Text = "Cancel...";
            CButton.UseVisualStyleBackColor = false;
            CButton.Click += CButton_Click;
            // 
            // OKSelButton
            // 
            OKSelButton.BackColor = System.Drawing.SystemColors.ControlText;
            OKSelButton.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(0, 192, 192);
            OKSelButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            OKSelButton.Location = new System.Drawing.Point(447, 143);
            OKSelButton.Name = "OKSelButton";
            OKSelButton.Size = new System.Drawing.Size(36, 25);
            OKSelButton.TabIndex = 3;
            OKSelButton.Text = "&OK";
            OKSelButton.UseVisualStyleBackColor = false;
            OKSelButton.Click += OKSelButton_Click;
            // 
            // SelectDeviceWindow_New
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            BackColor = System.Drawing.SystemColors.ControlText;
            ClientSize = new System.Drawing.Size(507, 185);
            Controls.Add(OKSelButton);
            Controls.Add(CButton);
            Controls.Add(DevSelectionBox);
            Controls.Add(L1);
            ForeColor = System.Drawing.SystemColors.Control;
            FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "SelectDeviceWindow_New";
            ShowIcon = false;
            ShowInTaskbar = false;
            StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            Text = "Device Selection Required";
            Load += SelectDeviceWindow_Load;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private System.Windows.Forms.Label L1;
        private System.Windows.Forms.ComboBox DevSelectionBox;
        private System.Windows.Forms.Button CButton;
        private System.Windows.Forms.Button OKSelButton;
    }
}