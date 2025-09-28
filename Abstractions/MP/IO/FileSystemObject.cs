

using MP.Annotations;

namespace MP.IO
{
    /// <summary>
    /// Defines an object in the file system - that is, a file or a directory.
    /// </summary>
    [Preliminary]
    public abstract class FileSystemObject
    {
        /// <summary>
        /// Gets the absolute path of this file system object.
        /// </summary>
        public abstract System.String FullName { get; }

        /// <summary>
        /// Gets a value whether this file system object is actually valid, that is, existing on the file system.
        /// </summary>
        public abstract System.Boolean Exists { get; }

        /// <summary>
        /// Gets the length in bytes of this file system object. <br />
        /// Might not be supported for directories, for such cases 0 must be returned.
        /// </summary>
        public abstract System.Int64 Length { get; }



    }
}
