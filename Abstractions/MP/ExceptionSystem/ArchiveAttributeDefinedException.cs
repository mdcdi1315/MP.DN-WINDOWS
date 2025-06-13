

namespace MP.ExceptionSystem
{
    /// <summary>
    /// Thrown when attempted to add an archive attribute with the same name in a <see cref="ArchivedPlaylistAttributeCollection"/> instance.
    /// </summary>
    public sealed class ArchiveAttributeDefinedException : BaseException
    {
        private System.String name;

        /// <summary>
        /// Constructs a new <see cref="ArchiveAttributeDefinedException"/> instance with the specified
        /// attribute name that caused this exception.
        /// </summary>
        /// <param name="attributename">The name of the attribute that caused this exception.</param>
        public ArchiveAttributeDefinedException(System.String attributename)
            : base($"The archive attribute {attributename} has already been defined in the collection.")
        {
            name = attributename;
        }

        /// <summary>
        /// Gets the name of the archived attribute that caused this exception.
        /// </summary>
        public System.String AttributeName => name;
    }
}