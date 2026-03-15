using System.Diagnostics.CodeAnalysis;

namespace MP.Graphics.Windowing.Input
{
    /// <summary>Provides the delegate for mouse scrolled events.</summary>
    /// <param name="provider">The <see cref="IInputProvider"/> that dispatched the event.</param>
    /// <param name="x_offset">The horizontal offset that the mouse wheel was moved.</param>
    /// <param name="y_offset">The vertical offset that the mouse wheel was moved.</param>
    public delegate void MouseScrolledEventCallback([DisallowNull] IInputProvider provider, double x_offset, double y_offset);
}