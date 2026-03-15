

using System;
using System.Runtime.InteropServices;

namespace MP.NativeInterop.OSX.BlocksRuntime
{
    /// <summary>
    /// Provides an extended version of the <see cref="BLOCK_DESCRIPTOR"/> structure. <br />
    /// This does not exist in the actual OSX definitions; it is defined so that you can access a .NET object from the block.
    /// </summary>
    public struct EXTENDED_BLOCK_DESCRIPTOR
    {
        /// <summary>
        /// Gets the base descriptor data that are elsewise needed.
        /// </summary>
        public BLOCK_DESCRIPTOR Base;

        private readonly IntPtr GCObjectReference;

        /// <summary>
        /// Initializes a new instance of the <see cref="EXTENDED_BLOCK_DESCRIPTOR"/> from the spefied .NET object.
        /// </summary>
        /// <param name="o">The object to be retained and be accessible from the block implementation.</param>
        public unsafe EXTENDED_BLOCK_DESCRIPTOR(Object o) {
            ArgumentNullException.ThrowIfNull(o);
            Base = new();
            Base.Size = sizeof(EXTENDED_BLOCK_DESCRIPTOR).ToUInt64();
            GCObjectReference = GCHandle.ToIntPtr(GCHandle.Alloc(o , GCHandleType.Normal));
        }

        /// <summary>
        /// Gets the .NET object associated with this block descriptor.
        /// </summary>
        /// <returns>The attached .NET object.</returns>
        public readonly Object GetAttachedObject() => GCHandle.FromIntPtr(GCObjectReference).Target;

        /// <summary>
        /// Deletes the GC Handle reference once this extended block descriptor is deleted.
        /// </summary>
        public readonly void DeleteGCHandle() => GCHandle.FromIntPtr(GCObjectReference).Free();
    }
}