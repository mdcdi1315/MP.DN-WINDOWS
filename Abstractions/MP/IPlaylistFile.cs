

namespace MP
{
    /// <summary>
    /// Defines an abstraction for an audio file contained in a playlist.
    /// </summary>
    public interface IPlaylistFile
    {
        /// <summary>
        /// The absolute path of the file, either this is into an archived playlist, or is a physical file, or a downloaded file.
        /// </summary>
        public System.String FullName { get; }

        /// <summary>
        /// The name of the file, including it's extension.
        /// </summary>
        public System.String Name { get; }

        /// <summary>
        /// The extension of the file, including the dot.
        /// </summary>
        public System.String Extension { get; }

        /// <summary>
        /// Gets an estimate when this file was created in UTC time.
        /// </summary>
        public System.DateTime CreationTimeUtc { get; }

        /// <summary>
        /// Gets an estimate when this file was lastly modified in UTC time.
        /// </summary>
        public System.DateTime LastModificationTimeUtc { get; }

        /// <summary>
        /// Gets an estimate when this file was lastly written in UTC time.
        /// </summary>
        public System.DateTime LastWriteTimeUtc { get; }

        /// <summary>
        /// Gets the file's length in bytes.
        /// </summary>
        public System.Int64 Length { get; }

        /// <summary>
        /// Gets a value whether the file does actually exist.
        /// </summary>
        public System.Boolean Exists { get; }

        /// <summary>
        /// Opens a stream to the file to read data from.
        /// </summary>
        public AbstractPropertyStream GetStream();
    }
}