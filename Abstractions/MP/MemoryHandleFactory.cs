
using System;
using System.Numerics;
using MP.Annotations.CodeAnalysis;
using Microsoft.Win32.SafeHandles;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace MP
{
    /// <summary>
    /// Defines a way to retrieve native memory blocks through a simple mechanism.
    /// </summary>
    [SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "This is a factory interface.")]
    public unsafe interface MemoryHandleFactory
    {
        private delegate void FreeFunction(void* p_handle);

        private sealed class DefaultMemHandle : SafeBaseMemoryHandle
        {
            private int length;
            private FreeFunction function;

            public DefaultMemHandle(MemoryHandleFactory p_this, ulong requested)
            {
                handle = new(p_this.CreateRaw(requested));
                function = new(p_this.FreeRaw);
                length = requested.ToInt32();
            }

            public override int MemoryLength => length;

            protected override bool ReleaseHandle() {
                function(handle.ToPointer());
                function = null;
                return true;
            }
        }

        private sealed class DefaultAlignedMemHandle : SafeBaseMemoryHandle
        {
            private int length;
            private FreeFunction function;

            public DefaultAlignedMemHandle(MemoryHandleFactory p_this, ulong requested, uint align)
            {
                handle = new(p_this.CreateRawAligned(requested, align));
                function = new(p_this.FreeRawAligned);
                length = requested.ToInt32();
            }

            public override int MemoryLength => length;

            protected override bool ReleaseHandle()
            {
                function(handle.ToPointer());
                function = null;
                return true;
            }
        }

        /// <summary>
        /// Creates directly a memory block, and returns it. The block is unaligned.
        /// </summary>
        /// <param name="size">The size of the newly created memory block.</param>
        /// <returns>The raw memory block.</returns>
        /// <exception cref="InsufficientMemoryException">Not enough memory to create the requested memory block.</exception>
        [return: NotNull]
        [Throws(typeof(InsufficientMemoryException))]
        public abstract void* CreateRaw(System.UInt64 size);

        /// <summary>
        /// Creates directly a memory block, and returns it. The block is aligned. <br />
        /// It should be noted down that when you override this method, you should also override the <see cref="FreeRawAligned(void*)"/> method.
        /// </summary>
        /// <param name="size">The size of the newly created memory block.</param>
        /// <param name="align">The alignment to use for this memory block.</param>
        /// <returns>The raw memory block.</returns>
        /// <exception cref="InsufficientMemoryException">Not enough memory to create the requested memory block.</exception>
        [return: NotNull]
        [Throws(typeof(InsufficientMemoryException))]
        public void* CreateRawAligned(System.UInt64 size, System.UInt32 align) => RawAlignedWithOffset(size, align, 0UL);

        private void* RawAlignedWithOffset(System.UInt64 size, System.UInt64 align, System.UInt64 offset)
        {
            System.UIntPtr ptr, retptr, gap;
            System.UInt64 nonuser_size, block_size, ptr_sz = (ulong)sizeof(void*);

            /* validation section */
            if (!BitOperations.IsPow2(align)) {
                // _VALIDATE_RETURN(IS_2_POW_N(align), EINVAL, nullptr);
                throw new ArgumentException("The specified alignment is not a power of 2", nameof(align));
            }
            if (offset != 0UL && offset >= size) {
                // _VALIDATE_RETURN(offset == 0 || offset < size, EINVAL, nullptr);
                throw new ArgumentException("The specified offset is larger than the size of the memory block requested", nameof(align));
            }

            align = (align > ptr_sz ? align : ptr_sz) - 1;

            /* gap = number of bytes needed to round up offset to align with PTR_SZ*/
            gap = (nuint)((0UL - offset) & (ptr_sz - 1UL));

            nonuser_size = ptr_sz + gap + align;
            block_size = nonuser_size + size;
            if (size > block_size) {
                // _VALIDATE_RETURN_NOEXC(size <= block_size, ENOMEM, nullptr)
                throw new InsufficientMemoryException("Not enough block size to allocate the aligned memory!");
            }

            if ((ptr = (nuint)CreateRaw(block_size)) == 0) { return null; }

            retptr = (nuint)(((ptr + nonuser_size + offset) & ~align) - offset);
            ((nuint*)(retptr - gap))[-1] = ptr;

            return (void*)retptr;
        }

        /// <summary>
        /// Frees a previously allocated raw memory block by using the <see cref="CreateRaw(ulong)"/> API.
        /// </summary>
        /// <param name="ptr">The pointer pointing to the memory block. Can be <see langword="null"/>, which in such case this method must do nothing.</param>
        public abstract void FreeRaw([MaybeNull] void* ptr);

        /// <summary>
        /// Frees a previously allocated raw memory block by using the <see cref="CreateRawAligned(ulong, uint)"/> API.
        /// </summary>
        /// <param name="block">The pointer pointing to the memory block. Can be <see langword="null"/>, which in such case this method must do nothing.</param>
        public void FreeRawAligned([MaybeNull] void* block)
        {
            System.UIntPtr ptr;
            System.UIntPtr ptr_sz = (nuint)sizeof(void*);

            if (block is null) { return; }

            ptr = (nuint)block;

            /* ptr points to the pointer to starting of the memory block */
            ptr = (ptr & ~(ptr_sz - 1)) - ptr_sz;

            /* ptr is the pointer to the start of memory block*/
            ptr = *(nuint*)ptr;
            FreeRaw((void*)ptr);
        }

        /// <summary>
        /// Creates a new native memory handle of the specified size, and returns it.
        /// </summary>
        /// <param name="size">The size of the newly created memory handle.</param>
        /// <returns>The memory block of the size requested in the <paramref name="size"/> parameter.</returns>
        /// <exception cref="InsufficientMemoryException">Not enough memory to create the memory handle.</exception>
        [Throws(typeof(InsufficientMemoryException))]
        public IMemoryHandle CreateMemoryHandle(System.UInt64 size) => new DefaultMemHandle(this, size);

        /// <summary>
        /// Creates a new native aligned memory handle of the specified size, and returns it.
        /// </summary>
        /// <param name="size">The size of the newly created memory handle.</param>
        /// <param name="align">The alignment to use for the memory block created by this memory handle.</param>
        /// <returns>The memory block of the size requested in the <paramref name="size"/> parameter.</returns>
        /// <exception cref="InsufficientMemoryException">Not enough memory to create the memory handle.</exception>
        [Throws(typeof(InsufficientMemoryException))]
        public IMemoryHandle CreateAlignedMemoryHandle(System.UInt64 size, System.UInt32 align) => new DefaultAlignedMemHandle(this, size, align);

        /// <summary>
        /// Returns platform memory statistics, if these are available. <br />
        /// The values supported by each platform may be different, and maybe these do not even exist, where in such case this method returns <see langword="null"/>.
        /// </summary>
        /// <returns>An object extending the <see cref="IAttributeable"/> interface.</returns>
        [return: MaybeNull]
        public abstract IAttributeable GetStatistics();
    }

    /// <summary>
    /// Defines handy extension methods for the <see cref="MemoryHandleFactory"/> interface.
    /// </summary>
    public static unsafe class MemoryHandleFactoryExtensions
    {
        /// <summary>Creates a new native memory handle of the specified size, and returns it.</summary>
        /// <param name="handlefactory">The <see cref="MemoryHandleFactory"/> instance to create a new native memory handle from.</param>
        /// <param name="size">The size, in bytes, of the newly created memory handle.</param>
        /// <returns>The memory block of the size requested in the <paramref name="size"/> parameter.</returns>
        /// <exception cref="OutOfMemoryException">Not enough memory to create the memory handle.</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="size"/> was negative.</exception>
        [Throws(typeof(ArgumentOutOfRangeException) , typeof(OutOfMemoryException))]
        public static IMemoryHandle CreateMemoryHandle(this MemoryHandleFactory handlefactory, int size)
        {
            if (size < 0) {
                throw new ArgumentOutOfRangeException(nameof(size) , "size cannot be negative.");
            }
            return handlefactory.CreateMemoryHandle(size.ToUInt64());
        }

        /// <summary>Creates a new native memory handle of the specified size, and returns it.</summary>
        /// <param name="handlefactory">The <see cref="MemoryHandleFactory"/> instance to create a new native memory handle from.</param>
        /// <param name="size">The size, in bytes, of the newly created memory handle.</param>
        /// <returns>The memory block of the size requested in the <paramref name="size"/> parameter.</returns>
        /// <exception cref="OutOfMemoryException">Not enough memory to create the memory handle.</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="size"/> was negative.</exception>
        [Throws(typeof(ArgumentOutOfRangeException), typeof(OutOfMemoryException))]
        public static IMemoryHandle CreateMemoryHandle(this MemoryHandleFactory handlefactory, long size)
        {
            if (size < 0) {
                throw new ArgumentOutOfRangeException(nameof(size), "size cannot be negative.");
            }
            return handlefactory.CreateMemoryHandle(size.ToUInt64());
        }

        /// <summary>
        /// Creates a new <see cref="IMemoryHandle"/> instance from the specified string contents. <br />
        /// The data are packed as UTF-16 contents with NULL termination.
        /// </summary>
        /// <param name="handlefactory">The <see cref="MemoryHandleFactory"/> instance to create a new native memory handle from.</param>
        /// <param name="str">The string to create it's native memory representation.</param>
        /// <returns>A new memory block containing the string passed in the <paramref name="str"/> and it is null-terminated.</returns>
        public static IMemoryHandle CreateFromStringUTF16NullTerminated(this MemoryHandleFactory handlefactory , String str)
        {
            ArgumentNullException.ThrowIfNull(str);
            System.UInt32 len = ((str.Length + 1) * sizeof(System.Char)).ToUInt32();
            IMemoryHandle mem = handlefactory.CreateMemoryHandle(len);
            fixed (System.Char* pcs = str)
            {
                Unsafe.CopyBlockUnaligned(mem.MemoryPointer, pcs, len);
            }
            return mem;
        }

        /// <summary>
        /// Creates a new <see cref="IMemoryHandle"/> instance from the specified string contents. <br />
        /// The data are packed as UTF-8 contents with NULL termination.
        /// </summary>
        /// <param name="handlefactory">The <see cref="MemoryHandleFactory"/> instance to create a new native memory handle from.</param>
        /// <param name="str">The string to create it's native memory representation.</param>
        /// <returns>A new memory block containing the string passed in the <paramref name="str"/> and it is null-terminated.</returns>
        public static IMemoryHandle CreateFromStringUTF8NullTerminated(this MemoryHandleFactory handlefactory , String str)
        {
            ArgumentNullException.ThrowIfNull(str);
            var utf8 = System.Text.Encoding.UTF8;
            IMemoryHandle handle;
            int desiredchars = str.Length + 1;
            fixed (System.Char* pcs = str)
            {
                int lengthinbytes = utf8.GetByteCount(pcs, desiredchars);
                handle = handlefactory.CreateMemoryHandle(lengthinbytes);
                utf8.GetBytes(pcs, desiredchars, handle.MemoryPointer, lengthinbytes);
            }
            return handle;
        }

        /// <summary>
        /// Creates a new <see cref="IMemoryHandle"/> instance from the specified string contents. <br />
        /// The data are packed as UTF-16 contents. The NULL terminator is not appended to the memory block, it is assumed that you can handle string length.
        /// </summary>
        /// <param name="handlefactory">The <see cref="MemoryHandleFactory"/> instance to create a new native memory handle from.</param>
        /// <param name="str">The string to create it's native memory representation.</param>
        /// <returns>A new memory block containing the string passed in the <paramref name="str"/>.</returns>
        public static IMemoryHandle CreateFromStringUTF16FixedLength(this MemoryHandleFactory handlefactory , String str)
        {
            ArgumentNullException.ThrowIfNull(str);
            System.UInt32 len = (str.Length * sizeof(System.Char)).ToUInt32();
            IMemoryHandle mem = handlefactory.CreateMemoryHandle(len);
            fixed (System.Char* pcs = str)
            {
                Unsafe.CopyBlockUnaligned(mem.MemoryPointer, pcs, len);
            }
            return mem;
        }

        /// <summary>
        /// Creates a new <see cref="IMemoryHandle"/> instance from the specified string contents. <br />
        /// The data are packed as UTF-8 contents. The NULL terminator is not appended to the memory block, it is assumed that you can handle string length.
        /// </summary>
        /// <param name="handlefactory">The <see cref="MemoryHandleFactory"/> instance to create a new native memory handle from.</param>
        /// <param name="str">The string to create it's native memory representation.</param>
        /// <returns>A new memory block containing the string passed in the <paramref name="str"/>.</returns>
        public static IMemoryHandle CreateFromStringUTF8FixedLength(this MemoryHandleFactory handlefactory, String str)
        {
            ArgumentNullException.ThrowIfNull(str);
            var utf8 = System.Text.Encoding.UTF8;
            IMemoryHandle handle;
            fixed (System.Char* pcs = str)
            {
                int lengthinbytes = utf8.GetByteCount(pcs, str.Length);
                handle = handlefactory.CreateMemoryHandle(lengthinbytes);
                utf8.GetBytes(pcs, str.Length, handle.MemoryPointer, lengthinbytes);
            }
            return handle;
        }
    }
}