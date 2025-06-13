/*
 * Copyright © 2000-2018 SharpZipLib Contributors

Permission is hereby granted, free of charge, to any person obtaining a copy of this
software and associated documentation files (the "Software"), to deal in the Software
without restriction, including without limitation the rights to use, copy, modify, merge,
publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
to whom the Software is furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all copies or
substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
DEALINGS IN THE SOFTWARE.

*/

#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP3_1_OR_GREATER
    #define VECTORIZE_MEMORY_MOVE
#endif

using System;
using System.Runtime.CompilerServices;


namespace System.IO.ManagedZip.Checksum
{
    /// <summary>
    /// Computes Adler32 checksum for a stream of data. An Adler32
    /// checksum is not as reliable as a CRC32 checksum, but a lot faster to
    /// compute.
    ///
    /// The specification for Adler32 may be found in RFC 1950.
    /// ZLIB Compressed Data Format Specification version 3.3)
    ///
    ///
    /// From that document:
    ///
    ///      "ADLER32 (Adler-32 checksum)
    ///       This contains a checksum value of the uncompressed data
    ///       (excluding any dictionary data) computed according to Adler-32
    ///       algorithm. This algorithm is a 32-bit extension and improvement
    ///       of the Fletcher algorithm, used in the ITU-T X.224 / ISO 8073
    ///       standard.
    ///
    ///       Adler-32 is composed of two sums accumulated per byte: s1 is
    ///       the sum of all bytes, s2 is the sum of all s1 values. Both sums
    ///       are done modulo 65521. s1 is initialized to 1, s2 to zero.  The
    ///       Adler-32 checksum is stored as s2*65536 + s1 in most-
    ///       significant-byte first (network) order."
    ///
    ///  "8.2. The Adler-32 algorithm
    ///
    ///    The Adler-32 algorithm is much faster than the CRC32 algorithm yet
    ///    still provides an extremely low probability of undetected errors.
    ///
    ///    The modulo on unsigned long accumulators can be delayed for 5552
    ///    bytes, so the modulo operation time is negligible.  If the bytes
    ///    are a, b, c, the second sum is 3a + 2b + c + 3, and so is position
    ///    and order sensitive, unlike the first sum, which is just a
    ///    checksum.  That 65521 is prime is important to avoid a possible
    ///    large class of two-byte errors that leave the check unchanged.
    ///    (The Fletcher checksum uses 255, which is not prime and which also
    ///    makes the Fletcher check insensitive to single byte changes 0 -
    ///    255.)
    ///
    ///    The sum s1 is initialized to 1 instead of zero to make the length
    ///    of the sequence part of s2, so that the length does not have to be
    ///    checked separately. (Any sequence of zeroes has a Fletcher
    ///    checksum of zero.)"
    /// </summary>
    /// <see cref="System.IO.ManagedZip.Zip.Compression.Streams.InflaterInputStream"/>
    /// <see cref="System.IO.ManagedZip.Zip.Compression.Streams.DeflaterOutputStream"/>
    public sealed class Adler32 : IChecksum
    {
        #region Instance Fields

        /// <summary>
        /// largest prime smaller than 65536
        /// </summary>
        private static readonly uint BASE = 65521;

        /// <summary>
        /// The CRC data checksum so far.
        /// </summary>
        private uint checkValue;

        #endregion Instance Fields

        /// <summary>
        /// Initialise a default instance of <see cref="Adler32"></see>
        /// </summary>
        public Adler32()
        {
            Reset();
        }

        /// <summary>
        /// Resets the Adler32 data checksum as if no update was ever called.
        /// </summary>
        public void Reset()
        {
            checkValue = 1;
        }

        /// <summary>
        /// Returns the Adler32 data checksum computed so far.
        /// </summary>
        public long Value
        {
            get
            {
                return checkValue;
            }
        }

        /// <summary>
        /// Updates the checksum with the byte b.
        /// </summary>
        /// <param name="bval">
        /// The data value to add. The high byte of the int is ignored.
        /// </param>
        public void Update(int bval)
        {
            // We could make a length 1 byte array and call update again, but I
            // would rather not have that overhead
            uint s1 = checkValue & 0xFFFF;
            uint s2 = checkValue >> 16;

            s1 = (s1 + ((uint)bval & 0xFF)) % BASE;
            s2 = (s1 + s2) % BASE;

            checkValue = (s2 << 16) + s1;
        }

