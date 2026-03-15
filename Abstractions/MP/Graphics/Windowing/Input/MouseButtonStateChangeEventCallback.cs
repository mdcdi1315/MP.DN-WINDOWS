using System.Diagnostics.CodeAnalysis;

namespace MP.Graphics.Windowing.Input
{
    /// <summary>
    /// Defines the delegate signature for mouse button input events.
    /// </summary>
    /// <param name="provider">The <see cref="IInputProvider"/> that dispatched this event.</param>
    /// <param name="button">The mouse button that was pressed or released.</param>
    /// <param name="pressed">A value whether the button was pressed. If not, it is considered that it is released.</param>
    public delegate void MouseButtonStateChangeEventCallback([DisallowNull] IInputProvider provider, MouseButtonCode button, bool pressed);
}