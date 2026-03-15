
using System;
using MP.Annotations.CodeAnalysis;

namespace MP.Collections.Hashing
{
    /// <summary>
    /// Defines the base interface for hashers that return 32-bit values.
    /// </summary>
    public interface I32BitHasher
    {
        /// <summary>
        /// Hashes the specified memory pointer of the specified size.
        /// </summary>
        /// <param name="input">The memory pointer to hash.</param>
        /// <param name="length">The length of <paramref name="input"/>.</param>
        /// <returns>The hash for <paramref name="input"/>.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="input"/> is a <see langword="null"/> reference.</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="length"/> is a negative value.</exception>
        [Throws(typeof(ArgumentNullException), typeof(ArgumentOutOfRangeException))]
        System.UInt32 Hash(ref System.Byte input, int length);
    }
}