        /// <summary>
        /// Updates the Adler32 data checksum with the bytes taken from
        /// a block of data.
        /// </summary>
        /// <param name="buffer">Contains the data to update the checksum with.</param>
        public void Update(byte[] buffer)
        {
            if (buffer == null)
            {
                throw new ArgumentNullException(nameof(buffer));
            }

            Update(new ArraySegment<byte>(buffer, 0, buffer.Length));
        }

        /// <summary>
        /// Update Adler32 data checksum based on a portion of a block of data
        /// </summary>
        /// <param name = "segment">
        /// The chunk of data to add
        /// </param>
        public void Update(ArraySegment<byte> segment)
        {
            //(By Per Bothner)
            uint s1 = checkValue & 0xFFFF;
            uint s2 = checkValue >> 16;
            var count = segment.Count;
            var offset = segment.Offset;
            while (count > 0)
            {
                // We can defer the modulo operation:
                // s1 maximally grows from 65521 to 65521 + 255 * 3800
                // s2 maximally grows by 3800 * median(s1) = 2090079800 < 2^31
                int n = 3800;
                if (n > count)
                {
                    n = count;
                }
                count -= n;
                while (--n >= 0)
                {
                    s1 = s1 + (uint)(segment.Array[offset++] & 0xff);
                    s2 = s2 + s1;
                }
                s1 %= BASE;
                s2 %= BASE;
            }
            checkValue = (s2 << 16) | s1;
        }
    }

    /// <summary>
    /// CRC-32 with unreversed data and reversed output
    /// </summary>
    /// <remarks>
    /// Generate a table for a byte-wise 32-bit CRC calculation on the polynomial:
    /// x^32+x^26+x^23+x^22+x^16+x^12+x^11+x^10+x^8+x^7+x^5+x^4+x^2+x^1+x^0.
    ///
    /// Polynomials over GF(2) are represented in binary, one bit per coefficient,
    /// with the lowest powers in the most significant bit.  Then adding polynomials
    /// is just exclusive-or, and multiplying a polynomial by x is a right shift by
    /// one.  If we call the above polynomial p, and represent a byte as the
    /// polynomial q, also with the lowest power in the most significant bit (so the
    /// byte 0xb1 is the polynomial x^7+x^3+x+1), then the CRC is (q*x^32) mod p,
    /// where a mod b means the remainder after dividing a by b.
    ///
    /// This calculation is done using the shift-register method of multiplying and
    /// taking the remainder.  The register is initialized to zero, and for each
    /// incoming bit, x^32 is added mod p to the register if the bit is a one (where
    /// x^32 mod p is p+x^32 = x^26+...+1), and the register is multiplied mod p by
    /// x (which is shifting right by one and adding x^32 mod p if the bit shifted
    /// out is a one).  We start with the highest power (least significant bit) of
    /// q and repeat for all eight bits of q.
    ///
    /// This implementation uses sixteen lookup tables stored in one linear array
    /// to implement the slicing-by-16 algorithm, a variant of the slicing-by-8
    /// algorithm described in this Intel white paper:
    ///
    /// https://web.archive.org/web/20120722193753/http://download.intel.com/technology/comms/perfnet/download/slicing-by-8.pdf
    ///
    /// The first lookup table is simply the CRC of all possible eight bit values.
    /// Each successive lookup table is derived from the original table generated
    /// by Sarwate's algorithm. Slicing a 16-bit input and XORing the outputs
    /// together will produce the same output as a byte-by-byte CRC loop with
    /// fewer arithmetic and bit manipulation operations, at the cost of increased
    /// memory consumed by the lookup tables. (Slicing-by-16 requires a 16KB table,
    /// which is still small enough to fit in most processors' L1 cache.)
    /// </remarks>
    public sealed class BZip2Crc : IChecksum
    {
        #region Instance Fields

        private const uint crcInit = 0xFFFFFFFF;
        //const uint crcXor = 0x00000000;

        private static readonly uint[] crcTable = CrcUtilities.GenerateSlicingLookupTable(0x04C11DB7, isReversed: false);

        /// <summary>
        /// The CRC data checksum so far.
        /// </summary>
        private uint checkValue;

        #endregion Instance Fields

        /// <summary>
        /// Initialise a default instance of <see cref="BZip2Crc"></see>
        /// </summary>
        public BZip2Crc()
        {
            Reset();
        }

