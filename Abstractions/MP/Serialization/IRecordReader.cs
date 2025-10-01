

using System;

namespace MP.Serialization
{
    /// <summary>
    /// Defines the base of deserialzing classes into reusuable records.
    /// </summary>
    public interface IRecordReader : IDisposable
    {
        /// <summary>
        /// Initializes the reader for a new payload to be deserialized.
        /// </summary>
        /// <param name="stream">The payload to be deserialized. The stream must be at least readable.</param>
        public void InitializeForNewPayload(System.IO.Stream stream);

        /// <summary>
        /// Indicates to the reader that the payload decoding previously declared with <see cref="InitializeForNewPayload(System.IO.Stream)"/> has now ended. <br />
        /// It is primarily used to flag to the reader that it should internally dispose any used or sensitive data.
        /// </summary>
        public void EndPayloadDecoding();

        /// <summary>
        /// Gets the decoded payload, as a <see cref="Record"/> instance. <br />
        /// For this member to be retrieved the <see cref="InitializeForNewPayload(System.IO.Stream)"/> must have been called and completed without exceptions.
        /// </summary>
        /// <exception cref="InvalidOperationException">The <see cref="InitializeForNewPayload(System.IO.Stream)"/> method has not been called sucessfully before.</exception>
        public Record Payload { get; }
    }
}