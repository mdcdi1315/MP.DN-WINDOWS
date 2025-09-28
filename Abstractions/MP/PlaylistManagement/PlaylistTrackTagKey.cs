

namespace MP.PlaylistManagement
{
    /// <summary>
    /// Denotes the serialized track tag key to use. Used by the tag serialization services.
    /// </summary>
    public enum PlaylistTrackTagKey : System.UInt16
    {
        /// <summary>The key is undefined. This is a reserved and well-known value.</summary>
        None = 0,
        /// <summary>
        /// Defines the <see cref="ITagReader.Title1"/> property.
        /// </summary>
        Title1,
        /// <summary>
        /// Defines the <see cref="ITagReader.Title2"/> property.
        /// </summary>
        Title2,
        /// <summary>
        /// Defines the <see cref="ITagReader.SubTitle"/> property.
        /// </summary>
        SubTitle,
        /// <summary>
        /// Defines the <see cref="ITagReader.ContributingArtists"/> property.
        /// </summary>
        ContributingArtists,
        /// <summary>
        /// Defines the <see cref="ITagReader.AlbumName"/> property.
        /// </summary>
        AlbumName,
        /// <summary>
        /// Defines the <see cref="ITagReader.AlbumArtist"/> property.
        /// </summary>
        AlbumArtist,
        /// <summary>
        /// Defines the <see cref="ITagReader.Comments"/> property.
        /// </summary>
        Comments,
        /// <summary>
        /// Defines the <see cref="ITagReader.Genre"/> property.
        /// </summary>
        Genre,
        /// <summary>
        /// Defines the <see cref="ITagReader.EncodedBy"/> property.
        /// </summary>
        EncodedBy,
        /// <summary>
        /// Defines the <see cref="ITagReader.WebSiteEncoderUrl"/> property.
        /// </summary>
        WebSiteEncoderUrl,
        /// <summary>
        /// Defines the <see cref="ITagReader.PublisherURL"/> property.
        /// </summary>
        PublisherURL,
        /// <summary>
        /// Defines the <see cref="ITagReader.TrackNumber"/> property.
        /// </summary>
        TrackNumber,
        /// <summary>
        /// Defines the <see cref="ITagReader.DiscOrdinal"/> property.
        /// </summary>
        DiscOrdinal,
        /// <summary>
        /// Defines the <see cref="ITagReader2.Copyright"/> property.
        /// </summary>
        Copyright,
        /// <summary>
        /// Defines the <see cref="ITagReader2.CreationDate"/> property.
        /// </summary>
        CreationDate,
        /// <summary>
        /// Defines the <see cref="ITagReader2.Publisher"/> property.
        /// </summary>
        Publisher,
        /// <summary>
        /// Defines the <see cref="ITagReader2.ImageFormat"/> property.
        /// </summary>
        ImageFormat,
        /// <summary>
        /// Defines the <see cref="ITagReader.Image"/> property.
        /// </summary>
        Image,
        /// <summary>
        /// Defines the <see cref="IOggTagReaderBase.Vendor"/> property.
        /// </summary>
        Vendor,
        /// <summary>
        /// Defines the <see cref="IID3V2TagReaderBase.Lyrics"/> property.
        /// </summary>
        Lyrics,
        /// <summary>
        /// Defines the currently last value existing as a binary key. The value associated with this enumeration case can change in subsequent releases.
        /// </summary>
        LastDefinedValue = Lyrics,

        /// <summary>
        /// Defines a special constant indicating that values including it are user-defined tag value keys. <br />
        /// It is encouraged to provide your own tag value keys if you create a reader that defines additional tag properties.
        /// </summary>
        StartOfUserDefinedKeys = 8000
    }
}