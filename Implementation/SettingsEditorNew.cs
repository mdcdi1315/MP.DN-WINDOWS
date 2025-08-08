using System;
using Microsoft.IO;
using MP.SettingsTree;
using static MP.Settings;
using System.Windows.Forms;
using MP.ExtensibilitySystem;
using MP.AudioLibrary.MMDevice;
using System.Collections.Generic;

namespace MP
{
    internal partial class SettingsEditorNew : Form
    {
        private const System.String rootobjname = "__ST_ROOT";
        private System.Boolean updatedevice, showingsuggestion, canclose;
        private SettingsTreeBuilder builder;
        private ExtensionEngine engine;

        public SettingsEditorNew(SettingsTreeBuilder builder, ExtensionEngine eng)
        {
            if (builder is null) { throw new ArgumentNullException(nameof(builder)); }
            if (eng is null) { throw new ArgumentNullException(nameof(eng)); }
            this.builder = builder;
            canclose = true;
            updatedevice = false;
            showingsuggestion = false;
            engine = eng;
            InitializeComponent();
        }

        private void CreateWinFormsTree()
        {
            TreeNode translated = new();
            translated.Tag = rootobjname;
            translated.Text = Global.Resources.GetStringResource("SettingsEditorNew_RootText");
            // RunRecursively builds apporpriately all the child elements, and their corresponding settings.
            SettingsEditorHelpers.RunRecursively(translated, builder.RootElement);
            // We still need to add the root element settings if any.
            if (builder.RootElement.SettingsCount > 0)
            {
                foreach (var s in builder.RootElement.Settings)
                {
                    translated.Nodes.Add(new TreeNode()
                    {
                        Text = s.FriendlyName ?? s.ID,
                        Tag = s.ID
                    });
                }
            }
            SettingsNodesView.Nodes.Add(translated);
            // Add the loaded extensions to be shown in the settings panel
            TreeNode extnode = new();
            extnode.Text = "Loaded extensions...";
            extnode.Tag = "__G_EXTENSIONS";
            SettingsEditorHelpers.ExtensionData dt;
            foreach (var pkg in engine.Packages)
            {
                dt = new();
                dt.Name = pkg.Name;
                dt.Version = pkg.Version;
                dt.Description = pkg.Description;
                extnode.Nodes.Add(new TreeNode() { Text = $"Package {pkg.Name}...", Tag = dt });
            }
            SettingsNodesView.Nodes.Add(extnode);
            if (Global.YtDlpInstallationDirectory.Exists) {
                SettingsNodesView.Nodes.Add(new TreeNode() { Text = "Verify YtDlp Installation..." , Tag = "__G_YTDLPVERIFY" });
            }
            // Also add the Credits pane to be loaded from here.
            // This item would only invoke the credits pane when 
            // the item is double-clicked.
            SettingsNodesView.Nodes.Add(new TreeNode() { Text = "Credits...", Tag = "__G_CREDITS" });
        }

        private void F_LOAD(object sender, EventArgs e)
        {
            Text = AppInfo.FormatWindowTitle(Global.Resources.GetStringResource("SettingsEditorNew_TitleText"));
            ValuesSettingPanel.Hide();
            ChooseFileOrFolderButton.Hide();
            SettingsNodesView.BeginUpdate();
            CreateWinFormsTree();
            SettingsNodesView.EndUpdate();
        }

