
using System;
using System.Runtime.CompilerServices;
using MP.Annotations;

namespace MP.NativeInterop.OSX.BlocksRuntime
{
    /// <summary>
    /// Defines a set of flags for an individual block.
    /// </summary>
    [Flags]
    public enum BLOCK_FLAGS : System.Int32
    {
        // Unused field: BLOCK_REFCOUNT_MASK = 0xffff 
        // The above field is implemented from the extension methods provided in the BlockFlagsExtensions class.

        /// <summary>The block needs to be freed.</summary>
        BLOCK_NEEDS_FREE = 1 << 24,
        /// <summary>The block has implemented the copy and dispose methods.</summary>
        BLOCK_HAS_COPY_DISPOSE = 1 << 25,
        /// <summary>The block has a constructor.</summary>
        BLOCK_HAS_CTOR = 1 << 26, /* Helpers have C++ code. */
        /// <summary>The block is defined by a Objective-C garbage collector. Objective-C does not use GC anymore, so this is useless.</summary>
        // [DeprecatedMayBeRemoved]
        BLOCK_IS_GC = 1 << 27,
        /// <summary>The block is allocated like it is a global variable.</summary>
        BLOCK_IS_GLOBAL = 1 << 28,
        /// <summary>The block has defined a descriptor for it. For most OSX calls, this is REQUIRED to be defined.</summary>
        BLOCK_HAS_DESCRIPTOR = 1 << 29
    }

    /// <summary>
    /// Some extension methods to manage reference counting in <see cref="BLOCK_FLAGS"/> values.
    /// </summary>
    public static class BlockFlagsExtensions
    {
        private const BLOCK_FLAGS REFCOUNT_MASK = (BLOCK_FLAGS)0x0000FFFF;
        private const BLOCK_FLAGS OPTIONS_MASK = unchecked((BLOCK_FLAGS)0xFFFF0000);

        /// <summary>Gets the reference count for the current block flags enumeration value.</summary>
        /// <param name="flags">The block flags enumeration value to extract the current reference count value.</param>
        /// <returns>The current reference count of the object.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static System.UInt16 GetReferenceCount(this BLOCK_FLAGS flags) => unchecked((System.UInt16)(flags & REFCOUNT_MASK));
        
        /// <summary>
        /// Increments the reference count on the specified block flags enumeration value.
        /// </summary>
        /// <param name="flags">The block flags enumeration to increment the reference count value.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void IncrementReferenceCount(this ref BLOCK_FLAGS flags) => flags = (((flags & REFCOUNT_MASK) + 1) & REFCOUNT_MASK) | flags & unchecked(OPTIONS_MASK);

        /// <summary>
        /// Decrements the reference count on the specified block flags enumeration value.
        /// </summary>
        /// <param name="flags">The block flags enumeration to decrement the reference count value.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void DecrementReferenceCount(this ref BLOCK_FLAGS flags) => flags = (((flags & REFCOUNT_MASK) - 1) & REFCOUNT_MASK) | flags & unchecked(OPTIONS_MASK);
    }
}