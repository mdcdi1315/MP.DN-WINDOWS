
using System;
using MP.Annotations.CodeAnalysis;
using System.Diagnostics.CodeAnalysis;

namespace MP.IO
{
    /// <summary>
    /// Defines basic methods and is the abstraction layer for the IO namespace.
    /// </summary>
    public abstract class FileSystem
    {
        /// <summary>
        /// By an abstract file path, relative or not, it returns a <see cref="File"/> object representing the file.
        /// </summary>
        /// <param name="path">The abstract file path to be resolved.</param>
        /// <returns>A <see cref="File"/> corresponding to <paramref name="path"/>.</returns>
        [Throws(typeof(ArgumentNullException))]
        public abstract File GetFile([NotNull] string path);

        /// <summary>
        /// By an abstract directory path, relative or not, it returns a <see cref="Directory"/> object representing the directory.
        /// </summary>
        /// <param name="path">The abstract directory path to be resolved.</param>
        /// <returns>A <see cref="Directory"/> corresponding to <paramref name="path"/>.</returns>
        [Throws(typeof(ArgumentNullException))]
        public abstract Directory GetDirectory([NotNull] string path);

        /// <summary>Gets all the drives mounted on the current OS.</summary>
        /// <returns>A <see cref="Drive"/> array corressponding to all the mounted drives on the system.</returns>
        [return: MaybeReturnEmptyCollectionButNeverNull]
        public abstract Drive[] GetDrives();

        /// <summary>
        /// By a file system object, this method returns a file-system specific string that can be subsequently used to native file API's of the file system.
        /// </summary>
        /// <remarks>
        /// Due to the nature of the method itself, there should be the ability to return a valid <see cref="File"/> or <see cref="Directory"/> object
        /// when passing to either <see cref="GetFile(string)"/> or <see cref="GetDirectory(string)"/> methods the return value of this method.
        /// </remarks>
        /// <param name="o">The file system object to resolve a full file-system path.</param>
        /// <returns>A file-system specific string representing the fully qualified path to the file.</returns>
        [Throws(typeof(ArgumentNullException))]
        public abstract System.String CreateFullPath(FileSystemObject o);
    }
}