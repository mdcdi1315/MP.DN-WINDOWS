using System;
using System.Runtime.InteropServices;

namespace MP
{
    /// <summary>
    /// Defines a lightweight abstraction for managing dynamically-loaded library handles and getting function ordinals from them.
    /// </summary>
    public unsafe interface ILibraryHandle : IDisposable
    {
        /// <summary>
        /// Gets a function ordinal from the native library and returns it's function pointer.
        /// </summary>
        /// <param name="name">The name of the function ordinal to load.</param>
        /// <returns>The native function pointer.</returns>
        public void* GetFunction(System.String name);

        /// <summary>
        /// Gets the underlying OS handle for the currently dynamically-loaded library.
        /// </summary>
        public System.IntPtr Handle { get; }
    }

    /// <summary>
    /// Defines extension methods for the <see cref="ILibraryHandle"/> interface.
    /// </summary>
    public static unsafe class ILibraryHandleExtensions
    {
        /// <summary>
        /// Gets a function ordinal from the native library and returns it's marshalled .NET delegate.
        /// </summary>
        /// <typeparam name="T">The type of the native delegate to be returned.</typeparam>
        /// <param name="lh">The library handle.</param>
        /// <param name="name">The name of the function ordinal to load.</param>
        /// <returns>The marshalled .NET delegate, that, when invoking it, invokes the requested function.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="name"/> was <see langword="null"/>.</exception>
        /// <exception cref="ArgumentException"><paramref name="name"/> represented the empty string, or it contained white-space characters only.</exception>
        public static T GetFunction<T>(this ILibraryHandle lh, System.String name)
            where T : Delegate
        {
            ArgumentException.ThrowIfNullOrEmpty(name);
            return Marshal.GetDelegateForFunctionPointer<T>(new(lh.GetFunction(name)));
        }
    }
}
