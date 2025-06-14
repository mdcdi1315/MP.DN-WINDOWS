using MP;
using System;

namespace Microsoft.Win32.SafeHandles
{
    /// <summary>
    /// Like <see cref="SafeLibcMemoryHandle"/>, this memory handle allocates memory from Microsoft's OLE allocator. <br />
    /// Specifically it uses the <strong>CoTaskMemAlloc</strong> function.
    /// </summary>
    public sealed unsafe class SafeCoTaskMemoryHandle : SafeBaseMemoryHandle
    {
        private System.UInt32 bytelength;

        /// <summary>
        /// Allocates native memory of the specified size.
        /// </summary>
        /// <param name="size">The number of bytes to allocate.</param>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="size"/> was not a positive or a zero value.</exception>
        /// <exception cref="MP.ExceptionSystem.NativeWindowsException">A native error occured.</exception>
        /// <exception cref="OutOfMemoryException">Allocation failed.</exception>
        public SafeCoTaskMemoryHandle(System.Int32 size)
        {
            if (size < 0) { throw new ArgumentOutOfRangeException(nameof(size), "Size must be a positive or zeroed value."); }
            if (size == 0) { size = 1; } // Ensure that the block will be correctly allocated
            bytelength = size.ToUInt32();
            handle = new(Interop.Ole32.CoTaskMemAlloc(bytelength));
            if (IsInvalid) { throw new OutOfMemoryException($"Cannot allocate {size} bytes."); }
        }

        public void Reallocate(System.Int32 newsize)
        {
            if (IsInvalid) { throw new ObjectDisposedException(nameof(SafeLibcMemoryHandle)); }
            if (newsize < 0) { throw new ArgumentOutOfRangeException(nameof(newsize), "Size must be a positive or zeroed value."); }
            System.IntPtr old = handle;
            handle = new(Interop.Ole32.CoTaskMemRealloc(old.ToPointer(), newsize.ToUInt32()));
            if (IsInvalid) { handle = old; throw new OutOfMemoryException($"Cannot re-allocate {newsize} bytes."); }
            bytelength = newsize.ToUInt32();
        }

        public override int MemoryLength => bytelength.ToInt32();

        protected override bool ReleaseHandle()
        {
            Interop.Ole32.CoTaskMemFree(handle.ToPointer());
            return true;
        }
    }
}
