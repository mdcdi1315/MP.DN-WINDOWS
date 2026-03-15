
using System;
using MP.Utilities;
using MP.Annotations.CodeAnalysis;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace MP.NativeInterop
{
    static unsafe partial class INativeMemoryManagerExtensions
    {
        private abstract class BaseStringHandle : StringHandle
        {
            private FreeFunction ff;
            protected int string_length;
            protected bool null_terminated;

            private delegate bool FreeFunction([AllowNull] void* ptr);

            protected BaseStringHandle(INativeMemoryManager manager) => ff = new(manager.Free);

            public sealed override int LengthInChars => string_length;

            public sealed override bool HasNullTerminationChar => null_terminated;

            protected sealed override bool ReleaseHandle()
            {
                if (ff is null) {
                    return false;
                } else {
                    bool value = ff(handle.ToPointer());
                    ff = null;
                    return value;
                }
            }

            // Efficient wrappers around Encoding instances
            // These are used to convert our strings in string handles and vice-versa.

            // Something similar is also done in the StreamMethods class. See that for more information.

            protected void StoreAsCharactersWithEncoding(System.Text.Encoding enc, String str)
            {
                System.Text.Encoder encoder = enc.GetEncoder();

                ReadOnlySpan<System.Char> char_buffer = (null_terminated ? (str + "\0") : str).AsSpan();

                System.Byte[] temp_buffer = System.Buffers.ArrayPool<System.Byte>.Shared.Rent(2048);

                try {
                    ref byte p_dest_reference = ref this.PointerAsReference(),
                                p_src_reference = ref temp_buffer[0];

                    bool completed;

                    uint tbu;
                    ulong used = 0;

                    do {
                        encoder.Convert(char_buffer, temp_buffer, true, out int cu, out int bu, out completed);

                        tbu = bu.ToUInt32();

                        Unsafe.CopyBlockUnaligned(
                            ref Unsafe.AddByteOffset(ref p_dest_reference, new UIntPtr(used)),
                            ref p_src_reference,
                            tbu
                        );

                        used += tbu;

                        char_buffer = char_buffer.Slice(cu);
                    } while (!completed);
                } finally {
                    System.Buffers.ArrayPool<System.Byte>.Shared.Return(temp_buffer);
                }
            }

            protected String ReadCharactersWithEncoding(System.Text.Encoding encoding)
            {
                System.Text.StringBuilder sb = new(string_length);

                System.Text.Decoder dec = encoding.GetDecoder();

                System.Char[] data = System.Buffers.ArrayPool<System.Char>.Shared.Rent(2048);

                try {
                    System.Byte* cp = MemoryPointer;

                    ReadOnlySpan<System.Byte> ros;

                    ulong len_processed = 0UL, mem_len = MemoryLength;

                    while (len_processed < mem_len)
                    {
                        int rb = 0;
                        ros = new ReadOnlySpan<byte>(cp, MathHelpers.ComputeBufferSize(len_processed, mem_len, 2048));
                        bool completed;
                        do
                        {
                            dec.Convert(ros, data, len_processed + 2048UL > mem_len, out int bu, out int cu, out completed);

                            rb += bu;
                            sb.Append(data, 0, cu);
                        } while (!completed);

                        cp += rb;
                        len_processed += rb.ToUInt32();
                    }
                } finally {
                    System.Buffers.ArrayPool<System.Char>.Shared.Return(data);
                }

                if (null_terminated) {
                    sb.Remove(sb.Length - 1, 1);
                }

                return sb.ToString();
            }
        }

        private sealed class UTF16LEStringHandle : BaseStringHandle
        {
            public UTF16LEStringHandle(INativeMemoryManager mgr, String str, bool null_terminated) : base(mgr)
            {
                string_length = str.Length;
                this.null_terminated = null_terminated;
                handle = new(mgr.Allocate(MemoryLength));
                StoreAsCharactersWithEncoding(System.Text.Encoding.Unicode, str);
            }

            public override int SingleCharSize => sizeof(System.Char);

            public override string ToString() => ReadCharactersWithEncoding(System.Text.Encoding.Unicode);
        }

        private sealed class UTF8StringHandle : BaseStringHandle
        {
            private readonly ulong size;

            public UTF8StringHandle(INativeMemoryManager manager, String str, bool null_terminated) : base(manager)
            {
                string_length = str.Length;
                size = System.Text.Encoding.UTF8.GetByteCount(str).ToUInt64() + ((this.null_terminated = null_terminated) ? SingleCharSize : 0).ToUInt64();
                handle = new(manager.Allocate(size));
                StoreAsCharactersWithEncoding(System.Text.Encoding.UTF8, str);
            }

            public override ulong MemoryLength => size;

            public override int SingleCharSize => sizeof(System.Byte);

            public override string ToString() => ReadCharactersWithEncoding(System.Text.Encoding.UTF8);
        }

        private sealed class UTF16BEStringHandle : BaseStringHandle
        {
            public UTF16BEStringHandle(INativeMemoryManager mgr, String str, bool null_terminated) : base(mgr)
            {
                string_length = str.Length;
                this.null_terminated = null_terminated;
                handle = new(mgr.Allocate(MemoryLength));
                StoreAsCharactersWithEncoding(System.Text.Encoding.BigEndianUnicode, str);
            }

            public override int SingleCharSize => sizeof(System.Char);

            public override string ToString() => ReadCharactersWithEncoding(System.Text.Encoding.BigEndianUnicode);
        }

        private sealed class ASCIIStringHandle : BaseStringHandle
        {
            public ASCIIStringHandle(INativeMemoryManager manager, String str, bool null_terminated) : base(manager)
            {
                string_length = str.Length;
                this.null_terminated = null_terminated;
                handle = new(manager.Allocate(MemoryLength));
                StoreAsCharactersWithEncoding(System.Text.Encoding.ASCII , str);
            }

            public override int SingleCharSize => 1;

            public override string ToString() => ReadCharactersWithEncoding(System.Text.Encoding.ASCII);
        }

        /// <summary>
        /// Creates a new UTF-16 string handle from the specified string, encoded with little endianess.
        /// </summary>
        /// <param name="manager">The native memory manager to allocate memory from.</param>
        /// <param name="str">The string to create as a string handle.</param>
        /// <param name="null_terminated">A value whether the string handle should also append a NULL termination character at the end. Specify <see langword="false"/> when this is not needed.</param>
        /// <returns>A new <see cref="StringHandle"/> instance containing the contents of <paramref name="str"/>.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="str"/> is <see langword="null"/>.</exception>
        /// <exception cref="InsufficientMemoryException">Not enough memory to allocate the string handle itself.</exception>
        [Throws(typeof(ArgumentNullException), typeof(InsufficientMemoryException))]
        public static StringHandle CreateUTF16LEStringHandle(this INativeMemoryManager manager, String str, Boolean null_terminated = true)
        {
            ArgumentNullException.ThrowIfNull(str);
            return new UTF16LEStringHandle(manager, str, null_terminated);
        }

        /// <summary>
        /// Creates a new UTF-16 string handle from the specified string, encoded with big endianess.
        /// </summary>
        /// <param name="manager">The native memory manager to allocate memory from.</param>
        /// <param name="str">The string to create as a string handle.</param>
        /// <param name="null_terminated">A value whether the string handle should also append a NULL termination character at the end. Specify <see langword="false"/> when this is not needed.</param>
        /// <returns>A new <see cref="StringHandle"/> instance containing the contents of <paramref name="str"/>.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="str"/> is <see langword="null"/>.</exception>
        /// <exception cref="InsufficientMemoryException">Not enough memory to allocate the string handle itself.</exception>
        [Throws(typeof(ArgumentNullException), typeof(InsufficientMemoryException))]
        public static StringHandle CreateUTF16BEStringHandle(this INativeMemoryManager manager, String str, Boolean null_terminated = true)
        {
            ArgumentNullException.ThrowIfNull(str);
            return new UTF16BEStringHandle(manager, str, null_terminated);
        }

        /// <summary>
        /// Creates a new UTF-16 string handle from the specified string, encoded by the platform's endianess.
        /// </summary>
        /// <param name="manager">The native memory manager to allocate memory from.</param>
        /// <param name="str">The string to create as a string handle.</param>
        /// <param name="null_terminated">A value whether the string handle should also append a NULL termination character at the end. Specify <see langword="false"/> when this is not needed.</param>
        /// <returns>A new <see cref="StringHandle"/> instance containing the contents of <paramref name="str"/>.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="str"/> is <see langword="null"/>.</exception>
        /// <exception cref="InsufficientMemoryException">Not enough memory to allocate the string handle itself.</exception>
        [Throws(typeof(ArgumentNullException), typeof(InsufficientMemoryException))]
        public static StringHandle CreatePlatformDependentUTF16StringHandle(this INativeMemoryManager manager, String str, Boolean null_terminated = true)
        {
            ArgumentNullException.ThrowIfNull(str);
            return UnsafeMethods.IsLittleEndian ? new UTF16LEStringHandle(manager, str, null_terminated) : new UTF16BEStringHandle(manager, str, null_terminated);
        }

        /// <summary>
        /// Creates a new UTF-8 string handle from the specified string.
        /// </summary>
        /// <param name="manager">The native memory manager to allocate memory from.</param>
        /// <param name="str">The string to create as a string handle.</param>
        /// <param name="null_terminated">A value whether the string handle should also append a NULL termination character at the end. Specify <see langword="false"/> when this is not needed.</param>
        /// <returns>A new <see cref="StringHandle"/> instance containing the contents of <paramref name="str"/>.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="str"/> is <see langword="null"/>.</exception>
        /// <exception cref="InsufficientMemoryException">Not enough memory to allocate the string handle itself.</exception>
        [Throws(typeof(ArgumentNullException), typeof(InsufficientMemoryException))]
        public static StringHandle CreateUTF8StringHandle(this INativeMemoryManager manager, String str, Boolean null_terminated = true)
        {
            ArgumentNullException.ThrowIfNull(str);
            return new UTF8StringHandle(manager, str, null_terminated);
        }

        /// <summary>
        /// Creates a new ASCII string handle from the specified string.
        /// </summary>
        /// <param name="manager">The native memory manager to allocate memory from.</param>
        /// <param name="str">The string to create as a string handle.</param>
        /// <param name="null_terminated">A value whether the string handle should also append a NULL termination character at the end. Specify <see langword="false"/> when this is not needed.</param>
        /// <returns>A new <see cref="StringHandle"/> instance containing the contents of <paramref name="str"/>.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="str"/> is <see langword="null"/>.</exception>
        /// <exception cref="InsufficientMemoryException">Not enough memory to allocate the string handle itself.</exception>
        [Throws(typeof(ArgumentNullException), typeof(InsufficientMemoryException))]
        public static StringHandle CreateASCIIStringHandle(this INativeMemoryManager manager, String str, Boolean null_terminated = true)
        {
            ArgumentNullException.ThrowIfNull(str);
            return new ASCIIStringHandle(manager, str, null_terminated);
        }

        
    }
}