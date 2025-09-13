
using System;
using MP.Graphics.Imaging;

namespace MP.Graphics.Windowing
{
    /// <summary>
    /// Represents a window in the OS context. <br />
    /// Different contexts and OS'es may need a different window implementation; 
    /// that's why this is an abstract class.
    /// </summary>
    public abstract class Window : IDisposable
    {
        private WindowDispatcher dispatcher;

        /// <summary>
        /// Default constructor.
        /// </summary>
        public Window()
        {
            dispatcher = new();
        }

        /// <summary>
        /// A method providing a value whether the window should close.
        /// </summary>
        /// <returns>A value whether the window should close.</returns>
        protected abstract bool ShouldClose();

        /// <summary>
        /// Creates the window and enters the rendering loop.
        /// </summary>
        public void Run()
        {
            Create();
            while (ShouldClose() == false)
            {
                OnBeforeDispatching();
                dispatcher.RunDispatches();
                OnAfterDispatching();
            }
        }

        /// <summary>
        /// This method runs every time before the dispatcher starts dispatching the queued methods.
        /// </summary>
        protected virtual void OnBeforeDispatching() { }

        /// <summary>
        /// This method runs every time after the dispatcher has dispatched the queued methods.
        /// </summary>
        protected virtual void OnAfterDispatching() { }

        /// <summary>
        /// Creates the window , setting it up for a new window session.
        /// </summary>
        protected abstract void Create();

        /// <summary>
        /// Gets the <see cref="Window"/> size, in pixels.
        /// </summary>
        public abstract Size Size { get; }

        /// <summary>
        /// Gets or sets the cursor to be used in the bounds of the window.
        /// </summary>
        public abstract Cursor Cursor { get; set; }

        /// <summary>
        /// Gets or sets the icon of the window. <br />
        /// Implementers must be ensured that after a new image is provided, the older one must be disposed.
        /// </summary>
        public abstract IImage Icon { get; set; }

        /// <summary>
        /// Gets the <see cref="WindowDispatcher"/> instance for this <see cref="Window"/>.
        /// </summary>
        public WindowDispatcher Dispatcher => dispatcher;
        
        /// <summary>
        /// Must be provided by extending classes to dispose native resources.
        /// </summary>
        /// <param name="disposing">A value whether disposal succeeded.</param>
        protected abstract void Dispose(bool disposing);

        /// <summary>
        /// Releases all the resources and the data used by the current <see cref="Window"/>.
        /// </summary>
        public void Dispose()
        {
            try {
                Dispose(true);
            } finally {
                GC.SuppressFinalize(this);
            }
        }

        /// <summary>
        /// Default destructor implementation.
        /// </summary>
        ~Window() => Dispose(false);
    }
}
