
using System;
using System.Buffers;
using System.Threading;
using MP.Annotations.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Diagnostics.CodeAnalysis;

namespace MP.IO.Buffers
{
    /// <summary>
    /// Provides information about an acquired buffer, along with the size of the buffer that was requested. <br />
    /// Useful in I/O tasks, where you can use this with a <see langword="using"/> context.
    /// </summary>
    /// <typeparam name="T">The type of data the <see cref="ArrayPoolBufferAcquireContext{T}.Buffer"/> field will hold.</typeparam>
    public sealed class ArrayPoolBufferAcquireContext<T> : IDisposable
    {
        // A method signature delegating to the ArrayPool.Return method.
        // It is useful to only keep the method reference, and not to retain the pool itself.
        private delegate void ReturnBufferMethod(T[] array, bool clear_array = false);

        /// <summary>The requested buffer size.</summary>
        public readonly int Size;
        /// <summary>The buffer that was returned.</summary>
        [NotNull]
        public readonly T[] Buffer;
        private ReturnBufferMethod ret_buffer_method;

        /// <summary>
        /// Initializes a new instance of the <see cref="ArrayPoolBufferAcquireContext{T}"/> structure,
        /// that rents a buffer of the specified size by the specified array pool.
        /// </summary>
        /// <param name="size">The desired size that the rented buffer will have.</param>
        /// <param name="pool">The <see cref="ArrayPool{T}"/> instance to rent the buffer from.</param>
        /// <exception cref="ArgumentNullException"><paramref name="pool"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="size"/> is a negative value.</exception>
        [Throws(typeof(ArgumentOutOfRangeException))]
        public ArrayPoolBufferAcquireContext(int size, ArrayPool<T> pool)
        {
            ArgumentNullException.ThrowIfNull(pool);
            if (size < 0) {
                throw new ArgumentOutOfRangeException(nameof(size), "Buffer size cannot be a negative value.");
            } else {
                Size = size;
                Buffer = pool.Rent(size);
                ret_buffer_method = new(pool.Return);
            }
        }

        /// <summary>
        /// Gets the actual length of <see cref="Buffer"/>.
        /// </summary>
        public int ActualLength => Buffer.Length;

        /// <summary>Implicit operator to directly acquire the buffer reference.</summary>
        /// <param name="c">The context to retrieve the rented buffer.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)] // The JIT should aggressively inline it, when possible.
        public static implicit operator T[](ArrayPoolBufferAcquireContext<T> c) => c.Buffer;

        /// <summary>Gets a reference to the 0th element of <see cref="Buffer"/>, if not yet freed.</summary>
        /// <remarks>The method will return <see cref="Unsafe.NullRef{T}"/> if <see cref="Dispose"/> has been called on this structure instance.</remarks>
        /// <returns>The reference at the 0th element of <see cref="Buffer"/>.</returns>
        [return: MaybeNull]
        public ref T GetPinnableReference()
        {
            if (ret_buffer_method is null) {
                return ref Unsafe.NullRef<T>();
            } else {
                return ref Buffer[0];
            }
        }

        /// <summary>
        /// Returns the rented buffer back to the pool, and then unreferences the pool that rented the buffer.
        /// </summary>
        public void Dispose()
        {
            // Guard the call if in case of multiple threads.
            Monitor.Enter(this);
            try {
                if (ret_buffer_method is not null)
                {
                    ret_buffer_method.Invoke(Buffer);
                    ret_buffer_method = null;
                }
            } finally {
                Monitor.Exit(this);
            }
        }
    }
}