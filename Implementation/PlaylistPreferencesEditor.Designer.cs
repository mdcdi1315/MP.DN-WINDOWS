namespace MP
{
    partial class PlaylistPreferencesEditor
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
            L2 = new System.Windows.Forms.Label();
            OptionBox = new System.Windows.Forms.ComboBox();
            ConfOptionButton = new System.Windows.Forms.Button();
            SelectionDescription = new System.Windows.Forms.Label();
            L3 = new System.Windows.Forms.Label();
            StatusLabel = new System.Windows.Forms.Label();
            ExitButton = new System.Windows.Forms.Button();
            ValueBox_TextBox = new System.Windows.Forms.TextBox();
            BrowseFileButton = new System.Windows.Forms.Button();
            ValueBox_CheckBox = new System.Windows.Forms.CheckBox();
            SuspendLayout();
            // 
            // L1
            // 
            L1.AutoSize = true;
            L1.Location = new System.Drawing.Point(30, 9);
            L1.Name = "L1";
            L1.Size = new System.Drawing.Size(244, 15);
            L1.TabIndex = 0;
            L1.Text = "You are editing the preferences of {0} playlist.";
            // 
            // L2
            // 
            L2.AutoSize = true;
            L2.Location = new System.Drawing.Point(33, 37);
            L2.Name = "L2";
            L2.Size = new System.Drawing.Size(216, 15);
            L2.TabIndex = 1;
            L2.Text = "Select a preference from the box below:";
            // 
            // OptionBox
            // 
            OptionBox.DropDownHeight = 156;
            OptionBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            OptionBox.FormattingEnabled = true;
            OptionBox.IntegralHeight = false;
            OptionBox.Items.AddRange(new object[] { "(No Item Selected)" });
            OptionBox.Location = new System.Drawing.Point(33, 57);
            OptionBox.MaxDropDownItems = 5;
            OptionBox.Name = "OptionBox";
            OptionBox.Size = new System.Drawing.Size(423, 23);
            OptionBox.TabIndex = 2;
            OptionBox.SelectedIndexChanged += UpdateSelectionData;
            // 
            // ConfOptionButton
            // 
            ConfOptionButton.Location = new System.Drawing.Point(30, 206);
            ConfOptionButton.Name = "ConfOptionButton";
            ConfOptionButton.Size = new System.Drawing.Size(110, 23);
            ConfOptionButton.TabIndex = 3;
            ConfOptionButton.Text = "Confirm Option...";
            ConfOptionButton.UseVisualStyleBackColor = true;
            ConfOptionButton.Click += ConfirmOption_Click;
            // 
            // SelectionDescription
            // 
            SelectionDescription.AutoSize = true;
            SelectionDescription.Location = new System.Drawing.Point(37, 85);
            SelectionDescription.Name = "SelectionDescription";
            SelectionDescription.Size = new System.Drawing.Size(145, 15);
            SelectionDescription.TabIndex = 4;
            SelectionDescription.Text = "(No Description Available)";
            // 
            // L3
            // 
            L3.AutoSize = true;
            L3.Location = new System.Drawing.Point(35, 160);
            L3.Name = "L3";
            L3.Size = new System.Drawing.Size(112, 15);
            L3.TabIndex = 5;
            L3.Text = "Select Option Value:";
            // 
            // StatusLabel
            // 
            StatusLabel.AutoSize = true;
            StatusLabel.Location = new System.Drawing.Point(156, 210);
            StatusLabel.Name = "StatusLabel";
            StatusLabel.Size = new System.Drawing.Size(41, 15);
            StatusLabel.TabIndex = 6;
            StatusLabel.Text = "Saved!";
            // 
            // ExitButton
            // 
            ExitButton.Location = new System.Drawing.Point(447, 212);
            ExitButton.Name = "ExitButton";
            ExitButton.Size = new System.Drawing.Size(93, 23);
            ExitButton.TabIndex = 7;
            ExitButton.Text = "Exit...";
            ExitButton.UseVisualStyleBackColor = true;
            ExitButton.Click += E_FORM;
            // 
            // ValueBox_TextBox
            // 
            ValueBox_TextBox.Location = new System.Drawing.Point(30, 178);
            ValueBox_TextBox.Name = "ValueBox_TextBox";
            ValueBox_TextBox.Size = new System.Drawing.Size(380, 23);
            ValueBox_TextBox.TabIndex = 8;
            ValueBox_TextBox.Visible = false;
            // 
            // BrowseFileButton
            // 
            BrowseFileButton.Location = new System.Drawing.Point(426, 177);
            BrowseFileButton.Name = "BrowseFileButton";
            BrowseFileButton.Size = new System.Drawing.Size(75, 23);
            BrowseFileButton.TabIndex = 9;
            BrowseFileButton.Text = "Browse...";
            BrowseFileButton.UseVisualStyleBackColor = true;
            BrowseFileButton.Visible = false;
            BrowseFileButton.Click += BrowseFile_Click;
            // 
            // ValueBox_CheckBox
            // 
            ValueBox_CheckBox.AutoSize = true;
            ValueBox_CheckBox.Location = new System.Drawing.Point(37, 182);
            ValueBox_CheckBox.Name = "ValueBox_CheckBox";
            ValueBox_CheckBox.Size = new System.Drawing.Size(184, 19);
            ValueBox_CheckBox.TabIndex = 10;
            ValueBox_CheckBox.Text = "Toggle or disable this option...";
            ValueBox_CheckBox.UseVisualStyleBackColor = true;
            ValueBox_CheckBox.Visible = false;
            // 
            // PlaylistPreferencesEditor
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(550, 241);
            Controls.Add(ValueBox_CheckBox);
            Controls.Add(BrowseFileButton);
            Controls.Add(ValueBox_TextBox);
            Controls.Add(ExitButton);
            Controls.Add(StatusLabel);
            Controls.Add(L3);
            Controls.Add(SelectionDescription);
            Controls.Add(ConfOptionButton);
            Controls.Add(OptionBox);
            Controls.Add(L2);
            Controls.Add(L1);
            FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "PlaylistPreferencesEditor";
            ShowIcon = false;
            ShowInTaskbar = false;
            StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            Text = "Playlist Preferences Editor";
            Load += F_LOAD;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private System.Windows.Forms.Label L1;
        private System.Windows.Forms.Label L2;
        private System.Windows.Forms.ComboBox OptionBox;
        private System.Windows.Forms.Button ConfOptionButton;
        private System.Windows.Forms.Label SelectionDescription;
        private System.Windows.Forms.Label L3;
        private System.Windows.Forms.Label StatusLabel;
        private System.Windows.Forms.Button ExitButton;
        private System.Windows.Forms.TextBox ValueBox_TextBox;
        private System.Windows.Forms.Button BrowseFileButton;
        private System.Windows.Forms.CheckBox ValueBox_CheckBox;
    }
}