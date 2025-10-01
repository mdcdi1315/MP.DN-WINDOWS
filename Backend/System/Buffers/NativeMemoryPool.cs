
using MP;
using Microsoft.Win32;
using System.Collections.Generic;
using Microsoft.Win32.SafeHandles;

namespace System.Buffers
{
    public unsafe sealed class NativeMemoryOwner : MemoryManager<System.Byte>
    {
        private System.Byte* pointer;
        private System.Int32 length;
        private NativeMemoryPool reference;

        internal NativeMemoryOwner(NativeMemoryPool reference , SafeLibcMemoryHandle memhandle) {
            pointer = memhandle.MemoryPointer;
            length = memhandle.MemoryLength;
            this.reference = reference;
        }

        public override Span<byte> GetSpan() => new(pointer, length);

        // No pinning happens here, just the pointer is itself returned.
        public override MemoryHandle Pin(int elementIndex = 0)
        {
            if (elementIndex < 0 || elementIndex > length) {
                throw new ArgumentOutOfRangeException(nameof(elementIndex) , "Index was out of the native memory bounds.");
            }
            return new(pointer + elementIndex);
        }

        // Pinning in fact does not happen because we are working with native pointers.
        // However, this capability must be provided.
        public override void Unpin() {}

        protected override void Dispose(bool disposing) 
        {
            reference?.PoolBuffer(pointer);
            reference = null;
            pointer = null;
            length = 0;
        }
    }

    /// <summary>
    /// Represents a native memory pool which can be used for faster
    /// and safe unmanaged memory manipulation.
    /// </summary>
    public sealed class NativeMemoryPool : MemoryPool<System.Byte>
    {
        private const System.Int32 DefaultBufferSize = 2048, Default_MaxBufferSize = System.Int32.MaxValue;

        private struct NativeMemoryPoolBlock
        {
            public System.Boolean Occupied;
            public System.Int64 PointerAddress;
            public SafeLibcMemoryHandle Handle;

            public unsafe NativeMemoryPoolBlock(NativeMemoryPool pool , System.Int32 size)
            {
                Occupied = false;
                Handle = pool.nativeheap.Allocate(size.ToUInt32());
                PointerAddress = new System.IntPtr(Handle.MemoryPointer).ToInt64();
            }
        }

        private MemoryHeap nativeheap;
        private List<NativeMemoryPoolBlock> blocks;

        /// <summary>
        /// Creates a new native memory pool.
        /// </summary>
        public NativeMemoryPool() : base()
        {
            nativeheap = MemoryHeap.CreateFixed(Default_MaxBufferSize);
            blocks = new(8);
        }

        internal unsafe void PoolBuffer(System.Byte* pointer)
        {
            if (pointer is null) { return; }
            System.Int64 addr = new System.IntPtr(pointer);
            for (System.Int32 I = 0; I < blocks.Count; I++) 
            {
                NativeMemoryPoolBlock block = blocks[I];
                if (block.PointerAddress == addr) {
                    block.Occupied = false;
                    blocks[I] = block;
                    break;
                }
            }
        }

        public override int MaxBufferSize => Default_MaxBufferSize;

        public override NativeMemoryOwner Rent(int minBufferSize = -1)
        {
            minBufferSize = minBufferSize == -1 ? DefaultBufferSize : minBufferSize;
            // Search first in the pool blocks for any available buffer.
            for (System.Int32 I = 0; I < blocks.Count; I++) 
            {
                NativeMemoryPoolBlock block = blocks[I];
                if (block.Occupied == false && block.Handle.MemoryLength >= minBufferSize) 
                {
                    if (minBufferSize - DefaultBufferSize > 0) {
                        // If this buffer is very small (Possibly such a small buffer is created when an appropriate block was not found)
                        // reallocate it to the default buffer size.
                        block.Handle.Reallocate(DefaultBufferSize);
                    }
                    var obj = new NativeMemoryOwner(this , block.Handle);
                    block.Occupied = true; // Mark as used
                    blocks[I] = block;
                    return obj;
                }
            }
            // We could not find an appropriate block, allocate a new one.
            // Do not cap it to the default buffer size, because we have already possibly done a lot of work.
            // Do this when the block will be re-used.
            NativeMemoryPoolBlock blk = new(this, minBufferSize);
            // Zeroize the memory contents
            blk.Handle.ZeroMemory();
            blk.Occupied = true;
            blocks.Add(blk);
            return new(this , blk.Handle);
        }

        protected override void Dispose(bool disposing)
        {
            nativeheap?.Dispose();
            nativeheap = null;
            blocks?.Clear(); // We can safely do only this because the native heap disposal will dispose all memory references.
            blocks = null;
        }
    }
}