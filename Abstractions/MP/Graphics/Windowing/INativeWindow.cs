using System;

namespace MP.Graphics.Windowing
{
    /// <summary>
    /// Defines a mechanism for accessing the native reprsentation of windows in a user interface-based OS system.
    /// </summary>
    public interface INativeWindow : IDisposable
    {
        /// <summary>
        /// Gets a handle to the native descriptor of the window <br />
        /// It must be assumed as an arbitrary memory pointer, since this interface is platform-agnostic, even when a graphics library provides this pointer.
        /// </summary>
        public System.IntPtr Handle { get; }
    }
}
