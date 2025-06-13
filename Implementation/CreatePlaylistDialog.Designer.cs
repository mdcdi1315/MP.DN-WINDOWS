namespace MP
{
    partial class CreatePlaylistDialog
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
            NameBox = new System.Windows.Forms.TextBox();
            OpenAfterCreation = new System.Windows.Forms.CheckBox();
            ConfirmButton = new System.Windows.Forms.Button();
            CButton = new System.Windows.Forms.Button();
            SuspendLayout();
            // 
            // L1
            // 
            L1.AutoSize = true;
            L1.ForeColor = System.Drawing.Color.White;
            L1.Location = new System.Drawing.Point(9, 10);
            L1.Name = "L1";
            L1.Size = new System.Drawing.Size(389, 30);
            L1.TabIndex = 0;
            L1.Text = "You have been reached here after selecting the files to add in the playlist.\r\nPlease supply playlist name:";
            // 
            // NameBox
            // 
            NameBox.BackColor = System.Drawing.Color.Black;
            NameBox.ForeColor = System.Drawing.Color.White;
            NameBox.Location = new System.Drawing.Point(9, 52);
            NameBox.Name = "NameBox";
            NameBox.Size = new System.Drawing.Size(389, 23);
            NameBox.TabIndex = 1;
            // 
            // OpenAfterCreation
            // 
            OpenAfterCreation.AutoSize = true;
            OpenAfterCreation.ForeColor = System.Drawing.Color.White;
            OpenAfterCreation.Location = new System.Drawing.Point(9, 101);
            OpenAfterCreation.Name = "OpenAfterCreation";
            OpenAfterCreation.Size = new System.Drawing.Size(188, 19);
            OpenAfterCreation.TabIndex = 2;
            OpenAfterCreation.Text = "Open after successfull creation";
            OpenAfterCreation.UseVisualStyleBackColor = true;
            // 
            // ConfirmButton
            // 
            ConfirmButton.BackColor = System.Drawing.Color.Black;
            ConfirmButton.ForeColor = System.Drawing.Color.White;
            ConfirmButton.Location = new System.Drawing.Point(290, 94);
            ConfirmButton.Name = "ConfirmButton";
            ConfirmButton.Size = new System.Drawing.Size(108, 23);
            ConfirmButton.TabIndex = 3;
            ConfirmButton.Text = "O&K";
            ConfirmButton.UseVisualStyleBackColor = false;
            ConfirmButton.Click += ConfirmButton_Click;
            // 
            // CButton
            // 
            CButton.BackColor = System.Drawing.Color.Black;
            CButton.ForeColor = System.Drawing.Color.White;
            CButton.Location = new System.Drawing.Point(290, 123);
            CButton.Name = "CButton";
            CButton.Size = new System.Drawing.Size(108, 23);
            CButton.TabIndex = 4;
            CButton.Text = "&Cancel";
            CButton.UseVisualStyleBackColor = false;
            CButton.Click += CancelButton_Click;
            // 
            // CreatePlaylistDialog
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            BackColor = System.Drawing.Color.Black;
            ClientSize = new System.Drawing.Size(421, 162);
            Controls.Add(CButton);
            Controls.Add(ConfirmButton);
            Controls.Add(OpenAfterCreation);
            Controls.Add(NameBox);
            Controls.Add(L1);
            FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "CreatePlaylistDialog";
            ShowIcon = false;
            StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            Text = "Create Playlist Dialog";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private System.Windows.Forms.Label L1;
        private System.Windows.Forms.TextBox NameBox;
        private System.Windows.Forms.CheckBox OpenAfterCreation;
        private System.Windows.Forms.Button ConfirmButton;
        private System.Windows.Forms.Button CButton;
    }
}