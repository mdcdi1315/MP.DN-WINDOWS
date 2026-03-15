
using System;
using System.Collections;
using System.Collections.Generic;

namespace Microsoft.Win32.SafeHandles
{
    /// <summary>
    /// Defines the enumerator implementation for the derivants of the <see cref="SafeBaseMemoryHandle"/> class.
    /// </summary>
    public unsafe sealed class SafeNativeMemoryEnumerator : IEnumerator<System.Byte>
    {
        private System.Boolean reseted;
        private System.Byte* ptrcurrent;
        private SafeBaseMemoryHandle handle;

        /// <summary>
        /// Creates a new memory block enumerator that will enumerate all the elements of the handle.
        /// </summary>
        /// <param name="handle">The memory block to enumerate all it's elements.</param>
        /// <exception cref="ArgumentNullException"><paramref name="handle"/> was <see langword="null"/>.</exception>
        public SafeNativeMemoryEnumerator(SafeBaseMemoryHandle handle)
        {
            if (handle is null || handle.IsClosed || handle.IsInvalid)
            {
                throw new ArgumentNullException(nameof(handle));
            }
            this.handle = handle;
            reseted = true;
            ptrcurrent = handle.MemoryPointer - 1;
        }

        /// <summary>
        /// Moves to the next byte into the memory block. If no more bytes do exist , then it returns <see langword="false"/>.
        /// </summary>
        /// <returns>A value whether moving to the next byte was successfull or not.</returns>
        /// <exception cref="ObjectDisposedException">The enumerator has been disposed and does not have access to the handle.</exception>
        public System.Boolean MoveNext()
        {
            if (ptrcurrent is null) { throw new ObjectDisposedException(nameof(SafeNativeMemoryEnumerator)); }
            reseted = false;
            return (ulong)(++ptrcurrent - handle.MemoryPointer) < handle.MemoryLength;
        }

        /// <summary>
        /// Gets the current byte from the memory block.
        /// </summary>
        /// <exception cref="ObjectDisposedException">The enumerator has been disposed and does not have access to the handle.</exception>
        /// <exception cref="InvalidOperationException">The enumeration has not begun yet.</exception>
        public System.Byte Current
        {
            get
            {
                if (ptrcurrent is null) { throw new ObjectDisposedException(nameof(SafeNativeMemoryEnumerator)); }
                if (reseted) { throw new InvalidOperationException("The enumeration has not begun yet."); }
                return *ptrcurrent;
            }
        }

        System.Object IEnumerator.Current => Current;

        /// <summary>
        /// Resets the enumeration before the first memory element in the block.
        /// </summary>
        /// <exception cref="ObjectDisposedException">This enumerator has been disposed of.</exception>
        public void Reset()
        {
            if (ptrcurrent is null) { throw new ObjectDisposedException(nameof(SafeNativeMemoryEnumerator)); }
            ptrcurrent = handle.MemoryPointer - 1;
            reseted = true;
        }

        /// <summary>
        /// Disposes this enumerator instance.
        /// </summary>
        public void Dispose()
        {
            ptrcurrent = null;
            handle = null;
        }
    }
}