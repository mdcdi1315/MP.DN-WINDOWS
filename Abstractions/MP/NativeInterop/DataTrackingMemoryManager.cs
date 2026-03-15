
using MP.Utilities;
using System.Threading;
using System.Diagnostics.CodeAnalysis;

namespace MP.NativeInterop
{
    /// <summary>
    /// Provides basic information about the allocated memory on a derived memory manager.
    /// </summary>
    public unsafe abstract class DataTrackingMemoryManager : INativeMemoryManager
    {
        /// <summary>
        /// Gets the number of the currently allocated memory blocks.
        /// </summary>
        public const System.String ALLOCATED_MEM_BLOCKS = "1";

        /// <summary>
        /// Gets the number of the allocated bytes during the entire life of the memory manager.
        /// </summary>
        public const System.String ALLOCATED_BYTES_TOTAL = "2";
        
        /// <summary>
        /// Gets the number of the reallocated memory blocks throughout the entire life of the memory manager.
        /// </summary>
        public const System.String REALLOCATED_MEM_BLOCKS = "3";
        
        private sealed class Data : IAttributeable
        {
            private System.UInt64 allocated_mem_blocks, allocated_bytes_total, realloc_mem_blocks;

            public Data()
            {
                allocated_bytes_total = 0;
                allocated_mem_blocks = 0;
                realloc_mem_blocks = 0;
            }

            public void SetAttribute(string name, [AllowNull] object value) { }

            public bool TryGetAttribute(string attribute, [MaybeNullWhen(true)] out object value)
            {
                value = attribute switch {
                    ALLOCATED_MEM_BLOCKS => allocated_mem_blocks,
                    ALLOCATED_BYTES_TOTAL => allocated_bytes_total,
                    REALLOCATED_MEM_BLOCKS => realloc_mem_blocks,
                    _ => null,
                };
                return value is not null;
            }

            public void IncrementMemBlocks() => Interlocked.Increment(ref allocated_mem_blocks);

            public void DecrementMemBlocks() => Interlocked.Decrement(ref allocated_mem_blocks);

            public void AddAllocatedBytes(ulong size) => Interlocked.Add(ref allocated_bytes_total , size);

            public void IncrementReallocBlocks() => Interlocked.Increment(ref realloc_mem_blocks);
        }

        private readonly Data stats;

        /// <summary>
        /// Initializes a new instance of the <see cref="DataTrackingMemoryManager"/> class.
        /// </summary>
        protected DataTrackingMemoryManager() => stats = new();

        /// <inheritdoc />
        [return: NotNull]
        public void* Allocate(ulong size)
        {
            void* p = AllocateImpl(size);
            if (p is not null) {
                stats.IncrementMemBlocks(); 
                stats.AddAllocatedBytes(size);
            }
            return p;
        }

        /// <summary>
        /// Provides the way to allocate a memory block. 
        /// See <see cref="INativeMemoryManager.Allocate(ulong)"/> for more information.
        /// </summary>
        protected abstract void* AllocateImpl(ulong size);

        /// <inheritdoc />
        public bool Free([AllowNull] void* pointer)
        {
            bool v = FreeImpl(pointer);
            if (v) { stats.DecrementMemBlocks(); }
            return v;
        }

        /// <summary>
        /// Provides the way to free a previously allocated memory block. 
        /// See <see cref="INativeMemoryManager.Free(void*)"/> for more information.
        /// </summary>
        protected abstract bool FreeImpl([AllowNull] void* pointer);

        /// <inheritdoc />
        public abstract ulong GetSize([AllowNull] void* pointer);

        /// <inheritdoc />
        [return: NotNull]
        public void* ReAllocate([AllowNull] void* old, ulong size)
        {
            void* p = ReAllocateImpl(old, size);
            if (p is not null) {
                stats.IncrementReallocBlocks();
            }
            return p;
        }

        /// <summary>
        /// Provides the way to re-allocate a memory block. 
        /// See <see cref="INativeMemoryManager.ReAllocate(void*, ulong)"/> for more information.
        /// </summary>
        protected abstract void* ReAllocateImpl([AllowNull] void* old, ulong size);

        /// <inheritdoc />
        public Optional<IAttributeable> GetStatistics() => Optional<IAttributeable>.Of(stats);
    }
}