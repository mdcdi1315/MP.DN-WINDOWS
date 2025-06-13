// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.IO;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace Microsoft.Security.Cryptography
{
    public abstract class SHA1 : HashAlgorithm
    {
        /// <summary>
        /// The hash size produced by the SHA1 algorithm, in bits.
        /// </summary>
        public const int HashSizeInBits = 160;

        /// <summary>
        /// The hash size produced by the SHA1 algorithm, in bytes.
        /// </summary>
        public const int HashSizeInBytes = HashSizeInBits / 8;

        protected SHA1()
        {
            HashSizeValue = HashSizeInBits;
        }

        [SuppressMessage("Microsoft.Security", "CA5350", Justification = "This is the implementation of SHA1")]
        public static SHA1 Create() => new Implementation();

        /// <summary>
        /// Computes the hash of data using the SHA1 algorithm.
        /// </summary>
        /// <param name="source">The data to hash.</param>
        /// <returns>The hash of the data.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="source" /> is <see langword="null" />.
        /// </exception>
        public static byte[] HashData(byte[] source)
        {
            ArgumentNullException.ThrowIfNull(source);

            return HashData(new ReadOnlySpan<byte>(source));
        }

        /// <summary>
        /// Computes the hash of data using the SHA1 algorithm.
        /// </summary>
        /// <param name="source">The data to hash.</param>
        /// <returns>The hash of the data.</returns>
        public static byte[] HashData(ReadOnlySpan<byte> source)
        {
            byte[] buffer = GC.AllocateUninitializedArray<byte>(HashSizeInBytes);

            int written = HashData(source, buffer.AsSpan());
            Debug.Assert(written == buffer.Length);

            return buffer;
        }

        /// <summary>
        /// Computes the hash of data using the SHA1 algorithm.
        /// </summary>
        /// <param name="source">The data to hash.</param>
        /// <param name="destination">The buffer to receive the hash value.</param>
        /// <returns>The total number of bytes written to <paramref name="destination" />.</returns>
        /// <exception cref="ArgumentException">
        /// The buffer in <paramref name="destination"/> is too small to hold the calculated hash
        /// size. The SHA1 algorithm always produces a 160-bit hash, or 20 bytes.
        /// </exception>
        public static int HashData(ReadOnlySpan<byte> source, Span<byte> destination)
        {
            if (!TryHashData(source, destination, out int bytesWritten))
                throw new ArgumentException(SR.Argument_DestinationTooShort, nameof(destination));

            return bytesWritten;
        }

        /// <summary>
        /// Attempts to compute the hash of data using the SHA1 algorithm.
        /// </summary>
        /// <param name="source">The data to hash.</param>
        /// <param name="destination">The buffer to receive the hash value.</param>
        /// <param name="bytesWritten">
        /// When this method returns, the total number of bytes written into <paramref name="destination"/>.
        /// </param>
        /// <returns>
        /// <see langword="false"/> if <paramref name="destination"/> is too small to hold the
        /// calculated hash, <see langword="true"/> otherwise.
        /// </returns>
        public static bool TryHashData(ReadOnlySpan<byte> source, Span<byte> destination, out int bytesWritten)
        {
            if (destination.Length < HashSizeInBytes)
            {
                bytesWritten = 0;
                return false;
            }

            bytesWritten = BCryptHashProvider.HashDataAndReturnDirectly(HashAlgorithmNames.SHA1, source, destination);
            Debug.Assert(bytesWritten == HashSizeInBytes);

            return true;
        }

        /// <summary>
        /// Computes the hash of a stream using the SHA1 algorithm.
        /// </summary>
        /// <param name="source">The stream to hash.</param>
        /// <param name="destination">The buffer to receive the hash value.</param>
        /// <returns>The total number of bytes written to <paramref name="destination" />.</returns>
        /// <exception cref="ArgumentNullException">
        ///   <paramref name="source" /> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentException">
        ///   <p>
        ///   The buffer in <paramref name="destination"/> is too small to hold the calculated hash
        ///   size. The SHA1 algorithm always produces a 160-bit hash, or 20 bytes.
        ///   </p>
        ///   <p>-or-</p>
        ///   <p>
        ///   <paramref name="source" /> does not support reading.
        ///   </p>
        /// </exception>
        public static int HashData(Stream source, Span<byte> destination)
        {
            ArgumentNullException.ThrowIfNull(source);

            if (destination.Length < HashSizeInBytes)
                throw new ArgumentException(SR.Argument_DestinationTooShort, nameof(destination));

            if (!source.CanRead)
                throw new ArgumentException(SR.Argument_StreamNotReadable, nameof(source));

            return BCryptHashProvider.HashDataStreamAndReturnDirectly(HashAlgorithmNames.SHA1, source, destination);
        }

        /// <summary>
        /// Computes the hash of a stream using the SHA1 algorithm.
        /// </summary>
        /// <param name="source">The stream to hash.</param>
        /// <returns>The hash of the data.</returns>
        /// <exception cref="ArgumentNullException">
        ///   <paramref name="source" /> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentException">
        ///   <paramref name="source" /> does not support reading.
        /// </exception>
        public static byte[] HashData(Stream source)
        {
            ArgumentNullException.ThrowIfNull(source);

            if (!source.CanRead)
                throw new ArgumentException(SR.Argument_StreamNotReadable, nameof(source));

            System.Byte[] buffertemp = new System.Byte[HashSizeInBytes];

            BCryptHashProvider.HashDataStreamAndReturnDirectly(HashAlgorithmNames.SHA1, source, buffertemp);

            return buffertemp;
        }

        private sealed class Implementation : SHA1
        {
            private readonly HashProvider _hashProvider;

            public Implementation()
            {
                _hashProvider = new BCryptHashProvider(HashAlgorithmNames.SHA1);
                HashSizeValue = _hashProvider.HashSizeInBytes * 8;
            }

            protected sealed override void HashCore(byte[] array, int ibStart, int cbSize) =>
                _hashProvider.AppendHashData(array, ibStart, cbSize);

            protected sealed override void HashCore(ReadOnlySpan<byte> source) =>
                _hashProvider.AppendHashData(source);

            protected sealed override byte[] HashFinal() =>
                _hashProvider.FinalizeHashAndReset();

            protected sealed override bool TryHashFinal(Span<byte> destination, out int bytesWritten) =>
                _hashProvider.TryFinalizeHashAndReset(destination, out bytesWritten);

            public sealed override void Initialize() => _hashProvider.Reset();

            protected sealed override void Dispose(bool disposing)
            {
                _hashProvider.Dispose(disposing);
                base.Dispose(disposing);
            }
        }
    }
}
