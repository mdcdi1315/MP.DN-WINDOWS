using System;

namespace MP
{
    /// <summary>
    /// Defines a lightweight abstraction for managing library handles and getting function ordinals from them.
    /// </summary>
    public interface ILibraryHandle : IDisposable
    {
        /// <summary>
        /// Gets a function ordinal from the native library and returns it's marshalled .NET delegate.
        /// </summary>
        /// <typeparam name="T">The type of the native delegate to be returned.</typeparam>
        /// <param name="name">The name of the function ordinal to load.</param>
        /// <returns>The marshalled .NET delegate, that, when invoking it, invokes the requested function.</returns>
        public T GetFunction<T>(System.String name) where T : Delegate;

        /// <summary>
        /// Gets the underlying OS handle for the current loaded library.
        /// </summary>
        public System.IntPtr Handle { get; }
    }
}
