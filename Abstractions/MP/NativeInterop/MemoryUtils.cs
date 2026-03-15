
using System;
using System.Numerics;
using System.Threading;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Diagnostics.CodeAnalysis;

namespace MP.NativeInterop
{
    // Memory utilities. Some of these are based off the Windows UCRT SDK , version 10.0.19041.0
    internal static unsafe partial class MemoryUtils
    {
        private static readonly HashSet<UIntPtr> aligned_pointers;

        private sealed class NuintEqualityComparer : IEqualityComparer<UIntPtr>
        {
            public bool Equals(nuint x, nuint y) => x == y;

            public int GetHashCode([DisallowNull] nuint obj) => obj.GetHashCode();
        }

        static MemoryUtils() {
            aligned_pointers = new(0, new NuintEqualityComparer());
        }

        [StructLayout(LayoutKind.Sequential)] // 24 bytes
        public struct ALIGNEDMEMDETAILS
        {
            public UInt64 Size;
            public UInt64 Offset;
            public UInt64 Alignment;

            public UInt64 Actual; // Contains a number which is the pointer to the simply allocated data block

            public ALIGNEDMEMDETAILS(UInt64 size, UInt64 align, UInt64 ofs)
            {
                Size = size;
                Offset = ofs;
                Alignment = align;
            }

            /*
            public void PrintDetails(System.Boolean o)
            {
                System.Console.WriteLine(o ? "Out Data: " : "In Data: ");
                System.Console.WriteLine("Actual Pointer: {0}" , Actual);
                System.Console.WriteLine("Size: {0}", Size);
                System.Console.WriteLine("Alignment: {0}", Alignment);
                System.Console.WriteLine("Offset: {0}", Offset);
            }
            */
        }

        public static void AddAlignedPointer(UIntPtr ptr)
        {
            Monitor.Enter(aligned_pointers);
            try {
                aligned_pointers.Add(ptr);
            } finally {
                Monitor.Exit(aligned_pointers);
            }
        }

        public static void RemoveAlignedPointer(UIntPtr ptr)
        {
            Monitor.Enter(aligned_pointers);
            try {
                aligned_pointers.Remove(ptr);
            } finally {
                Monitor.Exit(aligned_pointers);
            }
        }

        public static bool IsAlignedPointer(UIntPtr ptr)
        {
            Monitor.Enter(aligned_pointers);
            try {
                return aligned_pointers.Contains(ptr);
            } finally {
                Monitor.Exit(aligned_pointers);
            }
        }

        public static bool IsAlignedPointer(void* ptr)
        {
            Monitor.Enter(aligned_pointers);
            try {
                return aligned_pointers.Contains(new(ptr));
            } finally {
                Monitor.Exit(aligned_pointers);
            }
        }

        /*
         // size: Total size of the mem block
        // align: Returned pointer alignment.
        // offset: Offset to return in the mem block. Typically is zero
        public static void* AllocateAlignedWithOffset(INativeMemoryManager manager, System.UInt64 size, System.UInt64 align, System.UInt64 offset)
        {
            System.UIntPtr ptr, retptr, gap;
            System.UInt64 nonuser_size, block_size, ptr_sz = (ulong)sizeof(void*);

            // validation section 
            if (!BitOperations.IsPow2(align))
            {
                // _VALIDATE_RETURN(IS_2_POW_N(align), EINVAL, nullptr);
                throw new ArgumentException("The specified alignment is not a power of 2", nameof(align));
            }
            if (offset != 0UL && offset >= size)
            {
                // _VALIDATE_RETURN(offset == 0 || offset < size, EINVAL, nullptr);
                throw new ArgumentException("The specified offset is larger than the size of the memory block requested", nameof(align));
            }

            align = (align > ptr_sz ? align : ptr_sz) - 1UL;

            // gap = number of bytes needed to round up offset to align with PTR_SZ
            gap = (nuint)((0UL - offset) & (ptr_sz - 1UL));

            nonuser_size = ptr_sz + gap + align;
            block_size = nonuser_size + size;
            // We do not perform this check for a number of reasons.
            // _VALIDATE_RETURN_NOEXC(size <= block_size, ENOMEM, nullptr)

            if ((ptr = (nuint)manager.Allocate(block_size)) == 0)
            {
                throw new InsufficientMemoryException("Failed to allocate a memory block.");
            }

            retptr = (nuint)(((ptr + nonuser_size + offset) & ~align) - offset);
            ((nuint*)(retptr - gap))[-1] = ptr;

            AddAlignedPointer(retptr);

            return (void*)retptr;
        }
         */

