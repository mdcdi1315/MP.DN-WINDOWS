using MP.ExceptionSystem;

namespace MP.BinaryPlaylist
{
    /// <summary>
    /// Must be thrown when the reader encounters a situation where it expected something that should be based on the 
    /// Binary Format rules but it was not there.
    /// </summary>
    public sealed class InvalidBinaryFormatStreamException : BaseException
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
    /// Defines the exception that is thrown when attempting to write a blob writer that has not been finalized , i.e. has the <see cref="PlaylistBlobWriter.IsCompleted"/> field has the <see langword="false"/> value.
    /// </summary>
    public sealed class BlobWriterIncompleteException : BaseException
    {
        /// <summary>
        /// Creates a default instance of the <see cref="BlobWriterIncompleteException"/> class.
        /// </summary>
        public BlobWriterIncompleteException() : base("The blob writer is yet incomplete and cannot be saved in it's current state.") { }
    }
}
