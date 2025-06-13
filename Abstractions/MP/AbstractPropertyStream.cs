
using System;
using MP.Utilities;
using System.Collections.Generic;

namespace MP
{
    /// <summary>
    /// Defines a data stream that carries, along the stream, additional attributes for it. <br />
    /// The attributes for a data stream are called stream properties too.
    /// </summary>
    public abstract class AbstractPropertyStream : System.IO.Stream, IAttributeable , IStreamOwnerBase
    {
        /// <summary>
        /// Specifies the stream property that will auto-dispose the stream upon calling the current <see cref="System.IO.Stream.Dispose()"/> method.
        /// </summary>
        public const System.String IsStreamOwnerProperty = "AutoDispose";
        /// <summary>
        /// Specifies the stream property that will auto-dispose the stream in any way. You should use this property to force disposal in any way.
        /// </summary>
        protected const System.String IsStreamOwnerForced = "ForceAutoDispose";

        /// <summary>
        /// Placeholder variable that keeps the underlying stream.
        /// </summary>
        protected System.IO.Stream Wrapped;
        private readonly System.Boolean sync;
        private Dictionary<System.String, System.Object> attributes;

        /// <summary>
        /// When the constructor alternatives are undesired.
        /// </summary>
        protected AbstractPropertyStream() : base()
        {
            Wrapped = null;
            sync = false;
            attributes = new(10);
            attributes.Add(IsStreamOwnerProperty, false);
            attributes.Add(IsStreamOwnerForced , false);
        }

        /// <summary>
        /// Creates a new instance of the <see cref="AbstractPropertyStream"/> class by specifying the stream to wrap.
        /// </summary>
        /// <param name="wrappingstream">The stream to wrap.</param>
        /// <exception cref="ArgumentNullException"><paramref name="wrappingstream"/> was null.</exception>
        public AbstractPropertyStream(System.IO.Stream wrappingstream) : this(wrappingstream, false) { }

        /// <summary>
        /// Creates a new instance of the <see cref="AbstractPropertyStream"/> class by specifying the stream to wrap.
        /// </summary>
        /// <param name="wrappingstream">The stream to wrap.</param>
        /// <param name="synchronize">A value whether the underlying stream should be synchronized.</param>
        /// <exception cref="ArgumentNullException"><paramref name="wrappingstream"/> was null.</exception>
        public AbstractPropertyStream(System.IO.Stream wrappingstream, System.Boolean synchronize) : this()
        {
            if (wrappingstream is null) { throw new ArgumentNullException(nameof(wrappingstream)); }
            sync = synchronize;
            if (sync) {
                Wrapped = Synchronized(wrappingstream);
            } else {
                Wrapped = wrappingstream;
            }
        }

        /// <summary>
        /// Gets or sets a value whether the <see cref="AbstractPropertyStream"/> class instance should also dispose
        /// the underlying stream upon calling <see cref="System.IO.Stream.Dispose()"/>.
        /// </summary>
        public System.Boolean IsStreamOwner
        {
            get => this.GetBooleanAttribute(IsStreamOwnerProperty);
            set => this.SetBooleanAttribute(IsStreamOwnerProperty, value);
        }

        /// <summary>
        /// Gets a value whether this stream wrapper is syncronized. <br />
        /// (that is , the object returned from <see cref="System.IO.Stream.Synchronized(System.IO.Stream)"/>).
        /// </summary>
        public System.Boolean IsSyncronized => sync;

        /// <summary>
        /// Gets a value whether the underlying stream is readable.
        /// </summary>
        public override System.Boolean CanRead => Wrapped.CanRead;

        /// <summary>
        /// Gets a value whether the underlying stream is seekable.
        /// </summary>
        public override System.Boolean CanSeek => Wrapped.CanSeek;

        /// <summary>
        /// Gets a value whether the underlying stream is writeable.
        /// </summary>
        public override System.Boolean CanWrite => Wrapped.CanWrite;

        /// <summary>
        /// Gets the underlying stream length.
        /// </summary>
        public override System.Int64 Length => Wrapped.Length;

        /// <summary>
        /// Gets or sets the position within the underlying stream.
        /// </summary>
        /// <returns>The current position within the underlying stream.</returns>
        public override System.Int64 Position
        {
            get => Wrapped.Position;
            set => Wrapped.Position = value;
        }

        /// <inheritdoc />
        public System.Object GetAttribute(string name)
        {
            if (System.String.IsNullOrEmpty(name)) { throw new ArgumentNullException(nameof(name), "Attribute name must not be the empty string."); }
            try {
                return attributes[name];
            } catch (KeyNotFoundException) {
                throw new ExceptionSystem.AttributeNotFoundException(name);
            }
        }
        
        /// <inheritdoc />
        public void SetAttribute(string name, System.Object value)
        {
            if (System.String.IsNullOrEmpty(name)) { throw new ArgumentNullException(nameof(name), "Attribute name must not be the empty string."); }
            attributes[name] = value;
        }

