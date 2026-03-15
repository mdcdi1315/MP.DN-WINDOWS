
using System;
using MP.Annotations.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace MP.Collections.Hashing
{
    /// <summary>
    /// Extension methods for basic hashing interfaces.
    /// </summary>
    public static class HashingExtensions
    {
        /// <summary>
        /// Executes the <see cref="I32BitHasher.Hash(ref byte, int)"/> method and returns the result as a <see cref="int"/>.
        /// </summary>
        /// <param name="hasher">The <see cref="I32BitHasher"/> instance to be used.</param>
        /// <param name="input">The input reference to be used.</param>
        /// <param name="length">The length of <paramref name="input"/>.</param>
        /// <returns>The hash value of the specified memory block.</returns>
        public static System.Int32 HashAsInt32(this I32BitHasher hasher, ref System.Byte input, int length) => hasher.Hash(ref input, length).ToInt32();

        /// <summary>
        /// Executes the <see cref="I32BitStreamingHasher.Digest"/> method and returns the result as a <see cref="int"/>.
        /// </summary>
        /// <param name="hasher">The <see cref="I32BitStreamingHasher"/> instance to be used.</param>
        /// <returns>The hash value accumulated so far.</returns>
        public static System.Int32 DigestAsInt32(this I32BitStreamingHasher hasher) => hasher.Digest().ToInt32();

        /// <summary>Hashes the specified <see cref="System.Int16"/>.</summary>
        /// <param name="hasher">The <see cref="I32BitHasher"/> instance to be used.</param>
        /// <param name="value">The value to be hashed.</param>
        /// <returns>The hash for <paramref name="value"/>.</returns>
        public static System.UInt32 Hash(this I32BitHasher hasher, System.Int16 value) => hasher.Hash(ref Unsafe.As<System.Int16, System.Byte>(ref value), sizeof(System.Int16));

        /// <summary>Hashes the specified <see cref="System.Int32"/>.</summary>
        /// <param name="hasher">The <see cref="I32BitHasher"/> instance to be used.</param>
        /// <param name="value">The value to be hashed.</param>
        /// <returns>The hash for <paramref name="value"/>.</returns>
        public static System.UInt32 Hash(this I32BitHasher hasher, System.Int32 value) => hasher.Hash(ref Unsafe.As<System.Int32, System.Byte>(ref value), sizeof(System.Int32));

        /// <summary>Hashes the specified <see cref="System.Int64"/>.</summary>
        /// <param name="hasher">The <see cref="I32BitHasher"/> instance to be used.</param>
        /// <param name="value">The value to be hashed.</param>
        /// <returns>The hash for <paramref name="value"/>.</returns>
        public static System.UInt32 Hash(this I32BitHasher hasher, System.Int64 value) => hasher.Hash(ref Unsafe.As<System.Int64, System.Byte>(ref value), sizeof(System.Int64));

        /// <summary>Hashes the specified <see cref="System.Int128"/>.</summary>
        /// <param name="hasher">The <see cref="I32BitHasher"/> instance to be used.</param>
        /// <param name="value">The value to be hashed.</param>
        /// <returns>The hash for <paramref name="value"/>.</returns>
        public static unsafe System.UInt32 Hash(this I32BitHasher hasher, System.Int128 value) => hasher.Hash(ref Unsafe.As<System.Int128, System.Byte>(ref value), sizeof(System.Int128));

        /// <summary>Hashes the specified <see cref="System.UInt16"/>.</summary>
        /// <param name="hasher">The <see cref="I32BitHasher"/> instance to be used.</param>
        /// <param name="value">The value to be hashed.</param>
        /// <returns>The hash for <paramref name="value"/>.</returns>
        public static System.UInt32 Hash(this I32BitHasher hasher, System.UInt16 value) => hasher.Hash(ref Unsafe.As<System.UInt16, System.Byte>(ref value), sizeof(System.UInt16));

        /// <summary>Hashes the specified <see cref="System.UInt32"/>.</summary>
        /// <param name="hasher">The <see cref="I32BitHasher"/> instance to be used.</param>
        /// <param name="value">The value to be hashed.</param>
        /// <returns>The hash for <paramref name="value"/>.</returns>
        public static System.UInt32 Hash(this I32BitHasher hasher, System.UInt32 value) => hasher.Hash(ref Unsafe.As<System.UInt32, System.Byte>(ref value), sizeof(System.UInt32));

        /// <summary>Hashes the specified <see cref="System.UInt64"/>.</summary>
        /// <param name="hasher">The <see cref="I32BitHasher"/> instance to be used.</param>
        /// <param name="value">The value to be hashed.</param>
        /// <returns>The hash for <paramref name="value"/>.</returns>
        public static System.UInt32 Hash(this I32BitHasher hasher, System.UInt64 value) => hasher.Hash(ref Unsafe.As<System.UInt64, System.Byte>(ref value), sizeof(System.UInt64));

        /// <summary>Hashes the specified <see cref="System.UInt128"/>.</summary>
        /// <param name="hasher">The <see cref="I32BitHasher"/> instance to be used.</param>
        /// <param name="value">The value to be hashed.</param>
        /// <returns>The hash for <paramref name="value"/>.</returns>
        public static unsafe System.UInt32 Hash(this I32BitHasher hasher, System.UInt128 value) => hasher.Hash(ref Unsafe.As<System.UInt128, System.Byte>(ref value), sizeof(System.UInt128));

        /// <summary>Hashes the specified string and returns the result.</summary>
        /// <param name="hasher">The <see cref="I32BitHasher"/> instance to be used.</param>
        /// <param name="value">The string to be hashed. Note that strings above 1073741823 characters will cause overflow and fail.</param>
        /// <returns>The hash for <paramref name="value"/>.</returns>
        [Throws(typeof(ArgumentNullException))]
        public static System.UInt32 HashString(this I32BitHasher hasher, string value)
        {
            ArgumentNullException.ThrowIfNull(value);
            return hasher.Hash(ref Unsafe.As<System.Char, System.Byte>(ref Unsafe.AsRef(value.GetPinnableReference())), value.Length * sizeof(System.Char));
        }

        /// <summary>
        /// Updates the specified <see cref="I32BitStreamingHasher"/> instance by the specified string.
        /// </summary>
        /// <param name="hasher">The <see cref="I32BitStreamingHasher"/> instance to be used.</param>
        /// <param name="value">The string value to update the streaming hasher with.</param>
        [Throws(typeof(ArgumentNullException))]
        public static void UpdateByString(this I32BitStreamingHasher hasher, string value)
        {
            ArgumentNullException.ThrowIfNull(value);
            ref System.Byte string_ref = ref Unsafe.As<System.Char, System.Byte>(ref Unsafe.AsRef(value.GetPinnableReference()));
            if (value.Length > 1073741823) {
                hasher.Update(ref string_ref, 1073741823 * sizeof(System.Char));
                hasher.Update(ref Unsafe.AddByteOffset(ref string_ref, 1073741823 * sizeof(System.Char)), (value.Length - 1073741823) * sizeof(System.Char));
            } else {
                hasher.Update(ref string_ref, value.Length * sizeof(System.Char));
            }
        }

        /// <summary>
        /// Hashes the specified buffer and returns the hash of the selected <paramref name="buffer"/> portion.
        /// </summary>
        /// <param name="hasher">The <see cref="I32BitStreamingHasher"/> instance to be used.</param>
        /// <param name="buffer">The buffer that contains the data to compute the hash from.</param>
        /// <param name="offset">A position in <paramref name="buffer"/> to starting hashing data from.</param>
        /// <param name="count">The number of bytes in <paramref name="buffer"/> to be hashed.</param>
        /// <returns>The computed hash value for <paramref name="buffer"/>.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="buffer"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentException"><paramref name="count"/> + <paramref name="offset"/> are outside of <paramref name="buffer"/> bounds.</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="count"/> and/or <paramref name="offset"/> are negative values.</exception>
        public static System.UInt32 HashBuffer(this I32BitHasher hasher, byte[] buffer, int offset, int count)
        {
            ArgumentNullException.ThrowIfNull(buffer);
            if (offset < 0) { 
                throw new ArgumentOutOfRangeException(nameof(offset));
            } else if (count < 0) {
                throw new ArgumentOutOfRangeException(nameof(count));
            } else if (count + offset >= buffer.Length) {
                throw new ArgumentException("count + offset cannot exceed the buffer's bounds.", nameof(count));
            } else {
                return hasher.Hash(ref buffer[offset], count);
            }
        }

        public static void UpdateBySpan<T>(this I32BitStreamingHasher hasher, Span<T> span)
            where T : unmanaged // <- This will only work well on unmanaged types.
        {
            if (span.IsEmpty) { return; }
            ref System.Byte reference = ref Unsafe.As<T, System.Byte>(ref span[0]);
            long buf_length = span.Length * Unsafe.SizeOf<T>().ToInt64();
            while (buf_length > System.Int32.MaxValue)
            {
                hasher.Update(ref reference, System.Int32.MaxValue);
                buf_length -= System.Int32.MaxValue;
                reference = ref Unsafe.AddByteOffset(ref reference, System.Int32.MaxValue);
            }
            hasher.Update(ref reference, buf_length.ToInt32());
        }
    }
}
