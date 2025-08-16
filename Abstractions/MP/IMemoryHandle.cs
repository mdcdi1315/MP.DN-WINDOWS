using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace MP
{
    /// <summary>
    /// Defines a lightweight abstraction for native memory handles. <br />
    /// It tries to become as most as possible type-safe although that native handles never are in fact.
    /// </summary>
    public unsafe interface IMemoryHandle : IEnumerable<System.Byte> , IDisposable
    {
        /// <summary>
        /// Gets the memory handle's length in bytes.
        /// </summary>
        public System.Int32 MemoryLength { get; }

        /// <summary>
        /// Gets a pointer to the allocated memory block.
        /// </summary>
        public System.Byte* MemoryPointer { get; }

        /// <summary>
        /// Gets or sets a byte inside the memory, specifying which byte to manipulate in the <paramref name="index"/> parameter.
        /// </summary>
        /// <param name="index">The byte to be either set or got.</param>
        /// <returns>The retrieved byte at <paramref name="index"/>.</returns>
        public System.Byte this[System.Int32 index] { get; set; }

        /// <summary>
        /// Gets a value whether this memory block is no longer accessible by clients.
        /// </summary>
        public System.Boolean IsClosed { get; }
    }

    /// <summary>
    /// Defines basic extension methods for the <see cref="IMemoryHandle"/> interface.
    /// </summary>
    public unsafe static class MemoryHandleExtensions
    {
        /// <summary>
        /// Returns the handle's native data to a new managed array.
        /// </summary>
        /// <param name="handle">The handle to retrieve the native data from.</param>
        /// <returns>A new managed array with the contents of <paramref name="handle"/>.</returns>
        public static System.Byte[] ToManaged(this IMemoryHandle handle)
        {
            System.Byte[] ret = new System.Byte[handle.MemoryLength];
            fixed (System.Byte* dst = ret)
            {
                Unsafe.CopyBlockUnaligned(dst, handle.MemoryPointer, handle.MemoryLength.ToUInt32());
            }
            return ret;
        }

        /// <summary>
        /// Returns the handle's native data to a new managed array, of type <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T">The type of every single element in the returned array.</typeparam>
        /// <param name="handle">The handle to retrieve the native data from.</param>
        /// <returns>A new managed array of type <typeparamref name="T"/> with the contents of <paramref name="handle"/>.</returns>
        /// <exception cref="ArgumentException">Memory handle length is not <typeparamref name="T"/>-aligned.</exception>
        public static T[] ToManagedTArray<T>(this IMemoryHandle handle)
            where T : unmanaged
        {
            System.Int32 tsize = sizeof(T), memlen = handle.MemoryLength;
            if (memlen % tsize != 0)
            {
                throw new ArgumentException("Memory handle length is not T-aligned.");
            }
            T[] ret = new T[memlen / tsize];
            fixed (T* dst = ret)
            {
                Unsafe.CopyBlockUnaligned(dst, handle.MemoryPointer, memlen.ToUInt32());
            }
            return ret;
        }

        /// <summary>
        /// Zeroes the memory's contents.
        /// </summary>
        /// <param name="handle">The memory handle to operate on.</param>
        public static void ZeroMemory(this IMemoryHandle handle)
            => Unsafe.InitBlockUnaligned(handle.MemoryPointer, 0, handle.MemoryLength.ToUInt32());

        /// <summary>
        /// Zeroes the memory's contents. <br />
        /// Unlike the <see cref="ZeroMemory"/> method, this one assures that does always perform a memory clear of the block,
        /// suitable for cryptographic operations.
        /// </summary>
        /// <param name="handle">The memory handle to operate on.</param>
        // NoOptimize to prevent the optimizer from deciding this call is unnecessary
        // NoInlining to prevent the inliner from forgetting that the method was no-optimize
        [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
        public static void CryptographicZeroMemory(this IMemoryHandle handle)
            => Unsafe.InitBlockUnaligned(handle.MemoryPointer, 0, handle.MemoryLength.ToUInt32());

        /// <summary>
        /// Initializes a native memory block from a preexisting managed array.
        /// </summary>
        /// <param name="handle">The safe handle to initialize from.</param>
        /// <param name="bytes">The managed array.</param>
        /// <param name="index">The index of the element inside <paramref name="bytes"/> to start copying from.</param>
        /// <param name="count">The number of bytes to copy to the memory block.</param>
        /// <exception cref="ArgumentNullException"><paramref name="bytes"/> was null.</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="index"/> or <paramref name="count"/> have invalid values.</exception>
        public static void FromManaged(this IMemoryHandle handle, System.Byte[] bytes, System.Int32 index, System.Int32 count)
        {
            if (bytes is null) { throw new ArgumentNullException(nameof(bytes)); }
            if (index < 0) { throw new ArgumentOutOfRangeException(nameof(index), "Index must not be negative."); }
            if (count < 0) { throw new ArgumentOutOfRangeException(nameof(count), "Count must not be negative."); }
            if (count - index > handle.MemoryLength)
            {
                throw new ArgumentOutOfRangeException(nameof(bytes), "The native handle is not large enough so as to fit the selected region.");
            }
            fixed (System.Byte* src = &bytes[index])
            {
                Unsafe.CopyBlockUnaligned(handle.MemoryPointer, src, count.ToUInt32());
            }
        }

        /// <summary>
        /// Copies the data of a native memory block to another block.
        /// </summary>
        /// <param name="srchandle">The source memory block.</param>
        /// <param name="dsthandle">The destination memory block.</param>
        /// <exception cref="ArgumentNullException"><paramref name="dsthandle"/> was null.</exception>
        /// <exception cref="ObjectDisposedException"><paramref name="dsthandle"/> was disposed.</exception>
        /// <exception cref="InsufficientMemoryException"><paramref name="dsthandle"/> is not large enough to accomondate the contents of <paramref name="srchandle"/>.</exception>
        public static void CopyTo(this IMemoryHandle srchandle, IMemoryHandle dsthandle)
        {
            if (dsthandle is null) { throw new ArgumentNullException(nameof(dsthandle)); }
            if (dsthandle.IsClosed) { throw new ObjectDisposedException(nameof(dsthandle)); }
            if (dsthandle.MemoryLength < srchandle.MemoryLength) { throw new InsufficientMemoryException("The destination handle is not large enough to accomondate the data of the current handle."); }
            Unsafe.CopyBlockUnaligned(dsthandle.MemoryPointer, srchandle.MemoryPointer, srchandle.MemoryLength.ToUInt32());
        }

        /// <summary>
        /// Translates the native pointer that the current memory handle represents as a mutable .NET reference.
        /// </summary>
        /// <param name="handle">The source memory handle</param>
        /// <returns>The translated .NET reference to <see cref="IMemoryHandle.MemoryPointer"/>, whatever that is.</returns>
        public static ref System.Byte PointerAsReference(this IMemoryHandle handle) => ref *handle.MemoryPointer;

        /// <summary>
        /// Translates the native pointer that the current memory handle represents as a mutable .NET reference of the specified unmanaged structure.
        /// </summary>
        /// <typeparam name="T">The structure to translate the <see cref="IMemoryHandle.MemoryPointer"/> as.</typeparam>
        /// <param name="handle">The source memory handle</param>
        /// <returns>The translated .NET reference of type <typeparamref name="T"/> to <see cref="IMemoryHandle.MemoryPointer"/>, whatever that is.</returns>
        public static ref T PointerAsReferenceTo<T>(this IMemoryHandle handle) where T : unmanaged => ref *(T*)handle.MemoryPointer;
    }
}
