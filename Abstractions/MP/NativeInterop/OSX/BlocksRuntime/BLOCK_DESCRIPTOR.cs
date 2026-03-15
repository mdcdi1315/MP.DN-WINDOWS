
using System.Runtime.InteropServices;
using MP.Annotations;

namespace MP.NativeInterop.OSX.BlocksRuntime
{
    /// <summary>
    /// Defines the descriptor for a block instance.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public unsafe struct BLOCK_DESCRIPTOR
    {
        /// <summary>This field is reserved and should not be modified by your code.</summary>
        [DeprecatedMayBeRemoved]
        public System.UInt64 RSVD;

        /// <summary>Defines the size of this structure.</summary>
        /// <remarks>
        /// This must be set to sizeof(BLOCK_DESCRIPTOR) == 32 bytes (24 for 32-bit apps).
        /// With this value it means that the descriptor is 32 bytes long (24 for 32-bit apps) -
        /// for internal block copy and release operations.
        /// </remarks>
        public System.UInt64 Size;

        /// <summary>
        /// Provides a function pointer to copy the block that this descriptor is referenced to.
        /// </summary>
        // Required signature: void BLKCOPY(void* dst , void* src)
        public delegate* unmanaged[Cdecl]<void*, void*, void> CopyFunc;

        /// <summary>
        /// Provides a function pointer to dispose the block that this descriptor is referenced to.
        /// </summary>
        // Required signature: void BLKDISPOSE(void* ptr)
        public delegate* unmanaged[Cdecl]<void*, void> DisposeFunc;

        /// <summary>
        /// Initializes a new instance of the <see cref="BLOCK_DESCRIPTOR"/> structure with default properties.
        /// </summary>
        public BLOCK_DESCRIPTOR()
        {
            RSVD = 0;
            CopyFunc = null;
            DisposeFunc = null;
            Size = sizeof(BLOCK_DESCRIPTOR).ToUInt64();
        }
    }
}