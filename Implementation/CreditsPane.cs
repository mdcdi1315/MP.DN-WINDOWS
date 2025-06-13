using System;
using static MP.Settings;
using System.Windows.Forms;

namespace MP
{
    internal partial class CreditsPane : Form
    {
        private System.UInt16 liclines;
        private System.String allocatedlic;

        public CreditsPane()
        {
            InitializeComponent();
        }

        private void F_LOAD(object sender, EventArgs e)
        {
            L1.Text = Global.Resources.GetStringResource("CreditsPane_HeaderText");
            if (allocatedlic is null)
            {
                try
                {
                    allocatedlic = Global.Resources.GetStringResource("AttributionFile");
                    System.Int32 I = 0;
                    while (I < allocatedlic.Length)
                    {
                        if (allocatedlic[I] == '\n') { liclines++; }
                        I++;
                    }
                    CreditsTextScrollBar.Maximum = liclines - 10;
                    CreditsTextScrollBar.Value = 0;
                    CreditsTextScrollBar.Minimum = 0;
                }
                catch { allocatedlic = Global.Resources.GetStringResource("Error_AttributionStreamNotLoaded"); }
            }
        }

        private void F_CLOSING(object sender, FormClosingEventArgs e)
        {
            if (allocatedlic is not null)
            {
                allocatedlic = null;
            }
        }

        private System.UInt16 FindCharacter(System.Char ch, System.UInt16 times)
        {
            System.UInt16 tf = 0;
            for (System.Int32 I = 0; I < allocatedlic.Length; I++)
            {
                if (allocatedlic[I] == ch)
                {
                    if (tf < times)
                    {
                        tf++;
                    }
                    else
                    {
                        return (System.UInt16)I;
                    }
                }
            }
            return 0;
        }

        private void ScrollBar_ValueChanged(object sender, EventArgs e)
        {
            if (allocatedlic is null) { return; }
            if (CreditsTextScrollBar.Value == 0)
            {
                using (var gs = CreditsTextPanel.CreateGraphics())
                {
                    gs.Clear(CreditsTextPanel.BackColor);
                    gs.DrawString(allocatedlic, CreditsTextPanel.Font,
                        new System.Drawing.SolidBrush(CreditsTextPanel.ForeColor),
                        CreditsTextPanel.ClientRectangle);
                }
                return;
            }
            System.UInt16 fc = FindCharacter('\n', CreditsTextScrollBar.Value.ToUInt16());
            using (var gs = CreditsTextPanel.CreateGraphics())
            {
                gs.Clear(CreditsTextPanel.BackColor);
                gs.DrawString(allocatedlic.Substring(fc + 1), CreditsTextPanel.Font,
                    new System.Drawing.SolidBrush(CreditsTextPanel.ForeColor),
                    CreditsTextPanel.ClientRectangle);
            }
        }

        private void F_SHOWN(object sender, EventArgs e)
        {
            using (var gs = CreateGraphics())
            using (var loaded = Global.Resources.LoadIconResourceAndOrdinal("MusicPlayerIconFull", 0))
            {
                gs.DrawImage(loaded, new System.Drawing.Rectangle(0, 0, 75, 75));
            }
            using (var gs = CreditsTextPanel.CreateGraphics())
            {
                gs.Clear(CreditsTextPanel.BackColor);
                gs.DrawString(allocatedlic, CreditsTextPanel.Font,
                    new System.Drawing.SolidBrush(CreditsTextPanel.ForeColor),
                    CreditsTextPanel.DisplayRectangle);
            }
            Update();
        }
    }
}
