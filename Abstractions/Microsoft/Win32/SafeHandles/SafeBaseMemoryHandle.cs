using MP.NativeInterop;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Microsoft.Win32.SafeHandles
{
    /// <summary>
    /// Defines the base handle class for native memory handles. <br />
    /// Implements the <see cref="IMemoryHandle"/> interface.
    /// </summary>
    public unsafe abstract class SafeBaseMemoryHandle : CriticalHandleZeroOrMinusOneIsInvalid , IMemoryHandle
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        protected SafeBaseMemoryHandle() { }

        /// <summary>
        /// Gets the length of this memory block in bytes.
        /// </summary>
        public abstract System.UInt64 MemoryLength { get; }

        /// <summary>
        /// Gets the actual pointer to the memory block.
        /// </summary>
        public virtual System.Byte* MemoryPointer
        {
            get {
                ObjectDisposedException.ThrowIf(IsClosed || IsInvalid, this);
                return (System.Byte*)handle.ToPointer();
            }
        }

        /// <summary>
        /// Gets or sets a byte of the memory block at the specified index.
        /// </summary>
        /// <param name="index">The index inside the memory block bounds of the element to set.</param>
        /// <returns>The byte at <paramref name="index"/>.</returns>
        /// <exception cref="ObjectDisposedException">The object is disposed.</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="index"/> was outside the memory block bounds or was negative.</exception>
        public System.Byte this[System.UInt64 index] 
        {
            get {
                ObjectDisposedException.ThrowIf(IsClosed || IsInvalid, this);
                if (index < 0 || index >= MemoryLength) { throw new ArgumentOutOfRangeException(nameof(index) , "Index must be positive or zero and less than the memory length."); }
                return *(MemoryPointer + index);
            }
            set {
                ObjectDisposedException.ThrowIf(IsClosed || IsInvalid, this);
                if (index < 0 || index >= MemoryLength) { throw new ArgumentOutOfRangeException(nameof(index), "Index must be positive or zero and less than the memory length."); }
                *(MemoryPointer + index) = value;
            }
        }

        /// <summary>
        /// Gets a native memory enumerator that can enumerate through the native memory elements.
        /// </summary>
        /// <returns>A new instance of the <see cref="SafeNativeMemoryEnumerator"/> class.</returns>
        public SafeNativeMemoryEnumerator GetEnumerator() => new(this);

        IEnumerator<System.Byte> IEnumerable<System.Byte>.GetEnumerator() => GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
