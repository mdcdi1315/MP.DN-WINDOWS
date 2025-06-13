namespace MP.ExceptionSystem
{
    /// <summary>
    /// Thrown when any member in the <see cref="ArchivedPlaylistAttribute"/> class
    /// is modified after the <see cref="ArchivedPlaylistAttributeFlags.ReadOnly"/> flag is specified.
    /// </summary>
    public sealed class ReadOnlyArchiveAttributeException : BaseException
    {
        private System.String name;

        /// <summary>
        /// Constructs a new <see cref="ReadOnlyArchiveAttributeException"/> instance with the specified
        /// attribute name that caused this exception.
        /// </summary>
        /// <param name="attributename">The name of the attribute that caused this exception.</param>
        public ReadOnlyArchiveAttributeException(System.String attributename) 
            : base($"The archive attribute {attributename} is read-only and cannot be modified.")
        {
            name = attributename;
        }

        /// <summary>
        /// Gets the name of the archived attribute that caused this exception.
        /// </summary>
        public System.String AttributeName => name;
    }
}