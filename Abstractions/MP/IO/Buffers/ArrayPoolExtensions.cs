
using System;
using System.Buffers;
using MP.Annotations.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace MP.IO.Buffers
{
    /// <summary>
    /// Extension methods for the <see cref="ArrayPool{T}"/> class.
    /// </summary>
    public static class ArrayPoolExtensions
    {
        /// <summary>
        /// Rents a buffer from the current <see cref="ArrayPool{T}"/> object, 
        /// and returns it through a <see cref="ArrayPoolBufferAcquireContext{T}"/> instance that is suitable
        /// to be used in I/O tasks with <see langword="try"/> and <see langword="using"/> constructs.
        /// </summary>
        /// <typeparam name="T">The type of the elements that the rented buffer will have.</typeparam>
        /// <param name="pool">The <see cref="ArrayPool{T}"/> instance to rent the buffer from.</param>
        /// <param name="size">The size of the newly rented buffer.</param>
        /// <returns>An <see cref="ArrayPoolBufferAcquireContext{T}"/> instance.</returns>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="size"/> is a negative value.</exception>
        [Throws(typeof(ArgumentOutOfRangeException))]
        [MethodImpl(MethodImplOptions.AggressiveInlining)] // The benefit is greater if the JIT just invokes the context constructor directly.
        public static ArrayPoolBufferAcquireContext<T> RentByContext<T>(this ArrayPool<T> pool, int size) => new(size, pool);
    }
}