

using System;
using MP.Caches;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace MP.PlaylistManagement.Caching
{
    /// <summary>
    /// Defines a cache reader for associating disk files with cookies.
    /// </summary>
    public sealed class TagsManagerImageCacheReader : CacheReader
    {
        /// <summary>
        /// Creates a new instance of the <see cref="TagsManagerImageCacheReader"/> class.
        /// </summary>
        /// <param name="stream">The stream to read cache entries from.</param>
        /// <exception cref="CacheFormatInvalidException">The cache was not a valid Tags Manager Image Cache stream.</exception>
        public TagsManagerImageCacheReader(System.IO.Stream stream)
        {
            Initialize(stream, nameof(stream));
            if (UniqueTypeIdentifier != TagsManagerImageCache.TAGS_MANAGER_IMAGE_CACHE_IDENTIFIER) {
                throw new CacheFormatInvalidException("This stream is a valid cache stream , but not a Tags Manager Image Cache stream.");
            }
        }

        /// <summary>
        /// Enumerates through all the entries found in the cache. <br />
        /// The entries are key-valued pairs that contain the cookie first, then the associated file name.
        /// </summary>
        /// <returns>An enumerable providing the cache entries to read.</returns>
        public IEnumerable<KeyValuePair<String , String>> GetEntries()
        {
            if (Reader.CanSeek) { Reader.Seek(BaseOffsetToData, System.IO.SeekOrigin.Begin); }
            int ssize = Unsafe.SizeOf<TAGSCACHESSTATICENTRYV1>();
            int read = 0;
            TAGSCACHESSTATICENTRYV1 entry;
            System.Byte[] temp = new System.Byte[ssize];
            while (true)
            {
                read = Reader.Read(temp, 0, ssize);
                if (read == ssize) {
                    entry = temp.ReadStructure<TAGSCACHESSTATICENTRYV1>(0);
                    yield return new(Reader.ReadUTF16LEString(entry.CookieCharLength * sizeof(System.Char)) , Reader.ReadUTF16LEString(entry.FileNameLength * sizeof(System.Char)));
                } else {
                    break;
                }
            }
        }
    }
}