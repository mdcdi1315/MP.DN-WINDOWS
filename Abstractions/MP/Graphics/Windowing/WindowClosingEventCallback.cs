
using MP.Annotations.CodeAnalysis;
using System.Diagnostics.CodeAnalysis;

namespace MP.Graphics.Windowing
{
    /// <summary>Defines the delegate signature for the window closing event.</summary>
    /// <param name="information">The information that are associated with the window closing event.</param>
    [CallerMustHandleExceptions]
    public delegate void WindowClosingEventCallback([DisallowNull] WindowClosingEventCallbackInfo information);
}