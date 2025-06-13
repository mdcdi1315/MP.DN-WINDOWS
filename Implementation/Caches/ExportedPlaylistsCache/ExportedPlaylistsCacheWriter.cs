

using System;

namespace MP.Caches.ExportedPlaylistsCache
{
    /// <summary>
    /// Defines the cache writer which marks which playlists are created by exporting an archived playlist. <br />
    /// The cache is saved wherever the exported playlists folder is located to.
    /// </summary>
    public sealed class ExportedPlaylistCacheWriter : CacheWriter
    {
        public ExportedPlaylistCacheWriter(System.IO.Stream stream) : base()
        {
            UniqueTypeIdentifier = 19495785750416464;
            Version = 1;
            Initialize(stream, nameof(stream));
            WriteHeader();
        }

        public void WritePlaylist(ExportedPlaylistItem item)
        {
            ArgumentNullException.ThrowIfNullOrEmpty(item.PlaylistName, nameof(item));
            ArgumentNullException.ThrowIfNullOrEmpty(item.ReferencingFolderName, nameof(item));
            Writer.WriteUInt16(item.PlaylistName.Length.ToUInt16());
            Writer.WriteUTF16LEString(item.PlaylistName);
            Writer.WriteUInt16(item.ReferencingFolderName.Length.ToUInt16());
            Writer.WriteUTF16LEString(item.ReferencingFolderName);
        }
    }
}

