using MP.Annotations;
using System;
using System.Runtime.CompilerServices;

namespace MP.BinaryPlaylist
{
    /// <summary>
    /// Represents a blob reader inside a binary playlist. <br />
    /// This is the abstract blob reader implementation; Derived classes provide information
    /// read from the playlist.
    /// </summary>
    public class PlaylistBlobReader : IDisposable
    {
        private BinaryPlaylistReader reader;
        private BLOBHEADER header;
        private System.Int64 position;
        private System.Int64 offset , length;

        /// <summary>
        /// Use this constructor to initialize your own fields at derived classes.
        /// </summary>
        public PlaylistBlobReader()
        {
            position = 0;
            header = default;
            reader = null;
            offset = 0;
            length = 0;
        }
        
        // Binary Playlist infrastracture support
        internal static T Initialize<T>(BinaryPlaylistReader reader, System.Int64 offset, BLOBHEADER header) 
            where T : PlaylistBlobReader , new()
        {
            T rdr = new();
            rdr.reader = reader;
            rdr.offset = offset;
            rdr.position = 0;
            rdr.length = header.Length;
            rdr.header = header;
            return rdr;
        }

        /// <summary>
        /// Initializes a derived instance of <see cref="PlaylistBlobReader"/> class by using the specified binary playlist reader , 
        /// and the blob reader to initialize the derived instance. <br />
        /// Any custom-derived blob reader is ONLY usuable by using this method...
        /// </summary>
        /// <typeparam name="T">The derived type of <see cref="PlaylistBlobReader"/> to initialize.</typeparam>
        /// <param name="reader">The reader to initialize and bind the derived blob reader.</param>
        /// <param name="blobreader">The blob reader to initialize the derived instance from.</param>
        /// <returns>A derived instance of <see cref="PlaylistBlobReader"/> class with type <typeparamref name="T"/>.</returns>
        public static T Initialize<T>(BinaryPlaylistReader reader, PlaylistBlobReader blobreader) 
            where T : PlaylistBlobReader , new()
            => Initialize<T>(reader, blobreader.BlobOffset, blobreader.Header);

        /// <summary>
        /// Reads a sequence of bytes of the current blob.
        /// </summary>
        /// <param name="b">The buffer to save the given data</param>
        /// <param name="offset">The offset inside the buffer to start reading from</param>
        /// <param name="length">The number of bytes to read from the stream.</param>
        /// <returns>The number of bytes read from the underlying stream. If reached the end of the blob , it returns 0.</returns>
        protected System.Int32 Read(System.Byte[] b, System.Int32 offset, System.Int32 length) 
            => reader.BlobReader_Read(b, offset, length , this.length , ref position , this.offset);

        /// <summary>
        /// Seeks the stream position given at <paramref name="offset"/> value by the specified origin.
        /// The returned value is the seeked position , relative to the blob's offset.
        /// </summary>
        /// <param name="offset">The offset to seek.</param>
        /// <param name="origin">The stream origin to seek.</param>
        /// <returns>The seeked position.</returns>
        protected System.Int64 Seek(System.Int64 offset, System.IO.SeekOrigin origin)
            => reader.BlobReader_Seek(offset, origin, length, ref position, this.offset);

        /// <summary>
        /// Reads any numeric type from the stream. <br />
        /// The type of the number to read is specified by the generic type parameter.
        /// </summary>
        /// <typeparam name="T">The numeric type to directly read from the stream.</typeparam>
        /// <returns>The read numeric value from the stream.</returns>
        /// <exception cref="System.IO.IOException">Cannot read the number from the stream.</exception>
        [DeprecatedMayBeRemoved]
        protected unsafe T ReadNumber<T>() where T : unmanaged
        {
            System.Byte[] dt = new System.Byte[sizeof(T)];
            if (Read(dt , 0 , dt.Length) < dt.Length) { throw new System.IO.IOException($"Could not read {dt.Length} bytes from the stream."); }
            return Unsafe.ReadUnaligned<T>(ref dt[0]);
        }

        /// <summary>
        /// Reads any structure from the stream. <br />
        /// The type of the structure to read is specified by the generic type parameter.
        /// </summary>
        /// <typeparam name="T">The structure to read.</typeparam>
        /// <returns></returns>
        /// <exception cref="System.IO.IOException">Cannot read the structure from the stream.</exception>
        [DeprecatedMayBeRemoved]
        protected unsafe T ReadStructure<T>() where T : struct
        {
            System.Byte[] dt = new System.Byte[Unsafe.SizeOf<T>()];
            if (Read(dt, 0, dt.Length) < dt.Length) { throw new System.IO.IOException($"Could not read {dt.Length} bytes from the stream."); }
            return dt.ReadStructure<T>(0);
        }

