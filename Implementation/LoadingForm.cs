using System.Windows.Forms;

namespace MP
{
    public partial class LoadingForm : Form
    {
        private enum LoadingThreadState : System.Byte
        {
            S0,
            S1, 
            S2, 
            S3
        }

        private LoadingThreadState state;

        public LoadingForm()
        {
            InitializeComponent();
            AppPictureBox.Image = Settings.Global.Resources.LoadIconResourceAndOrdinal("ApplicationIcon", 0);
        }

        public void ThreadCode()
        {
            switch (state)
            {
                case LoadingThreadState.S0:
                    L1.Text = "Starting up";
                    break;
                case LoadingThreadState.S1:
                    L1.Text = "Starting up.";
                    break;
                case LoadingThreadState.S2:
                    L1.Text = "Starting up..";
                    break;
                case LoadingThreadState.S3:
                    L1.Text = "Starting up...";
                    break;
            }
            state++;
            if (state > LoadingThreadState.S3) { state = LoadingThreadState.S0; }
        }

        private void F_LOAD(object sender, System.EventArgs e)
        {
            L2.Text += AppInfo.DisplayVersionStringLoadForm;
        }

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
    }
}
