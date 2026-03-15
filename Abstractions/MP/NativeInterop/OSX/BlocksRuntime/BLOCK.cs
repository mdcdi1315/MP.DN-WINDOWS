
using MP.Annotations;
using System.Runtime.InteropServices;

namespace MP.NativeInterop.OSX.BlocksRuntime
{
    /// <summary>
    /// Defines the exact data structure as corressponds to a block.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public unsafe struct BLOCK
    {
        /// <summary>Pointer pointing to the location of the used memory stack where this object was allocated from.</summary>
        public void* ISA;

        /// <summary>Flags for the current block.</summary>
        public BLOCK_FLAGS Flags;

        /// <summary>This field is reserved and is not meant to be used by your code.</summary>
        [DeprecatedMayBeRemoved]
        public System.Int32 RSVD;

        /// <summary>
        /// Defines the function that will be invoked once the block is invoked. <br />
        /// The first argument of the function pointer must always be this block object.
        /// </summary>
        // Required signature: void INVOKE(void* ptr , ...)
        //                                              ^ -> any other arguments to call go here
        public void* InvokeFunc;

        /// <summary>
        /// Provides a pointer to a block descriptor. <br />
        /// Note that the flag <see cref="BLOCK_FLAGS.BLOCK_HAS_DESCRIPTOR"/> must be defined for this to be valid.
        /// </summary>
        public BLOCK_DESCRIPTOR* Descriptor; // Place the descriptor struct here.
                                             // Allocate it with 'malloc' and 'free' it after usage.
    }
}