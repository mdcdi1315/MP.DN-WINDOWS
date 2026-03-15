

using MP.Annotations;
using MP.Annotations.CodeAnalysis;

namespace MP.IO
{
    /// <summary>
    /// Defines an object in the file system - that is, a file or a directory.
    /// </summary>
    public abstract class FileSystemObject
    {
        /// <summary>
        /// Gets a value whether this file system object is actually valid, that is, existing on the file system.
        /// </summary>
        public abstract System.Boolean Exists { get; }

        /// <summary>
        /// Gets the length in bytes of this file system object. <br />
        /// Might not be supported for directories, for such cases 0 must be returned.
        /// </summary>
        public abstract System.Int64 Length { get; }

        /// <summary>
        /// Gets the name of this file system object.
        /// </summary>
        public abstract System.String Name { get; }

        /// <summary>
        /// Gets the parent file system object where this object is located to.
        /// </summary>
        public abstract FileSystemObject Parent { get; }

        /// <summary>
        /// Gets/sets the modification/creation/access times of this file system object.
        /// </summary>
        public abstract FileSystemObjectTimes Times { get; set; }

        /// <summary>
        /// Gets/sets the attributes for this file system object.
        /// </summary>
        public abstract FileAttributes Attributes { get; set; }

        /// <summary>Deletes this <see cref="FileSystemObject"/> from the file system.</summary>
        /// <returns>A value whether the OS managed to delete the specified object.</returns>
        /// <exception cref="IOException">An I/O exception was occurred.</exception>
        [Throws(typeof(IOException))]
        public abstract bool Delete();

        /// <summary>Refreshes the state of this <see cref="FileSystemObject"/> from the OS.</summary>
        /// <exception cref="IOException">An I/O exception was occurred.</exception>
        [Throws(typeof(IOException))]
        public abstract void Refresh();
    }
}
