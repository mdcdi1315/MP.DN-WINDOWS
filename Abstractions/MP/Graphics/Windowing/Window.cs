
using System;
using System.Diagnostics;
using MP.Graphics.Imaging;
using MP.Graphics.Windowing.Input;
using MP.Annotations.CodeAnalysis;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace MP.Graphics.Windowing
{
    /// <summary>
    /// Represents a window in the OS context. <br />
    /// Different contexts and OS'es may need a different window implementation; 
    /// that's why this is an abstract class.
    /// </summary>
    public abstract class Window : INativeWindow, IInputProvider, IDisposable
    {
        private System.String title;
        private volatile Flags flags;
        private INativeWindow parent;
        private WindowDispatcher dispatcher;

        [Flags]
        private enum Flags : System.Byte
        {
            None = 0,
            ShouldClose = 1 << 0,
            Running = 1 << 1,
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private bool HasFlagFast(Flags f) => (flags & f) == f;

        private static void KeyboardKeyStateChanged_DummyTarget(IInputProvider p , KeyboardKeyCode code, KeyState type, char mapping) { }

        private static void MouseButtonStateChanged_DummyTarget(IInputProvider p , MouseButtonCode button, bool pressed) { }

        private static void Closing_DummyTarget(WindowClosingEventCallbackInfo i) { }

        private static void MouseMovedScrolled_DummyTarget(IInputProvider p, double x, double y) { }

        /// <summary>
        /// Creates and runs the specified <see cref="Window"/> instance. <br />
        /// Remarks: <br />
        /// <list type="number">
        ///     <item>This must be called from the rendering thread.</item>
        ///     <item>This method does not return until either the loop is terminated or failed with an exception.</item>
        /// </list>
        /// </summary>
        /// <param name="instance">The <see cref="Window"/> instance to instantiate.</param>
        /// <exception cref="ArgumentNullException"><paramref name="instance"/> is <see langword="null"/>.</exception>
        [Throws(typeof(ArgumentNullException))]
        public static void Run(Window instance)
        {
            ArgumentNullException.ThrowIfNull(instance);
            instance.Run();
        }

        /// <summary>Default constructor.</summary>
        public Window()
        {
            parent = null;
            flags = Flags.None;
            dispatcher = new();
            Closing = new(Closing_DummyTarget);
            MouseMoved = new(MouseMovedScrolled_DummyTarget);
            MouseScrolled = new(MouseMovedScrolled_DummyTarget);
            MouseButtonStateChanged = new(MouseButtonStateChanged_DummyTarget);
            KeyboardKeyStateChanged = new(KeyboardKeyStateChanged_DummyTarget);
        }

        /// <summary>
        /// A method providing a value whether the window should close. <br />
        /// Usually this calls in some native method that checks whether the rendering loop should still run.
        /// </summary>
        /// <returns>A value whether the window should close.</returns>
        protected abstract bool ShouldClose();

        [DebuggerHidden]
        [StackTraceHidden]
        private void Run()
        {
            Create();
            // At least one of two cases must occur
            try {
                flags |= Flags.Running;
                while ((ShouldClose() | HasFlagFast(Flags.ShouldClose)) == false)
                {
                    OnBeforeDispatching();
                    dispatcher.RunDispatches();
                    OnAfterDispatching();
                }
            } finally {
                // Make sure that the state of the object is not corrupted, even on hard failures.
                flags &= ~Flags.ShouldClose;
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
        /// Gets the <see cref="Window"/> size, in OS screen measurements.
        /// </summary>
        public abstract Size Size { get; }

        /// <summary>
        /// Gets/sets the title of this <see cref="Window"/>.
        /// </summary>
        public virtual System.String Title
        {
            [return: MaybeNull]
            get => title;
            [Throws(typeof(ArgumentNullException))]
            set {
                ArgumentNullException.ThrowIfNull(value);
                title = value;
            }
        }

        /// <summary>
        /// Gets or sets the cursor to be used in the bounds of the window.
        /// </summary>
        public abstract Cursor Cursor 
        {
            [return: MaybeNull]
            get;
            [Throws(
                typeof(ArgumentNullException),
                typeof(NotSupportedException)
            )]
            set;
        }

        /// <summary>
        /// Gets or sets the icon of the window. <br />
        /// Implementers must be ensured that after a new image is provided, the older one must be disposed.
        /// </summary>
        public abstract IImage Icon 
        {
            [return: MaybeNull]
            get;
            [Throws(
                typeof(ArgumentNullException), 
                typeof(NotSupportedException)
            )]
            set; 
        }

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
        [AllowNull]
        public virtual INativeWindow Parent
        {
            [return: MaybeNull]
            get => parent;
            set {
                if (HasFlagFast(Flags.Running)) {
                    throw new InvalidOperationException("The window is running!");
                } else {
                    parent = value;
                }
            }
        }

        /// <summary>
        /// Gets the input state as reported by the OS-specific API's. <br />
        /// Must at least return an empty instance, but never <see langword="null"/>.
        /// </summary>
        [NotNull]
        public abstract InputDataState InputState { get; }

        /// <summary>
        /// Provides a way for invoking the <see cref="MouseButtonStateChanged"/> event from derived classes.
        /// </summary>
        /// <param name="data">The mouse button data providing the data for the event.</param>
        protected void OnMouseButtonStateChanged(MouseButtonData data) => MouseButtonStateChanged.Invoke(this, data.Code, data.Pressed);

        /// <summary>
        /// Provides a way for invoking the <see cref="KeyboardKeyStateChanged"/> event from derived classes.
        /// </summary>
        /// <param name="data">The keyboard data providing the data for the event.</param>
        protected void OnKeyboardKeyStateChanged(KeyboardKeyData data) => KeyboardKeyStateChanged.Invoke(this, data.Code, data.State , data.Key);

        /// <summary>
        /// Provides a way for invoking the <see cref="MouseMoved"/> event from derived classes.
        /// </summary>
        /// <param name="x_position">The new X-coordinate position of the cursor.</param>
        /// <param name="y_position">The new Y-coordinate position of the cursor.</param>
        protected void OnMouseMoved(double x_position, double y_position) => MouseMoved.Invoke(this, x_position, y_position);

        /// <summary>
        /// Provides a way for invoking the <see cref="MouseScrolled"/> event from derived classes.
        /// </summary>
        /// <param name="x_offset">The horizontal offset that the mouse wheel was moved.</param>
        /// <param name="y_offset">The vertical offset that the mouse wheel was moved.</param>
        protected void OnMouseScrolled(double x_offset, double y_offset) => MouseScrolled.Invoke(this, x_offset, y_offset);

        /// <inheritdoc />
        public event MouseButtonStateChangeEventCallback MouseButtonStateChanged;

        /// <inheritdoc />
        public event KeyboardKeyStateChangeEventCallback KeyboardKeyStateChanged;

        /// <inheritdoc />
        public event MouseMovedEventCallback MouseMoved;

        /// <inheritdoc />
        public event MouseScrolledEventCallback MouseScrolled;

        /// <summary>
        /// Gets the event that is called before the window is closed. <br />
        /// This event is invoked on the thread that called the <see cref="Close"/> method.
        /// </summary>
        public event WindowClosingEventCallback Closing;

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
        /// </remarks>
        public void Close() 
        {
            WindowClosingEventCallbackInfo i = new(this);
            try {
                Closing.Invoke(i);
                if (i.ShouldClose) {
                    flags |= Flags.ShouldClose;
                } else {
                    flags &= ~Flags.ShouldClose;
                }
            } catch (Exception e) {
                DebugProvider.WriteLine(String.Format("Cannot call the window closing event due to an exception: {0}\nTerminating the window anyway." , e));
                flags |= Flags.ShouldClose;
            }
        }

        /// <summary>
        /// Must be provided by extending classes to dispose native resources.
        /// </summary>
        /// <param name="disposing">A value whether disposal runs from the <see cref="Dispose()"/> method; otherwise, it runs from the finalizer.</param>
        protected abstract void Dispose(bool disposing);

        /// <summary>
        /// Releases all the resources and the data used by the current <see cref="Window"/>.
        /// </summary>
        public void Dispose()
        {
            try {
                flags |= Flags.ShouldClose; // Set the close flag if not already assigned before
                Dispose(true);
            } finally {
                if (dispatcher is not null) {
                    dispatcher.Dispose();
                    dispatcher = null;
                }
                GC.SuppressFinalize(this);
            }
        }

        /// <summary>
        /// Default finalizer implementation.
        /// </summary>
        ~Window() => Dispose(false);

        InputDataState IInputProvider.State => InputState;
    }
}
