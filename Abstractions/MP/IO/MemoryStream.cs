
// Keep in sync with the backend's MemoryStream implementation.

using System;
using System.IO;
using System.Runtime.CompilerServices;

namespace MP.IO
{
    /// <summary>
    /// Defines a faster and lightweight alternative compared to the classic MemoryStream class. <br />
    /// The data copy , read and write are performed by using the <see cref="Unsafe"/> class methods.
    /// </summary>
    public class MemoryStream : Stream
    {
        private const System.Int64 MEMSTREAM_MAXLEN = System.Int32.MaxValue;

        private System.Byte[] buffer;
        private System.Boolean expandable;
        private System.Int64 position, length, capacity;

        /// <summary>
        /// Creates a new empty memory stream instance that is expandable.
        /// </summary>
        public MemoryStream()
        {
            capacity = 0;
            expandable = true;
            position = 0;
            length = 0;
            buffer = System.Array.Empty<System.Byte>();
        }

        /// <summary>
        /// Creates a new instance of a memory stream with the specified pre-allocated capacity and a value whether the new stream should be expandable.
        /// </summary>
        /// <param name="requiredcap">The capacity to be pre-allocated.</param>
        /// <param name="isexpandable">A value whether the new stream should be expandable or not.</param>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="requiredcap"/> was negative.</exception>
        public MemoryStream(System.Int64 requiredcap, System.Boolean isexpandable)
        {
            if (requiredcap < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(requiredcap), "The required array capacity cannot be less than zero.");
            }
            position = 0;
            length = requiredcap;
            capacity = requiredcap;
            expandable = isexpandable;
            buffer = new System.Byte[length];
        }

        /// <summary>
        /// Creates a new instance of a memory stream that has the contents contained in <paramref name="buffer"/>.
        /// </summary>
        /// <param name="buffer">The buffer to initialize the memory stream from.</param>
        /// <param name="isexpandable">A value whether the new should be expandable or not.</param>
        /// <exception cref="ArgumentNullException"><paramref name="buffer"/> was <see langword="null"/>.</exception>
        public MemoryStream(System.Byte[] buffer, System.Boolean isexpandable)
        {
            if (buffer is null) { throw new ArgumentNullException(nameof(buffer)); }
            position = 0;
            expandable = isexpandable;
            this.buffer = buffer;
            length = capacity = this.buffer.LongLength;
        }

        /// <summary>
        /// Creates a new instance of an expandable memory stream that has the contents contained in <paramref name="buffer"/>,
        /// </summary>
        /// <param name="buffer">The buffer to initialize the memory stream from.</param>
        /// <exception cref="ArgumentNullException"><paramref name="buffer"/> was <see langword="null"/>.</exception>
        public MemoryStream(System.Byte[] buffer) : this(buffer, true) { }

        /// <summary>
        /// Creates a new instance of an expandable memory stream with the specified pre-allocated capacity.
        /// </summary>
        /// <param name="requiredcap">The capacity to be pre-allocated.</param>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="requiredcap"/> was negative.</exception>
        public MemoryStream(System.Int64 requiredcap) : this(requiredcap, true) { }

        private void SetNewCapacity(System.Int64 newcap)
        {
            if (newcap < length)
            {
                throw new ArgumentOutOfRangeException(nameof(newcap), "The capacity given is too small.");
            }
            // Maintain compatibility with the previous implementation
            if (newcap != capacity)
            {
                if (expandable)
                {
                    // Check if we have empty capacity , which in such case we can just delete the buffer.
                    if (newcap == 0)
                    {
                        // Just delete the buffer and return.
                        buffer = System.Array.Empty<System.Byte>();
                        capacity = 0;
                        return;
                    }
                    System.Byte[] newbuf = new System.Byte[newcap];
                    if (buffer.LongLength > 0)
                    {
                        Unsafe.CopyBlockUnaligned(ref newbuf[0], ref buffer[0], buffer.LongLength.ToUInt32());
                    }
                    buffer = newbuf;
                    capacity = newcap;
                }
                else
                {
                    throw new NotSupportedException("The memory stream is not expandable.");
                }
            }
        }

