

namespace MP.Graphics.Windowing.Input
{
    /// <summary>
    /// Defines the common mouse buttons that are found in mouse input devices. <br />
    /// All input API's should translate values based on this enumeration members.
    /// </summary>
    public enum MouseButtonCode : System.Byte
    {
        /// <summary>
        /// Defines a dummy value, meaning that the value retrieved from the system cannot be mapped to one of this enumeration's members.
        /// </summary>
        Undefined,
        /// <summary>
        /// Defines the left mouse button.
        /// </summary>
        Left,
        /// <summary>
        /// Defines the scroll wheel mouse button. <br />
        /// It is called that way because it is in between of the two primary mouse buttons.
        /// </summary>
        Middle,
        /// <summary>
        /// Defines the right mouse button.
        /// </summary>
        Right,
        /// <summary>Defines the mouse button 0.</summary>
        Extra0,
        /// <summary>Defines the mouse button 1.</summary>
        Extra1,
        /// <summary>Defines the mouse button 2.</summary>
        Extra2,
        /// <summary>Defines the mouse button 3.</summary>
        Extra3,
        /// <summary>Defines the mouse button 4.</summary>
        Extra4,
        /// <summary>Defines the mouse button 5.</summary>
        Extra5,
    }
}