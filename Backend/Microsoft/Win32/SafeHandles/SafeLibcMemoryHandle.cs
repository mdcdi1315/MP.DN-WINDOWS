
using MP;
using System;

namespace Microsoft.Win32.SafeHandles
{
    /// <summary>
    /// Allocates data from the native memory. <br />
    /// Conventionally called as 'Safe Libc Memory Handle' because on Unix the mem allocation function that is commonly is used is that exproted by libc. (libc.so on Linux and libc.dylib on OSX)
    /// </summary>
    public unsafe sealed class SafeLibcMemoryHandle : SafeBaseMemoryHandle
    {
        private System.UInt32 bytelength;
        private System.IntPtr refheap; // The reference handle to the heap used by this memory block

        // Called by the MemoryHeap class.
        [System.Diagnostics.DebuggerHidden]
        internal SafeLibcMemoryHandle(System.IntPtr heaphandle , System.Int32 size)
        {
            if (size < 0) { throw new ArgumentOutOfRangeException(nameof(size), "Size must be a positive or zeroed value."); }
            if (size == 0) { size = 1; } // Ensure that the block will be correctly allocated
            bytelength = size.ToUInt32();
            refheap = heaphandle;
            handle = new(Interop.Kernel32.HeapAlloc(refheap, Interop.Kernel32.HeapAllocFlags.None, bytelength));
            if (IsInvalid) { throw new OutOfMemoryException($"Cannot allocate {size} bytes."); }
        }

        /// <summary>
        /// Allocates native memory of the specified size.
        /// </summary>
        /// <param name="size">The number of bytes to allocate.</param>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="size"/> was not a positive or a zero value.</exception>
        /// <exception cref="MP.ExceptionSystem.NativeWindowsException">A native error occured.</exception>
        /// <exception cref="OutOfMemoryException">Allocation failed.</exception>
        public SafeLibcMemoryHandle(System.Int32 size)
        {
            if (size < 0) { throw new ArgumentOutOfRangeException(nameof(size), "Size must be a positive or zeroed value."); }
            if (size == 0) { size = 1; } // Ensure that the block will be correctly allocated
            bytelength = size.ToUInt32();
            refheap = Interop.Kernel32.GetProcessHeap();
            if (refheap == System.IntPtr.Zero) {
                throw new MP.ExceptionSystem.NativeWindowsException();
            }
            handle = new(Interop.Kernel32.HeapAlloc(refheap, Interop.Kernel32.HeapAllocFlags.None, bytelength));
            if (IsInvalid) { throw new OutOfMemoryException($"Cannot allocate {size} bytes."); }
        }

        public void Reallocate(System.Int32 newsize)
        {
            if (IsInvalid) { throw new ObjectDisposedException(nameof(SafeLibcMemoryHandle)); }
            if (newsize < 0) { throw new ArgumentOutOfRangeException(nameof(newsize) , "Size must be a positive or zeroed value."); }
            System.IntPtr old = handle;
            handle = new(Interop.Kernel32.HeapReAlloc(refheap , Interop.Kernel32.HeapReAllocFlags.None , old.ToPointer() , newsize.ToUInt32()));
            if (IsInvalid) { handle = old; throw new OutOfMemoryException($"Cannot re-allocate {newsize} bytes."); }
            bytelength = newsize.ToUInt32();
        }

        public override int MemoryLength => bytelength.ToInt32();

        protected override bool ReleaseHandle()
        {
            Interop.Kernel32.HeapFree(refheap , Interop.Kernel32.HeapFreeFlags.None , handle.ToPointer());
            refheap = System.IntPtr.Zero;
            return true;
        }
    }
}
