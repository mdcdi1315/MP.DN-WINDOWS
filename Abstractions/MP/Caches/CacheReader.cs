

using System;

namespace MP.Caches
{
    /// <summary>
    /// Defines the abstraction for the cached data defined by the Music Player app.
    /// </summary>
    public abstract class CacheReader : IDisposable
    {
        /// <summary>
        /// Defines the stream where the extending reader can read the cached data from.
        /// </summary>
        protected System.IO.Stream Reader;
        /// <summary>
        /// Defines the base offset from the beginning of the stream where the extending reader must start reading from.
        /// </summary>
        protected System.Int64 BaseOffsetToData;
        /// <summary>
        /// Gets the unique identifier of this cache stream. Used for validation and if set it does not change the reader state.
        /// </summary>
        protected System.Int64 UniqueTypeIdentifier;
        /// <summary>
        /// Gets a custom version identifier of this cache stream. Used for validation and if set it does not change the reader state.
        /// </summary>
        protected System.UInt16 Version;
        private System.Boolean strmown;

        /// <summary>
        /// Default constructor.
        /// </summary>
        protected CacheReader() { strmown = false; }

        /// <summary>
        /// IMPORTANT: Must be called before any cache read!!!
        /// </summary>
        [System.Diagnostics.DebuggerHidden]
        protected void Initialize(System.IO.Stream stream , System.String parametername)
        {
            if (stream is null) { throw new ArgumentNullException(parametername); }
            if (stream.CanSeek == false) { throw new ArgumentException("Stream was unseekable." , parametername); }
            if (stream.CanRead == false) { throw new ArgumentException("Stream was unreadable.", parametername); }
            MPCACHEHEADER header = stream.ReadStructure<MPCACHEHEADER>();
            if (header.IsValid == false) { throw new CacheFormatInvalidException("This stream does not contain a MP Cache file."); }
            BaseOffsetToData = stream.Position;
            UniqueTypeIdentifier = header.FormatIdentifier;
            Version = header.FormatVersion;
            Reader = stream;
        }

        /// <summary>
        /// Gets or sets a value whether this instance has lifetime control over the underlying stream.
        /// </summary>
        public System.Boolean IsStreamOwner
        {
            get => strmown;
            set => strmown = value;
        }

        /// <summary>
        /// Custom disposal code that are needed by derived classes should be executed here.
        /// </summary>
        /// <param name="disposing"></param>
        protected virtual void Dispose(System.Boolean disposing) { }

        /// <summary>
        /// Disposes this Cache Reader.
        /// </summary>
        public void Dispose()
        {
            try {
                Dispose(true);
            } catch (System.Exception e) {
                throw new AggregateException("Custom disposal code threw an exception. This is unexpected.", e);
            }
            if (strmown && Reader is not null) {
                Reader.Dispose();
            }
            Reader = null;
            GC.SuppressFinalize(this);
        }

    }
}