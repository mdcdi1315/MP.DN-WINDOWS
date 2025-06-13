
using System;
using System.IO;
using System.Threading;

namespace MP.Archiving
{
    /// <summary>
    /// Defines a thread-safe read-only archive offset stream for reading archive entries. <br />
    /// With this way, we do not have to allocate memory streams and thus is more memory-friendly.
    /// </summary>
    public sealed class ArchiveOffsetStream : Stream
    {
        private Stream actual;
        private System.Int64 ofs, len, pos;
        private SemaphoreSlim semaphore;

        /// <summary>
        /// Creates a new offset stream that is around the stream provided in the <paramref name="wrapping"/> parameter. <br />
        /// The offset is created from the current position of the underlying stream.
        /// </summary>
        /// <param name="wrapping">The stream to wrap around.</param>
        /// <param name="length">The length of this offset stream.</param>
        /// <exception cref="ArgumentNullException"><paramref name="wrapping"/> was null.</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="length"/> was out of the underlying stream length bounds.</exception>
        /// <exception cref="NotSupportedException">The stream given in <paramref name="wrapping"/> is not supported.</exception>
        public ArchiveOffsetStream(Stream wrapping , System.Int64 length)
        {
            if (wrapping is null) { throw new ArgumentNullException(nameof(wrapping)); }
            if (length <= 0) { throw new ArgumentOutOfRangeException(nameof(length), "Length cannot be negative or zero."); }
            try {
                if (length > wrapping.Length)
                {
                    throw new ArgumentOutOfRangeException(nameof(length), "Length must be less than or equal to the wrapping stream length.");
                }
                len = length;
            } catch (System.NotSupportedException causing) {
                throw new NotSupportedException("Streams where their length cannot be retrieved cannot be wrapped." , causing);
            }
            if (wrapping.CanSeek == false)
            {
                throw new NotSupportedException("Unseekable streams are not supported.");
            }
            if (wrapping.CanRead == false)
            {
                throw new NotSupportedException("Unreadable streams are not supported.");
            }
            ofs = wrapping.Position;
            pos = 0;
            actual = wrapping;
            semaphore = new(1);
        }

        // Length is more or less a constant so we can freely read from this.
        public override long Length => len;

        public override long Position 
        {
            get => pos; // we can read from the field from any thread at any time
            set {
                semaphore.Wait();
                try {
                    // But we need the underlying stream to apply a new position , so the
                    // lock must be acquired...
                    pos = actual.Seek(ofs + value , SeekOrigin.Begin) - ofs;
                } finally {
                    semaphore.Release();
                }
            }
        }

        /// <summary>
        /// Gets the exact position inside the underlying stream.
        /// </summary>
        public System.Int64 ActualPosition => ofs + pos;

        public override long Seek(long offset, SeekOrigin origin)
        {
            // Validate and create seek offset based on the stream's beginning
            // Thankfully we can perform these checks without acquiring the lock.
            System.Int64 ofsfinal;
            switch (origin)
            {
                case SeekOrigin.Begin:
                    ofsfinal = ofs + offset;
                    break;
                case SeekOrigin.Current:
                    // When current this is equal as sending begin origin 
                    // with current offset and position.
                    ofsfinal = ofs + pos + offset;
                    // To the wrapping stream we must send begin origin
                    // so that the above becomes correct.
                    break;
                case SeekOrigin.End:
                    // When specifying the end of stream we must devirtualize by the desired stream length.
                    ofsfinal = ofs + (len - offset);
                    // To the wrapping stream we must send begin origin
                    // so that the above becomes correct.
                    break;
                default:
                    throw new NotSupportedException($"Seek origin not supported: {origin}");
            }
            // These checks will work since we have fully devirtualized by the stream's beginning.
            if (ofsfinal < ofs)
            {
                throw new ArgumentOutOfRangeException(nameof(offset) , "Seek offset was out of the stream's bounds.");
            }
            if (ofsfinal > ofs + len)
            {
                throw new ArgumentOutOfRangeException(nameof(offset), "Seek offset was out of the stream's bounds.");
            }
            semaphore.Wait();
            try {
                // The returned value is by the underlying stream's position,
                // so we must bring this number back to the devirtualized position of current stream.
                System.Int64 ofapplied = actual.Seek(ofsfinal, SeekOrigin.Begin) - ofs; 
                // Update stream's position appopriately.
                pos = ofapplied;
                return ofapplied;
            } finally {
                semaphore.Release();
            }
        }

        public override void SetLength(long value) => throw new NotSupportedException("Setting length is unsupported.");

        public override bool CanRead => true;

        public override bool CanSeek => true;

        public override bool CanWrite => false;

        public override bool CanTimeout => false;

        public override void Flush() {}

        public override int Read(byte[] buffer, int offset, int count)
        {
            // We can validate the buffer arguments before acquiring the lock
            ValidateBufferArguments(buffer, offset, count);
            semaphore.Wait();
            try {
                // I could transfer this if sequence outside the semaphore wait sequence , 
                // but pos could be changed before the actual call , so be sure that pos will not be changed during this stage.
                if (pos + count > len)
                {
                    // This will always be in an Int32 boundary
                    count = (len - pos).ToInt32();
                    // Mark as EOF if count has resulted to be zero.
                    if (count == 0) { return 0; }
                }
                // Seek to the correct offset before executing the operation
                actual.Seek(ofs + pos, SeekOrigin.Begin);
                // And now read data with offset!
                System.Int32 rb = actual.Read(buffer, offset, count);
                // If it was negative do not modify position and return the value as is.
                if (rb < 0) { return rb; }
                pos += rb;
                return rb;
            } finally {
                semaphore.Release();
            }
        }

        public override void Write(byte[] buffer, int offset, int count) => throw new NotSupportedException("Writing to such a stream is not supported.");

        protected override void Dispose(bool disposing)
        {
            // This was created from another stream so do not request disposal routines
            actual = null;
            if (semaphore is not null)
            {
                // The thread that called Dispose must wait until all the semaphore operations have been finished.
                while (semaphore.CurrentCount == 0) { Thread.Sleep(10); }
                semaphore.Dispose();
                semaphore = null;
            }
            base.Dispose(disposing);
        }
    }
}