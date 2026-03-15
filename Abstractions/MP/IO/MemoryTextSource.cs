
using System;
using System.Text;
using System.Threading;
using MP.Annotations.CodeAnalysis;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;

// Portions of code for the below class have
// been adapted from the MemoryStream class.

namespace MP.IO
{
    /// <summary>
    /// Like <see cref="MemoryStream"/> for data streams, the <see cref="MemoryTextSource"/> class provides an equivalent implementation targeting the <see cref="ITextSource"/> interface.
    /// </summary>
    public class MemoryTextSource : ITextSource
    {
        private System.Char[] buffer;
        private System.Boolean disposed;
        private readonly System.Boolean expandable;
        private System.Int64 position, length, capacity;

        /// <summary>
        /// Initializes a new and empty instance of the <see cref="MemoryTextSource"/> class with a length of zero and has an expandable capacity.
        /// </summary>
        public MemoryTextSource()
        {
            disposed = false;
            expandable = true;
            position = length = capacity = 0L;
            buffer = Array.Empty<System.Char>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MemoryTextSource"/> class, that gets it's data from 
        /// the specified character buffer and specifying whether the source can be expandable or not. <br />
        /// The text source length and capacity properties are filled based on the input buffer length.
        /// </summary>
        /// <exception cref="ArgumentNullException"><paramref name="initial_buffer"/> is <see langword="null"/>.</exception>
        [Throws(typeof(ArgumentNullException))]
        public MemoryTextSource(System.Char[] initial_buffer, System.Boolean expandable)
        {
            ArgumentNullException.ThrowIfNull(initial_buffer);
            position = 0L;
            disposed = false;
            buffer = initial_buffer;
            this.expandable = expandable;
            length = capacity = initial_buffer.LongLength;
        }

        /// <summary>
        /// Gets a value whether the current memory text source object can be expanded.<br />
        /// Can be overriden for custom implementations
        /// </summary>
        public virtual System.Boolean CanExpand => expandable;

        /// <summary>
        /// Gets the text source's length in characters.
        /// </summary>
        public System.Int64 Length => length;

        /// <summary>
        /// Gets or sets the position inside the internal character buffer. <br />
        /// Can be overriden for custom implementations
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">Attempted to set a position that is larger than <see cref="System.Array.MaxLength"/>.</exception>
        public System.Int64 Position
        {
            get => position;
            set {
                if (value < 0) {
                    throw new ArgumentOutOfRangeException("position", "Position cannot be a negative value.");
                } else if (value > length) {
                    throw new ArgumentOutOfRangeException("position", "Position cannot be larger than the text source length.");
                } else {
                    position = value;
                }
            }
        }

        /// <summary>
        /// Adjusts the memory space that the current <see cref="MemoryTextSource"/> instance is occupying. <br />
        /// Can be overriden for custom implementations
        /// </summary>
        /// <exception cref="NotSupportedException">For the original implementation, the stream is not expandable.</exception>
        public virtual System.Int64 Capacity
        {
            get => capacity;
            set => EnsureCapacity(value);
        }

        /// <inheritdoc />
        public DataStreamMode Mode => DataStreamMode.ReadWrite;

        /// <inheritdoc />
        public Encoding Encoding => UnsafeMethods.IsLittleEndian ? Encoding.Unicode : Encoding.BigEndianUnicode;

        private void SetNewCapacity(System.Int64 newcap)
        {
            if (newcap < length) {
                throw new ArgumentOutOfRangeException(nameof(newcap), "The capacity given is too small.");
            } else if (newcap != capacity) { // Maintain compatibility with the previous implementation
                if (expandable) {
                    // Check if we have empty capacity , which in such case we can just delete the buffer.
                    if (newcap == 0)
                    {
                        // Just delete the buffer and return.
                        buffer = System.Array.Empty<System.Char>();
                        capacity = 0;
                        return;
                    }
                    System.Char[] newbuf = new System.Char[newcap];
                    if (buffer.LongLength > 0) { Array.Copy(buffer, 0L, newbuf, 0L, buffer.LongLength); }
                    buffer = newbuf;
                    capacity = newcap;
                } else {
                    throw new NotSupportedException("The memory text source is not expandable.");
                }
            }
        }

        // Copied from the actual MemoryStream implementation.
        /// <summary>
        /// Ensures that the buffer can fit '<paramref name="value"/>' bytes. 
        /// </summary>
        /// <param name="value">The buffer capacity to request</param>
        /// <returns>A value whether the internal array was needed to be modified.</returns>
        /// <exception cref="NotSupportedException">The memory text source is not expandable.</exception>
        protected System.Boolean EnsureCapacity(System.Int64 value)
        {
            // Check for overflow
            if (value < 0) {
                throw new IOException("Memory text source capacity is too large.");
            } else if (value > capacity) {
                System.Int64 newCapacity = Math.Max(value, 256);
                System.Int64 double_capacity = capacity * 2L;

                // We are ok with this overflowing since the next statement will deal
                // with the cases where capacity*2 overflows.
                if (newCapacity < double_capacity)
                {
                    newCapacity = double_capacity;
                }

                // We want to expand the array up to Array.MaxLength.
                // And we want to give the user the value that they asked for
                if (unchecked((uint)double_capacity) > Array.MaxLength)
                {
                    newCapacity = Math.Max(value, Array.MaxLength);
                }

                SetNewCapacity(newCapacity);
                return true;
            }
            // No need to allocate a new buffer , just return with false
            return false;
        }

        /// <summary>
        /// Truncates the memory text source so that it is of the specified length. <br />
        /// If the passed in value exceeds the length of the memory text source, 
        /// then it is appropriately resized to fit the requested length.
        /// </summary>
        /// <param name="value">Number of characters that is the desired length of the source</param>
        /// <exception cref="NotSupportedException">The memory text source is not expandable.</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="value"/> is negative</exception>
        [Throws(typeof(NotSupportedException), typeof(ArgumentOutOfRangeException))]
        public void SetLength(System.Int64 value)
        {
            if (value < 0) {
                throw new ArgumentOutOfRangeException(nameof(value), "Memory text source length cannot be less than zero.");
            } else {
                System.Boolean allocatedNewArray = EnsureCapacity(value);
                if (allocatedNewArray == false && value > length)
                {
                    Unsafe.InitBlockUnaligned(ref Unsafe.As<System.Char, System.Byte>(ref buffer[length]), 0, ((value - length) * sizeof(System.Char)).ToUInt32());
                }
                length = value;
                if (position > value) { position = value; }
            }
        }

        /// <inheritdoc />
        public int Read(Span<char> buffer)
        {
            int count = buffer.Length;
            if (count == 0) {
                return 0; 
            } else if (position >= length) { 
                return -1; 
            } else {
                System.Int64 ccount = position + count > length ? length - position : count;
                // Even if zero this unsafe call should not do anything else but nothing,
                // but the index would hit failures if the provided buffer is zero-length,
                // so this is handled separately above with the if statement.
                Unsafe.CopyBlockUnaligned(ref Unsafe.As<System.Char, System.Byte>(ref MemoryMarshal.GetReference(buffer)), ref Unsafe.As<System.Char, System.Byte>(ref this.buffer[position]), (ccount * sizeof(System.Char)).ToUInt32());
                // Update position
                position += ccount;
                return ccount.ToInt32();
            }
        }

        /// <inheritdoc />
        public void Write(ReadOnlySpan<char> buffer)
        {
            int count = buffer.Length;
            // No meaning to perform anything when the user wants to write nothing.
            if (count == 0) { return; }
            // We must write 'count' bytes to the stream , but what happens when the position has been changed 
            // and the underlying array bounds cannot afford the current capacity?
            System.Int64 fc = capacity, bound = position + count;
            // Just check whether the position + count gets off the current capacity and appropriately update.
            // This should be correct under these cases.
            if (bound >= fc) { fc = bound; }
            // Ensure now the new capacity
            EnsureCapacity(fc);
            // Write the buffered data to our underlying buffer...
            // Note that we do not need to zeroize; the contents will be elsewise overwritten.
            Unsafe.CopyBlockUnaligned(ref Unsafe.As<System.Char, System.Byte>(ref this.buffer[position]), ref Unsafe.As<System.Char, System.Byte>(ref MemoryMarshal.GetReference(buffer)), (count * sizeof(System.Char)).ToUInt32());
            // Update both position and length , if deemed appropriate
            position += count;
            if (position >= length) { length = position; }
        }

        /// <summary>
        /// Provides code that needs to be disposed of. <br />
        /// For derived classes that need to dispose additional data, they should call in their overriden
        /// implementation this implementation by using the <see langword="base"/> convention.
        /// </summary>
        /// <param name="disposing">A value whether the <see cref="Dispose()"/> method performs the disposal of the object.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing) {
                position = length = capacity = 0L;
            }
            buffer = null;
        }

        /// <summary>
        /// Disposes this <see cref="MemoryTextSource"/> object.
        /// </summary>
        public void Dispose()
        {
            // Derived classes: do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Monitor.Enter(this);
            try {
                if (!disposed) { Dispose(true); }
            } finally { 
                disposed = true;
                Monitor.Exit(this); 
            }
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Gets the contents of this memory text source as a string. <br />
        /// Will fail with <see cref="OverflowException"/> if the memory text source length is larger than <see cref="System.Int32.MaxValue"/> characters.
        /// </summary>
        /// <returns>The contents of the memory text source as a string.</returns>
        /// <exception cref="OverflowException">The memory text source length is larger than <see cref="System.Int32.MaxValue"/> characters.</exception>
        [Throws(typeof(OverflowException), typeof(OutOfMemoryException))]
        public override string ToString() => new(buffer, 0, (int)length);
    }
}