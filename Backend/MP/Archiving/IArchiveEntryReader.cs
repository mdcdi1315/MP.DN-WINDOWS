using System;

namespace MP.Archiving
{
    /// <summary>
    /// Defines a single archive entry reader. <br />
    /// Archive readers should return such classes that implement this interface. <br />
    /// The client may request multiple entries and additionally could read these entries concurrently.
    /// </summary>
    public interface IArchiveEntryReader : IDisposable
    {
        /// <summary>
        /// The entry that is currently being read from the archive.
        /// </summary>
        public ArchiveEntry Entry { get; }

        /// <summary>
        /// An offset stream inside the archive stream to read file data from. <br />
        /// Should return <see langword="null"/> when this entry reader represents a directory or an empty file.
        /// </summary>
        public System.IO.Stream EntryStream { get; }
    }
}