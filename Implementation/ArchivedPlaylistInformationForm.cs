using System;
using static MP.Settings;
using System.Windows.Forms;
using DotNetResourcesExtensions;

namespace MP
{
    internal partial class ArchivedPlaylistInformationForm : Form
    {
        private System.IO.Stream imgstream;
        private PlaylistPreferences preferences;
        private ArchivedPlaylistAttributeCollection attributes;

        public ArchivedPlaylistInformationForm(ArchivedTrackPlaylist plt)
        {
            if (plt is null) { throw new ArgumentNullException(nameof(plt)); }
            if (plt.Attributes is null)
            {
                MusicPlayerHelper.ShowErrorResourceMessage("ArchivedPlaylistInformationForm_NoAttributesFound");
                DialogResult = DialogResult.Abort;
                return;
            }
            imgstream = plt.PlaylistImageData;
            preferences = plt.Preferences;
            attributes = plt.Attributes;
            InitializeComponent();
        }

        private void F_LOAD(object sender, EventArgs e)
        {
            AdditionalItemsLabel.Text = Global.Resources.GetStringResource("ArchivedPlaylistInformationForm_AdditionalInfoLabelText");
            Microsoft.IO.MemoryStream ts = null;
            try {
                ts = new();
                try { imgstream.Position = 0; } catch { }
                imgstream.DirectCopyToStream(ts);
                ts.Position = 0;
                PlaylistCoverBox.Image = new System.Drawing.Bitmap(ts , true);
            } catch {
                try {
                    ts.Dispose();
                    ts = new(Global.Resources.GetByteArrayResource("CoverImageNotFound"));
                    ts.Position = 0;
                    PlaylistCoverBox.Image = new System.Drawing.Bitmap(ts, true);
                } catch { }
            } finally { 
                ts?.Dispose();
                ts = null;
            }
            imgstream = null;
            AdditionalAttributesBox.BeginUpdate();
            System.String s1 = null , s2 = null , s3 = null;
            foreach (var attr in attributes)
            {
                if (attr.Name == "Creator") { s1 = attr.Value; continue; }
                if (attr.Name == "CreationTime") { s2 = attr.Value; continue; }
                if (attr.Name == "Name") { s3 = attr.Value; continue; }
                AdditionalAttributesBox.Items.Add($"[Attribute] {attr.Name}: {attr.Value ?? "(Null)"}");
            }
            for (System.Int32 I = 0; I < preferences.Count; I++) 
            {
                AdditionalAttributesBox.Items.Add($"[Preference] {preferences[I].Name}: {preferences[I].Value ?? "(Null)"}");
            }
            AdditionalAttributesBox.EndUpdate();
            BasicInformationLabel.Text = System.String.Format(Global.Resources.GetStringResource("ArchivedPlaylistInformationForm_PrimaryData"), s3, s1, s2);
            s1 = s2 = s3 = null;
        }

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (components is not null) {
                    components.Dispose();
                    components = null;
                }
                preferences = null;
                attributes = null;
                imgstream = null;
            }
            base.Dispose(disposing);
        }
    }
}