        private void F_NODESEL(object sender, TreeViewEventArgs e)
        {
            if (e.Node is null) { return; }
            System.String nodeid = e.Node.Tag as System.String;
            CleanSuggestionData();
            SettingDescriptionLabel.Text = null;
            if (e.Node.Tag is SettingsEditorHelpers.ExtensionData dt)
            {
                // Special case, we need the settings panel to show package info
                ShowPackageInfo(dt);
            }
            else if (nodeid.StartsWith("__NODE-"))
            {
                // It is a settings tree node , ignore it and disappear the setting panel.
                ValuesSettingPanel.Hide();
            }
            else
            {
                switch (nodeid)
                {
                    case rootobjname:
                        // In such case show a suggestion on how using this revamped settings window.
                        ShowSuggestionData();
                        break;
                    case "__G_YTDLPVERIFY":
                    case "__G_CREDITS":
                        // Just a default so as to not search these elements with no reason.
                        break;
                    case "__G_EXTENSIONS":
                        ValuesSettingPanel.Show();
                        StringValueTextBox.Hide();
                        RangeSettingUpDown.Hide();
                        SettingValueColorBox.Hide();
                        SettingDescriptionLabel.Show();
                        ValidValuesBoxForSetting.Hide();
                        ConfirmSettingValueButton.Hide();
                        ChooseFileOrFolderButton.Hide();
                        System.Text.StringBuilder sb = new(1000);
                        IList<Version> vers = engine.EngineVersioning.AppVersions;
                        for (int I = 0; I < vers.Count; I++)
                        {
                            sb.AppendFormat("Application Version Ordinal #{0}: {1}\n" , I+1 , vers[I]);
                        }
                        SettingDescriptionLabel.Text = MusicPlayerHelper.WrapStringBy($"Extension Engine Information: \nEngine Version: {engine.EngineVersioning.EngineVersion} \nInjected application versions ({vers.Count} versions discovered):\n {sb}\nLoaded Extension packages: {engine.LoadedPackages}\nNumber of extension loading failures: {engine.FailedExtensionsCount}", 70);
                        break;
                    default:
                        CreateSettingPane(builder.RootElement.GetSetting(nodeid));
                        break;
                }
            }
        }

        private void ShowPackageInfo(SettingsEditorHelpers.ExtensionData dt)
        {
            ValuesSettingPanel.Show();
            StringValueTextBox.Hide();
            RangeSettingUpDown.Hide();
            SettingValueColorBox.Hide();
            SettingDescriptionLabel.Show();
            ValidValuesBoxForSetting.Hide();
            ConfirmSettingValueButton.Hide();
            ChooseFileOrFolderButton.Hide();
            SettingDescriptionLabel.Text = MusicPlayerHelper.WrapStringBy($"Package {dt.Name}:\nVersion: {dt.Version}\nDescription: {dt.Description}", 70);
        }

        private void ShowSuggestionData()
        {
            if (showingsuggestion) { return; }
            DebugProvider.WriteLine("SettingsManager: Showing usage hint to the user...");
            System.Drawing.Pen pn = null;
            System.Drawing.Bitmap bm = null;
            System.Drawing.Graphics gs = null;
            try
            {
                ValuesSettingPanel.Show();
                StringValueTextBox.Hide();
                RangeSettingUpDown.Hide();
                SettingValueColorBox.Hide();
                SettingDescriptionLabel.Show();
                ValidValuesBoxForSetting.Hide();
                ConfirmSettingValueButton.Hide();
                ChooseFileOrFolderButton.Hide();
                gs = ValuesSettingPanel.CreateGraphics();
                pn = System.Drawing.Pens.White;
                gs.DrawRectangle(pn, new System.Drawing.Rectangle(28, 152, 352, 232));
                SettingDescriptionLabel.Text = Global.Resources.GetStringResource("SettingsEditorNew_WelcomeAndUsageMessage");
                bm = Global.Resources.LoadNormalBitmap("SettingsEditorNew_SuggestionShowCaseImage");
                gs.DrawImage(bm, new System.Drawing.Rectangle(30, 154, 350, 230));
                showingsuggestion = true;
            }
            catch (System.Exception e)
            {
                showingsuggestion = false;
                ValuesSettingPanel?.Hide();
                DebugProvider.WriteLine($"SettingsManager: Cannot load the settings suggestion data due to an underlying exception: \n{e}");
            }
            finally
            {
                // Pen is IDisposable but the system color pens are not.
                // If you add a custom one make sure to properly dispose it
                // by uncommenting the line below.
                // pn?.Dispose();  
                pn = null;
                bm?.Dispose();
                bm = null;
                gs?.Dispose();
                gs = null;
            }
        }

        private void CleanSuggestionData()
        {
            if (showingsuggestion == false) { return; }
            showingsuggestion = false;
            DebugProvider.WriteLine("SettingsManager: Destroying usage hint..");
            using (var gs = ValuesSettingPanel.CreateGraphics())
            {
                gs.Clear(ValuesSettingPanel.BackColor);
            }
        }

        private static System.Decimal CreateDecimal(System.Object obj)
            => new(obj switch
            {
                System.Byte b => b,
                System.SByte sb => sb,
                System.Int16 sh => sh,
                System.UInt16 su => su,
                System.Int32 si => si,
                System.UInt32 ui => ui,
                System.Int64 si => si,
                System.UInt64 ui => ui,
                _ => throw new NotImplementedException($"Object type {obj.GetType().FullName} conversion to decimal is not yet implemented.")
            });

