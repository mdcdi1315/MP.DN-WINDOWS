

using System;
using System.Diagnostics.CodeAnalysis;

namespace MP
{
    /// <summary>
    /// Defines a way to retrieve native memory blocks through a simple mechanism.
    /// </summary>
    [SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "This is a factory interface.")]
    public interface MemoryHandleFactory
    {
        /// <summary>
        /// Creates a new native memory handle of the specified size, and returns it.
        /// </summary>
        /// <param name="size">The size of the newly created memory handle.</param>
        /// <returns>The memory block of the size requested in the <paramref name="size"/> parameter.</returns>
        /// <exception cref="OutOfMemoryException">Not enough memory to create the memory handle.</exception>
        public abstract IMemoryHandle CreateMemoryHandle(System.UInt64 size);

        /// <summary>
        /// Returns platform memory statistics, if these are available. <br />
        /// The values supported by each platform may be different, and maybe these do not even exist, where in such case this method returns <see langword="null"/>.
        /// </summary>
        /// <returns>An object extending the <see cref="IAttributeable"/> interface.</returns>
        [return: MaybeNull]
        public abstract IAttributeable GetStatistics();
    }
}