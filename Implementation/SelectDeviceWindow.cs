using System;
using System.Windows.Forms;
using MP.AudioLibrary.MMDevice;
using System.Collections.Generic;

namespace MP
{
    public partial class SelectDeviceWindow : Form
    {
        private System.SByte selectedindex;
        private List<MMDevice> collection;

        public SelectDeviceWindow(IEnumerable<MMDevice> coll)
        {
            selectedindex = -1;
            collection = new(coll);
            InitializeComponent();
            L1.Text = Settings.Global.Resources.GetStringResource("SelectDeviceWindow_SelectDevText");
        }

        public MMDevice SelectedDevice => collection[selectedindex];

        public System.SByte SelectedIndex => selectedindex;

        private void CButton_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private void OKSelButton_Click(object sender, EventArgs e)
        {
            selectedindex = (System.SByte)DevSelectionBox.SelectedIndex;
            if (selectedindex < 0)
            {
                MusicPlayerHelper.ShowErrorResourceMessage("SelectDeviceWindow_NoDevSelected");
                return;
            }
            DialogResult = DialogResult.OK;
            Close();
        }

        private void SelectDeviceWindow_Load(object sender, EventArgs e)
        {
            foreach (var device in collection)
            {
                DevSelectionBox.Items.Add(device.FriendlyName);
            }
        }

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (collection is not null)
                {
                    for (System.Int32 I = 0; I < collection.Count; I++)
                    {
                        if (I == selectedindex) { continue; }
                        collection[I].Dispose();
                    }
                    collection = null;
                }
                if (components is not null)
                {
                    components.Dispose();
                    components = null;
                }
            }
            base.Dispose(disposing);
        }
    }
}