        private static System.Object CreateNumber(System.Decimal dec, System.Type t)
        {
            if (t == typeof(System.Byte))
            {
                return (System.Byte)dec;
            }
            else if (t == typeof(System.SByte))
            {
                return (System.SByte)dec;
            }
            else if (t == typeof(System.Int16))
            {
                return (System.Int16)dec;
            }
            else if (t == typeof(System.UInt16))
            {
                return (System.UInt16)dec;
            }
            else if (t == typeof(System.Int32))
            {
                return (System.Int32)dec;
            }
            else if (t == typeof(System.UInt32))
            {
                return (System.UInt32)dec;
            }
            else if (t == typeof(System.Int64))
            {
                return (System.Int64)dec;
            }
            else if (t == typeof(System.UInt64))
            {
                return (System.UInt64)dec;
            }
            else
            {
                throw new NotImplementedException($"Object type {t.FullName} conversion to decimal is not yet implemented.");
            }
        }

        private void CreateSettingPane(SettingsTreeSetting set)
        {
            if (set is null)
            {
                DebugProvider.WriteLine("SettingsManager: The given setting element was null. Not updating the Setting Panel.");
                return;
            }
            ValuesSettingPanel.Show();
            // Hide all the boxes first
            StringValueTextBox.Hide();
            RangeSettingUpDown.Hide();
            SettingValueColorBox.Hide();
            SettingDescriptionLabel.Hide();
            ValidValuesBoxForSetting.Hide();
            ChooseFileOrFolderButton.Hide();
            ConfirmSettingValueButton.Show();
            try
            {
                SettingDescriptionLabel.Text = set.DescriptionIsResource ? Global.Resources.GetStringResource(set.Description) : set.Description;
            }
            catch (DotNetResourcesExtensions.ResourceNotFoundException e)
            {
                SettingDescriptionLabel.Text = System.String.Format(Global.Resources.GetStringResource("SettingsEditorNew_BrokenDescTextWarn_ResNotFound"), e.ResourceName);
            }
            if (System.String.IsNullOrWhiteSpace(SettingDescriptionLabel.Text))
            {
                SettingDescriptionLabel.Text = System.String.Format(Global.Resources.GetStringResource("SettingsEditorNew_BrokenDescTextWarn"), set.ID);
            }
            SettingDescriptionLabel.Show();
            switch (set.Type)
            {
                case SettingType.Default:
                    if (set.TypeOfValue == typeof(System.String))
                    {
                        // Show textbox pane
                        StringValueTextBox.Show();
                        // And set it's value...
                        StringValueTextBox.Text = builder.GetSettingValue<System.String>(set);
                    }
                    break;
                case SettingType.HasSpecificRange:
                    RangeSettingUpDown.Minimum = System.Decimal.MinValue;
                    RangeSettingUpDown.Maximum = System.Decimal.MaxValue;
                    RangeSettingUpDown.Value = CreateDecimal(builder.GetSettingValueAsObject(set));
                    RangeSettingUpDown.Minimum = CreateDecimal(set.ValidNumericRange.MinimumBound);
                    RangeSettingUpDown.Maximum = CreateDecimal(set.ValidNumericRange.MaximumBound);
                    RangeSettingUpDown.Show();
                    break;
                case SettingType.Color:
                    SettingValueColorBox.DepictedColor = builder.GetSettingValue<System.Drawing.Color>(set);
                    SettingValueColorBox.Show();
                    break;
                case SettingType.ValueList:
                    ValidValuesBoxForSetting.Items.Clear();
                    if (set.ID == nameof(Settings.AudioDeviceId))
                    {
                        MMDeviceEnumerator enumerator = null;
                        try
                        {
                            ValidValuesBoxForSetting.BeginUpdate();
                            enumerator = new();
                            var mdc = enumerator.EnumAudioEndpoints(EDataFlow.Render, DEVICE_STATE.ACTIVE);
                            foreach (var dev in mdc)
                            {
                                ValidValuesBoxForSetting.Items.Add(new SettingsEditorHelpers.AudioDeviceData() { FriendlyName = dev.FriendlyName , DeviceID = dev.ID });
                                dev.Dispose();
                            }
                            ValidValuesBoxForSetting.Show();
                        } catch (System.Exception e) {
                            ShowErrorMessageWithRes("SettingsEditorNew_CannotLoadSettingsObject", e);
                            return;
                        } finally {
                            ValidValuesBoxForSetting.EndUpdate();
                            enumerator?.Dispose();
                            enumerator = null;
                        }
                    }
                    break;
                case SettingType.IsFilePath:
                    if (set.TypeOfValue == typeof(System.String))
                    {
                        // Show textbox pane
                        StringValueTextBox.Show();
                        // And set it's value...
                        StringValueTextBox.Text = builder.GetSettingValue<System.String>(set);
                        // Additionally show the file/folder dialog button
                        ChooseFileOrFolderButton.Text = "Choose File...";
                        ChooseFileOrFolderButton.Show();
                    }
                    else if (set.TypeOfValue == typeof(Microsoft.IO.FileInfo))
                    {
                        // Show textbox pane
                        StringValueTextBox.Show();
                        // And set it's value...
                        StringValueTextBox.Text = builder.GetSettingValue<Microsoft.IO.FileInfo>(set).FullName;
                        // Additionally show the file/folder dialog button
                        ChooseFileOrFolderButton.Text = "Choose File...";
                        ChooseFileOrFolderButton.Show();
                    }
                    break;
                case SettingType.IsFolderPath:
                    if (set.TypeOfValue == typeof(System.String))
                    {
                        // Show textbox pane
                        StringValueTextBox.Show();
                        // And set it's value...
                        StringValueTextBox.Text = builder.GetSettingValue<System.String>(set);
                        // Additionally show the file/folder dialog button
                        ChooseFileOrFolderButton.Text = "Choose Folder...";
                        ChooseFileOrFolderButton.Show();
                    }
                    else if (set.TypeOfValue == typeof(Microsoft.IO.DirectoryInfo))
                    {
                        // Show textbox pane
                        StringValueTextBox.Show();
                        // And set it's value...
                        StringValueTextBox.Text = builder.GetSettingValue<Microsoft.IO.DirectoryInfo>(set).FullName;
                        // Additionally show the file/folder dialog button
                        ChooseFileOrFolderButton.Text = "Choose Folder...";
                        ChooseFileOrFolderButton.Show();
                    }
                    break;
            }
        }

