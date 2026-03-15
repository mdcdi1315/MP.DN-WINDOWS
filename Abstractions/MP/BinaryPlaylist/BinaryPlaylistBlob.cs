
using MP.IO;
using System;
using MP.PlaylistManagement;
using System.Diagnostics.CodeAnalysis;

namespace MP.BinaryPlaylist
{
    /// <summary>
    /// Provides a layout of a Binary Playlist Blob. <br />
    /// These blobs are constructed on demand at run-time when requested to do so.
    /// </summary>
    public abstract class BinaryPlaylistBlob
    {
        private readonly SaveablePlaylist playlist;
        private readonly BinaryPlaylistBlobInfo info;

        /// <summary>
        /// Constructs a new instance of the <see cref="BinaryPlaylistBlob"/> class.
        /// </summary>
        /// <param name="info">The <see cref="BinaryPlaylistBlobInfo"/> object to construct this instance from.</param>
        /// <param name="playlist">The <see cref="SaveablePlaylist"/> object from which to save data or load data.</param>
        public BinaryPlaylistBlob(BinaryPlaylistBlobInfo info, SaveablePlaylist playlist)
        {
            ArgumentNullException.ThrowIfNull(info);
            ArgumentNullException.ThrowIfNull(playlist);
            this.info = info;
            this.playlist = playlist;
        }

        /// <summary>Loads this blob from the current data stream into memory.</summary>
        /// <param name="stream">A reader-specific <see cref="DataStream"/> object where data are read from.</param>
        public abstract void Load([DisallowNull] DataStream stream);

        /// <summary>Saves this blob from memory to the current data stream.</summary>
        /// <param name="stream">The <see cref="DataStream"/> that can be used to write data.</param>
        public abstract void Save([DisallowNull] DataStream stream);

        /// <summary>Gets information about this blob.</summary>
        public BinaryPlaylistBlobInfo Information => info;

        /// <summary>The playlist to load or save data into.</summary>
        public SaveablePlaylist Playlist => playlist;
    }

}