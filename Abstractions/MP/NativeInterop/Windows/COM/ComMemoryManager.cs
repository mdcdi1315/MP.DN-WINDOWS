
using System;
using System.Diagnostics.CodeAnalysis;

namespace MP.NativeInterop.Windows.COM
{
    /// <summary>
    /// Defines a wrapper around COM's memory allocator, namely the <see cref="IMalloc"/> interface.
    /// </summary>
    public class COMMemoryManager : DataTrackingMemoryManager
    {
        private readonly IMalloc malloc;

        /// <summary>
        /// Gets the name of the memory manager used by the <see cref="ComInterop"/> class. <br />
        /// This is also the name under which the allocator must be registered to <see cref="AbstractPlatformLayer.RegisterMemoryManager(string, INativeMemoryManager)"/> as.
        /// </summary>
        public const System.String NAME = MPComWrappersSubsystem.COM_ALLOCATOR;

        /// <summary>
        /// Initializes a new instance of the <see cref="COMMemoryManager"/> class from the specified <see cref="IMalloc"/> interface instance.
        /// </summary>
        /// <param name="mlc">The implementation of the <see cref="IMalloc"/> interface to use.</param>
        public COMMemoryManager(IMalloc mlc)
        {
            ArgumentNullException.ThrowIfNull(mlc);
            malloc = mlc;
        }

        /// <inheritdoc />
        protected override unsafe void* AllocateImpl(ulong size) => malloc.Alloc(size);

        /// <inheritdoc />
        protected override unsafe bool FreeImpl([AllowNull] void* pointer)
        {
            if (pointer is null) {
                return false;
            } else {
                malloc.Free(pointer);
                return true;
            }
        }

        /// <inheritdoc />
        public override unsafe ulong GetSize([AllowNull] void* pointer) =>  pointer is null ? 0UL : malloc.GetSize(pointer);

        /// <inheritdoc />
        protected override unsafe void* ReAllocateImpl([AllowNull] void* old, ulong size)
        {
            void* p = malloc.ReAlloc(old , size);
            if (p is null) { throw new InsufficientMemoryException(); }
            return p;
        }
    }
}