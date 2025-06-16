
using System;
using System.Windows.Forms;

namespace MP
{
    internal partial class PlaylistPreferencesEditor : Form
    {
        private TrackPlayList playlist;
        private System.Boolean updstatus;

        public PlaylistPreferencesEditor(ref TrackPlayList playlist)
        {
            if (playlist is null) { throw new ArgumentNullException(nameof(playlist)); }
            InitializeComponent();
            this.playlist = playlist;
            updstatus = true;
        }

        private void F_LOAD(object sender, EventArgs e)
        {
            L1.ForeColor = ForeColor;
            L1.BackColor = BackColor;
            L2.ForeColor = ForeColor;
            L2.BackColor = BackColor;
            L3.ForeColor = ForeColor;
            L3.BackColor = BackColor;
            StatusLabel.ForeColor = ForeColor;
            StatusLabel.BackColor = BackColor;
            SelectionDescription.ForeColor = ForeColor;
            SelectionDescription.BackColor = BackColor;
            ExitButton.ForeColor = ForeColor;
            ExitButton.BackColor = BackColor;
            OptionBox.ForeColor = ForeColor;
            OptionBox.BackColor = BackColor;
            ValueBox_CheckBox.ForeColor = ForeColor;
            ValueBox_CheckBox.BackColor = BackColor;
            ValueBox_TextBox.ForeColor = ForeColor;
            ValueBox_TextBox.BackColor = BackColor;
            BrowseFileButton.ForeColor = ForeColor;
            BrowseFileButton.BackColor = BackColor;
            ConfOptionButton.ForeColor = ForeColor;
            ConfOptionButton.BackColor = BackColor;
            L1.Text = System.String.Format(Settings.Global.Resources.GetStringResource("PlaylistPreferencesEditingWindow_Heading") , playlist.PlaylistName);
            UpdateAndReloadPreferences(null);
            StatusLabel.Text = System.String.Empty;
            ConfOptionButton.Visible = false;
        }

        private void UpdateSelectionData(object sender, EventArgs e)
        {
            if (updstatus) { StatusLabel.Text = System.String.Empty; }
            if (OptionBox.SelectedIndex <= 0) {
                ValueBox_TextBox.Visible = false;
                ValueBox_CheckBox.Visible = false;
                BrowseFileButton.Visible = false;
                SelectionDescription.Text = "(No Description Available)";
                ConfOptionButton.Visible = false;
            } else {
                ConfOptionButton.Visible = true;
                System.Int32 sidx = OptionBox.SelectedIndex - 1;
                Preference prf = playlist.Preferences[sidx];
                switch (prf.TypeOfValue)
                {
                    case PreferenceValueType.String:
                        ValueBox_TextBox.Visible = true;
                        ValueBox_TextBox.Text = prf.Value.ToString();
                        ValueBox_CheckBox.Visible = false;
                        BrowseFileButton.Visible = prf.Flags.HasFlag(PreferenceBehaviorFlags.FilePath);
                        break;
                    case PreferenceValueType.Boolean:
                        ValueBox_TextBox.Visible = false;
                        ValueBox_CheckBox.Visible = true;
                        ValueBox_CheckBox.Checked = prf.BooleanValue;
                        BrowseFileButton.Visible = false;
                        break;
                    case PreferenceValueType.Number:
                        ValueBox_TextBox.Visible = true;
                        ValueBox_TextBox.Text = prf.NumericValue.ToString();
                        ValueBox_CheckBox.Visible = false;
                        BrowseFileButton.Visible = false;
                        break;
                }
                SelectionDescription.Text = MusicPlayerHelper.WrapStringBy(prf.Description, 94);
            }
        }

        private void E_FORM(object sender, EventArgs e) => Close();

        private void ConfirmOption_Click(object sender, EventArgs e)
        {
            System.Int32 sidx = OptionBox.SelectedIndex - 1;
            Preference prf = playlist.Preferences[sidx];
            switch (prf.TypeOfValue)
            {
                case PreferenceValueType.String:
                    playlist.Preferences.Update(prf.Name, ValueBox_TextBox.Text);
                    break;
                case PreferenceValueType.Boolean:
                    playlist.Preferences.Update(prf.Name, ValueBox_CheckBox.Checked);
                    break;
                case PreferenceValueType.Number:
                    System.Int64 n;
                    try {
                        n = System.Int64.Parse(ValueBox_TextBox.Text);
                        if (prf.Flags.HasFlag(PreferenceBehaviorFlags.NumberShouldBePositiveOnly) && n < 0)
                        {
                            MusicPlayerHelper.ShowErrorMessage("The numeric value should be negative.");
                            return;
                        }
                    } catch {
                        MusicPlayerHelper.ShowErrorMessage("Cannot parse the string given to a number. Check that the value given is a parseable number.");
                        return;
                    }
                    playlist.Preferences.Update(prf.Name, n);
                    break;
            }
            StatusLabel.Text = "Saved!";
            UpdateAndReloadPreferences(prf.Name);
        }

        private void UpdateAndReloadPreferences(System.String selpref)
        {
            try {
                OptionBox.BeginUpdate();
                updstatus = false;
                OptionBox.Items.Clear();
                OptionBox.Items.Add("(No Item Selected)");
                for (System.Int32 I = 0; I < playlist.Preferences.Count; I++)
                {
                    OptionBox.Items.Add(playlist.Preferences[I].Name);
                }
                OptionBox.SelectedIndex = 0;
                if (System.String.IsNullOrEmpty(selpref) == false)
                {
                    for (System.Int32 I = 1; I < OptionBox.Items.Count; I++)
                    {
                        if (selpref == OptionBox.Items[I].ToString())
                        {
                            OptionBox.SelectedIndex = I;
                            break;
                        }
                    }
                }
            } finally {
                OptionBox.EndUpdate();
                updstatus = true;
            }
        }

        private void BrowseFile_Click(object sender, EventArgs e)
        {
            System.Threading.Thread td = new(() => {
                Dialogs.OpenFileDialog ofd = new();
                ofd.Title = $"Select value for option {playlist.Preferences[OptionBox.SelectedIndex - 1].Name} ...";
                ofd.AddFilter(new("*.*", "All Files"));
                ofd.MultiSelect = false;
                ofd.CheckFilePath = true;
                ofd.CheckPath = true;
                if (ofd.SpawnDialog(Handle)) {
                    ValueBox_TextBox.Text = ofd.FilePaths[0];
                }
            });
            td.TrySetApartmentState(System.Threading.ApartmentState.STA);
            td.Start();
        }
    }
}
