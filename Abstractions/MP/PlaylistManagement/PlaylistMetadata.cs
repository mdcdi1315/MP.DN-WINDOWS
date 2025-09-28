
using System;
using System.Collections;
using System.Collections.Generic;
using MP.Annotations.CodeAnalysis;
using System.Diagnostics.CodeAnalysis;

namespace MP.PlaylistManagement
{
    /// <summary>
    /// Defines a collection of playlist metadata, that are individual values storing state for the playlist. <br />
    /// The playlist metadata items are retrieved using ordinal ignore-case rules (see <see cref="StringComparer.OrdinalIgnoreCase"/> for more information) <br /> <br />
    /// 
    /// Modifying values in the metadata <br />
    /// Modifying values in <see cref="PlaylistMetadata"/> is done in a straightforward and easy way. <br />
    /// Via simply getting the metadata item, you can directly modify the reference. <br />
    /// The changes will be elsewise reflected to the internal data structures of the playlist metadata,
    /// because the <see cref="PlaylistMetadataValue"/> is a class and the <see cref="PlaylistMetadataValue.Value"/> 
    /// property is passed by reference to you.
    /// </summary>
    public sealed class PlaylistMetadata : IEnumerable<KeyValuePair<String , PlaylistMetadataValue>>
    {
        private readonly Dictionary<String, PlaylistMetadataValue> metadata;

        /// <summary>
        /// Constructs a new and empty instance of the <see cref="PlaylistMetadata"/> class.
        /// </summary>
        public PlaylistMetadata() => metadata = new(StringComparer.OrdinalIgnoreCase);

        /// <summary>
        /// Constructs a new instance of the <see cref="PlaylistMetadata"/> class, along with the specified array of metadata to initialize this instance.
        /// </summary>
        /// <param name="metadata">The metadata to initialize this instance from.</param>
        /// <exception cref="ArgumentNullException"><paramref name="metadata"/> was <see langword="null"/>.</exception>
        public PlaylistMetadata(KeyValuePair<String, PlaylistMetadataValue>[] metadata) => this.metadata = new(metadata , StringComparer.OrdinalIgnoreCase);

        /// <summary>
        /// Constructs a new instance of the <see cref="PlaylistMetadata"/> class, along with the specified metadata enumerable to initialize this instance.
        /// </summary>
        /// <param name="metadata">The metadata enumerable to initialize this instance from.</param>
        /// <exception cref="ArgumentNullException"><paramref name="metadata"/> was <see langword="null"/>.</exception>
        public PlaylistMetadata(IEnumerable<KeyValuePair<String, PlaylistMetadataValue>> metadata) => this.metadata = new(metadata , StringComparer.OrdinalIgnoreCase);

        /// <summary>
        /// Gets the specified playlist metadata value. <br />
        /// If not found it fails with <see cref="KeyNotFoundException"/>.
        /// </summary>
        /// <param name="key">The key that is associated with the metadata value which you wish to be retrieved.</param>
        /// <returns>The metadata value instance.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="key"/> was <see langword="null"/>.</exception>
        /// <exception cref="KeyNotFoundException"><paramref name="key"/> was not found in the playlist metadata.</exception>
        [Throws(typeof(ArgumentNullException) , typeof(KeyNotFoundException))]
        public PlaylistMetadataValue Get(String key) => metadata[key];

        /// <summary>
        /// Gets or creates the specified playlist metadata value. <br />
        /// If the specified key was not found, it is created.
        /// </summary>
        /// <param name="key">The key that is associated with the metadata value which you wish to be retrieved.</param>
        /// <returns>The metadata value instance.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="key"/> was <see langword="null"/>.</exception>
        [Throws(typeof(ArgumentNullException))]
        public PlaylistMetadataValue GetOrCreate(String key)
        {
            if (metadata.TryGetValue(key, out var value)) {
                return value;
            }
            value = new();
            metadata.Add(key, value);
            return value;
        }

        /// <summary>
        /// Adds the specified metadata to this instance. <br />
        /// If the specified metadata name is already existing, it is overwritten.
        /// </summary>
        /// <param name="key">The name of the metadata to add.</param>
        /// <param name="value">The value that will be associated with <paramref name="key"/>.</param>
        /// <exception cref="ArgumentNullException"><paramref name="key"/> and/or <paramref name="value"/> were <see langword="null"/>.</exception>
        public void Add(String key , PlaylistMetadataValue value)
        {
            ArgumentNullException.ThrowIfNull(value);
            metadata[key] = value;
        }

        /// <summary>
        /// Removes the specified metadata from this object, returning the value before removal.
        /// </summary>
        /// <param name="key">The key of the metadata item you wish to be removed.</param>
        /// <param name="value">The value of the metadata item removed</param>
        /// <returns>A value whether the specified metadata was successfully removed from this object.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="key"/> was <see langword="null"/>.</exception>
        public System.Boolean Remove(String key , [NotNullWhen(true)] out PlaylistMetadataValue value) => metadata.Remove(key , out value);

        /// <summary>
        /// Removes the specified metadata from this object.
        /// </summary>
        /// <param name="key">The key of the metadata item you wish to be removed.</param>
        /// <returns>A value whether the specified metadata was successfully removed from this object.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="key"/> was <see langword="null"/>.</exception>
        public System.Boolean Remove(String key) => metadata.Remove(key);

        /// <summary>
        /// Gets the number of the contained metadata items in this object.
        /// </summary>
        public int MetadataCount => metadata.Count;

        /// <summary>
        /// Gets an enumerator able to return all the metadata items contained in this object.
        /// </summary>
        /// <returns>An enumerator instance returning all the metadata items as key-valued pairs.</returns>
        public IEnumerator<KeyValuePair<String, PlaylistMetadataValue>> GetEnumerator() => metadata.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}