        /// <summary>
        /// Resets the CRC data checksum as if no update was ever called.
        /// </summary>
        public void Reset()
        {
            checkValue = crcInit;
        }

        /// <summary>
        /// Returns the CRC data checksum computed so far.
        /// </summary>
        /// <remarks>Reversed Out = true</remarks>
        public long Value
        {
            get
            {
                // Technically, the output should be:
                //return (long)(~checkValue ^ crcXor);
                // but x ^ 0 = x, so there is no point in adding
                // the XOR operation
                return (long)(~checkValue);
            }
        }

        /// <summary>
        /// Updates the checksum with the int bval.
        /// </summary>
        /// <param name = "bval">
        /// the byte is taken as the lower 8 bits of bval
        /// </param>
        /// <remarks>Reversed Data = false</remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Update(int bval)
        {
            checkValue = unchecked(crcTable[(byte)(((checkValue >> 24) & 0xFF) ^ bval)] ^ (checkValue << 8));
        }

        /// <summary>
        /// Updates the CRC data checksum with the bytes taken from
        /// a block of data.
        /// </summary>
        /// <param name="buffer">Contains the data to update the CRC with.</param>
        public void Update(byte[] buffer)
        {
            if (buffer == null)
            {
                throw new ArgumentNullException(nameof(buffer));
            }

            Update(buffer, 0, buffer.Length);
        }

        /// <summary>
        /// Update CRC data checksum based on a portion of a block of data
        /// </summary>
        /// <param name = "segment">
        /// The chunk of data to add
        /// </param>
        public void Update(ArraySegment<byte> segment)
        {
            Update(segment.Array, segment.Offset, segment.Count);
        }

        /// <summary>
        /// Internal helper function for updating a block of data using slicing.
        /// </summary>
        /// <param name="data">The array containing the data to add</param>
        /// <param name="offset">Range start for <paramref name="data"/> (inclusive)</param>
        /// <param name="count">The number of bytes to checksum starting from <paramref name="offset"/></param>
        private void Update(byte[] data, int offset, int count)
        {
            int remainder = count % CrcUtilities.SlicingDegree;
            int end = offset + count - remainder;

            while (offset != end)
            {
                checkValue = CrcUtilities.UpdateDataForNormalPoly(data, offset, crcTable, checkValue);
                offset += CrcUtilities.SlicingDegree;
            }

            if (remainder != 0)
            {
                SlowUpdateLoop(data, offset, end + remainder);
            }
        }

        /// <summary>
        /// A non-inlined function for updating data that doesn't fit in a 16-byte
        /// block. We don't expect to enter this function most of the time, and when
        /// we do we're not here for long, so disabling inlining here improves
        /// performance overall.
        /// </summary>
        /// <param name="data">The array containing the data to add</param>
        /// <param name="offset">Range start for <paramref name="data"/> (inclusive)</param>
        /// <param name="end">Range end for <paramref name="data"/> (exclusive)</param>
        [MethodImpl(MethodImplOptions.NoInlining)]
        private void SlowUpdateLoop(byte[] data, int offset, int end)
        {
            while (offset != end)
            {
                Update(data[offset++]);
            }
        }
    }

    /// <summary>
    /// CRC-32 with reversed data and unreversed output
    /// </summary>
    /// <remarks>
    /// Generate a table for a byte-wise 32-bit CRC calculation on the polynomial:
    /// x^32+x^26+x^23+x^22+x^16+x^12+x^11+x^10+x^8+x^7+x^5+x^4+x^2+x^1+x^0.
    ///
    /// Polynomials over GF(2) are represented in binary, one bit per coefficient,
    /// with the lowest powers in the most significant bit.  Then adding polynomials
    /// is just exclusive-or, and multiplying a polynomial by x is a right shift by
    /// one.  If we call the above polynomial p, and represent a byte as the
    /// polynomial q, also with the lowest power in the most significant bit (so the
    /// byte 0xb1 is the polynomial x^7+x^3+x+1), then the CRC is (q*x^32) mod p,
    /// where a mod b means the remainder after dividing a by b.
    ///
    /// This calculation is done using the shift-register method of multiplying and
    /// taking the remainder.  The register is initialized to zero, and for each
    /// incoming bit, x^32 is added mod p to the register if the bit is a one (where
    /// x^32 mod p is p+x^32 = x^26+...+1), and the register is multiplied mod p by
    /// x (which is shifting right by one and adding x^32 mod p if the bit shifted
    /// out is a one).  We start with the highest power (least significant bit) of
    /// q and repeat for all eight bits of q.
    ///
    /// This implementation uses sixteen lookup tables stored in one linear array
    /// to implement the slicing-by-16 algorithm, a variant of the slicing-by-8
    /// algorithm described in this Intel white paper:
    ///
    /// https://web.archive.org/web/20120722193753/http://download.intel.com/technology/comms/perfnet/download/slicing-by-8.pdf
    ///
    /// The first lookup table is simply the CRC of all possible eight bit values.
    /// Each successive lookup table is derived from the original table generated
    /// by Sarwate's algorithm. Slicing a 16-bit input and XORing the outputs
    /// together will produce the same output as a byte-by-byte CRC loop with
    /// fewer arithmetic and bit manipulation operations, at the cost of increased
    /// memory consumed by the lookup tables. (Slicing-by-16 requires a 16KB table,
    /// which is still small enough to fit in most processors' L1 cache.)
    /// </remarks>
    public sealed class Crc32 : IChecksum
    {
        #region Instance Fields

