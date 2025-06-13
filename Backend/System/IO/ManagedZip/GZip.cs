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

namespace System.IO.ManagedZip.GZip
{
    using System;
    using System.IO;
    using System.Text;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using System.IO.ManagedZip.Core;
    using System.Runtime.Serialization;
    using System.IO.ManagedZip.Checksum;
    using System.IO.ManagedZip.Zip.Compression;
    using System.IO.ManagedZip.Zip.Compression.Streams;
    using static System.IO.ManagedZip.Zip.Compression.Deflater;

    /// <summary>
    /// An example class to demonstrate compression and decompression of GZip streams.
    /// </summary>
    public static class GZip
    {
        /// <summary>
        /// Decompress the <paramref name="inStream">input</paramref> writing
        /// uncompressed data to the <paramref name="outStream">output stream</paramref>
        /// </summary>
        /// <param name="inStream">The readable stream containing data to decompress.</param>
        /// <param name="outStream">The output stream to receive the decompressed data.</param>
        /// <param name="isStreamOwner">Both streams are closed on completion if true.</param>
        /// <exception cref="ArgumentNullException">Input or output stream is null</exception>
        public static void Decompress(Stream inStream, Stream outStream, bool isStreamOwner)
        {
            if (inStream == null)
                throw new ArgumentNullException(nameof(inStream), "Input stream is null");

            if (outStream == null)
                throw new ArgumentNullException(nameof(outStream), "Output stream is null");

            try
            {
                using (GZipInputStream gzipInput = new GZipInputStream(inStream))
                {
                    gzipInput.IsStreamOwner = isStreamOwner;
                    Core.StreamUtils.Copy(gzipInput, outStream, new byte[4096]);
                }
            }
            finally
            {
                if (isStreamOwner)
                {
                    // inStream is closed by the GZipInputStream if stream owner
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
        /// <param name="bufferSize">Deflate buffer size, minimum 512</param>
        /// <param name="level">Deflate compression level, 0-9</param>
        /// <exception cref="ArgumentNullException">Input or output stream is null</exception>
        /// <exception cref="ArgumentOutOfRangeException">Buffer Size is smaller than 512</exception>
        /// <exception cref="ArgumentOutOfRangeException">Compression level outside 0-9</exception>
        public static void Compress(Stream inStream, Stream outStream, bool isStreamOwner, int bufferSize = 512, int level = 6)
        {
            if (inStream == null)
                throw new ArgumentNullException(nameof(inStream), "Input stream is null");

            if (outStream == null)
                throw new ArgumentNullException(nameof(outStream), "Output stream is null");

            if (bufferSize < 512)
                throw new ArgumentOutOfRangeException(nameof(bufferSize), "Deflate buffer size must be >= 512");

            if (level < NO_COMPRESSION || level > BEST_COMPRESSION)
                throw new ArgumentOutOfRangeException(nameof(level), "Compression level must be 0-9");

            try
            {
                using (GZipOutputStream gzipOutput = new GZipOutputStream(outStream, bufferSize))
                {
                    gzipOutput.SetLevel(level);
                    gzipOutput.IsStreamOwner = isStreamOwner;
                    Core.StreamUtils.Copy(inStream, gzipOutput, new byte[bufferSize]);
                }
            }
            finally
            {
                if (isStreamOwner)
                {
                    // outStream is closed by the GZipOutputStream if stream owner
                    inStream.Dispose();
                }
            }
        }
    }

    /// <summary>
    /// This class contains constants used for gzip.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Naming", "CA1707:Identifiers should not contain underscores", Justification = "kept for backwards compatibility")]
    public sealed class GZipConstants
    {
        /// <summary>
        /// First GZip identification byte
        /// </summary>
        public const byte ID1 = 0x1F;

        /// <summary>
        /// Second GZip identification byte
        /// </summary>
        public const byte ID2 = 0x8B;

        /// <summary>
        /// Deflate compression method
        /// </summary>
        public const byte CompressionMethodDeflate = 0x8;

        /// <summary>
        /// Get the GZip specified encoding (CP-1252 if supported, otherwise ASCII)
        /// </summary>
        public static Encoding Encoding
        {
            get
            {
                try
                {
                    return Encoding.GetEncoding(1252);
                }
                catch
                {
                    return Encoding.ASCII;
                }
            }
        }

    }

    /// <summary>
    /// GZip header flags
    /// </summary>
    [Flags]
    public enum GZipFlags : byte
    {
        /// <summary>
        /// Text flag hinting that the file is in ASCII
        /// </summary>
        FTEXT = 0x1 << 0,

        /// <summary>
        /// CRC flag indicating that a CRC16 preceeds the data
        /// </summary>
        FHCRC = 0x1 << 1,

        /// <summary>
        /// Extra flag indicating that extra fields are present
        /// </summary>
        FEXTRA = 0x1 << 2,

        /// <summary>
        /// Filename flag indicating that the original filename is present
        /// </summary>
        FNAME = 0x1 << 3,

        /// <summary>
        /// Flag bit mask indicating that a comment is present
        /// </summary>
        FCOMMENT = 0x1 << 4,
    }

    /// <summary>
    /// GZipException represents exceptions specific to GZip classes and code.
    /// </summary>
    [Serializable]
    public class GZipException : SharpZipBaseException
    {
        /// <summary>
        /// Initialise a new instance of <see cref="GZipException" />.
        /// </summary>
        public GZipException()
        {
        }

        /// <summary>
        /// Initialise a new instance of <see cref="GZipException" /> with its message string.
        /// </summary>
        /// <param name="message">A <see cref="string"/> that describes the error.</param>
        public GZipException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initialise a new instance of <see cref="GZipException" />.
        /// </summary>
        /// <param name="message">A <see cref="string"/> that describes the error.</param>
        /// <param name="innerException">The <see cref="Exception"/> that caused this exception.</param>
        public GZipException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }

    /// <summary>
    /// This filter stream is used to decompress a "GZIP" format stream.
    /// The "GZIP" format is described baseInputStream RFC 1952.
    ///
    /// author of the original java version : John Leuner
    /// </summary>
    /// <example> This sample shows how to unzip a gzipped file
    /// <code>
    /// using System;
    /// using System.IO;
    ///
    /// using System.IO.ManagedZip.Core;
    /// using System.IO.ManagedZip.GZip;
    ///
    /// class MainClass
    /// {
    /// 	public static void Main(string[] args)
    /// 	{
    ///			using (Stream inStream = new GZipInputStream(File.OpenRead(args[0])))
    ///			using (FileStream outStream = File.Create(Path.GetFileNameWithoutExtension(args[0]))) {
    ///				byte[] buffer = new byte[4096];
    ///				StreamUtils.Copy(inStream, outStream, buffer);
    /// 		}
    /// 	}
    /// }
    /// </code>
    /// </example>
    public class GZipInputStream : InflaterInputStream
    {
        #region Instance Fields

        /// <summary>
        /// CRC-32 value for uncompressed data
        /// </summary>
        protected Crc32 crc;

        /// <summary>
        /// Flag to indicate if we've read the GZIP header yet for the current member (block of compressed data).
        /// This is tracked per-block as the file is parsed.
        /// </summary>
        private bool readGZIPHeader;

        /// <summary>
        /// Flag to indicate if at least one block in a stream with concatenated blocks was read successfully.
        /// This allows us to exit gracefully if downstream data is not in gzip format.
        /// </summary>
        private bool completedLastBlock;

        private string fileName;

        #endregion Instance Fields

        #region Constructors

        /// <summary>
        /// Creates a GZipInputStream with the default buffer size
        /// </summary>
        /// <param name="baseInputStream">
        /// The stream to read compressed data from (baseInputStream GZIP format)
        /// </param>
        public GZipInputStream(Stream baseInputStream)
            : this(baseInputStream, 4096)
        {
        }

        /// <summary>
        /// Creates a GZIPInputStream with the specified buffer size
        /// </summary>
        /// <param name="baseInputStream">
        /// The stream to read compressed data from (baseInputStream GZIP format)
        /// </param>
        /// <param name="size">
        /// Size of the buffer to use
        /// </param>
        public GZipInputStream(Stream baseInputStream, int size)
            : base(baseInputStream, InflaterPool.Instance.Rent(true), size)
        {
        }

        #endregion Constructors

        #region Stream overrides

        /// <summary>
        /// Reads uncompressed data into an array of bytes
        /// </summary>
        /// <param name="buffer">
        /// The buffer to read uncompressed data into
        /// </param>
        /// <param name="offset">
        /// The offset indicating where the data should be placed
        /// </param>
        /// <param name="count">
        /// The number of uncompressed bytes to be read
        /// </param>
        /// <returns>Returns the number of bytes actually read.</returns>
        public override int Read(byte[] buffer, int offset, int count)
        {
            // A GZIP file can contain multiple blocks of compressed data, although this is quite rare.
            // A compressed block could potentially be empty, so we need to loop until we reach EOF or
            // we find data.
            while (true)
            {
                // If we haven't read the header for this block, read it
                if (!readGZIPHeader)
                {
                    // Try to read header. If there is no header (0 bytes available), this is EOF. If there is
                    // an incomplete header, this will throw an exception.
                    try
                    {
                        if (!ReadHeader())
                        {
                            return 0;
                        }
                    }
                    catch (Exception ex) when (completedLastBlock && (ex is GZipException || ex is EndOfStreamException))
                    {
                        // if we completed the last block (i.e. we're in a stream that has multiple blocks concatenated
                        // we want to return gracefully from any header parsing exceptions since sometimes there may
                        // be trailing garbage on a stream
                        return 0;
                    }
                }

                // Try to read compressed data
                int bytesRead = base.Read(buffer, offset, count);
                if (bytesRead > 0)
                {
                    crc.Update(new ArraySegment<byte>(buffer, offset, bytesRead));
                }

                // If this is the end of stream, read the footer
                if (inf.IsFinished)
                {
                    ReadFooter();
                }

                // Attempting to read 0 bytes will never yield any bytesRead, so we return instead of looping forever
                if (bytesRead > 0 || count == 0)
                {
                    return bytesRead;
                }
            }
        }

        /// <summary>
        /// Retrieves the filename header field for the block last read
        /// </summary>
        /// <returns></returns>
        public string GetFilename()
        {
            return fileName;
        }

        #endregion Stream overrides

        #region Support routines

        private bool ReadHeader()
        {
            // Initialize CRC for this block
            crc = new Crc32();

            // Make sure there is data in file. We can't rely on ReadLeByte() to fill the buffer, as this could be EOF,
            // which is fine, but ReadLeByte() throws an exception if it doesn't find data, so we do this part ourselves.
            if (inputBuffer.Available <= 0)
            {
                inputBuffer.Fill();
                if (inputBuffer.Available <= 0)
                {
                    // No header, EOF.
                    return false;
                }
            }

            var headCRC = new Crc32();

            // 1. Check the two magic bytes

            var magic = inputBuffer.ReadLeByte();
            headCRC.Update(magic);
            if (magic != GZipConstants.ID1)
            {
                throw new GZipException("Error GZIP header, first magic byte doesn't match");
            }

            magic = inputBuffer.ReadLeByte();
            if (magic != GZipConstants.ID2)
            {
                throw new GZipException("Error GZIP header,  second magic byte doesn't match");
            }
            headCRC.Update(magic);

            // 2. Check the compression type (must be 8)
            var compressionType = inputBuffer.ReadLeByte();

            if (compressionType != GZipConstants.CompressionMethodDeflate)
            {
                throw new GZipException("Error GZIP header, data not in deflate format");
            }
            headCRC.Update(compressionType);

            // 3. Check the flags
            var flagsByte = inputBuffer.ReadLeByte();

            headCRC.Update(flagsByte);

            // 3.1 Check the reserved bits are zero

            if ((flagsByte & 0xE0) != 0)
            {
                throw new GZipException("Reserved flag bits in GZIP header != 0");
            }

            var flags = (GZipFlags)flagsByte;

            // 4.-6. Skip the modification time, extra flags, and OS type
            for (int i = 0; i < 6; i++)
            {
                headCRC.Update(inputBuffer.ReadLeByte());
            }

            // 7. Read extra field
            if (flags.HasFlag(GZipFlags.FEXTRA))
            {
                // XLEN is total length of extra subfields, we will skip them all
                var len1 = inputBuffer.ReadLeByte();
                var len2 = inputBuffer.ReadLeByte();

                headCRC.Update(len1);
                headCRC.Update(len2);

                int extraLen = (len2 << 8) | len1;      // gzip is LSB first
                for (int i = 0; i < extraLen; i++)
                {
                    headCRC.Update(inputBuffer.ReadLeByte());
                }
            }

            // 8. Read file name
            if (flags.HasFlag(GZipFlags.FNAME))
            {
                var fname = new byte[1024];
                var fnamePos = 0;
                int readByte;
                while ((readByte = inputBuffer.ReadLeByte()) > 0)
                {
                    if (fnamePos < 1024)
                    {
                        fname[fnamePos++] = (byte)readByte;
                    }
                    headCRC.Update(readByte);
                }

                headCRC.Update(readByte);

                fileName = GZipConstants.Encoding.GetString(fname, 0, fnamePos);
            }
            else
            {
                fileName = null;
            }

            // 9. Read comment
            if (flags.HasFlag(GZipFlags.FCOMMENT))
            {
                int readByte;
                while ((readByte = inputBuffer.ReadLeByte()) > 0)
                {
                    headCRC.Update(readByte);
                }

                headCRC.Update(readByte);
            }

            // 10. Read header CRC
            if (flags.HasFlag(GZipFlags.FHCRC))
            {
                int tempByte;
                int crcval = inputBuffer.ReadLeByte();
                if (crcval < 0)
                {
                    throw new EndOfStreamException("EOS reading GZIP header");
                }

                tempByte = inputBuffer.ReadLeByte();
                if (tempByte < 0)
                {
                    throw new EndOfStreamException("EOS reading GZIP header");
                }

                crcval = (crcval << 8) | tempByte;
                if (crcval != ((int)headCRC.Value & 0xffff))
                {
                    throw new GZipException("Header CRC value mismatch");
                }
            }

            readGZIPHeader = true;
            return true;
        }

        private void ReadFooter()
        {
            byte[] footer = new byte[8];

            // End of stream; reclaim all bytes from inf, read the final byte count, and reset the inflator
            long bytesRead = inf.TotalOut & 0xffffffff;
            inputBuffer.Available += inf.RemainingInput;
            inf.Reset();

            // Read footer from inputBuffer
            int needed = 8;
            while (needed > 0)
            {
                int count = inputBuffer.ReadClearTextBuffer(footer, 8 - needed, needed);
                if (count <= 0)
                {
                    throw new EndOfStreamException("EOS reading GZIP footer");
                }
                needed -= count; // Jewel Jan 16
            }

            // Calculate CRC
            int crcval = (footer[0] & 0xff) | ((footer[1] & 0xff) << 8) | ((footer[2] & 0xff) << 16) | (footer[3] << 24);
            if (crcval != (int)crc.Value)
            {
                throw new GZipException($"GZIP crc sum mismatch, theirs \"{crcval:x8}\" and ours \"{(int)crc.Value:x8}\"");
            }

            // NOTE The total here is the original total modulo 2 ^ 32.
            uint total =
                (uint)((uint)footer[4] & 0xff) |
                (uint)(((uint)footer[5] & 0xff) << 8) |
                (uint)(((uint)footer[6] & 0xff) << 16) |
                (uint)((uint)footer[7] << 24);

            if (bytesRead != total)
            {
                throw new GZipException("Number of bytes mismatch in footer");
            }

            // Mark header read as false so if another header exists, we'll continue reading through the file
            readGZIPHeader = false;

            // Indicate that we succeeded on at least one block so we can exit gracefully if there is trailing garbage downstream
            completedLastBlock = true;
        }

        #endregion Support routines
    }

    /// <summary>
    /// This filter stream is used to compress a stream into a "GZIP" stream.
    /// The "GZIP" format is described in RFC 1952.
    ///
    /// author of the original java version : John Leuner
    /// </summary>
    /// <example> This sample shows how to gzip a file
    /// <code>
    /// using System;
    /// using System.IO;
    ///
    /// using System.IO.ManagedZip.GZip;
    /// using System.IO.ManagedZip.Core;
    ///
    /// class MainClass
    /// {
    /// 	public static void Main(string[] args)
    /// 	{
    /// 			using (Stream s = new GZipOutputStream(File.Create(args[0] + ".gz")))
    /// 			using (FileStream fs = File.OpenRead(args[0])) {
    /// 				byte[] writeData = new byte[4096];
    /// 				Streamutils.Copy(s, fs, writeData);
    /// 			}
    /// 		}
    /// 	}
    /// }
    /// </code>
    /// </example>
    public class GZipOutputStream : DeflaterOutputStream
    {
        private enum OutputState
        {
            Header,
            Footer,
            Finished,
            Closed,
        };

        #region Instance Fields

        /// <summary>
        /// CRC-32 value for uncompressed data
        /// </summary>
        protected Crc32 crc = new Crc32();

        private OutputState state_ = OutputState.Header;

        private string fileName;

        private GZipFlags flags = 0;

        #endregion Instance Fields

        #region Constructors

        /// <summary>
        /// Creates a GzipOutputStream with the default buffer size
        /// </summary>
        /// <param name="baseOutputStream">
        /// The stream to read data (to be compressed) from
        /// </param>
        public GZipOutputStream(Stream baseOutputStream)
            : this(baseOutputStream, 4096)
        {
        }

        /// <summary>
        /// Creates a GZipOutputStream with the specified buffer size
        /// </summary>
        /// <param name="baseOutputStream">
        /// The stream to read data (to be compressed) from
        /// </param>
        /// <param name="size">
        /// Size of the buffer to use
        /// </param>
        public GZipOutputStream(Stream baseOutputStream, int size) : base(baseOutputStream, new Deflater(Deflater.DEFAULT_COMPRESSION, true), size)
        {
        }

        #endregion Constructors

        #region Public API

        /// <summary>
        /// Sets the active compression level (0-9).  The new level will be activated
        /// immediately.
        /// </summary>
        /// <param name="level">The compression level to set.</param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// Level specified is not supported.
        /// </exception>
        /// <see cref="Deflater"/>
        public void SetLevel(int level)
        {
            if (level < Deflater.NO_COMPRESSION || level > Deflater.BEST_COMPRESSION)
                throw new ArgumentOutOfRangeException(nameof(level), "Compression level must be 0-9");

            deflater_.SetLevel(level);
        }

        /// <summary>
        /// Get the current compression level.
        /// </summary>
        /// <returns>The current compression level.</returns>
        public int GetLevel()
        {
            return deflater_.GetLevel();
        }

        /// <summary>
        /// Original filename
        /// </summary>
        public string FileName
        {
            get => fileName;
            set
            {
                fileName = CleanFilename(value);
                if (string.IsNullOrEmpty(fileName))
                {
                    flags &= ~GZipFlags.FNAME;
                }
                else
                {
                    flags |= GZipFlags.FNAME;
                }
            }
        }

        /// <summary>
        /// If defined, will use this time instead of the current for the output header
        /// </summary>
        public DateTime? ModifiedTime { get; set; }

        #endregion Public API

        #region Stream overrides

        /// <summary>
        /// Write given buffer to output updating crc
        /// </summary>
        /// <param name="buffer">Buffer to write</param>
        /// <param name="offset">Offset of first byte in buf to write</param>
        /// <param name="count">Number of bytes to write</param>
        public override void Write(byte[] buffer, int offset, int count)
            => WriteSyncOrAsync(buffer, offset, count, null).GetAwaiter().GetResult();

        private async Task WriteSyncOrAsync(byte[] buffer, int offset, int count, CancellationToken? ct)
        {
            if (state_ == OutputState.Header)
            {
                if (ct.HasValue)
                {
                    await WriteHeaderAsync(ct.Value).ConfigureAwait(false);
                }
                else
                {
                    WriteHeader();
                }
            }

            if (state_ != OutputState.Footer)
                throw new InvalidOperationException("Write not permitted in current state");

            crc.Update(new ArraySegment<byte>(buffer, offset, count));

            if (ct.HasValue)
            {
                await base.WriteAsync(buffer, offset, count, ct.Value).ConfigureAwait(false);
            }
            else
            {
                base.Write(buffer, offset, count);
            }
        }

        /// <summary>
        /// Asynchronously write given buffer to output updating crc
        /// </summary>
        /// <param name="buffer">Buffer to write</param>
        /// <param name="offset">Offset of first byte in buf to write</param>
        /// <param name="count">Number of bytes to write</param>
        /// <param name="ct">The token to monitor for cancellation requests</param>
        public override async Task WriteAsync(byte[] buffer, int offset, int count, CancellationToken ct)
            => await WriteSyncOrAsync(buffer, offset, count, ct).ConfigureAwait(false);

        /// <summary>
        /// Writes remaining compressed output data to the output stream
        /// and closes it.
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            try
            {
                Finish();
            }
            finally
            {
                if (state_ != OutputState.Closed)
                {
                    state_ = OutputState.Closed;
                    if (IsStreamOwner)
                    {
                        baseOutputStream_.Dispose();
                    }
                }
            }
        }

#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP3_1_OR_GREATER
        /// <inheritdoc cref="DeflaterOutputStream.Dispose"/>
        public override async ValueTask DisposeAsync()
        {
            try
            {
                await FinishAsync(CancellationToken.None).ConfigureAwait(false);
            }
            finally
            {
                if (state_ != OutputState.Closed)
                {
                    state_ = OutputState.Closed;
                    if (IsStreamOwner)
                    {
                        await baseOutputStream_.DisposeAsync().ConfigureAwait(false);
                    }
                }

                await base.DisposeAsync().ConfigureAwait(false);
            }
        }
#endif

        /// <summary>
        /// Flushes the stream by ensuring the header is written, and then calling <see cref="DeflaterOutputStream.Flush">Flush</see>
        /// on the deflater.
        /// </summary>
        public override void Flush()
        {
            if (state_ == OutputState.Header)
            {
                WriteHeader();
            }

            base.Flush();
        }

        /// <inheritdoc cref="Flush"/>
        public override async Task FlushAsync(CancellationToken ct)
        {
            if (state_ == OutputState.Header)
            {
                await WriteHeaderAsync(ct).ConfigureAwait(false);
            }
            await base.FlushAsync(ct).ConfigureAwait(false);
        }

        #endregion Stream overrides

        #region DeflaterOutputStream overrides

        /// <summary>
        /// Finish compression and write any footer information required to stream
        /// </summary>
        public override void Finish()
        {
            // If no data has been written a header should be added.
            if (state_ == OutputState.Header)
            {
                WriteHeader();
            }

            if (state_ == OutputState.Footer)
            {
                state_ = OutputState.Finished;
                base.Finish();
                var gzipFooter = GetFooter();
                baseOutputStream_.Write(gzipFooter, 0, gzipFooter.Length);
            }
        }

        /// <inheritdoc cref="Finish"/>
        public override async Task FinishAsync(CancellationToken ct)
        {
            // If no data has been written a header should be added.
            if (state_ == OutputState.Header)
            {
                await WriteHeaderAsync(ct).ConfigureAwait(false);
            }

            if (state_ == OutputState.Footer)
            {
                state_ = OutputState.Finished;
                await base.FinishAsync(ct).ConfigureAwait(false);
                var gzipFooter = GetFooter();
                await baseOutputStream_.WriteAsync(gzipFooter, 0, gzipFooter.Length, ct).ConfigureAwait(false);
            }
        }

        #endregion DeflaterOutputStream overrides

        #region Support Routines

        private byte[] GetFooter()
        {
            var totalin = (uint)(deflater_.TotalIn & 0xffffffff);
            var crcval = (uint)(crc.Value & 0xffffffff);

            byte[] gzipFooter;

            unchecked
            {
                gzipFooter = new[] {
                    (byte) crcval,
                    (byte) (crcval >> 8),
                    (byte) (crcval >> 16),
                    (byte) (crcval >> 24),
                    (byte) totalin,
                    (byte) (totalin >> 8),
                    (byte) (totalin >> 16),
                    (byte) (totalin >> 24),
                };
            }

            return gzipFooter;
        }

        private byte[] GetHeader()
        {
            var modifiedUtc = ModifiedTime?.ToUniversalTime() ?? DateTime.UtcNow;
            var modTime = (int)((modifiedUtc - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).Ticks / 10000000L);  // Ticks give back 100ns intervals
            byte[] gzipHeader = {
				// The two magic bytes
				GZipConstants.ID1,
                GZipConstants.ID2,

				// The compression type
				GZipConstants.CompressionMethodDeflate,

				// The flags (not set)
				(byte)flags,

				// The modification time
				(byte) modTime, (byte) (modTime >> 8),
                (byte) (modTime >> 16), (byte) (modTime >> 24),

				// The extra flags
				0,

				// The OS type (unknown)
				255
            };

            if (!flags.HasFlag(GZipFlags.FNAME))
            {
                return gzipHeader;
            }


            return gzipHeader
                .Concat(GZipConstants.Encoding.GetBytes(fileName))
                .Concat(new byte[] { 0 }) // End filename string with a \0
                .ToArray();
        }

        private static string CleanFilename(string path)
            => path.Substring(path.LastIndexOf('/') + 1);

        private void WriteHeader()
        {
            if (state_ != OutputState.Header) return;
            state_ = OutputState.Footer;
            var gzipHeader = GetHeader();
            baseOutputStream_.Write(gzipHeader, 0, gzipHeader.Length);
        }

        private async Task WriteHeaderAsync(CancellationToken ct)
        {
            if (state_ != OutputState.Header) return;
            state_ = OutputState.Footer;
            var gzipHeader = GetHeader();
            await baseOutputStream_.WriteAsync(gzipHeader, 0, gzipHeader.Length, ct).ConfigureAwait(false);
        }

        #endregion Support Routines
    }
}


