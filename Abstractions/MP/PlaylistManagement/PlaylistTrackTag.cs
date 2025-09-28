
using System;
using MP.Collections;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace MP.PlaylistManagement
{
    /// <summary>
    /// Defines a class that can hold audio tags for the playlist's tracks.
    /// </summary>
    public class PlaylistTrackTag : ITagReader2 , IEnumerable<KeyValuePair<PlaylistTrackTagKey , String>>
    {
        private ImageSupplier imgsupplier;
        private Dictionary<PlaylistTrackTagKey, System.String> values;

        /// <summary>
        /// Creates a new and empty instance of the <see cref="PlaylistTrackTag"/> class.
        /// The image supplier passed must be a function that does lazily load image files.
        /// </summary>
        /// <param name="supplier">The function that does lazily load image files from the playlist.</param>
        /// <exception cref="ArgumentNullException"><paramref name="supplier"/> was <see langword="null"/>.</exception>
        public PlaylistTrackTag(ImageSupplier supplier)
        {
            ArgumentNullException.ThrowIfNull(supplier);
            imgsupplier = supplier;
            values = new();
        }

        /// <summary>
        /// Creates a <see cref="PlaylistTrackTag"/> from existing data.
        /// </summary>
        /// <param name="supplier">The function that does lazily load image files from the playlist.</param>
        /// <param name="existing">The track tag data to use so that the <see cref="PlaylistTrackTag"/> instance can be initialized.</param>
        /// <returns>A new and properly initialized <see cref="PlaylistTrackTag"/> instance with the specified data.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="supplier"/> and/or <paramref name="existing"/> were <see langword="null"/>.</exception>
        public static PlaylistTrackTag Of(ImageSupplier supplier , ICollection<KeyValuePair<PlaylistTrackTagKey , String>> existing)
        {
            PlaylistTrackTag ptt = new(supplier);
            ArgumentNullException.ThrowIfNull(existing);
            ptt.values.EnsureCapacity(existing.Count);
            foreach (var kvp in existing) {
                ptt.values.Add(kvp);
            }
            return ptt;
        }

        /// <summary>
        /// Creates a new <see cref="PlaylistTrackTag"/> from the specified tag reader, plus specifying the cookie string for accessing the image data.
        /// </summary>
        /// <param name="supplier">The function that does lazily load image files from the playlist.</param>
        /// <param name="reader">The tag reader to obtain all the string values from.</param>
        /// <param name="imagestring">The image cookie string to use. Can be null or empty, indicating no image for the tag.</param>
        /// <returns>A new and initialized <see cref="PlaylistTrackTag"/> instance.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="supplier"/> and/or <paramref name="reader"/> were <see langword="null"/>.</exception>
        public static PlaylistTrackTag From(ImageSupplier supplier , ITagReader reader , [AllowNull] System.String imagestring)
        {
            PlaylistTrackTag ptt = new(supplier);
            ArgumentNullException.ThrowIfNull(reader);

            // try { ptt.values.Add(PlaylistTrackTagKey , reader); } catch { }

            if (reader is IOggTagReaderBase oggtb) {
                try { ptt.values.Add(PlaylistTrackTagKey.Vendor, oggtb.Vendor); } catch { }
            }

            if (reader is IID3V2TagReaderBase id3v2tb) {
                try { ptt.values.Add(PlaylistTrackTagKey.Lyrics, id3v2tb.Lyrics); } catch { }
            }

            if (reader is ITagReader2 tg2) {
                try { ptt.values.Add(PlaylistTrackTagKey.ImageFormat, tg2.ImageFormat); } catch { }
                try { ptt.values.Add(PlaylistTrackTagKey.Copyright, tg2.Copyright); } catch { }
                try { ptt.values.Add(PlaylistTrackTagKey.Publisher, tg2.Publisher); } catch { }
                try { ptt.values.Add(PlaylistTrackTagKey.CreationDate, tg2.CreationDate); } catch { }
            }

            try { ptt.values.Add(PlaylistTrackTagKey.Title1, reader.Title1); } catch { }
            try { ptt.values.Add(PlaylistTrackTagKey.Title2, reader.Title2); } catch { }
            try { ptt.values.Add(PlaylistTrackTagKey.SubTitle , reader.SubTitle); } catch { }
            try { ptt.values.Add(PlaylistTrackTagKey.ContributingArtists, reader.ContributingArtists); } catch { }
            try { ptt.values.Add(PlaylistTrackTagKey.Comments, reader.Comments); } catch { }
            try { ptt.values.Add(PlaylistTrackTagKey.Genre, reader.Genre); } catch { }
            try { ptt.values.Add(PlaylistTrackTagKey.AlbumArtist, reader.AlbumArtist); } catch { }
            try { ptt.values.Add(PlaylistTrackTagKey.AlbumName, reader.AlbumName); } catch { }
            try { ptt.values.Add(PlaylistTrackTagKey.EncodedBy, reader.EncodedBy); } catch { }
            try { ptt.values.Add(PlaylistTrackTagKey.DiscOrdinal, reader.DiscOrdinal); } catch { }
            try { ptt.values.Add(PlaylistTrackTagKey.TrackNumber, reader.TrackNumber); } catch { }
            try { ptt.values.Add(PlaylistTrackTagKey.PublisherURL, reader.PublisherURL); } catch { }
            try { ptt.values.Add(PlaylistTrackTagKey.WebSiteEncoderUrl, reader.WebSiteEncoderUrl); } catch { }
            
            if (!System.String.IsNullOrEmpty(imagestring)) {
                ptt.values.Add(PlaylistTrackTagKey.Image, imagestring);
            }

            return ptt;
        }

        /// <inheritdoc />
        public string ImageFormat => GetValue(PlaylistTrackTagKey.ImageFormat);

        /// <inheritdoc />
        public string Publisher => GetValue(PlaylistTrackTagKey.Publisher);

        /// <inheritdoc />
        public string Copyright => GetValue(PlaylistTrackTagKey.Copyright);

        /// <inheritdoc />
        public string CreationDate => GetValue(PlaylistTrackTagKey.CreationDate);

        /// <inheritdoc />
        public string WebSiteEncoderUrl => GetValue(PlaylistTrackTagKey.WebSiteEncoderUrl);

        /// <inheritdoc />
        public string EncodedBy => GetValue(PlaylistTrackTagKey.EncodedBy);

        /// <inheritdoc />
        public string AlbumName => GetValue(PlaylistTrackTagKey.AlbumName);

        /// <inheritdoc />
        public string Title1 => GetValue(PlaylistTrackTagKey.Title1);

        /// <inheritdoc />
        public string Title2 => GetValue(PlaylistTrackTagKey.Title2);

        /// <inheritdoc />
        public string DiscOrdinal => GetValue(PlaylistTrackTagKey.DiscOrdinal);

        /// <inheritdoc />
        public string TrackNumber => GetValue(PlaylistTrackTagKey.TrackNumber);

        /// <inheritdoc />
        public string ContributingArtists => GetValue(PlaylistTrackTagKey.ContributingArtists);

        /// <inheritdoc />
        public string AlbumArtist => GetValue(PlaylistTrackTagKey.AlbumArtist);

        /// <inheritdoc />
        public string Comments => GetValue(PlaylistTrackTagKey.Comments);

        /// <inheritdoc />
        public string SubTitle => GetValue(PlaylistTrackTagKey.SubTitle);

        /// <inheritdoc />
        public string PublisherURL => GetValue(PlaylistTrackTagKey.PublisherURL);

        /// <inheritdoc />
        public string Genre => GetValue(PlaylistTrackTagKey.Genre);

        /// <inheritdoc />
        public virtual byte[] Image
        {
            get {
                String s = GetValue(PlaylistTrackTagKey.Image);
                if (System.String.IsNullOrEmpty(s)) { return null; }
                return imgsupplier(s);
            }
        }

        /// <summary>
        /// Gets a value associated with the specified binary key.
        /// </summary>
        /// <param name="key">The key to retrieve it's value.</param>
        /// <returns>A string holding the value of the key passed in the <paramref name="key"/> parameter.</returns>
        public System.String GetValue(PlaylistTrackTagKey key)
        {
            if (values.TryGetValue(key, out var value)) {
                return value;
            }
            values.Add(key, System.String.Empty);
            return System.String.Empty;
        }

        /// <summary>
        /// Sets or updates a value associated with the specified binary key. 
        /// </summary>
        /// <param name="key">The key to set or update the value.</param>
        /// <param name="newvalue">The new value to set by <paramref name="key"/>.</param>
        /// <returns>The previous value assigned at <paramref name="key"/>, or <see langword="null"/> if no value was set before.</returns>
        [return: MaybeNull]
        public System.String SetValue(PlaylistTrackTagKey key , [AllowNull] System.String newvalue)
        {
            System.String old;
            values.TryGetValue(key, out old);
            values[key] = newvalue ?? System.String.Empty;
            return old;
        }

        /// <summary>
        /// Gets the number of properties contained in the current playlist track tag.
        /// </summary>
        public int PropertyCount => values.Count;

        /// <summary>
        /// Gets an enumerator returning all the properties contained in the current playlist track tag.
        /// </summary>
        /// <returns>An enumerator returning all the contained tag properties.</returns>
        public IEnumerator<KeyValuePair<PlaylistTrackTagKey, System.String>> GetEnumerator() => values.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        /// <summary>
        /// Disposes this <see cref="PlaylistTrackTag"/> instance. <br />
        /// Note that this method is not used by the <see cref="TagsManager"/> class,
        /// since there are no meaningful resources to be released. <br />
        /// This is only for supporting the <see cref="ITagReader"/> infrastracture.
        /// </summary>
        public void Dispose()
        {
            values = null;
            imgsupplier = null;
            GC.SuppressFinalize(this);
        }
    }
}