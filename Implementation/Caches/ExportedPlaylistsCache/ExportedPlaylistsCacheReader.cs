

using System.Collections.Generic;

namespace MP.Caches.ExportedPlaylistsCache
{
    /// <summary>
    /// Defines the reader counterpart of the <see cref="ExportedPlaylistCacheWriter"/> class. <br />
    /// The class is not inheritable.
    /// </summary>
    public sealed class ExportedPlaylistsCacheReader : CacheReader
    {
        public ExportedPlaylistsCacheReader(System.IO.Stream stream) 
        {
            Initialize(stream , nameof(stream));
            if (UniqueTypeIdentifier != 19495785750416464) {
                throw new CacheFormatInvalidException("Expected to find the Exported Playlists cache format , but returned an invalid header.");
            }
        }

        public IEnumerable<ExportedPlaylistItem> Playlists
        {
            get {
                Reader.Seek(BaseOffsetToData, System.IO.SeekOrigin.Begin);
                System.UInt16 length;
                System.String name;
                while (Reader.Position < Reader.Length)
                {
                    length = Reader.ReadUInt16();
                    name = Reader.ReadUTF16LEString(length * sizeof(System.Char));
                    length = Reader.ReadUInt16();
                    yield return new() { PlaylistName = name , ReferencingFolderName = Reader.ReadUTF16LEString(length * sizeof(System.Char)) };
                }
            }
        }

        public ExportedPlaylistItem[] GetPlaylists()
        {
            Reader.Seek(BaseOffsetToData, System.IO.SeekOrigin.Begin);
            List<ExportedPlaylistItem> strings = new(20);
            System.UInt16 length;
            System.String name;
            while (Reader.Position < Reader.Length)
            {
                length = Reader.ReadUInt16();
                name = Reader.ReadUTF16LEString(length * sizeof(System.Char));
                length = Reader.ReadUInt16();
                strings.Add(new() { PlaylistName = name, ReferencingFolderName = Reader.ReadUTF16LEString(length * sizeof(System.Char)) });
            }
            return strings.ToArray();
        }
    }
}