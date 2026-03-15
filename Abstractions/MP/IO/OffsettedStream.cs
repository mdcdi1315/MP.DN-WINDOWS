
using System;

namespace MP.IO
{
    /// <summary>
    /// Provides the base class for offsetted stream implementations.
    /// </summary>
    public abstract class OffsettedStream : WrapperStream
    {
        /// <summary>
        /// Gets the offset to the beginning of the data stream that the current data stream view projects.
        /// </summary>
        protected readonly long Offset;
        /// <summary>
        /// Gets/sets the virtual, NOT the actual position of the wrapped stream. <br />
        /// The actual position can be obtained if doing the following mathematical expression: <br />
        /// <c><see cref="Offset"/> + <see cref="OffsettedPosition"/></c>.
        /// </summary>
        protected long OffsettedPosition;
        /// <summary>
        /// Gets/sets the virtual, NOT the actual length of the wrapped stream.
        /// </summary>
        protected long OffsettedLength;

        /// <summary>
        /// Initializes a new instance of the <see cref="OffsettedStream"/> class.
        /// </summary>
        /// <param name="wrapped">The <see cref="DataStream"/> to be wrapped.</param>
        public OffsettedStream(DataStream wrapped) : base(wrapped)
        {
            Offset = wrapped.Seek(0L, SeekDisplacement.Current);
        }

        /// <inheritdoc />
        public override long Seek(long offset, SeekDisplacement origin) => origin switch 
        {
            SeekDisplacement.Begin => base.Seek(Offset, SeekDisplacement.Begin),
            SeekDisplacement.Current => base.Seek(Offset + OffsettedPosition, SeekDisplacement.Begin),
            SeekDisplacement.End => base.Seek(Offset + OffsettedLength + offset, SeekDisplacement.Begin),
            _ => throw new ArgumentException("Invalid seek origin: " + origin, nameof(origin))
        };

        /// <inheritdoc />
        public override long Position
        {
            get => OffsettedPosition;
            set => base.Seek(Offset + (OffsettedPosition = value), SeekDisplacement.Begin);
        }

        /// <inheritdoc />
        public override long Length => OffsettedLength;
    }
}