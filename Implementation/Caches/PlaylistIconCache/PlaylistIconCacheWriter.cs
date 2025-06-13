

namespace MP.Caches.PlaylistIconCache
{
    public sealed class IconCacheWriter : CacheWriter
    {
        public IconCacheWriter(System.IO.Stream stream) 
        {
            Initialize(stream, nameof(stream));
            Version = 1;
            UniqueTypeIdentifier = 4992314134521007184; // Is 'PLICACHE' in ASCII.
            WriteHeader();
        }

        public void WriteEntries(PlaylistIconCacheItem[] items)
        {
            if (items is null) {
                throw new System.ArgumentNullException(nameof(items));
            }
            if (items.Length == 0) {
                WriteEmpty();
                return;
            }
            Writer.WriteUInt16(items.Length.ToUInt16());
            foreach (var pl in items) 
            {
                // Because playlist names are file names , we can use UTF-8 encoding.
                Writer.WriteUInt16(pl.ReferencingPlaylistName.Length.ToUInt16());
                Writer.WriteASCIIString(pl.ReferencingPlaylistName);
                System.String temp = pl.LargeIconFileName;
                Writer.WriteUInt16(temp.Length.ToUInt16());
                Writer.WriteASCIIString(temp);
                temp = pl.SmallIconFileName;
                Writer.WriteUInt16(temp.Length.ToUInt16());
                Writer.WriteASCIIString(temp);
            }
        }

        public void WriteEmpty() => Writer.WriteUInt16(0);
    }
}