
using System;
using MP.Annotations.CodeAnalysis;

namespace MP.Collections.Hashing
{
    /// <summary>
    /// Defines the basic methods that a 32-bit streaming hasher should be comprised of.
    /// </summary>
    public interface I32BitStreamingHasher
    {
        /// <summary>
        /// Updates the hash by appending the specified memory pointer.
        /// </summary>
        /// <param name="input">The memory pointer to hash.</param>
        /// <param name="length">The length of <paramref name="input"/>.</param>
        /// <exception cref="ArgumentNullException"><paramref name="input"/> is a <see langword="null"/> reference.</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="length"/> is a negative value.</exception>
        [Throws(typeof(ArgumentNullException), typeof(ArgumentOutOfRangeException))] 
        void Update(ref System.Byte input, int length);

        /// <summary>
        /// Computes the hash from the accumulated data so far and returns it.
        /// </summary>
        /// <returns>The combined hash of all <see cref="Update(ref byte, int)"/> calls.</returns>
        System.UInt32 Digest();
    }
}