        /// <summary>
        /// Queries for the given property; if it does exist it returns the boxed value , otherwise returns null.
        /// </summary>
        /// <param name="name">The property name to query.</param>
        /// <returns>The property value if that property was found.</returns>
        public System.Object QueryAttribute(System.String name)
        {
            System.Object result;
            attributes.TryGetValue(name, out result);
            return result;
        }

        /// <summary>
        /// Reads a sequence of bytes from the underlying stream and advances the position by the number of bytes read.
        /// </summary>
        /// <param name="buffer">An array of bytes. When this method returns, the <paramref name="buffer"/> contains the specified byte array with the values between <paramref name="offset"/> and (<paramref name="offset"/> + <paramref name="count"/> - 1) replaced by the bytes read from the current underlying source.</param>
        /// <param name="offset">The zero-based byte offset in <paramref name="buffer"/> at which to begin storing the data read from the underlying stream.</param>
        /// <param name="count">The maximum number of bytes to be read from the current stream.</param>
        /// <returns>The total number of bytes read into the buffer. This can be less than the number of bytes requested if that many bytes are not currently available, or zero (0) if <paramref name="count"/> is 0 or the end of the underlying stream has been reached.</returns>
        /// <exception cref="ArgumentException">The sum of <paramref name="offset"/> and <paramref name="count"/> is larger than the buffer length.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="buffer"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="offset"/> or <paramref name="count"/> is negative.</exception>
        /// <exception cref="System.IO.IOException">An I/O error occurs.</exception>
        /// <exception cref="NotSupportedException">The stream does not support reading.</exception>
        /// <exception cref="ObjectDisposedException">Methods were called after the stream was closed.</exception>
        public override System.Int32 Read(byte[] buffer, int offset, int count) => Wrapped.Read(buffer, offset, count);

        /// <summary>
        /// Writes a sequence of bytes to the underlying stream and advances the current position within that stream by the number of bytes written.
        /// </summary>
        /// <param name="buffer">An array of bytes. This method copies <paramref name="count"/> bytes from <paramref name="buffer"/> to the current stream.</param>
        /// <param name="offset">The zero-based byte offset in <paramref name="buffer"/> at which to begin copying bytes to the current stream.</param>
        /// <param name="count">The number of bytes to be written to the underlying stream.</param>
        /// <exception cref="ArgumentException">The sum of <paramref name="offset"/> and <paramref name="count"/> is larger than the buffer length.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="buffer"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="offset"/> or <paramref name="count"/> is negative.</exception>
        /// <exception cref="System.IO.IOException">An I/O error occurs.</exception>
        /// <exception cref="NotSupportedException">The stream does not support writing.</exception>
        /// <exception cref="ObjectDisposedException"><see cref="Write(byte[], int, int)"/> was called after the stream was closed.</exception>
        public override void Write(byte[] buffer, int offset, int count) => Wrapped.Write(buffer, offset, count);

        /// <summary>
        /// Sets the length of the underlying stream.
        /// </summary>
        /// <param name="value">The desired length of the underlying stream in bytes.</param>
        /// <exception cref="System.IO.IOException">An I/O error occurs.</exception>
        /// <exception cref="NotSupportedException">The underlying stream does not support both writing and seeking, such as if it was constructed from a pipe or console output.</exception>
        /// <exception cref="ObjectDisposedException"><see cref="SetLength(long)"/> was called after the stream was closed.</exception>
        public override void SetLength(long value) => Wrapped.SetLength(value);

        /// <summary>
        /// Sets the position within the underlying stream.
        /// </summary>
        /// <param name="offset">A byte offset relative to the <paramref name="origin"/> parameter.</param>
        /// <param name="origin">A value of type <see cref="System.IO.SeekOrigin"/> indicating the reference point used to obtain the new position.</param>
        /// <returns>The new position within the current stream.</returns>
        /// <exception cref="System.IO.IOException">An I/O error occurs.</exception>
        /// <exception cref="NotSupportedException">The underlying stream does not support seeking, such as if it was constructed from a pipe or console output.</exception>
        /// <exception cref="ObjectDisposedException"><see cref="Seek(long, System.IO.SeekOrigin)"/> was called after the stream was closed.</exception>
        public override System.Int64 Seek(long offset, System.IO.SeekOrigin origin) => Wrapped.Seek(offset, origin);

        /// <summary>
        /// Clears all buffers for the underlying stream and causes any buffered data to be written to the underlying device.
        /// </summary>
        /// <exception cref="System.IO.IOException">An I/O error occurs.</exception>
        public override void Flush() => Wrapped.Flush();

        /// <summary>
        /// Releases all the resources used by this <see cref="AbstractPropertyStream"/> instance, optionally freeing all the unmanaged resources too. <br />
        /// When you override this method , do not forget to call this one by using the base.Dispose convention.
        /// </summary>
        /// <param name="disposing">A value whether to free all managed resources too.</param>
        protected override void Dispose(bool disposing)
        {
            if (attributes is not null)
            {
                if (this.GetBooleanAttribute(IsStreamOwnerProperty) || this.GetBooleanAttribute(IsStreamOwnerForced)) { Wrapped?.Dispose(); }
                attributes.Clear();
                attributes = null;
            }
            Wrapped = null;
            base.Dispose(disposing);
        }
    }
}