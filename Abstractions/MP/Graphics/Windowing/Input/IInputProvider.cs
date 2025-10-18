
using System;
using MP.Annotations.CodeAnalysis;
using System.Diagnostics.CodeAnalysis;

namespace MP.Graphics.Windowing.Input
{
    /// <summary>
    /// Provides the object that provides window input. <br />
    /// Implementers of this interface are not required to extend the <see cref="Window"/> class;
    /// the data might come from a virtual window or are directly read by the OS input API's.
    /// </summary>
    public interface IInputProvider
    {
        /// <summary>
        /// Gets or sets the <see cref="Input.Cursor"/> class instance that is provided by the window provider. 
        /// Getting it may be <see langword="null"/>, and it is possible when setting a new cursor to not be supported.
        /// </summary>
        /// <exception cref="NotSupportedException">Setting a new cursor is not supported.</exception>
        public Cursor Cursor 
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
        /// Gets the input state as reported by the OS-specific API's. <br />
        /// Must at least return an empty instance, but never <see langword="null"/>.
        /// </summary>
        [NotNull]
        public InputDataState State { get; }

        /// <summary>
        /// An event that is fired when the state of one of the mouse buttons was changed.
        /// </summary>
        public event MouseButtonStateChangeEventCallback MouseButtonStateChanged;

        /// <summary>
        /// An event that is fired when the state of one of the keyboard buttons was changed.
        /// </summary>
        public event KeyboardKeyStateChangeEventCallback KeyboardKeyStateChanged;
    }
}