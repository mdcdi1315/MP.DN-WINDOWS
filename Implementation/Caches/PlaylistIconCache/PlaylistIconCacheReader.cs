

using System.Collections.Generic;

namespace MP.Caches.PlaylistIconCache
{
    public sealed class IconCacheReader : CacheReader
    {
        public IconCacheReader(System.IO.Stream stream)
        {
            Initialize(stream , nameof(stream));
            if (UniqueTypeIdentifier != 4992314134521007184) {
                throw new CacheFormatInvalidException("This stream is a valid cache stream , but not an Icon Cache stream.");
            }
        }

        public IEnumerable<PlaylistIconCacheItem> CachedItems
        {
            get {
                Reader.Seek(BaseOffsetToData, System.IO.SeekOrigin.Begin);
                System.UInt16 elementcount = Reader.ReadUInt16();
                System.String pln, largeicon;
                for (System.Int32 I = 0; I < elementcount; I++)
                {
                    System.UInt16 plnanelen = Reader.ReadUInt16();
                    pln = Reader.ReadASCIIString(plnanelen);
                    System.UInt16 iconchlen = Reader.ReadUInt16();
                    largeicon = Reader.ReadASCIIString(iconchlen);
                    iconchlen = Reader.ReadUInt16();
                    yield return new() { SmallIconFileName = Reader.ReadASCIIString(iconchlen), LargeIconFileName = largeicon, ReferencingPlaylistName = pln };
                }
            }
        }

        public System.Int32 Count
        {
            get {
                Reader.Seek(BaseOffsetToData, System.IO.SeekOrigin.Begin);
                return Reader.ReadUInt16();
            }
        }
    }
}