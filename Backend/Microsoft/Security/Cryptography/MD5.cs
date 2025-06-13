// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.IO;
using System.Diagnostics;
using System.Runtime.Versioning;

namespace Microsoft.Security.Cryptography
{
    public abstract class MD5 : HashAlgorithm
    {
        /// <summary>
        /// The hash size produced by the MD5 algorithm, in bits.
        /// </summary>
        public const int HashSizeInBits = 128;

        /// <summary>
        /// The hash size produced by the MD5 algorithm, in bytes.
        /// </summary>
        public const int HashSizeInBytes = HashSizeInBits / 8;

        protected MD5()
        {
            HashSizeValue = HashSizeInBits;
        }

        [UnsupportedOSPlatform("browser")]
        public static MD5 Create() => new Implementation();

        /// <summary>
        /// Computes the hash of data using the MD5 algorithm.
        /// </summary>
        /// <param name="source">The data to hash.</param>
        /// <returns>The hash of the data.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="source" /> is <see langword="null" />.
        /// </exception>
        [UnsupportedOSPlatform("browser")]
        public static byte[] HashData(byte[] source)
        {
            ArgumentNullException.ThrowIfNull(source);

            return HashData(new ReadOnlySpan<byte>(source));
        }

        /// <summary>
        /// Computes the hash of data using the MD5 algorithm.
        /// </summary>
        /// <param name="source">The data to hash.</param>
        /// <returns>The hash of the data.</returns>
        [UnsupportedOSPlatform("browser")]
        public static byte[] HashData(ReadOnlySpan<byte> source)
        {
            byte[] buffer = GC.AllocateUninitializedArray<byte>(HashSizeInBytes);

            int written = HashData(source, buffer.AsSpan());
            Debug.Assert(written == buffer.Length);

            return buffer;
        }

        /// <summary>
        /// Computes the hash of data using the MD5 algorithm.
        /// </summary>
        /// <param name="source">The data to hash.</param>
        /// <param name="destination">The buffer to receive the hash value.</param>
        /// <returns>The total number of bytes written to <paramref name="destination" />.</returns>
        /// <exception cref="ArgumentException">
        /// The buffer in <paramref name="destination"/> is too small to hold the calculated hash
        /// size. The MD5 algorithm always produces a 128-bit hash, or 16 bytes.
        /// </exception>
        [UnsupportedOSPlatform("browser")]
        public static int HashData(ReadOnlySpan<byte> source, Span<byte> destination)
        {
            if (!TryHashData(source, destination, out int bytesWritten))
                throw new ArgumentException(SR.Argument_DestinationTooShort, nameof(destination));

            return bytesWritten;
        }

        /// <summary>
        /// Attempts to compute the hash of data using the MD5 algorithm.
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
        [UnsupportedOSPlatform("browser")]
        public static bool TryHashData(ReadOnlySpan<byte> source, Span<byte> destination, out int bytesWritten)
        {
            if (destination.Length < HashSizeInBytes)
            {
                bytesWritten = 0;
                return false;
            }

            bytesWritten = BCryptHashProvider.HashDataAndReturnDirectly(HashAlgorithmNames.MD5, source, destination);
            Debug.Assert(bytesWritten == HashSizeInBytes);

            return true;
        }

        /// <summary>
        /// Computes the hash of a stream using the MD5 algorithm.
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
        ///   size. The MD5 algorithm always produces a 128-bit hash, or 16 bytes.
        ///   </p>
        ///   <p>-or-</p>
        ///   <p>
        ///   <paramref name="source" /> does not support reading.
        ///   </p>
        /// </exception>
        [UnsupportedOSPlatform("browser")]
        public static int HashData(Stream source, Span<byte> destination)
        {
            ArgumentNullException.ThrowIfNull(source);

            if (destination.Length < HashSizeInBytes)
                throw new ArgumentException(SR.Argument_DestinationTooShort, nameof(destination));

            if (!source.CanRead)
                throw new ArgumentException(SR.Argument_StreamNotReadable, nameof(source));

            return BCryptHashProvider.HashDataStreamAndReturnDirectly(HashAlgorithmNames.MD5, source, destination);
        }

        /// <summary>
        /// Computes the hash of a stream using the MD5 algorithm.
        /// </summary>
        /// <param name="source">The stream to hash.</param>
        /// <returns>The hash of the data.</returns>
        /// <exception cref="ArgumentNullException">
        ///   <paramref name="source" /> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentException">
        ///   <paramref name="source" /> does not support reading.
        /// </exception>
        [UnsupportedOSPlatform("browser")]
        public static byte[] HashData(Stream source)
        {
            ArgumentNullException.ThrowIfNull(source);

            if (!source.CanRead)
                throw new ArgumentException(SR.Argument_StreamNotReadable, nameof(source));

            System.Byte[] buffertemp = new System.Byte[HashSizeInBytes];

            BCryptHashProvider.HashDataStreamAndReturnDirectly(HashAlgorithmNames.MD5 , source , buffertemp);

            return buffertemp;
        }

        private sealed class Implementation : MD5
        {
            private readonly BCryptHashProvider _hashProvider;

            public Implementation()
            {
                _hashProvider = new BCryptHashProvider(HashAlgorithmNames.MD5);
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
