namespace MP
{
    /// <summary>
    /// Defines common recieve command types.
    /// </summary>
    public enum CommonRecieveCommandTypes : System.Byte
    {
        /// <summary>Empty dummy type.</summary>
        None = 0,
        /// <summary></summary>
        GetPlaylists,
        /// <summary></summary>
        LoadTrackOrdinal,
        /// <summary></summary>
        DestroyPlayer,
        /// <summary></summary>
        DeletePlaylist,
        /// <summary></summary>
        CloseActivePlaylist,
        /// <summary></summary>
        OpenPlaylist,
        /// <summary></summary>
        OpenPlaylistUseLastTrack,
        /// <summary></summary>
        ReloadOpenedPlaylist,
        /// <summary></summary>
        SaveOpenedPlaylist,
        /// <summary>Indicates that an unexpected failure has occured that should had not be happening. The target must be destroyed directly and show the exception carried through the event.</summary>
        HardFailAppMustClose
    }

    /// <summary>
    /// Sent to the RCU Engine to inform it that it has recieved a valid command and that it must process the request.
    /// </summary>
    public sealed class RecieveCommand
    {
        /// <summary>
        /// Gets the specific command type.
        /// </summary>
        public CommonRecieveCommandTypes Type;
        /// <summary>
        /// Gets additional data that must be submitted with the request.
        /// </summary>
        public System.Object Data;

        /// <summary>
        /// Creates a new instance of the <see cref="RecieveCommand"/> class.
        /// </summary>
        /// <param name="type">The common type of this command.</param>
        public RecieveCommand(CommonRecieveCommandTypes type) => Type = type;
    }
}