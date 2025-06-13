using System;
using System.Threading;

namespace MP.BinaryPlaylist
{
    /// <summary>
    /// Specifies the .MPBPL playlist reader.
    /// </summary>
    public sealed class BinaryPlaylistReader : IDisposable
    {
        private System.Boolean dispose;
        private System.IO.Stream stream;
        private PLAYLISTHDR headercore;
        private System.Int64 firstbloboffset;
        private SemaphoreSlim streamaccess;

        /// <summary>
        /// Constructs a new binary playlist reader from the specified stream. <br />
        /// The stream must be both readable and seekable.
        /// </summary>
        /// <param name="strm">The stream to read data from.</param>
        /// <exception cref="ArgumentNullException"><paramref name="strm"/> was <see langword="null"/>.</exception>
        /// <exception cref="ArgumentException"><paramref name="strm"/> is not readable and/or seekable.</exception>
        public BinaryPlaylistReader(System.IO.Stream strm) 
        {
            if (strm is null) { throw new ArgumentNullException(nameof(strm)); }
            if (strm.CanSeek == false || strm.CanRead == false) { throw new ArgumentException("The given stream must be seekable and readable."); }
            stream = strm;
            dispose = false;
            // The access must be syncronized so that only one blob reader can access the stream each time.
            // This must happen because many blob readers can be returned from this class, 
            // this means that many blob readers may request data at the same time , which is impossible 
            // to happen because each blob will refer to a different position in the stream ,
            // and thus will request something in a different position than any other blob.
            streamaccess = new(1, 1);
            Initialize();
        }

        private void Initialize()
        {
            headercore = stream.ReadStructure<PLAYLISTHDR>();
            if (headercore.IsValidHeader == false) {
                throw new InvalidBinaryFormatStreamException();
            }
            firstbloboffset = stream.Position;
            if (headercore.Version > BinaryPlaylistHelpers.Version) {
                throw new InvalidBinaryFormatStreamException($"This reader version cannot read V{headercore.Version} data.");
            }
            if (headercore.Blobs == 0) { throw new InvalidBinaryFormatStreamException("The stream is empty."); }
        }

        #region Read/Seek thread-safe blob reader prototypes
        
        internal System.Int32 BlobReader_Read(System.Byte[] d , System.Int32 offset, System.Int32 count , System.Int64 length , ref System.Int64 position , System.Int64 blobofs)
        {
            if (stream is null) { throw new ObjectDisposedException(nameof(BinaryPlaylistReader)); }
            System.Int32 rd;
            if (position + count > length) { return 0; }
            streamaccess.Wait();
            try {
                stream.Position = position + blobofs;
                rd = stream.Read(d, offset, count);
                position += rd;
            } finally {
                streamaccess.Release();
            }
            return rd;
        }

        internal System.Int64 BlobReader_Seek(System.Int64 value , System.IO.SeekOrigin origin , System.Int64 bloblength , ref System.Int64 blobcurrent , System.Int64 bloboffset)
        {
            // Validate parameters first , then acquire the lock.
            if (stream is null) { throw new ObjectDisposedException(nameof(BinaryPlaylistReader)); }
            switch (origin) 
            {
                case System.IO.SeekOrigin.Begin:
                    if (value < 0) { throw new ArgumentOutOfRangeException("seekp" ,"The seek value must be zero or positive."); }
                    if (value >= bloblength) { throw new ArgumentOutOfRangeException("seekp" ,"The seek value must not overpass the blob's length."); }
                    break;
                case System.IO.SeekOrigin.Current:
                    if (blobcurrent + value >= bloblength) { throw new ArgumentOutOfRangeException("seekp", "The seek value must not overpass the blob's length."); }
                    break;
                case System.IO.SeekOrigin.End:
                    if (value > 0) { throw new ArgumentOutOfRangeException("seekp", "The seek value must be negative or zero.");  }
                    if (blobcurrent - value >= bloblength) { throw new ArgumentOutOfRangeException("seekp", "The seek value must not overpass the blob's length."); }
                    break;
            }
            // Acquire the exclusive lock.
            System.Int64 ret = 0;
            streamaccess.Wait();
            // Whatever happens inside the try block the lock will have been acquired.
            try {
                switch (origin) { 
                    // Set the stream offset based on the blob offset.
                    case System.IO.SeekOrigin.Begin:
                        ret = stream.Seek(bloboffset + value , System.IO.SeekOrigin.Begin);
                        break;
                    case System.IO.SeekOrigin.Current:
                        ret = stream.Seek(bloboffset + blobcurrent + value, System.IO.SeekOrigin.Begin);
                        break;
                    case System.IO.SeekOrigin.End:
                        ret = stream.Seek((bloboffset + bloblength) - value, System.IO.SeekOrigin.Begin);
                        break;
                }
                ret -= bloboffset; // Remove blob's offset from the seeked value.
                // Set the blob's Position property.
                blobcurrent = ret;
            } finally {
                streamaccess.Release();
            }
            return ret;
        }

        #endregion

        /// <summary>
        /// Returns the number of blobs contained in the current stream.
        /// </summary>
        public System.Int32 BlobCount => headercore.Blobs;

        /// <summary>
        /// Gets a blob reader from the current binary playlist with the specified index.
        /// </summary>
        /// <typeparam name="T">The specific <see cref="PlaylistBlobReader"/> to retrieve from the playlist. Must contain a public and callable constructor</typeparam>
        /// <param name="blobindex">The blob index of the reader to retrieve.</param>
        /// <returns>A new instance of a derived <see cref="PlaylistBlobReader"/> class.</returns>
        public T GetReader<T>(System.Int32 blobindex) where T : PlaylistBlobReader, new()
        {
            BLOBHEADER hdr;
            if (stream is null) { throw new ObjectDisposedException(nameof(BinaryPlaylistReader)); }
            if (blobindex >= headercore.Blobs) { throw new ArgumentOutOfRangeException(nameof(blobindex) ,"The blob index attempted to be retrieved is invalid."); }
            if (streamaccess.CurrentCount != 1) { throw new InvalidOperationException("Invalid attempt to access a blob reader while another blob reader is doing read/seek in the stream."); }
            streamaccess.Wait(); // Access and lock the stream.
            System.Int32 iterations = 0;
            T rdr = null;
            stream.Seek(firstbloboffset , System.IO.SeekOrigin.Begin);
            while (iterations < headercore.Blobs)
            {
                hdr = stream.ReadStructure<BLOBHEADER>();
                if (iterations >= blobindex)
                {
                    rdr = PlaylistBlobReader.Initialize<T>(this, stream.Position, hdr);
                    break;
                }
                stream.Seek(hdr.Length, System.IO.SeekOrigin.Current);
                iterations++;
            }
            streamaccess.Release();
            return rdr;
        }

        /// <summary>
        /// Gets or sets a value whether the underlying stream will be disposed when the <see cref="Dispose()"/> method is called.
        /// </summary>
        public System.Boolean DisposeAfterUse
        {
            get => dispose;
            set => dispose = value;
        }

        /// <summary>
        /// Disposes this instance of the <see cref="BinaryPlaylistReader"/> class.
        /// </summary>
        public void Dispose()
        {
            if (stream is null) { return; }
            streamaccess.Dispose();
            streamaccess = null;
            if (dispose) { stream.Dispose(); }
            firstbloboffset = 0;
            stream = null;
        }
    }
}