        private void OnSettingConfirmation(object sender, EventArgs e)
        {
            System.String nid = null;
            if (SettingsNodesView.SelectedNode is not null)
            {
                nid = SettingsNodesView.SelectedNode.Tag as System.String;
            }
            if (nid.StartsWith("Node-")) { nid = null; }
            if (nid is null)
            {
                ShowErrorMessageWithRes("SettingsEditorNew_SettingNotSelected");
                return;
            }
            SettingsTreeSetting st = builder.RootElement.GetSetting(nid);
            // st will not be null, however assert it in debug
            System.Diagnostics.Debug.Assert(st is not null, "This was unexpected , hmm...");
            switch (st.Type)
            {
                case SettingType.Default:
                    if (st.TypeOfValue == typeof(System.String))
                    {
                        builder.SetSettingValue(st, StringValueTextBox.Text);
                    }
                    break;
                case SettingType.ValueList:
                    if (ValidValuesBoxForSetting.SelectedIndex == -1)
                    {
                        ShowErrorMessageWithRes("SettingsEditorNew_ValidValueFromValuesNotSelected");
                        return;
                    }
                    if (st.TypeOfValue == typeof(System.String))
                    {
                        if (ValidValuesBoxForSetting.SelectedItem is SettingsEditorHelpers.AudioDeviceData devdat) {
                            builder.SetSettingValue(st, devdat.DeviceID);
                        } else {
                            builder.SetSettingValue(st, ValidValuesBoxForSetting.SelectedItem as System.String);
                        }
                    }
                    break;
                case SettingType.HasSpecificRange:
                    builder.SetSettingValueAsObject(st, CreateNumber(RangeSettingUpDown.Value, st.TypeOfValue));
                    break;
                case SettingType.Color:
                    builder.SetSettingValue(st, SettingValueColorBox.DepictedColor);
                    break;
                case SettingType.IsFilePath:
                    if (System.String.IsNullOrWhiteSpace(StringValueTextBox.Text))
                    {
                        ShowErrorMessageWithRes("SettingsEditorNew_PathCannotBeEmpty");
                        return;
                    }
                    MP.Dialogs.FileOperationDialog fop = null;
                    try
                    {
                        Microsoft.IO.FileInfo fi = new(StringValueTextBox.Text);
                        if (st.TypeOfValue == typeof(System.String))
                        {
                            fop = new(Dialogs.FileOperationType.MoveFile, builder.GetSettingValue<System.String>(st), fi.FullName);
                        }
                        else if (st.TypeOfValue == typeof(Microsoft.IO.FileInfo))
                        {
                            fop = new(Dialogs.FileOperationType.MoveFile, builder.GetSettingValue<Microsoft.IO.FileInfo>(st).FullName, fi.FullName);
                        }
                        fop.ForegroundColor = ForeColor;
                        fop.BackgroundColor = BackColor;
                        canclose = false;
                        fop.ShowDialog(this);
                        if (fop.HasSuccessfullyCompleted)
                        {
                            if (st.TypeOfValue == typeof(System.String))
                            {
                                builder.SetSettingValue(st, fi.FullName);
                            }
                            else if (st.TypeOfValue == typeof(Microsoft.IO.FileInfo))
                            {
                                builder.SetSettingValue(st, fi);
                            }
                        }
                        canclose = true;
                    }
                    catch (System.Exception ex)
                    {
                        Dialogs.NativeMessageBox.Show(Global.Resources.GetStringResource("SettingsEditorNew_FileDirOperationFailed"), "Error", Dialogs.ButtonSelection.OK, Dialogs.IconSelection.Error);
                        DebugProvider.WriteLine($"SettingsManager: [ERROR] File Operation Failed with: \n{ex}");
                        return;
                    }
                    finally
                    {
                        fop?.Dispose();
                        fop = null;
                    }
                    break;
                case SettingType.IsFolderPath:
                    if (System.String.IsNullOrWhiteSpace(StringValueTextBox.Text))
                    {
                        ShowErrorMessageWithRes("SettingsEditorNew_PathCannotBeEmpty");
                        return;
                    }
                    fop = null;
                    try
                    {
                        Microsoft.IO.DirectoryInfo di = new(StringValueTextBox.Text);
                        System.String val = null;
                        if (st.TypeOfValue == typeof(System.String))
                        {
                            val = builder.GetSettingValue<System.String>(st);
                        }
                        else if (st.TypeOfValue == typeof(Microsoft.IO.DirectoryInfo))
                        {
                            val = builder.GetSettingValue<Microsoft.IO.DirectoryInfo>(st).FullName;
                        }
                        if (Microsoft.IO.Directory.Exists(val) == false)
                        {
                            if (st.TypeOfValue == typeof(System.String))
                            {
                                builder.SetSettingValue(st, di.FullName);
                            }
                            else if (st.TypeOfValue == typeof(Microsoft.IO.DirectoryInfo))
                            {
                                builder.SetSettingValue(st, di);
                            }
                            canclose = true;
                            return;
                        }
                        fop = new(Dialogs.FileOperationType.MoveDirectory, val, di.FullName);
                        fop.ForegroundColor = ForeColor;
                        fop.BackgroundColor = BackColor;
                        canclose = false;
                        fop.ShowDialog(this);
                        if (fop.HasSuccessfullyCompleted)
                        {
                            if (st.TypeOfValue == typeof(System.String))
                            {
                                builder.SetSettingValue(st, di.FullName);
                            }
                            else if (st.TypeOfValue == typeof(Microsoft.IO.DirectoryInfo))
                            {
                                builder.SetSettingValue(st, di);
                            }
                        }
                        canclose = true;
                    }
                    catch (System.Exception ex)
                    {
                        Dialogs.NativeMessageBox.Show(Global.Resources.GetStringResource("SettingsEditorNew_FileDirOperationFailed"), "Error", Dialogs.ButtonSelection.OK, Dialogs.IconSelection.Error);
                        DebugProvider.WriteLine($"SettingsManager: [ERROR] Directory Operation Failed with: \n{ex}");
                        return;
                    }
                    finally
                    {
                        fop?.Dispose();
                        fop = null;
                    }
                    break;
            }
            updatedevice = st.ID == nameof(Settings.AudioDeviceId) && MusicPlayerHelper.ShowResourceQuestionMessage("SettingsEditorNew_ApplyNewDeviceNow");
            if (updatedevice) { Invoke(Close); }
        }

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (components is not null)
                {
                    components.Dispose();
                    components = null;
                }
                builder = null;
                engine = null;
            }
            base.Dispose(disposing);
        }

        private void F_CB_CLICK(object sender, EventArgs e)
        {
            if (SettingsNodesView.SelectedNode is null) { return; }
            ColorDialog cdlg = null;
            try
            {
                cdlg = new();
                cdlg.AnyColor = true;
                cdlg.AllowFullOpen = true;
                cdlg.Color = SettingValueColorBox.DepictedColor;
                if (cdlg.ShowDialog(this) == DialogResult.OK)
                {
                    SettingValueColorBox.DepictedColor = cdlg.Color;
                }
            }
            catch { }
            finally
            {
                cdlg?.Dispose();
                cdlg = null;
            }
        }

        private void F_NODECLICK(object sender, TreeNodeMouseClickEventArgs e)
        {
            if (e.Node is null) { return; }
            F_NodeClickedOrEnterButton(e.Node.Tag as System.String);
        }

        private void F_NodeClickedOrEnterButton(System.String nodeid)
        {
            switch (nodeid)
            {
                case "__G_CREDITS":
                    CreditsPane cp = null;
                    try
                    {
                        cp = new();
                        cp.ShowDialog(this);
                    }
                    catch (Exception ex)
                    {
                        DebugProvider.WriteLine($"SettingsManager: Cannot load the Credits panel due to an underlying exception: \n{ex}");
                        return;
                    }
                    finally
                    {
                        cp?.Dispose();
                        cp = null;
                    }
                    return;
                case "__G_YTDLPVERIFY":
                    if (ShowQuestionMsgWithRes("SettingsEditorNew_VerifyYtDlpInstallationDataQuestion") == false) { return; }
                    DownloadVerifier dv = null;
                    FileInfo fi = Global.YtDlpInstallationDirectory.GetFile("VerifierData.json");
                    if (fi is null)
                    {
                        ShowErrorMessageWithRes("SettingsEditorNew_CannotFindYtDlpInstallationFile");
                        return;
                    }
                    try
                    {
                        dv = new(Global.YtDlpInstallationDirectory, fi);
                        dv.ShowMinimizeButton = false;
                        dv.ShowDialog(this);
                    }
                    finally
                    {
                        dv?.Dispose();
                        dv = null;
                    }
                    break;
            }
        }

        private void F_SHOWN(object sender, EventArgs e) => ShowSuggestionData();

        private void F_CHOOSEPATH(object sender, EventArgs e)
        {
            Dialogs.FileDialog mpf;
            if (ChooseFileOrFolderButton.Text == "Choose File...")
            {
                mpf = new Dialogs.OpenFileDialog();
                mpf.MultiSelect = false;
                mpf.CheckFilePath = true;
                mpf.CheckPath = true;
                mpf.AddFilter(new("*.*", "All Files"));
            }
            else if (ChooseFileOrFolderButton.Text == "Choose Folder...")
            {
                mpf = new Dialogs.OpenFolderDialog();
                mpf.MultiSelect = false;
            }
            else { return; }
            if (mpf.SpawnDialog(this)) { StringValueTextBox.Text = mpf.FilePaths[0]; }
            mpf = null;
        }

        private void F_CLOSING(object sender, FormClosingEventArgs e)
        {
            e.Cancel = canclose == false;
        }

        private void F_TN_KEYDOWN(object sender, KeyEventArgs e)
        {
            var sn = SettingsNodesView.SelectedNode;
            if (sn is null) { return; }
            if (e.KeyCode != Keys.Enter) { return; }
            F_NodeClickedOrEnterButton(sn.Tag as System.String);
        }

        private void ShowErrorMessageWithRes(System.String resource, params System.Object[] fmt)
        {
            System.String resfmt = System.String.Format(Global.Resources.GetStringResource(resource), args: fmt);
            using (MP.Dialogs.NewGenMessageBox mb = new())
            {
                mb.Text = resfmt;
                mb.Title = "Error";
                mb.Buttons = Dialogs.ButtonSelection.OK;
                mb.SelectedIcon = Dialogs.IconSelection.Error;
                mb.ShowDialog(this);
            }
        }

        private System.Boolean ShowQuestionMsgWithRes(System.String resource, params System.Object[] fmt)
        {
            System.String resfmt = System.String.Format(Global.Resources.GetStringResource(resource), args: fmt);
            using (MP.Dialogs.NewGenMessageBox mb = new())
            {
                mb.Text = resfmt;
                mb.Title = "Question";
                mb.Buttons = Dialogs.ButtonSelection.YesNo;
                mb.SelectedIcon = Dialogs.IconSelection.Question;
                mb.ShowDialog(this);
                return mb.ReturnedButton == Dialogs.ButtonReturned.Yes;
            }
        }

        public System.Boolean ShouldUpdateDevice => updatedevice;
    }
}
