
using System;
using System.Windows.Forms;
using MP.Caches.StatisticsCache;
using System.Collections.Generic;

namespace MP
{
    internal partial class StatisticsViewer : Form
    {
        private System.Int32 laststatindex;
        private StatisticsCacheInstance stats;
        private PlaylistMetadataItemCollection metadata;

        public StatisticsViewer(StatisticsCacheInstance instance, PlaylistMetadataItemCollection metadata = null)
        {
            InitializeComponent();
            stats = instance;
            this.metadata = metadata;
            laststatindex = System.Int32.MaxValue;
        }

        private void F_LOAD(object sender, EventArgs e)
        {
            foreach (var s in stats.AllStatistics)
            {
                laststatindex = StatSelectionBox.Items.Add(s.Name);
            }
            if (metadata is not null)
            {
                foreach (var m in metadata)
                {
                    StatSelectionBox.Items.Add($"[Playlist Metadata Item] {m.Name}");
                }
            }
        }

        private void StatSelectionBox_ItemChange(object sender, EventArgs e)
        {
            if (StatSelectionBox.SelectedIndex == -1) { return; }
            if (StatSelectionBox.SelectedIndex > laststatindex)
            {
                var i = metadata[StatSelectionBox.SelectedIndex - laststatindex - 1];
                ValueBox.Text = i.TypeCode switch {
                    MetadataItemTypeCode.Null => "<Cleared>",
                    MetadataItemTypeCode.String => System.String.IsNullOrEmpty(i.Value as System.String) ? "<Empty>" : i.Value as System.String,
                    MetadataItemTypeCode.Boolean => ((System.Boolean)i.Value) ? "Yes" : "No",
                    _ => i.Value.ToString(),
                };
            } else {
                var s = stats.GetAt(StatSelectionBox.SelectedIndex);
                ValueBox.Text = s.TypeOfValue switch {
                    StatisticType.Null => "<Cleared>",
                    StatisticType.Boolean => ((System.Boolean)s.Value) ? "Yes" : "No",
                    StatisticType.String => System.String.IsNullOrEmpty(s.Value as System.String) ? "<Empty>" : s.Value as System.String,
                    StatisticType.DateTime => ((DateTime)s.Value).ToString("G"),
                    StatisticType.TimeSpan => ((TimeSpan)s.Value).ToString("-hh\\:mm\\:ss\\.fffff"),
                    _ => s.Value.ToString(),
                };
            }
        }

        private void F_EXIT(object sender, EventArgs e) => Close();

        private void F_ClearStatValue(object sender, EventArgs e)
        {
            if (StatSelectionBox.SelectedIndex == -1) { return; }
            if (StatSelectionBox.SelectedIndex > laststatindex)
            {
                System.Int32 idx = StatSelectionBox.SelectedIndex - laststatindex - 1;
                var i = metadata[idx];
                switch (i.TypeCode)
                {
                    case MetadataItemTypeCode.String:
                        i.Value = System.String.Empty;
                        break;
                    case MetadataItemTypeCode.Boolean:
                        i.Value = false;
                        break;
                }
                metadata[i.Name] = i.Value;
            } else {
                var s = stats.GetAt(StatSelectionBox.SelectedIndex);
                if (s.Name == "StatisticsVersion") {
                    F_ShowErrorResourceBox("Error_FieldDisallowedToBeCleared" , s.Name);
                    return;
                }
                switch (s.TypeOfValue)
                {
                    case StatisticType.Boolean:
                        s.Value = false;
                        break;
                    case StatisticType.String:
                        s.Value = System.String.Empty;
                        break;
                    case StatisticType.LongNumber:
                        s.Value = 0L; 
                        break;
                    case StatisticType.UnsignedLongNumber:
                        s.Value = 0UL;
                        break;
                }
                stats.Update(s);
            }
            StatSelectionBox_ItemChange(sender, e);
        }

        private void F_ShowErrorResourceBox(System.String reserror ,params System.Object[] format)
        {
            using (MP.Dialogs.NewGenMessageBox mb = new())
            {
                mb.Text = System.String.Format(Settings.Global.Resources.GetStringResource(reserror),args: format);
                mb.Title = "Error";
                mb.Buttons = Dialogs.ButtonSelection.OK;
                mb.SelectedIcon = Dialogs.IconSelection.Error;
                mb.ShowDialog(this);
            }
        }

        /// <summary>Clean up any resources being used.</summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (components is not null) {
                    components.Dispose();
                    components = null;
                }
                stats = null;
                metadata = null;
            }
            base.Dispose(disposing);
        }
    }
}