        private static readonly uint crcInit = 0xFFFFFFFF;
        private static readonly uint crcXor = 0xFFFFFFFF;

        private static readonly uint[] crcTable = CrcUtilities.GenerateSlicingLookupTable(0xEDB88320, isReversed: true);

        /// <summary>
        /// The CRC data checksum so far.
        /// </summary>
        private uint checkValue;

        #endregion Instance Fields

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static uint ComputeCrc32(uint oldCrc, byte bval)
        {
            return (uint)(Crc32.crcTable[(oldCrc ^ bval) & 0xFF] ^ (oldCrc >> 8));
        }

        /// <summary>
        /// Initialise a default instance of <see cref="Crc32"></see>
        /// </summary>
        public Crc32()
        {
            Reset();
        }

        /// <summary>
        /// Resets the CRC data checksum as if no update was ever called.
        /// </summary>
        public void Reset()
        {
            checkValue = crcInit;
        }

        /// <summary>
        /// Returns the CRC data checksum computed so far.
        /// </summary>
        /// <remarks>Reversed Out = false</remarks>
        public long Value
        {
            get
            {
                return (long)(checkValue ^ crcXor);
            }
        }

        /// <summary>
        /// Updates the checksum with the int bval.
        /// </summary>
        /// <param name = "bval">
        /// the byte is taken as the lower 8 bits of bval
        /// </param>
        /// <remarks>Reversed Data = true</remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Update(int bval)
        {
            checkValue = unchecked(crcTable[(checkValue ^ bval) & 0xFF] ^ (checkValue >> 8));
        }

        /// <summary>
        /// Updates the CRC data checksum with the bytes taken from
        /// a block of data.
        /// </summary>
        /// <param name="buffer">Contains the data to update the CRC with.</param>
        public void Update(byte[] buffer)
        {
            if (buffer == null)
            {
                throw new ArgumentNullException(nameof(buffer));
            }

            Update(buffer, 0, buffer.Length);
        }

        /// <summary>
        /// Update CRC data checksum based on a portion of a block of data
        /// </summary>
        /// <param name = "segment">
        /// The chunk of data to add
        /// </param>
        public void Update(ArraySegment<byte> segment)
        {
            Update(segment.Array, segment.Offset, segment.Count);
        }

        /// <summary>
        /// Internal helper function for updating a block of data using slicing.
        /// </summary>
        /// <param name="data">The array containing the data to add</param>
        /// <param name="offset">Range start for <paramref name="data"/> (inclusive)</param>
        /// <param name="count">The number of bytes to checksum starting from <paramref name="offset"/></param>
        private void Update(byte[] data, int offset, int count)
        {
            int remainder = count % CrcUtilities.SlicingDegree;
            int end = offset + count - remainder;

            while (offset != end)
            {
                checkValue = CrcUtilities.UpdateDataForReversedPoly(data, offset, crcTable, checkValue);
                offset += CrcUtilities.SlicingDegree;
            }

            if (remainder != 0)
            {
                SlowUpdateLoop(data, offset, end + remainder);
            }
        }

