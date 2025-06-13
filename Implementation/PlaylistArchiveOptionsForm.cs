using System;
using static MP.Settings;
using System.Windows.Forms;

namespace MP
{
    internal partial class PlaylistArchiveOptionsForm : Form
    {
        private FormMode mode;
        private ArchivedPlaylistAttributeCollection attributes;

        private enum FormMode : System.Byte
        {
            EditAttributes,
            AddAttribute
        }

        public PlaylistArchiveOptionsForm(ArchivedPlaylistAttributeCollection currentattributes)
        {
            mode = FormMode.EditAttributes;
            attributes = new(currentattributes.Count);
            foreach (var attribute in currentattributes) 
            {
                attributes.Add(attribute);
            }
            InitializeComponent();
        }

        private void UpdateAttributesBox()
        {
            AttributesBox.BeginUpdate();
            AttributesBox.Items.Clear();
            AttributesBox.SelectedIndex = -1;
            foreach (var attribute in attributes)
            {
                AttributesBox.Items.Add(attribute.Name);
            }
            AttributesBox.EndUpdate();
        }

        private void TryLoadKnownAttributeDescWithName(System.String attrname)
        {
            try
            {
                AttribDescLabel.Text = Global.Resources.GetStringResource($"PlaylistArchiveOptionsForm_ATTR_{attrname}");
            }
            catch (DotNetResourcesExtensions.ResourceNotFoundException)
            {
                AttribDescLabel.Text = Global.Resources.GetStringResource("PlaylistArchiveOptionsForm_IsCustomAttrDesc");
            }
        }

        private void F_LOAD(object sender, EventArgs e)
        {
            ShouldBecomeReadOnlyAttributeChkBox.Text = Global.Resources.GetStringResource("PlaylistArchiveOptionsForm_SpecifyReadOnlyAttrText");
            L1.Text = Global.Resources.GetStringResource("PlaylistArchiveOptionsForm_UsageDesc");
            AttribValBox.Text = System.String.Empty;
            UpdateAttributesBox();
        }

        private void TransitionToNormalMode()
        {
            mode = FormMode.EditAttributes;
            UpdateAttributesBox();
            AttributesBox.Show();
            ConfirmButton.Show();
            GiveDateInAttribValueButton.Show();
            CustomAttributeButton.Show();
            ShouldBecomeReadOnlyAttributeChkBox.Show();
            AttribValBox.Text = System.String.Empty;
            UpdateAttribValueButton.Text = "Update...";
            UpdateAttribValueButton.Enabled = true;
            AttribDescLabel.Text = System.String.Empty;
        }

        private void UPDATEATTRVAL(object sender, EventArgs e)
        {
            switch (mode)
            {
                case FormMode.AddAttribute:
                    if (System.String.IsNullOrEmpty(AttribValBox.Text))
                    {
                        TransitionToNormalMode();
                        break;
                    }
                    try
                    {
                        attributes.UpdateAttributeValue(AttribValBox.Text, null);
                    }
                    catch (MP.ExceptionSystem.ReadOnlyArchiveAttributeException)
                    {
                        Dialogs.NativeMessageBox.Show(Global.Resources.GetStringResource("PlaylistArchiveOptionsForm_DefinedAsReadOnlyAttribute"), "Error", MP.Dialogs.ButtonSelection.OK, Dialogs.IconSelection.Error);
                        TransitionToNormalMode();
                        break;
                    }
                    TransitionToNormalMode();
                    break;
                case FormMode.EditAttributes:
                    if (AttributesBox.SelectedIndex == -1)
                    {
                        Dialogs.NativeMessageBox.Show(Global.Resources.GetStringResource("PlaylistArchiveOptionsForm_AttributeNotSelected"), "Error", MP.Dialogs.ButtonSelection.OK, Dialogs.IconSelection.Error);
                        break;
                    }
                    UpdateAttribValueButton.Enabled = false;
                    if (ShouldBecomeReadOnlyAttributeChkBox.Checked)
                    {
                        attributes.RemoveAt(AttributesBox.SelectedIndex);
                        attributes.Add(new(AttributesBox.Items[AttributesBox.SelectedIndex] as System.String, AttribValBox.Text) { Flags = ArchivedPlaylistAttributeFlags.ReadOnly });
                        UpdateAttributesBox();
                        break;
                    }
                    try {
                        attributes.UpdateAttributeValue(AttributesBox.Items[AttributesBox.SelectedIndex] as System.String, AttribValBox.Text);
                    } catch (MP.ExceptionSystem.ReadOnlyArchiveAttributeException) {
                        Dialogs.NativeMessageBox.Show(Global.Resources.GetStringResource("PlaylistArchiveOptionsForm_DefinedAsReadOnlyAttribute"), "Error", MP.Dialogs.ButtonSelection.OK, Dialogs.IconSelection.Error);
                        break;
                    }
                    break;
            }
        }

        private void NEWCUSTOMATTR(object sender, EventArgs e)
        {
            if (mode == FormMode.AddAttribute) { return; }
            mode = FormMode.AddAttribute;
            AttributesBox.Hide();
            ConfirmButton.Hide();
            CustomAttributeButton.Hide();
            GiveDateInAttribValueButton.Hide();
            UpdateAttribValueButton.Enabled = true;
            UpdateAttribValueButton.Text = "Add...";
            AttribValBox.Text = System.String.Empty;
            ShouldBecomeReadOnlyAttributeChkBox.Hide();
            AttribDescLabel.Text = Global.Resources.GetStringResource("PlaylistArchiveOptionsForm_ProvideAttributeNameText");
        }

        private void ATTRIBUTECHANGED(object sender, EventArgs e)
        {
            System.Int32 si;
            if ((si = AttributesBox.SelectedIndex) == -1) { return; }
            UpdateAttribValueButton.Enabled = true;
            TryLoadKnownAttributeDescWithName(AttributesBox.Items[si] as System.String);
            AttribValBox.Text = attributes[si].Value;
            ShouldBecomeReadOnlyAttributeChkBox.Checked = attributes[si].Flags.HasFlag(ArchivedPlaylistAttributeFlags.ReadOnly);
        }

        private void GIVEDATE(object sender, EventArgs e) => AttribValBox.Text = SystemInfo.Now.ToString("G");

        public ArchivedPlaylistAttributeCollection NewAttributes => attributes;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (components is not null) { components.Dispose(); components = null; }
                attributes?.Clear();
                attributes = null;
            }
            base.Dispose(disposing);
        }
    }
}
