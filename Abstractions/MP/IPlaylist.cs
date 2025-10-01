
using System;
using MP.Annotations;
using System.Collections.Generic;

namespace MP
{
    /// <summary>
    /// Defines a Music Player playlist.
    /// </summary>
    [DeprecatedMayBeRemoved("2.0.0.0")]
    public interface IPlaylist : IDisposable
    {
        /// <summary>
        /// Gets the playlist's name.
        /// </summary>
        public System.String PlaylistName { get; }

        /// <summary>
        /// Gets the playlist files that this playlist does contain.
        /// </summary>
        public IList<IPlaylistFile> TracksContained { get; }

        /// <summary>
        /// Gets the current track that is subject to playback.
        /// </summary>
        public IPlaylistFile CurrentTrack { get; set; }

        /// <summary>
        /// Gets the current track time on playback. Usually this is set when the playlist is about to close.
        /// </summary>
        public TimeSpan CurrentTrackProcessedTime { get; set; }

        /// <summary>
        /// Gets all the current preferences that comprise this playlist.
        /// </summary>
        public PlaylistPreferences Preferences { get; }

        /// <summary>
        /// Gets a value whether all the audio tags have been retrieved from the respective tracks.
        /// </summary>
        public System.Boolean AudioTagsDetermined { get; }

        /// <summary>
        /// Gets the metadata defined for this playlist instance.
        /// </summary>
        public PlaylistMetadataItemCollection Metadata { get; }

        /// <summary>
        /// Determines all the audio tags at once.
        /// </summary>
        public void DetermineAudioTags();

        /// <summary>
        /// Gets the <see cref="CurrentTrack"/> property as an array index inside the <see cref="TracksContained"/> list.
        /// </summary>
        /// <returns>The array index inside the <see cref="TracksContained"/> property.</returns>
        public System.Int32 GetCurrentTrackIndex();

        /// <summary>
        /// Removes the specified track from the playlist.
        /// </summary>
        /// <param name="track">The track to remove.</param>
        /// <returns>A value whether the track was found and removed successfully.</returns>
        public System.Boolean Remove(IPlaylistFile track);

        /// <summary>
        /// Removes the specified track from the playlist , referenced at <paramref name="index"/> parameter.
        /// </summary>
        /// <param name="index">The track index to remove.</param>
        /// <returns>A value whether the track was found and removed successfully.</returns>
        public System.Boolean Remove(System.Int32 index);

        /// <summary>
        /// Determines an audio tag and returns the tag data for the given file.
        /// </summary>
        /// <param name="file">The determined audio tag.</param>
        /// <returns>The requested audio tag.</returns>
        /// <remarks>
        /// See also the <see cref="ITagReader2"/> and <see cref="IOggTagReaderBase"/> interfaces.
        /// </remarks>
        public ITagReader DetermineAudioTag(IPlaylistFile file);

        /// <summary>
        /// Gets an already determined audio tag for the specified <paramref name="file"/>.
        /// </summary>
        /// <param name="file">The file to determine the tag for.</param>
        /// <returns>The already saved tag.</returns>
        public SavedDataTag GetTagFromFile(IPlaylistFile file);

        /// <summary>
        /// Determines a tag for the specified playlist file or updates the tag.
        /// </summary>
        /// <param name="file">The file to determine the audio tag for.</param>
        public void DetermineAudioTagAndAddToTagList(IPlaylistFile file);
    }

}