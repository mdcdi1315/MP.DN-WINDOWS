

using MP.Annotations.CodeAnalysis;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace MP
{
    /// <summary>
    /// Defines a way to retrieve native memory blocks through a simple mechanism.
    /// </summary>
    [SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "This is a factory interface.")]
    public interface MemoryHandleFactory
    {
        /// <summary>
        /// Creates a new native memory handle of the specified size, and returns it.
        /// </summary>
        /// <param name="size">The size of the newly created memory handle.</param>
        /// <returns>The memory block of the size requested in the <paramref name="size"/> parameter.</returns>
        /// <exception cref="OutOfMemoryException">Not enough memory to create the memory handle.</exception>
        [Throws(typeof(OutOfMemoryException))]
        public abstract IMemoryHandle CreateMemoryHandle(System.UInt64 size);

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