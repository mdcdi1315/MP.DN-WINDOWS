

namespace MP.ExceptionSystem
{
    /// <summary>
    /// Thrown when an metadata item was not found. <br />
    /// Used for the <see cref="PlaylistMetadataItemCollection"/> class.
    /// </summary>
    public sealed class MetadataItemNotFoundException : BaseException
    {
        private System.String metadataname;

        /// <summary>
        /// Creates a new instance of the <see cref="MetadataItemNotFoundException"/> class with the specified item name that was not found.
        /// </summary>
        /// <param name="metadataname">The item's name that is the cause of this exception.</param>
        public MetadataItemNotFoundException(System.String metadataname)
            : base($"The specified metadata item was not found. \nItem Name: {metadataname}")
        {
            this.metadataname = metadataname;
        }

        /// <summary>
        /// Gets the name of the metadata item that caused this exception.
        /// </summary>
        public System.String Name => metadataname;
    }
}