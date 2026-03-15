
using System;
using MP.Annotations.CodeAnalysis;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace MP.NativeInterop
{
    /// <summary>
    /// Provides handy extension methods around the <see cref="INativeMemoryManager"/> interface.
    /// </summary>
    public static unsafe partial class INativeMemoryManagerExtensions
    {
        /// <summary>
        /// Allocates a block of memory of the specified size in bytes. <br />
        /// The contents of the returned pointer are guaranteed to be zeroes. <br />
        /// No additional alignment is guaranteed.
        /// </summary>
        /// <param name="manager">The native memory manager to allocate memory from.</param>
        /// <param name="size">The number of bytes of the newly created memory block.</param>
        /// <returns>A raw pointer to the native memory block.</returns>
        /// <exception cref="InsufficientMemoryException">Not enough memory to allocate <paramref name="size"/> bytes.</exception>
        [return: NotNull]
        [Throws(typeof(InsufficientMemoryException))]
        public static void* AllocateZeroed(this INativeMemoryManager manager, uint size)
        {
            void* p_data = manager.Allocate(size.ToUInt64());
            Unsafe.InitBlockUnaligned(p_data , 0, size);
            return p_data;
        }

        /// <summary>
        /// Allocates a block of memory of the specified size in bytes. <br />
        /// The contents of the returned pointer are guaranteed to be zeroes. <br />
        /// No additional alignment is guaranteed.
        /// </summary>
        /// <param name="manager">The native memory manager to allocate memory from.</param>
        /// <param name="size">The number of bytes of the newly created memory block.</param>
        /// <returns>A raw pointer to the native memory block.</returns>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="size"/> is negative.</exception>
        /// <exception cref="InsufficientMemoryException">Not enough memory to allocate <paramref name="size"/> bytes.</exception>
        [return: NotNull]
        [Throws(typeof(InsufficientMemoryException), typeof(ArgumentOutOfRangeException))]
        public static void* AllocateZeroed(this INativeMemoryManager manager, int size)
        {
            if (size < 0) {
                throw new ArgumentOutOfRangeException(nameof(size), "Size cannot be a negative value.");
            } else {
                void* p_data = manager.Allocate(size.ToUInt64());
                Unsafe.InitBlockUnaligned(p_data, 0, size.ToUInt32());
                return p_data;
            }
        }

        /// <summary>
        /// Allocates a block of memory of the specified size in bytes. <br />
        /// No additional alignment is guaranteed.
        /// </summary>
        /// <param name="manager">The native memory manager to allocate memory from.</param>
        /// <param name="size">The number of bytes of the newly created memory block.</param>
        /// <returns>A raw pointer to the native memory block.</returns>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="size"/> is negative.</exception>
        /// <exception cref="InsufficientMemoryException">Not enough memory to allocate <paramref name="size"/> bytes.</exception>
        [return: NotNull]
        [Throws(typeof(InsufficientMemoryException), typeof(ArgumentOutOfRangeException))]
        public static void* Allocate(this INativeMemoryManager manager, int size)
        {
            if (size < 0) {
                throw new ArgumentOutOfRangeException(nameof(size), "Size cannot be a negative value.");
            } else {
                return manager.Allocate(size.ToUInt64());
            }
        }

        /// <summary>
        /// Allocates a block of memory of the specified size in bytes. <br />
        /// No additional alignment is guaranteed.
        /// </summary>
        /// <param name="manager">The native memory manager to allocate memory from.</param>
        /// <param name="size">The number of bytes of the newly created memory block.</param>
        /// <returns>A raw pointer to the native memory block.</returns>
        /// <exception cref="InsufficientMemoryException">Not enough memory to allocate <paramref name="size"/> bytes.</exception>
        [return: NotNull]
        [Throws(typeof(InsufficientMemoryException))]
        public static void* Allocate(this INativeMemoryManager manager, uint size) => manager.Allocate(size.ToUInt64());

        /// <summary>
        /// Reallocates a previously allocated block of memory and returns the new memory pointer. <br />
        /// The older pointer is freed by the method if required so, do not free it yourself!
        /// </summary>
        /// <param name="manager">The native memory manager to reallocate memory from.</param>
        /// <param name="old">The pointer to the memory block to be reallocated.</param>
        /// <param name="size">The size of the new memory block.</param>
        /// <returns>A raw pointer to the newly created native memory block.</returns>
        /// <exception cref="InsufficientMemoryException">Not enough memory to allocate <paramref name="size"/> bytes.</exception>
        [return: NotNull]
        [Throws(typeof(InsufficientMemoryException))]
        public static void* ReAllocate(this INativeMemoryManager manager, void* old, uint size) => manager.ReAllocate(old, size.ToUInt64());

        /// <summary>
        /// Reallocates a previously allocated block of memory and returns the new memory pointer. <br />
        /// The older pointer is freed by the method if required so, do not free it yourself!
        /// </summary>
        /// <param name="manager">The native memory manager to reallocate memory from.</param>
        /// <param name="old">The pointer to the memory block to be reallocated.</param>
        /// <param name="size">The size of the new memory block.</param>
        /// <returns>A raw pointer to the newly created native memory block.</returns>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="size"/> is negative.</exception>
        /// <exception cref="InsufficientMemoryException">Not enough memory to allocate <paramref name="size"/> bytes.</exception>
        [return: NotNull]
        [Throws(typeof(InsufficientMemoryException), typeof(ArgumentOutOfRangeException))]
        public static void* ReAllocate(this INativeMemoryManager manager, void* old, int size)
        {
            if (size < 0) {
                throw new ArgumentOutOfRangeException(nameof(size), "Size cannot be a negative value.");
            } else {
                return manager.ReAllocate(old, size.ToUInt64());
            }
        }

        /// <summary>
        /// Creates an aligned memory block, and returns it.
        /// </summary>
        /// <param name="manager">The native memory manager to allocate aligned memory from.</param>
        /// <param name="size">The size, in bytes of the newly created memory block.</param>
        /// <param name="align">The alignment, in bytes, to use for this memory block.</param>
        /// <returns>The raw memory block.</returns>
        /// <exception cref="ArgumentException"><paramref name="align"/> is not a power of 2.</exception>
        /// <exception cref="InsufficientMemoryException">Not enough memory to create the requested memory block.</exception>
        [return: NotNull]
        [Throws(typeof(InsufficientMemoryException), typeof(ArgumentException))]
        public static void* AllocateAligned(this INativeMemoryManager manager, System.UInt32 size, System.UInt32 align) => manager.AllocateAligned(size.ToUInt64(), align);

        /// <summary>
        /// Creates an aligned memory block, and returns it.
        /// </summary>
        /// <param name="manager">The native memory manager to allocate aligned memory from.</param>
        /// <param name="size">The size, in bytes of the newly created memory block.</param>
        /// <param name="align">The alignment, in bytes, to use for this memory block.</param>
        /// <returns>The raw memory block.</returns>
        /// <exception cref="ArgumentException"><paramref name="align"/> is not a power of 2.</exception>
        /// <exception cref="InsufficientMemoryException">Not enough memory to create the requested memory block.</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="size"/> and/or <paramref name="align"/> parameters are negative values.</exception>
        [return: NotNull]
        [Throws(typeof(InsufficientMemoryException), typeof(ArgumentException), typeof(ArgumentOutOfRangeException))]
        public static void* AllocateAligned(this INativeMemoryManager manager, System.Int32 size, System.Int32 align)
        {
            if (size < 0) {
                throw new ArgumentOutOfRangeException(nameof(size), "Size cannot be a negative value.");
            } else if (align < 0) {
                throw new ArgumentOutOfRangeException(nameof(size), "Alignment cannot be a negative value.");
            } else {
                return manager.AllocateAligned(size.ToUInt64(), align.ToUInt32());
            }
        }

        /// <summary>
        /// Reallocates a memory block, and returns it. <br />
        /// Be noted that the method has control of the lifetime of the <paramref name="old"/> pointer; 
        /// thus, do not free the passed pointer after returning from the call.
        /// </summary>
        ///  <param name="manager">The native memory manager to reallocate aligned memory from.</param>
        /// <param name="old">The old memory block that is to be re-allocated. If <see langword="null"/>, the method wires to the <see cref="INativeMemoryManager.AllocateAligned(ulong, uint)"/> method.</param>
        /// <param name="size">The size of the re-allocated memory block. If zero, it returns <see langword="null"/> and frees the <paramref name="old"/> memory block.</param>
        /// <param name="align">The desired alignment of the reallocated memory block.</param>
        /// <returns>A pointer to the reallocated memory block.</returns>
        /// <exception cref="ArgumentException"><paramref name="align"/> is not a power of 2.</exception>
        /// <exception cref="InsufficientMemoryException">Not enough memory to create the requested memory block.</exception>
        [return: MaybeNull]
        [Throws(typeof(InsufficientMemoryException), typeof(ArgumentException))]
        public static void* ReAllocateAligned(this INativeMemoryManager manager, [AllowNull] void* old, System.UInt64 size, System.UInt32 align) => manager.ReAllocateAligned(old, size, align.ToUInt64());

        /// <summary>
        /// Reallocates a memory block, and returns it. <br />
        /// Be noted that the method has control of the lifetime of the <paramref name="old"/> pointer; 
        /// thus, do not free the passed pointer after returning from the call.
        /// </summary>
        ///  <param name="manager">The native memory manager to reallocate aligned memory from.</param>
        /// <param name="old">The old memory block that is to be re-allocated. If <see langword="null"/>, the method wires to the <see cref="INativeMemoryManager.AllocateAligned(ulong, uint)"/> method.</param>
        /// <param name="size">The size of the re-allocated memory block. If zero, it returns <see langword="null"/> and frees the <paramref name="old"/> memory block.</param>
        /// <param name="align">The desired alignment of the reallocated memory block.</param>
        /// <returns>A pointer to the reallocated memory block.</returns>
        /// <exception cref="ArgumentException"><paramref name="align"/> is not a power of 2.</exception>
        /// <exception cref="InsufficientMemoryException">Not enough memory to create the requested memory block.</exception>
        [return: MaybeNull]
        [Throws(typeof(InsufficientMemoryException), typeof(ArgumentException))]
        public static void* ReAllocateAligned(this INativeMemoryManager manager, [AllowNull] void* old, System.UInt32 size, System.UInt32 align) => manager.ReAllocateAligned(old, size.ToUInt64(), align.ToUInt64());

        /// <summary>
        /// Reallocates a memory block, and returns it. <br />
        /// Be noted that the method has control of the lifetime of the <paramref name="old"/> pointer; 
        /// thus, do not free the passed pointer after returning from the call.
        /// </summary>
        ///  <param name="manager">The native memory manager to reallocate aligned memory from.</param>
        /// <param name="old">The old memory block that is to be re-allocated. If <see langword="null"/>, the method wires to the <see cref="INativeMemoryManager.AllocateAligned(ulong, uint)"/> method.</param>
        /// <param name="size">The size of the re-allocated memory block. If zero, it returns <see langword="null"/> and frees the <paramref name="old"/> memory block.</param>
        /// <param name="align">The desired alignment of the reallocated memory block.</param>
        /// <returns>A pointer to the reallocated memory block.</returns>
        /// <exception cref="ArgumentException"><paramref name="align"/> is not a power of 2.</exception>
        /// <exception cref="InsufficientMemoryException">Not enough memory to create the requested memory block.</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="size"/> and/or <paramref name="align"/> parameters are negative values.</exception>
        [return: MaybeNull]
        [Throws(typeof(InsufficientMemoryException), typeof(ArgumentException), typeof(ArgumentOutOfRangeException))]
        public static void* ReAllocateAligned(this INativeMemoryManager manager, [AllowNull] void* old, System.Int32 size, System.Int32 align)
        {
            if (size < 0) {
                throw new ArgumentOutOfRangeException(nameof(size), "Size cannot be a negative value.");
            } else if (align < 0) {
                throw new ArgumentOutOfRangeException(nameof(size), "Alignment cannot be a negative value.");
            } else {
                return manager.ReAllocateAligned(old, size.ToUInt64(), align.ToUInt32());
            }
        }

        /// <summary>
        /// Creates a memory handle of the specified number of bytes, and returns it. <br />
        /// The memory handle is created by the <see cref="INativeMemoryManager.Allocate(ulong)"/> method.
        /// </summary>
        /// <param name="manager">The native memory manager to allocate memory from.</param>
        /// <param name="size">The number of bytes to request.</param>
        /// <returns>The allocated memory handle. Dispose it once done with it.</returns>
        /// <exception cref="InsufficientMemoryException">Not enough memory to create the requested memory block.</exception>
        [return: NotNull]
        [Throws(typeof(InsufficientMemoryException))]
        public static IMemoryHandle AllocateMemoryHandle(this INativeMemoryManager manager, uint size) => manager.AllocateMemoryHandle(size.ToUInt64());

        /// <summary>
        /// Creates a memory handle of the specified number of bytes, and returns it. <br />
        /// The memory handle is created by the <see cref="INativeMemoryManager.Allocate(ulong)"/> method.
        /// </summary>
        /// <param name="manager">The native memory manager to allocate memory from.</param>
        /// <param name="size">The number of bytes to request.</param>
        /// <returns>The allocated memory handle. Dispose it once done with it.</returns>
        /// <exception cref="InsufficientMemoryException">Not enough memory to create the requested memory block.</exception>
        [return: NotNull]
        [Throws(typeof(InsufficientMemoryException), typeof(ArgumentOutOfRangeException))]
        public static IMemoryHandle AllocateMemoryHandle(this INativeMemoryManager manager, int size)
        {
            if (size < 0) {
                throw new ArgumentOutOfRangeException(nameof(size), "Size cannot be a negative value.");
            } else {
                return manager.AllocateMemoryHandle(size.ToUInt64());
            }
        }

        /// <summary>
        /// Creates a memory handle of the specified number of bytes, and returns it. <br />
        /// The memory handle is created by the <see cref="INativeMemoryManager.AllocateAligned(ulong, uint)"/> method.
        /// </summary>
        /// <param name="manager">The native memory manager to reallocate memory from.</param>
        /// <param name="size">The number of bytes to request.</param>
        /// <param name="align">The alignment, in bytes, to use for this memory handle.</param>
        /// <returns>The allocated memory handle. Dispose it once done with it.</returns>
        /// <exception cref="InsufficientMemoryException">Not enough memory to create the requested memory block.</exception>
        [return: NotNull]
        [Throws(typeof(InsufficientMemoryException))]
        public static IMemoryHandle AllocateAlignedMemoryHandle(this INativeMemoryManager manager, uint size, uint align) => manager.AllocateAlignedMemoryHandle(size.ToUInt64(), align);

        /// <summary>
        /// Creates a memory handle of the specified number of bytes, and returns it. <br />
        /// The memory handle is created by the <see cref="INativeMemoryManager.AllocateAligned(ulong, uint)"/> method.
        /// </summary>
        /// <param name="manager">The native memory manager to reallocate memory from.</param>
        /// <param name="size">The number of bytes to request.</param>
        /// <param name="align">The alignment, in bytes, to use for this memory handle.</param>
        /// <returns>The allocated memory handle. Dispose it once done with it.</returns>
        /// <exception cref="InsufficientMemoryException">Not enough memory to create the requested memory block.</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="size"/> and/or <paramref name="align"/> parameters are negative values.</exception>
        [return: NotNull]
        [Throws(typeof(InsufficientMemoryException), typeof(ArgumentOutOfRangeException))]
        public static IMemoryHandle AllocateAlignedMemoryHandle(this INativeMemoryManager manager, int size, int align)
        {
            if (size < 0) {
                throw new ArgumentOutOfRangeException(nameof(size), "Size cannot be a negative value.");
            } else if (align < 0) {
                throw new ArgumentOutOfRangeException(nameof(size), "Alignment cannot be a negative value.");
            } else {
                return manager.AllocateAlignedMemoryHandle(size.ToUInt64(), align.ToUInt32());
            }
        }
    }
}