        // Copied from the actual MemoryStream implementation.
        /// <summary>
        /// Ensures that the buffer can fit '<paramref name="value"/>' bytes. 
        /// </summary>
        /// <param name="value">The buffer capacity to request</param>
        /// <returns>A value whether the internal array was needed to be modified.</returns>
        /// <exception cref="NotSupportedException">The stream is not expandable.</exception>
        protected System.Boolean EnsureCapacity(System.Int64 value)
        {
            // Check for overflow
            if (value < 0)
                throw new IOException("Stream capacity is too large.");

            if (value > capacity)
            {
                System.Int64 newCapacity = Math.Max(value, 256);

                // We are ok with this overflowing since the next statement will deal
                // with the cases where capacity*2 overflows.
                if (newCapacity < capacity * 2)
                {
                    newCapacity = capacity * 2;
                }

                // We want to expand the array up to Array.MaxLength.
                // And we want to give the user the value that they asked for
                if ((uint)(capacity * 2) > Array.MaxLength)
                {
                    newCapacity = Math.Max(value, Array.MaxLength);
                }

                SetNewCapacity(newCapacity);
                return true;
            }
            // No need to allocate a new buffer , just return with false
            return false;
        }

        /// <inheritdoc />
        public override System.Boolean CanRead => true;

        /// <inheritdoc />
        public override System.Boolean CanSeek => true;

        /// <inheritdoc />
        public override System.Boolean CanWrite => true;

        /// <summary>
        /// Gets a value whether the current memory stream object can be expanded.<br />
        /// Can be overriden for custom implementations
        /// </summary>
        public virtual System.Boolean CanExpand => expandable;

        /// <summary>
        /// Gets the stream's length in bytes.
        /// </summary>
        public override System.Int64 Length => length;

        /// <summary>
        /// Gets or sets the position inside the internal stream buffer. <br />
        /// Can be overriden for custom implementations
        /// </summary>
        /// <exception cref="IOException">Attempted to set an invalid position within the buffer.</exception>
        /// <exception cref="ArgumentOutOfRangeException">Attempted to set a position that is larger than <see cref="System.Array.MaxLength"/>.</exception>
        public override System.Int64 Position
        {
            get => position;
            set => Seek(value, SeekOrigin.Begin);
        }

        /// <summary>
        /// Adjusts the memory space that the current <see cref="MemoryStream"/> instance is occupying. <br />
        /// Can be overriden for custom implementations
        /// </summary>
        /// <exception cref="NotSupportedException">For the original implementation, the stream is not expandable.</exception>
        public virtual System.Int64 Capacity
        {
            get => capacity;
            set => EnsureCapacity(value);
        }

        // It is a memory buffer, buffers can be thought as 'implicitly flushed data'
        /// <summary>
        /// Memory buffers are always implicitly flushed , so this method does nothing. <br />
        /// Cannot be overriden by custom implementations
        /// </summary>
        public sealed override void Flush() { }

