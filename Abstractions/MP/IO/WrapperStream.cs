namespace MP.IO
{
    /// <summary>
    /// Defines a wrapper stream that wraps the specified stream. <br />
    /// This is mainly to avoid calling <see cref="DataStream.Dispose()"/> and related methods on the wrapped stream,
    /// due to how an API is developed.
    /// </summary>
    public class WrapperStream : BasicWrapperStream
    {
        /// <summary>
        /// Creates a new instance of the <see cref="WrapperStream"/> class, specifying the stream to wrap.
        /// </summary>
        /// <param name="wrap">The <see cref="IDataStreamAccess"/> object to wrap.</param>
        public WrapperStream(DataStream wrap) : base(wrap) { }

        /// <inheritdoc />
        public override long Length => ((DataStream)wrapped).Length;

        /// <inheritdoc />
        public override long Position 
        { 
            get => ((DataStream)wrapped).Position;
            set => ((DataStream)wrapped).Position = value; 
        }

        /// <inheritdoc />
        public override long Seek(long offset, SeekDisplacement displacement) => ((DataStream)wrapped).Seek(offset, displacement);
    }
}
