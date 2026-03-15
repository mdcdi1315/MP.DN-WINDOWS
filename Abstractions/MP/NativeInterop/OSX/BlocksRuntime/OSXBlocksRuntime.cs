
using System;
using MP.Annotations;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;
using System.Diagnostics.CodeAnalysis;

namespace MP.NativeInterop.OSX.BlocksRuntime
{
    /// <summary>
    /// Provides the interop goodies for interoping OSX block instances.
    /// </summary>
    public unsafe static class OSXBlocksRuntime
    {
        private static void* isa;
        private static BLOCK_DESCRIPTOR* simple_descriptor;

        /// <summary>
        /// Initializes the OSX Blocks Runtime.
        /// </summary>
        [RequiresNativeLayer]
        public static void Initalize()
        {
            if (isa is not null) { throw new InvalidOperationException("The OSX Blocks runtime has already been initialized."); }
            isa = SystemInfo.GetDefaultMemoryManager().Allocate(32UL);
            simple_descriptor = (BLOCK_DESCRIPTOR*)SystemInfo.GetDefaultMemoryManager().Allocate(sizeof(BLOCK_DESCRIPTOR));
            *simple_descriptor = new() {
                CopyFunc = &BlockCopyDefault,
                DisposeFunc = &BlockDisposeDefault
            };
        }

        /// <summary>
        /// Creates a block for the given function pointer. <br />
        /// The function pointer's first parameter must be a pointer to a <see cref="BLOCK"/> structure, that represents the current block.
        /// </summary>
        /// <param name="p_function_pointer">The function pointer to be invoked.</param>
        /// <returns>The created <see cref="BLOCK"/> instance.</returns>
        [RequiresNativeLayer]
        public static BLOCK* CreateBlock(void* p_function_pointer)
        {
            ArgumentNullException.ThrowIfNull(p_function_pointer);
            BLOCK* p_new = (BLOCK*)SystemInfo.GetDefaultMemoryManager().Allocate(sizeof(BLOCK));
            p_new->ISA = isa;
            p_new->Flags = BLOCK_FLAGS.BLOCK_HAS_COPY_DISPOSE | BLOCK_FLAGS.BLOCK_HAS_DESCRIPTOR;
            p_new->InvokeFunc = p_function_pointer;
            p_new->RSVD = 0;
            p_new->Descriptor = simple_descriptor;
            return p_new;
        }

        /// <summary>
        /// Frees a block previously allocated with the <see cref="CreateBlock"/> or <see cref="CreateBlockWithDotNetObject"/> methods.
        /// </summary>
        /// <param name="block">The block to be freed.</param>
        public static void FreeBlock([AllowNull] BLOCK* block)
        {
            if (block is null) { return; } else {
                block->Descriptor->DisposeFunc(block);
            }
        }

        public static BLOCK* CreateBlockWithDotNetObject(void* p_function_pointer, Object o)
        {
            ArgumentNullException.ThrowIfNull(o);
            ArgumentNullException.ThrowIfNull(p_function_pointer);
            BLOCK* p_new = (BLOCK*)SystemInfo.GetDefaultMemoryManager().Allocate(sizeof(BLOCK));
            p_new->ISA = isa;
            p_new->Flags = BLOCK_FLAGS.BLOCK_HAS_COPY_DISPOSE | BLOCK_FLAGS.BLOCK_HAS_DESCRIPTOR;
            p_new->InvokeFunc = p_function_pointer;
            p_new->RSVD = 0;
            EXTENDED_BLOCK_DESCRIPTOR* pdesc = (EXTENDED_BLOCK_DESCRIPTOR*)SystemInfo.GetDefaultMemoryManager().Allocate(sizeof(EXTENDED_BLOCK_DESCRIPTOR));
            EXTENDED_BLOCK_DESCRIPTOR d = new(o);
            d.Base.CopyFunc = &BlockCopyWithCustomObject;
            d.Base.DisposeFunc = &BlockDisposeWithCustomObject;
            *pdesc = d;
            p_new->Descriptor = (BLOCK_DESCRIPTOR*)pdesc;
            return p_new;
        }

        [UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvCdecl) })]
        private static void BlockCopyDefault(void* dst, void* src)
        {
            if (dst is null || src is null) {
                throw new AccessViolationException("Invalid attempt to access a non-existent pointer!");
            }
            BLOCK* source = (BLOCK*)src;
            BLOCK* destination = (BLOCK*)dst;

            destination->ISA = isa;
            destination->Flags = source->Flags | BLOCK_FLAGS.BLOCK_HAS_DESCRIPTOR | BLOCK_FLAGS.BLOCK_HAS_COPY_DISPOSE;
            destination->Descriptor = simple_descriptor;
            destination->InvokeFunc = source->InvokeFunc;
        }

        [RequiresNativeLayer]
        [UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvCdecl) })]
        private static void BlockDisposeDefault(void* p_block)
        {
            if (p_block is null) {
                throw new AccessViolationException("Invalid attempt to access a non-existent pointer!");
            }
            BLOCK* p_data = (BLOCK*)p_block;
            if (p_data->Flags.HasFlag(BLOCK_FLAGS.BLOCK_HAS_DESCRIPTOR)) {
                p_data->Descriptor = null;
            }
            p_data->ISA = null;
            p_data->InvokeFunc = null;
            SystemInfo.GetDefaultMemoryManager().Free(p_data);
        }

        [UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvCdecl) })]
        private static void BlockCopyWithCustomObject(void* dst, void* src)
        {
            if (dst is null || src is null) {
                throw new AccessViolationException("Invalid attempt to access a non-existent pointer!");
            }
            BLOCK* source = (BLOCK*)src;
            BLOCK* destination = (BLOCK*)dst;

            destination->ISA = isa;
            source->Flags.IncrementReferenceCount();
            destination->Flags = source->Flags | BLOCK_FLAGS.BLOCK_HAS_DESCRIPTOR | BLOCK_FLAGS.BLOCK_HAS_COPY_DISPOSE;
            destination->Descriptor = source->Descriptor;
            destination->InvokeFunc = source->InvokeFunc;
        }

        [RequiresNativeLayer]
        [UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvCdecl) })]
        private static void BlockDisposeWithCustomObject(void* p_block)
        {
            if (p_block is null) {
                throw new AccessViolationException("Invalid attempt to access a non-existent pointer!");
            }
            BLOCK* p_data = (BLOCK*)p_block;
            p_data->Flags.DecrementReferenceCount();
            if (p_data->Flags.GetReferenceCount() == 0 && p_data->Flags.HasFlag(BLOCK_FLAGS.BLOCK_HAS_DESCRIPTOR))
            {
                ((EXTENDED_BLOCK_DESCRIPTOR*)p_data->Descriptor)->DeleteGCHandle();
                SystemInfo.GetDefaultMemoryManager().Free(p_data->Descriptor);
            }
            p_data->ISA = null;
            p_data->Descriptor = null;
            p_data->InvokeFunc = null;
            SystemInfo.GetDefaultMemoryManager().Free(p_data);
        }

    }
}