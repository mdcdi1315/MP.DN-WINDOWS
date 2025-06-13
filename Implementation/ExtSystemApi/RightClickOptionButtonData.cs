

namespace MP.ExtSystemApi
{
    public struct RightClickOptionButtonData
    {
        /// <summary>
        /// Identifies the UI to be finally invoked
        /// </summary>
        public System.Object Tag;
        /// <summary>
        /// Identifies the current playlist at the time this was called.
        /// </summary>
        public IPlaylist Playlist;
        /// <summary>
        /// Identifies the track that was selected.
        /// </summary>
        public IPlaylistFile Current;
    }
}