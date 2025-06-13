namespace System.Windows.Forms
{
    /// <summary>
    /// Provides the controller snapshot state when the <see cref="ControllerCompatibleForm.ControllerEvents"/> event is fired.
    /// </summary>
    public sealed class ControllerEventArgs : EventArgs
    {
        private Control control;

        public ControllerEventArgs(Control control) { this.control = control; }

        /// <summary>
        /// Gets the target control to be invoked.
        /// </summary>
        public Control Target => control;
    }
}
