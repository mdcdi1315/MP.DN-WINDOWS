
using System;
using MP.Utilities;
using MP.Annotations.CodeAnalysis;
using System.Diagnostics.CodeAnalysis;

namespace MP.NativeInterop
{
    /// <summary>
    /// Provides the abstraction of the operating-system provided native memory manager.
    /// </summary>
    public unsafe interface INativeMemoryManager
    {
        /// <summary>
        /// Allocates a block of memory of the specified size in bytes. <br />
        /// No additional alignment is guaranteed.
        /// </summary>
        /// <param name="size">The number of bytes of the newly created memory block.</param>
        /// <returns>A raw pointer to the native memory block.</returns>
        /// <exception cref="InsufficientMemoryException">Not enough memory to allocate <paramref name="size"/> bytes.</exception>
        [return: NotNull]
        [Throws(typeof(InsufficientMemoryException))]
        public void* Allocate(ulong size);

        /// <summary>
        /// Reallocates a previously allocated block of memory and returns the new memory pointer. <br />
        /// The older pointer is freed by the method if required so, do not free it yourself!
        /// </summary>
        /// <param name="old">The pointer to the memory block to be reallocated.</param>
        /// <param name="size">The size of the new memory block.</param>
        /// <returns>A raw pointer to the newly created native memory block.</returns>
        /// <exception cref="InsufficientMemoryException">Not enough memory to allocate <paramref name="size"/> bytes.</exception>
        [return: NotNull]
        [Throws(typeof(InsufficientMemoryException))]
        public void* ReAllocate([AllowNull] void* old, ulong size);

        /// <summary>
        /// Frees a previously allocated memory block allocated with the <see cref="Allocate(ulong)"/> method.
        /// </summary>
        /// <param name="pointer">The pointer to the allocated memory block to be freed.</param>
        /// <returns>A value whether the operation succeeded. It returns <see langword="false"/> if the pointer is <see langword="null"/>.</returns>
        public System.Boolean Free([AllowNull] void* pointer);

        /// <summary>
        /// Gets the size of the pointer allocated with the <see cref="Allocate(ulong)"/> function. <br />
        /// This is solely provided for use by the native memory manager itself and as such, it can throw <see cref="NotSupportedException"/> while using it.
        /// </summary>
        /// <param name="pointer">The pointer to get it's size.</param>
        /// <returns>The absolute size of the memory block. Note: This might be more than the actually requested size.</returns>
        /// <exception cref="NotSupportedException">This method is not supported by the in-use implementation.</exception>
        [Throws(typeof(NotSupportedException))]
        public System.UInt64 GetSize([AllowNull] void* pointer);

        /// <summary>Creates an aligned memory block, and returns it.</summary>
        /// <remarks>
        /// Implementations overriding this method should also override the <see cref="FreeAligned(void*)"/> method.
        /// </remarks>
        /// <param name="size">The size, in bytes of the newly created memory block.</param>
        /// <param name="align">The alignment, in bytes, to use for this memory block.</param>
        /// <returns>The raw memory block.</returns>
        /// <exception cref="ArgumentException"><paramref name="align"/> is not a power of 2.</exception>
        /// <exception cref="InsufficientMemoryException">Not enough memory to create the requested memory block.</exception>
        [return: NotNull]
        [Throws(typeof(InsufficientMemoryException), typeof(ArgumentException))]
        public void* AllocateAligned(System.UInt64 size, System.UInt32 align) => MemoryUtils.AllocateAlignedWithOffset(this , size, align, 0UL);

        /// <summary>
        /// Reallocates a memory block, and returns it. <br />
        /// Be noted that the method has control of the lifetime of the <paramref name="old"/> pointer; 
        /// thus, do not free the passed pointer after returning from the call.
        /// </summary>
        /// <param name="old">The old memory block that is to be re-allocated. If <see langword="null"/>, the method wires to the <see cref="AllocateAligned(ulong, uint)"/> method.</param>
        /// <param name="size">The size of the re-allocated memory block. If zero, it returns <see langword="null"/> and frees the <paramref name="old"/> memory block.</param>
        /// <param name="align">The desired alignment of the reallocated memory block.</param>
        /// <returns>A pointer to the reallocated memory block.</returns>
        /// <exception cref="ArgumentException"><paramref name="align"/> is not a power of 2.</exception>
        /// <exception cref="InsufficientMemoryException">Not enough memory to create the requested memory block.</exception>
        [return: MaybeNull]
        [Throws(typeof(InsufficientMemoryException), typeof(ArgumentException))]
        public void* ReAllocateAligned([AllowNull] void* old, System.UInt64 size, System.UInt64 align) => MemoryUtils.ReallocateAlignedWithOffset(this, old, size, align, 0UL);

        /// <summary>
        /// Frees a previously allocated aligned memory block allocated with the <see cref="AllocateAligned(ulong, uint)"/> method.
        /// </summary>
        /// <param name="pointer">The pointer to the allocated memory block to be freed.</param>
        /// <returns>A value whether the operation succeeded. It returns <see langword="false"/> if the pointer is <see langword="null"/>.</returns>
        public System.Boolean FreeAligned([AllowNull] void* pointer) => MemoryUtils.FreeAligned(this , pointer);

        /// <summary>
        /// Returns platform memory statistics, if these are available. <br />
        /// The values supported by each platform may be different, and maybe these do not even exist, where in such case this method returns <see cref="Optional{T}.Empty"/>.
        /// </summary>
        /// <returns>An optional object implementing the <see cref="IAttributeable"/> interface.</returns>
        public Optional<IAttributeable> GetStatistics() => Optional<IAttributeable>.Empty();

        /// <summary>
        /// Creates a memory handle of the specified number of bytes, and returns it. <br />
        /// The memory handle is created by the <see cref="Allocate(ulong)"/> method.
        /// </summary>
        /// <param name="size">The number of bytes to request.</param>
        /// <returns>The allocated memory handle. Dispose it once done with it.</returns>
        /// <exception cref="InsufficientMemoryException">Not enough memory to create the requested memory block.</exception>
        [return: NotNull]
        [Throws(typeof(InsufficientMemoryException))]
        public IMemoryHandle AllocateMemoryHandle(ulong size) => MemoryUtils.CreateDefaultNonAlignedHandle(this, size);

        /// <summary>
        /// Creates a memory handle of the specified number of bytes, and returns it. <br />
        /// The memory handle is created by the <see cref="AllocateAligned(ulong, uint)"/> method.
        /// </summary>
        /// <param name="size">The number of bytes to request.</param>
        /// <param name="align">The alignment, in bytes, to use for this memory handle.</param>
        /// <returns>The allocated memory handle. Dispose it once done with it.</returns>
        /// <exception cref="InsufficientMemoryException">Not enough memory to create the requested memory block.</exception>
        [return: NotNull]
        [Throws(typeof(InsufficientMemoryException))]
        public IMemoryHandle AllocateAlignedMemoryHandle(ulong size, uint align) => MemoryUtils.CreateDefaultAlignedHandle(this, size, align);
    }
}