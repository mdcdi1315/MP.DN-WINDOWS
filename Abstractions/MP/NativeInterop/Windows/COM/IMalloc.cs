
using MP.Annotations;

namespace MP.NativeInterop.Windows.COM
{
    /// <summary>
    /// Provides the communicating interface for the COM memory allocator. <br />
    /// Typically, an instance of this interface is provided through the CoGetMalloc function.
    /// </summary>
    [COMInterfaceGenerator("00000002-0000-0000-C000-000000000046")]
    public unsafe partial interface IMalloc
    {
        /// <summary>Allocates a memory block.</summary>
        /// <remarks>Applications should always check the return value from this method, 
        /// even when requesting small amounts of memory, 
        /// because there is no guarantee the memory will be allocated.</remarks>
        /// <param name="size">The size of the memory block to be allocated, in bytes.</param>
        /// <returns>If the method succeeds, the return value is a pointer to the allocated block of memory. Otherwise, it is <see langword="null"/>.</returns>
        public void* Alloc(ulong size);

        /// <summary>Changes the size of a previously allocated memory block.</summary>
        /// <param name="pv">A pointer to the block of memory to be reallocated. This parameter can be <see langword="null"/>, as discussed in the Remarks section below.</param>
        /// <param name="size">The size of the memory block to be reallocated, in bytes. This parameter can be 0, as discussed in the Remarks section below.</param>
        /// <returns>If the method succeeds, the return value is a pointer to the reallocated block of memory. Otherwise, it is <see langword="null"/>.</returns>
        public void* ReAlloc(void* pv, ulong size);

        /// <summary>Frees a previously allocated memory block.</summary>
        /// <param name="pv">A pointer to the memory block to be freed. If this parameter is <see langword="null"/>, this method has no effect.</param>
        public void Free(void* pv);

        /// <summary>
        /// Retrieves the size of a previously allocated memory block.
        /// </summary>
        /// <param name="pv">A pointer to the block of memory.</param>
        /// <returns>The size of the allocated memory block in bytes or, if <paramref name="pv"/> is a <see langword="null"/> pointer, unchecked((ulong)-1).</returns>
        public ulong GetSize(void* pv);

        /// <summary>
        /// Determines whether this allocator was used to allocate the specified memory block.
        /// </summary>
        /// <param name="pv">A pointer to the block of memory. If this parameter is a <see langword="null"/> pointer, -1 is returned.</param>
        /// <returns>
        /// This method can return the following values: <br />
        /// <list type="table">
        ///     <listheader>
        ///         <term>Return Value</term>
        ///         <description>Description</description>
        ///     </listheader>
        ///     <item>
        ///         <term>1</term>
        ///         <description>The block of memory was allocated by this allocator.</description>
        ///     </item>
        ///     <item>
        ///         <term>0</term>
        ///         <description>The block of memory was not allocated by this allocator.</description>
        ///     </item>
        ///     <item>
        ///         <term>-1</term>
        ///         <description>This method cannot determine whether this allocator allocated the block of memory.</description>
        ///     </item>
        /// </list>
        /// </returns>
        public int DidAlloc(void* pv);

        /// <summary>
        /// Minimizes the heap as much as possible by releasing unused memory to the operating system, coalescing adjacent free blocks, and committing free pages.
        /// </summary>
        public void HeapMinimize();
    }
}