


using System;

namespace MP.PlaylistManagement
{
    /// <summary>
    /// Defines an abstract class for working with playlist files. <br />
    /// This class provides the means and the source of any valid playlist item for playback.
    /// </summary>
    public abstract class PlaylistFile : IEquatable<PlaylistFile>
    {
        /// <summary>
        /// The absolute path of the file, either this is into an archived playlist, or is a physical file, or a downloaded file.
        /// </summary>
        public abstract System.String FullName { get; }

        /// <summary>
        /// The name of the file, including it's extension.
        /// </summary>
        public abstract System.String Name { get; }

        /// <summary>
        /// The extension of the file, including the dot. <br />
        /// An empty string is returned if the file's name does not have an extension. <br />
        /// By default an implementation for this is provided based on the <see cref="Name"/> property, but if required it can be overriden by extending classes.
        /// </summary>
        public virtual System.String Extension
        {
            get {
                string n = Name;
                int index = n.LastIndexOf('.');
                if (index == -1) { 
                    return System.String.Empty; 
                } else {
                    return n.Substring(index);
                }
            }
        }

        /// <summary>
        /// Gets an estimate when this file was created in UTC time.
        /// </summary>
        public abstract System.DateTime CreationTimeUtc { get; }

        /// <summary>
        /// Gets an estimate when this file was lastly modified in UTC time.
        /// </summary>
        public abstract System.DateTime LastModificationTimeUtc { get; }

        /// <summary>
        /// Gets an estimate when this file was lastly written in UTC time.
        /// </summary>
        public abstract System.DateTime LastWriteTimeUtc { get; }

        /// <summary>
        /// Gets the file's length in bytes.
        /// </summary>
        public abstract System.Int64 Length { get; }

        /// <summary>
        /// Gets a value whether the file does actually exist.
        /// </summary>
        public abstract System.Boolean Exists { get; }

        /// <summary>
        /// Opens a stream to the file to read data from.
        /// </summary>
        public abstract AbstractPropertyStream GetStream();

        /// <summary>
        /// Specifies how two <see cref="PlaylistFile"/> instances are equal. <br />
        /// This is mostly used by <see cref="PlaylistFileCollection"/> class to identify whether two playlist files do represent the same file.
        /// </summary>
        /// <param name="obj">The other object to compare, which must at least be a <see cref="PlaylistFile"/> instance.</param>
        /// <returns>A value whether this and the specified object are equal.</returns>
        public override bool Equals(object obj) => (obj is PlaylistFile pf) && pf.FullName.Equals(FullName , StringComparison.OrdinalIgnoreCase);

        /// <summary>
        /// Specifies how two <see cref="PlaylistFile"/> instances are equal. <br />
        /// This is mostly used by <see cref="PlaylistFileCollection"/> class to identify whether two playlist files do represent the same file.
        /// </summary>
        /// <param name="other">The other object to compare, which must at least be a <see cref="PlaylistFile"/> instance.</param>
        /// <returns>A value whether this and the specified object are equal.</returns>
        public virtual bool Equals(PlaylistFile other) => other is not null && other.FullName.Equals(FullName, StringComparison.OrdinalIgnoreCase);

        /// <summary>
        /// Gets a hash code for this <see cref="PlaylistFile"/> instance.
        /// </summary>
        /// <returns>A signed 32-bit integer containing a hash code for this object.</returns>
        public override int GetHashCode() => FullName.GetHashCode();
    }

}