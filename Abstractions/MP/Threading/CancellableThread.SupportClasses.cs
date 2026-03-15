
using System.Diagnostics.CodeAnalysis;

namespace MP.Threading
{
    /// <summary>
    /// Provides details about the cancellable thread. <br />
    /// This is internally implemented by the <see cref="CancellableThread"/> class.
    /// </summary>
    public interface ICancellableThreadDetails
    {
        /// <summary>
        /// Gets a value whether the current running thread should cancel the work is doing and return immediately.
        /// </summary>
        public bool ShouldCancel { get; }

        /// <summary>
        /// Gets a user-defined value to additionally pass to the thread code.
        /// </summary>
        [MaybeNull]
        public object AdditionalData { get; }
    }

    /// <summary>Provides the signature for cancellable threads.</summary>
    /// <param name="details">An instance of the cancellable thread details. This will never be <see langword="null"/>.</param>
    public delegate void CancellableThreadStart([DisallowNull] ICancellableThreadDetails details);
}