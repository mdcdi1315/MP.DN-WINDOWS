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


namespace System.IO.ManagedZip.BZip2
{
    using System;
    using System.IO;
    using System.Runtime.Serialization;
    using System.IO.ManagedZip.Checksum;

    /// <summary>
    /// An example class to demonstrate compression and decompression of BZip2 streams.
    /// </summary>
    public static class BZip2
    {
        /// <summary>
        /// Decompress the <paramref name="inStream">input</paramref> writing
        /// uncompressed data to the <paramref name="outStream">output stream</paramref>
        /// </summary>
        /// <param name="inStream">The readable stream containing data to decompress.</param>
        /// <param name="outStream">The output stream to receive the decompressed data.</param>
        /// <param name="isStreamOwner">Both streams are closed on completion if true.</param>
        public static void Decompress(Stream inStream, Stream outStream, bool isStreamOwner)
        {
            if (inStream == null)
                throw new ArgumentNullException(nameof(inStream));

            if (outStream == null)
                throw new ArgumentNullException(nameof(outStream));

            try
            {
                using (BZip2InputStream bzipInput = new BZip2InputStream(inStream))
                {
                    bzipInput.IsStreamOwner = isStreamOwner;
                    Core.StreamUtils.Copy(bzipInput, outStream, new byte[4096]);
                }
            }
            finally
            {
                if (isStreamOwner)
                {
                    // inStream is closed by the BZip2InputStream if stream owner
                    outStream.Dispose();
                }
            }
        }

        /// <summary>
        /// Compress the <paramref name="inStream">input stream</paramref> sending
        /// result data to <paramref name="outStream">output stream</paramref>
        /// </summary>
        /// <param name="inStream">The readable stream to compress.</param>
        /// <param name="outStream">The output stream to receive the compressed data.</param>
        /// <param name="isStreamOwner">Both streams are closed on completion if true.</param>
        /// <param name="level">Block size acts as compression level (1 to 9) with 1 giving
        /// the lowest compression and 9 the highest.</param>
        public static void Compress(Stream inStream, Stream outStream, bool isStreamOwner, int level)
        {
            if (inStream == null)
                throw new ArgumentNullException(nameof(inStream));

            if (outStream == null)
                throw new ArgumentNullException(nameof(outStream));

            try
            {
                using (BZip2OutputStream bzipOutput = new BZip2OutputStream(outStream, level))
                {
                    bzipOutput.IsStreamOwner = isStreamOwner;
                    Core.StreamUtils.Copy(inStream, bzipOutput, new byte[4096]);
                }
            }
            finally
            {
                if (isStreamOwner)
                {
                    // outStream is closed by the BZip2OutputStream if stream owner
                    inStream.Dispose();
                }
            }
        }
    }

    /// <summary>
    /// Defines internal values for both compression and decompression
    /// </summary>
    internal static class BZip2Constants
    {
        /// <summary>
        /// Random numbers used to randomise repetitive blocks
        /// </summary>
        public readonly static int[] RandomNumbers = {
            619, 720, 127, 481, 931, 816, 813, 233, 566, 247,
            985, 724, 205, 454, 863, 491, 741, 242, 949, 214,
            733, 859, 335, 708, 621, 574,  73, 654, 730, 472,
            419, 436, 278, 496, 867, 210, 399, 680, 480,  51,
            878, 465, 811, 169, 869, 675, 611, 697, 867, 561,
            862, 687, 507, 283, 482, 129, 807, 591, 733, 623,
            150, 238,  59, 379, 684, 877, 625, 169, 643, 105,
            170, 607, 520, 932, 727, 476, 693, 425, 174, 647,
             73, 122, 335, 530, 442, 853, 695, 249, 445, 515,
            909, 545, 703, 919, 874, 474, 882, 500, 594, 612,
            641, 801, 220, 162, 819, 984, 589, 513, 495, 799,
            161, 604, 958, 533, 221, 400, 386, 867, 600, 782,
            382, 596, 414, 171, 516, 375, 682, 485, 911, 276,
             98, 553, 163, 354, 666, 933, 424, 341, 533, 870,
            227, 730, 475, 186, 263, 647, 537, 686, 600, 224,
            469,  68, 770, 919, 190, 373, 294, 822, 808, 206,
            184, 943, 795, 384, 383, 461, 404, 758, 839, 887,
            715,  67, 618, 276, 204, 918, 873, 777, 604, 560,
            951, 160, 578, 722,  79, 804,  96, 409, 713, 940,
            652, 934, 970, 447, 318, 353, 859, 672, 112, 785,
            645, 863, 803, 350, 139,  93, 354,  99, 820, 908,
            609, 772, 154, 274, 580, 184,  79, 626, 630, 742,
            653, 282, 762, 623, 680,  81, 927, 626, 789, 125,
            411, 521, 938, 300, 821,  78, 343, 175, 128, 250,
            170, 774, 972, 275, 999, 639, 495,  78, 352, 126,
            857, 956, 358, 619, 580, 124, 737, 594, 701, 612,
            669, 112, 134, 694, 363, 992, 809, 743, 168, 974,
            944, 375, 748,  52, 600, 747, 642, 182, 862,  81,
            344, 805, 988, 739, 511, 655, 814, 334, 249, 515,
            897, 955, 664, 981, 649, 113, 974, 459, 893, 228,
            433, 837, 553, 268, 926, 240, 102, 654, 459,  51,
            686, 754, 806, 760, 493, 403, 415, 394, 687, 700,
            946, 670, 656, 610, 738, 392, 760, 799, 887, 653,
            978, 321, 576, 617, 626, 502, 894, 679, 243, 440,
            680, 879, 194, 572, 640, 724, 926,  56, 204, 700,
            707, 151, 457, 449, 797, 195, 791, 558, 945, 679,
            297,  59,  87, 824, 713, 663, 412, 693, 342, 606,
            134, 108, 571, 364, 631, 212, 174, 643, 304, 329,
            343,  97, 430, 751, 497, 314, 983, 374, 822, 928,
            140, 206,  73, 263, 980, 736, 876, 478, 430, 305,
            170, 514, 364, 692, 829,  82, 855, 953, 676, 246,
            369, 970, 294, 750, 807, 827, 150, 790, 288, 923,
            804, 378, 215, 828, 592, 281, 565, 555, 710,  82,
            896, 831, 547, 261, 524, 462, 293, 465, 502,  56,
            661, 821, 976, 991, 658, 869, 905, 758, 745, 193,
            768, 550, 608, 933, 378, 286, 215, 979, 792, 961,
             61, 688, 793, 644, 986, 403, 106, 366, 905, 644,
            372, 567, 466, 434, 645, 210, 389, 550, 919, 135,
            780, 773, 635, 389, 707, 100, 626, 958, 165, 504,
            920, 176, 193, 713, 857, 265, 203,  50, 668, 108,
            645, 990, 626, 197, 510, 357, 358, 850, 858, 364,
            936, 638
        };

        /// <summary>
        /// When multiplied by compression parameter (1-9) gives the block size for compression
        /// 9 gives the best compression but uses the most memory.
        /// </summary>
        public const int BaseBlockSize = 100000;

        /// <summary>
        /// Backend constant
        /// </summary>
        public const int MaximumAlphaSize = 258;

        /// <summary>
        /// Backend constant
        /// </summary>
        public const int MaximumCodeLength = 23;

        /// <summary>
        /// Backend constant
        /// </summary>
        public const int RunA = 0;

        /// <summary>
        /// Backend constant
        /// </summary>
        public const int RunB = 1;

        /// <summary>
        /// Backend constant
        /// </summary>
        public const int GroupCount = 6;

        /// <summary>
        /// Backend constant
        /// </summary>
        public const int GroupSize = 50;

        /// <summary>
        /// Backend constant
        /// </summary>
        public const int NumberOfIterations = 4;

        /// <summary>
        /// Backend constant
        /// </summary>
        public const int MaximumSelectors = (2 + (900000 / GroupSize));

        /// <summary>
        /// Backend constant
        /// </summary>
        public const int OvershootBytes = 20;
    }

    /// <summary>
    /// BZip2Exception represents exceptions specific to BZip2 classes and code.
    /// </summary>
    [Serializable]
    public class BZip2Exception : SharpZipBaseException
    {
        /// <summary>
        /// Initialise a new instance of <see cref="BZip2Exception" />.
        /// </summary>
        public BZip2Exception()
        {
        }

