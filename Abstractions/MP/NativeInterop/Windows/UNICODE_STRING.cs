
using System;
using MP.Annotations;
using MP.Annotations.CodeAnalysis;
using System.Runtime.InteropServices;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace MP.NativeInterop.Windows
{
    /// <summary>
    /// Provides a way to create and pass <see cref="UNICODE_STRING"/> structures to unmanaged side. <br />
    /// These are typically used by the Windows NTDLL API's.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public unsafe struct UNICODE_STRING
    {
        /// <summary>Length in bytes, not including the null terminator, if any.</summary>
        public ushort Length;

        /// <summary>Max size of the buffer in bytes</summary>
        public ushort MaximumLength;

        /// <summary>The actual pointer to the string buffer</summary>
        public char* Buffer;

        /// <summary>
        /// Allocates and creates a new <see cref="UNICODE_STRING"/> structure from the specified string.
        /// </summary>
        /// <param name="str">The string to create the <see cref="UNICODE_STRING"/> structure from.</param>
        /// <exception cref="ArgumentNullException"><paramref name="str"/> was <see langword="null"/>.</exception>
        /// <exception cref="InsufficientMemoryException">Not enough memory to allocate for the <see cref="UNICODE_STRING"/> string buffer.</exception>
        /// <exception cref="ArgumentOutOfRangeException">The specified span is too large to fit into a constructed <see cref="UNICODE_STRING"/> structure.</exception>
        [Throws(
            typeof(ArgumentNullException),
            typeof(InsufficientMemoryException),
            typeof(ArgumentOutOfRangeException)
        )]
        [RequiresNativeLayer]
        public static UNICODE_STRING CreateFromStringExact(string str)
        {
            ArgumentNullException.ThrowIfNull(str);
            if (str.Length > 32767) { throw new ArgumentOutOfRangeException(nameof(str), "The provided string is too large to fit into a UNICODE_STRING structure"); }
            UNICODE_STRING created = new();
            created.Length = created.MaximumLength = (str.Length * sizeof(char)).ToUInt16();
            void* p = SystemInfo.GetDefaultMemoryManager().Allocate(created.Length);
            DebugProvider.WriteFormattedLine("UNICODE_STRING_DEBUG: Allocating buffer {0}, with size {1}", new IntPtr(created.Buffer), created.Length);
            fixed (char* p_source = str) {
                Unsafe.CopyBlockUnaligned(p, p_source, created.Length);
            }
            return created;
        }

        /// <summary>
        /// Allocates and creates a new <see cref="UNICODE_STRING"/> structure from the specified string. <br />
        /// This variant does allocate the maximum length of the buffer if a native method uses and returns results through this structure.
        /// </summary>
        /// <param name="str">The string to create the <see cref="UNICODE_STRING"/> structure from.</param>
        /// <exception cref="ArgumentNullException"><paramref name="str"/> was <see langword="null"/>.</exception>
        /// <exception cref="InsufficientMemoryException">Not enough memory to allocate for the <see cref="UNICODE_STRING"/> string buffer.</exception>
        /// <exception cref="ArgumentOutOfRangeException">The specified span is too large to fit into a constructed <see cref="UNICODE_STRING"/> structure.</exception>
        [Throws(
            typeof(ArgumentNullException),
            typeof(InsufficientMemoryException),
            typeof(ArgumentOutOfRangeException)
        )]
        [RequiresNativeLayer]
        public static UNICODE_STRING CreateFromString(string str) 
        {
            ArgumentNullException.ThrowIfNull(str);
            if (str.Length > 32767) { throw new ArgumentOutOfRangeException(nameof(str), "The provided string is too large to fit into a UNICODE_STRING structure"); }
            UNICODE_STRING created = new();
            created.Length = (str.Length * sizeof(char)).ToUInt16();
            void* p = SystemInfo.GetDefaultMemoryManager().Allocate(created.MaximumLength = 65535);
            DebugProvider.WriteFormattedLine("UNICODE_STRING_DEBUG: Allocating buffer {0}, with size {1}", new IntPtr(created.Buffer), created.Length);
            fixed (char* p_source = str) {
                Unsafe.CopyBlockUnaligned(p, p_source, created.Length);
            }
            return created;
        }

        /// <summary>
        /// Allocates and creates a new <see cref="UNICODE_STRING"/> structure from the specified read-only span of characters. <br />
        /// This variant does allocate the maximum length of the buffer if a native method uses and returns results through this structure.
        /// </summary>
        /// <param name="span">The span of characters to create the <see cref="UNICODE_STRING"/> structure from.</param>
        /// <exception cref="InsufficientMemoryException">Not enough memory to allocate for the <see cref="UNICODE_STRING"/> string buffer.</exception>
        /// <exception cref="ArgumentOutOfRangeException">The specified span is too large to fit into a constructed <see cref="UNICODE_STRING"/> structure.</exception>
        [RequiresNativeLayer]
        [Throws(typeof(InsufficientMemoryException), typeof(ArgumentOutOfRangeException))]
        public static UNICODE_STRING CreateFromSpan(ReadOnlySpan<System.Char> span)
        {
            if (span.Length > 32767) { 
                throw new ArgumentOutOfRangeException(nameof(span), "The provided string is too large to fit into a UNICODE_STRING structure"); 
            } else {
                UNICODE_STRING created = new();
                created.Length = (span.Length * sizeof(System.Char)).ToUInt16();
                System.Char* pbuf = (System.Char*)SystemInfo.GetDefaultMemoryManager().Allocate(created.MaximumLength = 65535);
                DebugProvider.WriteFormattedLine("UNICODE_STRING_DEBUG: Allocating buffer {0}, with size {1}", new IntPtr(created.Buffer), created.Length);
                Unsafe.CopyBlockUnaligned(
                    ref *(System.Byte*)pbuf,
                    ref Unsafe.As<System.Char, System.Byte>(ref Unsafe.AsRef(in span[0])),
                    created.Length
                );
                created.Buffer = pbuf;
                return created;
            }
        }

        /// <summary>
        /// Allocates and creates a new <see cref="UNICODE_STRING"/> structure from the specified read-only span of characters.
        /// </summary>
        /// <param name="span">The span of characters to create the <see cref="UNICODE_STRING"/> structure from.</param>
        /// <exception cref="InsufficientMemoryException">Not enough memory to allocate for the <see cref="UNICODE_STRING"/> string buffer.</exception>
        /// <exception cref="ArgumentOutOfRangeException">The specified span is too large to fit into a constructed <see cref="UNICODE_STRING"/> structure.</exception>
        [RequiresNativeLayer]
        [Throws(typeof(InsufficientMemoryException), typeof(ArgumentOutOfRangeException))]
        public static UNICODE_STRING CreateFromSpanExact(ReadOnlySpan<System.Char> span)
        {
            if (span.Length > 32767) { throw new ArgumentOutOfRangeException(nameof(span), "The provided string is too large to fit into a UNICODE_STRING structure"); }
            UNICODE_STRING strnew = new();
            strnew.MaximumLength = strnew.Length = (span.Length * sizeof(System.Char)).ToUInt16();
            System.Char* pbuf = (System.Char*)SystemInfo.GetDefaultMemoryManager().Allocate(strnew.Length);
            DebugProvider.WriteFormattedLine("UNICODE_STRING_DEBUG: Allocating buffer {0}, with size {1}", new IntPtr(strnew.Buffer), strnew.Length);
            Unsafe.CopyBlockUnaligned(
                ref *(System.Byte*)pbuf,
                ref Unsafe.As<System.Char, System.Byte>(ref Unsafe.AsRef(in span[0])),
                strnew.Length
            );
            strnew.Buffer = pbuf;
            return strnew;
        }

        /// <summary>
        /// Frees a previously created string from the static methods, and explicitly assigns the <see cref="Buffer"/> field to <see langword="null"/>. <br />
        /// Information about what was the buffer's length can be still retrieved.
        /// </summary>
        [RequiresNativeLayer]
        public void FreeCreatedString()
        {
            DebugProvider.WriteFormattedLine("UNICODE_STRING_DEBUG: Freeing buffer {0}, with size {1}", new IntPtr(Buffer), Length);
            SystemInfo.GetDefaultMemoryManager().Free(Buffer);
            Buffer = null;
        }

        /// <summary>
        /// Retrieves the contents of the <see cref="Buffer"/> memory block as a <see cref="string"/>.
        /// </summary>
        /// <returns>The string represented by this <see cref="UNICODE_STRING"/> structure.</returns>
        [return: MaybeNull]
        public readonly override string ToString() => Buffer is null ? null : new string(Buffer, 0 , Length / sizeof(char));
    }
}