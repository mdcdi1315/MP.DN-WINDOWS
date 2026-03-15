
using MP.IO;
using System;
using MP.Annotations.CodeAnalysis;

namespace MP.PlaylistManagement
{
    /// <summary>
    /// Defines a playlist that can be reconstructed later. <br />
    /// Playlist objects having the behavior that their data are saved to an external location should extend this class instead.
    /// </summary>
    public abstract class SaveablePlaylist : Playlist
    {
        /// <summary>Saves this playlist instance as-is to the specified stream.</summary>
        /// <param name="stream">The stream to use to save the current playlist to.</param>
        /// <exception cref="ArgumentNullException"><paramref name="stream"/> was <see langword="null"/>.</exception>
        /// <exception cref="ArgumentException"><paramref name="stream"/> was unwriteable and/or unseekable, if the implementation requires that.</exception>
        [Throws(typeof(ArgumentNullException), typeof(ArgumentException))]
        public abstract void SaveTo(IDataStreamAccess stream);
    }
}