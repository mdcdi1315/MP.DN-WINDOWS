using MP;
using MP.GamepadBackend;
using System.ComponentModel;

namespace System.Windows.Forms
{
    /// <summary>
    /// Provides support for controller event handling in Windows Forms. <br />
    /// The class can be normally derived like the usual designer forms.
    /// </summary>
    [ToolboxItem(false)]
    [DesignTimeVisible(false)]
    [DesignerCategory("Form")]
    [DefaultEvent(nameof(Load))]
    [InitializationEvent(nameof(Load))]
    [ToolboxItemFilter("System.Windows.Forms.Control.TopLevel")]
    [Designer("System.Windows.Forms.Design.FormDocumentDesigner, System.Design, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", typeof(System.ComponentModel.Design.IRootDesigner))]
    public class ControllerCompatibleForm : Form
    {
        private GamepadReader reader;
        private System.DateTime tsmcurrent;
        private System.Boolean disablesupport;
        /// <summary>
        /// Gets or sets the currently active control index.
        /// </summary>
        protected System.Int32 controlindex;

        private static void ControllerEventsDummyFunction(System.Object obj, ControllerEventArgs e) { }

        private static void ControllerAttachedDummyFunction(System.Object obj, EventArgs e) { }

        [EditorBrowsable(EditorBrowsableState.Never)] 
        public ControllerCompatibleForm() : base()
        {
            disablesupport = true;
            controlindex = 0;
            reader = null;
            tsmcurrent = SystemInfo.Now;
            ControllerEvents = new(ControllerEventsDummyFunction);
            ControllerAttached = new(ControllerAttachedDummyFunction);
        }

        public ControllerCompatibleForm(GamepadReader rdr) : this()
        {
            disablesupport = rdr is null;
            if (disablesupport == false) { reader = rdr.Clone(); }
            disablesupport = reader is null;
            Load += F_LOAD;
        }

        protected override void Dispose(bool disposing)
        {
            disablesupport = true;
            base.Dispose(disposing);
            if (reader is not null) {
                reader.GamepadAction -= ControllerEventsHandler;
                reader.Dispose();
                reader = null;
            }
        }

        private void F_LOAD(System.Object send , System.EventArgs e)
        {
            if (disablesupport == false) {
                var delaythread = new System.Threading.Thread(() => {
                    System.Threading.Thread.Sleep(230);
                    reader.GamepadAction += ControllerEventsHandler;
                    reader.Listen = true;
                    Invoke(() => { ControllerAttached.Invoke(this, null); });
                });
                delaythread.IsBackground = true;
                delaythread.Name = "[MP] Form background initialization thread";
                delaythread.Start();
            }
            Activated += F_ACTIVATED;
            Deactivate += F_DEACTIVATED;
            FormClosed += F_CLOSED;
        }

        private void F_CLOSED(System.Object sender, System.EventArgs e)
        {
            if (disablesupport == false) { reader.Listen = false; }
            Activated -= F_ACTIVATED;
            Deactivate -= F_DEACTIVATED;
        }

        private void F_ACTIVATED(System.Object send , System.EventArgs e)
        {
            if (disablesupport == false) { reader.Listen = true; }
        }

        private void F_DEACTIVATED(System.Object send , System.EventArgs e)
        {
            if (disablesupport == false) { reader.Listen = false; }
        }

