
using System;
using MP.Caches;
using System.Collections.Generic;
using MP.Annotations.CodeAnalysis;

namespace MP.PlaylistManagement.Caching
{
    /// <summary>
    /// Provides a cache for storing track cover images to a disk,
    /// because loading them into memory can be expensive.
    /// </summary>
    public abstract class TagsManagerImageCache : CacheInstance<TagsManagerImageCacheWriter , TagsManagerImageCacheReader>
    {
        /// <summary>
        /// Gets the unique identifier for identifying tags manager image cache index files.
        /// </summary>
        public const System.Int64 TAGS_MANAGER_IMAGE_CACHE_IDENTIFIER = 73956339241037;

        /// <summary>
        /// Holds the cache mappings currently defined as part of this cache instance. <br />
        /// Do not attempt to find and use the internal representation of this object, as it is subject to change in subsequent revisions of this class. <br />
        /// The only guaranteed members that this field does contain are those provided by the <see cref="IDictionary{TKey, TValue}"/> interface.
        /// </summary>
        protected readonly IDictionary<String, String> Mappings;

        /// <summary>
        /// Creates a new and empty instance of the <see cref="TagsManagerImageCache"/> class.
        /// </summary>
        public TagsManagerImageCache() {
            Mappings = new Dictionary<String, String>(10);
        }

        /// <summary>
        /// Gets a function that can load images. <br />
        /// You use this to create <see cref="PlaylistTrackTag"/> instances appropriately.
        /// </summary>
        public abstract ImageSupplier ImageLoader { get; }

        /// <summary>
        /// Marks the specified image cache entry as invalid. <br />
        /// The cookie passed here must be the value obtained from <see cref="PlaylistTrackTag.GetValue(PlaylistTrackTagKey)" /> with a parameter of <see cref="PlaylistTrackTagKey.Image"/>.
        /// </summary>
        /// <param name="encodedimagecookie">The cookie that denotes the cache entry to be deleted in the close future.</param>
        /// <remarks>
        /// The method must handle cases where the <paramref name="encodedimagecookie"/> is null or empty.  <br />
        /// In such cases, the method should just ignore such case and return immediately.
        /// </remarks>
        public abstract void MarkAsBroken(string encodedimagecookie);

        /// <summary>
        /// Adds a new image to be cached, and returns a cookie for it. <br />
        /// The cache will later write this entry to disk.
        /// </summary>
        /// <param name="datastream">The stream that contains the image data to flush to the cache.</param>
        /// <param name="disposestream">A value whether the passed <paramref name="datastream"/> should be disposed too when caching of it finishes.</param>
        /// <returns>A cookie for referencing this cache entry.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="datastream"/> was <see langword="null"/>.</exception>
        /// <exception cref="ArgumentException"><paramref name="datastream"/> was not a readable stream.</exception>
        [Throws(typeof(ArgumentNullException) , typeof(ArgumentException))]
        public abstract string AddEntry(System.IO.Stream datastream , bool disposestream);

        /// <summary>
        /// Adds a new image to be cached, and returns a cookie for it. <br />
        /// The cache will later write this entry to disk.
        /// </summary>
        /// <param name="rawdata">The raw image data to be written to disk.</param>
        /// <returns>A cookie for referencing this cache entry.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="rawdata"/> was <see langword="null"/>.</exception>
        [Throws(typeof(ArgumentNullException))]
        public string AddEntry(byte[] rawdata)
        {
            ArgumentNullException.ThrowIfNull(rawdata);
            return AddEntry(new IO.MemoryStream(rawdata), true);
        }

        /// <summary>
        /// Loads the cache from the specified reader instance into this instance.
        /// </summary>
        /// <param name="reader">The cache reader to use for loading the entries from the disk.</param>
        /// <exception cref="ArgumentNullException"><paramref name="reader"/> was <see langword="null"/>.</exception>
        [Throws(typeof(ArgumentNullException))]
        public sealed override void LoadCache(TagsManagerImageCacheReader reader)
        {
            ArgumentNullException.ThrowIfNull(reader);
            foreach (var entry in reader.GetEntries()) {
                Mappings.Add(entry.Key, entry.Value);
            }
        }

        /// <summary>Saves the modified cache data to the specified writer.</summary>
        /// <param name="writer">The writer to save the currently held data.</param>
        /// <exception cref="ArgumentNullException"><paramref name="writer"/> was <see langword="null"/>.</exception>
        [Throws(typeof(ArgumentNullException))]
        public sealed override void SaveCache(TagsManagerImageCacheWriter writer)
        {
            ArgumentNullException.ThrowIfNull(writer);
            writer.WriteEntries(Mappings);
        }

        /// <summary>
        /// Clears ALL the cache entries as part of this instance. <br />
        /// Extending classes are responsible for also breaking the entries before clearing them.
        /// </summary>
        public override void ClearCacheEntries() => Mappings.Clear();
    }
}