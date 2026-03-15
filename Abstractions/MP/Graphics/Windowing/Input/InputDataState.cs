

namespace MP.Graphics.Windowing.Input
{
    /// <summary>
    /// Defines the input state at the time that the <see cref="IInputProvider.State"/> property was invoked. <br />
    /// More data may be supported, based on what the OS-specific API's do provide.
    /// </summary>
    public abstract class InputDataState
    {
        /// <summary>
        /// Gets the key that it's state changed before the call to the OS-specific API's do occur.
        /// </summary>
        public abstract KeyboardKeyData LastKeyboardKeyState { get; }

        /// <summary>
        /// Gets the mouse button that it's state changed before the call to the OS-specific API's do occur.
        /// </summary>
        public abstract MouseButtonData LastMouseButtonState { get; }

        /// <summary>
        /// Gets a value whether an input event was actually recorded in this instance. <br />
        /// When no input events are recorded, this class must have empty values for the properties <see cref="LastKeyboardKeyState"/> and <see cref="LastMouseButtonState"/>.
        /// </summary>
        public virtual bool HasInputData =>
            LastKeyboardKeyState.Code != KeyboardKeyCode.Unmapped ||
            LastMouseButtonState.Code != MouseButtonCode.Undefined;

        /// <summary>
        /// Gets the location of the cursor in screen coordinates.
        /// </summary>
        public abstract Point CursorLocation { get; }
    }
}