        /// <summary>
        /// Reads a string value from the stream , specifying it's length and whether is in the ASCII or in UTF-16LE format.
        /// </summary>
        /// <param name="charlength">The number of characters to read , the final string length.</param>
        /// <param name="isascii">When set to <see langword="true"/>, the method reads ASCII characters; otherwise it read UTF-16LE characters.</param>
        /// <returns>The read string.</returns>
        /// <exception cref="System.IO.IOException">Could not read the string from the stream.</exception>
        [DeprecatedMayBeRemoved]
        protected unsafe System.String ReadString(System.Int32 charlength , System.Boolean isascii)
        {
            if (isascii) {
                System.Byte[] dt = new System.Byte[charlength];
                if (Read(dt, 0, dt.Length) < dt.Length) { throw new System.IO.IOException($"Could not read {dt.Length} bytes from the stream."); }
                fixed (System.Byte* psrc = dt) 
                {
                    return new((System.SByte*)psrc , 0 , charlength);
                }
            } else {
                System.Byte[] dt = new System.Byte[charlength * sizeof(System.Char)];
                if (Read(dt, 0, dt.Length) < dt.Length) { throw new System.IO.IOException($"Could not read {dt.Length} bytes from the stream."); }
                fixed (System.Byte* psrc = dt)
                {
                    return new((System.Char*)psrc , 0 , charlength);
                }
            }
        }

        /// <summary>
        /// Reads a byte from the blob.
        /// </summary>
        /// <returns>The read byte or -1 indicating that the end of the blob has been reached.</returns>
        [DeprecatedMayBeRemoved]
        protected System.Int32 ReadByte()
        {
            System.Byte[] dt = new System.Byte[1];
            System.Int32 rb = Read(dt, 0 , dt.Length);
            if (rb == 0) { return -1; }
            return dt[0];
        }

        /// <summary>
        /// Fetches a buffer from the blob.
        /// </summary>
        /// <param name="count">The number of bytes to be fetched into a new buffer</param>
        /// <returns>The fetched buffer.</returns>
        [DeprecatedMayBeRemoved]
        protected System.Byte[] ReadBytes(System.Int64 count)
        {
            const System.Int32 BUFSIZE = 4096;
            if (count == 0) { return System.Array.Empty<System.Byte>(); }
            System.Byte[] ret = new System.Byte[count];
            // Read 'count' bytes from the stream. To achieve that , use a second temp buffer which will copy the stream data incrementally to the result buffer.
            // This is done to achieve offset indexes longer than 2147483647.
            System.Byte[] tempbuf = new System.Byte[BUFSIZE];
            // cb variable: Consumed bytes.
            // rbb variable: Factually read bytes. Used as an index in the copy operation.
            System.Int64 cb = count, rbb = 0;
            System.Int32 rb; // Read bytes from the stream.
            do
            {
                // The condition specifies that if we have bufferable data , the entire buffer will be used;
                // otherwise , read only the required bytes. Do that in order for the stream's position to
                // only advance by count bytes.
                rb = Read(tempbuf, 0, (cb >= BUFSIZE) ? tempbuf.Length : cb.ToInt32());
                // No more data to read , exit and return whatever we found.
                if (rb == 0) { break; }

                // Bump the read bytes into the final buffer.
                tempbuf.Copy(0, ret, rbb, rb.ToUInt32());

                // Update index and consumed bytes.
                rbb += rb;
                cb -= rb;
            } while (cb > 0); // Do this until the entire buffer has been fetched.
            tempbuf = null;
            return ret;
        }

        /// <summary>
        /// Indicates the stream position inside the blob.
        /// </summary>
        public System.Int64 Position
        {
            get => position;
            protected set => Seek(value , System.IO.SeekOrigin.Begin);
        }

        /// <summary>Gets the blob length in bytes.</summary>
        public System.Int64 Length => length;

        /// <summary>Gets the blob offset , relative to the whole source stream.</summary>
        public System.Int64 BlobOffset => offset;

        /// <summary>Gets the header information of this blob.</summary>
        public BLOBHEADER Header => header;

        private System.Int64 PositionFunc() => position;

        private System.Int64 LengthFunc() => length;

        /// <summary>
        /// Gets a stream adapted to the current blob suitable for reading. <br />
        /// It is recommended for newer designs to use this stream object instead. <br />
        /// Make sure to dispose this object once you are done using it.
        /// </summary>
        protected System.IO.Stream Stream
        {
            get {
                ObjectDisposedException.ThrowIf(reader is null, this);
                return new PlaylistBlobReaderAdapterStream(
                    new(Read),
                    new(Seek),
                    new(PositionFunc),
                    new(LengthFunc)
                );
            }
        }

        /// <summary>
        /// Disposes the current blob reader. <br />
        /// When overriden by deriving classes, they must call this method by using the <see langword="base"/> convention.
        /// </summary>
        /// <param name="disposing">A value whether and all the resources allocated by the managed memory should be freed too.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing) {
                position = 0;
                offset = 0;
                length = 0;
            }
            reader = null;
        }

        /// <summary>
        /// Disposes this instance of the <see cref="PlaylistBlobReader"/> class.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Default finalizer.
        /// </summary>
        ~PlaylistBlobReader() => Dispose(false);
    }
}