        private void ControllerEventsHandler(System.Object sender, GamepadEventEventArgs e)
        {
            System.Int32 si;
            if (IsDisposed) { return; }
            if (IsHandleCreated == false) { return; }
            if (Controls.Count <= 0) { return; }
            // If a second event is dispatched in less than 100 ms ignore it.
            // However , update the timestamp for the next event.
            if (SystemInfo.Now.Subtract(tsmcurrent).TotalMilliseconds < 100) { 
                tsmcurrent = SystemInfo.Now;
                return; 
            }
            ValidControllerEvent(sender, e);
            switch (e.Type)
            {
                case GamepadMode.Button:
                    switch (e.Button)
                    {
                        case GamepadButton.B:
                            Close();
                            break;
                        case GamepadButton.A:
                            // Always verify that we are going to dispatch this on a visible control
                            if (Controls[controlindex].Visible) {
                                InvokeOnClick(Controls[controlindex], System.EventArgs.Empty);
                                OnControllerEvents(new(Controls[controlindex]));
                            }
                            break;
                        case GamepadButton.DPadRight:
                            do {
                                if (controlindex < 0) { controlindex = 0; goto g_undispatch; }
                                controlindex--;
                            } while (IsValidControl(controlindex) == false);
                            switch (Controls[controlindex])
                            {
                                case ListBox: // Controls that are presenting items do not need to be selected.
                                case ComboBox:
                                case ListView:
                                    break;
                                default:
                                    Controls[controlindex].Refresh();
                                    Controls[controlindex].Select();
                                    break;
                            }
                            break;
                        case GamepadButton.DPadLeft:
                            do {
                                if (controlindex >= Controls.Count) { controlindex = 0; goto g_undispatch; }
                                controlindex++;
                            } while (IsValidControl(controlindex) == false);
                            switch (Controls[controlindex])
                            {
                                case ListBox: // Controls that are presenting items do not need to be selected.
                                case ComboBox:
                                case ListView:
                                    break;
                                default:
                                    Controls[controlindex].Refresh();
                                    Controls[controlindex].Select();
                                    break;
                            }
                            break;
                        // In case that the given control is a control that presents items , then allow the user to search between the options.
                        case GamepadButton.DPadUp:
                            switch (Controls[controlindex])
                            {
                                case ListBox lb:
                                    if (lb.SelectedIndices.Count == 0) { lb.SelectedIndices.Add(0); break; }
                                    if (lb.SelectedIndices.Count > 1) { lb.SelectedIndices.Clear(); lb.SelectedIndices.Add(0); break; }
                                    si = lb.SelectedIndices[0];
                                    lb.SelectedIndices.Remove(0);
                                    si--; 
                                    if (si < 0) { si = 0; }
                                    lb.SelectedIndices.Add(si);
                                    break;
                                case ListView lv:
                                    if (lv.SelectedIndices.Count == 0) { lv.SelectedIndices.Add(0); break; }
                                    if (lv.SelectedIndices.Count > 1) { lv.SelectedIndices.Clear(); lv.SelectedIndices.Add(0); break; }
                                    si = lv.SelectedIndices[0];
                                    lv.SelectedIndices.Remove(0);
                                    si--;
                                    if (si < 0) { si = 0; }
                                    lv.SelectedIndices.Add(si);
                                    break;
                                case ComboBox cb:
                                    if (cb.SelectedIndex < 0) { cb.SelectedIndex = 0; break; }
                                    cb.SelectedIndex--;
                                    if (cb.SelectedIndex < 0) { cb.SelectedIndex = 0; break; }
                                    break;
                            }
                            break;
                        case GamepadButton.DPadDown:
                            switch (Controls[controlindex])
                            {
                                case ListBox lb:
                                    if (lb.SelectedIndices.Count == 0) { lb.SelectedIndices.Add(0); break; }
                                    if (lb.SelectedIndices.Count > 1) { lb.SelectedIndices.Clear(); lb.SelectedIndices.Add(0); break; }
                                    si = lb.SelectedIndices[0];
                                    lb.SelectedIndices.Remove(0);
                                    si++;
                                    if (si >= lb.Items.Count) { si = 0; }
                                    lb.SelectedIndices.Add(si);
                                    break;
                                case ListView lv:
                                    if (lv.SelectedIndices.Count == 0) { lv.SelectedIndices.Add(0); break; }
                                    if (lv.SelectedIndices.Count > 1) { lv.SelectedIndices.Clear(); lv.SelectedIndices.Add(0); break; }
                                    si = lv.SelectedIndices[0];
                                    lv.SelectedIndices.Remove(0);
                                    si++;
                                    if (si >= lv.Items.Count) { si = 0; }
                                    lv.SelectedIndices.Add(si);
                                    break;
                                case ComboBox cb:
                                    if (cb.SelectedIndex < 0) { cb.SelectedIndex = 0; break; }
                                    cb.SelectedIndex++;
                                    if (cb.SelectedIndex >= cb.Items.Count) { cb.SelectedIndex = 0; break; }
                                    break;
                            }
                            break;
                    }
                    break;
            }
        g_undispatch:
            tsmcurrent = SystemInfo.Now;
        }

        private System.Boolean IsValidControl(System.Int32 index)
        {
            if (index < 0 || index >= Controls.Count) { return false; }
            switch (Controls[index])
            {
                case Button:
                case CheckBox:
                case RadioButton:
                case ButtonBase:
                case ListBox:
                case ComboBox:
                case ListView:
                    return Controls[index].Visible && Controls[index].Enabled;
                default:
                    return false;
            }
        }

        /// <summary>
        /// Override this method to provide additional custom controller actions at run-time.
        /// </summary>
        protected virtual void ValidControllerEvent(System.Object sender, GamepadEventEventArgs e) { }

        /// <summary>
        /// Invokes the event <see cref="ControllerEvents"/> with the given control that is selected.
        /// </summary>
        /// <param name="e">The event args which do contain the selected control.</param>
        protected void OnControllerEvents(ControllerEventArgs e) => ControllerEvents.Invoke(this, e);

        /// <summary>
        /// Listening to this event provides the selected control data to invoke each time.
        /// </summary>
        [Category("Behavior")]
        [Description("Listens to the invoked controller events. The actions must be performed on control-level.")]
        public event EventHandler<ControllerEventArgs> ControllerEvents;

        /// <summary>
        /// Listening to this event informs you that the controller is listening to new events. <br />
        /// Be noted that the controller might be never attached to this instance;  <br />
        /// so you can use this event to execute only code specific for the controller handling.
        /// </summary>
        [Category("Behavior")]
        [Description("Fired when the controller is successfully attached to this form. By default a small delay is applied so as avoid early command processing.")]
        public event EventHandler ControllerAttached;

        /// <summary>
        /// Only settable at construction-time.
        /// </summary>
        [ReadOnly(true)]
        [Description("Gets the backend controller that is used by this instance.")]
        [Category("Controller Handling")]
        public GamepadReader Controller => reader;

        /// <summary>
        /// Determines whether the controller is not available to the form.
        /// </summary>
        [ReadOnly(true)]
        [Description("Determines whether controller support is disabled. Always true for design-time stuff.")]
        [Category("Controller Handling")]
        public System.Boolean ControllerDisabled => disablesupport;
    }
}
