using System;
using static MP.Settings;
using MP.GamepadBackend;
using System.Windows.Forms;


namespace MP
{
    internal partial class ChangeTrackIndexDialog : ControllerCompatibleForm
    {
        private System.Int32 selected , selectedtochange;

        public ChangeTrackIndexDialog(System.String[] titles , System.Int32 tc , GamepadReader rdr) : base(rdr)
        {
            InitializeComponent();
            selected = -1;
            selectedtochange = tc;
            L1.Text = System.String.Format(Global.Resources.GetStringResource("ChangeTrackIndexDialog_Heading"), titles[selectedtochange]);
            try {
                TracksView.BeginUpdate();
                foreach (System.String s in titles) 
                {
                    TracksView.Items.Add(s);
                }
            } catch { } finally { TracksView.EndUpdate(); }
        }

        private void F_CLOSING(object sender, FormClosingEventArgs e)
        {
            if (DialogResult == DialogResult.OK) 
            {
                if (selected <= -1 || selected > TracksView.Items.Count)
                {
                    MusicPlayerHelper.ShowErrorResourceMessage("ChangeTrackIndexDialog_NoItemSelected");
                    e.Cancel = true;
                    return;
                }
                if (MusicPlayerHelper.ShowResourceQuestionMessage("ChangeTrackIndexDialog_Confirmation" , TracksView.Items[selectedtochange] , TracksView.Items[selected]) == false)
                {
                    e.Cancel = true;
                    return;
                }
            }
        }

        private void Selected_Changed(object sender, EventArgs e)
        {
            selected = TracksView.SelectedIndex;
        }

        public System.Int32 SelectedTrack => selected;
    }
}
