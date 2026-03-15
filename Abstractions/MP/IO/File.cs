
using System;
using MP.Annotations;
using MP.Annotations.CodeAnalysis;
using System.Diagnostics.CodeAnalysis;

namespace MP.IO
{
    /// <summary>
    /// Provides the <see cref="FileSystemObject"/> that is a file object. <br />
    /// Should be implemented by interoped API's
    /// </summary>
    public abstract class File : FileSystemObject
    {
        /// <summary>
        /// Provides the <see cref="Directory"/> object where this <see cref="File"/> object is located to.
        /// </summary>
        [MaybeNull]
        public override abstract Directory Parent { get; }

        /// <summary>Copies the current <see cref="File"/> object to the specified target file.</summary>
        /// <param name="target">The target <see cref="File"/> where the contents of this <see cref="File"/> object should be copied to.</param>
        /// <param name="options">Additional copy options to apply when the <see cref="File"/> will be copied.</param>
        /// <exception cref="ArgumentNullException"><paramref name="target"/> is <see langword="null"/>.</exception>
        [Throws(typeof(ArgumentNullException))]
        public abstract void CopyTo(File target, CopyMoveFileOptions options);

        /// <summary>Moves the current <see cref="File"/> object to the specified target file.</summary>
        /// <param name="target">The target <see cref="File"/> where the contents of this <see cref="File"/> object should be moved to.</param>
        /// <param name="options">Additional move options to apply when the <see cref="File"/> will be moved.</param>
        /// <exception cref="ArgumentNullException"><paramref name="target"/> is <see langword="null"/>.</exception>
        [Throws(typeof(ArgumentNullException))]
        public abstract void MoveTo(File target, CopyMoveFileOptions options);

        /// <summary>Opens a <see cref="AbstractFileStream"/> to the current <see cref="File"/> object.</summary>
        /// <param name="mode">The file mode to open the file under.</param>
        /// <param name="access">Permissions required for manipulating the data stream.</param>
        /// <param name="share">Sharing options required for manipulating the data stream.</param>
        /// <returns>A new <see cref="AbstractFileStream"/> representing a handle to the opened file.</returns>
        [return: NotNull]
        public abstract AbstractFileStream Open(FileMode mode, FileAccess access, FileShare share);

        /// <summary>Opens a <see cref="AbstractFileStream"/> to the current <see cref="File"/> object for reading.</summary>
        /// <returns>A new <see cref="AbstractFileStream"/> representing a handle to the opened file.</returns>
        [return: NotNull]
        public virtual AbstractFileStream OpenRead() => Open(FileMode.Open, FileAccess.Read, FileShare.Read);

        /// <summary>Opens a <see cref="AbstractFileStream"/> to the current <see cref="File"/> object for writing.</summary>
        /// <returns>A new <see cref="AbstractFileStream"/> representing a handle to the opened file.</returns>
        public virtual AbstractFileStream OpenWrite() => Open(FileMode.OpenOrCreate, FileAccess.Write, FileShare.None);
    }
}