        /// <summary>
        /// A non-inlined function for updating data that doesn't fit in a 16-byte
        /// block. We don't expect to enter this function most of the time, and when
        /// we do we're not here for long, so disabling inlining here improves
        /// performance overall.
        /// </summary>
        /// <param name="data">The array containing the data to add</param>
        /// <param name="offset">Range start for <paramref name="data"/> (inclusive)</param>
        /// <param name="end">Range end for <paramref name="data"/> (exclusive)</param>
        [MethodImpl(MethodImplOptions.NoInlining)]
        private void SlowUpdateLoop(byte[] data, int offset, int end)
        {
            while (offset != end)
            {
                Update(data[offset++]);
            }
        }
    }

    internal static class CrcUtilities
    {
        /// <summary>
        /// The number of slicing lookup tables to generate.
        /// </summary>
        internal const int SlicingDegree = 16;

        /// <summary>
        /// Generates multiple CRC lookup tables for a given polynomial, stored
        /// in a linear array of uints. The first block (i.e. the first 256
        /// elements) is the same as the byte-by-byte CRC lookup table. 
        /// </summary>
        /// <param name="polynomial">The generating CRC polynomial</param>
        /// <param name="isReversed">Whether the polynomial is in reversed bit order</param>
        /// <returns>A linear array of 256 * <see cref="SlicingDegree"/> elements</returns>
        /// <remarks>
        /// This table could also be generated as a rectangular array, but the
        /// JIT compiler generates slower code than if we use a linear array.
        /// Known issue, see: https://github.com/dotnet/runtime/issues/30275
        /// </remarks>
        internal static uint[] GenerateSlicingLookupTable(uint polynomial, bool isReversed)
        {
            var table = new uint[256 * SlicingDegree];
            uint one = isReversed ? 1 : (1U << 31);

            for (int i = 0; i < 256; i++)
            {
                uint res = (uint)(isReversed ? i : i << 24);
                for (int j = 0; j < SlicingDegree; j++)
                {
                    for (int k = 0; k < 8; k++)
                    {
                        if (isReversed)
                        {
                            res = (res & one) == 1 ? polynomial ^ (res >> 1) : res >> 1;
                        }
                        else
                        {
                            res = (res & one) != 0 ? polynomial ^ (res << 1) : res << 1;
                        }
                    }

                    table[(256 * j) + i] = res;
                }
            }

            return table;
        }

        /// <summary>
        /// Mixes the first four bytes of input with <paramref name="checkValue"/>
        /// using normal ordering before calling <see cref="UpdateDataCommon"/>.
        /// </summary>
        /// <param name="input">Array of data to checksum</param>
        /// <param name="offset">Offset to start reading <paramref name="input"/> from</param>
        /// <param name="crcTable">The table to use for slicing-by-16 lookup</param>
        /// <param name="checkValue">Checksum state before this update call</param>
        /// <returns>A new unfinalized checksum value</returns>
        /// <seealso cref="UpdateDataForReversedPoly"/>
        /// <remarks>
        /// Assumes input[offset]..input[offset + 15] are valid array indexes.
        /// For performance reasons, this must be checked by the caller.
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static uint UpdateDataForNormalPoly(byte[] input, int offset, uint[] crcTable, uint checkValue)
        {
            byte x1 = (byte)((byte)(checkValue >> 24) ^ input[offset]);
            byte x2 = (byte)((byte)(checkValue >> 16) ^ input[offset + 1]);
            byte x3 = (byte)((byte)(checkValue >> 8) ^ input[offset + 2]);
            byte x4 = (byte)((byte)checkValue ^ input[offset + 3]);

            return UpdateDataCommon(input, offset, crcTable, x1, x2, x3, x4);
        }

        /// <summary>
        /// Mixes the first four bytes of input with <paramref name="checkValue"/>
        /// using reflected ordering before calling <see cref="UpdateDataCommon"/>.
        /// </summary>
        /// <param name="input">Array of data to checksum</param>
        /// <param name="offset">Offset to start reading <paramref name="input"/> from</param>
        /// <param name="crcTable">The table to use for slicing-by-16 lookup</param>
        /// <param name="checkValue">Checksum state before this update call</param>
        /// <returns>A new unfinalized checksum value</returns>
        /// <seealso cref="UpdateDataForNormalPoly"/>
        /// <remarks>
        /// Assumes input[offset]..input[offset + 15] are valid array indexes.
        /// For performance reasons, this must be checked by the caller.
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static uint UpdateDataForReversedPoly(byte[] input, int offset, uint[] crcTable, uint checkValue)
        {
            byte x1 = (byte)((byte)checkValue ^ input[offset]);
            byte x2 = (byte)((byte)(checkValue >>= 8) ^ input[offset + 1]);
            byte x3 = (byte)((byte)(checkValue >>= 8) ^ input[offset + 2]);
            byte x4 = (byte)((byte)(checkValue >>= 8) ^ input[offset + 3]);

            return UpdateDataCommon(input, offset, crcTable, x1, x2, x3, x4);
        }

