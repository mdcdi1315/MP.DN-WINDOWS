

namespace MP.BinaryPlaylist
{
    /// <summary>
    /// Contains a handful of reserved blob types that could exist across all .MPBPL playlists.
    /// </summary>
    public enum BlobTypes : System.Byte
    {
        /// <summary>
        /// Contains a list of structs that do contain the playlist's tracks.
        /// </summary>
        TRACKBLOB,
        /// <summary>
        /// Contains a list of the current playlist preferences.
        /// </summary>
        PREFBLOB,
        /// <summary>
        /// Contains a list of strings that are referenced by all the containing blobs. <br />
        /// Each string in here is being referenced by the number of string that was read.
        /// </summary>
        STRINGBLOB,
        /// <summary>
        /// It is the same as the string blob , but it contains any data that can be expressed in a byte array , even strings. <br />
        /// For string readiness and memory optimization though it is recommended to save strings in the string blob if possible.
        /// </summary>
        DATABLOB,
        /// <summary>
        /// It is the same as the data blob, but it contains serialized structures written in bytes. <br />
        /// Currently not used by any means.
        /// </summary>
        STRUCTBLOB,
        /// <summary>
        /// Contains a list of structs that do contain tag data for each single playlist track 
        /// that has an audio tag available.
        /// </summary>
        TAGBLOB,
        /// <summary>
        /// Contains metadata information regarding the playlist state left by the Music Player , 
        /// such as the last track played , which and how many track tags have been determined, statistics and many more!
        /// </summary>
        METADATABLOB,
        /// <summary>
        /// The last value before all the custom blob types are defined. 
        /// Types above this value are considered custom and must have the Custom flag
        /// defined in their headers.
        /// </summary>
        MAXEMBEDDEDBLOBVAL
    }
}