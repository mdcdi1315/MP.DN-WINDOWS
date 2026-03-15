

namespace MP.Graphics.Windowing.Input
{
    /// <summary>
    /// Provides information about a pressed mouse button.
    /// </summary>
    public readonly struct MouseButtonData
    {
        /// <summary>
        /// Defines the mouse button that had it's state changed.
        /// </summary>
        public readonly MouseButtonCode Code;

        /// <summary>
        /// A value whether the button was pressed or not.
        /// </summary>
        public readonly bool Pressed;

        /// <summary>
        /// Constructs a new <see cref="MouseButtonData"/> structure from the mouse button code and whether that button was pressed or not.
        /// </summary>
        /// <param name="code">The mouse button that it's state was changed.</param>
        /// <param name="pressed">A value whether the mouse button was pressed or not.</param>
        public MouseButtonData(MouseButtonCode code, bool pressed)
        {
            Code = code;
            Pressed = pressed;
        }
    }
}