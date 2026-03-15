using System.Diagnostics.CodeAnalysis;

namespace MP.Graphics.Windowing.Input
{
    /// <summary>Provides the delegate for mouse cursor moved events.</summary>
    /// <param name="provider">The <see cref="IInputProvider"/> that dispatched the event.</param>
    /// <param name="x_position">The new X-coordinate position of the mouse.</param>
    /// <param name="y_position">The new Y-coordinate position of the mouse.</param>
    public delegate void MouseMovedEventCallback([DisallowNull] IInputProvider provider, double x_position, double y_position);
}