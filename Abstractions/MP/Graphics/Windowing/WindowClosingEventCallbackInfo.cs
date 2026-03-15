
using System;
using MP.Annotations.CodeAnalysis;

namespace MP.Graphics.Windowing
{
    /// <summary>
    /// Provides information for any event that it's signature is <see cref="WindowClosingEventCallback"/>.
    /// </summary>
    public sealed class WindowClosingEventCallbackInfo
    {
        private readonly Window window;
        private bool shouldclose;

        /// <summary>
        /// Creates a new instance of the <see cref="WindowClosingEventCallbackInfo"/> class, defining the <see cref="Windowing.Window"/> that caused the event to fire.
        /// </summary>
        /// <param name="window">The <see cref="Window"/> to create the object from.</param>
        [Throws(typeof(ArgumentNullException))]
        public WindowClosingEventCallbackInfo(Window window)
        {
            ArgumentNullException.ThrowIfNull(window);
            this.window = window;
            shouldclose = true;
        }

        /// <summary>
        /// Gets the <see cref="Windowing.Window"/> instance that caused the event to fire.
        /// </summary>
        public Window Window => window;

        /// <summary>
        /// Gets or sets a value whether the window should close. <br />
        /// By default, it is set to <see langword="true"/>.
        /// </summary>
        /// <returns>
        /// A value whether the window will close once the event finishes execution. <br />
        /// <see langword="true"/> means that the window will be closed;
        /// otherwise, <see langword="false"/> and the window will not be closed.
        /// </returns>
        public bool ShouldClose
        {
            get => shouldclose;
            set => shouldclose = value;
        }

    }
}