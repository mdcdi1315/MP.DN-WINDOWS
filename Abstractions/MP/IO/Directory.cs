
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using MP.Annotations;
using MP.Annotations.CodeAnalysis;

namespace MP.IO
{
    /// <summary>
    /// Provides the <see cref="FileSystemObject"/> that is a directory object. <br />
    /// Should be implemented by interoped API's
    /// </summary>
    public abstract class Directory : FileSystemObject
    {
        /// <summary>
        /// Moves the current contents of the current <see cref="Directory"/> to another <see cref="Directory"/>.
        /// </summary>
        /// <param name="target">The target <see cref="Directory"/> to move the data to.</param>
        /// <param name="options">Additional options to use for when moving the <see cref="Directory"/>.</param>
        /// <exception cref="System.IO.IOException">An I/O exception was occurred.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="target"/> is <see langword="null"/>.</exception>
        /// <exception cref="UnauthorizedAccessException">Access to the <paramref name="target"/> is denied.</exception>
        [Throws(typeof(ArgumentNullException))]
        public abstract void MoveTo(Directory target, CopyMoveDirectoryOptions options);

        /// <summary>
        /// Moves the current contents of the current <see cref="Directory"/> to another <see cref="Directory"/>.
        /// </summary>
        /// <param name="target">The target <see cref="Directory"/> to move the data to.</param>
        /// <param name="options">Additional options to use for when moving the <see cref="Directory"/>.</param>
        /// <exception cref="System.IO.IOException">An I/O exception was occurred.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="target"/> is <see langword="null"/>.</exception>
        /// <exception cref="UnauthorizedAccessException">Access to the <paramref name="target"/> is denied.</exception>
        [Throws(typeof(ArgumentNullException))]
        public abstract void CopyTo(Directory target, CopyMoveDirectoryOptions options);

        /// <summary>
        /// Deletes this <see cref="Directory"/> object.
        /// </summary>
        /// <param name="recursive">A value whether all the contents of the directory should be deleted as well.</param>
        /// <returns>A value whether the <see cref="Directory"/> was deleted.</returns>
        /// <exception cref="System.IO.IOException">An I/O exception was occurred.</exception>
        public abstract bool Delete(bool recursive);

        /// <summary>
        /// Gets the parent <see cref="Directory"/> object where this object belongs to. <br />
        /// Can be <see langword="null"/> if the directory is located at the root of the drive.
        /// </summary>
        [MaybeNull]
        public abstract override Directory Parent { get; }

        /// <summary>
        /// Gets a subdirectory on the current <see cref="Directory"/> object. <br />
        /// The <see cref="Directory"/> returned may be existing or not. <br />
        /// However, the directory will be created in event that it does not exist.
        /// </summary>
        /// <param name="name">The name of the subdirectory to retrieve or create.</param>
        /// <returns>A <see cref="Directory"/> representing the created sub-directory object.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="name"/> is <see langword="null"/>.</exception>
        [Throws(typeof(ArgumentNullException))]
        public abstract Directory Subdirectory(System.String name);

        /// <summary>Deletes this <see cref="Directory"/> object.</summary>
        /// <returns>A value whether the <see cref="Directory"/> was deleted.</returns>
        public sealed override bool Delete() => Delete(false);

        #region Enumeration

        /// <summary>
        /// Gets all the <see cref="File"/> objects contained in the current <see cref="Directory"/> object.
        /// </summary>
        /// <returns>An implemented <see cref="IEnumerable{T}"/> interface object that returns abstract <see cref="File"/> objects.</returns>
        [return: MaybeReturnEmptyCollectionButNeverNull]
        public virtual IEnumerable<File> GetFiles()
        {
            foreach (var o in GetObjects())
            {
                if (o is File f) { yield return f; }
            }
        }

        /// <summary>
        /// Gets all the child <see cref="Directory"/> objects contained in the current <see cref="Directory"/> object.
        /// </summary>
        /// <returns>An implemented <see cref="IEnumerable{T}"/> interface object that returns abstract <see cref="Directory"/> objects.</returns>
        [return: MaybeReturnEmptyCollectionButNeverNull]
        public virtual IEnumerable<Directory> GetDirectories()
        {
            foreach (var o in GetObjects())
            {
                if (o is Directory d) { yield return d; }
            }
        }

        /// <summary>
        /// Gets all the <see cref="File"/> and <see cref="Directory"/> objects contained in the current <see cref="Directory"/> object.
        /// </summary>
        /// <param name="recursive">Whether to perform a recursive search instead.</param>
        /// <returns>An implemented <see cref="IEnumerable{T}"/> interface object that returns abstract <see cref="File"/> and <see cref="Directory"/> objects.</returns>
        public abstract IEnumerable<FileSystemObject> GetObjects(bool recursive = false);

        #endregion
    }
}
