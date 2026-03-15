
using Microsoft.Win32.SafeHandles;
using System.Runtime.CompilerServices;

namespace MP.NativeInterop
{
    /// <summary>
    /// Specialization of the <see cref="IMemoryHandle"/> interface for string data. <br />
    /// This is mostly provided for when needing to pass around strings to native methods. <br />
    /// Prefer to use this whenever possible, as it is currently the most type-safe method for passing strings around.
    /// </summary>
    public abstract class StringHandle : SafeBaseMemoryHandle
    {
        /// <summary>
        /// Gets the absolute size, in bytes, of this string handle.
        /// </summary>
        public override ulong MemoryLength => (LengthInChars.ToUInt64() * SingleCharSize.ToUInt64()) + (HasNullTerminationChar ? SingleCharSize : 0).ToUInt64();

        /// <summary>
        /// Gets the number of characters contained in the current string handle.
        /// </summary>
        public abstract int LengthInChars { get; }

        /// <summary>
        /// Gets the number of bytes required so that to store a single string character.
        /// </summary>
        public abstract int SingleCharSize { get; }

        /// <summary>
        /// Gets a value whether this string handle has a NULL termination character appended to the end of the string.
        /// </summary>
        public abstract bool HasNullTerminationChar { get; }

        /// <summary>
        /// Reconstructs the native memory contents back to a .NET string instance.
        /// </summary>
        /// <returns>The .NET string representing the contents of this string handle.</returns>
        public abstract override string ToString();

        /// <summary>
        /// Provides a direct pointer translation of this string handle.
        /// </summary>
        /// <param name="self">The string handle to obtain it's pointer representation.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe implicit operator void*(StringHandle self) => self.MemoryPointer;

        /// <summary>
        /// Provides a direct pointer translation of this string handle.
        /// </summary>
        /// <param name="self">The string handle to obtain it's pointer representation.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe implicit operator byte*(StringHandle self) => self.MemoryPointer;

        /// <summary>
        /// Provides a direct pointer translation of this string handle.
        /// </summary>
        /// <param name="self">The string handle to obtain it's pointer representation.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe implicit operator char*(StringHandle self) => (char*)self.MemoryPointer;
    }
}
