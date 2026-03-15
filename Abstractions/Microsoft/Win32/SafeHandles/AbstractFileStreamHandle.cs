
using MP.IO;
using System;
using System.Runtime.InteropServices;

namespace Microsoft.Win32.SafeHandles
{
    /// <summary>
    /// Defines the main handle abstraction for <see cref="AbstractFileStream"/> objects.
    /// </summary>
    public abstract class AbstractFileStreamHandle : CriticalHandle
    {
        /// <summary>
        /// Creates a new instance of the <see cref="AbstractFileStreamHandle"/> class.
        /// </summary>
        protected AbstractFileStreamHandle() : base(IntPtr.Zero) { }

        /// <summary>
        /// Gets a value whether this file stream handle is invalid.
        /// </summary>
        public override bool IsInvalid => handle == IntPtr.Zero;

        /// <summary>
        /// Gets the length of this file stream handle, in bytes.
        /// </summary>
        public abstract long Length { get; }

        /// <summary>
        /// Gets the read/written position of this file stream handle, in bytes.
        /// </summary>
        public virtual long Position => Seek(0, SeekDisplacement.Current);

        /// <summary>
        /// Seeks into the data of the file stream handle.
        /// </summary>
        /// <param name="offset">The offset to seek to, relative to <paramref name="origin"/>.</param>
        /// <param name="origin">The search origin to specify.</param>
        /// <returns>The new absolute position in the stream</returns>
        public abstract long Seek(long offset , SeekDisplacement origin);

        /// <summary>
        /// Sets the length of the file to specified number of bytes.
        /// </summary>
        /// <param name="value">The number of bytes that will be the new length of the file. Cannot be negative.</param>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="value"/> was negative.</exception>
        public abstract void SetLength(long value);

        /// <summary>
        /// Flushes all the buffered data to the opened file.
        /// </summary>
        public abstract void Flush();

        /// <summary>
        /// Gets the mode used to open the file.
        /// </summary>
        public abstract FileMode Mode { get; }

        /// <summary>
        /// Gets the share option used to open the file.
        /// </summary>
        public abstract FileShare Share { get; }

        /// <summary>
        /// Gets the access option used to open the file.
        /// </summary>
        public abstract DataStreamMode StreamMode { get; }
    }
}