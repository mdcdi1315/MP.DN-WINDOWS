namespace MP.Graphics.Windowing.Input
{
    /// <summary>
    /// Defines the delegate signature for keyboard input events.
    /// </summary>
    /// <param name="window">The <see cref="Window"/> that fired the event.</param>
    /// <param name="code">The key that was hit or released on the keyboard.</param>
    /// <param name="type">The key's status, whether the key was released or not.</param>
    public delegate void KeyHitEventCallback(Window window, KeyboardKeyCode code, KeyHitType type);
}