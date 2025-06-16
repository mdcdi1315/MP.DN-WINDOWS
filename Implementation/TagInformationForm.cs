using System;
using Microsoft.IO;
using System.Drawing;
using MP.GamepadBackend;
using System.Windows.Forms;

namespace MP
{
    internal partial class TagInformationForm : ControllerCompatibleForm
    {
        private Microsoft.IO.MemoryStream imgstream;
        private System.String resultdata, purl, eurl;

        public TagInformationForm(SavedDataTag rdr, GamepadReader grdr = null) : base(grdr)
        {
            imgstream = null;
            InitializeComponent();
            resultdata =
             $"\nTitle: {rdr.Title2}\n" +
             $"Contributing Artists: {rdr.ContributingArtists}\n" +
             $"Album Artist: {rdr.AlbumArtist} \n" +
             $"Album Name: {rdr.AlbumName} \n" +
             $"Track Number: {rdr.TrackNumber} \n" +
             $"Track Disc: {rdr.DiscOrdinal} \n" +
             $"Sub Title: {rdr.SubTitle} \n" +
             $"Publisher URL: {purl = rdr.PublisherURL} \n" +
             $"Web site Encoder URL: {eurl = rdr.WebSiteEncoderUrl} \n" +
             $"Genre: {rdr.Genre} \n" +
             $"Encoded By: {rdr.EncodedBy}\n" +
             $"Comments: {rdr.Comments} \n" +
             $"Publisher: {rdr.Publisher}\n" +
             $"Creation Date: {rdr.CreationDate}\n" +
             $"Legal Copyright: {rdr.Copyright}";
            ReconstructString();
            if (rdr.Image is not null && rdr.Image.Length > 0) { imgstream = new(rdr.Image); }
            if (System.String.IsNullOrEmpty(purl)) { PURLLinkButton.Visible = false; }
            if (System.String.IsNullOrEmpty(eurl)) { EURLLinkButton.Visible = false; }
        }

        private void ReconstructString()
        {
            if (resultdata is null) { return; }
            System.Text.StringBuilder sb = new(resultdata.Length);
            foreach (System.Char c in resultdata)
            {
                switch (c)
                {
                    case '&':
                        sb.Append("&&");
                        break;
                    default:
                        sb.Append(c);
                        break;
                }
            }
            resultdata = sb.ToString();
            sb.Clear();
            sb = null;
        }

        private void EXB_CLICK(object sender, EventArgs e) => Close();

        private void F_LOAD(object sender, EventArgs e)
        {
            TextInfo.Text += resultdata;
            if (imgstream is not null)
            {
                try { CoverImageBox.Image = new Bitmap(imgstream); } catch { }
            }
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
            if (imgstream != null)
            {
                imgstream.Dispose();
                if (CoverImageBox.Image is not null)
                {
                    CoverImageBox.Image.Dispose();
                    CoverImageBox.Image = null;
                }
                imgstream = null;
            }
            eurl = null;
            purl = null;
            base.Dispose(disposing);
        }

        private void EURLLinkButton_Click(object sender, EventArgs e)
        {
            System.String urlrun = eurl;
            if (urlrun.StartsWith("http://") == false && urlrun.StartsWith("https://") == false)
            {
                urlrun = "http://" + urlrun;
            }
            new System.Uri(urlrun).Launch();
        }

        private void PURLLinkButton_Click(object sender, EventArgs e)
        {
            System.String urlrun = purl;
            if (urlrun.StartsWith("http://") == false && urlrun.StartsWith("https://") == false)
            {
                urlrun = "http://" + urlrun;
            }
            new System.Uri(urlrun).Launch();
        }

        private void CoverImageBox_Export(object sender, EventArgs e)
        {
            if (imgstream is null) { return; }
            System.Threading.Thread td = new(() => {
                Dialogs.SaveFileDialog sfd = new();
                sfd.DefaultFilterExtension = ".jpeg";
                sfd.AddMultipleFiltersFromString(Settings.Global.Resources.GetStringResource("ExportCoverImage_ImageFormats"));
                sfd.CheckFilePath = true;
                sfd.Title = Settings.Global.Resources.GetStringResource("ExportCoverImage_Title");
                if (sfd.SpawnDialog(Handle))
                {
                    FileStream fs = null;
                    System.String extension = Path.GetExtension(sfd.FilePaths[0]);
                    System.Drawing.Bitmap bm = null;
                    try {
                        fs = new(sfd.FilePaths[0], FileMode.Create);
                        imgstream.Position = 0;
                        bm = new(imgstream);
                        bm.Save(fs, extension switch
                        {
                            ".jpg" => System.Drawing.Imaging.ImageFormat.Jpeg,
                            ".jpeg" => System.Drawing.Imaging.ImageFormat.Jpeg,
                            ".png" => System.Drawing.Imaging.ImageFormat.Png,
                            _ => System.Drawing.Imaging.ImageFormat.Jpeg
                        });
                    } catch (System.Exception ex)
                    {
                        MusicPlayerHelper.ShowErrorResourceMessage("CoverImageCouldNotBeSavedError", Controller, sfd.FilePaths[0], ex);
                    } finally {
                        bm?.Dispose();
                        bm = null;
                        fs?.Dispose();
                        fs = null;
                    }
                }
            });
            td.TrySetApartmentState(System.Threading.ApartmentState.STA);
            td.Start();
        }
    }
}
