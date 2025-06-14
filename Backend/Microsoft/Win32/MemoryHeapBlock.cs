
using MP;
using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Microsoft.Win32
{
    /// <summary>
    /// Defines the Windows memory heap API wrapped into a critical handle.
    /// </summary>
    public sealed class MemoryHeap : CriticalHandle
    {
        [Flags]
        private enum MEMHEAPCLASSFLAGS : System.Byte
        {
            None = 0,
            IsDefaultProcessHeap = 0x02,
            IsProtectedHeap = 0x04
        }

        private MEMHEAPCLASSFLAGS flags;

        /// <summary>
        /// Gets the default process heap object, which is always expandable.
        /// </summary>
        public static MemoryHeap ProcessHeap
        {
            get {
                MemoryHeap obj = new();
                obj.flags = MEMHEAPCLASSFLAGS.IsDefaultProcessHeap;
                obj.handle = Interop.Kernel32.GetProcessHeap();
                if (obj.IsInvalid) {
                    throw new MP.ExceptionSystem.NativeWindowsException();
                }
                return obj;
            }
        }

        /// <summary>
        /// Creates a new default memory heap with an expandable capacity.
        /// </summary>
        /// <returns>The created memory heap.</returns>
        public static MemoryHeap Create()
        {
            MemoryHeap obj = new();
            obj.handle = Interop.Kernel32.HeapCreate(Interop.Kernel32.HeapCreateFlags.None, 8192, 0);
            if (obj.IsInvalid) {
                throw new MP.ExceptionSystem.NativeWindowsException();
            }
            return obj;
        }

        /// <summary>
        /// Creates a new memory heap with a fixed capacity. <br />
        /// Note that even if this implicit bound is allocated and you request more <br />
        /// memory, the allocation will not fail, but instead a virtual allocation will happen.
        /// </summary>
        /// <returns>The created memory heap object.</returns>
        /// <exception cref="ArgumentException"><paramref name="fixedsize"/> was less than 1.</exception>
        public static MemoryHeap CreateFixed(System.Int32 fixedsize)
        {
            if (fixedsize < 1) { throw new ArgumentException("Fixed capacity must not be less than one (1) byte." , nameof(fixedsize)); }
            MemoryHeap obj = new();
            obj.handle = Interop.Kernel32.HeapCreate(Interop.Kernel32.HeapCreateFlags.None, 0, fixedsize.ToUInt32());
            if (obj.IsInvalid) {
                throw new MP.ExceptionSystem.NativeWindowsException();
            }
            return obj;
        }

        private MemoryHeap() : base(System.IntPtr.Zero)
        {
            flags = MEMHEAPCLASSFLAGS.None;
            handle = System.IntPtr.Zero;
        }

        /// <summary>
        /// Allocates a new memory block and returns it as a <see cref="SafeHandles.SafeLibcMemoryHandle"/> instance.
        /// </summary>
        /// <param name="size">The number of bytes to allocate.</param>
        /// <returns>The allocated memory block , returned as a memory block handle.</returns>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="size"/> was not a positive or a zero value.</exception>
        /// <exception cref="MP.ExceptionSystem.NativeWindowsException">A native error occured.</exception>
        /// <exception cref="OutOfMemoryException">Allocation failed.</exception>
        public SafeHandles.SafeLibcMemoryHandle Allocate(System.Int32 size)
        {
            if (IsInvalid) { throw new ObjectDisposedException(nameof(MemoryHeap)); }
            return new(handle, size);
        }

        /// <summary>
        /// Gets a value whether the current memory heap object has been corrupted or not. <br />
        /// If it happens this object to be the value of <see cref="ProcessHeap"/> property ,  <br />
        /// a critical <see cref="AccessViolationException"/> is thrown to the caller. <br />
        /// For corrupted memory heaps, you can safely destroy them by using the <see cref="CriticalHandle.Dispose"/> method.
        /// </summary>
        public unsafe System.Boolean IsValid
        {
            get {
                if (IsInvalid) { throw new ObjectDisposedException(nameof(MemoryHeap)); }
                System.Boolean ret = Interop.Kernel32.HeapValidate(handle, Interop.Kernel32.HeapValidateFlags.None) != Interop.BOOL.FALSE;
                if (ret == false && flags.HasFlag(MEMHEAPCLASSFLAGS.IsDefaultProcessHeap)) {
                    throw new AccessViolationException("Default process heap corrupted!");
                }
                return ret;
            }
        }

        public override bool IsInvalid
        {
            // Usually this must be replaced with the accessor contents.
            [MethodImpl(MethodImplOptions.AggressiveInlining)] 
            get => handle == System.IntPtr.Zero;
        }

        protected override bool ReleaseHandle()
        {
            if (flags.HasFlag(MEMHEAPCLASSFLAGS.IsDefaultProcessHeap | MEMHEAPCLASSFLAGS.IsProtectedHeap)) {
                // Ignore memory heaps returned by non-managed locations (Such as the GetProcessHeap function)
                return true;
            }
            Interop.BOOL ret = Interop.Kernel32.HeapDestroy(handle);
            handle = System.IntPtr.Zero;
            return ret != Interop.BOOL.FALSE;
        }
    }
}