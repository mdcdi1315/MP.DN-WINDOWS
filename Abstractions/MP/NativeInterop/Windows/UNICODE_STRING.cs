

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
        [Throws(
            typeof(ArgumentNullException),
            typeof(InsufficientMemoryException)
        )]
        [RequiresNativeLayer]
        public static UNICODE_STRING CreateFromStringExact(string str)
        {
            ArgumentNullException.ThrowIfNull(str);
            if (str.Length > 32767) { throw new ArgumentOutOfRangeException(nameof(str), "The provided string is too large to fit into a UNICODE_STRING structure"); }
            UNICODE_STRING created = new();
            created.Length = created.MaximumLength = (str.Length * sizeof(char)).ToUInt16();
            void* p = SystemInfo.GetMemoryHandleFactory().CreateRaw(created.Length);
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
        [Throws(
            typeof(ArgumentNullException),
            typeof(InsufficientMemoryException)
        )]
        [RequiresNativeLayer]
        public static UNICODE_STRING CreateFromString(string str) 
        {
            ArgumentNullException.ThrowIfNull(str);
            if (str.Length > 32767) { throw new ArgumentOutOfRangeException(nameof(str), "The provided string is too large to fit into a UNICODE_STRING structure"); }
            UNICODE_STRING created = new();
            created.Length = (str.Length * sizeof(char)).ToUInt16();
            void* p = SystemInfo.GetMemoryHandleFactory().CreateRaw(created.MaximumLength = 65535);
            fixed (char* p_source = str) {
                Unsafe.CopyBlockUnaligned(p, p_source, created.Length);
            }
            return created;
        }

        /// <summary>
        /// Frees a previously created string from the static methods, and explicitly assigns the <see cref="Buffer"/> field to <see langword="null"/>. <br />
        /// Information about what was the buffer's length can be still retrieved.
        /// </summary>
        [RequiresNativeLayer]
        public void FreeCreatedString()
        {
            SystemInfo.GetMemoryHandleFactory().FreeRaw(Buffer);
            Buffer = null;
        }

        /// <summary>
        /// Retrieves the contents of the <see cref="Buffer"/> memory block as a <see cref="string"/>.
        /// </summary>
        /// <returns>The string represented by this <see cref="UNICODE_STRING"/> structure.</returns>
        [return: MaybeNull]
        public readonly override string ToString() => Buffer == null ? null : new string(Buffer, 0 , Length / sizeof(char));
    }
}