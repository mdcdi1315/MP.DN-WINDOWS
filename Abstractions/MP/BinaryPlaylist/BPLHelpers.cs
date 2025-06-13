
namespace MP.BinaryPlaylist
{
    /// <summary>
    /// Defines common constants and helper methods for working generally <br />
    /// on Binary Playlist streams.
    /// </summary>
    public static class BinaryPlaylistHelpers
    {
        /// <summary>
        /// Defines the current version of the Music Player Binary Format. This value is constant.
        /// </summary>
        public const System.Int16 Version = 1;
        /// <summary>
        /// Defines the length of the <see cref="Identifier"/> constant. This value is constant.
        /// </summary>
        public const System.Int16 IdentifierLength = 3;
        /// <summary>
        /// Defines the identifier which is used to uniquely identify that the opened stream is a Music Player Binary Format stream.
        /// </summary>
        public const System.String Identifier = "BPL";

        /// <summary>
        /// Checks if the current stream at the current position encapsulates the Music Player Binary Format stream.
        /// </summary>
        /// <param name="stream">The stream to check.</param>
        /// <returns>A value whether the stream given is the Music Player Binary Format stream or not.</returns>
        public static System.Boolean VerifyIdentifier(System.IO.Stream stream)
        {
            System.Byte[] idch = new System.Byte[IdentifierLength];
            if (stream.Read(idch, 0, idch.Length) < IdentifierLength) { return false; }
            System.Boolean result = true;
            for (System.Int32 I = 0; I < IdentifierLength; I++)
            {
                if (idch[I] != Identifier[I].ToByte()) { result = false; break; }
            }
            return result;
        }
    }

    internal enum WriterCallState : System.Byte
    {
        Success,
        Incomplete,
        Disposed
    }
}
