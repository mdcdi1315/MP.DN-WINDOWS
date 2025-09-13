


using System;

namespace MP.IO
{
    /// <summary>
    /// A structure defining all the supported file system object times.
    /// </summary>
    public readonly struct FileSystemObjectTimes
    {
        /// <summary>Gets the creation time.</summary>
        public readonly DateTime CreationTime;
        /// <summary>Gets the modification time.</summary>
        public readonly DateTime ModificationTime;
        /// <summary>Gets the last access time.</summary>
        public readonly DateTime LastAccessTime;

        /// <summary>
        /// Constructor for initializing the supported fields with valid values.
        /// </summary>
        /// <param name="creation">The creation time to specify.</param>
        /// <param name="modificationtime">The modification time to specify.</param>
        /// <param name="lastaccess">The last access time to specify.</param>
        public FileSystemObjectTimes(DateTime creation , DateTime modificationtime, DateTime lastaccess)
        {
            CreationTime = creation;
            ModificationTime = modificationtime;
            LastAccessTime = lastaccess;
        }

        /// <summary>
        /// Modifies the creation date and time and returns the result as a new <see cref="FileSystemObjectTimes"/> structure. <br />
        /// The rest fields do remain unchanged.
        /// </summary>
        /// <param name="creationTime">The new creation date and time.</param>
        /// <returns>A new instance of the <see cref="FileSystemObjectTimes"/> structure.</returns>
        public FileSystemObjectTimes WithNewCreationTime(DateTime creationTime) => new(creationTime, ModificationTime, LastAccessTime);

        /// <summary>
        /// Modifies the modification date and time and returns the result as a new <see cref="FileSystemObjectTimes"/> structure. <br />
        /// The rest fields do remain unchanged.
        /// </summary>
        /// <param name="modtime">The new modification date and time.</param>
        /// <returns>A new instance of the <see cref="FileSystemObjectTimes"/> structure.</returns>
        public FileSystemObjectTimes WithNewModificationTime(DateTime modtime) => new(CreationTime, modtime, LastAccessTime);

        /// <summary>
        /// Modifies the last access date and time and returns the result as a new <see cref="FileSystemObjectTimes"/> structure. <br />
        /// The rest fields do remain unchanged.
        /// </summary>
        /// <param name="accesstime">The new last access date and time.</param>
        /// <returns>A new instance of the <see cref="FileSystemObjectTimes"/> structure.</returns>
        public FileSystemObjectTimes WithNewLastAccessTime(DateTime accesstime) => new(CreationTime, ModificationTime, accesstime);
    }
}