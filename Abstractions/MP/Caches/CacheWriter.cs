

using System;

namespace MP.Caches
{
    /// <summary>
    /// Defines the abstraction for the cached data defined by the Music Player app.
    /// </summary>
    public abstract class CacheWriter : IDisposable
    {
        [Flags]
        private enum CWFLAGS : System.Byte
        {
            None = 0,
            Generated = 0x02,
            StreamOwner = 0x04,
            WroteHeader = 0x08
        }

        /// <summary>
        /// Defines the stream where all the cache data are written.
        /// </summary>
        protected System.IO.Stream Writer;
        /// <summary>
        /// Defines a unique identifier that does uniquely identify the current cache stream.
        /// </summary>
        protected System.Int64 UniqueTypeIdentifier;
        /// <summary>
        /// Gets the version of the cache stream. Can be used by the deriving readers.
        /// </summary>
        protected System.UInt16 Version;
        private CWFLAGS flags;

        /// <summary>
        /// Default constructor.
        /// </summary>
        protected CacheWriter() { flags = CWFLAGS.None; }

        /// <summary>
        /// This must be called by your extending writer in order to initialize the private data that this class defines.
        /// </summary>
        /// <param name="stream">The stream where this writer will write data to.</param>
        /// <param name="parametername">The name of the parameter of the stream parameter in your constructor that calls in this method.</param>
        /// <exception cref="ArgumentNullException"><paramref name="stream"/> was <see langword="null"/>.</exception>
        /// <exception cref="ArgumentException"><paramref name="stream"/> was unwriteable.</exception>
        [System.Diagnostics.DebuggerHidden]
        protected void Initialize(System.IO.Stream stream , System.String parametername)
        {
            if (stream is null) { throw new ArgumentNullException(parametername); }
            if (stream.CanWrite == false) { throw new ArgumentException("Stream was unwriteable." , parametername); }
            Writer = stream;
        }

        /// <summary>
        /// Writes the current cache format header. <br />
        /// Must be written before all the data are written to the cache stream.
        /// </summary>
        protected void WriteHeader()
        {
            MPCACHEHEADER header = new();
            header.FormatVersion = Version;
            header.FormatIdentifier = UniqueTypeIdentifier;
            Writer.WriteStructure(header);
            flags |= CWFLAGS.WroteHeader;
        }

        /// <summary>
        /// Gets or sets a value whether this instance has lifetime control over the underlying stream.
        /// </summary>
        public System.Boolean IsStreamOwner
        {
            get => flags.HasFlag(CWFLAGS.StreamOwner);
            set {
                if (value) { 
                    flags |= CWFLAGS.StreamOwner; 
                } else {
                    if (flags.HasFlag(CWFLAGS.StreamOwner)) {
                        flags ^= CWFLAGS.StreamOwner;
                    }
                }
            }
        }

        /// <summary>
        /// Custom disposal code that are needed by derived classes should be executed here.
        /// </summary>
        /// <param name="disposing"></param>
        protected virtual void Dispose(System.Boolean disposing) { }

        /// <summary>
        /// Disposes this Cache Writer.
        /// </summary>
        /// <exception cref="InvalidOperationException">The cache header was not written.</exception>
        public void Dispose()
        {
            if ((flags & CWFLAGS.WroteHeader) != CWFLAGS.WroteHeader) {
                throw new InvalidOperationException("Header not generated yet.");
            }
            try {
                Dispose(true);
            } catch (System.Exception e) {
                throw new AggregateException("Custom disposal code threw an exception. This is unexpected.", e);
            }
            if (flags.HasFlag(CWFLAGS.StreamOwner) && Writer is not null) {
                Writer.Dispose();
            }
            Writer = null;
            GC.SuppressFinalize(this);
        }
    }
}