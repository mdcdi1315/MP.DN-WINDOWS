using System;

namespace MP.BinaryPlaylist
{
    /// <summary>
    /// Specifies the .MPBPL binary playlist writer.
    /// </summary>
    public sealed class BinaryPlaylistWriter : IDisposable
    {
        private System.Int32 blobs;
        private PLAYLISTHDR corehdr;
        private System.Boolean dispose;
        private System.IO.Stream stream;

        /// <summary>
        /// Constructs a new binary blaylist writer that will write to the specified stream,
        /// and the number of blobs that will be finally included when the playlist will be fully written.
        /// </summary>
        /// <param name="stream">The stream where the playlist data will be saved to.</param>
        /// <param name="blobs">The number of blobs that will be written for this playlist.</param>
        /// <exception cref="ArgumentNullException"><paramref name="stream"/> was <see langword="null"/>.</exception>
        /// <exception cref="ArgumentException"><paramref name="stream"/> was not readable.</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="blobs"/> was zero or represented a negative number.</exception>
        public BinaryPlaylistWriter(System.IO.Stream stream, System.Int32 blobs) : this(stream, BlobTypes.MAXEMBEDDEDBLOBVAL, blobs) { }

        /// <summary>
        /// Constructs a new binary blaylist writer that will write to the specified stream,
        /// and the number of blobs that will be finally included when the playlist will be fully written.
        /// </summary>
        /// <param name="stream">The stream where the playlist data will be saved to.</param>
        /// <param name="maxcontainedblobtype">The last blob type that is expected to be contained in the stream. The reader supports this value so you must appropriately initialize it.</param>
        /// <param name="blobs">The number of blobs that will be written for this playlist.</param>
        /// <exception cref="ArgumentNullException"><paramref name="stream"/> was <see langword="null"/>.</exception>
        /// <exception cref="ArgumentException"><paramref name="stream"/> was not readable.</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="blobs"/> was zero or represented a negative number.</exception>
        public BinaryPlaylistWriter(System.IO.Stream stream, BlobTypes maxcontainedblobtype , System.Int32 blobs)
        {
            if (stream is null) { throw new ArgumentNullException("stream"); }
            if (stream.CanWrite == false) { throw new ArgumentException("The stream must be writeable."); }
            if (blobs < 0) { throw new ArgumentOutOfRangeException(nameof(blobs), "The number of blobs to write must not be negative!"); }
            corehdr = new();
            corehdr.MaxType = maxcontainedblobtype;
            corehdr.Blobs = blobs.ToUInt16();
            corehdr.Version = BinaryPlaylistHelpers.Version;
            stream.WriteStructure(corehdr);
            this.blobs = blobs;
            this.stream = stream;
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
        /// Writes the specified data from the specified blob writer to this stream.
        /// </summary>
        /// <param name="writer">The writer instance to write the generated blob into this stream.</param>
        /// <param name="dispose">Whether to dispose the provided writer after usage.</param>
        /// <exception cref="ArgumentNullException"><paramref name="writer"/> was null.</exception>
        /// <exception cref="InvalidOperationException"><paramref name="writer"/> was disposed before calling this method, or all the expected blobs were written.</exception>
        public void Write(PlaylistBlobWriter writer , System.Boolean dispose)
        {
            if (blobs <= 0) {
                throw new InvalidOperationException("All the playlist blobs are written! If you need a new blob to be saved , you must update the number of requested blobs.");
            }
            if (writer is null) { throw new ArgumentNullException(nameof(writer)); }
            switch (writer.WriterCall_Write(stream))
            {
                case WriterCallState.Incomplete:
                    throw new BlobWriterIncompleteException();
                case WriterCallState.Disposed:
                    throw new InvalidOperationException("Cannot write to the final stream from a disposed writer.");
            }
            if (dispose) { writer.Dispose(); }
            writer = null;
            blobs--;
        }

        /// <summary>
        /// Writes the specified data from the specified writer to this stream.
        /// </summary>
        /// <param name="writer">The writer instance to write the generated blob into this stream.</param>
        /// <exception cref="ArgumentNullException"><paramref name="writer"/> was null.</exception>
        /// <exception cref="InvalidOperationException"><paramref name="writer"/> was disposed before calling this method.</exception>
        public void Write(PlaylistBlobWriter writer) => Write(writer, false);

        /// <summary>
        /// Disposes this writer object.
        /// </summary>
        /// <exception cref="InvalidOperationException">The writer is incomplete.</exception>
        public void Dispose()
        {
            if (blobs > 0) { throw new InvalidOperationException($"The writer cannot be disposed before ALL blobs were written! (Remaining {blobs} blobs to write)"); }
            if (stream is null) { return; }
            if (dispose) { stream.Close(); stream.Dispose(); }
            stream = null;
        }
    }
}
