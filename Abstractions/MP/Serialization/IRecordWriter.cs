
using System;
using MP.Annotations.CodeAnalysis;

namespace MP.Serialization
{
    /// <summary>
    /// Defines the base of serializing classes through records.
    /// </summary>
    public interface IRecordWriter : IDisposable
    {
        /// <summary>
        /// Writes a new serialized result to the specified stream by the specified record. <br />
        /// The method must dispose any used resources after writing the result.
        /// </summary>
        /// <param name="stream">The stream to write the result to.</param>
        /// <param name="record">The <see cref="Record"/> to write to <paramref name="stream"/>.</param>
        /// <exception cref="ArgumentNullException"><paramref name="stream"/> and/or <paramref name="record"/> were <see langword="null"/>.</exception>
        /// <exception cref="ArgumentException"><paramref name="stream"/> was unwriteable.</exception>
        [Throws(typeof(ArgumentNullException) , typeof(ArgumentException))]
        public void WriteNew(System.IO.Stream stream , Record record);
    }
}