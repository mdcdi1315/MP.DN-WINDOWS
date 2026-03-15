

using MP.IO;
using System;

namespace MP.Serialization.MPSBF
{
    /// <summary>
    /// Defines a <see cref="IRecordWriter"/> for encoding <see cref="Record"/>s to Serialized Binary Format.
    /// </summary>
    public sealed class MPSBFRecordWriter : IRecordWriter
    {
        /// <summary>
        /// Disposes this <see cref="MPSBFRecordWriter"/> class instance.
        /// </summary>
        public void Dispose() { }

        /// <inheritdoc />
        public void WriteNew(DataStream stream, Record record)
        {
            ArgumentNullException.ThrowIfNull(stream);
            ArgumentNullException.ThrowIfNull(record);
            if (!stream.CanWrite) {
                throw new ArgumentException("Stream is unwriteable." , nameof(stream));
            }
            new SBF_HEADER().Save(stream);
            Helpers.EncodeRecord(stream, record);
        }
    }
}