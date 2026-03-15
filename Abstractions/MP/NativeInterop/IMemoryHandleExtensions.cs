
using System;
using MP.Annotations.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace MP.NativeInterop
{
    /// <summary>
    /// Defines handy extension methods for the <see cref="IMemoryHandle"/> interface.
    /// </summary>
    public unsafe static class MemoryHandleExtensions
    {
        /// <summary>
        /// Returns the handle's native data to a new managed array.
        /// </summary>
        /// <param name="handle">The handle to retrieve the native data from.</param>
        /// <returns>A new managed array with the contents of <paramref name="handle"/>.</returns>
        public static byte[] ToManaged(this IMemoryHandle handle)
        {
            byte[] ret = new byte[handle.MemoryLength];
            fixed (byte* dst = ret)
            {
                UnsafeMethods.MemoryCopy(handle.MemoryPointer, dst, handle.MemoryLength);
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
            ulong tsize = sizeof(T).ToUInt64();
            ulong memlen = handle.MemoryLength;
            if (memlen % tsize != 0)
            {
                throw new ArgumentException("Memory handle length is not T-aligned.");
            }
            T[] ret = new T[memlen / tsize];
            fixed (T* dst = ret)
            {
                UnsafeMethods.MemoryCopy(handle.MemoryPointer, dst, memlen);
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
        [Throws(typeof(ArgumentNullException), typeof(ArgumentOutOfRangeException))]
        public static void FromManaged(this IMemoryHandle handle, byte[] bytes, int index, int count)
        {
            ArgumentNullException.ThrowIfNull(bytes);
            if (index < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(index), "Index must not be negative.");
            }
            else if (count < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(count), "Count must not be negative.");
            }
            else if ((ulong)(count - index) > handle.MemoryLength)
            {
                throw new ArgumentOutOfRangeException(nameof(bytes), "The native handle is not large enough so as to fit the selected region.");
            }
            fixed (byte* src = &bytes[index])
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
        [Throws(typeof(ArgumentNullException), typeof(ObjectDisposedException), typeof(InsufficientMemoryException))]
        public static void CopyTo(this IMemoryHandle srchandle, IMemoryHandle dsthandle)
        {
            ArgumentNullException.ThrowIfNull(dsthandle);
            ObjectDisposedException.ThrowIf(dsthandle.IsClosed, dsthandle);
            if (dsthandle.MemoryLength < srchandle.MemoryLength) { throw new InsufficientMemoryException("The destination handle is not large enough to accomondate the data of the current handle."); }
            Unsafe.CopyBlockUnaligned(dsthandle.MemoryPointer, srchandle.MemoryPointer, srchandle.MemoryLength.ToUInt32());
        }

        /// <summary>
        /// Translates the native pointer that the current memory handle represents as a mutable .NET reference.
        /// </summary>
        /// <param name="handle">The source memory handle</param>
        /// <returns>The translated .NET reference to <see cref="IMemoryHandle.MemoryPointer"/>, whatever that is.</returns>
        public static ref byte PointerAsReference(this IMemoryHandle handle) => ref *handle.MemoryPointer;

        /// <summary>
        /// Translates the native pointer that the current memory handle represents as a read-only .NET reference.
        /// </summary>
        /// <param name="handle">The source memory handle</param>
        /// <returns>The translated .NET reference to <see cref="IMemoryHandle.MemoryPointer"/>, whatever that is.</returns>
        public static ref readonly byte PointerAsReadOnlyReference(this IMemoryHandle handle) => ref *handle.MemoryPointer;

        /// <summary>
        /// Translates the native pointer that the current memory handle represents as a mutable .NET reference of the specified unmanaged structure.
        /// </summary>
        /// <typeparam name="T">The structure to translate the <see cref="IMemoryHandle.MemoryPointer"/> as.</typeparam>
        /// <param name="handle">The source memory handle</param>
        /// <returns>The translated .NET reference of type <typeparamref name="T"/> to <see cref="IMemoryHandle.MemoryPointer"/>, whatever that is.</returns>
        public static ref T PointerAsReferenceTo<T>(this IMemoryHandle handle) where T : unmanaged => ref *(T*)handle.MemoryPointer;

        /// <summary>
        /// Gets the current <see cref="IMemoryHandle"/> instance as a mutable span. <br />
        /// Convenient for cases that you need interoperability with Span-related API's. <br />
        /// Note that, changes performed by the returned span are visible to the bounded <see cref="IMemoryHandle"/> instance.
        /// </summary>
        /// <param name="handle">The <see cref="IMemoryHandle"/> instance to create a <see cref="Span{T}"/> from.</param>
        /// <returns>A <see cref="Span{T}"/> of type <see cref="System.Byte"/> mapping to the memory handle contents.</returns>
        /// <exception cref="OverflowException">The handle provided is too large to be handled as a <see cref="Span{T}"/> instance.</exception>
        [Throws(typeof(OverflowException))]
        public static Span<System.Byte> AsSpan(this IMemoryHandle handle)
        {
            int len;
            if (handle.MemoryLength > System.Int32.MaxValue)
            {
                throw new OverflowException("The memory handle is too large to be handled by a Span instance.");
            }
            else
            {
                len = handle.MemoryLength.ToInt32();
            }
            return new(handle.MemoryPointer, len);
        }

        /// <summary>
        /// Gets a portion of the current <see cref="IMemoryHandle"/> instance as a mutable span. <br />
        /// Convenient for cases that you need interoperability with Span-related API's. <br />
        /// Note that, changes performed by the span are visible to the bounded <see cref="IMemoryHandle"/> instance.
        /// </summary>
        /// <param name="handle">The <see cref="IMemoryHandle"/> instance to create a <see cref="Span{T}"/> from.</param>
        /// <param name="index">The starting index inside the current memory handle instance that the span returned can manage.</param>
        /// <param name="count">The number of bytes that the returned span can manage.</param>
        /// <returns>A <see cref="Span{T}"/> of type <see cref="System.Byte"/> mapping to the memory handle contents.</returns>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="count"/> is negative.</exception>
        /// <exception cref="OverflowException">The handle provided is too large to be handled as a <see cref="Span{T}"/> instance.</exception>
        [Throws(typeof(ArgumentOutOfRangeException), typeof(OverflowException))]
        public static Span<System.Byte> PortionAsSpan(this IMemoryHandle handle, ulong index, int count)
        {
            if (count < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(count), "Count cannot be a negative number.");
            }
            else if (index + (ulong)count > handle.MemoryLength)
            {
                throw new OverflowException("The memory handle is too large to be handled by a Span instance.");
            }
            else
            {
                return new(handle.MemoryPointer + index, count);
            }
        }

        /// <summary>
        /// Copies all the data defined in the current byte span to the specified memory handle.
        /// </summary>
        /// <param name="byte_span">The span to copy the data from.</param>
        /// <param name="mem_handle">The memory handle to store the data to.</param>
        /// <exception cref="ArgumentNullException"><paramref name="mem_handle"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentException"><paramref name="mem_handle"/> capacity is not enough to store all the bytes defined in the <paramref name="byte_span"/>.</exception>
        [Throws(typeof(ArgumentException), typeof(ArgumentNullException))]
        public static void CopyTo(this Span<System.Byte> byte_span, IMemoryHandle mem_handle)
        {
            ArgumentNullException.ThrowIfNull(mem_handle);
            if (mem_handle.MemoryLength < (ulong)byte_span.Length)
            {
                throw new ArgumentException("The specified memory handle capacity is not sufficient to copy into it the contents of the current byte span.", nameof(mem_handle));
            }
            Unsafe.CopyBlockUnaligned(ref PointerAsReference(mem_handle), ref byte_span[0], byte_span.Length.ToUInt32());
        }

        /// <summary>Copies all the data defined in the current byte span to the specified memory handle.</summary>
        /// <param name="byte_span">The span to copy the data from.</param>
        /// <param name="index">The index inside the <paramref name="byte_span"/> memory block at which copying begins.</param>
        /// <param name="count">The number of bytes to copy from <paramref name="byte_span"/> to <paramref name="mem_handle"/>.</param>
        /// <param name="mem_handle">The memory handle to store the data to.</param>
        /// <exception cref="ArgumentNullException"><paramref name="mem_handle"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="index"/> and/or <paramref name="count"/> are negative values.</exception>
        /// <exception cref="ArgumentException"><paramref name="mem_handle"/> capacity is not enough to store all the bytes defined in the <paramref name="byte_span"/>.</exception>
        [Throws(typeof(ArgumentException), typeof(ArgumentNullException), typeof(ArgumentOutOfRangeException))]
        public static void CopyTo(this Span<System.Byte> byte_span, IMemoryHandle mem_handle, int index, int count)
        {
            ArgumentNullException.ThrowIfNull(mem_handle);
            if (index < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(index), "Index cannot be a negative value.");
            }
            else if (count < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(count), "Count cannot be a negative value.");
            }
            else if ((ulong)(count - index) > mem_handle.MemoryLength)
            {
                throw new ArgumentException("The specified combination of the index and count parameters is illegal because the target memory block has a less capacity.");
            }
            Unsafe.CopyBlockUnaligned(ref PointerAsReference(mem_handle), ref byte_span[index], count.ToUInt32());
        }
    }
}