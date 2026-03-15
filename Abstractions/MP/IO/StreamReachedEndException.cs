using System;
using System.Diagnostics.CodeAnalysis;

namespace MP.IO
{
    /// <summary>
    /// A special case of the <see cref="IOException"/> class, that is thrown when a 
    /// method call requires the stream seek pointer to not point to the end of it. <br />
    /// Typically used by reading methods.
    /// </summary>
    public sealed class StreamReachedEndException : IOException
    {
        private const System.String DefaultMsg = "The specified stream has reached it's end.";

        /// <summary>
        /// Constructs a new instance of the <see cref="StreamReachedEndException"/> class, specifying a default message.
        /// </summary>
        public StreamReachedEndException() : base(DefaultMsg) { }

        /// <summary>
        /// Constructs a new instance of the <see cref="StreamReachedEndException"/> class, specifying the exact error that was occurred.
        /// </summary>
        /// <param name="message">The error that was occurred. Can be <see langword="null"/>. If <see langword="null"/>, the default error message is appended instead.</param>
        public StreamReachedEndException([AllowNull] String message) : base(message ?? DefaultMsg) { }
    }
}
