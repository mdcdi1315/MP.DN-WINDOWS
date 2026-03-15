
using System;
using MP.ExceptionSystem;
using System.Diagnostics.CodeAnalysis;

namespace MP.BinaryPlaylist
{
    /// <summary>Base exception class for all binary playlist-related exceptions.</summary>
    public class BinaryPlaylistException : BaseException
    {
        /// <summary>
        /// Creates a default instance of the <see cref="BinaryPlaylistException"/> class.
        /// </summary>
        public BinaryPlaylistException() : base() { }

        /// <summary>
        /// Creates a new instance of the <see cref="BinaryPlaylistException"/> class with the specified detailed error message.
        /// </summary>
        /// <param name="message">The error message to show.</param>
        public BinaryPlaylistException([AllowNull] System.String message) : base(message) { }

        /// <summary>
        /// Creates a new instance of the <see cref="BinaryPlaylistException"/> class, with the specified 
        /// detailed error message and the inner exception that is the cause of this exception.
        /// </summary>
        /// <param name="message">The error message to show.</param>
        /// <param name="innerException">The exception that is the cause of this exception.</param>
        public BinaryPlaylistException([AllowNull] System.String message, [AllowNull] Exception innerException) : base(message, innerException) { }

        /// <summary>
        /// Creates a new instance of the <see cref="BinaryPlaylistException"/> class, with the specified 
        /// detailed error message and the inner exception that is the cause of this exception.
        /// </summary>
        /// <param name="message">The error message to show.</param>
        /// <param name="innerException">The exception that is the cause of this exception.</param>
        public BinaryPlaylistException([AllowNull] System.String message, [AllowNull] BinaryPlaylistException innerException) : base(message, innerException) { }
        
        /// <inheritdoc />
        protected override string DefaultSourceValue => "MP Abstractions Library: Binary Playlist subsystem";
    }

    /// <summary>
    /// Must be thrown when the reader encounters a situation where it expected something that should be based on the 
    /// Binary Format rules but it was not there.
    /// </summary>
    public sealed class InvalidBinaryFormatStreamException : BinaryPlaylistException
    {
        /// <summary>
        /// Creates a default instance of the <see cref="InvalidBinaryFormatStreamException"/> class.
        /// </summary>
        public InvalidBinaryFormatStreamException() : base("The stream is not the Music Player Binary Format.") { }

        /// <summary>
        /// Creates a new instance of the <see cref="InvalidBinaryFormatStreamException"/> class with the specified error message that describes this exception.
        /// </summary>
        /// <param name="message">The error message that describes the reason why this exception was thrown.</param>
        public InvalidBinaryFormatStreamException(string message) : base(message) { }
    }

    /// <summary>
    /// Defines the exception that is thrown when attempting to write a blob writer that has not been finalized.
    /// </summary>
    public sealed class BlobWriterIncompleteException : BinaryPlaylistException
    {
        /// <summary>
        /// Creates a default instance of the <see cref="BlobWriterIncompleteException"/> class.
        /// </summary>
        public BlobWriterIncompleteException() : base("The blob writer is yet incomplete and cannot be saved in it's current state.") { }
    }
}
