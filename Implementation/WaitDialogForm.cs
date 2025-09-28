
using System;
using System.Windows.Forms;

namespace MP
{
    public partial class WaitDialogForm : Form
    {
        public WaitDialogForm()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Gets or sets the specific wait message to display on the client. <br />
        /// Thread-safe.
        /// </summary>
        public new System.String Text
        {
            get => TextDataLabel.Text;
            set {
                if (value is null) { return; }
                // Add some spaces to the string, will help to pad appropriately the form
                System.String ft = System.String.Concat(value, "   ");
                if (IsHandleCreated) {
                    Invoke(new Action<System.String>(UpdateTextLabelDataAction), ft);
                } else {
                    UpdateTextLabelDataAction(ft);
                }
            }
        }

        private void UpdateTextLabelDataAction(System.String str) => TextDataLabel.Text = str;
    }
}
