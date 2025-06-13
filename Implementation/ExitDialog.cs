using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MP.GamepadBackend;
using System.Windows.Forms;

namespace MP
{
    internal partial class ExitDialog : ControllerCompatibleForm
    {
        private System.Boolean last_opt1;

        public ExitDialog(System.Boolean isvalidandphysplaylist, GamepadReader reader) : base(reader)
        {
            if (isvalidandphysplaylist) {
                last_opt1 = Settings.Global.InitializeLastTrackFromLastPlaylist;
            } else {
                last_opt1 = false;
            }
            InitializeComponent();
            if (isvalidandphysplaylist == false) { ContinuePlaybackOpt.Visible = false; }
        }

        private void Update_Value_OPT1(object sender, EventArgs e)
        {
            Settings.Global.InitializeLastTrackFromLastPlaylist = ContinuePlaybackOpt.Checked;
        }

        private void F_CLOSING(object sender, FormClosingEventArgs e)
        {
            if (DialogResult == DialogResult.Cancel)
            {
                Settings.Global.InitializeLastTrackFromLastPlaylist = last_opt1;
            }
        }

        private void F_LOAD(object sender, EventArgs e)
        {
        }
    }
}
