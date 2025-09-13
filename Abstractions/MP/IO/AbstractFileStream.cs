
using System.IO;
using Microsoft.Win32.SafeHandles;

namespace MP.IO
{
    /// <summary>
    /// Defines the abstraction for OS file streams.
    /// </summary>
    public abstract class AbstractFileStream : Stream
    {
        /// <summary>
        /// Gets the operating system handle that is used to open the file.
        /// </summary>
        public abstract AbstractFileStreamHandle SafeFileHandle { get; }

        /// <summary>
        /// Clears all the stream's buffers and flushes them to the underlying file
        /// </summary>
        public override void Flush() => SafeFileHandle.Flush();

        /// <inheritdoc />
        public override long Seek(long offset, SeekOrigin origin) => SafeFileHandle.Seek(offset, origin);

        /// <summary>
        /// Gets the length in bytes of this file.
        /// </summary>
        public override long Length => SafeFileHandle.Length;

        /// <summary>
        /// Gets or sets the file pointer position in the file data.
        /// </summary>
        public override long Position 
        { 
            get => SafeFileHandle.Position;
            set => Seek(value , SeekOrigin.Begin); 
        }

        /// <summary>
        /// Sets the exact length of the file, truncating the file if required. 
        /// This operation requires <see cref="FileAccess.Write"/> access.
        /// </summary>
        /// <param name="value">The desired exact length of the file. The <see cref="Length"/> property will report this value after this method completes.</param>
        public override void SetLength(long value) => SafeFileHandle.SetLength(value);

        /// <inheritdoc/>
        protected override void Dispose(bool disposing)
        {
            SafeFileHandle.Dispose();
            base.Dispose(disposing);
        }
    }
}