        // size: Total size of the mem block
        // align: Returned pointer alignment.
        // offset: Offset to return in the mem block. Typically is zero
        public static void* AllocateAlignedWithOffset(INativeMemoryManager manager, System.UInt64 size, System.UInt64 align, System.UInt64 offset)
        {
            System.UIntPtr ptr, retptr, gap;
            System.UInt64 nonuser_size, block_size, details_sz = (ulong)sizeof(ALIGNEDMEMDETAILS);

            /* validation section */
            if (!BitOperations.IsPow2(align))
            {
                // _VALIDATE_RETURN(IS_2_POW_N(align), EINVAL, nullptr);
                throw new ArgumentException("The specified alignment is not a power of 2", nameof(align));
            }
            if (offset != 0UL && offset >= size)
            {
                // _VALIDATE_RETURN(offset == 0 || offset < size, EINVAL, nullptr);
                throw new ArgumentException("The specified offset is larger than the size of the memory block requested", nameof(align));
            }

            ALIGNEDMEMDETAILS details = new(size, align, offset);

            align = (align > details_sz ? align : details_sz) - 1UL;

            /* gap = number of bytes needed to round up offset to align with DETAILS_SZ*/
            gap = (nuint)((0UL - offset) & (details_sz - 1UL));

            nonuser_size = details_sz + gap + align;
            block_size = nonuser_size + size;
            // We do not perform this check for a number of reasons.
            // _VALIDATE_RETURN_NOEXC(size <= block_size, ENOMEM, nullptr)

            if ((ptr = (nuint)manager.Allocate(block_size)) == 0)
            {
                throw new InsufficientMemoryException("Failed to allocate a memory block.");
            }

            details.Actual = ptr.ToUInt64();

            retptr = (nuint)(((ptr + nonuser_size + offset) & ~align) - offset);
            // Store memory block details
            ((ALIGNEDMEMDETAILS*)(retptr - gap))[-1] = details;

            AddAlignedPointer(retptr);

            return (void*)retptr;
        }

        public static void* ReallocateAlignedWithOffset(INativeMemoryManager manager, void* old, System.UInt64 size, System.UInt64 align, System.UInt64 offset)
        {
            /* Special cases */

            if (old is null) {
                return AllocateAlignedWithOffset(manager, size, align, offset);
            } else if (size == 0) {
                FreeAligned(manager, old);
                return null;
            }
            /* Validation section */
            else if (!BitOperations.IsPow2(align)) {
                // _VALIDATE_RETURN(IS_2_POW_N(align), EINVAL, nullptr);
                throw new ArgumentException("The specified alignment is not a power of 2", nameof(align));
            } else if (offset != 0UL && offset >= size) {
                // _VALIDATE_RETURN(offset == 0 || offset < size, EINVAL, nullptr);
                throw new ArgumentException("The specified offset is larger than the size of the memory block requested", nameof(align));
            }

            bool needs_new;
            void* p_free_later;
            ulong size_old;
            if (IsAlignedPointer(old)) {
                var details = GetAlignedPointerDetails(old);
                needs_new = size > (size_old = details.Size) || details.Alignment != align || details.Offset != offset;
                p_free_later = (void*)details.Actual;
            } else {
                // Normal pointer...
                size_old = manager.GetSize(old);
                needs_new = size > size_old || ((ulong)old % align) != 0;
                p_free_later = old;
            }

            if (needs_new) {
                // We need a new data block in any way. So malloc an aligned one.
                void* p = AllocateAlignedWithOffset(manager , size , align , offset);
                UnsafeMethods.MemoryCopy(old, p, size_old);
                manager.Free(p_free_later);
                return p;
            } else {
                // We can work around with the old one, so just return the old one as-is.
                return old;
            }
        }

        public static ALIGNEDMEMDETAILS GetAlignedPointerDetails(void* pointer)
        {
            System.UIntPtr ptr;
            System.UIntPtr details_sz = (nuint)sizeof(ALIGNEDMEMDETAILS);

            ptr = (nuint)pointer;

            /* ptr points to the ALIGNEDMEMDETAILS structure to starting of the memory block */
            ptr = (ptr & ~(details_sz - 1)) - details_sz;

            /* ptr is the ALIGNEDMEMDETAILS structure to the start of memory block*/
            return *(ALIGNEDMEMDETAILS*)ptr;
        }

        public static System.Boolean FreeAligned(INativeMemoryManager manager, void* pointer)
        {
            if (pointer is null) { 
                return false; 
            } else if (IsAlignedPointer(pointer)) {
                UIntPtr ptr = new(GetAlignedPointerDetails(pointer).Actual);

                RemoveAlignedPointer(ptr);

                return manager.Free((void*)ptr);
            } else {
                // Fallback path
                return manager.Free(pointer);
            }
        }
    }
}