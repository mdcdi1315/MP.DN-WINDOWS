using System.Diagnostics.CodeAnalysis;

namespace MP.Graphics.Windowing.Input
{
    /// <summary>
    /// Defines the delegate signature for keyboard input events.
    /// </summary>
    /// <param name="provider">The <see cref="IInputProvider"/> that fired the event.</param>
    /// <param name="code">The key that was hit or released on the keyboard.</param>
    /// <param name="type">The key's status, whether the key was released or not.</param>
    /// <param name="mapping">Holds the Unicode character mapping for the currently pressed key.</param>
    public delegate void KeyboardKeyStateChangeEventCallback([DisallowNull] IInputProvider provider, KeyboardKeyCode code, KeyState type , char mapping);
}