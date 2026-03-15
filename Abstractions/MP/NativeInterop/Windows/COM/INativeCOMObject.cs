
using System;
using MP.Annotations.CodeAnalysis;
using System.Diagnostics.CodeAnalysis;

namespace MP.NativeInterop.Windows.COM
{
    /// <summary>
    /// An abstraction of a native wrapper for a COM interface. <br />
    /// This is the base RCW abstraction for COM objects imported through this mechanism. <br />
    /// Typically, this interface is implemented by the RCW proxy object which is generated through the MP COM source generator.
    /// </summary>
    public unsafe interface INativeCOMObject
    {
        /// <summary>
        /// Gets the native pointer passed to the creation method.
        /// </summary>
        void* Native { get; }

        /// <summary>
        /// Casts this native COM Object returned through the COM interop services to the specified interface type.
        /// </summary>
        /// <typeparam name="T">The interface type to cast this native COM object.</typeparam>
        /// <returns>The casted instance to <typeparamref name="T"/>.</returns>
        /// <exception cref="InvalidCastException">This native COM object does not implement the interface of type <typeparamref name="T"/>.</exception>
        [return: NotNull]
        [Throws(typeof(InvalidCastException))]
        sealed T Cast<T>() where T : class => (T)this;
    }
}