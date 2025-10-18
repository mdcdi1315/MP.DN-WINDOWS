


namespace MP.Graphics.Windowing.Input
{
    /// <summary>
    /// Provides the different states that a keyboard key might be into.
    /// </summary>
    public enum KeyState : System.Byte
    {
        /// <summary>A key was pressed in the keyboard.</summary>
        Pressed,
        /// <summary>A key is still being pressed in the keyboard. May be not supported in all cases.</summary>
        Held,
        /// <summary>A key was released in the keyboard.</summary>
        Released,
    }
}