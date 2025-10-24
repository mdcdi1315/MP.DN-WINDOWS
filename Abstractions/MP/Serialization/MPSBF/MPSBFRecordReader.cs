

using System;
using System.IO;

namespace MP.Serialization.MPSBF
{
    /// <summary>
    /// Provides a reader for decoding Serialized Binary Format records.
    /// </summary>
    public sealed class MPSBFRecordReader : IRecordReader
    {
        private Record temp;

        /// <inheritdoc />
        public Record Payload
        {
            get {
                if (temp is null) {
                    throw new InvalidOperationException("The record reader is not yet initialized.");
                }
                return temp;
            }
        }

        /// <summary>
        /// Disposes this record reader, ensuring that all resources held by it have been released.
        /// </summary>
        public void Dispose() 
        {
            temp = null;
            GC.SuppressFinalize(this);
        }

        /// <inheritdoc />
        public void EndPayloadDecoding() => temp = null;

        /// <inheritdoc />
        public void InitializeForNewPayload(Stream stream)
        {
            ArgumentNullException.ThrowIfNull(stream);
            if (!stream.CanRead) {
                throw new ArgumentException("Stream was unreadable." , nameof(stream));
            }
            SBF_HEADER header = stream.ReadStructure<SBF_HEADER>(); 
            if (!header.IsValid) {
                throw new SerializationException($"Invalid header type: {header.Header_0}{header.Header_1}{header.Header_2}.");
            }
            if (header.VersionCode > 0) {
                throw new SerializationException($"Version not supported: {header.VersionCode}");
            }
            temp = Helpers.DecodeRecord(stream);
        }
    }
}