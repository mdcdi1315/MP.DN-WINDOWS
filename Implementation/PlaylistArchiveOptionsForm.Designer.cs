namespace MP
{
    partial class PlaylistArchiveOptionsForm
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
            AttributesBox = new System.Windows.Forms.ComboBox();
            ConfirmButton = new System.Windows.Forms.Button();
            AttribValBox = new System.Windows.Forms.TextBox();
            UpdateAttribValueButton = new System.Windows.Forms.Button();
            GiveDateInAttribValueButton = new System.Windows.Forms.Button();
            AttribDescLabel = new System.Windows.Forms.Label();
            CustomAttributeButton = new System.Windows.Forms.Button();
            ShouldBecomeReadOnlyAttributeChkBox = new System.Windows.Forms.CheckBox();
            SuspendLayout();
            // 
            // L1
            // 
            L1.AutoSize = true;
            L1.Location = new System.Drawing.Point(23, 9);
            L1.Name = "L1";
            L1.Size = new System.Drawing.Size(0, 15);
            L1.TabIndex = 0;
            // 
            // AttributesBox
            // 
            AttributesBox.BackColor = System.Drawing.SystemColors.WindowText;
            AttributesBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            AttributesBox.ForeColor = System.Drawing.SystemColors.Window;
            AttributesBox.FormattingEnabled = true;
            AttributesBox.Location = new System.Drawing.Point(25, 69);
            AttributesBox.Name = "AttributesBox";
            AttributesBox.Size = new System.Drawing.Size(588, 23);
            AttributesBox.TabIndex = 1;
            AttributesBox.SelectedIndexChanged += ATTRIBUTECHANGED;
            // 
            // ConfirmButton
            // 
            ConfirmButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            ConfirmButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            ConfirmButton.Location = new System.Drawing.Point(556, 234);
            ConfirmButton.Name = "ConfirmButton";
            ConfirmButton.Size = new System.Drawing.Size(82, 23);
            ConfirmButton.TabIndex = 2;
            ConfirmButton.Text = "Confirm...";
            ConfirmButton.UseVisualStyleBackColor = true;
            // 
            // AttribValBox
            // 
            AttribValBox.BackColor = System.Drawing.SystemColors.WindowText;
            AttribValBox.ForeColor = System.Drawing.SystemColors.Window;
            AttribValBox.Location = new System.Drawing.Point(25, 186);
            AttribValBox.Name = "AttribValBox";
            AttribValBox.Size = new System.Drawing.Size(519, 23);
            AttribValBox.TabIndex = 3;
            // 
            // UpdateAttribValueButton
            // 
            UpdateAttribValueButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            UpdateAttribValueButton.Location = new System.Drawing.Point(556, 186);
            UpdateAttribValueButton.Name = "UpdateAttribValueButton";
            UpdateAttribValueButton.Size = new System.Drawing.Size(82, 32);
            UpdateAttribValueButton.TabIndex = 4;
            UpdateAttribValueButton.Text = "Update...";
            UpdateAttribValueButton.UseVisualStyleBackColor = true;
            UpdateAttribValueButton.Click += UPDATEATTRVAL;
            // 
            // GiveDateInAttribValueButton
            // 
            GiveDateInAttribValueButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            GiveDateInAttribValueButton.Location = new System.Drawing.Point(25, 234);
            GiveDateInAttribValueButton.Name = "GiveDateInAttribValueButton";
            GiveDateInAttribValueButton.Size = new System.Drawing.Size(225, 23);
            GiveDateInAttribValueButton.TabIndex = 5;
            GiveDateInAttribValueButton.Text = "Use Current Date as Attribute Value...";
            GiveDateInAttribValueButton.UseVisualStyleBackColor = true;
            GiveDateInAttribValueButton.Click += GIVEDATE;
            // 
            // AttribDescLabel
            // 
            AttribDescLabel.AutoSize = true;
            AttribDescLabel.Location = new System.Drawing.Point(23, 107);
            AttribDescLabel.Name = "AttribDescLabel";
            AttribDescLabel.Size = new System.Drawing.Size(0, 15);
            AttribDescLabel.TabIndex = 6;
            // 
            // CustomAttributeButton
            // 
            CustomAttributeButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            CustomAttributeButton.Location = new System.Drawing.Point(272, 234);
            CustomAttributeButton.Name = "CustomAttributeButton";
            CustomAttributeButton.Size = new System.Drawing.Size(149, 23);
            CustomAttributeButton.TabIndex = 7;
            CustomAttributeButton.Text = "New Custom Attribute...";
            CustomAttributeButton.UseVisualStyleBackColor = true;
            CustomAttributeButton.Click += NEWCUSTOMATTR;
            // 
            // ShouldBecomeReadOnlyAttributeChkBox
            // 
            ShouldBecomeReadOnlyAttributeChkBox.AutoSize = true;
            ShouldBecomeReadOnlyAttributeChkBox.Location = new System.Drawing.Point(28, 153);
            ShouldBecomeReadOnlyAttributeChkBox.Name = "ShouldBecomeReadOnlyAttributeChkBox";
            ShouldBecomeReadOnlyAttributeChkBox.Size = new System.Drawing.Size(15, 14);
            ShouldBecomeReadOnlyAttributeChkBox.TabIndex = 8;
            ShouldBecomeReadOnlyAttributeChkBox.UseVisualStyleBackColor = true;
            // 
            // PlaylistArchiveOptionsForm
            // 
            AcceptButton = ConfirmButton;
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            BackColor = System.Drawing.SystemColors.ControlText;
            ClientSize = new System.Drawing.Size(650, 269);
            Controls.Add(ShouldBecomeReadOnlyAttributeChkBox);
            Controls.Add(CustomAttributeButton);
            Controls.Add(AttribDescLabel);
            Controls.Add(GiveDateInAttribValueButton);
            Controls.Add(UpdateAttribValueButton);
            Controls.Add(AttribValBox);
            Controls.Add(ConfirmButton);
            Controls.Add(AttributesBox);
            Controls.Add(L1);
            ForeColor = System.Drawing.SystemColors.Control;
            FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "PlaylistArchiveOptionsForm";
            ShowIcon = false;
            StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            Text = "Additional Options...";
            Load += F_LOAD;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private System.Windows.Forms.Label L1;
        private System.Windows.Forms.ComboBox AttributesBox;
        private System.Windows.Forms.Button ConfirmButton;
        private System.Windows.Forms.TextBox AttribValBox;
        private System.Windows.Forms.Button UpdateAttribValueButton;
        private System.Windows.Forms.Button GiveDateInAttribValueButton;
        private System.Windows.Forms.Label AttribDescLabel;
        private System.Windows.Forms.Button CustomAttributeButton;
        private System.Windows.Forms.CheckBox ShouldBecomeReadOnlyAttributeChkBox;
    }
}