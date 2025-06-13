namespace MP
{
    partial class StatisticsViewer
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
            StatSelectionBox = new System.Windows.Forms.ComboBox();
            L1 = new System.Windows.Forms.Label();
            L2 = new System.Windows.Forms.Label();
            ValueBox = new System.Windows.Forms.TextBox();
            ExitButton = new System.Windows.Forms.Button();
            ClearValueButton = new System.Windows.Forms.Button();
            SuspendLayout();
            // 
            // StatSelectionBox
            // 
            StatSelectionBox.BackColor = System.Drawing.SystemColors.ControlText;
            StatSelectionBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            StatSelectionBox.ForeColor = System.Drawing.SystemColors.Control;
            StatSelectionBox.FormattingEnabled = true;
            StatSelectionBox.Location = new System.Drawing.Point(30, 49);
            StatSelectionBox.Name = "StatSelectionBox";
            StatSelectionBox.Size = new System.Drawing.Size(575, 23);
            StatSelectionBox.TabIndex = 0;
            StatSelectionBox.SelectedIndexChanged += StatSelectionBox_ItemChange;
            // 
            // L1
            // 
            L1.AutoSize = true;
            L1.Location = new System.Drawing.Point(30, 9);
            L1.Name = "L1";
            L1.Size = new System.Drawing.Size(405, 30);
            L1.TabIndex = 1;
            L1.Text = "Statistics viewer. Please select a statistic from the below list to see it's value. \r\n You may also clear it's value, if that is allowed.";
            // 
            // L2
            // 
            L2.AutoSize = true;
            L2.Location = new System.Drawing.Point(29, 115);
            L2.Name = "L2";
            L2.Size = new System.Drawing.Size(82, 15);
            L2.TabIndex = 2;
            L2.Text = "Statistic Value:";
            // 
            // ValueBox
            // 
            ValueBox.BackColor = System.Drawing.SystemColors.ControlText;
            ValueBox.ForeColor = System.Drawing.SystemColors.Control;
            ValueBox.Location = new System.Drawing.Point(31, 137);
            ValueBox.Name = "ValueBox";
            ValueBox.ReadOnly = true;
            ValueBox.Size = new System.Drawing.Size(574, 23);
            ValueBox.TabIndex = 3;
            // 
            // ExitButton
            // 
            ExitButton.BackColor = System.Drawing.SystemColors.ControlText;
            ExitButton.FlatAppearance.MouseDownBackColor = System.Drawing.Color.DarkBlue;
            ExitButton.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Goldenrod;
            ExitButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            ExitButton.ForeColor = System.Drawing.SystemColors.Control;
            ExitButton.Location = new System.Drawing.Point(530, 227);
            ExitButton.Name = "ExitButton";
            ExitButton.Size = new System.Drawing.Size(75, 23);
            ExitButton.TabIndex = 4;
            ExitButton.Text = "Exit...";
            ExitButton.UseVisualStyleBackColor = false;
            ExitButton.Click += F_EXIT;
            // 
            // ClearValueButton
            // 
            ClearValueButton.FlatAppearance.MouseDownBackColor = System.Drawing.Color.DarkBlue;
            ClearValueButton.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Goldenrod;
            ClearValueButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            ClearValueButton.Location = new System.Drawing.Point(31, 227);
            ClearValueButton.Name = "ClearValueButton";
            ClearValueButton.Size = new System.Drawing.Size(89, 23);
            ClearValueButton.TabIndex = 5;
            ClearValueButton.Text = "Clear Value...";
            ClearValueButton.UseVisualStyleBackColor = true;
            ClearValueButton.Click += F_ClearStatValue;
            // 
            // StatisticsViewer
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            BackColor = System.Drawing.SystemColors.ControlText;
            ClientSize = new System.Drawing.Size(640, 272);
            Controls.Add(ClearValueButton);
            Controls.Add(ExitButton);
            Controls.Add(ValueBox);
            Controls.Add(L2);
            Controls.Add(L1);
            Controls.Add(StatSelectionBox);
            ForeColor = System.Drawing.SystemColors.Control;
            FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "StatisticsViewer";
            ShowIcon = false;
            StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            Text = "Music Player Statistics Viewer";
            Load += F_LOAD;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private System.Windows.Forms.ComboBox StatSelectionBox;
        private System.Windows.Forms.Label L1;
        private System.Windows.Forms.Label L2;
        private System.Windows.Forms.TextBox ValueBox;
        private System.Windows.Forms.Button ExitButton;
        private System.Windows.Forms.Button ClearValueButton;
    }
}