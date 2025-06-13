namespace MP
{
    partial class SettingsEditorNew
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
            SettingsNodesView = new System.Windows.Forms.TreeView();
            ValuesSettingPanel = new System.Windows.Forms.Panel();
            ChooseFileOrFolderButton = new System.Windows.Forms.Button();
            ConfirmSettingValueButton = new System.Windows.Forms.Button();
            SettingValueColorBox = new ColorBox();
            RangeSettingUpDown = new System.Windows.Forms.NumericUpDown();
            StringValueTextBox = new System.Windows.Forms.TextBox();
            ValidValuesBoxForSetting = new System.Windows.Forms.ComboBox();
            SettingDescriptionLabel = new System.Windows.Forms.Label();
            ValuesSettingPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)RangeSettingUpDown).BeginInit();
            SuspendLayout();
            // 
            // SettingsNodesView
            // 
            SettingsNodesView.BackColor = System.Drawing.SystemColors.ControlText;
            SettingsNodesView.Dock = System.Windows.Forms.DockStyle.Left;
            SettingsNodesView.ForeColor = System.Drawing.SystemColors.Control;
            SettingsNodesView.Location = new System.Drawing.Point(0, 0);
            SettingsNodesView.Name = "SettingsNodesView";
            SettingsNodesView.PathSeparator = "/";
            SettingsNodesView.Size = new System.Drawing.Size(255, 411);
            SettingsNodesView.TabIndex = 0;
            SettingsNodesView.AfterSelect += F_NODESEL;
            SettingsNodesView.NodeMouseClick += F_NODECLICK;
            SettingsNodesView.KeyDown += F_TN_KEYDOWN;
            // 
            // ValuesSettingPanel
            // 
            ValuesSettingPanel.Controls.Add(ChooseFileOrFolderButton);
            ValuesSettingPanel.Controls.Add(ConfirmSettingValueButton);
            ValuesSettingPanel.Controls.Add(SettingValueColorBox);
            ValuesSettingPanel.Controls.Add(RangeSettingUpDown);
            ValuesSettingPanel.Controls.Add(StringValueTextBox);
            ValuesSettingPanel.Controls.Add(ValidValuesBoxForSetting);
            ValuesSettingPanel.Controls.Add(SettingDescriptionLabel);
            ValuesSettingPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            ValuesSettingPanel.Location = new System.Drawing.Point(255, 0);
            ValuesSettingPanel.Name = "ValuesSettingPanel";
            ValuesSettingPanel.Size = new System.Drawing.Size(479, 411);
            ValuesSettingPanel.TabIndex = 1;
            // 
            // ChooseFileOrFolderButton
            // 
            ChooseFileOrFolderButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            ChooseFileOrFolderButton.Location = new System.Drawing.Point(223, 244);
            ChooseFileOrFolderButton.Name = "ChooseFileOrFolderButton";
            ChooseFileOrFolderButton.Size = new System.Drawing.Size(117, 23);
            ChooseFileOrFolderButton.TabIndex = 6;
            ChooseFileOrFolderButton.UseVisualStyleBackColor = true;
            ChooseFileOrFolderButton.Click += F_CHOOSEPATH;
            // 
            // ConfirmSettingValueButton
            // 
            ConfirmSettingValueButton.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Blue;
            ConfirmSettingValueButton.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(255, 128, 0);
            ConfirmSettingValueButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            ConfirmSettingValueButton.Location = new System.Drawing.Point(367, 244);
            ConfirmSettingValueButton.Name = "ConfirmSettingValueButton";
            ConfirmSettingValueButton.Size = new System.Drawing.Size(81, 23);
            ConfirmSettingValueButton.TabIndex = 5;
            ConfirmSettingValueButton.Text = "Confirm...";
            ConfirmSettingValueButton.UseVisualStyleBackColor = true;
            ConfirmSettingValueButton.Click += OnSettingConfirmation;
            // 
            // SettingValueColorBox
            // 
            SettingValueColorBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            SettingValueColorBox.DepictedColor = System.Drawing.Color.Empty;
            SettingValueColorBox.FallbackColor = System.Drawing.Color.Black;
            SettingValueColorBox.Location = new System.Drawing.Point(26, 171);
            SettingValueColorBox.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            SettingValueColorBox.Name = "SettingValueColorBox";
            SettingValueColorBox.Size = new System.Drawing.Size(66, 60);
            SettingValueColorBox.TabIndex = 4;
            SettingValueColorBox.Click += F_CB_CLICK;
            // 
            // RangeSettingUpDown
            // 
            RangeSettingUpDown.BackColor = System.Drawing.SystemColors.WindowText;
            RangeSettingUpDown.ForeColor = System.Drawing.SystemColors.Window;
            RangeSettingUpDown.Location = new System.Drawing.Point(27, 171);
            RangeSettingUpDown.Name = "RangeSettingUpDown";
            RangeSettingUpDown.Size = new System.Drawing.Size(120, 23);
            RangeSettingUpDown.TabIndex = 3;
            // 
            // StringValueTextBox
            // 
            StringValueTextBox.BackColor = System.Drawing.SystemColors.WindowText;
            StringValueTextBox.ForeColor = System.Drawing.SystemColors.Window;
            StringValueTextBox.Location = new System.Drawing.Point(26, 172);
            StringValueTextBox.Name = "StringValueTextBox";
            StringValueTextBox.Size = new System.Drawing.Size(421, 23);
            StringValueTextBox.TabIndex = 2;
            // 
            // ValidValuesBoxForSetting
            // 
            ValidValuesBoxForSetting.BackColor = System.Drawing.SystemColors.WindowText;
            ValidValuesBoxForSetting.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            ValidValuesBoxForSetting.ForeColor = System.Drawing.SystemColors.Window;
            ValidValuesBoxForSetting.FormattingEnabled = true;
            ValidValuesBoxForSetting.Location = new System.Drawing.Point(27, 172);
            ValidValuesBoxForSetting.Name = "ValidValuesBoxForSetting";
            ValidValuesBoxForSetting.Size = new System.Drawing.Size(421, 23);
            ValidValuesBoxForSetting.TabIndex = 1;
            // 
            // SettingDescriptionLabel
            // 
            SettingDescriptionLabel.AutoSize = true;
            SettingDescriptionLabel.Location = new System.Drawing.Point(27, 9);
            SettingDescriptionLabel.Name = "SettingDescriptionLabel";
            SettingDescriptionLabel.Size = new System.Drawing.Size(13, 15);
            SettingDescriptionLabel.TabIndex = 0;
            SettingDescriptionLabel.Text = "  ";
            // 
            // SettingsEditorNew
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            BackColor = System.Drawing.Color.Black;
            ClientSize = new System.Drawing.Size(734, 411);
            Controls.Add(ValuesSettingPanel);
            Controls.Add(SettingsNodesView);
            ForeColor = System.Drawing.Color.White;
            FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "SettingsEditorNew";
            StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            Text = "Music Player Settings";
            FormClosing += F_CLOSING;
            Load += F_LOAD;
            Shown += F_SHOWN;
            ValuesSettingPanel.ResumeLayout(false);
            ValuesSettingPanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)RangeSettingUpDown).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.TreeView SettingsNodesView;
        private System.Windows.Forms.Panel ValuesSettingPanel;
        private System.Windows.Forms.ComboBox ValidValuesBoxForSetting;
        private System.Windows.Forms.Label SettingDescriptionLabel;
        private System.Windows.Forms.TextBox StringValueTextBox;
        private ColorBox SettingValueColorBox;
        private System.Windows.Forms.NumericUpDown RangeSettingUpDown;
        private System.Windows.Forms.Button ConfirmSettingValueButton;
        private System.Windows.Forms.Button ChooseFileOrFolderButton;
    }
}