        /// <summary>
        /// A shared method for updating an unfinalized CRC checksum using slicing-by-16.
        /// </summary>
        /// <param name="input">Array of data to checksum</param>
        /// <param name="offset">Offset to start reading <paramref name="input"/> from</param>
        /// <param name="crcTable">The table to use for slicing-by-16 lookup</param>
        /// <param name="x1">First byte of input after mixing with the old CRC</param>
        /// <param name="x2">Second byte of input after mixing with the old CRC</param>
        /// <param name="x3">Third byte of input after mixing with the old CRC</param>
        /// <param name="x4">Fourth byte of input after mixing with the old CRC</param>
        /// <returns>A new unfinalized checksum value</returns>
        /// <remarks>
        /// <para>
        /// Even though the first four bytes of input are fed in as arguments,
        /// <paramref name="offset"/> should be the same value passed to this
        /// function's caller (either <see cref="UpdateDataForNormalPoly"/> or
        /// <see cref="UpdateDataForReversedPoly"/>). This method will get inlined
        /// into both functions, so using the same offset produces faster code.
        /// </para>
        /// <para>
        /// Because most processors running C# have some kind of instruction-level
        /// parallelism, the order of XOR operations can affect performance. This
        /// ordering assumes that the assembly code generated by the just-in-time
        /// compiler will emit a bunch of arithmetic operations for checking array
        /// bounds. Then it opportunistically XORs a1 and a2 to keep the processor
        /// busy while those other parts of the pipeline handle the range check
        /// calculations.
        /// </para>
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static uint UpdateDataCommon(byte[] input, int offset, uint[] crcTable, byte x1, byte x2, byte x3, byte x4)
        {
            uint result;
            uint a1 = crcTable[x1 + 3840] ^ crcTable[x2 + 3584];
            uint a2 = crcTable[x3 + 3328] ^ crcTable[x4 + 3072];

            result = crcTable[input[offset + 4] + 2816];
            result ^= crcTable[input[offset + 5] + 2560];
            a1 ^= crcTable[input[offset + 9] + 1536];
            result ^= crcTable[input[offset + 6] + 2304];
            result ^= crcTable[input[offset + 7] + 2048];
            result ^= crcTable[input[offset + 8] + 1792];
            a2 ^= crcTable[input[offset + 13] + 512];
            result ^= crcTable[input[offset + 10] + 1280];
            result ^= crcTable[input[offset + 11] + 1024];
            result ^= crcTable[input[offset + 12] + 768];
            result ^= a1;
            result ^= crcTable[input[offset + 14] + 256];
            result ^= crcTable[input[offset + 15]];
            result ^= a2;

            return result;
        }
    }

    /// <summary>
    /// Interface to compute a data checksum used by checked input/output streams.
    /// A data checksum can be updated by one byte or with a byte array. After each
    /// update the value of the current checksum can be returned by calling
    /// <c>getValue</c>. The complete checksum object can also be reset
    /// so it can be used again with new data.
    /// </summary>
    public interface IChecksum
    {
        /// <summary>
        /// Resets the data checksum as if no update was ever called.
        /// </summary>
        void Reset();

        /// <summary>
        /// Returns the data checksum computed so far.
        /// </summary>
        long Value { get; }

        /// <summary>
        /// Adds one byte to the data checksum.
        /// </summary>
        /// <param name = "bval">
        /// the data value to add. The high byte of the int is ignored.
        /// </param>
        void Update(int bval);

        /// <summary>
        /// Updates the data checksum with the bytes taken from the array.
        /// </summary>
        /// <param name="buffer">
        /// buffer an array of bytes
        /// </param>
        void Update(byte[] buffer);

        /// <summary>
        /// Adds the byte array to the data checksum.
        /// </summary>
        /// <param name = "segment">
        /// The chunk of data to add
        /// </param>
        void Update(ArraySegment<byte> segment);
    }
}
