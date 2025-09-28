
using System;
using MP.Caches;
using System.Collections.Generic;

namespace MP.PlaylistManagement.Caching
{
    /// <summary>
    /// Defines a cache writer for associating disk files with cookies.
    /// </summary>
	public sealed class TagsManagerImageCacheWriter : CacheWriter
    {
        /// <summary>
        /// Creates a new instance of the <see cref="TagsManagerImageCacheWriter"/> class.
        /// </summary>
        /// <param name="stream">The stream to write all the cache data to.</param>
        public TagsManagerImageCacheWriter(System.IO.Stream stream)
        {
            UniqueTypeIdentifier = TagsManagerImageCache.TAGS_MANAGER_IMAGE_CACHE_IDENTIFIER;
            Version = 1;
            Initialize(stream, nameof(stream));
            WriteHeader();
        }

        /// <summary>
        /// Writes a cache entry. <br />
        /// The key of the pair represents the active cookie used, while the entry value maps to the actual file name to retrieve.
        /// </summary>
        /// <param name="entry">The cache entry to write.</param>
        /// <exception cref="ArgumentException">The entry was invalid (one of the strings were <see langword="null"/> or were out of storage bounds).</exception>
        public void WriteEntry(KeyValuePair<String , String> entry)
        {
            String cookie = entry.Key;
            if (cookie is null || cookie.Length > 255) {
                throw new ArgumentException("Cookie string must not be null and it's length cannot exceed the 255 characters.");
            }
            String filename = entry.Value;
            if (filename is null || filename.Length > 65535) {
                throw new ArgumentException("The associated file name string must not be null and it's length cannot exceed the 65535 characters.");
            }
            Writer.WriteStructure(new TAGSCACHESSTATICENTRYV1() {
                Reserved = 0,
                CookieCharLength = cookie.Length.ToByte(),
                FileNameLength = filename.Length.ToUInt16()
            });
            Writer.WriteUTF16LEString(cookie);
            Writer.WriteUTF16LEString(filename);
        }

        /// <summary>
        /// Writes all the cache entries contained in the specified enumerable.
        /// </summary>
        /// <param name="entries">The cache entries to write. All the entries will be written.</param>
        /// <exception cref="ArgumentNullException"><paramref name="entries"/> was <see langword="null"/>.</exception>
        /// <exception cref="ArgumentException">One of the entries was invalid (one of the strings were <see langword="null"/> or were out of storage bounds).</exception>
        public void WriteEntries(IEnumerable<KeyValuePair<String , String>> entries)
        {
            ArgumentNullException.ThrowIfNull(entries);
            foreach (var entry in entries) { WriteEntry(entry); }
        }
    }
}