        /// <summary>
        /// Initialise a new instance of <see cref="BZip2Exception" /> with its message string.
        /// </summary>
        /// <param name="message">A <see cref="string"/> that describes the error.</param>
        public BZip2Exception(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initialise a new instance of <see cref="BZip2Exception" />.
        /// </summary>
        /// <param name="message">A <see cref="string"/> that describes the error.</param>
        /// <param name="innerException">The <see cref="Exception"/> that caused this exception.</param>
        public BZip2Exception(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }

    /// <summary>
    /// An input stream that decompresses files in the BZip2 format
    /// </summary>
    public class BZip2InputStream : Stream
    {
        #region Constants

        private const int START_BLOCK_STATE = 1;
        private const int RAND_PART_A_STATE = 2;
        private const int RAND_PART_B_STATE = 3;
        private const int RAND_PART_C_STATE = 4;
        private const int NO_RAND_PART_A_STATE = 5;
        private const int NO_RAND_PART_B_STATE = 6;
        private const int NO_RAND_PART_C_STATE = 7;

#if VECTORIZE_MEMORY_MOVE
        private static readonly int VectorSize = System.Numerics.Vector<byte>.Count;
#endif // VECTORIZE_MEMORY_MOVE

        #endregion Constants

        #region Instance Fields

        /*--
		index of the last char in the block, so
		the block size == last + 1.
		--*/
        private int last;

        /*--
		index in zptr[] of original string after sorting.
		--*/
        private int origPtr;

        /*--
		always: in the range 0 .. 9.
		The current block size is 100000 * this number.
		--*/
        private int blockSize100k;

        private bool blockRandomised;

        private int bsBuff;
        private int bsLive;
        private IChecksum mCrc = new BZip2Crc();

        private bool[] inUse = new bool[256];
        private int nInUse;

        private byte[] seqToUnseq = new byte[256];
        private byte[] unseqToSeq = new byte[256];

        private byte[] selector = new byte[BZip2Constants.MaximumSelectors];
        private byte[] selectorMtf = new byte[BZip2Constants.MaximumSelectors];

        private int[] tt;
        private byte[] ll8;

        /*--
		freq table collected to save a pass over the data
		during decompression.
		--*/
        private int[] unzftab = new int[256];

        private int[][] limit = new int[BZip2Constants.GroupCount][];
        private int[][] baseArray = new int[BZip2Constants.GroupCount][];
        private int[][] perm = new int[BZip2Constants.GroupCount][];
        private int[] minLens = new int[BZip2Constants.GroupCount];

        private readonly Stream baseStream;
        private bool streamEnd;

        private int currentChar = -1;

        private int currentState = START_BLOCK_STATE;

        private int storedBlockCRC, storedCombinedCRC;
        private int computedBlockCRC;
        private uint computedCombinedCRC;

        private int count, chPrev, ch2;
        private int tPos;
        private int rNToGo;
        private int rTPos;
        private int i2, j2;
        private byte z;

        #endregion Instance Fields

        /// <summary>
        /// Construct instance for reading from stream
        /// </summary>
        /// <param name="stream">Data source</param>
        public BZip2InputStream(Stream stream)
        {
            if (stream == null)
                throw new ArgumentNullException(nameof(stream));
            // init arrays
            for (int i = 0; i < BZip2Constants.GroupCount; ++i)
            {
                limit[i] = new int[BZip2Constants.MaximumAlphaSize];
                baseArray[i] = new int[BZip2Constants.MaximumAlphaSize];
                perm[i] = new int[BZip2Constants.MaximumAlphaSize];
            }

            baseStream = stream;
            bsLive = 0;
            bsBuff = 0;
            Initialize();
            InitBlock();
            SetupBlock();
        }

        /// <summary>
        /// Get/set flag indicating ownership of underlying stream.
        /// When the flag is true <see cref="Stream.Dispose()" /> will close the underlying stream also.
        /// </summary>
        public bool IsStreamOwner { get; set; } = true;

        #region Stream Overrides

        /// <summary>
        /// Gets a value indicating if the stream supports reading
        /// </summary>
        public override bool CanRead
        {
            get
            {
                return baseStream.CanRead;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the current stream supports seeking.
        /// </summary>
        public override bool CanSeek
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the current stream supports writing.
        /// This property always returns false
        /// </summary>
        public override bool CanWrite
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// Gets the length in bytes of the stream.
        /// </summary>
        public override long Length
        {
            get
            {
                return baseStream.Length;
            }
        }

        /// <summary>
        /// Gets the current position of the stream.
        /// Setting the position is not supported and will throw a NotSupportException.
        /// </summary>
        /// <exception cref="NotSupportedException">Any attempt to set the position.</exception>
        public override long Position
        {
            get
            {
                return baseStream.Position;
            }
            set
            {
                throw new NotSupportedException("BZip2InputStream position cannot be set");
            }
        }

        /// <summary>
        /// Flushes the stream.
        /// </summary>
        public override void Flush()
        {
            baseStream.Flush();
        }

        /// <summary>
        /// Set the streams position.  This operation is not supported and will throw a NotSupportedException
        /// </summary>
        /// <param name="offset">A byte offset relative to the <paramref name="origin"/> parameter.</param>
        /// <param name="origin">A value of type <see cref="SeekOrigin"/> indicating the reference point used to obtain the new position.</param>
        /// <returns>The new position of the stream.</returns>
        /// <exception cref="NotSupportedException">Any access</exception>
        public override long Seek(long offset, SeekOrigin origin)
        {
            throw new NotSupportedException("BZip2InputStream Seek not supported");
        }

        /// <summary>
        /// Sets the length of this stream to the given value.
        /// This operation is not supported and will throw a NotSupportedExceptionortedException
        /// </summary>
        /// <param name="value">The new length for the stream.</param>
        /// <exception cref="NotSupportedException">Any access</exception>
        public override void SetLength(long value)
        {
            throw new NotSupportedException("BZip2InputStream SetLength not supported");
        }

        /// <summary>
        /// Writes a block of bytes to this stream using data from a buffer.
        /// This operation is not supported and will throw a NotSupportedException
        /// </summary>
        /// <param name="buffer">The buffer to source data from.</param>
        /// <param name="offset">The offset to start obtaining data from.</param>
        /// <param name="count">The number of bytes of data to write.</param>
        /// <exception cref="NotSupportedException">Any access</exception>
        public override void Write(byte[] buffer, int offset, int count)
        {
            throw new NotSupportedException("BZip2InputStream Write not supported");
        }

        /// <summary>
        /// Writes a byte to the current position in the file stream.
        /// This operation is not supported and will throw a NotSupportedException
        /// </summary>
        /// <param name="value">The value to write.</param>
        /// <exception cref="NotSupportedException">Any access</exception>
        public override void WriteByte(byte value)
        {
            throw new NotSupportedException("BZip2InputStream WriteByte not supported");
        }

        /// <summary>
        /// Read a sequence of bytes and advances the read position by one byte.
        /// </summary>
        /// <param name="buffer">Array of bytes to store values in</param>
        /// <param name="offset">Offset in array to begin storing data</param>
        /// <param name="count">The maximum number of bytes to read</param>
        /// <returns>The total number of bytes read into the buffer. This might be less
        /// than the number of bytes requested if that number of bytes are not
        /// currently available or zero if the end of the stream is reached.
        /// </returns>
        public override int Read(byte[] buffer, int offset, int count)
        {
            if (buffer == null)
            {
                throw new ArgumentNullException(nameof(buffer));
            }

            for (int i = 0; i < count; ++i)
            {
                int rb = ReadByte();
                if (rb == -1)
                {
                    return i;
                }
                buffer[offset + i] = (byte)rb;
            }
            return count;
        }

        /// <summary>
        /// Closes the stream, releasing any associated resources.
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            if (disposing && IsStreamOwner)
            {
                baseStream.Dispose();
            }
        }

        /// <summary>
        /// Read a byte from stream advancing position
        /// </summary>
        /// <returns>byte read or -1 on end of stream</returns>
        public override int ReadByte()
        {
            if (streamEnd)
            {
                return -1; // ok
            }

            int retChar = currentChar;
            switch (currentState)
            {
                case RAND_PART_B_STATE:
                    SetupRandPartB();
                    break;

                case RAND_PART_C_STATE:
                    SetupRandPartC();
                    break;

                case NO_RAND_PART_B_STATE:
                    SetupNoRandPartB();
                    break;

                case NO_RAND_PART_C_STATE:
                    SetupNoRandPartC();
                    break;

                case START_BLOCK_STATE:
                case NO_RAND_PART_A_STATE:
                case RAND_PART_A_STATE:
                    break;
            }
            return retChar;
        }

        #endregion Stream Overrides

        private void MakeMaps()
        {
            nInUse = 0;
            for (int i = 0; i < 256; ++i)
            {
                if (inUse[i])
                {
                    seqToUnseq[nInUse] = (byte)i;
                    unseqToSeq[i] = (byte)nInUse;
                    nInUse++;
                }
            }
        }

        private void Initialize()
        {
            char magic1 = BsGetUChar();
            char magic2 = BsGetUChar();

            char magic3 = BsGetUChar();
            char magic4 = BsGetUChar();

            if (magic1 != 'B' || magic2 != 'Z' || magic3 != 'h' || magic4 < '1' || magic4 > '9')
            {
                streamEnd = true;
                return;
            }

            SetDecompressStructureSizes(magic4 - '0');
            computedCombinedCRC = 0;
        }

        private void InitBlock()
        {
            char magic1 = BsGetUChar();
            char magic2 = BsGetUChar();
            char magic3 = BsGetUChar();
            char magic4 = BsGetUChar();
            char magic5 = BsGetUChar();
            char magic6 = BsGetUChar();

            if (magic1 == 0x17 && magic2 == 0x72 && magic3 == 0x45 && magic4 == 0x38 && magic5 == 0x50 && magic6 == 0x90)
            {
                Complete();
                return;
            }

            if (magic1 != 0x31 || magic2 != 0x41 || magic3 != 0x59 || magic4 != 0x26 || magic5 != 0x53 || magic6 != 0x59)
            {
                BadBlockHeader();
                streamEnd = true;
                return;
            }

            storedBlockCRC = BsGetInt32();

            blockRandomised = (BsR(1) == 1);

            GetAndMoveToFrontDecode();

            mCrc.Reset();
            currentState = START_BLOCK_STATE;
        }

        private void EndBlock()
        {
            computedBlockCRC = (int)mCrc.Value;

            // -- A bad CRC is considered a fatal error. --
            if (storedBlockCRC != computedBlockCRC)
            {
                CrcError();
            }

            // 1528150659
            computedCombinedCRC = ((computedCombinedCRC << 1) & 0xFFFFFFFF) | (computedCombinedCRC >> 31);
            computedCombinedCRC = computedCombinedCRC ^ (uint)computedBlockCRC;
        }

        private void Complete()
        {
            storedCombinedCRC = BsGetInt32();
            if (storedCombinedCRC != (int)computedCombinedCRC)
            {
                CrcError();
            }

            streamEnd = true;
        }

        private void FillBuffer()
        {
            int thech = 0;

            try
            {
                thech = baseStream.ReadByte();
            }
            catch (Exception)
            {
                CompressedStreamEOF();
            }

            if (thech == -1)
            {
                CompressedStreamEOF();
            }

            bsBuff = (bsBuff << 8) | (thech & 0xFF);
            bsLive += 8;
        }

        private int BsR(int n)
        {
            while (bsLive < n)
            {
                FillBuffer();
            }

            int v = (bsBuff >> (bsLive - n)) & ((1 << n) - 1);
            bsLive -= n;
            return v;
        }

        private char BsGetUChar()
        {
            return (char)BsR(8);
        }

        private int BsGetIntVS(int numBits)
        {
            return BsR(numBits);
        }

        private int BsGetInt32()
        {
            int result = BsR(8);
            result = (result << 8) | BsR(8);
            result = (result << 8) | BsR(8);
            result = (result << 8) | BsR(8);
            return result;
        }

        private void RecvDecodingTables()
        {
            char[][] len = new char[BZip2Constants.GroupCount][];
            for (int i = 0; i < BZip2Constants.GroupCount; ++i)
            {
                len[i] = new char[BZip2Constants.MaximumAlphaSize];
            }

            bool[] inUse16 = new bool[16];

            //--- Receive the mapping table ---
            for (int i = 0; i < 16; i++)
            {
                inUse16[i] = (BsR(1) == 1);
            }

            for (int i = 0; i < 16; i++)
            {
                if (inUse16[i])
                {
                    for (int j = 0; j < 16; j++)
                    {
                        inUse[i * 16 + j] = (BsR(1) == 1);
                    }
                }
                else
                {
                    for (int j = 0; j < 16; j++)
                    {
                        inUse[i * 16 + j] = false;
                    }
                }
            }

            MakeMaps();
            int alphaSize = nInUse + 2;

            //--- Now the selectors ---
            int nGroups = BsR(3);
            int nSelectors = BsR(15);

            for (int i = 0; i < nSelectors; i++)
            {
                int j = 0;
                while (BsR(1) == 1)
                {
                    j++;
                }
                selectorMtf[i] = (byte)j;
            }

            //--- Undo the MTF values for the selectors. ---
            byte[] pos = new byte[BZip2Constants.GroupCount];
            for (int v = 0; v < nGroups; v++)
            {
                pos[v] = (byte)v;
            }

            for (int i = 0; i < nSelectors; i++)
            {
                int v = selectorMtf[i];
                byte tmp = pos[v];
                while (v > 0)
                {
                    pos[v] = pos[v - 1];
                    v--;
                }
                pos[0] = tmp;
                selector[i] = tmp;
            }

            //--- Now the coding tables ---
            for (int t = 0; t < nGroups; t++)
            {
                int curr = BsR(5);
                for (int i = 0; i < alphaSize; i++)
                {
                    while (BsR(1) == 1)
                    {
                        if (BsR(1) == 0)
                        {
                            curr++;
                        }
                        else
                        {
                            curr--;
                        }
                    }
                    len[t][i] = (char)curr;
                }
            }

            //--- Create the Huffman decoding tables ---
            for (int t = 0; t < nGroups; t++)
            {
                int minLen = 32;
                int maxLen = 0;
                for (int i = 0; i < alphaSize; i++)
                {
                    maxLen = Math.Max(maxLen, len[t][i]);
                    minLen = Math.Min(minLen, len[t][i]);
                }
                HbCreateDecodeTables(limit[t], baseArray[t], perm[t], len[t], minLen, maxLen, alphaSize);
                minLens[t] = minLen;
            }
        }

        private void GetAndMoveToFrontDecode()
        {
            byte[] yy = new byte[256];
            int nextSym;

            int limitLast = BZip2Constants.BaseBlockSize * blockSize100k;
            origPtr = BsGetIntVS(24);

            RecvDecodingTables();
            int EOB = nInUse + 1;
            int groupNo = -1;
            int groupPos = 0;

            /*--
			Setting up the unzftab entries here is not strictly
			necessary, but it does save having to do it later
			in a separate pass, and so saves a block's worth of
			cache misses.
			--*/
            for (int i = 0; i <= 255; i++)
            {
                unzftab[i] = 0;
            }

            for (int i = 0; i <= 255; i++)
            {
                yy[i] = (byte)i;
            }

            last = -1;

            if (groupPos == 0)
            {
                groupNo++;
                groupPos = BZip2Constants.GroupSize;
            }

            groupPos--;
            int zt = selector[groupNo];
            int zn = minLens[zt];
            int zvec = BsR(zn);
            int zj;

            while (zvec > limit[zt][zn])
            {
                if (zn > 20)
                { // the longest code
                    throw new BZip2Exception("Bzip data error");
                }
                zn++;
                while (bsLive < 1)
                {
                    FillBuffer();
                }
                zj = (bsBuff >> (bsLive - 1)) & 1;
                bsLive--;
                zvec = (zvec << 1) | zj;
            }
            if (zvec - baseArray[zt][zn] < 0 || zvec - baseArray[zt][zn] >= BZip2Constants.MaximumAlphaSize)
            {
                throw new BZip2Exception("Bzip data error");
            }
            nextSym = perm[zt][zvec - baseArray[zt][zn]];

            while (true)
            {
                if (nextSym == EOB)
                {
                    break;
                }

                if (nextSym == BZip2Constants.RunA || nextSym == BZip2Constants.RunB)
                {
                    int s = -1;
                    int n = 1;
                    do
                    {
                        if (nextSym == BZip2Constants.RunA)
                        {
                            s += (0 + 1) * n;
                        }
                        else if (nextSym == BZip2Constants.RunB)
                        {
                            s += (1 + 1) * n;
                        }

                        n <<= 1;

                        if (groupPos == 0)
                        {
                            groupNo++;
                            groupPos = BZip2Constants.GroupSize;
                        }

                        groupPos--;

                        zt = selector[groupNo];
                        zn = minLens[zt];
                        zvec = BsR(zn);

                        while (zvec > limit[zt][zn])
                        {
                            zn++;
                            while (bsLive < 1)
                            {
                                FillBuffer();
                            }
                            zj = (bsBuff >> (bsLive - 1)) & 1;
                            bsLive--;
                            zvec = (zvec << 1) | zj;
                        }
                        nextSym = perm[zt][zvec - baseArray[zt][zn]];
                    } while (nextSym == BZip2Constants.RunA || nextSym == BZip2Constants.RunB);

                    s++;
                    byte ch = seqToUnseq[yy[0]];
                    unzftab[ch] += s;

                    while (s > 0)
                    {
                        last++;
                        ll8[last] = ch;
                        s--;
                    }

                    if (last >= limitLast)
                    {
                        BlockOverrun();
                    }
                    continue;
                }
                else
                {
                    last++;
                    if (last >= limitLast)
                    {
                        BlockOverrun();
                    }

                    byte tmp = yy[nextSym - 1];
                    unzftab[seqToUnseq[tmp]]++;
                    ll8[last] = seqToUnseq[tmp];

                    var j = nextSym - 1;

#if VECTORIZE_MEMORY_MOVE
                    // This is vectorized memory move. Going from the back, we're taking chunks of array
                    // and write them at the new location shifted by one. Since chunks are VectorSize long,
                    // at the end we have to move "tail" (or head actually) of the array using a plain loop.
                    // If System.Numerics.Vector API is not available, the plain loop is used to do the whole copying.

                    while (j >= VectorSize)
                    {
                        var arrayPart = new System.Numerics.Vector<byte>(yy, j - VectorSize);
                        arrayPart.CopyTo(yy, j - VectorSize + 1);
                        j -= VectorSize;
                    }
#endif // VECTORIZE_MEMORY_MOVE

                    while (j > 0)
                    {
                        yy[j] = yy[--j];
                    }

                    yy[0] = tmp;

                    if (groupPos == 0)
                    {
                        groupNo++;
                        groupPos = BZip2Constants.GroupSize;
                    }

                    groupPos--;
                    zt = selector[groupNo];
                    zn = minLens[zt];
                    zvec = BsR(zn);
                    while (zvec > limit[zt][zn])
                    {
                        zn++;
                        while (bsLive < 1)
                        {
                            FillBuffer();
                        }
                        zj = (bsBuff >> (bsLive - 1)) & 1;
                        bsLive--;
                        zvec = (zvec << 1) | zj;
                    }
                    nextSym = perm[zt][zvec - baseArray[zt][zn]];
                    continue;
                }
            }
        }

        private void SetupBlock()
        {
            int[] cftab = new int[257];

            cftab[0] = 0;
            Array.Copy(unzftab, 0, cftab, 1, 256);

            for (int i = 1; i <= 256; i++)
            {
                cftab[i] += cftab[i - 1];
            }

            for (int i = 0; i <= last; i++)
            {
                byte ch = ll8[i];
                tt[cftab[ch]] = i;
                cftab[ch]++;
            }

            cftab = null;

            tPos = tt[origPtr];

            count = 0;
            i2 = 0;
            ch2 = 256;   /*-- not a char and not EOF --*/

            if (blockRandomised)
            {
                rNToGo = 0;
                rTPos = 0;
                SetupRandPartA();
            }
            else
            {
                SetupNoRandPartA();
            }
        }

        private void SetupRandPartA()
        {
            if (i2 <= last)
            {
                chPrev = ch2;
                ch2 = ll8[tPos];
                tPos = tt[tPos];
                if (rNToGo == 0)
                {
                    rNToGo = BZip2Constants.RandomNumbers[rTPos];
                    rTPos++;
                    if (rTPos == 512)
                    {
                        rTPos = 0;
                    }
                }
                rNToGo--;
                ch2 ^= (int)((rNToGo == 1) ? 1 : 0);
                i2++;

                currentChar = ch2;
                currentState = RAND_PART_B_STATE;
                mCrc.Update(ch2);
            }
            else
            {
                EndBlock();
                InitBlock();
                SetupBlock();
            }
        }

        private void SetupNoRandPartA()
        {
            if (i2 <= last)
            {
                chPrev = ch2;
                ch2 = ll8[tPos];
                tPos = tt[tPos];
                i2++;

                currentChar = ch2;
                currentState = NO_RAND_PART_B_STATE;
                mCrc.Update(ch2);
            }
            else
            {
                EndBlock();
                InitBlock();
                SetupBlock();
            }
        }

        private void SetupRandPartB()
        {
            if (ch2 != chPrev)
            {
                currentState = RAND_PART_A_STATE;
                count = 1;
                SetupRandPartA();
            }
            else
            {
                count++;
                if (count >= 4)
                {
                    z = ll8[tPos];
                    tPos = tt[tPos];
                    if (rNToGo == 0)
                    {
                        rNToGo = BZip2Constants.RandomNumbers[rTPos];
                        rTPos++;
                        if (rTPos == 512)
                        {
                            rTPos = 0;
                        }
                    }
                    rNToGo--;
                    z ^= (byte)((rNToGo == 1) ? 1 : 0);
                    j2 = 0;
                    currentState = RAND_PART_C_STATE;
                    SetupRandPartC();
                }
                else
                {
                    currentState = RAND_PART_A_STATE;
                    SetupRandPartA();
                }
            }
        }

        private void SetupRandPartC()
        {
            if (j2 < (int)z)
            {
                currentChar = ch2;
                mCrc.Update(ch2);
                j2++;
            }
            else
            {
                currentState = RAND_PART_A_STATE;
                i2++;
                count = 0;
                SetupRandPartA();
            }
        }

        private void SetupNoRandPartB()
        {
            if (ch2 != chPrev)
            {
                currentState = NO_RAND_PART_A_STATE;
                count = 1;
                SetupNoRandPartA();
            }
            else
            {
                count++;
                if (count >= 4)
                {
                    z = ll8[tPos];
                    tPos = tt[tPos];
                    currentState = NO_RAND_PART_C_STATE;
                    j2 = 0;
                    SetupNoRandPartC();
                }
                else
                {
                    currentState = NO_RAND_PART_A_STATE;
                    SetupNoRandPartA();
                }
            }
        }

        private void SetupNoRandPartC()
        {
            if (j2 < (int)z)
            {
                currentChar = ch2;
                mCrc.Update(ch2);
                j2++;
            }
            else
            {
                currentState = NO_RAND_PART_A_STATE;
                i2++;
                count = 0;
                SetupNoRandPartA();
            }
        }

        private void SetDecompressStructureSizes(int newSize100k)
        {
            if (!(0 <= newSize100k && newSize100k <= 9 && 0 <= blockSize100k && blockSize100k <= 9))
            {
                throw new BZip2Exception("Invalid block size");
            }

            blockSize100k = newSize100k;

            if (newSize100k == 0)
            {
                return;
            }

            int n = BZip2Constants.BaseBlockSize * newSize100k;
            ll8 = new byte[n];
            tt = new int[n];
        }

        private static void CompressedStreamEOF()
        {
            throw new EndOfStreamException("BZip2 input stream end of compressed stream");
        }

        private static void BlockOverrun()
        {
            throw new BZip2Exception("BZip2 input stream block overrun");
        }

        private static void BadBlockHeader()
        {
            throw new BZip2Exception("BZip2 input stream bad block header");
        }

        private static void CrcError()
        {
            throw new BZip2Exception("BZip2 input stream crc error");
        }

        private static void HbCreateDecodeTables(int[] limit, int[] baseArray, int[] perm, char[] length, int minLen, int maxLen, int alphaSize)
        {
            int pp = 0;

            for (int i = minLen; i <= maxLen; ++i)
            {
                for (int j = 0; j < alphaSize; ++j)
                {
                    if (length[j] == i)
                    {
                        perm[pp] = j;
                        ++pp;
                    }
                }
            }

            for (int i = 0; i < BZip2Constants.MaximumCodeLength; i++)
            {
                baseArray[i] = 0;
            }

            for (int i = 0; i < alphaSize; i++)
            {
                ++baseArray[length[i] + 1];
            }

            for (int i = 1; i < BZip2Constants.MaximumCodeLength; i++)
            {
                baseArray[i] += baseArray[i - 1];
            }

            for (int i = 0; i < BZip2Constants.MaximumCodeLength; i++)
            {
                limit[i] = 0;
            }

            int vec = 0;

            for (int i = minLen; i <= maxLen; i++)
            {
                vec += (baseArray[i + 1] - baseArray[i]);
                limit[i] = vec - 1;
                vec <<= 1;
            }

            for (int i = minLen + 1; i <= maxLen; i++)
            {
                baseArray[i] = ((limit[i - 1] + 1) << 1) - baseArray[i];
            }
        }
    }

    /// <summary>
    /// An output stream that compresses into the BZip2 format
    /// including file header chars into another stream.
    /// </summary>
    public class BZip2OutputStream : Stream
    {
        #region Constants

        private const int SETMASK = (1 << 21);
        private const int CLEARMASK = (~SETMASK);
        private const int GREATER_ICOST = 15;
        private const int LESSER_ICOST = 0;
        private const int SMALL_THRESH = 20;
        private const int DEPTH_THRESH = 10;

        /*--
		If you are ever unlucky/improbable enough
		to get a stack overflow whilst sorting,
		increase the following constant and try
		again.  In practice I have never seen the
		stack go above 27 elems, so the following
		limit seems very generous.
		--*/
        private const int QSORT_STACK_SIZE = 1000;

        /*--
		Knuth's increments seem to work better
		than Incerpi-Sedgewick here.  Possibly
		because the number of elems to sort is
		usually small, typically <= 20.
		--*/

        private readonly int[] increments = {
                                                  1, 4, 13, 40, 121, 364, 1093, 3280,
                                                  9841, 29524, 88573, 265720,
                                                  797161, 2391484
                                              };

        #endregion Constants

        #region Instance Fields

        /*--
		index of the last char in the block, so
		the block size == last + 1.
		--*/
        private int last;

        /*--
		index in zptr[] of original string after sorting.
		--*/
        private int origPtr;

        /*--
		always: in the range 0 .. 9.
		The current block size is 100000 * this number.
		--*/
        private int blockSize100k;

        private bool blockRandomised;

        private int bytesOut;
        private int bsBuff;
        private int bsLive;
        private IChecksum mCrc = new BZip2Crc();

        private bool[] inUse = new bool[256];
        private int nInUse;

        private char[] seqToUnseq = new char[256];
        private char[] unseqToSeq = new char[256];

        private char[] selector = new char[BZip2Constants.MaximumSelectors];
        private char[] selectorMtf = new char[BZip2Constants.MaximumSelectors];

        private byte[] block;
        private int[] quadrant;
        private int[] zptr;
        private short[] szptr;
        private int[] ftab;

        private int nMTF;

        private int[] mtfFreq = new int[BZip2Constants.MaximumAlphaSize];

        /*
		* Used when sorting.  If too many long comparisons
		* happen, we stop sorting, randomise the block
		* slightly, and try again.
		*/
        private int workFactor;
        private int workDone;
        private int workLimit;
        private bool firstAttempt;
        private int nBlocksRandomised;

        private int currentChar = -1;
        private int runLength;
        private uint blockCRC, combinedCRC;
        private int allowableBlockSize;
        private readonly Stream baseStream;
        private bool disposed_;

        #endregion Instance Fields

        /// <summary>
        /// Construct a default output stream with maximum block size
        /// </summary>
        /// <param name="stream">The stream to write BZip data onto.</param>
        public BZip2OutputStream(Stream stream) : this(stream, 9)
        {
        }

        /// <summary>
        /// Initialise a new instance of the <see cref="BZip2OutputStream"></see>
        /// for the specified stream, using the given blocksize.
        /// </summary>
        /// <param name="stream">The stream to write compressed data to.</param>
        /// <param name="blockSize">The block size to use.</param>
        /// <remarks>
        /// Valid block sizes are in the range 1..9, with 1 giving
        /// the lowest compression and 9 the highest.
        /// </remarks>
        public BZip2OutputStream(Stream stream, int blockSize)
        {
            if (stream == null)
                throw new ArgumentNullException(nameof(stream));

            baseStream = stream;
            bsLive = 0;
            bsBuff = 0;
            bytesOut = 0;

            workFactor = 50;
            if (blockSize > 9)
            {
                blockSize = 9;
            }

            if (blockSize < 1)
            {
                blockSize = 1;
            }
            blockSize100k = blockSize;
            AllocateCompressStructures();
            Initialize();
            InitBlock();
        }

        /// <summary>
        /// Ensures that resources are freed and other cleanup operations
        /// are performed when the garbage collector reclaims the BZip2OutputStream.
        /// </summary>
        ~BZip2OutputStream()
        {
            Dispose(false);
        }

        /// <summary>
        /// Gets or sets a flag indicating ownership of underlying stream.
        /// When the flag is true <see cref="Stream.Dispose()" /> will close the underlying stream also.
        /// </summary>
        /// <remarks>The default value is true.</remarks>
        public bool IsStreamOwner { get; set; } = true;

        /// <summary>
        /// Gets a value indicating whether the current stream supports reading
        /// </summary>
        public override bool CanRead
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the current stream supports seeking
        /// </summary>
        public override bool CanSeek
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the current stream supports writing
        /// </summary>
        public override bool CanWrite
        {
            get
            {
                return baseStream.CanWrite;
            }
        }

        /// <summary>
        /// Gets the length in bytes of the stream
        /// </summary>
        public override long Length
        {
            get
            {
                return baseStream.Length;
            }
        }

        /// <summary>
        /// Gets or sets the current position of this stream.
        /// </summary>
        public override long Position
        {
            get
            {
                return baseStream.Position;
            }
            set
            {
                throw new NotSupportedException("BZip2OutputStream position cannot be set");
            }
        }

        /// <summary>
        /// Sets the current position of this stream to the given value.
        /// </summary>
        /// <param name="offset">The point relative to the offset from which to being seeking.</param>
        /// <param name="origin">The reference point from which to begin seeking.</param>
        /// <returns>The new position in the stream.</returns>
        public override long Seek(long offset, SeekOrigin origin)
        {
            throw new NotSupportedException("BZip2OutputStream Seek not supported");
        }

        /// <summary>
        /// Sets the length of this stream to the given value.
        /// </summary>
        /// <param name="value">The new stream length.</param>
        public override void SetLength(long value)
        {
            throw new NotSupportedException("BZip2OutputStream SetLength not supported");
        }

        /// <summary>
        /// Read a byte from the stream advancing the position.
        /// </summary>
        /// <returns>The byte read cast to an int; -1 if end of stream.</returns>
        public override int ReadByte()
        {
            throw new NotSupportedException("BZip2OutputStream ReadByte not supported");
        }

        /// <summary>
        /// Read a block of bytes
        /// </summary>
        /// <param name="buffer">The buffer to read into.</param>
        /// <param name="offset">The offset in the buffer to start storing data at.</param>
        /// <param name="count">The maximum number of bytes to read.</param>
        /// <returns>The total number of bytes read. This might be less than the number of bytes
        /// requested if that number of bytes are not currently available, or zero
        /// if the end of the stream is reached.</returns>
        public override int Read(byte[] buffer, int offset, int count)
        {
            throw new NotSupportedException("BZip2OutputStream Read not supported");
        }

        /// <summary>
        /// Write a block of bytes to the stream
        /// </summary>
        /// <param name="buffer">The buffer containing data to write.</param>
        /// <param name="offset">The offset of the first byte to write.</param>
        /// <param name="count">The number of bytes to write.</param>
        public override void Write(byte[] buffer, int offset, int count)
        {
            if (buffer == null)
            {
                throw new ArgumentNullException(nameof(buffer));
            }

            if (offset < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(offset));
            }

            if (count < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(count));
            }

            if (buffer.Length - offset < count)
            {
                throw new ArgumentException("Offset/count out of range");
            }

            for (int i = 0; i < count; ++i)
            {
                WriteByte(buffer[offset + i]);
            }
        }

        /// <summary>
        /// Write a byte to the stream.
        /// </summary>
        /// <param name="value">The byte to write to the stream.</param>
        public override void WriteByte(byte value)
        {
            int b = (256 + value) % 256;
            if (currentChar != -1)
            {
                if (currentChar == b)
                {
                    runLength++;
                    if (runLength > 254)
                    {
                        WriteRun();
                        currentChar = -1;
                        runLength = 0;
                    }
                }
                else
                {
                    WriteRun();
                    runLength = 1;
                    currentChar = b;
                }
            }
            else
            {
                currentChar = b;
                runLength++;
            }
        }

        private void MakeMaps()
        {
            nInUse = 0;
            for (int i = 0; i < 256; i++)
            {
                if (inUse[i])
                {
                    seqToUnseq[nInUse] = (char)i;
                    unseqToSeq[i] = (char)nInUse;
                    nInUse++;
                }
            }
        }

        /// <summary>
        /// Get the number of bytes written to output.
        /// </summary>
        private void WriteRun()
        {
            if (last < allowableBlockSize)
            {
                inUse[currentChar] = true;
                for (int i = 0; i < runLength; i++)
                {
                    mCrc.Update(currentChar);
                }

                switch (runLength)
                {
                    case 1:
                        last++;
                        block[last + 1] = (byte)currentChar;
                        break;

                    case 2:
                        last++;
                        block[last + 1] = (byte)currentChar;
                        last++;
                        block[last + 1] = (byte)currentChar;
                        break;

                    case 3:
                        last++;
                        block[last + 1] = (byte)currentChar;
                        last++;
                        block[last + 1] = (byte)currentChar;
                        last++;
                        block[last + 1] = (byte)currentChar;
                        break;

                    default:
                        inUse[runLength - 4] = true;
                        last++;
                        block[last + 1] = (byte)currentChar;
                        last++;
                        block[last + 1] = (byte)currentChar;
                        last++;
                        block[last + 1] = (byte)currentChar;
                        last++;
                        block[last + 1] = (byte)currentChar;
                        last++;
                        block[last + 1] = (byte)(runLength - 4);
                        break;
                }
            }
            else
            {
                EndBlock();
                InitBlock();
                WriteRun();
            }
        }

        /// <summary>
        /// Get the number of bytes written to the output.
        /// </summary>
        public int BytesWritten
        {
            get { return bytesOut; }
        }

        /// <summary>
        /// Releases the unmanaged resources used by the <see cref="BZip2OutputStream"/> and optionally releases the managed resources.
        /// </summary>
        /// <param name="disposing">true to release both managed and unmanaged resources; false to release only unmanaged resources.</param>
        override protected void Dispose(bool disposing)
        {
            try
            {
                try
                {
                    base.Dispose(disposing);
                    if (!disposed_)
                    {
                        disposed_ = true;

                        if (runLength > 0)
                        {
                            WriteRun();
                        }

                        currentChar = -1;
                        EndBlock();
                        EndCompression();
                        Flush();
                    }
                }
                finally
                {
                    if (disposing)
                    {
                        if (IsStreamOwner)
                        {
                            baseStream.Dispose();
                        }
                    }
                }
            }
            catch
            {
            }
        }

        /// <summary>
        /// Flush output buffers
        /// </summary>
        public override void Flush()
        {
            baseStream.Flush();
        }

        private void Initialize()
        {
            bytesOut = 0;
            nBlocksRandomised = 0;

            /*--- Write header `magic' bytes indicating file-format == huffmanised,
			followed by a digit indicating blockSize100k.
			---*/

            BsPutUChar('B');
            BsPutUChar('Z');

            BsPutUChar('h');
            BsPutUChar('0' + blockSize100k);

            combinedCRC = 0;
        }

        private void InitBlock()
        {
            mCrc.Reset();
            last = -1;

            for (int i = 0; i < 256; i++)
            {
                inUse[i] = false;
            }

            /*--- 20 is just a paranoia constant ---*/
            allowableBlockSize = BZip2Constants.BaseBlockSize * blockSize100k - 20;
        }

        private void EndBlock()
        {
            if (last < 0)
            {       // dont do anything for empty files, (makes empty files compatible with original Bzip)
                return;
            }

            blockCRC = unchecked((uint)mCrc.Value);
            combinedCRC = (combinedCRC << 1) | (combinedCRC >> 31);
            combinedCRC ^= blockCRC;

            /*-- sort the block and establish position of original string --*/
            DoReversibleTransformation();

            /*--
			A 6-byte block header, the value chosen arbitrarily
			as 0x314159265359 :-).  A 32 bit value does not really
			give a strong enough guarantee that the value will not
			appear by chance in the compressed datastream.  Worst-case
			probability of this event, for a 900k block, is about
			2.0e-3 for 32 bits, 1.0e-5 for 40 bits and 4.0e-8 for 48 bits.
			For a compressed file of size 100Gb -- about 100000 blocks --
			only a 48-bit marker will do.  NB: normal compression/
			decompression do *not* rely on these statistical properties.
			They are only important when trying to recover blocks from
			damaged files.
			--*/
            BsPutUChar(0x31);
            BsPutUChar(0x41);
            BsPutUChar(0x59);
            BsPutUChar(0x26);
            BsPutUChar(0x53);
            BsPutUChar(0x59);

            /*-- Now the block's CRC, so it is in a known place. --*/
            unchecked
            {
                BsPutint((int)blockCRC);
            }

            /*-- Now a single bit indicating randomisation. --*/
            if (blockRandomised)
            {
                BsW(1, 1);
                nBlocksRandomised++;
            }
            else
            {
                BsW(1, 0);
            }

            /*-- Finally, block's contents proper. --*/
            MoveToFrontCodeAndSend();
        }

        private void EndCompression()
        {
            /*--
			Now another magic 48-bit number, 0x177245385090, to
			indicate the end of the last block.  (sqrt(pi), if
			you want to know.  I did want to use e, but it contains
			too much repetition -- 27 18 28 18 28 46 -- for me
			to feel statistically comfortable.  Call me paranoid.)
			--*/
            BsPutUChar(0x17);
            BsPutUChar(0x72);
            BsPutUChar(0x45);
            BsPutUChar(0x38);
            BsPutUChar(0x50);
            BsPutUChar(0x90);

            unchecked
            {
                BsPutint((int)combinedCRC);
            }

            BsFinishedWithStream();
        }

        private void BsFinishedWithStream()
        {
            while (bsLive > 0)
            {
                int ch = (bsBuff >> 24);
                baseStream.WriteByte((byte)ch); // write 8-bit
                bsBuff <<= 8;
                bsLive -= 8;
                bytesOut++;
            }
        }

        private void BsW(int n, int v)
        {
            while (bsLive >= 8)
            {
                int ch = (bsBuff >> 24);
                unchecked { baseStream.WriteByte((byte)ch); } // write 8-bit
                bsBuff <<= 8;
                bsLive -= 8;
                ++bytesOut;
            }
            bsBuff |= (v << (32 - bsLive - n));
            bsLive += n;
        }

        private void BsPutUChar(int c)
        {
            BsW(8, c);
        }

        private void BsPutint(int u)
        {
            BsW(8, (u >> 24) & 0xFF);
            BsW(8, (u >> 16) & 0xFF);
            BsW(8, (u >> 8) & 0xFF);
            BsW(8, u & 0xFF);
        }

        private void BsPutIntVS(int numBits, int c)
        {
            BsW(numBits, c);
        }

        private void SendMTFValues()
        {
            char[][] len = new char[BZip2Constants.GroupCount][];
            for (int i = 0; i < BZip2Constants.GroupCount; ++i)
            {
                len[i] = new char[BZip2Constants.MaximumAlphaSize];
            }

            int gs, ge, totc, bt, bc, iter;
            int nSelectors = 0, alphaSize, minLen, maxLen, selCtr;
            int nGroups;

            alphaSize = nInUse + 2;
            for (int t = 0; t < BZip2Constants.GroupCount; t++)
            {
                for (int v = 0; v < alphaSize; v++)
                {
                    len[t][v] = (char)GREATER_ICOST;
                }
            }

            /*--- Decide how many coding tables to use ---*/
            if (nMTF <= 0)
            {
                Panic();
            }

            if (nMTF < 200)
            {
                nGroups = 2;
            }
            else if (nMTF < 600)
            {
                nGroups = 3;
            }
            else if (nMTF < 1200)
            {
                nGroups = 4;
            }
            else if (nMTF < 2400)
            {
                nGroups = 5;
            }
            else
            {
                nGroups = 6;
            }

            /*--- Generate an initial set of coding tables ---*/
            int nPart = nGroups;
            int remF = nMTF;
            gs = 0;
            while (nPart > 0)
            {
                int tFreq = remF / nPart;
                int aFreq = 0;
                ge = gs - 1;
                while (aFreq < tFreq && ge < alphaSize - 1)
                {
                    ge++;
                    aFreq += mtfFreq[ge];
                }

                if (ge > gs && nPart != nGroups && nPart != 1 && ((nGroups - nPart) % 2 == 1))
                {
                    aFreq -= mtfFreq[ge];
                    ge--;
                }

                for (int v = 0; v < alphaSize; v++)
                {
                    if (v >= gs && v <= ge)
                    {
                        len[nPart - 1][v] = (char)LESSER_ICOST;
                    }
                    else
                    {
                        len[nPart - 1][v] = (char)GREATER_ICOST;
                    }
                }

                nPart--;
                gs = ge + 1;
                remF -= aFreq;
            }

            int[][] rfreq = new int[BZip2Constants.GroupCount][];
            for (int i = 0; i < BZip2Constants.GroupCount; ++i)
            {
                rfreq[i] = new int[BZip2Constants.MaximumAlphaSize];
            }

            int[] fave = new int[BZip2Constants.GroupCount];
            short[] cost = new short[BZip2Constants.GroupCount];
            /*---
			Iterate up to N_ITERS times to improve the tables.
			---*/
            for (iter = 0; iter < BZip2Constants.NumberOfIterations; ++iter)
            {
                for (int t = 0; t < nGroups; ++t)
                {
                    fave[t] = 0;
                }

                for (int t = 0; t < nGroups; ++t)
                {
                    for (int v = 0; v < alphaSize; ++v)
                    {
                        rfreq[t][v] = 0;
                    }
                }

                nSelectors = 0;
                totc = 0;
                gs = 0;
                while (true)
                {
                    /*--- Set group start & end marks. --*/
                    if (gs >= nMTF)
                    {
                        break;
                    }
                    ge = gs + BZip2Constants.GroupSize - 1;
                    if (ge >= nMTF)
                    {
                        ge = nMTF - 1;
                    }

                    /*--
					Calculate the cost of this group as coded
					by each of the coding tables.
					--*/
                    for (int t = 0; t < nGroups; t++)
                    {
                        cost[t] = 0;
                    }

                    if (nGroups == 6)
                    {
                        short cost0, cost1, cost2, cost3, cost4, cost5;
                        cost0 = cost1 = cost2 = cost3 = cost4 = cost5 = 0;
                        for (int i = gs; i <= ge; ++i)
                        {
                            short icv = szptr[i];
                            cost0 += (short)len[0][icv];
                            cost1 += (short)len[1][icv];
                            cost2 += (short)len[2][icv];
                            cost3 += (short)len[3][icv];
                            cost4 += (short)len[4][icv];
                            cost5 += (short)len[5][icv];
                        }
                        cost[0] = cost0;
                        cost[1] = cost1;
                        cost[2] = cost2;
                        cost[3] = cost3;
                        cost[4] = cost4;
                        cost[5] = cost5;
                    }
                    else
                    {
                        for (int i = gs; i <= ge; ++i)
                        {
                            short icv = szptr[i];
                            for (int t = 0; t < nGroups; t++)
                            {
                                cost[t] += (short)len[t][icv];
                            }
                        }
                    }

                    /*--
					Find the coding table which is best for this group,
					and record its identity in the selector table.
					--*/
                    bc = 999999999;
                    bt = -1;
                    for (int t = 0; t < nGroups; ++t)
                    {
                        if (cost[t] < bc)
                        {
                            bc = cost[t];
                            bt = t;
                        }
                    }
                    totc += bc;
                    fave[bt]++;
                    selector[nSelectors] = (char)bt;
                    nSelectors++;

                    /*--
					Increment the symbol frequencies for the selected table.
					--*/
                    for (int i = gs; i <= ge; ++i)
                    {
                        ++rfreq[bt][szptr[i]];
                    }

                    gs = ge + 1;
                }

                /*--
				Recompute the tables based on the accumulated frequencies.
				--*/
                for (int t = 0; t < nGroups; ++t)
                {
                    HbMakeCodeLengths(len[t], rfreq[t], alphaSize, 20);
                }
            }

            rfreq = null;
            fave = null;
            cost = null;

            if (!(nGroups < 8))
            {
                Panic();
            }

            if (!(nSelectors < 32768 && nSelectors <= (2 + (900000 / BZip2Constants.GroupSize))))
            {
                Panic();
            }

            /*--- Compute MTF values for the selectors. ---*/
            char[] pos = new char[BZip2Constants.GroupCount];
            char ll_i, tmp2, tmp;

            for (int i = 0; i < nGroups; i++)
            {
                pos[i] = (char)i;
            }

            for (int i = 0; i < nSelectors; i++)
            {
                ll_i = selector[i];
                int j = 0;
                tmp = pos[j];
                while (ll_i != tmp)
                {
                    j++;
                    tmp2 = tmp;
                    tmp = pos[j];
                    pos[j] = tmp2;
                }
                pos[0] = tmp;
                selectorMtf[i] = (char)j;
            }

            int[][] code = new int[BZip2Constants.GroupCount][];

            for (int i = 0; i < BZip2Constants.GroupCount; ++i)
            {
                code[i] = new int[BZip2Constants.MaximumAlphaSize];
            }

            /*--- Assign actual codes for the tables. --*/
            for (int t = 0; t < nGroups; t++)
            {
                minLen = 32;
                maxLen = 0;
                for (int i = 0; i < alphaSize; i++)
                {
                    if (len[t][i] > maxLen)
                    {
                        maxLen = len[t][i];
                    }
                    if (len[t][i] < minLen)
                    {
                        minLen = len[t][i];
                    }
                }
                if (maxLen > 20)
                {
                    Panic();
                }
                if (minLen < 1)
                {
                    Panic();
                }
                HbAssignCodes(code[t], len[t], minLen, maxLen, alphaSize);
            }

            /*--- Transmit the mapping table. ---*/
            bool[] inUse16 = new bool[16];
            for (int i = 0; i < 16; ++i)
            {
                inUse16[i] = false;
                for (int j = 0; j < 16; ++j)
                {
                    if (inUse[i * 16 + j])
                    {
                        inUse16[i] = true;
                    }
                }
            }

            for (int i = 0; i < 16; ++i)
            {
                if (inUse16[i])
                {
                    BsW(1, 1);
                }
                else
                {
                    BsW(1, 0);
                }
            }

            for (int i = 0; i < 16; ++i)
            {
                if (inUse16[i])
                {
                    for (int j = 0; j < 16; ++j)
                    {
                        if (inUse[i * 16 + j])
                        {
                            BsW(1, 1);
                        }
                        else
                        {
                            BsW(1, 0);
                        }
                    }
                }
            }

            /*--- Now the selectors. ---*/
            BsW(3, nGroups);
            BsW(15, nSelectors);
            for (int i = 0; i < nSelectors; ++i)
            {
                for (int j = 0; j < selectorMtf[i]; ++j)
                {
                    BsW(1, 1);
                }
                BsW(1, 0);
            }

            /*--- Now the coding tables. ---*/
            for (int t = 0; t < nGroups; ++t)
            {
                int curr = len[t][0];
                BsW(5, curr);
                for (int i = 0; i < alphaSize; ++i)
                {
                    while (curr < len[t][i])
                    {
                        BsW(2, 2);
                        curr++; /* 10 */
                    }
                    while (curr > len[t][i])
                    {
                        BsW(2, 3);
                        curr--; /* 11 */
                    }
                    BsW(1, 0);
                }
            }

            /*--- And finally, the block data proper ---*/
            selCtr = 0;
            gs = 0;
            while (true)
            {
                if (gs >= nMTF)
                {
                    break;
                }
                ge = gs + BZip2Constants.GroupSize - 1;
                if (ge >= nMTF)
                {
                    ge = nMTF - 1;
                }

                for (int i = gs; i <= ge; i++)
                {
                    BsW(len[selector[selCtr]][szptr[i]], code[selector[selCtr]][szptr[i]]);
                }

                gs = ge + 1;
                ++selCtr;
            }
            if (!(selCtr == nSelectors))
            {
                Panic();
            }
        }

        private void MoveToFrontCodeAndSend()
        {
            BsPutIntVS(24, origPtr);
            GenerateMTFValues();
            SendMTFValues();
        }

        private void SimpleSort(int lo, int hi, int d)
        {
            int i, j, h, bigN, hp;
            int v;

            bigN = hi - lo + 1;
            if (bigN < 2)
            {
                return;
            }

            hp = 0;
            while (increments[hp] < bigN)
            {
                hp++;
            }
            hp--;

            for (; hp >= 0; hp--)
            {
                h = increments[hp];

                i = lo + h;
                while (true)
                {
                    /*-- copy 1 --*/
                    if (i > hi)
                        break;
                    v = zptr[i];
                    j = i;
                    while (FullGtU(zptr[j - h] + d, v + d))
                    {
                        zptr[j] = zptr[j - h];
                        j = j - h;
                        if (j <= (lo + h - 1))
                            break;
                    }
                    zptr[j] = v;
                    i++;

                    /*-- copy 2 --*/
                    if (i > hi)
                    {
                        break;
                    }
                    v = zptr[i];
                    j = i;
                    while (FullGtU(zptr[j - h] + d, v + d))
                    {
                        zptr[j] = zptr[j - h];
                        j = j - h;
                        if (j <= (lo + h - 1))
                        {
                            break;
                        }
                    }
                    zptr[j] = v;
                    i++;

                    /*-- copy 3 --*/
                    if (i > hi)
                    {
                        break;
                    }
                    v = zptr[i];
                    j = i;
                    while (FullGtU(zptr[j - h] + d, v + d))
                    {
                        zptr[j] = zptr[j - h];
                        j = j - h;
                        if (j <= (lo + h - 1))
                        {
                            break;
                        }
                    }
                    zptr[j] = v;
                    i++;

                    if (workDone > workLimit && firstAttempt)
                    {
                        return;
                    }
                }
            }
        }

        private void Vswap(int p1, int p2, int n)
        {
            int temp = 0;
            while (n > 0)
            {
                temp = zptr[p1];
                zptr[p1] = zptr[p2];
                zptr[p2] = temp;
                p1++;
                p2++;
                n--;
            }
        }

        private void QSort3(int loSt, int hiSt, int dSt)
        {
            int unLo, unHi, ltLo, gtHi, med, n, m;
            int lo, hi, d;

            StackElement[] stack = new StackElement[QSORT_STACK_SIZE];

            int sp = 0;

            stack[sp].ll = loSt;
            stack[sp].hh = hiSt;
            stack[sp].dd = dSt;
            sp++;

            while (sp > 0)
            {
                if (sp >= QSORT_STACK_SIZE)
                {
                    Panic();
                }

                sp--;
                lo = stack[sp].ll;
                hi = stack[sp].hh;
                d = stack[sp].dd;

                if (hi - lo < SMALL_THRESH || d > DEPTH_THRESH)
                {
                    SimpleSort(lo, hi, d);
                    if (workDone > workLimit && firstAttempt)
                    {
                        return;
                    }
                    continue;
                }

                med = Med3(block[zptr[lo] + d + 1],
                           block[zptr[hi] + d + 1],
                           block[zptr[(lo + hi) >> 1] + d + 1]);

                unLo = ltLo = lo;
                unHi = gtHi = hi;

                while (true)
                {
                    while (true)
                    {
                        if (unLo > unHi)
                        {
                            break;
                        }
                        n = ((int)block[zptr[unLo] + d + 1]) - med;
                        if (n == 0)
                        {
                            int temp = zptr[unLo];
                            zptr[unLo] = zptr[ltLo];
                            zptr[ltLo] = temp;
                            ltLo++;
                            unLo++;
                            continue;
                        }
                        if (n > 0)
                        {
                            break;
                        }
                        unLo++;
                    }

                    while (true)
                    {
                        if (unLo > unHi)
                        {
                            break;
                        }
                        n = ((int)block[zptr[unHi] + d + 1]) - med;
                        if (n == 0)
                        {
                            int temp = zptr[unHi];
                            zptr[unHi] = zptr[gtHi];
                            zptr[gtHi] = temp;
                            gtHi--;
                            unHi--;
                            continue;
                        }
                        if (n < 0)
                        {
                            break;
                        }
                        unHi--;
                    }

                    if (unLo > unHi)
                    {
                        break;
                    }

                    {
                        int temp = zptr[unLo];
                        zptr[unLo] = zptr[unHi];
                        zptr[unHi] = temp;
                        unLo++;
                        unHi--;
                    }
                }

                if (gtHi < ltLo)
                {
                    stack[sp].ll = lo;
                    stack[sp].hh = hi;
                    stack[sp].dd = d + 1;
                    sp++;
                    continue;
                }

                n = ((ltLo - lo) < (unLo - ltLo)) ? (ltLo - lo) : (unLo - ltLo);
                Vswap(lo, unLo - n, n);
                m = ((hi - gtHi) < (gtHi - unHi)) ? (hi - gtHi) : (gtHi - unHi);
                Vswap(unLo, hi - m + 1, m);

                n = lo + unLo - ltLo - 1;
                m = hi - (gtHi - unHi) + 1;

                stack[sp].ll = lo;
                stack[sp].hh = n;
                stack[sp].dd = d;
                sp++;

                stack[sp].ll = n + 1;
                stack[sp].hh = m - 1;
                stack[sp].dd = d + 1;
                sp++;

                stack[sp].ll = m;
                stack[sp].hh = hi;
                stack[sp].dd = d;
                sp++;
            }
        }

        private void MainSort()
        {
            int i, j, ss, sb;
            int[] runningOrder = new int[256];
            int[] copy = new int[256];
            bool[] bigDone = new bool[256];
            int c1, c2;
            int numQSorted;

            /*--
			In the various block-sized structures, live data runs
			from 0 to last+NUM_OVERSHOOT_BYTES inclusive.  First,
			set up the overshoot area for block.
			--*/

            //   if (verbosity >= 4) fprintf ( stderr, "        sort initialise ...\n" );
            for (i = 0; i < BZip2Constants.OvershootBytes; i++)
            {
                block[last + i + 2] = block[(i % (last + 1)) + 1];
            }
            for (i = 0; i <= last + BZip2Constants.OvershootBytes; i++)
            {
                quadrant[i] = 0;
            }

            block[0] = (byte)(block[last + 1]);

            if (last < 4000)
            {
                /*--
				Use simpleSort(), since the full sorting mechanism
				has quite a large constant overhead.
				--*/
                for (i = 0; i <= last; i++)
                {
                    zptr[i] = i;
                }
                firstAttempt = false;
                workDone = workLimit = 0;
                SimpleSort(0, last, 0);
            }
            else
            {
                numQSorted = 0;
                for (i = 0; i <= 255; i++)
                {
                    bigDone[i] = false;
                }
                for (i = 0; i <= 65536; i++)
                {
                    ftab[i] = 0;
                }

                c1 = block[0];
                for (i = 0; i <= last; i++)
                {
                    c2 = block[i + 1];
                    ftab[(c1 << 8) + c2]++;
                    c1 = c2;
                }

                for (i = 1; i <= 65536; i++)
                {
                    ftab[i] += ftab[i - 1];
                }

                c1 = block[1];
                for (i = 0; i < last; i++)
                {
                    c2 = block[i + 2];
                    j = (c1 << 8) + c2;
                    c1 = c2;
                    ftab[j]--;
                    zptr[ftab[j]] = i;
                }

                j = ((block[last + 1]) << 8) + (block[1]);
                ftab[j]--;
                zptr[ftab[j]] = last;

                /*--
				Now ftab contains the first loc of every small bucket.
				Calculate the running order, from smallest to largest
				big bucket.
				--*/

                for (i = 0; i <= 255; i++)
                {
                    runningOrder[i] = i;
                }

                int vv;
                int h = 1;
                do
                {
                    h = 3 * h + 1;
                } while (h <= 256);
                do
                {
                    h = h / 3;
                    for (i = h; i <= 255; i++)
                    {
                        vv = runningOrder[i];
                        j = i;
                        while ((ftab[((runningOrder[j - h]) + 1) << 8] - ftab[(runningOrder[j - h]) << 8]) > (ftab[((vv) + 1) << 8] - ftab[(vv) << 8]))
                        {
                            runningOrder[j] = runningOrder[j - h];
                            j = j - h;
                            if (j <= (h - 1))
                            {
                                break;
                            }
                        }
                        runningOrder[j] = vv;
                    }
                } while (h != 1);

                /*--
				The main sorting loop.
				--*/
                for (i = 0; i <= 255; i++)
                {
                    /*--
					Process big buckets, starting with the least full.
					--*/
                    ss = runningOrder[i];

                    /*--
					Complete the big bucket [ss] by quicksorting
					any unsorted small buckets [ss, j].  Hopefully
					previous pointer-scanning phases have already
					completed many of the small buckets [ss, j], so
					we don't have to sort them at all.
					--*/
                    for (j = 0; j <= 255; j++)
                    {
                        sb = (ss << 8) + j;
                        if (!((ftab[sb] & SETMASK) == SETMASK))
                        {
                            int lo = ftab[sb] & CLEARMASK;
                            int hi = (ftab[sb + 1] & CLEARMASK) - 1;
                            if (hi > lo)
                            {
                                QSort3(lo, hi, 2);
                                numQSorted += (hi - lo + 1);
                                if (workDone > workLimit && firstAttempt)
                                {
                                    return;
                                }
                            }
                            ftab[sb] |= SETMASK;
                        }
                    }

                    /*--
					The ss big bucket is now done.  Record this fact,
					and update the quadrant descriptors.  Remember to
					update quadrants in the overshoot area too, if
					necessary.  The "if (i < 255)" test merely skips
					this updating for the last bucket processed, since
					updating for the last bucket is pointless.
					--*/
                    bigDone[ss] = true;

                    if (i < 255)
                    {
                        int bbStart = ftab[ss << 8] & CLEARMASK;
                        int bbSize = (ftab[(ss + 1) << 8] & CLEARMASK) - bbStart;
                        int shifts = 0;

                        while ((bbSize >> shifts) > 65534)
                        {
                            shifts++;
                        }

                        for (j = 0; j < bbSize; j++)
                        {
                            int a2update = zptr[bbStart + j];
                            int qVal = (j >> shifts);
                            quadrant[a2update] = qVal;
                            if (a2update < BZip2Constants.OvershootBytes)
                            {
                                quadrant[a2update + last + 1] = qVal;
                            }
                        }

                        if (!(((bbSize - 1) >> shifts) <= 65535))
                        {
                            Panic();
                        }
                    }

                    /*--
					Now scan this big bucket so as to synthesise the
					sorted order for small buckets [t, ss] for all t != ss.
					--*/
                    for (j = 0; j <= 255; j++)
                    {
                        copy[j] = ftab[(j << 8) + ss] & CLEARMASK;
                    }

                    for (j = ftab[ss << 8] & CLEARMASK; j < (ftab[(ss + 1) << 8] & CLEARMASK); j++)
                    {
                        c1 = block[zptr[j]];
                        if (!bigDone[c1])
                        {
                            zptr[copy[c1]] = zptr[j] == 0 ? last : zptr[j] - 1;
                            copy[c1]++;
                        }
                    }

                    for (j = 0; j <= 255; j++)
                    {
                        ftab[(j << 8) + ss] |= SETMASK;
                    }
                }
            }
        }

        private void RandomiseBlock()
        {
            int i;
            int rNToGo = 0;
            int rTPos = 0;
            for (i = 0; i < 256; i++)
            {
                inUse[i] = false;
            }

            for (i = 0; i <= last; i++)
            {
                if (rNToGo == 0)
                {
                    rNToGo = (int)BZip2Constants.RandomNumbers[rTPos];
                    rTPos++;
                    if (rTPos == 512)
                    {
                        rTPos = 0;
                    }
                }
                rNToGo--;
                block[i + 1] ^= (byte)((rNToGo == 1) ? 1 : 0);
                // handle 16 bit signed numbers
                block[i + 1] &= 0xFF;

                inUse[block[i + 1]] = true;
            }
        }

        private void DoReversibleTransformation()
        {
            workLimit = workFactor * last;
            workDone = 0;
            blockRandomised = false;
            firstAttempt = true;

            MainSort();

            if (workDone > workLimit && firstAttempt)
            {
                RandomiseBlock();
                workLimit = workDone = 0;
                blockRandomised = true;
                firstAttempt = false;
                MainSort();
            }

            origPtr = -1;
            for (int i = 0; i <= last; i++)
            {
                if (zptr[i] == 0)
                {
                    origPtr = i;
                    break;
                }
            }

            if (origPtr == -1)
            {
                Panic();
            }
        }

        private bool FullGtU(int i1, int i2)
        {
            int k;
            byte c1, c2;
            int s1, s2;

            c1 = block[i1 + 1];
            c2 = block[i2 + 1];
            if (c1 != c2)
            {
                return c1 > c2;
            }
            i1++;
            i2++;

            c1 = block[i1 + 1];
            c2 = block[i2 + 1];
            if (c1 != c2)
            {
                return c1 > c2;
            }
            i1++;
            i2++;

            c1 = block[i1 + 1];
            c2 = block[i2 + 1];
            if (c1 != c2)
            {
                return c1 > c2;
            }
            i1++;
            i2++;

            c1 = block[i1 + 1];
            c2 = block[i2 + 1];
            if (c1 != c2)
            {
                return c1 > c2;
            }
            i1++;
            i2++;

            c1 = block[i1 + 1];
            c2 = block[i2 + 1];
            if (c1 != c2)
            {
                return c1 > c2;
            }
            i1++;
            i2++;

            c1 = block[i1 + 1];
            c2 = block[i2 + 1];
            if (c1 != c2)
            {
                return c1 > c2;
            }
            i1++;
            i2++;

            k = last + 1;

            do
            {
                c1 = block[i1 + 1];
                c2 = block[i2 + 1];
                if (c1 != c2)
                {
                    return c1 > c2;
                }
                s1 = quadrant[i1];
                s2 = quadrant[i2];
                if (s1 != s2)
                {
                    return s1 > s2;
                }
                i1++;
                i2++;

                c1 = block[i1 + 1];
                c2 = block[i2 + 1];
                if (c1 != c2)
                {
                    return c1 > c2;
                }
                s1 = quadrant[i1];
                s2 = quadrant[i2];
                if (s1 != s2)
                {
                    return s1 > s2;
                }
                i1++;
                i2++;

                c1 = block[i1 + 1];
                c2 = block[i2 + 1];
                if (c1 != c2)
                {
                    return c1 > c2;
                }
                s1 = quadrant[i1];
                s2 = quadrant[i2];
                if (s1 != s2)
                {
                    return s1 > s2;
                }
                i1++;
                i2++;

                c1 = block[i1 + 1];
                c2 = block[i2 + 1];
                if (c1 != c2)
                {
                    return c1 > c2;
                }
                s1 = quadrant[i1];
                s2 = quadrant[i2];
                if (s1 != s2)
                {
                    return s1 > s2;
                }
                i1++;
                i2++;

                if (i1 > last)
                {
                    i1 -= last;
                    i1--;
                }
                if (i2 > last)
                {
                    i2 -= last;
                    i2--;
                }

                k -= 4;
                ++workDone;
            } while (k >= 0);

            return false;
        }

        private void AllocateCompressStructures()
        {
            int n = BZip2Constants.BaseBlockSize * blockSize100k;
            block = new byte[(n + 1 + BZip2Constants.OvershootBytes)];
            quadrant = new int[(n + BZip2Constants.OvershootBytes)];
            zptr = new int[n];
            ftab = new int[65537];

            if (block == null || quadrant == null || zptr == null || ftab == null)
            {
                //		int totalDraw = (n + 1 + NUM_OVERSHOOT_BYTES) + (n + NUM_OVERSHOOT_BYTES) + n + 65537;
                //		compressOutOfMemory ( totalDraw, n );
            }

            /*
			The back end needs a place to store the MTF values
			whilst it calculates the coding tables.  We could
			put them in the zptr array.  However, these values
			will fit in a short, so we overlay szptr at the
			start of zptr, in the hope of reducing the number
			of cache misses induced by the multiple traversals
			of the MTF values when calculating coding tables.
			Seems to improve compression speed by about 1%.
			*/
            //	szptr = zptr;

            szptr = new short[2 * n];
        }

        private void GenerateMTFValues()
        {
            char[] yy = new char[256];
            int i, j;
            char tmp;
            char tmp2;
            int zPend;
            int wr;
            int EOB;

            MakeMaps();
            EOB = nInUse + 1;

            for (i = 0; i <= EOB; i++)
            {
                mtfFreq[i] = 0;
            }

            wr = 0;
            zPend = 0;
            for (i = 0; i < nInUse; i++)
            {
                yy[i] = (char)i;
            }

            for (i = 0; i <= last; i++)
            {
                char ll_i;

                ll_i = unseqToSeq[block[zptr[i]]];

                j = 0;
                tmp = yy[j];
                while (ll_i != tmp)
                {
                    j++;
                    tmp2 = tmp;
                    tmp = yy[j];
                    yy[j] = tmp2;
                }
                yy[0] = tmp;

                if (j == 0)
                {
                    zPend++;
                }
                else
                {
                    if (zPend > 0)
                    {
                        zPend--;
                        while (true)
                        {
                            switch (zPend % 2)
                            {
                                case 0:
                                    szptr[wr] = (short)BZip2Constants.RunA;
                                    wr++;
                                    mtfFreq[BZip2Constants.RunA]++;
                                    break;

                                case 1:
                                    szptr[wr] = (short)BZip2Constants.RunB;
                                    wr++;
                                    mtfFreq[BZip2Constants.RunB]++;
                                    break;
                            }
                            if (zPend < 2)
                            {
                                break;
                            }
                            zPend = (zPend - 2) / 2;
                        }
                        zPend = 0;
                    }
                    szptr[wr] = (short)(j + 1);
                    wr++;
                    mtfFreq[j + 1]++;
                }
            }

            if (zPend > 0)
            {
                zPend--;
                while (true)
                {
                    switch (zPend % 2)
                    {
                        case 0:
                            szptr[wr] = (short)BZip2Constants.RunA;
                            wr++;
                            mtfFreq[BZip2Constants.RunA]++;
                            break;

                        case 1:
                            szptr[wr] = (short)BZip2Constants.RunB;
                            wr++;
                            mtfFreq[BZip2Constants.RunB]++;
                            break;
                    }
                    if (zPend < 2)
                    {
                        break;
                    }
                    zPend = (zPend - 2) / 2;
                }
            }

            szptr[wr] = (short)EOB;
            wr++;
            mtfFreq[EOB]++;

            nMTF = wr;
        }

        private static void Panic()
        {
            throw new BZip2Exception("BZip2 output stream panic");
        }

        private static void HbMakeCodeLengths(char[] len, int[] freq, int alphaSize, int maxLen)
        {
            /*--
			Nodes and heap entries run from 1.  Entry 0
			for both the heap and nodes is a sentinel.
			--*/
            int nNodes, nHeap, n1, n2, j, k;
            bool tooLong;

            int[] heap = new int[BZip2Constants.MaximumAlphaSize + 2];
            int[] weight = new int[BZip2Constants.MaximumAlphaSize * 2];
            int[] parent = new int[BZip2Constants.MaximumAlphaSize * 2];

            for (int i = 0; i < alphaSize; ++i)
            {
                weight[i + 1] = (freq[i] == 0 ? 1 : freq[i]) << 8;
            }

            while (true)
            {
                nNodes = alphaSize;
                nHeap = 0;

                heap[0] = 0;
                weight[0] = 0;
                parent[0] = -2;

                for (int i = 1; i <= alphaSize; ++i)
                {
                    parent[i] = -1;
                    nHeap++;
                    heap[nHeap] = i;
                    int zz = nHeap;
                    int tmp = heap[zz];
                    while (weight[tmp] < weight[heap[zz >> 1]])
                    {
                        heap[zz] = heap[zz >> 1];
                        zz >>= 1;
                    }
                    heap[zz] = tmp;
                }
                if (!(nHeap < (BZip2Constants.MaximumAlphaSize + 2)))
                {
                    Panic();
                }

                while (nHeap > 1)
                {
                    n1 = heap[1];
                    heap[1] = heap[nHeap];
                    nHeap--;
                    int zz = 1;
                    int yy = 0;
                    int tmp = heap[zz];
                    while (true)
                    {
                        yy = zz << 1;
                        if (yy > nHeap)
                        {
                            break;
                        }
                        if (yy < nHeap && weight[heap[yy + 1]] < weight[heap[yy]])
                        {
                            yy++;
                        }
                        if (weight[tmp] < weight[heap[yy]])
                        {
                            break;
                        }

                        heap[zz] = heap[yy];
                        zz = yy;
                    }
                    heap[zz] = tmp;
                    n2 = heap[1];
                    heap[1] = heap[nHeap];
                    nHeap--;

                    zz = 1;
                    yy = 0;
                    tmp = heap[zz];
                    while (true)
                    {
                        yy = zz << 1;
                        if (yy > nHeap)
                        {
                            break;
                        }
                        if (yy < nHeap && weight[heap[yy + 1]] < weight[heap[yy]])
                        {
                            yy++;
                        }
                        if (weight[tmp] < weight[heap[yy]])
                        {
                            break;
                        }
                        heap[zz] = heap[yy];
                        zz = yy;
                    }
                    heap[zz] = tmp;
                    nNodes++;
                    parent[n1] = parent[n2] = nNodes;

                    weight[nNodes] = (int)((weight[n1] & 0xffffff00) + (weight[n2] & 0xffffff00)) |
                        (int)(1 + (((weight[n1] & 0x000000ff) > (weight[n2] & 0x000000ff)) ? (weight[n1] & 0x000000ff) : (weight[n2] & 0x000000ff)));

                    parent[nNodes] = -1;
                    nHeap++;
                    heap[nHeap] = nNodes;

                    zz = nHeap;
                    tmp = heap[zz];
                    while (weight[tmp] < weight[heap[zz >> 1]])
                    {
                        heap[zz] = heap[zz >> 1];
                        zz >>= 1;
                    }
                    heap[zz] = tmp;
                }
                if (!(nNodes < (BZip2Constants.MaximumAlphaSize * 2)))
                {
                    Panic();
                }

                tooLong = false;
                for (int i = 1; i <= alphaSize; ++i)
                {
                    j = 0;
                    k = i;
                    while (parent[k] >= 0)
                    {
                        k = parent[k];
                        j++;
                    }
                    len[i - 1] = (char)j;
                    tooLong |= j > maxLen;
                }

                if (!tooLong)
                {
                    break;
                }

                for (int i = 1; i < alphaSize; ++i)
                {
                    j = weight[i] >> 8;
                    j = 1 + (j / 2);
                    weight[i] = j << 8;
                }
            }
        }

        private static void HbAssignCodes(int[] code, char[] length, int minLen, int maxLen, int alphaSize)
        {
            int vec = 0;
            for (int n = minLen; n <= maxLen; ++n)
            {
                for (int i = 0; i < alphaSize; ++i)
                {
                    if (length[i] == n)
                    {
                        code[i] = vec;
                        ++vec;
                    }
                }
                vec <<= 1;
            }
        }

        private static byte Med3(byte a, byte b, byte c)
        {
            byte t;
            if (a > b)
            {
                t = a;
                a = b;
                b = t;
            }
            if (b > c)
            {
                t = b;
                b = c;
                c = t;
            }
            if (a > b)
            {
                b = a;
            }
            return b;
        }

        private struct StackElement
        {
            public int ll;
            public int hh;
            public int dd;
        }
    }
}