        /// <inheritdoc />
        public override void SetLength(System.Int64 value)
        {
            if (value < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(value), "Stream length cannot be less than zero.");
            }
            System.Boolean allocatedNewArray = EnsureCapacity(value);
            if (allocatedNewArray == false && value > length)
            {
                Unsafe.InitBlockUnaligned(ref buffer[length], 0, (value - length).ToUInt32());
            }
            length = value;
            if (position > value)
            {
                position = value;
            }
        }

        /// <summary>
        /// Seeks inside the memory buffer and returns the new position that was applied. <br />
        /// Can be overriden for custom implementations
        /// </summary>
        /// <param name="offset">The offset to seek inside the memory buffer by the <paramref name="origin"/> specified.</param>
        /// <param name="origin">The seek origin where <paramref name="offset"/> will be applied as.</param>
        /// <returns>The new position that is applied if the call succeeded.</returns>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="offset"/> was a number larger than <see cref="System.Array.MaxLength"/>.</exception>
        /// <exception cref="ArgumentException"><paramref name="origin"/> was not a valid member of the <see cref="SeekOrigin"/> enumeration.</exception>
        /// <exception cref="IOException">Computed stream position was overflown.</exception>
        public override System.Int64 Seek(System.Int64 offset, SeekOrigin origin)
        {
            if (offset > MEMSTREAM_MAXLEN)
                throw new ArgumentOutOfRangeException(nameof(offset), "Offset is invalid.");

            System.Int64 displacement = origin switch
            {
                SeekOrigin.Begin => offset,
                SeekOrigin.Current => position + offset,
                SeekOrigin.End => length - offset,
                _ => throw new ArgumentException("Invalid seek origin provided.")
            };

            if (displacement < 0)
            {
                throw new IOException("Attempted to seek before the beginning of the stream.");
            }
            return position = displacement;
        }

        /// <inheritdoc />
        public override System.Int32 Read(System.Byte[] buffer, System.Int32 offset, System.Int32 count)
        {
            ValidateBufferArguments(buffer, offset, count);
            // Handle cases where the user does not want to fill his buffer but does perform the call.
            // This could happen in typed code where variable length data are retrieved.
            if (count == 0) { return 0; }
            if (position >= this.buffer.LongLength) { return 0; }
            System.UInt32 fc = (position + count > length ? length - position : count).ToUInt32();
            // Even if zero this unsafe call should not do anything else but nothing,
            // but the index would hit failures if the provided buffer is zero-length,
            // so this is handled separately above with the if statement.
            Unsafe.CopyBlockUnaligned(ref buffer[offset], ref this.buffer[position], fc);
            // Update position
            position += fc;
            return fc.ToInt32();
        }

        /// <inheritdoc />
        public override void Write(System.Byte[] buffer, System.Int32 offset, System.Int32 count)
        {
            ValidateBufferArguments(buffer, offset, count);
            // No meaning to perform anything when the user wants to write nothing.
            if (count == 0) { return; }
            // We must write 'count' bytes to the stream , but what happens when the position has been changed 
            // and the underlying array bounds cannot afford the current capacity?
            System.Int64 fc = capacity;
            // Just check whether the position + count gets off the current capacity and appropriately update.
            // This should be correct under these cases.
            if (position + count >= fc)
            {
                fc = position + count;
            }
            // Ensure now the new capacity
            EnsureCapacity(fc);
            // Write the buffered data to our underlying buffer...
            // Note that we do not need to zeroize; the contents will be elsewise overwritten.
            Unsafe.CopyBlockUnaligned(ref this.buffer[position], ref buffer[offset], count.ToUInt32());
            // Update both position and length , if deemed appropriate
            position += count;
            if (position >= length) { length = position; }
        }

        /// <inheritdoc />
        public override System.Int32 Read(Span<System.Byte> buffer)
        {
            if (buffer.Length == 0) { return 0; }
            if (position >= this.buffer.LongLength) { return 0; }
            System.UInt32 fc = (position + buffer.Length > length ? length - position : buffer.Length).ToUInt32();
            Unsafe.CopyBlockUnaligned(ref buffer[0], ref this.buffer[position], fc);
            position += fc;
            return fc.ToInt32();
        }

        /// <inheritdoc />
        public override void Write(ReadOnlySpan<System.Byte> buffer)
        {
            if (buffer.Length == 0) { return; }
            // We must write 'buffer.Length' bytes to the stream , but what happens when the position has been changed 
            // and the underlying array bounds cannot afford the current capacity?
            System.Int64 fc = capacity;
            // Just check whether the position + buffer.Length gets off the current capacity and appropriately update.
            // This should be correct under these cases.
            if (position + buffer.Length >= fc)
            {
                fc = position + buffer.Length;
            }
            // Ensure now the new capacity
            EnsureCapacity(fc);
            Unsafe.CopyBlockUnaligned(ref this.buffer[position], ref Unsafe.AsRef(in buffer[0]), buffer.Length.ToUInt32());
            // Update both position and length , if deemed appropriate
            position += buffer.Length;
            if (position >= length) { length = position; }
        }

        /// <inheritdoc />
        // Here we can provide a faster implementation for directly writing a byte to the stream.
        public override void WriteByte(System.Byte value)
        {
            EnsureCapacity(length + 1);
            buffer[position] = value;
            position++;
            if (position >= length) { length = position; }
        }

        /// <inheritdoc />
        // Also for the ReadByte method.
        public override System.Int32 ReadByte()
        {
            if (position < length)
            {
                return buffer[position++]; // Position is updated as we would expect
            }
            else
            {
                return -1;
            }
        }

        /// <summary>
        /// Gets a reference to the raw data that this stream contains.
        /// </summary>
        /// <returns>A reference to the raw data buffer</returns>
        protected virtual System.Byte[] GetRawBuffer() => buffer;

        /// <summary>
        /// Gets a copy of the data of this stream as a new buffer.
        /// </summary>
        /// <returns>The copy of the stream's data as a byte array.</returns>
        public virtual System.Byte[] ToArray()
        {
            System.Byte[] ret = new System.Byte[length];
            Unsafe.CopyBlockUnaligned(ref ret[0], ref buffer[0], ret.LongLength.ToUInt32());
            return ret;
        }
    }
}