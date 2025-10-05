
using System;
using MP.Graphics.Imaging;
using MP.Graphics.Windowing.Input;
using MP.Annotations.CodeAnalysis;

namespace MP.Graphics.Windowing
{
    /// <summary>
    /// Represents a window in the OS context. <br />
    /// Different contexts and OS'es may need a different window implementation; 
    /// that's why this is an abstract class.
    /// </summary>
    public abstract class Window : INativeWindow, IDisposable
    {
        private INativeWindow parent;
        private WindowDispatcher dispatcher;
        private volatile bool shouldcloseexternalevent;

        /// <summary>
        /// Default constructor.
        /// </summary>
        public Window()
        {
            parent = null;
            dispatcher = new();
            shouldcloseexternalevent = false;
        }

        /// <summary>
        /// A method providing a value whether the window should close. <br />
        /// Usually this calls in some native method that checks whether the rendering loop should still run.
        /// </summary>
        /// <returns>A value whether the window should close.</returns>
        protected abstract bool ShouldClose();

        /// <summary>
        /// Creates the window and enters the rendering loop.
        /// </summary>
        public void Run()
        {
            Create();
            // At least one of two cases must occur
            try {
                while ((ShouldClose() | shouldcloseexternalevent) == false)
                {
                    OnBeforeDispatching();
                    dispatcher.RunDispatches();
                    OnAfterDispatching();
                }
            } finally {
                // Make sure that the state of the object is not corrupted, even on hard failures.
                shouldcloseexternalevent = false;
            }
        }

        /// <summary>
        /// This method runs every time before the dispatcher starts dispatching the queued methods. <br />
        /// This method must not throw any exceptions. If it does, the rendering loop will be broken.
        /// </summary>
        [MustNotReportException]
        protected virtual void OnBeforeDispatching() { }

        /// <summary>
        /// This method runs every time after the dispatcher has dispatched the queued methods. <br />
        /// This method must not throw any exceptions. If it does, the rendering loop will be broken.
        /// </summary>
        [MustNotReportException]
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
        /// Gets the native window descriptor assigned to this window instance.
        /// </summary>
        public abstract IntPtr Handle { get; }

        /// <summary>
        /// Gets or sets the window where this one will be parented to. <br />
        /// Not having a specific parent window is also valid.
        /// </summary>
        public INativeWindow Parent
        {
            get => parent;
            set => parent = value; // TODO: improve this API to throw once Run has been called.
        }

        /// <summary>
        /// Hides this <see cref="Window"/> instance from the OS.
        /// </summary>
        public abstract void Hide();

        /// <summary>
        /// Makes a previously hidden <see cref="Window"/> to be visible again.
        /// </summary>
        public abstract void Show();

        /// <summary>
        /// Closes this <see cref="Window"/>, terminating the rendering loop. <br />
        /// Thread-safe.
        /// </summary>
        /// <remarks>
        /// The responsibility of <see cref="Close"/> is just to terminate the rendering loop, not to dispose the window. <br />
        /// You must still call the <see cref="Dispose()"/> method to release native resources. <br /> <br />
        /// 
        /// When overriding this method, extra care must be taken to ensure that this method invocation is thread-safe. <br />
        /// It is recommended instead to call the base implementation of this method which does provide a thread-safe way to terminate the rendering loop.
        /// </remarks>
        public virtual void Close() {
            shouldcloseexternalevent = true;
        }

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
                shouldcloseexternalevent = true;
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
